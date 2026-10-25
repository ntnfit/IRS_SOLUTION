using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Administrator;

public class frm_FilterSelect : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private CheckEditEx chk_DissMiss;

	private RadioGroupEx rdo_Select;

	private SimpleButtonEx btn_OK;

	public string UserName { get; set; }

	public string ObjType { get; set; }

	public string Selected { get; set; }

	public DataTable Source { get; set; }

	public frm_FilterSelect()
	{
		InitializeComponent();
	}

	private async void frm_FilterSelect_Load(object sender, EventArgs e)
	{
		try
		{
			if (!Source.IsNotNull())
			{
				return;
			}
			foreach (DataRow row in Source.Rows)
			{
				string description = Function.ToString(row["Name"]);
				string value = Function.ToString(row["Code"]);
				RadioGroupItem radioGroupItem = new RadioGroupItem();
				radioGroupItem.Value = value;
				radioGroupItem.Description = description;
				rdo_Select.Properties.Items.Add(radioGroupItem);
				rdo_Select.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			Selected = Function.ToString(rdo_Select.EditValue);
			if (string.IsNullOrEmpty(Selected))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseItem, "");
				return;
			}
			if (chk_DissMiss.Checked)
			{
				if (GlobalData.FilterSelect == null)
				{
					GlobalData.FilterSelect = new List<Common.ListItems>();
				}
				GlobalData.FilterSelect.Add(new Common.ListItems
				{
					Value = Selected,
					Text = ObjType
				});
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_FilterSelect_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			Selected = Function.ToString(rdo_Select.EditValue);
			if (string.IsNullOrEmpty(Selected))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseItem, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Return)
			{
				btn_OK_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_FilterSelect));
		this.chk_DissMiss = new Apzon.Controls.Editor.CheckEditEx();
		this.rdo_Select = new Apzon.Controls.Editor.RadioGroupEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.chk_DissMiss.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_Select.Properties).BeginInit();
		base.SuspendLayout();
		this.chk_DissMiss.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_DissMiss.Location = new System.Drawing.Point(12, 107);
		this.chk_DissMiss.Name = "chk_DissMiss";
		this.chk_DissMiss.Properties.Caption = "Don't show this message again";
		this.chk_DissMiss.Size = new System.Drawing.Size(187, 19);
		this.chk_DissMiss.TabIndex = 0;
		this.chk_DissMiss.Visible = false;
		this.rdo_Select.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rdo_Select.Location = new System.Drawing.Point(12, 13);
		this.rdo_Select.Name = "rdo_Select";
		this.rdo_Select.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdo_Select.Properties.Appearance.Options.UseBackColor = true;
		this.rdo_Select.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdo_Select.Size = new System.Drawing.Size(285, 88);
		this.rdo_Select.TabIndex = 1;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Image = (System.Drawing.Image)resources.GetObject("btn_OK.Image");
		this.btn_OK.Location = new System.Drawing.Point(12, 132);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(80, 20);
		this.btn_OK.TabIndex = 2;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(309, 154);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.rdo_Select);
		base.Controls.Add(this.chk_DissMiss);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_FilterSelect";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Select Filter";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FilterSelect_FormClosing);
		base.Load += new System.EventHandler(this.frm_FilterSelect_Load);
		((System.ComponentModel.ISupportInitialize)this.chk_DissMiss.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdo_Select.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
