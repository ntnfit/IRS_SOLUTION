using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Mrps;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Mrps;

public class frm_InputGenerateForecast : XtraFormEx
{
	public InputGenerateForecastImpl Input;

	private IContainer components;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnGenerate;

	private LabelLine lbSelectItem;

	private LabelLine lbAdvanceSettings;

	private LabelLine lbFrom;

	private LabelLine lbItemGroup;

	private LabelLine lbWarehouse;

	private LabelLine lbTo;

	private LabelLine lbBaseOnBackward;

	private LabelLine lbBaseOnForward;

	private LabelLine lbForward;

	private LabelLine lbBackward;

	private ButtonEditEx txtFromItem;

	private ButtonEditEx txtToItem;

	private ButtonEditEx txtWarehouse;

	private CalcEditEx txtBaxkward;

	private CalcEditEx txtFordward;

	private ButtonEditEx txtItemGroup;

	private LabelLine lbPropeties;

	private ButtonEditEx txtPropeties;

	private ButtonEditEx txtBackwardBaseOn;

	private ButtonEditEx txtForwardBaseOn;

	public frm_InputGenerateForecast()
	{
		InitializeComponent();
		Input = new InputGenerateForecastImpl();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.None;
		Close();
	}

	private void txtControl_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (((dynamic)sender).Name.Equals(txtFromItem.Name) || ((dynamic)sender).Name.Equals(txtToItem.Name))
			{
				if (e.Button.Index == 0)
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, ((dynamic)sender).Text);
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItem != null)
					{
						((dynamic)sender).EditValue = frm_CFL2.ChooseItem["ItemCode"];
					}
				}
				else
				{
					string text = Function.ToString(((dynamic)sender).Text);
					if (!string.IsNullOrWhiteSpace(text))
					{
						this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text);
					}
				}
			}
			else if (((dynamic)sender).Name.Equals(txtWarehouse.Name))
			{
				frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL3.ShowDialog(this);
				if (frm_CFL3.ChooseItem != null)
				{
					((dynamic)sender).EditValue = frm_CFL3.ChooseItem["WhsCode"];
				}
			}
			else if (((dynamic)sender).Name.Equals(txtItemGroup.Name))
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ItemGroup, multiSelect: true, "");
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItems == null)
				{
					return;
				}
				string text2 = "";
				foreach (object chooseItem in frm_ChooseFromList2.ChooseItems)
				{
					ItemGroupsImpl itemGroupsImpl = (ItemGroupsImpl)(dynamic)chooseItem;
					text2 = text2 + itemGroupsImpl.Number + ",";
				}
				((dynamic)sender).EditValue = (string.IsNullOrEmpty(text2) ? text2 : text2.Substring(0, text2.Length - 1));
			}
			else
			{
				if (!(((dynamic)sender).Name.Equals(txtPropeties.Name) ? true : false))
				{
					return;
				}
				frm_InputGenerateForecastItemPropeties frm_InputGenerateForecastItemPropeties2 = new frm_InputGenerateForecastItemPropeties();
				frm_InputGenerateForecastItemPropeties2.CompareType = Input.CompareType;
				frm_InputGenerateForecastItemPropeties2.IgnorePropeties = Input.IgnorePropeties;
				frm_InputGenerateForecastItemPropeties2.Propeties = Input.Propeties;
				frm_InputGenerateForecastItemPropeties2.ShowDialog(this);
				if (frm_InputGenerateForecastItemPropeties2.DialogResult != DialogResult.OK)
				{
					return;
				}
				Input.IgnorePropeties = frm_InputGenerateForecastItemPropeties2.IgnorePropeties;
				Input.CompareType = frm_InputGenerateForecastItemPropeties2.CompareType;
				Input.Propeties = frm_InputGenerateForecastItemPropeties2.Propeties;
				if (Input.IgnorePropeties.Equals("N"))
				{
					if (string.IsNullOrEmpty(Input.Propeties))
					{
						txtPropeties.EditValue = "None of the Properties";
						return;
					}
					string[] array = Input.Propeties.Split(',');
					txtPropeties.EditValue = $"{array.Length} propeties {Input.CompareType}";
				}
				else
				{
					txtPropeties.EditValue = "Ignore";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_InputGenerateForecast_Load(object sender, EventArgs e)
	{
		try
		{
			txtPropeties.EditValue = "Ignore";
			txtBaxkward.SetFormatControl(Enums.DisplayFormatType.Integer);
			txtFordward.SetFormatControl(Enums.DisplayFormatType.Integer);
			DialogResult = DialogResult.None;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnGenerate_Click(object sender, EventArgs e)
	{
		Input.FromItem = txtFromItem.Text;
		Input.ToItem = txtToItem.Text;
		Input.ItemGroup = Function.ToString(txtItemGroup.EditValue);
		Input.WhsCode = txtWarehouse.Text;
		Input.BwLength = Function.ParseInt(txtBaxkward.Text);
		Input.FwLength = Function.ParseInt(txtFordward.Text);
		Input.BwBase = Function.ToString(txtBackwardBaseOn.EditValue);
		Input.FwBase = Function.ToString(txtForwardBaseOn.EditValue);
		DialogResult = DialogResult.OK;
		Close();
	}

	private void txtBaseOn_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ButtonEdit buttonEdit = (ButtonEdit)sender;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ListBaseOnInputForecast, multiSelect: true, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null)
			{
				string text = "";
				for (int i = 0; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					text = string.Join(",", text, Function.ToString(frm_ChooseFromList2.ChooseItems[i]["Code"]));
				}
				buttonEdit.EditValue = (string.IsNullOrEmpty(text) ? "" : text.Substring(1));
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_InputGenerateForecast));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnGenerate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbSelectItem = new Apzon.Controls.Controls.LabelLine();
		this.lbAdvanceSettings = new Apzon.Controls.Controls.LabelLine();
		this.lbFrom = new Apzon.Controls.Controls.LabelLine();
		this.lbItemGroup = new Apzon.Controls.Controls.LabelLine();
		this.lbWarehouse = new Apzon.Controls.Controls.LabelLine();
		this.lbTo = new Apzon.Controls.Controls.LabelLine();
		this.lbBaseOnBackward = new Apzon.Controls.Controls.LabelLine();
		this.lbBaseOnForward = new Apzon.Controls.Controls.LabelLine();
		this.lbForward = new Apzon.Controls.Controls.LabelLine();
		this.lbBackward = new Apzon.Controls.Controls.LabelLine();
		this.txtFromItem = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtToItem = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtWarehouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtBaxkward = new Apzon.Controls.Editor.CalcEditEx();
		this.txtFordward = new Apzon.Controls.Editor.CalcEditEx();
		this.txtItemGroup = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbPropeties = new Apzon.Controls.Controls.LabelLine();
		this.txtPropeties = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtBackwardBaseOn = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtForwardBaseOn = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtFromItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtWarehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBaxkward.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFordward.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPropeties.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBackwardBaseOn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtForwardBaseOn.Properties).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(432, 272);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 10;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnGenerate.FieldName = null;
		this.btnGenerate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnGenerate.ImageOptions.Image");
		this.btnGenerate.IsSearchControl = false;
		this.btnGenerate.Location = new System.Drawing.Point(353, 272);
		this.btnGenerate.Name = "btnGenerate";
		this.btnGenerate.Size = new System.Drawing.Size(75, 23);
		this.btnGenerate.TabIndex = 9;
		this.btnGenerate.Text = "Generate";
		this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
		this.lbSelectItem.FieldName = null;
		this.lbSelectItem.HideWhenEmpty = false;
		this.lbSelectItem.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSelectItem.Location = new System.Drawing.Point(27, 13);
		this.lbSelectItem.Name = "lbSelectItem";
		this.lbSelectItem.ShowUnderLine = true;
		this.lbSelectItem.Size = new System.Drawing.Size(80, 20);
		this.lbSelectItem.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSelectItem.TabIndex = 0;
		this.lbSelectItem.Utext = "Select Item";
		this.lbAdvanceSettings.FieldName = null;
		this.lbAdvanceSettings.HideWhenEmpty = false;
		this.lbAdvanceSettings.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbAdvanceSettings.Location = new System.Drawing.Point(27, 154);
		this.lbAdvanceSettings.Name = "lbAdvanceSettings";
		this.lbAdvanceSettings.ShowUnderLine = true;
		this.lbAdvanceSettings.Size = new System.Drawing.Size(105, 20);
		this.lbAdvanceSettings.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbAdvanceSettings.TabIndex = 0;
		this.lbAdvanceSettings.Utext = "Advance Settings";
		this.lbFrom.FieldName = null;
		this.lbFrom.HideWhenEmpty = false;
		this.lbFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFrom.Location = new System.Drawing.Point(52, 46);
		this.lbFrom.Name = "lbFrom";
		this.lbFrom.ShowUnderLine = true;
		this.lbFrom.Size = new System.Drawing.Size(80, 20);
		this.lbFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFrom.TabIndex = 0;
		this.lbFrom.Utext = "From";
		this.lbItemGroup.FieldName = null;
		this.lbItemGroup.HideWhenEmpty = false;
		this.lbItemGroup.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbItemGroup.Location = new System.Drawing.Point(52, 79);
		this.lbItemGroup.Name = "lbItemGroup";
		this.lbItemGroup.ShowUnderLine = true;
		this.lbItemGroup.Size = new System.Drawing.Size(80, 20);
		this.lbItemGroup.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbItemGroup.TabIndex = 0;
		this.lbItemGroup.Utext = "Item Group";
		this.lbWarehouse.FieldName = null;
		this.lbWarehouse.HideWhenEmpty = false;
		this.lbWarehouse.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbWarehouse.Location = new System.Drawing.Point(52, 112);
		this.lbWarehouse.Name = "lbWarehouse";
		this.lbWarehouse.ShowUnderLine = true;
		this.lbWarehouse.Size = new System.Drawing.Size(80, 20);
		this.lbWarehouse.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbWarehouse.TabIndex = 0;
		this.lbWarehouse.Utext = "Warehouse";
		this.lbTo.FieldName = null;
		this.lbTo.HideWhenEmpty = false;
		this.lbTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTo.Location = new System.Drawing.Point(297, 46);
		this.lbTo.Name = "lbTo";
		this.lbTo.ShowUnderLine = true;
		this.lbTo.Size = new System.Drawing.Size(60, 20);
		this.lbTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTo.TabIndex = 0;
		this.lbTo.Utext = "To";
		this.lbBaseOnBackward.FieldName = null;
		this.lbBaseOnBackward.HideWhenEmpty = false;
		this.lbBaseOnBackward.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBaseOnBackward.Location = new System.Drawing.Point(297, 194);
		this.lbBaseOnBackward.Name = "lbBaseOnBackward";
		this.lbBaseOnBackward.ShowUnderLine = true;
		this.lbBaseOnBackward.Size = new System.Drawing.Size(60, 20);
		this.lbBaseOnBackward.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBaseOnBackward.TabIndex = 0;
		this.lbBaseOnBackward.Utext = "Base On";
		this.lbBaseOnForward.FieldName = null;
		this.lbBaseOnForward.HideWhenEmpty = false;
		this.lbBaseOnForward.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBaseOnForward.Location = new System.Drawing.Point(297, 234);
		this.lbBaseOnForward.Name = "lbBaseOnForward";
		this.lbBaseOnForward.ShowUnderLine = true;
		this.lbBaseOnForward.Size = new System.Drawing.Size(60, 20);
		this.lbBaseOnForward.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBaseOnForward.TabIndex = 0;
		this.lbBaseOnForward.Utext = "Base On";
		this.lbForward.FieldName = null;
		this.lbForward.HideWhenEmpty = false;
		this.lbForward.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbForward.Location = new System.Drawing.Point(52, 234);
		this.lbForward.Name = "lbForward";
		this.lbForward.ShowUnderLine = true;
		this.lbForward.Size = new System.Drawing.Size(80, 20);
		this.lbForward.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbForward.TabIndex = 0;
		this.lbForward.Utext = "Forward";
		this.lbBackward.FieldName = null;
		this.lbBackward.HideWhenEmpty = false;
		this.lbBackward.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBackward.Location = new System.Drawing.Point(52, 194);
		this.lbBackward.Name = "lbBackward";
		this.lbBackward.ShowUnderLine = true;
		this.lbBackward.Size = new System.Drawing.Size(80, 20);
		this.lbBackward.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBackward.TabIndex = 0;
		this.lbBackward.Utext = "Backward";
		this.txtFromItem.FieldName = null;
		this.txtFromItem.IsSearchControl = false;
		this.txtFromItem.Location = new System.Drawing.Point(132, 43);
		this.txtFromItem.Name = "txtFromItem";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtFromItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtFromItem.Size = new System.Drawing.Size(150, 20);
		this.txtFromItem.TabIndex = 1;
		this.txtFromItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtControl_ButtonClick);
		this.txtToItem.FieldName = null;
		this.txtToItem.IsSearchControl = false;
		this.txtToItem.Location = new System.Drawing.Point(357, 43);
		this.txtToItem.Name = "txtToItem";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txtToItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtToItem.Size = new System.Drawing.Size(150, 20);
		this.txtToItem.TabIndex = 2;
		this.txtToItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtControl_ButtonClick);
		this.txtWarehouse.FieldName = null;
		this.txtWarehouse.IsSearchControl = false;
		this.txtWarehouse.Location = new System.Drawing.Point(132, 109);
		this.txtWarehouse.Name = "txtWarehouse";
		this.txtWarehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtWarehouse.Size = new System.Drawing.Size(150, 20);
		this.txtWarehouse.TabIndex = 4;
		this.txtWarehouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtControl_ButtonClick);
		this.txtBaxkward.FieldName = null;
		this.txtBaxkward.IsSearchControl = false;
		this.txtBaxkward.Location = new System.Drawing.Point(132, 191);
		this.txtBaxkward.Name = "txtBaxkward";
		this.txtBaxkward.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBaxkward.Properties.IsSearchControl = false;
		this.txtBaxkward.Size = new System.Drawing.Size(150, 20);
		this.txtBaxkward.TabIndex = 5;
		this.txtFordward.FieldName = null;
		this.txtFordward.IsSearchControl = false;
		this.txtFordward.Location = new System.Drawing.Point(132, 231);
		this.txtFordward.Name = "txtFordward";
		this.txtFordward.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFordward.Properties.IsSearchControl = false;
		this.txtFordward.Size = new System.Drawing.Size(150, 20);
		this.txtFordward.TabIndex = 7;
		this.txtItemGroup.FieldName = null;
		this.txtItemGroup.IsSearchControl = false;
		this.txtItemGroup.Location = new System.Drawing.Point(132, 76);
		this.txtItemGroup.Name = "txtItemGroup";
		this.txtItemGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtItemGroup.Properties.ReadOnly = true;
		this.txtItemGroup.Size = new System.Drawing.Size(150, 20);
		this.txtItemGroup.TabIndex = 11;
		this.txtItemGroup.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtControl_ButtonClick);
		this.lbPropeties.FieldName = null;
		this.lbPropeties.HideWhenEmpty = false;
		this.lbPropeties.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPropeties.Location = new System.Drawing.Point(297, 79);
		this.lbPropeties.Name = "lbPropeties";
		this.lbPropeties.ShowUnderLine = true;
		this.lbPropeties.Size = new System.Drawing.Size(60, 20);
		this.lbPropeties.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPropeties.TabIndex = 0;
		this.lbPropeties.Utext = "Properties";
		this.txtPropeties.FieldName = null;
		this.txtPropeties.IsSearchControl = false;
		this.txtPropeties.Location = new System.Drawing.Point(357, 76);
		this.txtPropeties.Name = "txtPropeties";
		this.txtPropeties.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtPropeties.Properties.ReadOnly = true;
		this.txtPropeties.Size = new System.Drawing.Size(150, 20);
		this.txtPropeties.TabIndex = 12;
		this.txtPropeties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtControl_ButtonClick);
		this.txtBackwardBaseOn.FieldName = null;
		this.txtBackwardBaseOn.IsSearchControl = false;
		this.txtBackwardBaseOn.Location = new System.Drawing.Point(357, 191);
		this.txtBackwardBaseOn.Name = "txtBackwardBaseOn";
		this.txtBackwardBaseOn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtBackwardBaseOn.Properties.ReadOnly = true;
		this.txtBackwardBaseOn.Size = new System.Drawing.Size(150, 20);
		this.txtBackwardBaseOn.TabIndex = 13;
		this.txtBackwardBaseOn.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBaseOn_ButtonClick);
		this.txtForwardBaseOn.FieldName = null;
		this.txtForwardBaseOn.IsSearchControl = false;
		this.txtForwardBaseOn.Location = new System.Drawing.Point(357, 231);
		this.txtForwardBaseOn.Name = "txtForwardBaseOn";
		this.txtForwardBaseOn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtForwardBaseOn.Properties.ReadOnly = true;
		this.txtForwardBaseOn.Size = new System.Drawing.Size(150, 20);
		this.txtForwardBaseOn.TabIndex = 14;
		this.txtForwardBaseOn.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBaseOn_ButtonClick);
		base.AcceptButton = this.btnGenerate;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(526, 302);
		base.Controls.Add(this.txtForwardBaseOn);
		base.Controls.Add(this.txtBackwardBaseOn);
		base.Controls.Add(this.txtPropeties);
		base.Controls.Add(this.lbPropeties);
		base.Controls.Add(this.txtItemGroup);
		base.Controls.Add(this.txtFordward);
		base.Controls.Add(this.txtBaxkward);
		base.Controls.Add(this.txtWarehouse);
		base.Controls.Add(this.txtToItem);
		base.Controls.Add(this.txtFromItem);
		base.Controls.Add(this.lbBaseOnBackward);
		base.Controls.Add(this.lbBaseOnForward);
		base.Controls.Add(this.lbForward);
		base.Controls.Add(this.lbBackward);
		base.Controls.Add(this.lbTo);
		base.Controls.Add(this.lbWarehouse);
		base.Controls.Add(this.lbItemGroup);
		base.Controls.Add(this.lbFrom);
		base.Controls.Add(this.lbAdvanceSettings);
		base.Controls.Add(this.lbSelectItem);
		base.Controls.Add(this.btnGenerate);
		base.Controls.Add(this.btnCancel);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InputGenerateForecast";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Input Generate Forecast";
		base.Load += new System.EventHandler(this.frm_InputGenerateForecast_Load);
		((System.ComponentModel.ISupportInitialize)this.txtFromItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtWarehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBaxkward.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFordward.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPropeties.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBackwardBaseOn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtForwardBaseOn.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
