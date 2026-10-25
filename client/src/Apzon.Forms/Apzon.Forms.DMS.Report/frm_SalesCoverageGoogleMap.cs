using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CefSharp.WinForms;

namespace Apzon.Forms.DMS.Report;

public class frm_SalesCoverageGoogleMap : XtraFormEx
{
	public ChromiumWebBrowser browser;

	private IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public async Task InitBrowser()
	{
		try
		{
			browser = new ChromiumWebBrowser(await httpClient.Commons.GetGoogleMap() + "?uid=" + GlobalData.SessionId);
			Controls.Add(browser);
			browser.Dock = DockStyle.Fill;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_SalesCoverageGoogleMap()
	{
		InitializeComponent();
	}

	private async Task frm_SalesCoverageGoogleMap_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			await InitBrowser();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
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
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalesCoverageGoogleMap";
		base.ShowInTaskbar = false;
		this.Text = "Sales Coverage";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_SalesCoverageGoogleMap_Load);
		base.ResumeLayout(false);
	}
}
