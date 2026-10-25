using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;

namespace Apzon.Forms.Reports;

public class frm_XtraParameterSubmit : XtraFormEx
{
	public ParameterCollection lstPara = new ParameterCollection();

	public List<Common.ParameterValue> lstParamValue = new List<Common.ParameterValue>();

	public string _pathFile;

	public string _txtReport;

	public bool isUserEdit;

	private IContainer components;

	public frm_XtraParameterSubmit()
	{
		InitializeComponent();
	}

	private void frm_XtraParameterSubmit_Load(object sender, EventArgs e)
	{
		try
		{
			if (lstPara.Count <= 0)
			{
				return;
			}
			int num = 0;
			int num2 = 20;
			int num3 = 20;
			int num4 = 150;
			int num5 = 20;
			foreach (Parameter item in lstPara)
			{
				if (!item.Visible)
				{
					continue;
				}
				string name = item.Name;
				string description = item.Description;
				Type type = item.Type;
				_ = item.MultiValue;
				_ = item.LookUpSettings;
				object value = item.Value;
				LabelControl labelControl = new LabelControl();
				labelControl.Name = "lbl_" + name;
				labelControl.Text = description;
				labelControl.Location = new Point(num2, num3);
				Controls.Add(labelControl);
				if (type == typeof(int))
				{
					CalcEdit calcEdit = new CalcEdit();
					calcEdit.Name = name;
					calcEdit.Tag = description;
					calcEdit.ToolTip = description;
					calcEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					calcEdit.TabIndex = num;
					calcEdit.EditValue = value;
					calcEdit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEdit.Properties.DisplayFormat.FormatString = "n0";
					calcEdit.Properties.EditFormat.FormatType = FormatType.Numeric;
					calcEdit.Properties.EditFormat.FormatString = "n0";
					calcEdit.Properties.EditMask = "n0";
					num++;
					calcEdit.Location = new Point(num4, num5);
					calcEdit.Properties.AllowMouseWheel = false;
					calcEdit.Properties.Buttons.Clear();
					Controls.Add(calcEdit);
				}
				else if (type == typeof(bool))
				{
					CheckEdit checkEdit = new CheckEdit();
					checkEdit.Location = new Point(num4, num5);
					checkEdit.Name = name;
					checkEdit.Tag = item.Name;
					checkEdit.Text = "";
					checkEdit.TabIndex = num;
					num++;
					Controls.Add(checkEdit);
				}
				else if (type == typeof(DateTime))
				{
					DateEditEx dateEditEx = new DateEditEx();
					dateEditEx.Location = new Point(num4, num5);
					dateEditEx.Name = name;
					dateEditEx.Tag = description;
					dateEditEx.ToolTip = description;
					dateEditEx.TabIndex = num;
					num++;
					if (value != null && DateTime.MinValue != (DateTime)value)
					{
						dateEditEx.EditValue = value;
					}
					else
					{
						dateEditEx.EditValue = DateTime.Today;
					}
					Controls.Add(dateEditEx);
				}
				else if (type == typeof(string))
				{
					TextEdit textEdit = new TextEdit();
					textEdit.Location = new Point(num4, num5);
					textEdit.Name = name;
					textEdit.Tag = item.Name;
					textEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					textEdit.Text = "";
					textEdit.TabIndex = num;
					textEdit.EditValue = value;
					num++;
					Controls.Add(textEdit);
				}
				else if (type == typeof(decimal) || type == typeof(double))
				{
					CalcEdit calcEdit2 = new CalcEdit();
					calcEdit2.Name = name;
					calcEdit2.Tag = description;
					calcEdit2.ToolTip = description;
					calcEdit2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					calcEdit2.TabIndex = num;
					calcEdit2.EditValue = value;
					num++;
					calcEdit2.Location = new Point(num4, num5);
					calcEdit2.Properties.AllowMouseWheel = false;
					calcEdit2.Properties.Buttons.Clear();
					Controls.Add(calcEdit2);
				}
				num3 += 25;
				num5 += 25;
			}
			SimpleButton simpleButton = new SimpleButton();
			simpleButton.Image = Resources.apply_16x16;
			simpleButton.Name = "btn_Ok";
			simpleButton.Text = "OK";
			simpleButton.Location = new Point(num2, num3);
			simpleButton.Click += btn_Ok_Click;
			AcceptButton = simpleButton;
			SimpleButton simpleButton2 = new SimpleButton();
			simpleButton2.Image = Resources.cancel_16x16;
			simpleButton2.Name = "btn_Cancel";
			simpleButton2.Text = "Cancel";
			simpleButton2.Location = new Point(num4, num5);
			simpleButton2.Click += btnCancel_Click;
			CancelButton = simpleButton2;
			num3 += 25;
			num5 += 25;
			Controls.Add(simpleButton);
			Controls.Add(simpleButton2);
			Size = new Size(300, 50 + num5);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			lstParamValue.Clear();
			foreach (Control control in Controls)
			{
				Type type = control.GetType();
				if (!type.Name.Equals("LabelControl"))
				{
					switch (type.Name)
					{
					case "CalcEdit":
					{
						CalcEdit calcEdit = (CalcEdit)control;
						Common.ParameterValue parameterValue4 = new Common.ParameterValue();
						parameterValue4.Value = calcEdit.EditValue;
						parameterValue4.Name = calcEdit.Name;
						lstParamValue.Add(parameterValue4);
						break;
					}
					case "CheckEdit":
					{
						CheckEdit checkEdit = (CheckEdit)control;
						Common.ParameterValue parameterValue3 = new Common.ParameterValue();
						parameterValue3.Value = Function.ParseBool(checkEdit.Checked);
						parameterValue3.Name = checkEdit.Name;
						lstParamValue.Add(parameterValue3);
						break;
					}
					case "DateEditEx":
					{
						DateEditEx dateEditEx = (DateEditEx)control;
						Common.ParameterValue parameterValue2 = new Common.ParameterValue();
						parameterValue2.Value = dateEditEx.EditValue;
						parameterValue2.Name = dateEditEx.Name;
						lstParamValue.Add(parameterValue2);
						break;
					}
					case "TextEdit":
					{
						TextEdit textEdit = (TextEdit)control;
						Common.ParameterValue parameterValue = new Common.ParameterValue();
						parameterValue.Value = textEdit.EditValue;
						parameterValue.Name = textEdit.Name;
						lstParamValue.Add(parameterValue);
						break;
					}
					}
				}
			}
			if (lstParamValue.Count > 0)
			{
				isUserEdit = true;
			}
			XtraReport xtraReport = new XtraReport();
			if (!string.IsNullOrEmpty(_txtReport))
			{
				using StreamWriter streamWriter = new StreamWriter(new MemoryStream());
				streamWriter.Write(_txtReport);
				streamWriter.Flush();
				xtraReport = XtraReport.FromStream(streamWriter.BaseStream);
			}
			else if (!string.IsNullOrEmpty(_pathFile))
			{
				xtraReport.LoadLayout(_pathFile);
				MemoryStream stream = new MemoryStream();
				xtraReport.SaveLayout(stream);
			}
			WinFuntions.XtraReportViewer(xtraReport, lstParamValue);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 278);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_SelecttionCriteria";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Selecttion Criteria";
		base.Load += new System.EventHandler(this.frm_XtraParameterSubmit_Load);
		base.ResumeLayout(false);
	}
}
