using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.DataProcessing;
using DevExpress.XtraBars.Alerter;

namespace Apzon.Controls.Controls;

public class AlertControlEx : AlertControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ClearNotification ClearNotificationForm;

	private AlertButton _markAsReadButton;

	private IContainer components;

	public AlertControlEx(IContainer container)
		: base(container)
	{
		try
		{
			if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				ComponentResourceManager componentResourceManager = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.Notification, Apzon.Commons"));
				_markAsReadButton = new AlertButton((Image)componentResourceManager.GetObject("unread_icon"));
				_markAsReadButton.Style = AlertButtonStyle.CheckButton;
				_markAsReadButton.Down = true;
				_markAsReadButton.Hint = "Mark as Read";
				_markAsReadButton.Name = "buttonAlert";
				AlertButton item = new AlertButton((Image)componentResourceManager.GetObject("dismissAll"))
				{
					Style = AlertButtonStyle.Button,
					Down = true,
					Hint = "Dismiss All",
					Name = "baseDismissAll"
				};
				ButtonClick += DismissAll_Click;
				Buttons.Add(item);
				BeforeFormShow += CustomAlertControl_BeforeFormShow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void CustomAlertControl_BeforeFormShow(object sender, AlertFormEventArgs e)
	{
		int height = Screen.PrimaryScreen.Bounds.Height;
		int width = Screen.PrimaryScreen.Bounds.Width;
		if (AlertFormList.Count == 0)
		{
			e.Location = new Point(width - e.AlertForm.Width + (e.AlertForm.Size.Width - e.AlertForm.Width) / 2, e.Location.Y + (e.AlertForm.Size.Height - e.AlertForm.Height) / 2 + (height - 200));
			return;
		}
		int val = AlertFormList[AlertFormList.Count - 1].Location.Y - AlertFormList[AlertFormList.Count - 1].Height;
		val = Math.Max(0, val);
		e.Location = new Point(width - e.AlertForm.Width + (e.AlertForm.Size.Width - e.AlertForm.Width) / 2, val + (height - 200));
	}

	protected override AlertForm CreateAlertForm(Point location, AlertControl control, AlertInfo info)
	{
		return new AlertFormEx(location, control, info);
	}

	public void DismissAll_Click(object sender, AlertButtonClickEventArgs e)
	{
		try
		{
			if (e.ButtonName == "buttonAlert")
			{
				if (e.Info.Tag is NotifiedObj notifiedObj)
				{
					if (ClearNotificationForm != null)
					{
						ClearNotificationForm(notifiedObj.ObjectTypes, notifiedObj.NotifiedId, notifiedObj.TranType);
					}
					_httpClient.SubActivities.MarkAsRead(notifiedObj.CmtCode);
					e.AlertForm.Close();
				}
			}
			else
			{
				if (!(e.ButtonName == "baseDismissAll") || AlertFormList == null || AlertFormList.Count <= 0)
				{
					return;
				}
				for (int i = 0; i < AlertFormList.Count; i++)
				{
					NotifiedObj notifiedObj2 = AlertFormList[i].AlertInfo.Tag as NotifiedObj;
					if (ClearNotificationForm != null && notifiedObj2 != null)
					{
						ClearNotificationForm(notifiedObj2.ObjectTypes, notifiedObj2.NotifiedId, notifiedObj2.TranType);
					}
					AlertFormList[i].Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public void ShowAlert(System.Windows.Forms.Form form, AlertInfo info, Enums.TransactionType trantype = Enums.TransactionType.A)
	{
		AlertButton alertButton = Buttons.Cast<AlertButton>().FirstOrDefault((AlertButton t) => t.Name.Equals("buttonAlert"));
		if (alertButton != null)
		{
			Buttons.RemoveAt(Buttons.Cast<AlertButton>().IndexOf(alertButton));
		}
		AlertButton alertButton2 = Buttons.Cast<AlertButton>().FirstOrDefault((AlertButton t) => t.Name.Equals("buttonAlert"));
		if (trantype == Enums.TransactionType.M)
		{
			if (alertButton2 == null)
			{
				Buttons.Add(_markAsReadButton);
			}
		}
		else if (alertButton2 != null)
		{
			Buttons.RemoveAt(Buttons.Cast<AlertButton>().IndexOf(alertButton2));
		}
		Show(form, info);
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
		components = new Container();
	}
}
