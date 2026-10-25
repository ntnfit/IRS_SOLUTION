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
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Reports;

public class dlg_ImportReport : XtraFormEx
{
	private IContainer components;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Browse;

	private SimpleButtonEx btn_Save;

	private LabelLine lbl_Menu;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txt_PatchFile;

	private TextEditEx txt_ReportName;

	private MemoEditEx txt_Desc;

	private RadioGroupEx rdo_Report;

	private ButtonEditEx txt_DocumentType;

	private LabelLine lbl_DocumentType;

	private TextEditEx txt_DocCode;

	private LabelLine lbl_DocCode;

	private ButtonEditCFL bte_ParentMenuId;

	public string patchFile { get; set; }

	public string menu { get; set; }

	public string reportName { get; set; }

	public string desc { get; set; }

	public string docCode { get; set; }

	public string typeCode { get; set; }

	public string typeName { get; set; }

	public dlg_ImportReport()
	{
		InitializeComponent();
	}

	private void dlg_ImportXtraReport_Load(object sender, EventArgs e)
	{
		try
		{
			bte_ParentMenuId.Properties.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			bte_ParentMenuId.Properties.StoredProcedure = "Apz_Cfl_GetFatherMenu";
			bte_ParentMenuId.Properties.RefreshDataSource();
			bte_ParentMenuId.Properties.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL properties = bte_ParentMenuId.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm._tableType = "2";
			}));
			rdo_Report.EditValue = "R";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (ValidateForm())
			{
				patchFile = txt_PatchFile.Text;
				menu = Function.ToString(bte_ParentMenuId.EditValue);
				reportName = txt_ReportName.Text;
				desc = txt_Desc.Text;
				docCode = txt_DocCode.Text;
				if (rdo_Report.EditValue.Equals("L"))
				{
					menu = "";
				}
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".repx",
				Multiselect = false,
				Filter = "XreaReport Layout (.repx)|*.repx"
			};
			openFileDialog.ShowDialog(this);
			if (!string.IsNullOrEmpty(openFileDialog.FileName))
			{
				txt_PatchFile.Text = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool ValidateForm()
	{
		if (!Check._text(txt_PatchFile))
		{
			return false;
		}
		if (!Check._text(txt_ReportName))
		{
			return false;
		}
		if (rdo_Report.EditValue.Equals("R") && !Check._text(bte_ParentMenuId))
		{
			return false;
		}
		if (!rdo_Report.EditValue.Equals("R") && !Check._text(txt_DocumentType))
		{
			return false;
		}
		return true;
	}

	private void rdo_Report_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (rdo_Report.EditValue.Equals("R"))
			{
				lbl_DocCode.Visible = false;
				txt_DocCode.Visible = false;
				txt_DocCode.Text = "";
				lbl_DocumentType.Visible = false;
				txt_DocumentType.Visible = false;
				txt_DocumentType.Text = "";
				lbl_Menu.Visible = true;
				bte_ParentMenuId.Visible = true;
			}
			else
			{
				lbl_DocCode.Visible = true;
				txt_DocCode.Visible = true;
				lbl_DocumentType.Visible = true;
				txt_DocumentType.Visible = true;
				lbl_Menu.Visible = false;
				bte_ParentMenuId.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_DocumentType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string txtSeach = Function.ToString(txt_DocumentType.EditValue);
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, txtSeach, "Apz_XtraReport_LoadDocumentReport", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_DocumentType.EditValue = Function.ToString(frm_CFL2.ChooseItem["DocName"]);
				txt_DocCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["DocCode"]);
				typeCode = Function.ToString(frm_CFL2.ChooseItem["TypeCode"]);
				typeName = Function.ToString(frm_CFL2.ChooseItem["TypeName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_DocumentType_KeyDown(object sender, KeyEventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.dlg_ImportReport));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.btn_Browse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_PatchFile = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ReportName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Desc = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Menu = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.rdo_Report = new Apzon.Controls.Editor.RadioGroupEx();
		this.txt_DocumentType = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_DocumentType = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocCode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_DocCode = new Apzon.Controls.Controls.LabelLine();
		this.bte_ParentMenuId = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.txt_PatchFile.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ReportName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_Report.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Browse.FieldName = null;
		this.btn_Browse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Browse.ImageOptions.Image");
		this.btn_Browse.IsSearchControl = false;
		this.btn_Browse.Location = new System.Drawing.Point(349, 12);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.Size = new System.Drawing.Size(71, 23);
		this.btn_Browse.TabIndex = 14;
		this.btn_Browse.Text = "Browse";
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 287);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 22;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txt_PatchFile.FieldName = null;
		this.txt_PatchFile.IsSearchControl = false;
		this.txt_PatchFile.Location = new System.Drawing.Point(94, 13);
		this.txt_PatchFile.Name = "txt_PatchFile";
		this.txt_PatchFile.Properties.ReadOnly = true;
		this.txt_PatchFile.Size = new System.Drawing.Size(244, 20);
		this.txt_PatchFile.TabIndex = 29;
		this.txt_ReportName.FieldName = null;
		this.txt_ReportName.IsSearchControl = false;
		this.txt_ReportName.Location = new System.Drawing.Point(94, 72);
		this.txt_ReportName.Name = "txt_ReportName";
		this.txt_ReportName.Size = new System.Drawing.Size(244, 20);
		this.txt_ReportName.TabIndex = 30;
		this.txt_Desc.FieldName = null;
		this.txt_Desc.IsSearchControl = false;
		this.txt_Desc.Location = new System.Drawing.Point(94, 98);
		this.txt_Desc.Name = "txt_Desc";
		this.txt_Desc.Size = new System.Drawing.Size(244, 96);
		this.txt_Desc.TabIndex = 32;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(7, 99);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(87, 21);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 28;
		this.labelLine4.Utext = "Description";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(7, 75);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(87, 21);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 26;
		this.labelLine3.Utext = "Report Name";
		this.lbl_Menu.FieldName = null;
		this.lbl_Menu.HideWhenEmpty = false;
		this.lbl_Menu.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Menu.Location = new System.Drawing.Point(7, 207);
		this.lbl_Menu.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lbl_Menu.Name = "lbl_Menu";
		this.lbl_Menu.ShowUnderLine = true;
		this.lbl_Menu.Size = new System.Drawing.Size(87, 21);
		this.lbl_Menu.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Menu.TabIndex = 23;
		this.lbl_Menu.Utext = "Menu";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 15);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(87, 21);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 13;
		this.labelLine1.Utext = "File Location";
		this.rdo_Report.FieldName = null;
		this.rdo_Report.IsSearchControl = false;
		this.rdo_Report.Location = new System.Drawing.Point(94, 43);
		this.rdo_Report.Name = "rdo_Report";
		this.rdo_Report.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("R", "Report"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("L", "Layout")
		});
		this.rdo_Report.Size = new System.Drawing.Size(244, 23);
		this.rdo_Report.TabIndex = 34;
		this.rdo_Report.SelectedIndexChanged += new System.EventHandler(this.rdo_Report_SelectedIndexChanged);
		this.txt_DocumentType.FieldName = null;
		this.txt_DocumentType.IsSearchControl = false;
		this.txt_DocumentType.Location = new System.Drawing.Point(94, 233);
		this.txt_DocumentType.Name = "txt_DocumentType";
		this.txt_DocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_DocumentType.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_DocumentType_Properties_ButtonClick);
		this.txt_DocumentType.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocumentType_KeyDown);
		this.txt_DocumentType.Size = new System.Drawing.Size(244, 20);
		this.txt_DocumentType.TabIndex = 35;
		this.lbl_DocumentType.FieldName = null;
		this.lbl_DocumentType.HideWhenEmpty = false;
		this.lbl_DocumentType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocumentType.Location = new System.Drawing.Point(7, 236);
		this.lbl_DocumentType.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lbl_DocumentType.Name = "lbl_DocumentType";
		this.lbl_DocumentType.ShowUnderLine = true;
		this.lbl_DocumentType.Size = new System.Drawing.Size(87, 21);
		this.lbl_DocumentType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocumentType.TabIndex = 36;
		this.lbl_DocumentType.Utext = "Document Type";
		this.txt_DocCode.FieldName = null;
		this.txt_DocCode.IsSearchControl = false;
		this.txt_DocCode.Location = new System.Drawing.Point(94, 259);
		this.txt_DocCode.Name = "txt_DocCode";
		this.txt_DocCode.Size = new System.Drawing.Size(244, 20);
		this.txt_DocCode.TabIndex = 38;
		this.lbl_DocCode.FieldName = null;
		this.lbl_DocCode.HideWhenEmpty = false;
		this.lbl_DocCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocCode.Location = new System.Drawing.Point(7, 262);
		this.lbl_DocCode.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lbl_DocCode.Name = "lbl_DocCode";
		this.lbl_DocCode.ShowUnderLine = true;
		this.lbl_DocCode.Size = new System.Drawing.Size(87, 21);
		this.lbl_DocCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocCode.TabIndex = 37;
		this.lbl_DocCode.Utext = "Doc Code";
		this.bte_ParentMenuId.FieldName = null;
		this.bte_ParentMenuId.IsSearchControl = false;
		this.bte_ParentMenuId.Location = new System.Drawing.Point(94, 204);
		this.bte_ParentMenuId.Name = "bte_ParentMenuId";
		this.bte_ParentMenuId.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_ParentMenuId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions)
		});
		this.bte_ParentMenuId.Properties.ContainerControl = null;
		this.bte_ParentMenuId.Properties.DisplayMember = "Name";
		this.bte_ParentMenuId.Properties.DisplayMember2 = "";
		this.bte_ParentMenuId.Properties.LoadDataSourceWhenEnter = false;
		this.bte_ParentMenuId.Properties.SeparateCharacterForMulti = ';';
		this.bte_ParentMenuId.Properties.SeparateString = ", ";
		this.bte_ParentMenuId.Properties.UsingAutoCompleteBox = false;
		this.bte_ParentMenuId.Properties.UsingDataSource = true;
		this.bte_ParentMenuId.Properties.ValidateWhenNoDataSource = false;
		this.bte_ParentMenuId.Properties.ValueMember = "Code";
		this.bte_ParentMenuId.Size = new System.Drawing.Size(244, 20);
		this.bte_ParentMenuId.TabIndex = 39;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(426, 322);
		base.Controls.Add(this.bte_ParentMenuId);
		base.Controls.Add(this.txt_DocCode);
		base.Controls.Add(this.lbl_DocCode);
		base.Controls.Add(this.lbl_DocumentType);
		base.Controls.Add(this.txt_DocumentType);
		base.Controls.Add(this.rdo_Report);
		base.Controls.Add(this.txt_Desc);
		base.Controls.Add(this.txt_ReportName);
		base.Controls.Add(this.txt_PatchFile);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.lbl_Menu);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btn_Browse);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oImportReport";
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "dlg_ImportReport";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Import Report";
		base.Load += new System.EventHandler(this.dlg_ImportXtraReport_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_PatchFile.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ReportName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_Report.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
