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
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;

namespace Apzon.Forms.Commons;

public class frm_TestSMS : Form
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private Label lblStatus;

	private TextEditEx txt_Phone;

	private LabelControl labelControl1;

	private LabelControl labelControl2;

	private MemoEdit memo_Message;

	private SimpleButton simpleButton1;

	private MemoEdit memo_AdMessage;

	private LabelControl labelControl3;

	private LabelControl labelControl4;

	private TextEditEx txt_msgQuota;

	private LabelControl labelControl5;

	private LabelControl labelControl6;

	private TextEditEx txt_CampaignName;

	private SimpleButton simpleButton2;

	private MemoEdit memo_Phones;

	private LabelControl labelControl7;

	private DateEditEx txt_ScheduleDate;

	private HourEdit txt_ScheduleTime;

	public frm_TestSMS()
	{
		InitializeComponent();
		ShowStatusBar(string.Empty, type: true);
	}

	public void ShowStatusBar(string message, bool type)
	{
		if (message.Trim() == string.Empty)
		{
			lblStatus.Visible = false;
			return;
		}
		lblStatus.Visible = true;
		lblStatus.Text = message;
		lblStatus.ForeColor = Color.White;
		if (type)
		{
			lblStatus.BackColor = Color.FromArgb(79, 208, 154);
		}
		else
		{
			lblStatus.BackColor = Color.FromArgb(230, 112, 134);
		}
	}

	private async void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			string phoneNumber = Function.ToString(txt_Phone.EditValue);
			string message = Function.ToString(memo_Message.EditValue);
			string text = await _httpClient.SmsClient.SendOTPMessage("DuhocAAU", phoneNumber, message);
			if (string.IsNullOrWhiteSpace(text))
			{
				ShowStatusBar("Gửi tin thành thông!", type: true);
			}
			else
			{
				ShowStatusBar(text, type: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Master_Load(object sender, EventArgs e)
	{
	}

	private async void simpleButton2_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string phone = Function.ToString(txt_Phone.EditValue);
			string message = Function.ToString(memo_AdMessage.EditValue);
			string campaignName = Function.ToString(txt_CampaignName.EditValue);
			TimeSpan value = Function.ParseIntToTimeSpan(Function.ParseInt(txt_ScheduleTime.EditValue));
			string scheduleTime = Function.ParseDateTimes(txt_ScheduleDate.EditValue).Add(value).ToString("yyyy-MM-dd HH:mm");
			int quota = Function.ParseInt(txt_msgQuota.EditValue);
			string text = await _httpClient.SmsClient.CreateCampagin(campaignName, "DuhocAAU", message, scheduleTime, quota);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Error(Enums.MessageCode.None, "Không lấy được campaignCode!");
				return;
			}
			string text2 = await _httpClient.SmsClient.SendCampaignAd(text, phone.Split(','));
			if (string.IsNullOrWhiteSpace(text2))
			{
				ShowStatusBar("Gửi tin thành thông", type: true);
				WinCommons.Success(Enums.MessageCode.None, "Gửi tin nhắn thành công!");
			}
			else
			{
				ShowStatusBar(text2, type: false);
				WinCommons.Error(Enums.MessageCode.None, text2);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_TestSMS));
		this.lblStatus = new System.Windows.Forms.Label();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.memo_Message = new DevExpress.XtraEditors.MemoEdit();
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		this.txt_Phone = new Apzon.Controls.Editor.TextEditEx();
		this.memo_AdMessage = new DevExpress.XtraEditors.MemoEdit();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.txt_msgQuota = new Apzon.Controls.Editor.TextEditEx();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.txt_CampaignName = new Apzon.Controls.Editor.TextEditEx();
		this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
		this.memo_Phones = new DevExpress.XtraEditors.MemoEdit();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.txt_ScheduleDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ScheduleTime = new Apzon.Controls.Controls.HourEdit();
		((System.ComponentModel.ISupportInitialize)this.memo_Message.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Phone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_AdMessage.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_msgQuota.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CampaignName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Phones.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleTime.Properties).BeginInit();
		base.SuspendLayout();
		this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.lblStatus.Location = new System.Drawing.Point(0, 402);
		this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
		this.lblStatus.Size = new System.Drawing.Size(815, 25);
		this.lblStatus.TabIndex = 6;
		this.lblStatus.Text = "label3";
		this.labelControl1.Location = new System.Drawing.Point(313, 8);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(62, 13);
		this.labelControl1.TabIndex = 895;
		this.labelControl1.Text = "Số điện thoại";
		this.labelControl2.Location = new System.Drawing.Point(313, 40);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(65, 13);
		this.labelControl2.TabIndex = 896;
		this.labelControl2.Text = "Nội dung SMS";
		this.memo_Message.Location = new System.Drawing.Point(384, 38);
		this.memo_Message.Name = "memo_Message";
		this.memo_Message.Size = new System.Drawing.Size(211, 237);
		this.memo_Message.TabIndex = 897;
		this.simpleButton1.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleButton1.Appearance.Options.UseFont = true;
		this.simpleButton1.Location = new System.Drawing.Point(432, 290);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(163, 36);
		this.simpleButton1.TabIndex = 898;
		this.simpleButton1.Text = "GỬI";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.txt_Phone.FieldName = null;
		this.txt_Phone.Location = new System.Drawing.Point(384, 7);
		this.txt_Phone.Name = "txt_Phone";
		this.txt_Phone.Size = new System.Drawing.Size(179, 20);
		this.txt_Phone.TabIndex = 893;
		this.memo_AdMessage.Location = new System.Drawing.Point(78, 231);
		this.memo_AdMessage.Name = "memo_AdMessage";
		this.memo_AdMessage.Size = new System.Drawing.Size(211, 108);
		this.memo_AdMessage.TabIndex = 902;
		this.labelControl3.Location = new System.Drawing.Point(7, 233);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(65, 13);
		this.labelControl3.TabIndex = 901;
		this.labelControl3.Text = "Nội dung SMS";
		this.labelControl4.Appearance.Options.UseTextOptions = true;
		this.labelControl4.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControl4.Location = new System.Drawing.Point(10, 104);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(87, 13);
		this.labelControl4.TabIndex = 900;
		this.labelControl4.Text = "Định mức tin nhắn";
		this.txt_msgQuota.FieldName = null;
		this.txt_msgQuota.Location = new System.Drawing.Point(103, 103);
		this.txt_msgQuota.Name = "txt_msgQuota";
		this.txt_msgQuota.Size = new System.Drawing.Size(154, 20);
		this.txt_msgQuota.TabIndex = 899;
		this.labelControl5.Location = new System.Drawing.Point(10, 37);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(51, 13);
		this.labelControl5.TabIndex = 904;
		this.labelControl5.Text = "Lịch gửi tin";
		this.labelControl6.Location = new System.Drawing.Point(10, 11);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(68, 13);
		this.labelControl6.TabIndex = 906;
		this.labelControl6.Text = "Tên chiến dịch";
		this.txt_CampaignName.FieldName = null;
		this.txt_CampaignName.Location = new System.Drawing.Point(78, 10);
		this.txt_CampaignName.Name = "txt_CampaignName";
		this.txt_CampaignName.Size = new System.Drawing.Size(179, 20);
		this.txt_CampaignName.TabIndex = 905;
		this.simpleButton2.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.simpleButton2.Appearance.Options.UseFont = true;
		this.simpleButton2.Location = new System.Drawing.Point(32, 359);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.Size = new System.Drawing.Size(225, 36);
		this.simpleButton2.TabIndex = 907;
		this.simpleButton2.Text = "Tạo Campaign và gửi";
		this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
		this.memo_Phones.Location = new System.Drawing.Point(78, 129);
		this.memo_Phones.Name = "memo_Phones";
		this.memo_Phones.Size = new System.Drawing.Size(211, 96);
		this.memo_Phones.TabIndex = 908;
		this.labelControl7.Location = new System.Drawing.Point(6, 130);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(62, 13);
		this.labelControl7.TabIndex = 909;
		this.labelControl7.Text = "Số điện thoại";
		this.txt_ScheduleDate.EditValue = null;
		this.txt_ScheduleDate.FieldName = null;
		this.txt_ScheduleDate.Location = new System.Drawing.Point(78, 36);
		this.txt_ScheduleDate.Name = "txt_ScheduleDate";
		this.txt_ScheduleDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ScheduleDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ScheduleDate.Properties.Mask.EditMask = "";
		this.txt_ScheduleDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_ScheduleDate.Size = new System.Drawing.Size(179, 20);
		this.txt_ScheduleDate.TabIndex = 903;
		this.txt_ScheduleTime.FieldName = null;
		this.txt_ScheduleTime.Location = new System.Drawing.Point(157, 62);
		this.txt_ScheduleTime.Name = "txt_ScheduleTime";
		this.txt_ScheduleTime.Properties.MaxLength = 5;
		this.txt_ScheduleTime.Size = new System.Drawing.Size(100, 20);
		this.txt_ScheduleTime.TabIndex = 910;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(815, 427);
		base.Controls.Add(this.labelControl7);
		base.Controls.Add(this.memo_Phones);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.labelControl6);
		base.Controls.Add(this.txt_CampaignName);
		base.Controls.Add(this.labelControl5);
		base.Controls.Add(this.memo_AdMessage);
		base.Controls.Add(this.labelControl3);
		base.Controls.Add(this.labelControl4);
		base.Controls.Add(this.txt_msgQuota);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.memo_Message);
		base.Controls.Add(this.labelControl2);
		base.Controls.Add(this.labelControl1);
		base.Controls.Add(this.txt_Phone);
		base.Controls.Add(this.lblStatus);
		base.Controls.Add(this.txt_ScheduleDate);
		base.Controls.Add(this.txt_ScheduleTime);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(615, 425);
		base.Name = "frm_TestSMS";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Test SMS";
		base.Load += new System.EventHandler(this.Master_Load);
		((System.ComponentModel.ISupportInitialize)this.memo_Message.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Phone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_AdMessage.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_msgQuota.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CampaignName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Phones.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ScheduleTime.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
