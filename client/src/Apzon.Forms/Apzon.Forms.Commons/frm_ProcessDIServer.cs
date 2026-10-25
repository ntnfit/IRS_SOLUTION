using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit;

namespace Apzon.Forms.Commons;

public class frm_ProcessDIServer : XtraFormEx
{
	private string xmlRequest = "";

	private IContainer components;

	private RichEditControl txt_Xml;

	private SimpleButtonEx btn_Start;

	private SplitContainerControl splitContainerControl1;

	private RichEditControl txt_Log;

	private SpinEdit spin_Timer;

	private SpinEdit spin_Number;

	private LabelControl labelControl1;

	private LabelControl labelControl2;

	private Timer timer_Request;

	private SimpleButtonEx btn_Format;

	private SimpleButtonEx brn_Refresh;

	private SimpleButtonEx btn_Cancel;

	public frm_ProcessDIServer()
	{
		InitializeComponent();
	}

	private void btn_Start_Click(object sender, EventArgs e)
	{
		try
		{
			timer_Request.Enabled = btn_Start.Text.Equals("Start");
			if (btn_Start.Text.Equals("Start"))
			{
				btn_Start.Text = "Stop";
				txt_Xml.ReadOnly = true;
				spin_Timer.Enabled = false;
				spin_Number.Enabled = false;
			}
			else
			{
				btn_Start.Text = "Start";
				txt_Xml.ReadOnly = false;
				spin_Timer.Enabled = true;
				spin_Number.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spin_Timer_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			timer_Request.Interval = Function.ParseInt(spin_Timer.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void timer_Request_Tick(object sender, EventArgs e)
	{
		try
		{
			int times = Function.ParseInt(spin_Number.EditValue);
			for (int i = 0; i < times; i++)
			{
				HttpResult httpResult = await ProcessXmlRequest(new Tuple<string>(txt_Xml.Text));
				txt_Log.Document.InsertText(txt_Log.Document.Range.Start, $"{httpResult.Message} {httpResult.Content}\n");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<HttpResult> ProcessXmlRequest(Tuple<string> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Commons");
			return await helper.PostAsync("ProcessXmlRequest", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Xml_TextChanged(object sender, EventArgs e)
	{
		try
		{
			xmlRequest = txt_Xml.Text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Format_Click(object sender, EventArgs e)
	{
		try
		{
			XDocument xDocument = XDocument.Parse(txt_Xml.Text);
			txt_Xml.Text = xDocument.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ProcessDIServer_Load(object sender, EventArgs e)
	{
		txt_Log.CreateNewDocument();
	}

	private void brn_Refresh_Click(object sender, EventArgs e)
	{
		txt_Xml.CreateNewDocument();
		txt_Log.CreateNewDocument();
		spin_Number.EditValue = 1;
		spin_Timer.EditValue = 1000;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ProcessDIServer));
		this.txt_Xml = new DevExpress.XtraRichEdit.RichEditControl();
		this.btn_Start = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.txt_Log = new DevExpress.XtraRichEdit.RichEditControl();
		this.spin_Timer = new DevExpress.XtraEditors.SpinEdit();
		this.spin_Number = new DevExpress.XtraEditors.SpinEdit();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.timer_Request = new System.Windows.Forms.Timer(this.components);
		this.btn_Format = new Apzon.Controls.Controls.SimpleButtonEx();
		this.brn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Number.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_Xml.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.txt_Xml.Dock = System.Windows.Forms.DockStyle.Fill;
		this.txt_Xml.EnableToolTips = true;
		this.txt_Xml.Location = new System.Drawing.Point(0, 0);
		this.txt_Xml.Name = "txt_Xml";
		this.txt_Xml.Options.CopyPaste.MaintainDocumentSectionSettings = false;
		this.txt_Xml.Options.Fields.UpdateDocVariablesBeforeCopy = true;
		this.txt_Xml.Options.Fields.UpdateDocVariablesBeforePrint = true;
		this.txt_Xml.Options.Fields.UseCurrentCultureDateTimeFormat = false;
		this.txt_Xml.Options.MailMerge.KeepLastParagraph = false;
		this.txt_Xml.Size = new System.Drawing.Size(558, 457);
		this.txt_Xml.TabIndex = 0;
		this.txt_Xml.TextChanged += new System.EventHandler(this.txt_Xml_TextChanged);
		this.btn_Start.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Start.Image = (System.Drawing.Image)resources.GetObject("btn_Start.Image");
		this.btn_Start.Location = new System.Drawing.Point(12, 469);
		this.btn_Start.Name = "btn_Start";
		this.btn_Start.Size = new System.Drawing.Size(75, 23);
		this.btn_Start.TabIndex = 1;
		this.btn_Start.Text = "Start";
		this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(-2, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_Xml);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Log);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(833, 457);
		this.splitContainerControl1.SplitterPosition = 558;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.txt_Log.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.txt_Log.Dock = System.Windows.Forms.DockStyle.Fill;
		this.txt_Log.EnableToolTips = true;
		this.txt_Log.Location = new System.Drawing.Point(0, 0);
		this.txt_Log.Name = "txt_Log";
		this.txt_Log.Options.CopyPaste.MaintainDocumentSectionSettings = false;
		this.txt_Log.Options.Fields.UpdateDocVariablesBeforeCopy = true;
		this.txt_Log.Options.Fields.UpdateDocVariablesBeforePrint = true;
		this.txt_Log.Options.Fields.UseCurrentCultureDateTimeFormat = false;
		this.txt_Log.Options.MailMerge.KeepLastParagraph = false;
		this.txt_Log.Size = new System.Drawing.Size(270, 457);
		this.txt_Log.TabIndex = 1;
		this.spin_Timer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spin_Timer.EditValue = new decimal(new int[4] { 1000, 0, 0, 0 });
		this.spin_Timer.Location = new System.Drawing.Point(150, 471);
		this.spin_Timer.Name = "spin_Timer";
		this.spin_Timer.Properties.AllowMouseWheel = false;
		this.spin_Timer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_Timer.Properties.DisplayFormat.FormatString = "{0:n0} Miliseconds";
		this.spin_Timer.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Timer.Properties.EditFormat.FormatString = "{0:n0} Miliseconds";
		this.spin_Timer.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Timer.Properties.Mask.EditMask = "n0";
		this.spin_Timer.Properties.NullText = "0";
		this.spin_Timer.Size = new System.Drawing.Size(123, 20);
		this.spin_Timer.TabIndex = 56;
		this.spin_Timer.ToolTip = "Thời gian tự động gửi dữ liệu";
		this.spin_Timer.EditValueChanged += new System.EventHandler(this.spin_Timer_EditValueChanged);
		this.spin_Number.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spin_Number.EditValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_Number.Location = new System.Drawing.Point(318, 471);
		this.spin_Number.Name = "spin_Number";
		this.spin_Number.Properties.AllowMouseWheel = false;
		this.spin_Number.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_Number.Properties.DisplayFormat.FormatString = "{0:n0} Times";
		this.spin_Number.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Number.Properties.EditFormat.FormatString = "{0:n0} Times";
		this.spin_Number.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Number.Properties.Mask.EditMask = "n0";
		this.spin_Number.Properties.MaxValue = new decimal(new int[4] { 60, 0, 0, 0 });
		this.spin_Number.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_Number.Properties.NullText = "0";
		this.spin_Number.Size = new System.Drawing.Size(82, 20);
		this.spin_Number.TabIndex = 57;
		this.spin_Number.ToolTip = "Thời gian tự động gửi dữ liệu";
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelControl1.Location = new System.Drawing.Point(288, 474);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(24, 13);
		this.labelControl1.TabIndex = 58;
		this.labelControl1.Text = "Send";
		this.labelControl2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelControl2.Location = new System.Drawing.Point(116, 474);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(28, 13);
		this.labelControl2.TabIndex = 59;
		this.labelControl2.Text = "Every";
		this.timer_Request.Interval = 1000;
		this.timer_Request.Tick += new System.EventHandler(this.timer_Request_Tick);
		this.btn_Format.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Format.Image = (System.Drawing.Image)resources.GetObject("btn_Format.Image");
		this.btn_Format.Location = new System.Drawing.Point(446, 470);
		this.btn_Format.Name = "btn_Format";
		this.btn_Format.Size = new System.Drawing.Size(75, 23);
		this.btn_Format.TabIndex = 60;
		this.btn_Format.Text = "Format";
		this.btn_Format.Click += new System.EventHandler(this.btn_Format_Click);
		this.brn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.brn_Refresh.Image = (System.Drawing.Image)resources.GetObject("brn_Refresh.Image");
		this.brn_Refresh.Location = new System.Drawing.Point(527, 470);
		this.brn_Refresh.Name = "brn_Refresh";
		this.brn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.brn_Refresh.TabIndex = 61;
		this.brn_Refresh.Text = "Refresh";
		this.brn_Refresh.Click += new System.EventHandler(this.brn_Refresh_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(744, 470);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 62;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(831, 498);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.brn_Refresh);
		base.Controls.Add(this.btn_Format);
		base.Controls.Add(this.labelControl2);
		base.Controls.Add(this.labelControl1);
		base.Controls.Add(this.spin_Number);
		base.Controls.Add(this.spin_Timer);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Start);
		base.Name = "frm_ProcessDIServer";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Process DIServer";
		base.Load += new System.EventHandler(this.frm_ProcessDIServer_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.spin_Timer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Number.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
