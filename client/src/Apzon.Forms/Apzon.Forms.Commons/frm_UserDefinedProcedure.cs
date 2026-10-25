using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Compiler;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Portable;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Commons;

public class frm_UserDefinedProcedure : XtraFormEx
{
	protected CustomSyntaxHighlightService customSyntaxHighlightService;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtFormula = new DataTable();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private TreeList tv_Category;

	private LabelLine labelLine1;

	private LookUpEdit look_Events;

	private RichEditControl txt_Formula;

	private SimpleButtonEx btn_OK;

	private TreeListColumnEx treeListColumn1;

	private TreeListColumnEx treeListColumn2;

	private TreeListColumnEx treeListColumn3;

	private TreeListColumnEx treeListColumn4;

	private TreeListColumnEx treeListColumn5;

	private ContextMenuStrip ctx_Category;

	private ToolStripMenuItem createNewFormulaToolStripMenuItem;

	private SimpleButtonEx btn_Verify;

	private LabelLine labelLine2;

	private TextEditEx txt_Description;

	private GroupControl groupControl1;

	private SimpleButtonEx btn_Edit;

	private CheckEditEx chk_Active;

	public string FormCode { get; set; }

	public frm_UserDefinedProcedure()
	{
		InitializeComponent();
		customSyntaxHighlightService = new CustomSyntaxHighlightService(txt_Formula.Document);
	}

	private void frm_Formula_Load(object sender, EventArgs e)
	{
		try
		{
			using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(txt_Formula.Text)))
			{
				txt_Formula.ActiveViewType = RichEditViewType.Draft;
				txt_Formula.ReplaceService((ISyntaxHighlightService)customSyntaxHighlightService);
				txt_Formula.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
				txt_Formula.Document.Sections[0].Page.Width = 1000f;
				txt_Formula.Document.DefaultCharacterProperties.FontName = "Consolas";
				txt_Formula.Document.DefaultCharacterProperties.FontSize = 8.75f;
				txt_Formula.Views.SimpleView.AllowDisplayLineNumbers = true;
				txt_Formula.Views.DraftView.AllowDisplayLineNumbers = true;
				txt_Formula.Views.PrintLayoutView.AllowDisplayLineNumbers = true;
				txt_Formula.Views.SimpleView.Padding = new PortablePadding(80, 4, 0, 0);
				txt_Formula.Views.DraftView.Padding = new PortablePadding(80, 4, 0, 0);
				txt_Formula.Document.Sections[0].LineNumbering.RestartType = LineNumberingRestart.Continuous;
				txt_Formula.Document.Sections[0].LineNumbering.Start = 1;
				txt_Formula.Document.Sections[0].LineNumbering.CountBy = 1;
				txt_Formula.Document.Sections[0].LineNumbering.Distance = 0.1f;
				txt_Formula.Unit = DocumentUnit.Document;
				txt_Formula.Document.Sections[0].Margins.Top = 15f;
				txt_Formula.Document.Sections[0].Margins.Left = 50f;
				txt_Formula.Document.Sections[0].Margins.Bottom = 0f;
				txt_Formula.Document.Sections[0].Margins.Right = 0f;
				float num = Units.PixelsToDocumentsF(2048f, txt_Formula.DpiX);
				float num2 = Units.PixelsToDocumentsF(2048f, txt_Formula.DpiY);
				txt_Formula.Document.Sections[0].Page.Width = num;
				txt_Formula.Document.Sections[0].Page.Height = num2;
			}
			if (GlobalData.AllModule == null)
			{
				return;
			}
			DataTable dataSource = (from t in GlobalData.AllModule.AsEnumerable()
				where !CommonList.HiddenFunction.ContainsKey(Function.ToString(t["Code"]))
				select t).CopyToDataTable();
			tv_Category.DataSource = dataSource;
			tv_Category.RefreshDataSource();
			foreach (TreeListColumn column in tv_Category.Columns)
			{
				column.OptionsColumn.AllowEdit = false;
			}
			LoadEvents();
			_dtFormula = new DataTable();
			_dtFormula.Columns.Add("FormID", typeof(string));
			_dtFormula.Columns.Add("MethodID", typeof(string));
			_dtFormula.Columns.Add("Description", typeof(string));
			_dtFormula.Columns.Add("Formula", typeof(string));
			_dtFormula.Columns.Add("Active", typeof(string));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadEvents()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (object value in Enum.GetValues(typeof(Enums.EventId)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)(Enums.EventId)value;
				dataRow["Name"] = value;
				dataTable.Rows.Add(dataRow);
			}
			look_Events.Properties.DataSource = dataTable;
			look_Events.Properties.DisplayMember = "Name";
			look_Events.Properties.ValueMember = "Code";
			look_Events.Properties.ShowHeader = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void createNewFormulaToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ShowEditForm();
	}

	private void ShowEditForm()
	{
		try
		{
			if (!(Function.ToString(tv_Category.FocusedNode.GetValue("MenuType")) != "N"))
			{
				return;
			}
			string code = Function.ToString(tv_Category.FocusedNode.GetValue("Code"));
			Type type = ((!Main.FormType.ContainsKey(code)) ? AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetLoadableTypes()).FirstOrDefault((Type x) => x.Name == code) : Type.GetType(Main.FormType[code]));
			if (type != null)
			{
				object obj = Activator.CreateInstance(type);
				string obj2 = Function.ToString(look_Events.EditValue);
				XtraFormEx xtraFormEx = (XtraFormEx)obj;
				if (xtraFormEx.DisableLoadScript)
				{
					throw new Exception("DIY Script not available for this form.");
				}
				frm_UserDefinedProcedureCreate frm_UserDefinedProcedureCreate2 = new frm_UserDefinedProcedureCreate(xtraFormEx, code, Function.ParseInt(obj2));
				frm_UserDefinedProcedureCreate2.MdiParent = MdiParent;
				frm_UserDefinedProcedureCreate2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void tv_Category_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		await ShowText();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_OK.Text.Equals(Resources.ButtonOK))
			{
				Close();
			}
			if (!btn_OK.Text.Equals(Resources.ButtonSave))
			{
				return;
			}
			if (string.IsNullOrEmpty(FormCode))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCategory, "");
				return;
			}
			string value = Function.ToString(txt_Formula.Text);
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.PleaseWriteFomularBeforeSaving, "");
				return;
			}
			string value2 = Function.ToString(look_Events.EditValue);
			if (string.IsNullOrEmpty(value2))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseEventType, "");
				return;
			}
			string text = Verify();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable = _dtFormula.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["FormID"] = FormCode;
				dataRow["MethodID"] = value2;
				dataRow["Description"] = Function.ToString(txt_Description.EditValue);
				dataRow["Formula"] = value;
				dataRow["Active"] = Function.ToString(chk_Active.EditValue);
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await _httpClient.Formula.Save(dataTable);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				btn_OK.Text = Resources.ButtonOK;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Verify_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Verify();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string Verify()
	{
		try
		{
			string text = Function.ToString(txt_Formula.Text);
			if (!string.IsNullOrEmpty(text))
			{
				Type type = ((!Main.FormType.ContainsKey(FormCode)) ? AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetLoadableTypes()).FirstOrDefault((Type x) => x.Name == FormCode) : Type.GetType(Main.FormType[FormCode]));
				if (type != null)
				{
					XtraFormEx xtraFormEx = (XtraFormEx)Activator.CreateInstance(type);
					if (xtraFormEx.DisableLoadScript)
					{
						return "DIY Script not available for this form.";
					}
					return new Compiler().ParseSyntax(xtraFormEx, text).Message;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private async void look_Events_EditValueChanged(object sender, EventArgs e)
	{
		await ShowText();
	}

	private async Task ShowText()
	{
		try
		{
			if (Function.ToString(tv_Category.FocusedNode.GetValue("MenuType")) != "N")
			{
				FormCode = Function.ToString(tv_Category.FocusedNode.GetValue("Code"));
				string text = Function.ToString(look_Events.EditValue);
				if (!string.IsNullOrEmpty(text))
				{
					Enums.EventId enumEvent = (Enums.EventId)Function.ParseInt(text);
					DataTable dataTable = await _httpClient.Formula.GetFormula(FormCode, enumEvent, "");
					if (dataTable.IsNotNull())
					{
						txt_Formula.Text = Function.ToString(dataTable.Rows[0]["Formula"]);
						txt_Description.EditValue = Function.ToString(dataTable.Rows[0]["Description"]);
						chk_Active.EditValue = Function.ToString(dataTable.Rows[0]["Active"]);
					}
					else
					{
						txt_Formula.Text = "";
					}
				}
				else
				{
					txt_Formula.Text = "";
				}
			}
			else
			{
				txt_Formula.Text = "";
			}
			txt_Formula.Document.DefaultCharacterProperties.FontName = "Consolas";
			txt_Formula.Document.DefaultCharacterProperties.FontSize = 8.75f;
			txt_Formula.Document.Sections[0].LineNumbering.RestartType = LineNumberingRestart.Continuous;
			txt_Formula.Document.Sections[0].LineNumbering.Start = 1;
			txt_Formula.Document.Sections[0].LineNumbering.CountBy = 1;
			txt_Formula.Document.Sections[0].LineNumbering.Distance = 0.1f;
			txt_Formula.Unit = DocumentUnit.Document;
			txt_Formula.Document.Sections[0].Margins.Top = 15f;
			txt_Formula.Document.Sections[0].Margins.Left = 50f;
			txt_Formula.Document.Sections[0].Margins.Bottom = 0f;
			txt_Formula.Document.Sections[0].Margins.Right = 0f;
			float num = Units.PixelsToDocumentsF(2048f, txt_Formula.DpiX);
			float num2 = Units.PixelsToDocumentsF(2048f, txt_Formula.DpiY);
			txt_Formula.Document.Sections[0].Page.Width = num;
			txt_Formula.Document.Sections[0].Page.Height = num2;
			btn_OK.Text = Resources.ButtonOK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Formula_KeyPress(object sender, KeyPressEventArgs e)
	{
		try
		{
			btn_OK.Text = Resources.ButtonSave;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Edit_Click(object sender, EventArgs e)
	{
		ShowEditForm();
	}

	private void tv_Category_DoubleClick(object sender, EventArgs e)
	{
		ShowEditForm();
	}

	private void txt_Description_EditValueChanged(object sender, EventArgs e)
	{
		btn_OK.Text = Resources.ButtonSave;
	}

	private void chk_Active_CheckedChanged(object sender, EventArgs e)
	{
		btn_OK.Text = Resources.ButtonSave;
	}

	private void frm_UserDefinedProcedure_FormClosed(object sender, FormClosedEventArgs e)
	{
		customSyntaxHighlightService.timer.Stop();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_UserDefinedProcedure));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.tv_Category = new DevExpress.XtraTreeList.TreeList();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctx_Category = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.createNewFormulaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_Description = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Formula = new DevExpress.XtraRichEdit.RichEditControl();
		this.look_Events = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Verify = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Edit = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Category).BeginInit();
		this.ctx_Category.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Events.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.chk_Active);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Description);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Formula);
		this.splitContainerControl1.Panel2.Controls.Add(this.look_Events);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(942, 478);
		this.splitContainerControl1.SplitterPosition = 237;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl1.Controls.Add(this.tv_Category);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(237, 478);
		this.groupControl1.TabIndex = 1;
		this.groupControl1.Text = "Categories";
		this.tv_Category.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tv_Category.Appearance.FocusedCell.Options.UseBackColor = true;
		this.tv_Category.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tv_Category.Appearance.FocusedRow.Options.UseBackColor = true;
		this.tv_Category.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tv_Category.Appearance.SelectedRow.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.tv_Category.Appearance.SelectedRow.Options.UseBackColor = true;
		this.tv_Category.Appearance.SelectedRow.Options.UseFont = true;
		this.tv_Category.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[5] { this.treeListColumn1, this.treeListColumn2, this.treeListColumn3, this.treeListColumn4, this.treeListColumn5 });
		this.tv_Category.ContextMenuStrip = this.ctx_Category;
		this.tv_Category.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Category.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Category.KeyFieldName = "Id";
		this.tv_Category.Location = new System.Drawing.Point(2, 23);
		this.tv_Category.Name = "tv_Category";
		this.tv_Category.OptionsView.ShowColumns = false;
		this.tv_Category.OptionsView.ShowHorzLines = false;
		this.tv_Category.OptionsView.ShowIndentAsRowStyle = true;
		this.tv_Category.OptionsView.ShowIndicator = false;
		this.tv_Category.OptionsView.ShowVertLines = false;
		this.tv_Category.ParentFieldName = "FatherId";
		this.tv_Category.Size = new System.Drawing.Size(233, 453);
		this.tv_Category.TabIndex = 0;
		this.tv_Category.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Category_FocusedNodeChanged);
		this.tv_Category.DoubleClick += new System.EventHandler(this.tv_Category_DoubleClick);
		this.treeListColumn1.Caption = "Id";
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "Id";
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.OptionsColumn.AllowEdit = false;
		this.treeListColumn1.OptionsColumn.AllowMove = false;
		this.treeListColumn1.OptionsColumn.AllowSize = false;
		this.treeListColumn1.OptionsColumn.AllowSort = false;
		this.treeListColumn2.Caption = "FatherId";
		this.treeListColumn2.CurrencyDateField = null;
		this.treeListColumn2.CurrencyField = null;
		this.treeListColumn2.CurrencyFixed = null;
		this.treeListColumn2.CurrencyFixedRate = 0.0;
		this.treeListColumn2.CurrencyRateField = null;
		this.treeListColumn2.FieldName = "FatherId";
		this.treeListColumn2.Name = "treeListColumn2";
		this.treeListColumn2.OptionsColumn.AllowEdit = false;
		this.treeListColumn2.OptionsColumn.AllowMove = false;
		this.treeListColumn2.OptionsColumn.AllowSize = false;
		this.treeListColumn2.OptionsColumn.AllowSort = false;
		this.treeListColumn3.Caption = "Code";
		this.treeListColumn3.CurrencyDateField = null;
		this.treeListColumn3.CurrencyField = null;
		this.treeListColumn3.CurrencyFixed = null;
		this.treeListColumn3.CurrencyFixedRate = 0.0;
		this.treeListColumn3.CurrencyRateField = null;
		this.treeListColumn3.FieldName = "Code";
		this.treeListColumn3.Name = "treeListColumn3";
		this.treeListColumn3.OptionsColumn.AllowEdit = false;
		this.treeListColumn3.OptionsColumn.AllowMove = false;
		this.treeListColumn3.OptionsColumn.AllowSize = false;
		this.treeListColumn3.OptionsColumn.AllowSort = false;
		this.treeListColumn4.Caption = "Name";
		this.treeListColumn4.CurrencyDateField = null;
		this.treeListColumn4.CurrencyField = null;
		this.treeListColumn4.CurrencyFixed = null;
		this.treeListColumn4.CurrencyFixedRate = 0.0;
		this.treeListColumn4.CurrencyRateField = null;
		this.treeListColumn4.FieldName = "Name";
		this.treeListColumn4.Name = "treeListColumn4";
		this.treeListColumn4.OptionsColumn.AllowEdit = false;
		this.treeListColumn4.OptionsColumn.AllowMove = false;
		this.treeListColumn4.OptionsColumn.AllowSize = false;
		this.treeListColumn4.OptionsColumn.AllowSort = false;
		this.treeListColumn4.Visible = true;
		this.treeListColumn4.VisibleIndex = 0;
		this.treeListColumn5.Caption = "IsForm";
		this.treeListColumn5.CurrencyDateField = null;
		this.treeListColumn5.CurrencyField = null;
		this.treeListColumn5.CurrencyFixed = null;
		this.treeListColumn5.CurrencyFixedRate = 0.0;
		this.treeListColumn5.CurrencyRateField = null;
		this.treeListColumn5.FieldName = "IsForm";
		this.treeListColumn5.Name = "treeListColumn5";
		this.treeListColumn5.OptionsColumn.AllowEdit = false;
		this.treeListColumn5.OptionsColumn.AllowMove = false;
		this.treeListColumn5.OptionsColumn.AllowSize = false;
		this.treeListColumn5.OptionsColumn.AllowSort = false;
		this.ctx_Category.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.createNewFormulaToolStripMenuItem });
		this.ctx_Category.Name = "ctx_Category";
		this.ctx_Category.Size = new System.Drawing.Size(152, 26);
		this.createNewFormulaToolStripMenuItem.Name = "createNewFormulaToolStripMenuItem";
		this.createNewFormulaToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
		this.createNewFormulaToolStripMenuItem.Text = "Edit Procedure";
		this.createNewFormulaToolStripMenuItem.Click += new System.EventHandler(this.createNewFormulaToolStripMenuItem_Click);
		this.chk_Active.EditValue = "Y";
		this.chk_Active.FieldName = null;
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(543, 8);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 11;
		this.txt_Description.FieldName = null;
		this.txt_Description.IsSearchControl = false;
		this.txt_Description.Location = new System.Drawing.Point(366, 7);
		this.txt_Description.Name = "txt_Description";
		this.txt_Description.Size = new System.Drawing.Size(171, 20);
		this.txt_Description.TabIndex = 4;
		this.txt_Description.EditValueChanged += new System.EventHandler(this.txt_Description_EditValueChanged);
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(277, 10);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(90, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "Description";
		this.txt_Formula.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
		this.txt_Formula.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Formula.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.txt_Formula.Location = new System.Drawing.Point(0, 38);
		this.txt_Formula.Margin = new System.Windows.Forms.Padding(2);
		this.txt_Formula.Name = "txt_Formula";
		this.txt_Formula.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.txt_Formula.Size = new System.Drawing.Size(699, 438);
		this.txt_Formula.TabIndex = 2;
		this.txt_Formula.Views.DraftView.AllowDisplayLineNumbers = true;
		this.txt_Formula.Views.SimpleView.AdjustColorsToSkins = true;
		this.txt_Formula.Views.SimpleView.AllowDisplayLineNumbers = true;
		this.txt_Formula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Formula_KeyPress);
		this.look_Events.Location = new System.Drawing.Point(90, 10);
		this.look_Events.Name = "look_Events";
		this.look_Events.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Events.Properties.NullText = "";
		this.look_Events.Size = new System.Drawing.Size(171, 20);
		this.look_Events.TabIndex = 1;
		this.look_Events.EditValueChanged += new System.EventHandler(this.look_Events_EditValueChanged);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 12);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(90, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "On Event";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 491);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Verify.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Verify.FieldName = null;
		this.btn_Verify.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Verify.ImageOptions.Image");
		this.btn_Verify.IsSearchControl = false;
		this.btn_Verify.Location = new System.Drawing.Point(826, 491);
		this.btn_Verify.Name = "btn_Verify";
		this.btn_Verify.Size = new System.Drawing.Size(116, 23);
		this.btn_Verify.TabIndex = 2;
		this.btn_Verify.Text = "Verify Formula";
		this.btn_Verify.Click += new System.EventHandler(this.btn_Verify_Click);
		this.btn_Edit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Edit.FieldName = null;
		this.btn_Edit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Edit.ImageOptions.Image");
		this.btn_Edit.IsSearchControl = false;
		this.btn_Edit.Location = new System.Drawing.Point(704, 491);
		this.btn_Edit.Name = "btn_Edit";
		this.btn_Edit.Size = new System.Drawing.Size(116, 23);
		this.btn_Edit.TabIndex = 3;
		this.btn_Edit.Text = "Edit";
		this.btn_Edit.Click += new System.EventHandler(this.btn_Edit_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(943, 518);
		base.Controls.Add(this.btn_Edit);
		base.Controls.Add(this.btn_Verify);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUserDefinedProcedure";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UserDefinedProcedure.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UserDefinedProcedure";
		this.Text = "DIY Script";
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_UserDefinedProcedure_FormClosed);
		base.Load += new System.EventHandler(this.frm_Formula_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Category).EndInit();
		this.ctx_Category.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Events.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
