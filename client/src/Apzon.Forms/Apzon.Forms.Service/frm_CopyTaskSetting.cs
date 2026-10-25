using System;
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
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Service;

public class frm_CopyTaskSetting : XtraFormEx
{
	public bool CopyTask;

	private DataRow copyRow;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	public RadioGroupEx rgp_DueDate;

	public CheckEditEx chk_IsSubTask;

	public TextEditEx txtParentTask;

	public TextEditEx txtDetails;

	public CheckEditEx chk_CopySubTask;

	public CheckEditEx chk_CopySubTaskOfSubTask;

	public CheckEditEx chk_CopyAssignee;

	public CheckEditEx chk_CopyProject;

	public CheckEditEx chk_CopyAttachment;

	public CheckEditEx chk_CopyCompletedSubTask;

	public frm_CopyTaskSetting(DataRow sendRow)
	{
		InitializeComponent();
		try
		{
			copyRow = sendRow;
			txtDetails.EditValue = Function.ToString(copyRow["Details"]) + " (Copy)";
			int num = Function.ParseInt(copyRow["parentTask"]);
			if (num != 0)
			{
				chk_IsSubTask.Checked = true;
				txtParentTask.EditValue = num;
			}
			else
			{
				chk_IsSubTask.Checked = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_CopyTaskSetting_Load(object sender, EventArgs e)
	{
		try
		{
			ActiveControl = txtDetails;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_IsSubTask_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			CheckEditEx checkEditEx = chk_IsSubTask;
			if (checkEditEx.Checked)
			{
				txtParentTask.Visible = true;
			}
			if (!checkEditEx.Checked)
			{
				txtParentTask.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		CopyTask = true;
		Close();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		if (keyData == Keys.Return)
		{
			CopyTask = true;
			Close();
			return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void txtParentTask_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!(Function.ToString(txtParentTask.EditValue) == ""))
			{
				int num = Function.ParseInt(txtParentTask.EditValue);
				string userNameLogin = GlobalData.UserNameLogin;
				if (_httpClient.SubActivities.LoadSingleTaskSync(num, userNameLogin).EmptyData())
				{
					e.Cancel = true;
					WinCommons.Error(Enums.MessageCode.TaskDoesNotExistsOrNotAccess, "", Function.ToString(num));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		this.rgp_DueDate = new Apzon.Controls.Editor.RadioGroupEx();
		this.chk_IsSubTask = new Apzon.Controls.Editor.CheckEditEx();
		this.txtParentTask = new Apzon.Controls.Editor.TextEditEx();
		this.txtDetails = new Apzon.Controls.Editor.TextEditEx();
		this.chk_CopySubTask = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CopySubTaskOfSubTask = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CopyAssignee = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CopyProject = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CopyAttachment = new Apzon.Controls.Editor.CheckEditEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_CopyCompletedSubTask = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.rgp_DueDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsSubTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtParentTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopySubTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopySubTaskOfSubTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyAssignee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyProject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyAttachment.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyCompletedSubTask.Properties).BeginInit();
		base.SuspendLayout();
		this.rgp_DueDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rgp_DueDate.EditValue = 0;
		this.rgp_DueDate.Location = new System.Drawing.Point(12, 41);
		this.rgp_DueDate.Name = "rgp_DueDate";
		this.rgp_DueDate.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.rgp_DueDate.Properties.Appearance.Options.UseBackColor = true;
		this.rgp_DueDate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.rgp_DueDate.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Recalculate Due Date and Due Time"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "Recalculate Due Date Only"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(3, "Reset Due Date")
		});
		this.rgp_DueDate.Size = new System.Drawing.Size(308, 85);
		this.rgp_DueDate.TabIndex = 12;
		this.chk_IsSubTask.Location = new System.Drawing.Point(12, 283);
		this.chk_IsSubTask.Name = "chk_IsSubTask";
		this.chk_IsSubTask.Properties.Caption = "Is Sub Task of";
		this.chk_IsSubTask.Size = new System.Drawing.Size(92, 19);
		this.chk_IsSubTask.TabIndex = 14;
		this.chk_IsSubTask.CheckedChanged += new System.EventHandler(this.chk_IsSubTask_CheckedChanged);
		this.txtParentTask.Location = new System.Drawing.Point(110, 284);
		this.txtParentTask.Name = "txtParentTask";
		this.txtParentTask.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7.25f);
		this.txtParentTask.Properties.Appearance.Options.UseFont = true;
		this.txtParentTask.Properties.NullValuePrompt = "Parent Task ID...";
		this.txtParentTask.Size = new System.Drawing.Size(100, 18);
		this.txtParentTask.TabIndex = 15;
		this.txtParentTask.ToolTip = "Type Task ID here. Hover on the creation time text to see Task's ID";
		this.txtParentTask.Visible = false;
		this.txtParentTask.Validating += new System.ComponentModel.CancelEventHandler(this.txtParentTask_Validating);
		this.txtDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtDetails.EditValue = "New copied Task (Copy)";
		this.txtDetails.Location = new System.Drawing.Point(12, 10);
		this.txtDetails.Name = "txtDetails";
		this.txtDetails.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.txtDetails.Properties.Appearance.Options.UseFont = true;
		this.txtDetails.Size = new System.Drawing.Size(308, 20);
		this.txtDetails.TabIndex = 16;
		this.chk_CopySubTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CopySubTask.EditValue = true;
		this.chk_CopySubTask.Location = new System.Drawing.Point(12, 208);
		this.chk_CopySubTask.Name = "chk_CopySubTask";
		this.chk_CopySubTask.Properties.Caption = "Keep Sub Tasks";
		this.chk_CopySubTask.Size = new System.Drawing.Size(308, 19);
		this.chk_CopySubTask.TabIndex = 17;
		this.chk_CopySubTaskOfSubTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CopySubTaskOfSubTask.EditValue = true;
		this.chk_CopySubTaskOfSubTask.Location = new System.Drawing.Point(12, 233);
		this.chk_CopySubTaskOfSubTask.Name = "chk_CopySubTaskOfSubTask";
		this.chk_CopySubTaskOfSubTask.Properties.Caption = "Keep Sub Tasks of Sub Tasks";
		this.chk_CopySubTaskOfSubTask.Size = new System.Drawing.Size(308, 19);
		this.chk_CopySubTaskOfSubTask.TabIndex = 18;
		this.chk_CopyAssignee.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CopyAssignee.EditValue = true;
		this.chk_CopyAssignee.Location = new System.Drawing.Point(12, 133);
		this.chk_CopyAssignee.Name = "chk_CopyAssignee";
		this.chk_CopyAssignee.Properties.Caption = "Keep Assignee";
		this.chk_CopyAssignee.Size = new System.Drawing.Size(308, 19);
		this.chk_CopyAssignee.TabIndex = 19;
		this.chk_CopyProject.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CopyProject.EditValue = true;
		this.chk_CopyProject.Location = new System.Drawing.Point(12, 158);
		this.chk_CopyProject.Name = "chk_CopyProject";
		this.chk_CopyProject.Properties.Caption = "Keep Projects";
		this.chk_CopyProject.Size = new System.Drawing.Size(308, 19);
		this.chk_CopyProject.TabIndex = 20;
		this.chk_CopyAttachment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.chk_CopyAttachment.EditValue = true;
		this.chk_CopyAttachment.Location = new System.Drawing.Point(12, 183);
		this.chk_CopyAttachment.Name = "chk_CopyAttachment";
		this.chk_CopyAttachment.Properties.Caption = "Keep Attachments";
		this.chk_CopyAttachment.Size = new System.Drawing.Size(308, 19);
		this.chk_CopyAttachment.TabIndex = 21;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Location = new System.Drawing.Point(71, 321);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(53, 23);
		this.btCancel.TabIndex = 11;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.Location = new System.Drawing.Point(12, 321);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(53, 23);
		this.btnOK.TabIndex = 10;
		this.btnOK.Text = "Copy";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.chk_CopyCompletedSubTask.EditValue = true;
		this.chk_CopyCompletedSubTask.Location = new System.Drawing.Point(12, 258);
		this.chk_CopyCompletedSubTask.Name = "chk_CopyCompletedSubTask";
		this.chk_CopyCompletedSubTask.Properties.Caption = "Keep Completed Sub Tasks";
		this.chk_CopyCompletedSubTask.Size = new System.Drawing.Size(308, 19);
		this.chk_CopyCompletedSubTask.TabIndex = 22;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(332, 351);
		base.Controls.Add(this.chk_CopyCompletedSubTask);
		base.Controls.Add(this.chk_CopyAttachment);
		base.Controls.Add(this.chk_CopyProject);
		base.Controls.Add(this.chk_CopyAssignee);
		base.Controls.Add(this.chk_CopySubTaskOfSubTask);
		base.Controls.Add(this.chk_CopySubTask);
		base.Controls.Add(this.txtDetails);
		base.Controls.Add(this.txtParentTask);
		base.Controls.Add(this.chk_IsSubTask);
		base.Controls.Add(this.rgp_DueDate);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.LookAndFeel.SkinName = "Office 2016 Colorful";
		base.LookAndFeel.UseDefaultLookAndFeel = false;
		base.Name = "frm_CopyTaskSetting";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.Load += new System.EventHandler(this.frm_CopyTaskSetting_Load);
		((System.ComponentModel.ISupportInitialize)this.rgp_DueDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsSubTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtParentTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopySubTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopySubTaskOfSubTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyAssignee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyProject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyAttachment.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CopyCompletedSubTask.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
