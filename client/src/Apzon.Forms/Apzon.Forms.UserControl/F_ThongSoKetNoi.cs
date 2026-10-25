using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using Newtonsoft.Json;

namespace Apzon.Forms.UserControl;

public class F_ThongSoKetNoi : XtraFormEx
{
	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txt_MayChu;

	private TextEditEx txt_TenCSDL;

	private TextEditEx txt_TaiKhoan;

	private TextEditEx txt_MatKhau;

	private SimpleButtonEx btUpdate;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCheckConnection;

	private SimpleButtonEx btCancel;

	public F_ThongSoKetNoi()
	{
		InitializeComponent();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void F_ThongSoKetNoi_Load(object sender, EventArgs e)
	{
		GetConfig();
	}

	private async void GetConfig()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.GetAsync(Function.GetApiUri(Configuration.ApiServer, "ConfigConnect", "GetConfig"));
			if (response.IsSuccessStatusCode)
			{
				Function.ConfigData configData = JsonConvert.DeserializeObject<Function.ConfigData>(await response.Content.ReadAsStringAsync());
				txt_MayChu.Text = configData.Server;
				txt_TenCSDL.Text = configData.Data;
				txt_TaiKhoan.Text = configData.User;
				txt_MatKhau.Text = configData.Pass;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btUpdate_Click(object sender, EventArgs e)
	{
		try
		{
			Function.ConfigData configData = new Function.ConfigData();
			configData.Data = txt_TenCSDL.Text;
			configData.Server = txt_MayChu.Text;
			configData.User = txt_TaiKhoan.Text;
			configData.Pass = txt_MatKhau.Text;
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Content = new ObjectContent<Function.ConfigData>(configData, new JsonMediaTypeFormatter());
			httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "ConfigConnect", "SaveConfig"));
			httpRequestMessage.Method = HttpMethod.Post;
			new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					task.Result.Content.ReadAsStringAsync();
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void btCheckConnection_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!ValidateForm())
			{
				return;
			}
			WinCommons._OpenProcessing();
			string sql = "Data Source=" + txt_MayChu.Text + ";Initial Catalog=" + txt_TenCSDL.Text + ";Persist Security Info=True;User ID=" + txt_TaiKhoan.Text + ";Password=" + txt_MatKhau.Text;
			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.GetAsync(Function.GetApiUri(Configuration.ApiServer, "ConfigConnect", "CheckConnected", new
			{
				_sql = sql
			}));
			if (response.IsSuccessStatusCode)
			{
				if (JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync()))
				{
					WinCommons.Success(Enums.MessageCode.ConnectSuccessfully, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ConnectFailed, "");
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool ValidateForm()
	{
		if (txt_MayChu.Text == string.Empty)
		{
			txt_MayChu.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txt_MayChu.ToolTipIconType = ToolTipIconType.Error;
			txt_MayChu.ErrorText = "Can not empty !";
			txt_MayChu.Focus();
			return false;
		}
		if (txt_TenCSDL.Text == string.Empty)
		{
			txt_TenCSDL.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txt_TenCSDL.ToolTipIconType = ToolTipIconType.Error;
			txt_TenCSDL.ErrorText = "Can not empty !";
			txt_TenCSDL.Focus();
			return false;
		}
		if (txt_TaiKhoan.Text == string.Empty)
		{
			txt_TaiKhoan.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txt_TaiKhoan.ToolTipIconType = ToolTipIconType.Error;
			txt_TaiKhoan.ErrorText = "Can not empty !";
			txt_TaiKhoan.Focus();
			return false;
		}
		if (txt_MatKhau.Text == string.Empty)
		{
			txt_MatKhau.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txt_MatKhau.ToolTipIconType = ToolTipIconType.Error;
			txt_MatKhau.ErrorText = "Can not empty !";
			txt_MatKhau.Focus();
			return false;
		}
		return true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControl.F_ThongSoKetNoi));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_MayChu = new Apzon.Controls.Editor.TextEditEx();
		this.txt_TenCSDL = new Apzon.Controls.Editor.TextEditEx();
		this.txt_TaiKhoan = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MatKhau = new Apzon.Controls.Editor.TextEditEx();
		this.btUpdate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCheckConnection = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_MayChu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TenCSDL.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaiKhoan.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MatKhau.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.Location = new System.Drawing.Point(21, 24);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Server Name";
		this.labelLine2.Location = new System.Drawing.Point(21, 50);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Database";
		this.labelLine3.Location = new System.Drawing.Point(21, 76);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "User Name";
		this.labelLine4.Location = new System.Drawing.Point(21, 102);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "Password";
		this.txt_MayChu.Location = new System.Drawing.Point(97, 21);
		this.txt_MayChu.Name = "txt_MayChu";
		this.txt_MayChu.Size = new System.Drawing.Size(174, 20);
		this.txt_MayChu.TabIndex = 1;
		this.txt_TenCSDL.Location = new System.Drawing.Point(97, 47);
		this.txt_TenCSDL.Name = "txt_TenCSDL";
		this.txt_TenCSDL.Size = new System.Drawing.Size(174, 20);
		this.txt_TenCSDL.TabIndex = 1;
		this.txt_TaiKhoan.Location = new System.Drawing.Point(97, 73);
		this.txt_TaiKhoan.Name = "txt_TaiKhoan";
		this.txt_TaiKhoan.Size = new System.Drawing.Size(174, 20);
		this.txt_TaiKhoan.TabIndex = 1;
		this.txt_MatKhau.Location = new System.Drawing.Point(97, 99);
		this.txt_MatKhau.Name = "txt_MatKhau";
		this.txt_MatKhau.Properties.PasswordChar = '*';
		this.txt_MatKhau.Size = new System.Drawing.Size(174, 20);
		this.txt_MatKhau.TabIndex = 1;
		this.btUpdate.Image = (System.Drawing.Image)resources.GetObject("btUpdate.Image");
		this.btUpdate.Location = new System.Drawing.Point(85, 141);
		this.btUpdate.Name = "btUpdate";
		this.btUpdate.Size = new System.Drawing.Size(75, 23);
		this.btUpdate.TabIndex = 2;
		this.btUpdate.Text = "Update";
		this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
		this.btRefresh.Image = (System.Drawing.Image)resources.GetObject("btRefresh.Image");
		this.btRefresh.Location = new System.Drawing.Point(166, 141);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.Size = new System.Drawing.Size(75, 23);
		this.btRefresh.TabIndex = 2;
		this.btRefresh.Text = "Refresh";
		this.btCheckConnection.Image = (System.Drawing.Image)resources.GetObject("btCheckConnection.Image");
		this.btCheckConnection.Location = new System.Drawing.Point(4, 188);
		this.btCheckConnection.Name = "btCheckConnection";
		this.btCheckConnection.Size = new System.Drawing.Size(115, 23);
		this.btCheckConnection.TabIndex = 2;
		this.btCheckConnection.Text = "Check Connection";
		this.btCheckConnection.Click += new System.EventHandler(this.btCheckConnection_Click);
		this.btCancel.Image = (System.Drawing.Image)resources.GetObject("btCancel.Image");
		this.btCancel.Location = new System.Drawing.Point(217, 188);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 2;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(294, 214);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btCheckConnection);
		base.Controls.Add(this.btRefresh);
		base.Controls.Add(this.btUpdate);
		base.Controls.Add(this.txt_MatKhau);
		base.Controls.Add(this.txt_TaiKhoan);
		base.Controls.Add(this.txt_TenCSDL);
		base.Controls.Add(this.txt_MayChu);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MinimizeBox = false;
		base.Name = "F_ThongSoKetNoi";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Database Connection";
		base.Load += new System.EventHandler(this.F_ThongSoKetNoi_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_MayChu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TenCSDL.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TaiKhoan.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MatKhau.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
