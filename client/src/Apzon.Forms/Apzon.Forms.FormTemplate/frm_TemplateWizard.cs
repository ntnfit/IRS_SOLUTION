using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraWizard;

namespace Apzon.Forms.FormTemplate;

public class frm_TemplateWizard : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _absEntry;

	private DataSet _dsFormTemplate;

	private IContainer components;

	private WizardControlEx wizardControlEx1;

	private WelcomeWizardPage welcomeWizardPage1;

	private DevExpress.XtraWizard.WizardPage wizardPage_RegistrationMode;

	private CompletionWizardPage completionWizardPage1;

	private Panel panel1;

	private LabelControl labelControl1;

	private RadioGroupEx rdg_Mode;

	private GridControlEx grd_Form;

	private GridViewEx gv_Form;

	private DevExpress.XtraWizard.WizardPage wizardPage_BasicSetting;

	private Panel panel2;

	private LabelControl labelControl2;

	private LabelLine labelLine1;

	private TextEditEx txt_Caption;

	private LabelLine labelLine2;

	private CheckEditEx chk_CancelButton;

	private CheckEditEx chk_AcceptButton;

	private DevExpress.XtraWizard.WizardPage wizardPage_Container;

	private Panel panel3;

	private LabelControl labelControl3;

	private CheckEdit chk_Stack2;

	private CheckEdit chk_Stack1;

	private CheckEdit chk_Stack0;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem runDesignerToolStripMenuItem;

	private SimpleButton btn_DesignerStack;

	private SimpleButton btn_DesignerLines;

	private SimpleButton btn_DesignerHeader;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemCheckEdit rep_Checked;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private UcTemplate ucTemplate1;

	private ButtonEditCFL btn_FatherMenu;

	private TextEditEx txt_LayoutCode;

	private LabelLine labelLine3;

	private DevExpress.XtraWizard.WizardPage wizardPage1;

	private Panel panel4;

	private LabelControl labelControl4;

	private GridControl gridControl1;

	private TileView tileView1;

	private TileViewColumn colImage;

	private TileViewColumn colNotes;

	private ImageCollection imageCollection1;

	private ImageCollection imageCollection2;

	private LabelLine labelLine4;

	private LookUpEditEx look_FormType;

	private SimpleButton simpleButton1;

	public frm_TemplateWizard()
	{
		InitializeComponent();
	}

	private async void frm_TemplateWizard_Load(object sender, EventArgs e)
	{
		try
		{
			await ReloadData();
			rdg_Mode.EditValue = "A";
			btn_FatherMenu.Properties.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			btn_FatherMenu.Properties.DisplayMember = "Name";
			btn_FatherMenu.Properties.ValueMember = "Code";
			btn_FatherMenu.Properties.StoredProcedure = "Apz_Cfl_GetFatherMenu";
			btn_FatherMenu.Properties.RefreshDataSource();
			btn_FatherMenu.Properties.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL properties = btn_FatherMenu.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm._tableType = "2";
			}));
			btn_FatherMenu.ReadOnly = true;
			look_FormType.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "Default" },
				{ "1", "Input Box" },
				{ "2", "Header - Lines" },
				{ "3", "Header - Stacks" }
			});
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Image", typeof(Image));
			dataTable.Columns.Add("Notes", typeof(string));
			dataTable.Columns.Add("Index", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Image"] = Resources.designform1;
			dataRow["Notes"] = "Input Box";
			dataRow["Index"] = 1;
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Image"] = Resources.designform2;
			dataRow["Notes"] = "Header - Lines";
			dataRow["Index"] = 2;
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Image"] = Resources.designform3;
			dataRow["Notes"] = "Header - Stacks";
			dataRow["Index"] = 3;
			dataTable.Rows.Add(dataRow);
			gridControl1.DataSource = dataTable;
			gridControl1.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadData()
	{
		DataTable dataSource = await _httpClient.FormWizard.GetAll();
		grd_Form.DataSource = dataSource;
		grd_Form.RefreshDataSource();
	}

	private async void btn_DesignerHeader_Click(object sender, EventArgs e)
	{
		await ShowWizardDesigner("F", 0);
	}

	private async Task ShowWizardDesigner(string containerType, int stackgroup)
	{
		try
		{
			frm_TemplateWizardDesigner frm_TemplateWizardDesigner2 = new frm_TemplateWizardDesigner(_dsFormTemplate, containerType, stackgroup);
			frm_TemplateWizardDesigner2.ShowDialog();
			_dsFormTemplate = frm_TemplateWizardDesigner2.DsFormTemplate.Copy();
			ucTemplate1.WizardData = _dsFormTemplate;
			ucTemplate1.TemplateId = _absEntry;
			ucTemplate1.ObjectId = 0;
			ucTemplate1.ClearControl();
			await ucTemplate1.LoadLayoutControl(newRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void wizardControlEx1_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		_ = 3;
		try
		{
			if (!(e.Page.Name == "wizardPage_RegistrationMode"))
			{
				return;
			}
			string text = Function.ToString(rdg_Mode.EditValue);
			switch (text)
			{
			case "A":
			case "U":
				wizardPage_RegistrationMode.AllowNext = true;
				if (!(text == "A"))
				{
					if (text == "U")
					{
						_absEntry = Function.ParseInt(gv_Form.GetFocusedRowCellValue("AbsEntry"));
					}
				}
				else
				{
					_absEntry = 0;
				}
				_dsFormTemplate = await _httpClient.FormWizard.GetById(_absEntry);
				if (_dsFormTemplate.IsNotNull())
				{
					DataTable dataTable = _dsFormTemplate.Tables["OFWZ"];
					if (!dataTable.IsNotNull())
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Stack0"] = "Y";
						dataRow["Stack1"] = "Y";
						dataRow["Stack2"] = "Y";
						dataRow["Stack3"] = "Y";
						dataTable.Rows.Add(dataRow);
					}
					txt_Caption.EditValue = Function.ToString(dataTable.Rows[0]["Caption"]);
					txt_LayoutCode.EditValue = Function.ToString(dataTable.Rows[0]["LayoutCode"]);
					btn_FatherMenu.EditValue = Function.ToString(dataTable.Rows[0]["FatherMenu"]);
					chk_AcceptButton.EditValue = Function.ToString(dataTable.Rows[0]["AcceptButton"]);
					chk_CancelButton.EditValue = Function.ToString(dataTable.Rows[0]["CancelButton"]);
					chk_Stack0.EditValue = Function.ToString(dataTable.Rows[0]["Stack0"]);
					chk_Stack1.EditValue = Function.ToString(dataTable.Rows[0]["Stack1"]);
					chk_Stack2.EditValue = Function.ToString(dataTable.Rows[0]["Stack2"]);
					look_FormType.EditValue = Function.ToString(dataTable.Rows[0]["FormType"]);
				}
				ucTemplate1.TemplateId = _absEntry;
				ucTemplate1.ObjectId = 0;
				ucTemplate1.WizardData = _dsFormTemplate;
				ucTemplate1.ClearControl();
				await ucTemplate1.LoadLayoutControl(newRow: true);
				break;
			case "R":
				e.Handled = true;
				if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					int num = Function.ParseInt(gv_Form.GetFocusedRowCellValue("AbsEntry"));
					if (num != 0)
					{
						WinCommons._OpenProcessing();
						HttpResult httpResult = await _httpClient.FormWizard.Remove(num);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						}
						else
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							await ReloadData();
						}
						WinCommons._CloseProcessing();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					}
				}
				wizardControlEx1.SelectedPage = wizardPage_RegistrationMode;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rdg_Mode_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(rdg_Mode.EditValue);
			grd_Form.Visible = text != "A";
			wizardControlEx1.NextText = ((text == "R") ? "Finish" : "Next");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_DesignerLines_Click(object sender, EventArgs e)
	{
		try
		{
			await ShowWizardDesigner("T", 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_DesignerStack_Click(object sender, EventArgs e)
	{
		try
		{
			await ShowWizardDesigner("T", 2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void wizardControlEx1_FinishClick(object sender, CancelEventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				if (!_dsFormTemplate.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.DataIsEmpty, "");
					return;
				}
				string value = Function.ToString(txt_Caption.EditValue);
				if (string.IsNullOrWhiteSpace(value))
				{
					WinCommons.Error(Enums.MessageCode.CaptionIsMissing, "");
					return;
				}
				string value2 = Function.ToString(btn_FatherMenu.EditValue);
				if (string.IsNullOrWhiteSpace(value2))
				{
					WinCommons.Error(Enums.MessageCode.FatherMenuIsMissing, "");
					return;
				}
				_dsFormTemplate.Tables[0].Rows[0]["BaseForm"] = "W";
				_dsFormTemplate.Tables[0].Rows[0]["Caption"] = value;
				_dsFormTemplate.Tables[0].Rows[0]["FatherMenu"] = value2;
				_dsFormTemplate.Tables[0].Rows[0]["LayoutCode"] = Function.ToString(txt_LayoutCode.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["Valid"] = "Y";
				_dsFormTemplate.Tables[0].Rows[0]["AcceptButton"] = Function.ToString(chk_AcceptButton.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["CancelButton"] = Function.ToString(chk_CancelButton.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["Stack0"] = Function.ToString(chk_Stack0.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["Stack1"] = Function.ToString(chk_Stack1.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["Stack2"] = Function.ToString(chk_Stack2.EditValue);
				_dsFormTemplate.Tables[0].Rows[0]["FormType"] = Function.ToString(look_FormType.EditValue);
				WinCommons._OpenProcessing();
				HttpResult httpResult = await _httpClient.FormWizard.SaveData(_dsFormTemplate);
				WinCommons._CloseProcessing();
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					e.Cancel = true;
					wizardControlEx1.SelectedPage = wizardPage_RegistrationMode;
					await ReloadData();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void chk_Stack0_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ucTemplate1.SetStackVisibility(chk_Stack0.Checked, chk_Stack1.Checked, chk_Stack2.Checked, stack3: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Stack1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ucTemplate1.SetStackVisibility(chk_Stack0.Checked, chk_Stack1.Checked, chk_Stack2.Checked, stack3: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Stack2_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ucTemplate1.SetStackVisibility(chk_Stack0.Checked, chk_Stack1.Checked, chk_Stack2.Checked, stack3: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Caption_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txt_Caption.EditValue);
			Text = (string.IsNullOrEmpty(text) ? "Template Wizard" : ("Template Wizard - " + text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void wizardControlEx1_CancelClick(object sender, CancelEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileView1_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(tileView1.GetRowCellValue(e.Item.RowHandle, "Index"));
			look_FormType.EditValue = num.ToString();
			wizardControlEx1.SelectedPage = wizardPage_BasicSetting;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButton1_Click(object sender, EventArgs e)
	{
		await ShowWizardDesigner("F", 2);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.FormTemplate.frm_TemplateWizard));
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition obj = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.colNotes = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colImage = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.wizardControlEx1 = new Apzon.Controls.Controls.WizardControlEx();
		this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
		this.wizardPage_RegistrationMode = new DevExpress.XtraWizard.WizardPage();
		this.grd_Form = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Form = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Checked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rdg_Mode = new Apzon.Controls.Editor.RadioGroupEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
		this.wizardPage_BasicSetting = new DevExpress.XtraWizard.WizardPage();
		this.look_FormType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_LayoutCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_FatherMenu = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.chk_CancelButton = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AcceptButton = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Caption = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.panel2 = new System.Windows.Forms.Panel();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.wizardPage_Container = new DevExpress.XtraWizard.WizardPage();
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		this.ucTemplate1 = new Apzon.Forms.FormTemplate.UcTemplate();
		this.btn_DesignerStack = new DevExpress.XtraEditors.SimpleButton();
		this.btn_DesignerLines = new DevExpress.XtraEditors.SimpleButton();
		this.btn_DesignerHeader = new DevExpress.XtraEditors.SimpleButton();
		this.panel3 = new System.Windows.Forms.Panel();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.chk_Stack0 = new DevExpress.XtraEditors.CheckEdit();
		this.chk_Stack1 = new DevExpress.XtraEditors.CheckEdit();
		this.chk_Stack2 = new DevExpress.XtraEditors.CheckEdit();
		this.wizardPage1 = new DevExpress.XtraWizard.WizardPage();
		this.gridControl1 = new DevExpress.XtraGrid.GridControl();
		this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.panel4 = new System.Windows.Forms.Panel();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.runDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.imageCollection2 = new DevExpress.Utils.ImageCollection(this.components);
		((System.ComponentModel.ISupportInitialize)this.wizardControlEx1).BeginInit();
		this.wizardControlEx1.SuspendLayout();
		this.wizardPage_RegistrationMode.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Form).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Form).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Mode.Properties).BeginInit();
		this.panel1.SuspendLayout();
		this.wizardPage_BasicSetting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_FormType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LayoutCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_FatherMenu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CancelButton.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AcceptButton.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).BeginInit();
		this.panel2.SuspendLayout();
		this.wizardPage_Container.SuspendLayout();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack0.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack2.Properties).BeginInit();
		this.wizardPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		this.panel4.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.imageCollection2).BeginInit();
		base.SuspendLayout();
		this.colNotes.Caption = "Notes";
		this.colNotes.FieldName = "Notes";
		this.colNotes.Name = "colNotes";
		this.colNotes.Visible = true;
		this.colNotes.VisibleIndex = 1;
		this.colImage.Caption = "Image";
		this.colImage.FieldName = "Image";
		this.colImage.Name = "colImage";
		this.colImage.Visible = true;
		this.colImage.VisibleIndex = 0;
		this.wizardControlEx1.AllowPagePadding = false;
		this.wizardControlEx1.Controls.Add(this.welcomeWizardPage1);
		this.wizardControlEx1.Controls.Add(this.wizardPage_RegistrationMode);
		this.wizardControlEx1.Controls.Add(this.completionWizardPage1);
		this.wizardControlEx1.Controls.Add(this.wizardPage_BasicSetting);
		this.wizardControlEx1.Controls.Add(this.wizardPage_Container);
		this.wizardControlEx1.Controls.Add(this.wizardPage1);
		this.wizardControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wizardControlEx1.Name = "wizardControlEx1";
		this.wizardControlEx1.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[6] { this.welcomeWizardPage1, this.wizardPage_RegistrationMode, this.wizardPage1, this.wizardPage_BasicSetting, this.wizardPage_Container, this.completionWizardPage1 });
		this.wizardControlEx1.Size = new System.Drawing.Size(1003, 693);
		this.wizardControlEx1.TitleHeight = 0;
		this.wizardControlEx1.CancelClick += new System.ComponentModel.CancelEventHandler(this.wizardControlEx1_CancelClick);
		this.wizardControlEx1.FinishClick += new System.ComponentModel.CancelEventHandler(this.wizardControlEx1_FinishClick);
		this.wizardControlEx1.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wizardControlEx1_NextClick);
		this.welcomeWizardPage1.IntroductionText = "Use this wizard to add a new Form to Form Template collection, or to update or delete an existing Form.";
		this.welcomeWizardPage1.Name = "welcomeWizardPage1";
		this.welcomeWizardPage1.Size = new System.Drawing.Size(818, 577);
		this.wizardPage_RegistrationMode.Controls.Add(this.grd_Form);
		this.wizardPage_RegistrationMode.Controls.Add(this.rdg_Mode);
		this.wizardPage_RegistrationMode.Controls.Add(this.panel1);
		this.wizardPage_RegistrationMode.Name = "wizardPage_RegistrationMode";
		this.wizardPage_RegistrationMode.Size = new System.Drawing.Size(1003, 647);
		this.grd_Form.AllowPasteExcel = false;
		this.grd_Form.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Form.ClipboardData = null;
		this.grd_Form.Location = new System.Drawing.Point(0, 181);
		this.grd_Form.MainView = this.gv_Form;
		this.grd_Form.Name = "grd_Form";
		this.grd_Form.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Checked });
		this.grd_Form.Size = new System.Drawing.Size(1000, 463);
		this.grd_Form.TabIndex = 2;
		this.grd_Form.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Form });
		this.gv_Form.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Form.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Form.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Form.ColumnKey = null;
		this.gv_Form.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Form.DisableFormSetting = false;
		this.gv_Form.DisplayException = false;
		this.gv_Form.EditColumn = null;
		this.gv_Form.FieldCurrency = null;
		this.gv_Form.FieldRate = null;
		this.gv_Form.FormId = "oTemplateWizrd";
		this.gv_Form.GridControl = this.grd_Form;
		this.gv_Form.IgnoreFilter = false;
		this.gv_Form.IndexEditRow = -1;
		this.gv_Form.IndicatorWidth = 35;
		this.gv_Form.IsApzTable = false;
		this.gv_Form.IsInsertNewRow = false;
		this.gv_Form.IsLoadingData = false;
		this.gv_Form.ItemId = "OFORM";
		this.gv_Form.LstColumnAllowBatch = null;
		this.gv_Form.LstColumnNotAllowBatch = null;
		this.gv_Form.LstColumnNotEditable = null;
		this.gv_Form.LstColumNotSetting = null;
		this.gv_Form.Name = "gv_Form";
		this.gv_Form.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Form.OptionsPrint.AutoWidth = false;
		this.gv_Form.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Form.OptionsView.ColumnAutoWidth = false;
		this.gv_Form.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Form.OptionsView.ShowGroupPanel = false;
		this.gv_Form.UseBatchMod = false;
		this.gridColumnEx1.Caption = "AbsEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "FormId";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "FormId";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "BaseForm";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "BaseForm";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "BaseType";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "BaseType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx5.Caption = "Caption";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Caption";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 0;
		this.gridColumnEx5.Width = 270;
		this.gridColumnEx6.Caption = "FatherMenu";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "FatherMenu";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 250;
		this.gridColumnEx7.Caption = "Valid";
		this.gridColumnEx7.ColumnEdit = this.rep_Checked;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Valid";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 2;
		this.gridColumnEx7.Width = 100;
		this.rep_Checked.AutoHeight = false;
		this.rep_Checked.Name = "rep_Checked";
		this.rep_Checked.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_Checked.ValueChecked = "Y";
		this.rep_Checked.ValueGrayed = "N";
		this.rep_Checked.ValueUnchecked = "N";
		this.gridColumnEx8.Caption = "Accept Button";
		this.gridColumnEx8.ColumnEdit = this.rep_Checked;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "AcceptButton";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 3;
		this.gridColumnEx8.Width = 100;
		this.gridColumnEx9.Caption = "Cancel Button";
		this.gridColumnEx9.ColumnEdit = this.rep_Checked;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Cancel Button";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 4;
		this.gridColumnEx9.Width = 100;
		this.rdg_Mode.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.rdg_Mode.FieldName = null;
		this.rdg_Mode.IsSearchControl = false;
		this.rdg_Mode.Location = new System.Drawing.Point(110, 77);
		this.rdg_Mode.Name = "rdg_Mode";
		this.rdg_Mode.Properties.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.rdg_Mode.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_Mode.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("A", "Add a New Form"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("U", "Update an Existing Form"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("R", "Remove an existing Form")
		});
		this.rdg_Mode.Size = new System.Drawing.Size(791, 98);
		this.rdg_Mode.TabIndex = 1;
		this.rdg_Mode.SelectedIndexChanged += new System.EventHandler(this.rdg_Mode_SelectedIndexChanged);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.labelControl1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(1003, 70);
		this.panel1.TabIndex = 0;
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 13f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(22, 29);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(208, 16);
		this.labelControl1.TabIndex = 0;
		this.labelControl1.Text = "Selecting the Registration Mode";
		this.completionWizardPage1.Name = "completionWizardPage1";
		this.completionWizardPage1.ProceedText = "To save changes this wizard, click Finish";
		this.completionWizardPage1.Size = new System.Drawing.Size(818, 577);
		this.wizardPage_BasicSetting.Controls.Add(this.look_FormType);
		this.wizardPage_BasicSetting.Controls.Add(this.labelLine4);
		this.wizardPage_BasicSetting.Controls.Add(this.txt_LayoutCode);
		this.wizardPage_BasicSetting.Controls.Add(this.labelLine3);
		this.wizardPage_BasicSetting.Controls.Add(this.btn_FatherMenu);
		this.wizardPage_BasicSetting.Controls.Add(this.chk_CancelButton);
		this.wizardPage_BasicSetting.Controls.Add(this.chk_AcceptButton);
		this.wizardPage_BasicSetting.Controls.Add(this.labelLine2);
		this.wizardPage_BasicSetting.Controls.Add(this.txt_Caption);
		this.wizardPage_BasicSetting.Controls.Add(this.labelLine1);
		this.wizardPage_BasicSetting.Controls.Add(this.panel2);
		this.wizardPage_BasicSetting.Name = "wizardPage_BasicSetting";
		this.wizardPage_BasicSetting.Size = new System.Drawing.Size(1003, 647);
		this.look_FormType.FieldName = null;
		this.look_FormType.IsSearchControl = false;
		this.look_FormType.Location = new System.Drawing.Point(91, 173);
		this.look_FormType.Name = "look_FormType";
		this.look_FormType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_FormType.Properties.NullText = "";
		this.look_FormType.Size = new System.Drawing.Size(235, 20);
		this.look_FormType.TabIndex = 12;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(13, 176);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 11;
		this.labelLine4.Utext = "Form Type";
		this.txt_LayoutCode.FieldName = null;
		this.txt_LayoutCode.IsSearchControl = false;
		this.txt_LayoutCode.Location = new System.Drawing.Point(91, 147);
		this.txt_LayoutCode.Name = "txt_LayoutCode";
		this.txt_LayoutCode.Size = new System.Drawing.Size(235, 20);
		this.txt_LayoutCode.TabIndex = 10;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(13, 150);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 9;
		this.labelLine3.Utext = "Layout Code";
		this.btn_FatherMenu.FieldName = null;
		this.btn_FatherMenu.IsSearchControl = false;
		this.btn_FatherMenu.Location = new System.Drawing.Point(91, 121);
		this.btn_FatherMenu.Name = "btn_FatherMenu";
		this.btn_FatherMenu.Properties.AutoLoadDataSource = true;
		this.btn_FatherMenu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_FatherMenu.Properties.ContainerControl = null;
		this.btn_FatherMenu.Properties.DisplayMember = "";
		this.btn_FatherMenu.Properties.DisplayMember2 = "";
		this.btn_FatherMenu.Properties.LoadDataSourceWhenEnter = false;
		this.btn_FatherMenu.Properties.SeparateCharacterForMulti = ';';
		this.btn_FatherMenu.Properties.SeparateString = ", ";
		this.btn_FatherMenu.Properties.UsingAutoCompleteBox = false;
		this.btn_FatherMenu.Properties.UsingDataSource = true;
		this.btn_FatherMenu.Properties.ValidateWhenNoDataSource = false;
		this.btn_FatherMenu.Properties.ValueMember = "";
		this.btn_FatherMenu.Size = new System.Drawing.Size(235, 22);
		this.btn_FatherMenu.TabIndex = 8;
		this.chk_CancelButton.EditValue = "Y";
		this.chk_CancelButton.FieldName = null;
		this.chk_CancelButton.IsSearchControl = false;
		this.chk_CancelButton.Location = new System.Drawing.Point(484, 96);
		this.chk_CancelButton.Name = "chk_CancelButton";
		this.chk_CancelButton.Properties.Caption = "Cancel Button";
		this.chk_CancelButton.Properties.ValueChecked = "Y";
		this.chk_CancelButton.Properties.ValueGrayed = "N";
		this.chk_CancelButton.Properties.ValueUnchecked = "N";
		this.chk_CancelButton.Size = new System.Drawing.Size(100, 20);
		this.chk_CancelButton.TabIndex = 7;
		this.chk_AcceptButton.EditValue = "Y";
		this.chk_AcceptButton.FieldName = null;
		this.chk_AcceptButton.IsSearchControl = false;
		this.chk_AcceptButton.Location = new System.Drawing.Point(387, 96);
		this.chk_AcceptButton.Name = "chk_AcceptButton";
		this.chk_AcceptButton.Properties.Caption = "Accept Button";
		this.chk_AcceptButton.Properties.ValueChecked = "Y";
		this.chk_AcceptButton.Properties.ValueGrayed = "N";
		this.chk_AcceptButton.Properties.ValueUnchecked = "N";
		this.chk_AcceptButton.Size = new System.Drawing.Size(100, 20);
		this.chk_AcceptButton.TabIndex = 6;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(13, 124);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Father Menu";
		this.txt_Caption.FieldName = null;
		this.txt_Caption.IsSearchControl = false;
		this.txt_Caption.Location = new System.Drawing.Point(91, 95);
		this.txt_Caption.Name = "txt_Caption";
		this.txt_Caption.Size = new System.Drawing.Size(235, 20);
		this.txt_Caption.TabIndex = 3;
		this.txt_Caption.EditValueChanged += new System.EventHandler(this.txt_Caption_EditValueChanged);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 98);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "Caption";
		this.panel2.BackColor = System.Drawing.Color.White;
		this.panel2.Controls.Add(this.labelControl2);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel2.Location = new System.Drawing.Point(0, 0);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(1003, 70);
		this.panel2.TabIndex = 1;
		this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 13f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
		this.labelControl2.Appearance.Options.UseFont = true;
		this.labelControl2.Location = new System.Drawing.Point(22, 29);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(157, 16);
		this.labelControl2.TabIndex = 0;
		this.labelControl2.Text = "Modifying Basic Settings";
		this.wizardPage_Container.Controls.Add(this.simpleButton1);
		this.wizardPage_Container.Controls.Add(this.ucTemplate1);
		this.wizardPage_Container.Controls.Add(this.btn_DesignerStack);
		this.wizardPage_Container.Controls.Add(this.btn_DesignerLines);
		this.wizardPage_Container.Controls.Add(this.btn_DesignerHeader);
		this.wizardPage_Container.Controls.Add(this.panel3);
		this.wizardPage_Container.Name = "wizardPage_Container";
		this.wizardPage_Container.Size = new System.Drawing.Size(1003, 647);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.simpleButton1.Location = new System.Drawing.Point(12, 587);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(124, 23);
		this.simpleButton1.TabIndex = 11;
		this.simpleButton1.Text = "Designer - Header Stack";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.ucTemplate1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucTemplate1.Appearance.BackColor = System.Drawing.Color.Linen;
		this.ucTemplate1.Appearance.Options.UseBackColor = true;
		this.ucTemplate1.Attachments = null;
		this.ucTemplate1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ucTemplate1.FormId = "oTemplateWizrd";
		this.ucTemplate1.HasLeft = false;
		this.ucTemplate1.HasRight = false;
		this.ucTemplate1.Location = new System.Drawing.Point(142, 54);
		this.ucTemplate1.Name = "ucTemplate1";
		this.ucTemplate1.Size = new System.Drawing.Size(858, 590);
		this.ucTemplate1.SpliterPosition = 0;
		this.ucTemplate1.TabIndex = 10;
		this.ucTemplate1.TemplateId = 0;
		this.btn_DesignerStack.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_DesignerStack.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_DesignerStack.Location = new System.Drawing.Point(12, 616);
		this.btn_DesignerStack.Name = "btn_DesignerStack";
		this.btn_DesignerStack.Size = new System.Drawing.Size(124, 23);
		this.btn_DesignerStack.TabIndex = 9;
		this.btn_DesignerStack.Text = "Run Designer - Stack";
		this.btn_DesignerStack.Click += new System.EventHandler(this.btn_DesignerStack_Click);
		this.btn_DesignerLines.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_DesignerLines.Location = new System.Drawing.Point(12, 243);
		this.btn_DesignerLines.Name = "btn_DesignerLines";
		this.btn_DesignerLines.Size = new System.Drawing.Size(124, 23);
		this.btn_DesignerLines.TabIndex = 8;
		this.btn_DesignerLines.Text = "Run Designer - Lines";
		this.btn_DesignerLines.Click += new System.EventHandler(this.btn_DesignerLines_Click);
		this.btn_DesignerHeader.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_DesignerHeader.Location = new System.Drawing.Point(12, 85);
		this.btn_DesignerHeader.Name = "btn_DesignerHeader";
		this.btn_DesignerHeader.Size = new System.Drawing.Size(124, 23);
		this.btn_DesignerHeader.TabIndex = 0;
		this.btn_DesignerHeader.Text = "Run Designer -Header";
		this.btn_DesignerHeader.Click += new System.EventHandler(this.btn_DesignerHeader_Click);
		this.panel3.BackColor = System.Drawing.Color.White;
		this.panel3.Controls.Add(this.labelControl3);
		this.panel3.Controls.Add(this.chk_Stack0);
		this.panel3.Controls.Add(this.chk_Stack1);
		this.panel3.Controls.Add(this.chk_Stack2);
		this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel3.Location = new System.Drawing.Point(0, 0);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(1003, 50);
		this.panel3.TabIndex = 2;
		this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 13f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
		this.labelControl3.Appearance.Options.UseFont = true;
		this.labelControl3.Location = new System.Drawing.Point(22, 10);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(57, 16);
		this.labelControl3.TabIndex = 0;
		this.labelControl3.Text = "Designer";
		this.chk_Stack0.EditValue = "Y";
		this.chk_Stack0.Location = new System.Drawing.Point(153, 9);
		this.chk_Stack0.Name = "chk_Stack0";
		this.chk_Stack0.Properties.Caption = "Header";
		this.chk_Stack0.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Stack0.Properties.ValueChecked = "Y";
		this.chk_Stack0.Properties.ValueGrayed = "N";
		this.chk_Stack0.Properties.ValueUnchecked = "N";
		this.chk_Stack0.Size = new System.Drawing.Size(75, 20);
		this.chk_Stack0.TabIndex = 4;
		this.chk_Stack0.CheckedChanged += new System.EventHandler(this.chk_Stack0_CheckedChanged);
		this.chk_Stack1.EditValue = "Y";
		this.chk_Stack1.Location = new System.Drawing.Point(234, 9);
		this.chk_Stack1.Name = "chk_Stack1";
		this.chk_Stack1.Properties.Caption = "Lines";
		this.chk_Stack1.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Stack1.Properties.ValueChecked = "Y";
		this.chk_Stack1.Properties.ValueGrayed = "N";
		this.chk_Stack1.Properties.ValueUnchecked = "N";
		this.chk_Stack1.Size = new System.Drawing.Size(75, 20);
		this.chk_Stack1.TabIndex = 5;
		this.chk_Stack1.CheckedChanged += new System.EventHandler(this.chk_Stack1_CheckedChanged);
		this.chk_Stack2.EditValue = "Y";
		this.chk_Stack2.Location = new System.Drawing.Point(315, 9);
		this.chk_Stack2.Name = "chk_Stack2";
		this.chk_Stack2.Properties.Caption = "Stack Group";
		this.chk_Stack2.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Stack2.Properties.ValueChecked = "Y";
		this.chk_Stack2.Properties.ValueGrayed = "N";
		this.chk_Stack2.Properties.ValueUnchecked = "N";
		this.chk_Stack2.Size = new System.Drawing.Size(98, 20);
		this.chk_Stack2.TabIndex = 6;
		this.chk_Stack2.CheckedChanged += new System.EventHandler(this.chk_Stack2_CheckedChanged);
		this.wizardPage1.Controls.Add(this.gridControl1);
		this.wizardPage1.Controls.Add(this.panel4);
		this.wizardPage1.Name = "wizardPage1";
		this.wizardPage1.Size = new System.Drawing.Size(1003, 647);
		this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl1.Location = new System.Drawing.Point(0, 70);
		this.gridControl1.MainView = this.tileView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(1003, 577);
		this.gridControl1.TabIndex = 2;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView1 });
		this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.colImage, this.colNotes });
		this.tileView1.GridControl = this.gridControl1;
		this.tileView1.Images = this.imageCollection1;
		this.tileView1.Name = "tileView1";
		this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(456, 382);
		this.tileView1.OptionsView.BackgroundColorMode = DevExpress.XtraGrid.Views.Tile.BackgroundColorMode.ItemBackground;
		this.tileView1.TileColumns.Add(obj);
		tableRowDefinition.Length.Value = 260.0;
		tableRowDefinition2.Length.Value = 18.0;
		this.tileView1.TileRows.Add(tableRowDefinition);
		this.tileView1.TileRows.Add(tableRowDefinition2);
		tileViewItemElement.Column = this.colNotes;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "colNotes";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomCenter;
		tileViewItemElement2.Column = this.colImage;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "colImage";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileView1.TileTemplate.Add(tileViewItemElement);
		this.tileView1.TileTemplate.Add(tileViewItemElement2);
		this.tileView1.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileView1_ItemClick);
		this.imageCollection1.ImageSize = new System.Drawing.Size(64, 64);
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.imageCollection1.Images.SetKeyName(0, "designform1.PNG");
		this.imageCollection1.Images.SetKeyName(1, "designform2.PNG");
		this.imageCollection1.Images.SetKeyName(2, "designform3.PNG");
		this.panel4.BackColor = System.Drawing.Color.White;
		this.panel4.Controls.Add(this.labelControl4);
		this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel4.Location = new System.Drawing.Point(0, 0);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(1003, 70);
		this.panel4.TabIndex = 1;
		this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 13f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
		this.labelControl4.Appearance.Options.UseFont = true;
		this.labelControl4.Location = new System.Drawing.Point(22, 29);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(260, 16);
		this.labelControl4.TabIndex = 0;
		this.labelControl4.Text = "Select the form type you wish to create";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.runDesignerToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(145, 26);
		this.runDesignerToolStripMenuItem.Name = "runDesignerToolStripMenuItem";
		this.runDesignerToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.runDesignerToolStripMenuItem.Text = "Run Designer";
		this.imageCollection2.ImageSize = new System.Drawing.Size(100, 100);
		this.imageCollection2.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection2.ImageStream");
		this.imageCollection2.Images.SetKeyName(0, "icon-grid");
		this.imageCollection2.Images.SetKeyName(1, "icon-chart");
		this.imageCollection2.Images.SetKeyName(2, "icon-scatter");
		this.imageCollection2.Images.SetKeyName(3, "icon-pies");
		this.imageCollection2.Images.SetKeyName(4, "icon-gauges");
		this.imageCollection2.Images.SetKeyName(5, "icon-card");
		this.imageCollection2.Images.SetKeyName(6, "icon-map");
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1003, 693);
		base.Controls.Add(this.wizardControlEx1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oTemplateWizrd";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TemplateWizard";
		base.ShowInTaskbar = false;
		this.Text = "Template Wizard";
		base.Load += new System.EventHandler(this.frm_TemplateWizard_Load);
		((System.ComponentModel.ISupportInitialize)this.wizardControlEx1).EndInit();
		this.wizardControlEx1.ResumeLayout(false);
		this.wizardPage_RegistrationMode.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Form).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Form).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Mode.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.wizardPage_BasicSetting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_FormType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LayoutCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_FatherMenu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CancelButton.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AcceptButton.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).EndInit();
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		this.wizardPage_Container.ResumeLayout(false);
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack0.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Stack2.Properties).EndInit();
		this.wizardPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		this.panel4.ResumeLayout(false);
		this.panel4.PerformLayout();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.imageCollection2).EndInit();
		base.ResumeLayout(false);
	}
}
