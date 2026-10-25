using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Web;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using CefSharp;
using CefSharp.WinForms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Reports;

public class frm_ChromeWebBrowser : XtraForm
{
	public List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>();

	public string storeName = "";

	public Enums.WebType type;

	public bool isTimer;

	public string url = "";

	private Timer timeCounter;

	private Timer timer_AutoSearch;

	private int counter = 60;

	private IContainer components;

	private ChromiumWebBrowser chromiumWebBrowser1;

	private SplitContainerControl splitContainerControl1;

	private LabelLine labelLine1;

	private MemoEdit txt_Url;

	private SpinEdit spin_Auto;

	private CheckEditEx chk_Auto;

	private SimpleButtonEx btn_Refresh;

	private LabelLine lbl_CountDown;

	public frm_ChromeWebBrowser()
	{
		InitializeComponent();
	}

	private void frm_ChromeWebBrowser_Load(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			if (!isTimer || string.IsNullOrEmpty(storeName))
			{
				splitContainerControl1.Dock = DockStyle.Fill;
				btn_Refresh.Visible = false;
				chk_Auto.Visible = false;
				spin_Auto.Visible = false;
				lbl_CountDown.Visible = false;
			}
			else
			{
				timer_AutoSearch = new Timer(components);
				timeCounter = new Timer(components);
				timer_AutoSearch.Enabled = !chk_Auto.Checked;
				timeCounter.Enabled = !chk_Auto.Checked;
				timer_AutoSearch.Interval = Function.ParseInt(spin_Auto.EditValue) * 60000;
				timer_AutoSearch.Tick += timer_AutoSearch_Tick;
				timer_AutoSearch.Stop();
				timeCounter.Interval = 1000;
				timeCounter.Tick += timeCounter_Tick;
				timeCounter.Stop();
			}
			if (!string.IsNullOrEmpty(url))
			{
				txt_Url.Text = url;
				if (type == Enums.WebType.Address)
				{
					chromiumWebBrowser1.Load(url);
				}
				else
				{
					chromiumWebBrowser1.LoadHtml(url, "http://www.example.com/");
				}
			}
			else
			{
				LoadWeb();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void timeCounter_Tick(object sender, EventArgs e)
	{
		try
		{
			counter--;
			if (counter == 0)
			{
				timeCounter.Stop();
				timeCounter.Start();
				counter = Function.ParseInt(spin_Auto.EditValue) * 60;
			}
			lbl_CountDown.Utext = counter.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ChromeWebBrowser_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (timer_AutoSearch != null)
			{
				timer_AutoSearch.Stop();
				timer_AutoSearch.Dispose();
			}
			if (timeCounter != null)
			{
				timeCounter.Stop();
				timeCounter.Dispose();
			}
			chromiumWebBrowser1.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Auto_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			spin_Auto.Enabled = !chk_Auto.Checked;
			timer_AutoSearch.Enabled = chk_Auto.Checked;
			timeCounter.Enabled = chk_Auto.Checked;
			counter = Function.ParseInt(spin_Auto.EditValue) * 60;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spin_Auto_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			timer_AutoSearch.Interval = Function.ParseInt(spin_Auto.EditValue) * 60000;
			counter = Function.ParseInt(spin_Auto.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void timer_AutoSearch_Tick(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			LoadWeb();
			if (chk_Auto.Checked)
			{
				timer_AutoSearch.Start();
				counter = Function.ParseInt(spin_Auto.EditValue) * 60;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			LoadWeb();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void LoadWeb()
	{
		try
		{
			if (string.IsNullOrEmpty(storeName))
			{
				return;
			}
			DataSetResult dataSetResult = await HttpClientHelperFactory.GetHttpClient().Commons.ExecuteStore(storeName, lstpr);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
			}
			if (!dataSetResult.Data.IsNotNull() || !dataSetResult.Data.Tables[0].IsNotNull())
			{
				return;
			}
			string text = Function.ToString(dataSetResult.Data.Tables[0].Rows[0][0]);
			if (type == Enums.WebType.Address)
			{
				if (!text.StartsWith("http"))
				{
					text = "http://www.google.com/maps/place/" + HttpUtility.UrlEncode(text);
				}
				txt_Url.Text = text;
				chromiumWebBrowser1.Load(text);
			}
			else if (type == Enums.WebType.Html)
			{
				txt_Url.Text = text;
				chromiumWebBrowser1.LoadHtml(text, "http://www.example.com/");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ChromeWebBrowser));
		this.chromiumWebBrowser1 = new CefSharp.WinForms.ChromiumWebBrowser("");
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Url = new DevExpress.XtraEditors.MemoEdit();
		this.spin_Auto = new DevExpress.XtraEditors.SpinEdit();
		this.chk_Auto = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_CountDown = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Url.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Auto.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Auto.Properties).BeginInit();
		base.SuspendLayout();
		this.chromiumWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chromiumWebBrowser1.Location = new System.Drawing.Point(0, 0);
		this.chromiumWebBrowser1.Name = "chromiumWebBrowser1";
		this.chromiumWebBrowser1.Size = new System.Drawing.Size(997, 626);
		this.chromiumWebBrowser1.TabIndex = 0;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_Url);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.chromiumWebBrowser1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(997, 631);
		this.splitContainerControl1.SplitterPosition = 0;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 22);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(38, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "Url:";
		this.txt_Url.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Url.Location = new System.Drawing.Point(55, 3);
		this.txt_Url.Name = "txt_Url";
		this.txt_Url.Properties.ReadOnly = true;
		this.txt_Url.Size = new System.Drawing.Size(0, 0);
		this.txt_Url.TabIndex = 4;
		this.spin_Auto.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spin_Auto.EditValue = new decimal(new int[4] { 10, 0, 0, 0 });
		this.spin_Auto.Location = new System.Drawing.Point(254, 640);
		this.spin_Auto.Name = "spin_Auto";
		this.spin_Auto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_Auto.Properties.DisplayFormat.FormatString = "{0:n0} Minute";
		this.spin_Auto.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.spin_Auto.Properties.MaxValue = new decimal(new int[4] { 60, 0, 0, 0 });
		this.spin_Auto.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_Auto.Size = new System.Drawing.Size(91, 20);
		this.spin_Auto.TabIndex = 55;
		this.spin_Auto.EditValueChanged += new System.EventHandler(this.spin_Auto_EditValueChanged);
		this.chk_Auto.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_Auto.FieldName = null;
		this.chk_Auto.IsSearchControl = false;
		this.chk_Auto.Location = new System.Drawing.Point(160, 641);
		this.chk_Auto.Name = "chk_Auto";
		this.chk_Auto.Properties.Caption = "Auto Refresh";
		this.chk_Auto.Size = new System.Drawing.Size(88, 19);
		this.chk_Auto.TabIndex = 54;
		this.chk_Auto.CheckedChanged += new System.EventHandler(this.chk_Auto_CheckedChanged);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(12, 638);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 57;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.ToolTip = "Làm mới form";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.lbl_CountDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_CountDown.FieldName = null;
		this.lbl_CountDown.HideWhenEmpty = false;
		this.lbl_CountDown.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CountDown.Location = new System.Drawing.Point(368, 641);
		this.lbl_CountDown.Name = "lbl_CountDown";
		this.lbl_CountDown.ShowUnderLine = true;
		this.lbl_CountDown.Size = new System.Drawing.Size(63, 20);
		this.lbl_CountDown.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CountDown.TabIndex = 58;
		this.lbl_CountDown.Utext = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1001, 666);
		base.Controls.Add(this.lbl_CountDown);
		base.Controls.Add(this.btn_Refresh);
		base.Controls.Add(this.spin_Auto);
		base.Controls.Add(this.chk_Auto);
		base.Controls.Add(this.splitContainerControl1);
		base.MinimizeBox = false;
		base.Name = "frm_ChromeWebBrowser";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Web Browser";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ChromeWebBrowser_FormClosing);
		base.Load += new System.EventHandler(this.frm_ChromeWebBrowser_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Url.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Auto.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Auto.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
