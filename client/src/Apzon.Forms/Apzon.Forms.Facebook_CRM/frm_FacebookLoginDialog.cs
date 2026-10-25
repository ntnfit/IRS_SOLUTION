using System;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Facebook;

namespace Apzon.Forms.Facebook_CRM;

public class frm_FacebookLoginDialog : XtraForm
{
	private readonly Uri _loginUrl;

	protected FacebookClient _fb;

	private IContainer components;

	private WebBrowser webBrowser1;

	public FacebookOAuthResult FacebookOAuthResult { get; private set; }

	public frm_FacebookLoginDialog(string appId, string extendedPermissions)
		: this(new FacebookClient(), appId, extendedPermissions)
	{
	}

	public frm_FacebookLoginDialog(FacebookClient fb, string appId, string extendedPermissions)
	{
		if (fb == null)
		{
			throw new ArgumentNullException("fb");
		}
		if (string.IsNullOrWhiteSpace(appId))
		{
			throw new ArgumentNullException("appId");
		}
		_fb = fb;
		_loginUrl = GenerateLoginUrl(appId, extendedPermissions);
		InitializeComponent();
	}

	private Uri GenerateLoginUrl(string appId, string extendedPermissions)
	{
		dynamic val = new ExpandoObject();
		val.client_id = appId;
		val.redirect_uri = "https://www.facebook.com/connect/login_success.html";
		val.response_type = "token";
		val.display = "popup";
		if (!string.IsNullOrWhiteSpace(extendedPermissions))
		{
			val.scope = extendedPermissions;
		}
		return _fb.GetLoginUrl(val);
	}

	private void FacebookLoginDialog_Load(object sender, EventArgs e)
	{
		webBrowser1.Navigate(_loginUrl.AbsoluteUri);
	}

	private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
	{
		if (_fb.TryParseOAuthCallbackUrl(e.Url, out var facebookOAuthResult))
		{
			FacebookOAuthResult = facebookOAuthResult;
			DialogResult = (FacebookOAuthResult.IsSuccess ? DialogResult.OK : DialogResult.No);
		}
		else
		{
			FacebookOAuthResult = null;
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
		this.webBrowser1 = new System.Windows.Forms.WebBrowser();
		base.SuspendLayout();
		this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser1.Location = new System.Drawing.Point(0, 0);
		this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser1.Name = "webBrowser1";
		this.webBrowser1.Size = new System.Drawing.Size(661, 314);
		this.webBrowser1.TabIndex = 0;
		this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser1_Navigated);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(661, 314);
		base.Controls.Add(this.webBrowser1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FacebookLoginDialog";
		this.Text = "Login to Facebook";
		base.Load += new System.EventHandler(this.FacebookLoginDialog_Load);
		base.ResumeLayout(false);
	}
}
