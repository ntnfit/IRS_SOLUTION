using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.PM;

public class frm_ChangeStatus : XtraFormEx
{
	public string status;

	public int issueId;

	public string _oldValue;

	public Func<int, string, string, Task> SendDataBackToMain;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine labelLine9;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Save;

	private MemoEditEx txt_Comment;

	public LookUpEditDefineNew luk_Status;

	public frm_ChangeStatus()
	{
		InitializeComponent();
		OnLoadAsync += frm_ChangeStatus_Load;
	}

	private async Task frm_ChangeStatus_Load(object sender, EventArgs e)
	{
		try
		{
			luk_Status.Properties.ValueMember = "Code";
			luk_Status.Properties.DisplayMember = "Name";
			luk_Status.Properties.FormProperties.TableName = "APZ_OTIS";
			await luk_Status.Properties.RefreshDataSourceAsync();
			luk_Status.EditValue = status;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Status_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			Function.ToString(e.NewValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<string> DataStaus(string Id)
	{
		try
		{
			DataTable dataTable = await _httpClient.TrackingIssue.GetDataStatusById(Id);
			string result = "";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0]["Name"]);
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			int id = Function.ParseInt(issueId);
			string status = Function.ToString(luk_Status.EditValue);
			string comment = Function.ToString(txt_Comment.EditValue);
			string newValue = await DataStaus(status);
			string oldValue = await DataStaus(_oldValue);
			int user = Function.ParseInt(GlobalData.UserID);
			if (!string.IsNullOrEmpty(status) || id > 0)
			{
				DataSet ds = new DataSet();
				DataTable dt = WinFuntions.GetTableStructure("APZ_TGIE1");
				string cm = "Change status to " + await DataStaus(status) + ":" + comment;
				DataRow dataRow = dt.NewRow();
				dataRow["Id"] = id;
				dataRow["LineNum"] = 0;
				dataRow["OriginalValue"] = oldValue;
				dataRow["FieldName"] = "Status";
				dataRow["NewValue"] = newValue;
				dataRow["UserSign"] = user;
				dataRow["TrackingDate"] = DateTime.Now;
				dataRow["ActionDetail"] = "MadeChange";
				dataRow["OldVal"] = _oldValue;
				dataRow["NewVal"] = status;
				dt.Rows.Add(dataRow);
				ds.Tables.Add(dt);
				HttpResult httpResult = await _httpClient.TrackingIssue.UpdateStatus(ds);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					if (SendDataBackToMain != null)
					{
						await SendDataBackToMain(id, status, cm);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.StatusIsNullOrIdIsNull, "");
			}
			Close();
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
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Comment = new Apzon.Controls.Editor.MemoEditEx();
		this.luk_Status = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		((System.ComponentModel.ISupportInitialize)this.txt_Comment.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Save.FieldName = null;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 218);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 39;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 41);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(75, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 37;
		this.labelLine1.Utext = "Comment";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(12, 13);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(75, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 35;
		this.labelLine9.Utext = "Status";
		this.txt_Comment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Comment.FieldName = null;
		this.txt_Comment.IsSearchControl = false;
		this.txt_Comment.Location = new System.Drawing.Point(87, 36);
		this.txt_Comment.Name = "txt_Comment";
		this.txt_Comment.Size = new System.Drawing.Size(450, 178);
		this.txt_Comment.TabIndex = 40;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(87, 10);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.AutoHeight = false;
		this.luk_Status.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.CheckFields = null;
		this.luk_Status.Properties.CheckTables = null;
		this.luk_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Properties.ShowFooter = false;
		this.luk_Status.Properties.ShowHeader = false;
		this.luk_Status.Size = new System.Drawing.Size(181, 20);
		this.luk_Status.TabIndex = 98;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(547, 243);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.txt_Comment);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.labelLine9);
		base.FormId = "oTrackingIssue";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChangeStatus";
		this.Text = "Change Status ";
		((System.ComponentModel.ISupportInitialize)this.txt_Comment.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
