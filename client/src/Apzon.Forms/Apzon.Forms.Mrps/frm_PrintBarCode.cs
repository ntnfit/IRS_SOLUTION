using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;

namespace Apzon.Forms.Mrps;

public class frm_PrintBarCode : XtraFormEx
{
	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private TextEditEx txt_Line;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	public SimpleButtonEx btn_Print;

	public SimpleButtonEx btn_Cancel;

	private DateEditEx txt_FromDate;

	private DateEditEx txt_ToDate;

	private ButtonEditEx txt_ItemCode;

	private ButtonEditEx txt_PONo;

	public frm_PrintBarCode()
	{
		InitializeComponent();
	}

	private void frm_PrintBarCode_Load(object sender, EventArgs e)
	{
		InitFormatControl();
		txt_FromDate.EditValue = DateTime.Now;
		txt_ToDate.EditValue = DateTime.Now;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Print_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			string value = WinFuntions.DecryptStringAES(await GetDataReportLayout(), "APZ_RDOC");
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.CannotGetDataReport, "");
				EnableDisableControl(isEnable: true);
				WinCommons._CloseProcessing();
				return;
			}
			XtraReport rp;
			using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
			{
				streamWriter.Write(value);
				streamWriter.Flush();
				rp = XtraReport.FromStream(streamWriter.BaseStream);
			}
			List<Common.ParameterValue> lstParam = new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "fdate",
					Value = Function.ToString(Function.ParseDateTime(txt_FromDate.EditValue))
				},
				new Common.ParameterValue
				{
					Name = "tdate",
					Value = Function.ToString(Function.ParseDateTime(txt_ToDate.EditValue))
				},
				new Common.ParameterValue
				{
					Name = "itmcode",
					Value = Function.ToString(txt_ItemCode.EditValue)
				},
				new Common.ParameterValue
				{
					Name = "line",
					Value = Function.ToString(txt_Line.EditValue)
				},
				new Common.ParameterValue
				{
					Name = "pono",
					Value = Function.ToString(txt_PONo.EditValue)
				},
				new Common.ParameterValue
				{
					Name = "filter",
					Value = "202"
				},
				new Common.ParameterValue
				{
					Name = "cardid",
					Value = ""
				}
			};
			WinFuntions.XtraReportViewer(rp, lstParam);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void txt_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_ItemCode.Text = Function.ToString(frm_CFL2.ChooseItem["ItemCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_PONo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ProductionOrder, false, null, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_PONo.EditValue = Function.ToString(frm_CFL2.ChooseItem["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitFormatControl()
	{
		try
		{
			txt_FromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txt_ToDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> GetDataReportLayout()
	{
		return await HttpClientHelperFactory.GetHttpClient().XtraReportManagerClient.GetDataReportLayoutById(1);
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Print.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_PrintBarCode));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Line = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_PONo = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Line.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PONo.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.Location = new System.Drawing.Point(22, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(119, 20);
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "From Date";
		this.labelLine2.Location = new System.Drawing.Point(22, 41);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(119, 20);
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "To Date";
		this.labelLine3.Location = new System.Drawing.Point(22, 67);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(119, 20);
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Item Code";
		this.txt_Line.Location = new System.Drawing.Point(140, 90);
		this.txt_Line.Name = "txt_Line";
		this.txt_Line.Size = new System.Drawing.Size(158, 20);
		this.txt_Line.TabIndex = 4;
		this.labelLine4.Location = new System.Drawing.Point(22, 93);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(119, 20);
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "Line";
		this.labelLine5.Location = new System.Drawing.Point(22, 119);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.Size = new System.Drawing.Size(119, 20);
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "PO No";
		this.btn_Print.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Print.Appearance.Options.UseBackColor = true;
		this.btn_Print.Image = (System.Drawing.Image)resources.GetObject("btn_Print.Image");
		this.btn_Print.Location = new System.Drawing.Point(35, 158);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.Size = new System.Drawing.Size(99, 20);
		this.btn_Print.TabIndex = 6;
		this.btn_Print.Text = "Print";
		this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(140, 158);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 7;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_FromDate.EditValue = null;
		this.txt_FromDate.Location = new System.Drawing.Point(140, 12);
		this.txt_FromDate.Name = "txt_FromDate";
		this.txt_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Size = new System.Drawing.Size(158, 20);
		this.txt_FromDate.TabIndex = 1;
		this.txt_ToDate.EditValue = null;
		this.txt_ToDate.Location = new System.Drawing.Point(140, 38);
		this.txt_ToDate.Name = "txt_ToDate";
		this.txt_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Size = new System.Drawing.Size(158, 20);
		this.txt_ToDate.TabIndex = 2;
		this.txt_ItemCode.Location = new System.Drawing.Point(140, 64);
		this.txt_ItemCode.Name = "txt_ItemCode";
		this.txt_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ItemCode.Size = new System.Drawing.Size(158, 20);
		this.txt_ItemCode.TabIndex = 3;
		this.txt_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ItemCode_ButtonClick);
		this.txt_PONo.Location = new System.Drawing.Point(140, 116);
		this.txt_PONo.Name = "txt_PONo";
		this.txt_PONo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_PONo.Size = new System.Drawing.Size(158, 20);
		this.txt_PONo.TabIndex = 5;
		this.txt_PONo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_PONo_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(326, 190);
		base.Controls.Add(this.txt_PONo);
		base.Controls.Add(this.txt_ItemCode);
		base.Controls.Add(this.txt_ToDate);
		base.Controls.Add(this.txt_FromDate);
		base.Controls.Add(this.btn_Print);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_Line);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Name = "frm_PrintBarCode";
		base.ShowIcon = false;
		this.Text = "Print BarCode";
		base.Load += new System.EventHandler(this.frm_PrintBarCode_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Line.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PONo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
