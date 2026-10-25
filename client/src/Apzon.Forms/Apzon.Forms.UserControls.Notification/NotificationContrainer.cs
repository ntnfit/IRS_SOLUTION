using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.ApprovalProcess;
using Apzon.Forms.Helper;
using Apzon.Forms.PM;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;

namespace Apzon.Forms.UserControls.Notification;

public class NotificationContrainer : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly Color _unreadTextColor = Color.FromArgb(51, 153, 255);

	private IContainer components;

	private GridControl grdNotification;

	private TileView tileViewNotification;

	private TileViewColumn tvc_Title;

	private TileViewColumn tvc_Body;

	private TileViewColumn tvc_DueDate;

	private ToolTipController toolTipController1;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private TileViewColumn tvc_Group;

	private XtraTabControlEx xtraTabControlNotification;

	private XtraTabPageEx xtraTabPageExAll;

	private XtraTabPageEx xtraTabPageExIssue;

	private XtraTabPageEx xtraTabPageExTask;

	private XtraTabPageEx xtraTabPageExComments;

	private XtraTabPageEx xtraTabPageExEvent;

	private XtraTabPageEx xtraTabPageExAlert;

	private GridControl grdIssue;

	private TileView tileViewIssue;

	private TileViewColumn tileViewColumn1;

	private TileViewColumn tileViewColumn2;

	private RepositoryItemMemoEdit repositoryItemMemoEdit2;

	private TileViewColumn tileViewColumn3;

	private TileViewColumn tileViewColumn4;

	private GridControl grdTask;

	private TileView tileViewTask;

	private TileViewColumn tileViewColumn5;

	private TileViewColumn tileViewColumn6;

	private RepositoryItemMemoEdit repositoryItemMemoEdit3;

	private TileViewColumn tileViewColumn7;

	private TileViewColumn tileViewColumn8;

	private GridControl grdEvent;

	private TileView tileViewEvent;

	private TileViewColumn tileViewColumn9;

	private TileViewColumn tileViewColumn10;

	private RepositoryItemMemoEdit repositoryItemMemoEdit4;

	private TileViewColumn tileViewColumn11;

	private TileViewColumn tileViewColumn12;

	private GridControl grdComment;

	private TileView tileViewComment;

	private TileViewColumn tileViewColumn13;

	private TileViewColumn tileViewColumn14;

	private RepositoryItemMemoEdit repositoryItemMemoEdit5;

	private TileViewColumn tileViewColumn15;

	private TileViewColumn tileViewColumn16;

	private GridControl grdAlert;

	private TileView tileViewAlert;

	private TileViewColumn tileViewColumn17;

	private TileViewColumn tileViewColumn18;

	private RepositoryItemMemoEdit repositoryItemMemoEdit6;

	private TileViewColumn tileViewColumn19;

	private TileViewColumn tileViewColumn20;

	private AlertControlEx alertControlEx1;

	public Component ButtonControl { get; set; }

	public NotificationContrainer()
	{
		InitializeComponent();
	}

	private void NotificationContrainer_Load(object sender, EventArgs e)
	{
	}

	public void AddNotified(NotifiedObj obj)
	{
		try
		{
			List<NotifiedObj> list = grdNotification.DataSource as List<NotifiedObj>;
			if (list == null || list.Count == 0)
			{
				list = new List<NotifiedObj>();
			}
			NotifiedObj[] array = list.Where((NotifiedObj t) => (obj.TranType == Enums.TransactionType.M && t.TranType == Enums.TransactionType.M) || (obj.TranType != Enums.TransactionType.M && t.ObjectTypes == obj.ObjectTypes && t.TranType != Enums.TransactionType.M)).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				array[num].CountGroup++;
			}
			obj.CountGroup = array.Length + 1;
			list.Add(obj);
			grdNotification.DataSource = list;
			((dynamic)ButtonControl).Caption = ((list.Count > 99) ? "99+" : ((list.Count > 0) ? Function.ToString(list.Count) : ""));
			tileViewNotification.RefreshData();
			grdNotification.Refresh();
			XtraTabPageEx xtraTabPageEx = xtraTabPageExAll;
			string text;
			if (list.Count > 99)
			{
				text = "All(99+)";
			}
			else
			{
				text = ((list.Count > 0) ? $"All({Function.ToString(list.Count)})" : "All");
			}
			xtraTabPageEx.Text = text;
			xtraTabPageExAll.Appearance.Header.ForeColor = ((list.Count > 0) ? Color.Red : Color.Black);
			if (obj.TranType == Enums.TransactionType.M)
			{
				List<NotifiedObj> list2 = grdComment.DataSource as List<NotifiedObj>;
				if (list2 == null || list2.Count == 0)
				{
					list2 = new List<NotifiedObj>();
				}
				list2.Add(obj);
				grdComment.DataSource = list2;
				tileViewComment.RefreshData();
				grdComment.Refresh();
				XtraTabPageEx xtraTabPageEx2 = xtraTabPageExComments;
				string text2;
				if (list2.Count > 99)
				{
					text2 = "Comment(99+)";
				}
				else
				{
					text2 = ((list2.Count > 0) ? $"Comment({Function.ToString(list2.Count)})" : "Comment");
				}
				xtraTabPageEx2.Text = text2;
				xtraTabPageExComments.Appearance.Header.ForeColor = ((list2.Count > 0) ? Color.Red : Color.Black);
			}
			else if (obj.ObjectTypes == ApzObjectTypes.oTaskManagement)
			{
				List<NotifiedObj> list3 = grdTask.DataSource as List<NotifiedObj>;
				if (list3 == null || list3.Count == 0)
				{
					list3 = new List<NotifiedObj>();
				}
				list3.Add(obj);
				grdTask.DataSource = list3;
				tileViewTask.RefreshData();
				grdTask.Refresh();
				XtraTabPageEx xtraTabPageEx3 = xtraTabPageExTask;
				string text3;
				if (list3.Count > 99)
				{
					text3 = "Task(99+)";
				}
				else
				{
					text3 = ((list3.Count > 0) ? $"Task({Function.ToString(list3.Count)})" : "Task");
				}
				xtraTabPageEx3.Text = text3;
				xtraTabPageExTask.Appearance.Header.ForeColor = ((list3.Count > 0) ? Color.Red : Color.Black);
			}
			else if (obj.ObjectTypes == ApzObjectTypes.oTrackingIssue)
			{
				List<NotifiedObj> list4 = grdIssue.DataSource as List<NotifiedObj>;
				if (list4 == null || list4.Count == 0)
				{
					list4 = new List<NotifiedObj>();
				}
				list4.Add(obj);
				grdIssue.DataSource = list4;
				tileViewIssue.RefreshData();
				grdIssue.Refresh();
				XtraTabPageEx xtraTabPageEx4 = xtraTabPageExIssue;
				string text4;
				if (list4.Count > 99)
				{
					text4 = "Issue(99+)";
				}
				else
				{
					text4 = ((list4.Count > 0) ? $"Issue({Function.ToString(list4.Count)})" : "Issue");
				}
				xtraTabPageEx4.Text = text4;
				xtraTabPageExIssue.Appearance.Header.ForeColor = ((list4.Count > 0) ? Color.Red : Color.Black);
			}
			else if (obj.ObjectTypes == ApzObjectTypes.oEventManagement)
			{
				List<NotifiedObj> list5 = grdEvent.DataSource as List<NotifiedObj>;
				if (list5 == null || list5.Count == 0)
				{
					list5 = new List<NotifiedObj>();
				}
				list5.Add(obj);
				grdEvent.DataSource = list5;
				tileViewEvent.RefreshData();
				grdEvent.Refresh();
				XtraTabPageEx xtraTabPageEx5 = xtraTabPageExEvent;
				string text5;
				if (list5.Count > 99)
				{
					text5 = "Event(99+)";
				}
				else
				{
					text5 = ((list5.Count > 0) ? $"Event({Function.ToString(list5.Count)})" : "Event");
				}
				xtraTabPageEx5.Text = text5;
				xtraTabPageExEvent.Appearance.Header.ForeColor = ((list5.Count > 0) ? Color.Red : Color.Black);
			}
			else if (obj.ObjectTypes == ApzObjectTypes.oAlertOverview)
			{
				List<NotifiedObj> list6 = grdAlert.DataSource as List<NotifiedObj>;
				if (list6 == null || list6.Count == 0)
				{
					list6 = new List<NotifiedObj>();
				}
				list6.Add(obj);
				grdAlert.DataSource = list6;
				tileViewAlert.RefreshData();
				grdAlert.Refresh();
				XtraTabPageEx xtraTabPageEx6 = xtraTabPageExAlert;
				string text6;
				if (list6.Count > 99)
				{
					text6 = "Alert(99+)";
				}
				else
				{
					text6 = ((list6.Count > 0) ? $"Alert({Function.ToString(list6.Count)})" : "Alert");
				}
				xtraTabPageEx6.Text = text6;
				xtraTabPageExAlert.Appearance.Header.ForeColor = ((list6.Count > 0) ? Color.Red : Color.Black);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddNotification(List<NotifiedObj> lstSource)
	{
		try
		{
			List<NotifiedObj> list = new List<NotifiedObj>();
			List<NotifiedObj> list2 = new List<NotifiedObj>();
			List<NotifiedObj> list3 = new List<NotifiedObj>();
			List<NotifiedObj> list4 = new List<NotifiedObj>();
			List<NotifiedObj> list5 = new List<NotifiedObj>();
			List<NotifiedObj> list6 = new List<NotifiedObj>();
			if (lstSource != null)
			{
				int i = 0;
				while (i < lstSource.Count)
				{
					List<NotifiedObj> list7 = new List<NotifiedObj>();
					NotifiedObj[] array = lstSource.Where((NotifiedObj t) => (lstSource[i].TranType == Enums.TransactionType.M && t.TranType == Enums.TransactionType.M) || (lstSource[i].TranType != Enums.TransactionType.M && t.ObjectTypes == lstSource[i].ObjectTypes && t.TranType != Enums.TransactionType.M)).ToArray();
					int countGroup = array.Length;
					for (int num = 0; num < array.Length; num++)
					{
						array[num].CountGroup = countGroup;
						list7.Add(array[num]);
						lstSource.Remove(array[num]);
					}
					list.AddRange(list7);
					if (list7[0].TranType == Enums.TransactionType.M)
					{
						list5 = list7;
					}
					else if (list7[0].ObjectTypes == ApzObjectTypes.oTaskManagement)
					{
						list3 = list7;
					}
					else if (list7[0].ObjectTypes == ApzObjectTypes.oTrackingIssue)
					{
						list2 = list7;
					}
					else if (list7[0].ObjectTypes == ApzObjectTypes.oEventManagement)
					{
						list4 = list7;
					}
					else if (list7[0].ObjectTypes == ApzObjectTypes.oAlertOverview || list7[0].ObjectTypes == ApzObjectTypes.oWorkingProcess)
					{
						list6 = list7;
					}
				}
			}
			grdNotification.DataSource = list;
			tileViewNotification.RefreshData();
			grdNotification.Refresh();
			XtraTabPageEx xtraTabPageEx = xtraTabPageExAll;
			string text;
			if (list.Count > 99)
			{
				text = "All(99+)";
			}
			else
			{
				text = ((list.Count > 0) ? $"All({Function.ToString(list.Count)})" : "All");
			}
			xtraTabPageEx.Text = text;
			xtraTabPageExAll.Appearance.Header.ForeColor = ((list.Count > 0) ? Color.Red : Color.Black);
			grdIssue.DataSource = list2;
			tileViewIssue.RefreshData();
			grdIssue.Refresh();
			XtraTabPageEx xtraTabPageEx2 = xtraTabPageExIssue;
			string text2;
			if (list2.Count > 99)
			{
				text2 = "Issue(99+)";
			}
			else
			{
				text2 = ((list2.Count > 0) ? $"Issue({Function.ToString(list2.Count)})" : "Issue");
			}
			xtraTabPageEx2.Text = text2;
			xtraTabPageExIssue.Appearance.Header.ForeColor = ((list2.Count > 0) ? Color.Red : Color.Black);
			grdTask.DataSource = list3;
			tileViewTask.RefreshData();
			grdTask.Refresh();
			XtraTabPageEx xtraTabPageEx3 = xtraTabPageExTask;
			string text3;
			if (list3.Count > 99)
			{
				text3 = "Task(99+)";
			}
			else
			{
				text3 = ((list3.Count > 0) ? $"Task({Function.ToString(list3.Count)})" : "Task");
			}
			xtraTabPageEx3.Text = text3;
			xtraTabPageExTask.Appearance.Header.ForeColor = ((list3.Count > 0) ? Color.Red : Color.Black);
			grdEvent.DataSource = list4;
			tileViewEvent.RefreshData();
			grdEvent.Refresh();
			XtraTabPageEx xtraTabPageEx4 = xtraTabPageExEvent;
			string text4;
			if (list4.Count > 99)
			{
				text4 = "Event(99+)";
			}
			else
			{
				text4 = ((list4.Count > 0) ? $"Event({Function.ToString(list4.Count)})" : "Event");
			}
			xtraTabPageEx4.Text = text4;
			xtraTabPageExEvent.Appearance.Header.ForeColor = ((list4.Count > 0) ? Color.Red : Color.Black);
			grdComment.DataSource = list5;
			tileViewComment.RefreshData();
			grdComment.Refresh();
			XtraTabPageEx xtraTabPageEx5 = xtraTabPageExComments;
			string text5;
			if (list5.Count > 99)
			{
				text5 = "Comment(99+)";
			}
			else
			{
				text5 = ((list5.Count > 0) ? $"Comment({Function.ToString(list5.Count)})" : "Comment");
			}
			xtraTabPageEx5.Text = text5;
			xtraTabPageExComments.Appearance.Header.ForeColor = ((list5.Count > 0) ? Color.Red : Color.Black);
			grdAlert.DataSource = list6;
			tileViewAlert.RefreshData();
			grdAlert.Refresh();
			XtraTabPageEx xtraTabPageEx6 = xtraTabPageExAlert;
			string text6;
			if (list6.Count > 99)
			{
				text6 = "Alert(99+)";
			}
			else
			{
				text6 = ((list6.Count > 0) ? $"Alert({Function.ToString(list6.Count)})" : "Alert");
			}
			xtraTabPageEx6.Text = text6;
			xtraTabPageExAlert.Appearance.Header.ForeColor = ((list6.Count > 0) ? Color.Red : Color.Black);
			((dynamic)ButtonControl).Caption = ((list.Count > 99) ? "99+" : ((list.Count > 0) ? Function.ToString(list.Count) : ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ClearNotification()
	{
		try
		{
			grdNotification.DataSource = null;
			((dynamic)ButtonControl).Caption = "";
			tileViewNotification.RefreshData();
			grdNotification.Refresh();
			xtraTabPageExAll.Text = "All";
			xtraTabPageExAll.Appearance.Header.ForeColor = Color.Black;
			grdIssue.DataSource = null;
			tileViewIssue.RefreshData();
			grdIssue.Refresh();
			xtraTabPageExIssue.Text = "Issue";
			xtraTabPageExIssue.Appearance.Header.ForeColor = Color.Black;
			grdTask.DataSource = null;
			tileViewTask.RefreshData();
			grdTask.Refresh();
			xtraTabPageExTask.Text = "Task";
			xtraTabPageExTask.Appearance.Header.ForeColor = Color.Black;
			grdEvent.DataSource = null;
			tileViewEvent.RefreshData();
			grdEvent.Refresh();
			xtraTabPageExEvent.Text = "Event";
			xtraTabPageExEvent.Appearance.Header.ForeColor = Color.Black;
			grdComment.DataSource = null;
			tileViewComment.RefreshData();
			grdComment.Refresh();
			xtraTabPageExComments.Text = "Comment";
			xtraTabPageExComments.Appearance.Header.ForeColor = Color.Black;
			grdAlert.DataSource = null;
			tileViewAlert.RefreshData();
			grdAlert.Refresh();
			xtraTabPageExAlert.Text = "Alert";
			xtraTabPageExAlert.Appearance.Header.ForeColor = Color.Black;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ClearNotified(ApzObjectTypes objectTypes, string notifiedId, Enums.TransactionType transactionType)
	{
		try
		{
			if (!(grdNotification.DataSource is List<NotifiedObj> list))
			{
				return;
			}
			if (transactionType != Enums.TransactionType.M)
			{
				NotifiedObj notifiedObj = list.FirstOrDefault((NotifiedObj t) => t.ObjectTypes == objectTypes && t.NotifiedId == notifiedId && t.TranType != Enums.TransactionType.M);
				if (notifiedObj != null)
				{
					list.Remove(notifiedObj);
					NotifiedObj[] array = list.Where((NotifiedObj t) => t.ObjectTypes == objectTypes && t.TranType != Enums.TransactionType.M).ToArray();
					for (int num = 0; num < array.Length; num++)
					{
						array[num].CountGroup--;
					}
					if (objectTypes == ApzObjectTypes.oTaskManagement)
					{
						List<NotifiedObj> list2 = grdTask.DataSource as List<NotifiedObj>;
						if (list2 == null || list2.Count == 0)
						{
							list2 = new List<NotifiedObj>();
						}
						list2.Remove(notifiedObj);
						grdTask.DataSource = list2;
						tileViewTask.RefreshData();
						grdTask.Refresh();
						XtraTabPageEx xtraTabPageEx = xtraTabPageExTask;
						string text;
						if (list2.Count > 99)
						{
							text = "Task(99+)";
						}
						else
						{
							text = ((list2.Count > 0) ? $"Task({Function.ToString(list2.Count)})" : "Task");
						}
						xtraTabPageEx.Text = text;
						xtraTabPageExTask.Appearance.Header.ForeColor = ((list2.Count > 0) ? Color.Red : Color.Black);
					}
					else if (objectTypes == ApzObjectTypes.oTrackingIssue)
					{
						List<NotifiedObj> list3 = grdIssue.DataSource as List<NotifiedObj>;
						if (list3 == null || list3.Count == 0)
						{
							list3 = new List<NotifiedObj>();
						}
						list3.Remove(notifiedObj);
						grdIssue.DataSource = list3;
						tileViewIssue.RefreshData();
						grdIssue.Refresh();
						XtraTabPageEx xtraTabPageEx2 = xtraTabPageExIssue;
						string text2;
						if (list3.Count > 99)
						{
							text2 = "Issue(99+)";
						}
						else
						{
							text2 = ((list3.Count > 0) ? $"Issue({Function.ToString(list3.Count)})" : "Issue");
						}
						xtraTabPageEx2.Text = text2;
						xtraTabPageExIssue.Appearance.Header.ForeColor = ((list3.Count > 0) ? Color.Red : Color.Black);
					}
					else if (objectTypes == ApzObjectTypes.oEventManagement)
					{
						List<NotifiedObj> list4 = grdEvent.DataSource as List<NotifiedObj>;
						if (list4 == null || list4.Count == 0)
						{
							list4 = new List<NotifiedObj>();
						}
						list4.Remove(notifiedObj);
						grdEvent.DataSource = list4;
						tileViewEvent.RefreshData();
						grdEvent.Refresh();
						XtraTabPageEx xtraTabPageEx3 = xtraTabPageExEvent;
						string text3;
						if (list4.Count > 99)
						{
							text3 = "Event(99+)";
						}
						else
						{
							text3 = ((list4.Count > 0) ? $"Event({Function.ToString(list4.Count)})" : "Event");
						}
						xtraTabPageEx3.Text = text3;
						xtraTabPageExEvent.Appearance.Header.ForeColor = ((list4.Count > 0) ? Color.Red : Color.Black);
					}
					else if (objectTypes == ApzObjectTypes.oAlertOverview)
					{
						List<NotifiedObj> list5 = grdAlert.DataSource as List<NotifiedObj>;
						if (list5 == null || list5.Count == 0)
						{
							list5 = new List<NotifiedObj>();
						}
						list5.Remove(notifiedObj);
						grdAlert.DataSource = list5;
						tileViewAlert.RefreshData();
						grdAlert.Refresh();
						XtraTabPageEx xtraTabPageEx4 = xtraTabPageExAlert;
						string text4;
						if (list5.Count > 99)
						{
							text4 = "Alert(99+)";
						}
						else
						{
							text4 = ((list5.Count > 0) ? $"Alert({Function.ToString(list5.Count)})" : "Alert");
						}
						xtraTabPageEx4.Text = text4;
						xtraTabPageExAlert.Appearance.Header.ForeColor = ((list5.Count > 0) ? Color.Red : Color.Black);
					}
				}
			}
			NotifiedObj[] array2 = list.Where((NotifiedObj t) => t.ObjectTypes == objectTypes && t.NotifiedId == notifiedId && t.TranType == Enums.TransactionType.M).ToArray();
			if (array2.Length != 0)
			{
				List<NotifiedObj> list6 = grdComment.DataSource as List<NotifiedObj>;
				if (list6 == null || list6.Count == 0)
				{
					list6 = new List<NotifiedObj>();
				}
				for (int num2 = 0; num2 < array2.Length; num2++)
				{
					PushCommentToRead(array2[num2].CmtCode);
					list.Remove(array2[num2]);
					list6.Remove(array2[num2]);
				}
				NotifiedObj[] array3 = list.Where((NotifiedObj t) => t.TranType == Enums.TransactionType.M).ToArray();
				for (int num3 = 0; num3 < array3.Length; num3++)
				{
					array3[num3].CountGroup -= array2.Length;
				}
				grdComment.DataSource = list6;
				tileViewComment.RefreshData();
				grdComment.Refresh();
				XtraTabPageEx xtraTabPageEx5 = xtraTabPageExComments;
				string text5;
				if (list6.Count > 99)
				{
					text5 = "Comment(99+)";
				}
				else
				{
					text5 = ((list6.Count > 0) ? $"Comment({Function.ToString(list6.Count)})" : "Comment");
				}
				xtraTabPageEx5.Text = text5;
				xtraTabPageExComments.Appearance.Header.ForeColor = ((list6.Count > 0) ? Color.Red : Color.Black);
			}
			grdNotification.DataSource = list;
			tileViewNotification.RefreshData();
			grdNotification.Refresh();
			XtraTabPageEx xtraTabPageEx6 = xtraTabPageExAll;
			string text6;
			if (list.Count > 99)
			{
				text6 = "All(99+)";
			}
			else
			{
				text6 = ((list.Count > 0) ? $"All({Function.ToString(list.Count)})" : "All");
			}
			xtraTabPageEx6.Text = text6;
			xtraTabPageExAll.Appearance.Header.ForeColor = ((list.Count > 0) ? Color.Red : Color.Black);
			((dynamic)ButtonControl).Caption = ((list.Count > 99) ? "99+" : ((list.Count > 0) ? Function.ToString(list.Count) : ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public bool CheckExistNotified(ApzObjectTypes objectTypes, string notifiedId)
	{
		try
		{
			if (!(grdNotification.DataSource is List<NotifiedObj> { Count: not 0 } list))
			{
				return false;
			}
			if (list.FirstOrDefault((NotifiedObj t) => t.ObjectTypes == objectTypes && t.NotifiedId == notifiedId) != null)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	public void LoadAlertOverDue(AlertControlEx alertControl)
	{
		try
		{
			if (!(grdNotification.DataSource is List<NotifiedObj> { Count: not 0 } list))
			{
				return;
			}
			List<NotifiedObj> list2 = list.Where((NotifiedObj t) => t.ObjectTypes == ApzObjectTypes.oTaskManagement && t.DueDate.Subtract(DateTime.Now).TotalMinutes <= 15.0).ToList();
			if (list2.Count == 0)
			{
				return;
			}
			list2 = list2.OrderByDescending((NotifiedObj t) => t.DueDate.Subtract(DateTime.Now)).ToList();
			for (int num = 0; num < list2.Count; num++)
			{
				AlertInfo info = new AlertInfo(list2[num].NotifiedTitle, list2[num].NotifiedBody, list2[num].NotifiedBody, null, list2[num]);
				alertControl.ShowAlert(FindForm(), info, list2[num].TranType);
				if (num > 10)
				{
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void LoadAlertOverDueForEvent(AlertControlEx alertControl, int timerInterval = 100)
	{
		try
		{
			if (!(grdNotification.DataSource is List<NotifiedObj> { Count: not 0 } list))
			{
				return;
			}
			NotifiedObj[] array = list.Where((NotifiedObj t) => t.ObjectTypes == ApzObjectTypes.oEventManagement && t.DueDate.Subtract(DateTime.Now).TotalMilliseconds < t.TimeBeforeStart.TotalMilliseconds + TimeSpan.FromMilliseconds(timerInterval).TotalMilliseconds && t.DueDate.Subtract(DateTime.Now).TotalMilliseconds >= t.TimeBeforeStart.TotalMilliseconds).ToArray();
			if (array.Length != 0)
			{
				for (int num = 0; num < array.Length; num++)
				{
					AlertInfo info = new AlertInfo(array[num].NotifiedTitle, array[num].NotifiedBody, array[num].NotifiedBody, null, array[num]);
					alertControl.ShowAlert(FindForm(), info, array[num].TranType);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void UpdateNotification(NotifiedObj obj)
	{
		try
		{
			if (!(grdNotification.DataSource is List<NotifiedObj> list))
			{
				return;
			}
			NotifiedObj notifiedObj = list.FirstOrDefault((NotifiedObj t) => t.ObjectTypes == obj.ObjectTypes && t.NotifiedId == obj.NotifiedId && obj.TranType == t.TranType);
			if (notifiedObj != null)
			{
				list.Remove(notifiedObj);
				AppointmentType apptType = (AppointmentType)obj.ApptType;
				if (apptType == AppointmentType.Pattern && !string.IsNullOrWhiteSpace(obj.RecurrenceInfo))
				{
					Appointment pattern = StaticAppointmentFactory.CreateAppointment(apptType);
					DateTime dueDate = OccurrenceCalculator.CreateInstance(RecurrenceInfoXmlPersistenceHelper.ObjectFromXml(obj.RecurrenceInfo), TimeZoneInfo.Utc).FindNextOccurrenceTimeAfter(DateTime.Now, pattern);
					obj.DueDate = dueDate;
				}
				obj.CountGroup = notifiedObj.CountGroup;
				list.Add(obj);
				if (obj.TranType == Enums.TransactionType.M)
				{
					List<NotifiedObj> list2 = grdComment.DataSource as List<NotifiedObj>;
					if (list2 == null || list2.Count == 0)
					{
						list2 = new List<NotifiedObj>();
					}
					list2.Remove(notifiedObj);
					list2.Add(obj);
					grdComment.DataSource = list2;
					tileViewComment.RefreshData();
					grdComment.Refresh();
				}
				else if (obj.ObjectTypes == ApzObjectTypes.oTaskManagement)
				{
					List<NotifiedObj> list3 = grdTask.DataSource as List<NotifiedObj>;
					if (list3 == null || list3.Count == 0)
					{
						list3 = new List<NotifiedObj>();
					}
					list3.Remove(notifiedObj);
					list3.Add(obj);
					grdTask.DataSource = list3;
					tileViewTask.RefreshData();
					grdTask.Refresh();
				}
				else if (obj.ObjectTypes == ApzObjectTypes.oTrackingIssue)
				{
					List<NotifiedObj> list4 = grdIssue.DataSource as List<NotifiedObj>;
					if (list4 == null || list4.Count == 0)
					{
						list4 = new List<NotifiedObj>();
					}
					list4.Remove(notifiedObj);
					list4.Add(obj);
					grdIssue.DataSource = list4;
					tileViewIssue.RefreshData();
					grdIssue.Refresh();
				}
				else if (obj.ObjectTypes == ApzObjectTypes.oEventManagement)
				{
					List<NotifiedObj> list5 = grdEvent.DataSource as List<NotifiedObj>;
					if (list5 == null || list5.Count == 0)
					{
						list5 = new List<NotifiedObj>();
					}
					list5.Remove(notifiedObj);
					list5.Add(obj);
					grdEvent.DataSource = list5;
					tileViewEvent.RefreshData();
					grdEvent.Refresh();
				}
				else if (obj.ObjectTypes == ApzObjectTypes.oAlertOverview)
				{
					List<NotifiedObj> list6 = grdAlert.DataSource as List<NotifiedObj>;
					if (list6 == null || list6.Count == 0)
					{
						list6 = new List<NotifiedObj>();
					}
					list6.Remove(notifiedObj);
					list6.Add(obj);
					grdAlert.DataSource = list6;
					tileViewAlert.RefreshData();
					grdAlert.Refresh();
				}
			}
			grdNotification.DataSource = list;
			tileViewNotification.RefreshData();
			grdNotification.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void PushCommentToRead(int commentCode)
	{
		try
		{
			HttpResult httpResult = await _httpClient.SubActivities.MarkAsRead(commentCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", httpResult.Message);
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileViewNotification_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
	}

	private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (!(e.SelectedControl is GridControl gridControl))
			{
				return;
			}
			ToolTipControlInfo toolTipControlInfo = null;
			if (gridControl.GetViewAt(e.ControlMousePosition) is TileView tileView)
			{
				TileViewHitInfo tileViewHitInfo = tileView.CalcHitInfo(e.ControlMousePosition);
				if (tileViewHitInfo.HitTest == TileControlHitTest.Item)
				{
					string text = Function.ToString(tileViewHitInfo.HitTest) + Function.ToString(tileViewHitInfo.RowHandle);
					string text2 = Function.ToString(tileView.GetRowCellValue(tileViewHitInfo.RowHandle, tvc_Body.FieldName));
					toolTipControlInfo = new ToolTipControlInfo(text, text2);
				}
				if (toolTipControlInfo != null)
				{
					e.Info = toolTipControlInfo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileViewNotification_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			TileView tileView = sender as TileView;
			if (e.Item == null || e.Item.RowHandle < 0 || tileView == null || !(tileView.GetRow(e.Item.RowHandle) is NotifiedObj notifiedObj))
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (notifiedObj.ObjectTypes == ApzObjectTypes.oTrackingIssue)
			{
				frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue(notifiedObj.TranType == Enums.TransactionType.M, Function.ParseInt(notifiedObj.NotifiedId));
				frm_TrackingIssue2.MdiParent = FindForm();
				frm_TrackingIssue2.Show();
			}
			else if (notifiedObj.ObjectTypes == ApzObjectTypes.oWorkingProcess)
			{
				frm_WorkingProcess frm_WorkingProcess2 = new frm_WorkingProcess();
				frm_WorkingProcess2.MdiParent = FindForm();
				frm_WorkingProcess2.Show();
			}
			else if (notifiedObj.ObjectTypes != ApzObjectTypes.oAlertOverview)
			{
				new XtraFormEx
				{
					MdiParent = FindForm()
				}.DrillDown(notifiedObj.ObjectTypes, notifiedObj.NotifiedId);
			}
			else
			{
				frm_AlertOverview frm_AlertOverview2 = new frm_AlertOverview();
				frm_AlertOverview2.MdiParent = FindForm();
				frm_AlertOverview2.Show();
			}
			WinCommons._CloseProcessing();
			Visible = false;
			ClearNotified(notifiedObj.ObjectTypes, notifiedObj.NotifiedId, notifiedObj.TranType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void tileViewNotification_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
	}

	private void tileViewNotification_ContextButtonCustomize(object sender, TileViewContextButtonCustomizeEventArgs e)
	{
		try
		{
			if (!(sender is TileView tileView))
			{
				return;
			}
			if (e.Item.Name.Equals("btnContextClear"))
			{
				e.Item.AllowGlyphSkinning = DefaultBoolean.True;
				AppearanceObject appearanceHover = e.Item.AppearanceHover;
				Color foreColor = (e.Item.AppearanceNormal.ForeColor = ((ITileControl)tileView.GetViewInfo()).ViewInfo.AppearanceText.ForeColor);
				appearanceHover.ForeColor = foreColor;
			}
			else if (tileView.GetRow(e.RowHandle) is NotifiedObj { TranType: not Enums.TransactionType.M, DueDate: var dueDate })
			{
				double totalMinutes = dueDate.Subtract(DateTime.Now).TotalMinutes;
				if (e.Item.Name.Equals("btnContextTypeE"))
				{
					e.Item.AllowGlyphSkinning = DefaultBoolean.False;
					e.Item.Visibility = ((!(totalMinutes <= 5.0)) ? ContextItemVisibility.Hidden : ContextItemVisibility.Visible);
				}
				else
				{
					e.Item.AllowGlyphSkinning = DefaultBoolean.False;
					e.Item.Visibility = ContextItemVisibility.Hidden;
				}
			}
			else
			{
				e.Item.AllowGlyphSkinning = DefaultBoolean.False;
				e.Item.Visibility = ContextItemVisibility.Hidden;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileViewNotification_ContextButtonClick(object sender, ContextItemClickEventArgs e)
	{
		try
		{
			if (e.Item.Name.Equals("btnContextClear") && sender is TileView tileView)
			{
				dynamic val = Function.ParseInt(((dynamic)e.DataItem).RowHandle);
				if (tileView.GetRow(val) is NotifiedObj notifiedObj)
				{
					ClearNotified(notifiedObj.ObjectTypes, notifiedObj.NotifiedId, notifiedObj.TranType);
				}
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
		this.components = new System.ComponentModel.Container();
		DevExpress.Utils.ContextButton contextButton = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton2 = new DevExpress.Utils.ContextButton();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.Notification.NotificationContrainer));
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition4 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition5 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan3 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan4 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.Utils.ContextButton contextButton3 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton4 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition6 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition7 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition8 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition9 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition10 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan5 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan6 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan7 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan8 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.Utils.ContextButton contextButton5 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton6 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition11 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition12 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition13 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition14 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition15 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition5 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition6 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan9 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan10 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan11 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan12 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement8 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement9 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.Utils.ContextButton contextButton7 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton8 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition16 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition17 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition18 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition19 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition20 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition7 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition8 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan13 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan14 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan15 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan16 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement10 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement11 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement12 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.Utils.ContextButton contextButton9 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton10 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition21 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition22 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition23 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition24 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition25 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition9 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition10 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan17 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan18 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan19 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan20 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement13 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement14 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement15 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.Utils.ContextButton contextButton11 = new DevExpress.Utils.ContextButton();
		DevExpress.Utils.ContextButton contextButton12 = new DevExpress.Utils.ContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition26 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition27 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition28 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition29 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition30 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition11 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition12 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan21 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan22 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan23 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan24 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement16 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement17 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement18 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.tvc_Title = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tvc_DueDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tvc_Body = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn1 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn3 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn2 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn5 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn7 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn6 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn9 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn11 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn10 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn13 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn15 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn14 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn17 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn19 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn18 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.grdNotification = new DevExpress.XtraGrid.GridControl();
		this.tileViewNotification = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tvc_Group = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.xtraTabControlNotification = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageExAll = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabPageExIssue = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdIssue = new DevExpress.XtraGrid.GridControl();
		this.tileViewIssue = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn4 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.xtraTabPageExTask = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdTask = new DevExpress.XtraGrid.GridControl();
		this.tileViewTask = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn8 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.xtraTabPageExEvent = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdEvent = new DevExpress.XtraGrid.GridControl();
		this.tileViewEvent = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn12 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.xtraTabPageExComments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdComment = new DevExpress.XtraGrid.GridControl();
		this.tileViewComment = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn16 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.xtraTabPageExAlert = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdAlert = new DevExpress.XtraGrid.GridControl();
		this.tileViewAlert = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn20 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.alertControlEx1 = new Apzon.Controls.Controls.AlertControlEx(this.components);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdNotification).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewNotification).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlNotification).BeginInit();
		this.xtraTabControlNotification.SuspendLayout();
		this.xtraTabPageExAll.SuspendLayout();
		this.xtraTabPageExIssue.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdIssue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewIssue).BeginInit();
		this.xtraTabPageExTask.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdTask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewTask).BeginInit();
		this.xtraTabPageExEvent.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdEvent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewEvent).BeginInit();
		this.xtraTabPageExComments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdComment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewComment).BeginInit();
		this.xtraTabPageExAlert.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdAlert).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewAlert).BeginInit();
		base.SuspendLayout();
		this.tvc_Title.Caption = "Title";
		this.tvc_Title.FieldName = "NotifiedTitle";
		this.tvc_Title.Name = "tvc_Title";
		this.tvc_Title.Visible = true;
		this.tvc_Title.VisibleIndex = 0;
		this.tvc_DueDate.Caption = "DueDate";
		this.tvc_DueDate.FieldName = "DueDateView";
		this.tvc_DueDate.Name = "tvc_DueDate";
		this.tvc_DueDate.Visible = true;
		this.tvc_DueDate.VisibleIndex = 2;
		this.tvc_Body.Caption = "body";
		this.tvc_Body.ColumnEdit = this.repositoryItemMemoEdit1;
		this.tvc_Body.FieldName = "NotifiedBody";
		this.tvc_Body.Name = "tvc_Body";
		this.tvc_Body.Visible = true;
		this.tvc_Body.VisibleIndex = 1;
		this.repositoryItemMemoEdit1.LinesCount = 2;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.tileViewColumn1.Caption = "Title";
		this.tileViewColumn1.FieldName = "NotifiedTitle";
		this.tileViewColumn1.Name = "tileViewColumn1";
		this.tileViewColumn1.Visible = true;
		this.tileViewColumn1.VisibleIndex = 0;
		this.tileViewColumn3.Caption = "DueDate";
		this.tileViewColumn3.FieldName = "DueDateView";
		this.tileViewColumn3.Name = "tileViewColumn3";
		this.tileViewColumn3.Visible = true;
		this.tileViewColumn3.VisibleIndex = 2;
		this.tileViewColumn2.Caption = "body";
		this.tileViewColumn2.ColumnEdit = this.repositoryItemMemoEdit2;
		this.tileViewColumn2.FieldName = "NotifiedBody";
		this.tileViewColumn2.Name = "tileViewColumn2";
		this.tileViewColumn2.Visible = true;
		this.tileViewColumn2.VisibleIndex = 1;
		this.repositoryItemMemoEdit2.LinesCount = 2;
		this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
		this.tileViewColumn5.Caption = "Title";
		this.tileViewColumn5.FieldName = "NotifiedTitle";
		this.tileViewColumn5.Name = "tileViewColumn5";
		this.tileViewColumn5.Visible = true;
		this.tileViewColumn5.VisibleIndex = 0;
		this.tileViewColumn7.Caption = "DueDate";
		this.tileViewColumn7.FieldName = "DueDateView";
		this.tileViewColumn7.Name = "tileViewColumn7";
		this.tileViewColumn7.Visible = true;
		this.tileViewColumn7.VisibleIndex = 2;
		this.tileViewColumn6.Caption = "body";
		this.tileViewColumn6.ColumnEdit = this.repositoryItemMemoEdit3;
		this.tileViewColumn6.FieldName = "NotifiedBody";
		this.tileViewColumn6.Name = "tileViewColumn6";
		this.tileViewColumn6.Visible = true;
		this.tileViewColumn6.VisibleIndex = 1;
		this.repositoryItemMemoEdit3.LinesCount = 2;
		this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
		this.tileViewColumn9.Caption = "Title";
		this.tileViewColumn9.FieldName = "NotifiedTitle";
		this.tileViewColumn9.Name = "tileViewColumn9";
		this.tileViewColumn9.Visible = true;
		this.tileViewColumn9.VisibleIndex = 0;
		this.tileViewColumn11.Caption = "DueDate";
		this.tileViewColumn11.FieldName = "DueDateView";
		this.tileViewColumn11.Name = "tileViewColumn11";
		this.tileViewColumn11.Visible = true;
		this.tileViewColumn11.VisibleIndex = 2;
		this.tileViewColumn10.Caption = "body";
		this.tileViewColumn10.ColumnEdit = this.repositoryItemMemoEdit4;
		this.tileViewColumn10.FieldName = "NotifiedBody";
		this.tileViewColumn10.Name = "tileViewColumn10";
		this.tileViewColumn10.Visible = true;
		this.tileViewColumn10.VisibleIndex = 1;
		this.repositoryItemMemoEdit4.LinesCount = 2;
		this.repositoryItemMemoEdit4.Name = "repositoryItemMemoEdit4";
		this.tileViewColumn13.Caption = "Title";
		this.tileViewColumn13.FieldName = "NotifiedTitle";
		this.tileViewColumn13.Name = "tileViewColumn13";
		this.tileViewColumn13.Visible = true;
		this.tileViewColumn13.VisibleIndex = 0;
		this.tileViewColumn15.Caption = "DueDate";
		this.tileViewColumn15.FieldName = "DueDateView";
		this.tileViewColumn15.Name = "tileViewColumn15";
		this.tileViewColumn15.Visible = true;
		this.tileViewColumn15.VisibleIndex = 2;
		this.tileViewColumn14.Caption = "body";
		this.tileViewColumn14.ColumnEdit = this.repositoryItemMemoEdit5;
		this.tileViewColumn14.FieldName = "NotifiedBody";
		this.tileViewColumn14.Name = "tileViewColumn14";
		this.tileViewColumn14.Visible = true;
		this.tileViewColumn14.VisibleIndex = 1;
		this.repositoryItemMemoEdit5.LinesCount = 2;
		this.repositoryItemMemoEdit5.Name = "repositoryItemMemoEdit5";
		this.tileViewColumn17.Caption = "Title";
		this.tileViewColumn17.FieldName = "NotifiedTitle";
		this.tileViewColumn17.Name = "tileViewColumn17";
		this.tileViewColumn17.Visible = true;
		this.tileViewColumn17.VisibleIndex = 0;
		this.tileViewColumn19.Caption = "DueDate";
		this.tileViewColumn19.FieldName = "DueDateView";
		this.tileViewColumn19.Name = "tileViewColumn19";
		this.tileViewColumn19.Visible = true;
		this.tileViewColumn19.VisibleIndex = 2;
		this.tileViewColumn18.Caption = "body";
		this.tileViewColumn18.ColumnEdit = this.repositoryItemMemoEdit6;
		this.tileViewColumn18.FieldName = "NotifiedBody";
		this.tileViewColumn18.Name = "tileViewColumn18";
		this.tileViewColumn18.Visible = true;
		this.tileViewColumn18.VisibleIndex = 1;
		this.repositoryItemMemoEdit6.LinesCount = 2;
		this.repositoryItemMemoEdit6.Name = "repositoryItemMemoEdit6";
		this.grdNotification.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdNotification.Location = new System.Drawing.Point(0, 0);
		this.grdNotification.MainView = this.tileViewNotification;
		this.grdNotification.Name = "grdNotification";
		this.grdNotification.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit1 });
		this.grdNotification.Size = new System.Drawing.Size(454, 589);
		this.grdNotification.TabIndex = 0;
		this.grdNotification.ToolTipController = this.toolTipController1;
		this.grdNotification.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewNotification });
		this.tileViewNotification.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewNotification.Appearance.Group.Options.UseFont = true;
		this.tileViewNotification.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewNotification.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewNotification.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewNotification.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewNotification.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewNotification.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tvc_Title, this.tvc_Body, this.tvc_DueDate, this.tvc_Group });
		this.tileViewNotification.ColumnSet.GroupColumn = this.tvc_Group;
		contextButton.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton.Name = "btnContextClear";
		contextButton2.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton2.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		contextButton2.Name = "btnContextTypeE";
		contextButton2.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewNotification.ContextButtons.Add(contextButton);
		this.tileViewNotification.ContextButtons.Add(contextButton2);
		this.tileViewNotification.GridControl = this.grdNotification;
		this.tileViewNotification.Name = "tileViewNotification";
		this.tileViewNotification.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewNotification.OptionsTiles.AllowItemHover = true;
		this.tileViewNotification.OptionsTiles.AllowPressAnimation = false;
		this.tileViewNotification.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewNotification.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewNotification.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewNotification.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewNotification.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewNotification.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewNotification.OptionsTiles.RowCount = 0;
		this.tileViewNotification.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewNotification.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tvc_Group, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition.Length.Value = 19.0;
		tableColumnDefinition2.Length.Value = 35.0;
		tableColumnDefinition3.Length.Value = 279.0;
		tableColumnDefinition4.Length.Value = 126.0;
		tableColumnDefinition5.Length.Value = 17.0;
		this.tileViewNotification.TileColumns.Add(tableColumnDefinition);
		this.tileViewNotification.TileColumns.Add(tableColumnDefinition2);
		this.tileViewNotification.TileColumns.Add(tableColumnDefinition3);
		this.tileViewNotification.TileColumns.Add(tableColumnDefinition4);
		this.tileViewNotification.TileColumns.Add(tableColumnDefinition5);
		tableRowDefinition.Length.Value = 32.0;
		tableRowDefinition2.Length.Value = 18.0;
		this.tileViewNotification.TileRows.Add(tableRowDefinition);
		this.tileViewNotification.TileRows.Add(tableRowDefinition2);
		tableSpan.RowSpan = 2;
		tableSpan2.ColumnIndex = 3;
		tableSpan2.ColumnSpan = 2;
		tableSpan2.RowIndex = 1;
		tableSpan3.ColumnIndex = 1;
		tableSpan3.ColumnSpan = 3;
		tableSpan4.ColumnIndex = 1;
		tableSpan4.ColumnSpan = 2;
		tableSpan4.RowIndex = 1;
		this.tileViewNotification.TileSpans.Add(tableSpan);
		this.tileViewNotification.TileSpans.Add(tableSpan2);
		this.tileViewNotification.TileSpans.Add(tableSpan3);
		this.tileViewNotification.TileSpans.Add(tableSpan4);
		tileViewItemElement.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement.Column = this.tvc_Title;
		tileViewItemElement.ColumnIndex = 2;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "tvc_Title";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement2.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement2.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement2.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement2.Column = this.tvc_DueDate;
		tileViewItemElement2.ColumnIndex = 3;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 1;
		tileViewItemElement2.Text = "tvc_DueDate";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement3.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement3.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement3.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement3.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement3.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement3.Column = this.tvc_Body;
		tileViewItemElement3.ColumnIndex = 2;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.Text = "tvc_Body";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewNotification.TileTemplate.Add(tileViewItemElement);
		this.tileViewNotification.TileTemplate.Add(tileViewItemElement2);
		this.tileViewNotification.TileTemplate.Add(tileViewItemElement3);
		this.tileViewNotification.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewNotification.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewNotification.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewNotification.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewNotification.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tvc_Group.AppearanceCell.Options.UseTextOptions = true;
		this.tvc_Group.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tvc_Group.Caption = "tvc_Group";
		this.tvc_Group.FieldName = "ObjectView";
		this.tvc_Group.Name = "tvc_Group";
		this.tvc_Group.Visible = true;
		this.tvc_Group.VisibleIndex = 3;
		this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
		this.xtraTabControlNotification.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlNotification.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Right;
		this.xtraTabControlNotification.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Vertical;
		this.xtraTabControlNotification.IsSearchControl = false;
		this.xtraTabControlNotification.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlNotification.Name = "xtraTabControlNotification";
		this.xtraTabControlNotification.SelectedTabPage = this.xtraTabPageExAll;
		this.xtraTabControlNotification.Size = new System.Drawing.Size(486, 591);
		this.xtraTabControlNotification.TabIndex = 1;
		this.xtraTabControlNotification.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[6] { this.xtraTabPageExAll, this.xtraTabPageExIssue, this.xtraTabPageExTask, this.xtraTabPageExEvent, this.xtraTabPageExComments, this.xtraTabPageExAlert });
		this.xtraTabPageExAll.Controls.Add(this.grdNotification);
		this.xtraTabPageExAll.FieldName = "";
		this.xtraTabPageExAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExAll.ImageOptions.Image");
		this.xtraTabPageExAll.IsSearchControl = false;
		this.xtraTabPageExAll.Name = "xtraTabPageExAll";
		this.xtraTabPageExAll.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExAll.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExAll.Tag = "";
		this.xtraTabPageExAll.Text = "ALL";
		this.xtraTabPageExAll.Tooltip = "All Notification";
		this.xtraTabPageExIssue.Controls.Add(this.grdIssue);
		this.xtraTabPageExIssue.FieldName = "";
		this.xtraTabPageExIssue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExIssue.ImageOptions.Image");
		this.xtraTabPageExIssue.IsSearchControl = false;
		this.xtraTabPageExIssue.Name = "xtraTabPageExIssue";
		this.xtraTabPageExIssue.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExIssue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExIssue.Text = "Issue";
		this.xtraTabPageExIssue.Tooltip = "Uncomplete Issues";
		this.grdIssue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdIssue.Location = new System.Drawing.Point(0, 0);
		this.grdIssue.MainView = this.tileViewIssue;
		this.grdIssue.Name = "grdIssue";
		this.grdIssue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit2 });
		this.grdIssue.Size = new System.Drawing.Size(454, 589);
		this.grdIssue.TabIndex = 1;
		this.grdIssue.ToolTipController = this.toolTipController1;
		this.grdIssue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewIssue });
		this.tileViewIssue.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewIssue.Appearance.Group.Options.UseFont = true;
		this.tileViewIssue.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewIssue.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewIssue.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewIssue.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewIssue.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewIssue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tileViewColumn1, this.tileViewColumn2, this.tileViewColumn3, this.tileViewColumn4 });
		this.tileViewIssue.ColumnSet.GroupColumn = this.tileViewColumn4;
		contextButton3.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton3.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton3.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton3.Name = "btnContextClear";
		contextButton3.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
		contextButton4.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton4.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image1");
		contextButton4.Name = "btnContextTypeE";
		contextButton4.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewIssue.ContextButtons.Add(contextButton3);
		this.tileViewIssue.ContextButtons.Add(contextButton4);
		this.tileViewIssue.GridControl = this.grdIssue;
		this.tileViewIssue.Name = "tileViewIssue";
		this.tileViewIssue.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewIssue.OptionsTiles.AllowItemHover = true;
		this.tileViewIssue.OptionsTiles.AllowPressAnimation = false;
		this.tileViewIssue.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewIssue.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewIssue.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewIssue.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewIssue.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewIssue.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewIssue.OptionsTiles.RowCount = 0;
		this.tileViewIssue.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewIssue.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tileViewColumn4, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition6.Length.Value = 19.0;
		tableColumnDefinition7.Length.Value = 35.0;
		tableColumnDefinition8.Length.Value = 279.0;
		tableColumnDefinition9.Length.Value = 126.0;
		tableColumnDefinition10.Length.Value = 17.0;
		this.tileViewIssue.TileColumns.Add(tableColumnDefinition6);
		this.tileViewIssue.TileColumns.Add(tableColumnDefinition7);
		this.tileViewIssue.TileColumns.Add(tableColumnDefinition8);
		this.tileViewIssue.TileColumns.Add(tableColumnDefinition9);
		this.tileViewIssue.TileColumns.Add(tableColumnDefinition10);
		tableRowDefinition3.Length.Value = 32.0;
		tableRowDefinition4.Length.Value = 18.0;
		this.tileViewIssue.TileRows.Add(tableRowDefinition3);
		this.tileViewIssue.TileRows.Add(tableRowDefinition4);
		tableSpan5.RowSpan = 2;
		tableSpan6.ColumnIndex = 3;
		tableSpan6.ColumnSpan = 2;
		tableSpan6.RowIndex = 1;
		tableSpan7.ColumnIndex = 1;
		tableSpan7.ColumnSpan = 3;
		tableSpan8.ColumnIndex = 1;
		tableSpan8.ColumnSpan = 2;
		tableSpan8.RowIndex = 1;
		this.tileViewIssue.TileSpans.Add(tableSpan5);
		this.tileViewIssue.TileSpans.Add(tableSpan6);
		this.tileViewIssue.TileSpans.Add(tableSpan7);
		this.tileViewIssue.TileSpans.Add(tableSpan8);
		tileViewItemElement4.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement4.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement4.Column = this.tileViewColumn1;
		tileViewItemElement4.ColumnIndex = 2;
		tileViewItemElement4.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement4.RowIndex = 1;
		tileViewItemElement4.Text = "tvc_Title";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement5.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement5.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement5.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement5.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement5.Column = this.tileViewColumn3;
		tileViewItemElement5.ColumnIndex = 3;
		tileViewItemElement5.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement5.RowIndex = 1;
		tileViewItemElement5.Text = "tvc_DueDate";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement6.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement6.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement6.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement6.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement6.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement6.Column = this.tileViewColumn2;
		tileViewItemElement6.ColumnIndex = 2;
		tileViewItemElement6.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement6.Text = "tvc_Body";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewIssue.TileTemplate.Add(tileViewItemElement4);
		this.tileViewIssue.TileTemplate.Add(tileViewItemElement5);
		this.tileViewIssue.TileTemplate.Add(tileViewItemElement6);
		this.tileViewIssue.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewIssue.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewIssue.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewIssue.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewIssue.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tileViewColumn4.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn4.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewColumn4.Caption = "tvc_Group";
		this.tileViewColumn4.FieldName = "ObjectView";
		this.tileViewColumn4.Name = "tileViewColumn4";
		this.tileViewColumn4.Visible = true;
		this.tileViewColumn4.VisibleIndex = 3;
		this.xtraTabPageExTask.Controls.Add(this.grdTask);
		this.xtraTabPageExTask.FieldName = "";
		this.xtraTabPageExTask.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExTask.ImageOptions.Image");
		this.xtraTabPageExTask.IsSearchControl = false;
		this.xtraTabPageExTask.Name = "xtraTabPageExTask";
		this.xtraTabPageExTask.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExTask.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExTask.Text = "Task";
		this.xtraTabPageExTask.Tooltip = "Uncomplete Tasks";
		this.grdTask.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdTask.Location = new System.Drawing.Point(0, 0);
		this.grdTask.MainView = this.tileViewTask;
		this.grdTask.Name = "grdTask";
		this.grdTask.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit3 });
		this.grdTask.Size = new System.Drawing.Size(454, 589);
		this.grdTask.TabIndex = 1;
		this.grdTask.ToolTipController = this.toolTipController1;
		this.grdTask.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewTask });
		this.tileViewTask.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewTask.Appearance.Group.Options.UseFont = true;
		this.tileViewTask.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewTask.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewTask.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewTask.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewTask.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewTask.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tileViewColumn5, this.tileViewColumn6, this.tileViewColumn7, this.tileViewColumn8 });
		this.tileViewTask.ColumnSet.GroupColumn = this.tileViewColumn8;
		contextButton5.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton5.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton5.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton5.Name = "btnContextClear";
		contextButton5.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
		contextButton6.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton6.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image2");
		contextButton6.Name = "btnContextTypeE";
		contextButton6.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewTask.ContextButtons.Add(contextButton5);
		this.tileViewTask.ContextButtons.Add(contextButton6);
		this.tileViewTask.GridControl = this.grdTask;
		this.tileViewTask.Name = "tileViewTask";
		this.tileViewTask.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewTask.OptionsTiles.AllowItemHover = true;
		this.tileViewTask.OptionsTiles.AllowPressAnimation = false;
		this.tileViewTask.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewTask.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewTask.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewTask.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewTask.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewTask.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewTask.OptionsTiles.RowCount = 0;
		this.tileViewTask.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewTask.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tileViewColumn8, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition11.Length.Value = 19.0;
		tableColumnDefinition12.Length.Value = 35.0;
		tableColumnDefinition13.Length.Value = 279.0;
		tableColumnDefinition14.Length.Value = 126.0;
		tableColumnDefinition15.Length.Value = 17.0;
		this.tileViewTask.TileColumns.Add(tableColumnDefinition11);
		this.tileViewTask.TileColumns.Add(tableColumnDefinition12);
		this.tileViewTask.TileColumns.Add(tableColumnDefinition13);
		this.tileViewTask.TileColumns.Add(tableColumnDefinition14);
		this.tileViewTask.TileColumns.Add(tableColumnDefinition15);
		tableRowDefinition5.Length.Value = 32.0;
		tableRowDefinition6.Length.Value = 18.0;
		this.tileViewTask.TileRows.Add(tableRowDefinition5);
		this.tileViewTask.TileRows.Add(tableRowDefinition6);
		tableSpan9.RowSpan = 2;
		tableSpan10.ColumnIndex = 3;
		tableSpan10.ColumnSpan = 2;
		tableSpan10.RowIndex = 1;
		tableSpan11.ColumnIndex = 1;
		tableSpan11.ColumnSpan = 3;
		tableSpan12.ColumnIndex = 1;
		tableSpan12.ColumnSpan = 2;
		tableSpan12.RowIndex = 1;
		this.tileViewTask.TileSpans.Add(tableSpan9);
		this.tileViewTask.TileSpans.Add(tableSpan10);
		this.tileViewTask.TileSpans.Add(tableSpan11);
		this.tileViewTask.TileSpans.Add(tableSpan12);
		tileViewItemElement7.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement7.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement7.Column = this.tileViewColumn5;
		tileViewItemElement7.ColumnIndex = 2;
		tileViewItemElement7.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement7.RowIndex = 1;
		tileViewItemElement7.Text = "tvc_Title";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement8.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement8.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement8.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement8.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement8.Column = this.tileViewColumn7;
		tileViewItemElement8.ColumnIndex = 3;
		tileViewItemElement8.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement8.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement8.RowIndex = 1;
		tileViewItemElement8.Text = "tvc_DueDate";
		tileViewItemElement8.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement9.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement9.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement9.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement9.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement9.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement9.Column = this.tileViewColumn6;
		tileViewItemElement9.ColumnIndex = 2;
		tileViewItemElement9.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement9.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement9.Text = "tvc_Body";
		tileViewItemElement9.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewTask.TileTemplate.Add(tileViewItemElement7);
		this.tileViewTask.TileTemplate.Add(tileViewItemElement8);
		this.tileViewTask.TileTemplate.Add(tileViewItemElement9);
		this.tileViewTask.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewTask.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewTask.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewTask.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewTask.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tileViewColumn8.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn8.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewColumn8.Caption = "tvc_Group";
		this.tileViewColumn8.FieldName = "ObjectView";
		this.tileViewColumn8.Name = "tileViewColumn8";
		this.tileViewColumn8.Visible = true;
		this.tileViewColumn8.VisibleIndex = 3;
		this.xtraTabPageExEvent.Controls.Add(this.grdEvent);
		this.xtraTabPageExEvent.FieldName = "";
		this.xtraTabPageExEvent.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExEvent.ImageOptions.Image");
		this.xtraTabPageExEvent.IsSearchControl = false;
		this.xtraTabPageExEvent.Name = "xtraTabPageExEvent";
		this.xtraTabPageExEvent.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExEvent.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExEvent.Text = "Event";
		this.xtraTabPageExEvent.Tooltip = "Invited events";
		this.grdEvent.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdEvent.Location = new System.Drawing.Point(0, 0);
		this.grdEvent.MainView = this.tileViewEvent;
		this.grdEvent.Name = "grdEvent";
		this.grdEvent.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit4 });
		this.grdEvent.Size = new System.Drawing.Size(454, 589);
		this.grdEvent.TabIndex = 1;
		this.grdEvent.ToolTipController = this.toolTipController1;
		this.grdEvent.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewEvent });
		this.tileViewEvent.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewEvent.Appearance.Group.Options.UseFont = true;
		this.tileViewEvent.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewEvent.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewEvent.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewEvent.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewEvent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewEvent.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tileViewColumn9, this.tileViewColumn10, this.tileViewColumn11, this.tileViewColumn12 });
		this.tileViewEvent.ColumnSet.GroupColumn = this.tileViewColumn12;
		contextButton7.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton7.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton7.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton7.Name = "btnContextClear";
		contextButton7.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
		contextButton8.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton8.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image3");
		contextButton8.Name = "btnContextTypeE";
		contextButton8.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewEvent.ContextButtons.Add(contextButton7);
		this.tileViewEvent.ContextButtons.Add(contextButton8);
		this.tileViewEvent.GridControl = this.grdEvent;
		this.tileViewEvent.Name = "tileViewEvent";
		this.tileViewEvent.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewEvent.OptionsTiles.AllowItemHover = true;
		this.tileViewEvent.OptionsTiles.AllowPressAnimation = false;
		this.tileViewEvent.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewEvent.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewEvent.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewEvent.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewEvent.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewEvent.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewEvent.OptionsTiles.RowCount = 0;
		this.tileViewEvent.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewEvent.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tileViewColumn12, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition16.Length.Value = 19.0;
		tableColumnDefinition17.Length.Value = 35.0;
		tableColumnDefinition18.Length.Value = 279.0;
		tableColumnDefinition19.Length.Value = 126.0;
		tableColumnDefinition20.Length.Value = 17.0;
		this.tileViewEvent.TileColumns.Add(tableColumnDefinition16);
		this.tileViewEvent.TileColumns.Add(tableColumnDefinition17);
		this.tileViewEvent.TileColumns.Add(tableColumnDefinition18);
		this.tileViewEvent.TileColumns.Add(tableColumnDefinition19);
		this.tileViewEvent.TileColumns.Add(tableColumnDefinition20);
		tableRowDefinition7.Length.Value = 32.0;
		tableRowDefinition8.Length.Value = 18.0;
		this.tileViewEvent.TileRows.Add(tableRowDefinition7);
		this.tileViewEvent.TileRows.Add(tableRowDefinition8);
		tableSpan13.RowSpan = 2;
		tableSpan14.ColumnIndex = 3;
		tableSpan14.ColumnSpan = 2;
		tableSpan14.RowIndex = 1;
		tableSpan15.ColumnIndex = 1;
		tableSpan15.ColumnSpan = 3;
		tableSpan16.ColumnIndex = 1;
		tableSpan16.ColumnSpan = 2;
		tableSpan16.RowIndex = 1;
		this.tileViewEvent.TileSpans.Add(tableSpan13);
		this.tileViewEvent.TileSpans.Add(tableSpan14);
		this.tileViewEvent.TileSpans.Add(tableSpan15);
		this.tileViewEvent.TileSpans.Add(tableSpan16);
		tileViewItemElement10.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement10.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement10.Column = this.tileViewColumn9;
		tileViewItemElement10.ColumnIndex = 2;
		tileViewItemElement10.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement10.RowIndex = 1;
		tileViewItemElement10.Text = "tvc_Title";
		tileViewItemElement10.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement11.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement11.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement11.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement11.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement11.Column = this.tileViewColumn11;
		tileViewItemElement11.ColumnIndex = 3;
		tileViewItemElement11.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement11.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement11.RowIndex = 1;
		tileViewItemElement11.Text = "tvc_DueDate";
		tileViewItemElement11.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement12.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement12.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement12.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement12.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement12.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement12.Column = this.tileViewColumn10;
		tileViewItemElement12.ColumnIndex = 2;
		tileViewItemElement12.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement12.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement12.Text = "tvc_Body";
		tileViewItemElement12.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewEvent.TileTemplate.Add(tileViewItemElement10);
		this.tileViewEvent.TileTemplate.Add(tileViewItemElement11);
		this.tileViewEvent.TileTemplate.Add(tileViewItemElement12);
		this.tileViewEvent.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewEvent.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewEvent.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewEvent.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewEvent.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tileViewColumn12.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn12.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewColumn12.Caption = "tvc_Group";
		this.tileViewColumn12.FieldName = "ObjectView";
		this.tileViewColumn12.Name = "tileViewColumn12";
		this.tileViewColumn12.Visible = true;
		this.tileViewColumn12.VisibleIndex = 3;
		this.xtraTabPageExComments.Controls.Add(this.grdComment);
		this.xtraTabPageExComments.FieldName = "";
		this.xtraTabPageExComments.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExComments.ImageOptions.Image");
		this.xtraTabPageExComments.IsSearchControl = false;
		this.xtraTabPageExComments.Name = "xtraTabPageExComments";
		this.xtraTabPageExComments.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExComments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExComments.Text = "Comment";
		this.xtraTabPageExComments.Tooltip = "Unread comments";
		this.grdComment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdComment.Location = new System.Drawing.Point(0, 0);
		this.grdComment.MainView = this.tileViewComment;
		this.grdComment.Name = "grdComment";
		this.grdComment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit5 });
		this.grdComment.Size = new System.Drawing.Size(454, 589);
		this.grdComment.TabIndex = 1;
		this.grdComment.ToolTipController = this.toolTipController1;
		this.grdComment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewComment });
		this.tileViewComment.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewComment.Appearance.Group.Options.UseFont = true;
		this.tileViewComment.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewComment.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewComment.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewComment.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewComment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewComment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tileViewColumn13, this.tileViewColumn14, this.tileViewColumn15, this.tileViewColumn16 });
		this.tileViewComment.ColumnSet.GroupColumn = this.tileViewColumn16;
		contextButton9.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton9.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton9.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton9.Name = "btnContextClear";
		contextButton9.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
		contextButton10.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton10.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image4");
		contextButton10.Name = "btnContextTypeE";
		contextButton10.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewComment.ContextButtons.Add(contextButton9);
		this.tileViewComment.ContextButtons.Add(contextButton10);
		this.tileViewComment.GridControl = this.grdComment;
		this.tileViewComment.Name = "tileViewComment";
		this.tileViewComment.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewComment.OptionsTiles.AllowItemHover = true;
		this.tileViewComment.OptionsTiles.AllowPressAnimation = false;
		this.tileViewComment.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewComment.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewComment.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewComment.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewComment.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewComment.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewComment.OptionsTiles.RowCount = 0;
		this.tileViewComment.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewComment.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tileViewColumn16, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition21.Length.Value = 19.0;
		tableColumnDefinition22.Length.Value = 35.0;
		tableColumnDefinition23.Length.Value = 279.0;
		tableColumnDefinition24.Length.Value = 126.0;
		tableColumnDefinition25.Length.Value = 17.0;
		this.tileViewComment.TileColumns.Add(tableColumnDefinition21);
		this.tileViewComment.TileColumns.Add(tableColumnDefinition22);
		this.tileViewComment.TileColumns.Add(tableColumnDefinition23);
		this.tileViewComment.TileColumns.Add(tableColumnDefinition24);
		this.tileViewComment.TileColumns.Add(tableColumnDefinition25);
		tableRowDefinition9.Length.Value = 32.0;
		tableRowDefinition10.Length.Value = 18.0;
		this.tileViewComment.TileRows.Add(tableRowDefinition9);
		this.tileViewComment.TileRows.Add(tableRowDefinition10);
		tableSpan17.RowSpan = 2;
		tableSpan18.ColumnIndex = 3;
		tableSpan18.ColumnSpan = 2;
		tableSpan18.RowIndex = 1;
		tableSpan19.ColumnIndex = 1;
		tableSpan19.ColumnSpan = 3;
		tableSpan20.ColumnIndex = 1;
		tableSpan20.ColumnSpan = 2;
		tableSpan20.RowIndex = 1;
		this.tileViewComment.TileSpans.Add(tableSpan17);
		this.tileViewComment.TileSpans.Add(tableSpan18);
		this.tileViewComment.TileSpans.Add(tableSpan19);
		this.tileViewComment.TileSpans.Add(tableSpan20);
		tileViewItemElement13.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement13.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement13.Column = this.tileViewColumn13;
		tileViewItemElement13.ColumnIndex = 2;
		tileViewItemElement13.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement13.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement13.RowIndex = 1;
		tileViewItemElement13.Text = "tvc_Title";
		tileViewItemElement13.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement14.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement14.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement14.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement14.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement14.Column = this.tileViewColumn15;
		tileViewItemElement14.ColumnIndex = 3;
		tileViewItemElement14.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement14.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement14.RowIndex = 1;
		tileViewItemElement14.Text = "tvc_DueDate";
		tileViewItemElement14.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement15.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement15.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement15.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement15.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement15.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement15.Column = this.tileViewColumn14;
		tileViewItemElement15.ColumnIndex = 2;
		tileViewItemElement15.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement15.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement15.Text = "tvc_Body";
		tileViewItemElement15.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewComment.TileTemplate.Add(tileViewItemElement13);
		this.tileViewComment.TileTemplate.Add(tileViewItemElement14);
		this.tileViewComment.TileTemplate.Add(tileViewItemElement15);
		this.tileViewComment.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewComment.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewComment.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewComment.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewComment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tileViewColumn16.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn16.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewColumn16.Caption = "tvc_Group";
		this.tileViewColumn16.FieldName = "ObjectView";
		this.tileViewColumn16.Name = "tileViewColumn16";
		this.tileViewColumn16.Visible = true;
		this.tileViewColumn16.VisibleIndex = 3;
		this.xtraTabPageExAlert.Controls.Add(this.grdAlert);
		this.xtraTabPageExAlert.FieldName = "";
		this.xtraTabPageExAlert.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageExAlert.ImageOptions.Image");
		this.xtraTabPageExAlert.IsSearchControl = false;
		this.xtraTabPageExAlert.Name = "xtraTabPageExAlert";
		this.xtraTabPageExAlert.Size = new System.Drawing.Size(454, 589);
		this.xtraTabPageExAlert.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageExAlert.Text = "Alert";
		this.xtraTabPageExAlert.Tooltip = "All alert to user";
		this.grdAlert.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdAlert.Location = new System.Drawing.Point(0, 0);
		this.grdAlert.MainView = this.tileViewAlert;
		this.grdAlert.Name = "grdAlert";
		this.grdAlert.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoEdit6 });
		this.grdAlert.Size = new System.Drawing.Size(454, 589);
		this.grdAlert.TabIndex = 1;
		this.grdAlert.ToolTipController = this.toolTipController1;
		this.grdAlert.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewAlert });
		this.tileViewAlert.Appearance.Group.Font = new System.Drawing.Font("Tahoma", 12f);
		this.tileViewAlert.Appearance.Group.Options.UseFont = true;
		this.tileViewAlert.Appearance.Group.Options.UseTextOptions = true;
		this.tileViewAlert.Appearance.Group.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewAlert.Appearance.ItemHovered.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.tileViewAlert.Appearance.ItemHovered.Options.UseBackColor = true;
		this.tileViewAlert.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tileViewAlert.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tileViewColumn17, this.tileViewColumn18, this.tileViewColumn19, this.tileViewColumn20 });
		this.tileViewAlert.ColumnSet.GroupColumn = this.tileViewColumn20;
		contextButton11.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Far;
		contextButton11.Id = new System.Guid("d9e55bc5-ef74-4ba9-ab89-a4b142fd3f6c");
		contextButton11.ImageOptionsCollection.ItemNormal.Image = Apzon.Forms.Properties.Resources.round_delete_button;
		contextButton11.Name = "btnContextClear";
		contextButton11.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
		contextButton12.Id = new System.Guid("e298d463-4c12-43a9-97a5-6ddfb9996953");
		contextButton12.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image5");
		contextButton12.Name = "btnContextTypeE";
		contextButton12.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileViewAlert.ContextButtons.Add(contextButton11);
		this.tileViewAlert.ContextButtons.Add(contextButton12);
		this.tileViewAlert.GridControl = this.grdAlert;
		this.tileViewAlert.Name = "tileViewAlert";
		this.tileViewAlert.OptionsBehavior.AllowSmoothScrolling = true;
		this.tileViewAlert.OptionsTiles.AllowItemHover = true;
		this.tileViewAlert.OptionsTiles.AllowPressAnimation = false;
		this.tileViewAlert.OptionsTiles.IndentBetweenGroups = 14;
		this.tileViewAlert.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewAlert.OptionsTiles.ItemSize = new System.Drawing.Size(500, 66);
		this.tileViewAlert.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewAlert.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewAlert.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewAlert.OptionsTiles.RowCount = 0;
		this.tileViewAlert.OptionsTiles.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.TouchScrollBar;
		this.tileViewAlert.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.tileViewColumn20, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition26.Length.Value = 19.0;
		tableColumnDefinition27.Length.Value = 35.0;
		tableColumnDefinition28.Length.Value = 279.0;
		tableColumnDefinition29.Length.Value = 126.0;
		tableColumnDefinition30.Length.Value = 17.0;
		this.tileViewAlert.TileColumns.Add(tableColumnDefinition26);
		this.tileViewAlert.TileColumns.Add(tableColumnDefinition27);
		this.tileViewAlert.TileColumns.Add(tableColumnDefinition28);
		this.tileViewAlert.TileColumns.Add(tableColumnDefinition29);
		this.tileViewAlert.TileColumns.Add(tableColumnDefinition30);
		tableRowDefinition11.Length.Value = 32.0;
		tableRowDefinition12.Length.Value = 18.0;
		this.tileViewAlert.TileRows.Add(tableRowDefinition11);
		this.tileViewAlert.TileRows.Add(tableRowDefinition12);
		tableSpan21.RowSpan = 2;
		tableSpan22.ColumnIndex = 3;
		tableSpan22.ColumnSpan = 2;
		tableSpan22.RowIndex = 1;
		tableSpan23.ColumnIndex = 1;
		tableSpan23.ColumnSpan = 3;
		tableSpan24.ColumnIndex = 1;
		tableSpan24.ColumnSpan = 2;
		tableSpan24.RowIndex = 1;
		this.tileViewAlert.TileSpans.Add(tableSpan21);
		this.tileViewAlert.TileSpans.Add(tableSpan22);
		this.tileViewAlert.TileSpans.Add(tableSpan23);
		this.tileViewAlert.TileSpans.Add(tableSpan24);
		tileViewItemElement16.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement16.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement16.Column = this.tileViewColumn17;
		tileViewItemElement16.ColumnIndex = 2;
		tileViewItemElement16.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement16.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement16.RowIndex = 1;
		tileViewItemElement16.Text = "tvc_Title";
		tileViewItemElement16.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement17.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 8f);
		tileViewItemElement17.Appearance.Normal.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		tileViewItemElement17.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement17.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement17.Column = this.tileViewColumn19;
		tileViewItemElement17.ColumnIndex = 3;
		tileViewItemElement17.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement17.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement17.RowIndex = 1;
		tileViewItemElement17.Text = "tvc_DueDate";
		tileViewItemElement17.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement18.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 9.25f);
		tileViewItemElement18.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement18.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement18.Appearance.Normal.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		tileViewItemElement18.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement18.Column = this.tileViewColumn18;
		tileViewItemElement18.ColumnIndex = 2;
		tileViewItemElement18.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement18.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement18.Text = "tvc_Body";
		tileViewItemElement18.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		this.tileViewAlert.TileTemplate.Add(tileViewItemElement16);
		this.tileViewAlert.TileTemplate.Add(tileViewItemElement17);
		this.tileViewAlert.TileTemplate.Add(tileViewItemElement18);
		this.tileViewAlert.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tileViewNotification_ItemClick);
		this.tileViewAlert.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tileViewNotification_ContextButtonClick);
		this.tileViewAlert.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tileViewNotification_ContextButtonCustomize);
		this.tileViewAlert.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileViewNotification_ItemCustomize);
		this.tileViewAlert.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.tileViewNotification_CustomColumnDisplayText);
		this.tileViewColumn20.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn20.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tileViewColumn20.Caption = "tvc_Group";
		this.tileViewColumn20.FieldName = "ObjectView";
		this.tileViewColumn20.Name = "tileViewColumn20";
		this.tileViewColumn20.Visible = true;
		this.tileViewColumn20.VisibleIndex = 3;
		base.Appearance.BackColor = System.Drawing.Color.Gainsboro;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.xtraTabControlNotification);
		base.Name = "NotificationContrainer";
		base.Size = new System.Drawing.Size(486, 591);
		base.Load += new System.EventHandler(this.NotificationContrainer_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdNotification).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewNotification).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlNotification).EndInit();
		this.xtraTabControlNotification.ResumeLayout(false);
		this.xtraTabPageExAll.ResumeLayout(false);
		this.xtraTabPageExIssue.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdIssue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewIssue).EndInit();
		this.xtraTabPageExTask.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdTask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewTask).EndInit();
		this.xtraTabPageExEvent.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdEvent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewEvent).EndInit();
		this.xtraTabPageExComments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdComment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewComment).EndInit();
		this.xtraTabPageExAlert.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdAlert).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewAlert).EndInit();
		base.ResumeLayout(false);
	}
}
