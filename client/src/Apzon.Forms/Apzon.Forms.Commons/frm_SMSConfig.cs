using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Commons;

public class frm_SMSConfig : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine lbl_Endpoint;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private LookUpEditEx luk_Endpoint;

	private TextEditEx txt_ClientID;

	private TextEditEx txt_ClientSecret;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_SMSConfig()
	{
		InitializeComponent();
		OnLoadAsync += frm_SMSConfig_Load;
		luk_Endpoint.EditValueChanged += OnContentChanged;
		txt_ClientID.EditValueChanged += OnContentChanged;
		txt_ClientSecret.EditValueChanged += OnContentChanged;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData()
	{
		try
		{
			DataTable dt = DataSource as DataTable;
			string text = await _httpClient.SmsClient.SaveConfig(dt);
			if (string.IsNullOrWhiteSpace(text))
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_SMSConfig_Load(object sender, EventArgs e)
	{
		LoadInitForm();
		await LoadData();
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "S",
					Name = "Sandbox"
				},
				new Common.Table_Code
				{
					Code = "L",
					Name = "Live"
				}
			};
			SetDataSourceByList(luk_Endpoint, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await _httpClient.SmsClient.GetConfig();
			if (!dataTable.IsNotNull())
			{
				dataTable.Rows.Add(dataTable.NewRow());
			}
			DataSource = dataTable;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataSourceByList(LookUpEditEx luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.Properties.DataSource = list;
			luk.Properties.DisplayMember = "Name";
			luk.Properties.ValueMember = "Code";
			luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.Properties.ForceInitialize();
			luk.Properties.PopulateColumns();
			luk.Properties.Columns["Code"].Visible = false;
			luk.Properties.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_SMSConfig));
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Endpoint = new Apzon.Controls.Controls.LabelLine();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Endpoint = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_ClientID = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ClientSecret = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.luk_Endpoint.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientSecret.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine2.FieldName = "ClientSecret";
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(5, 59);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(96, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 10;
		this.labelLine2.Utext = "Client Secret";
		this.labelLine1.FieldName = "ClientID";
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 32);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(96, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Client ID";
		this.lbl_Endpoint.FieldName = "MachineID";
		this.lbl_Endpoint.HideWhenEmpty = false;
		this.lbl_Endpoint.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Endpoint.Location = new System.Drawing.Point(5, 5);
		this.lbl_Endpoint.Name = "lbl_Endpoint";
		this.lbl_Endpoint.ShowUnderLine = true;
		this.lbl_Endpoint.Size = new System.Drawing.Size(96, 20);
		this.lbl_Endpoint.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Endpoint.TabIndex = 6;
		this.lbl_Endpoint.Utext = "API Endpoint Type";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 90);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(75, 20);
		this.btn_Save.TabIndex = 14;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(82, 90);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 15;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.luk_Endpoint.FieldName = "EndpointType";
		this.luk_Endpoint.IsSearchControl = false;
		this.luk_Endpoint.Location = new System.Drawing.Point(102, 3);
		this.luk_Endpoint.Name = "luk_Endpoint";
		this.luk_Endpoint.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Endpoint.Properties.NullText = "";
		this.luk_Endpoint.Size = new System.Drawing.Size(125, 20);
		this.luk_Endpoint.TabIndex = 16;
		this.txt_ClientID.FieldName = "ClientID";
		this.txt_ClientID.IsSearchControl = false;
		this.txt_ClientID.Location = new System.Drawing.Point(102, 29);
		this.txt_ClientID.Name = "txt_ClientID";
		this.txt_ClientID.Size = new System.Drawing.Size(249, 20);
		this.txt_ClientID.TabIndex = 17;
		this.txt_ClientSecret.FieldName = "ClientSecret";
		this.txt_ClientSecret.IsSearchControl = false;
		this.txt_ClientSecret.Location = new System.Drawing.Point(102, 55);
		this.txt_ClientSecret.Name = "txt_ClientSecret";
		this.txt_ClientSecret.Size = new System.Drawing.Size(249, 20);
		this.txt_ClientSecret.TabIndex = 18;
		base.Appearance.Options.UseFont = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(352, 113);
		base.Controls.Add(this.txt_ClientSecret);
		base.Controls.Add(this.txt_ClientID);
		base.Controls.Add(this.luk_Endpoint);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lbl_Endpoint);
		this.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_SMSConfig.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SMSConfig";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "SMS Configuration (Fpt Api)";
		((System.ComponentModel.ISupportInitialize)this.luk_Endpoint.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientSecret.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
