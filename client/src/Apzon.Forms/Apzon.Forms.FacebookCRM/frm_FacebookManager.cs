using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Facebook_CRM;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraTab;
using Facebook;
using SAPbobsCOM;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace Apzon.Forms.FacebookCRM;

public class frm_FacebookManager : XtraFormEx
{
	private string _fUserAccessToken = "";

	private string _fPageAccessToken = "";

	private string _fUserId = "";

	private string _pageId = "";

	private const string AppId = "607026383425731";

	private const string ExtendedPermissions = "public_profile,manage_pages,read_page_mailboxes,publish_pages,pages_messaging";

	private string _nextDataConverUrl = "";

	private DataTable _dtPageUser = new DataTable();

	private string _defaultAvatar = "";

	private string _cardCode = "";

	private DateTime _lastUpdateTime = DateTime.MinValue;

	private string _lastMessageId = "";

	private IContainer components;

	private LookUpEdit txtFacebookPage;

	private SimpleButtonEx btnCheck;

	private TextEditEx txtFacebookUserName;

	private SimpleButtonEx btnLogin;

	private SimpleButtonEx btnActive;

	private SimpleButtonEx btnAccess;

	private SplitContainerControl splitUser;

	private XtraTabControlEx xtraTabUser;

	private XtraTabPageEx xtraTabPageAll;

	private SplitContainerControl splitMesage;

	private XtraTabControlEx xtraTabOrder;

	private XtraTabPageEx xtraTabPageCustomer;

	private XtraTabPageEx xtraTabPageOrder;

	private SimpleButtonEx btnCancel;

	private LabelLine lbFacebookUser;

	private LabelLine lbFacebookPage;

	private XtraTabPageEx xtraTabPageComment;

	private RadChat radChatControl;

	private RadThemeManager radThemeManager1;

	private GridControlEx grdMessage;

	private TileView tileViewMessage;

	private TileViewColumn tvcMessageAvatar;

	private TileViewColumn tvcMessageName;

	private RepositoryItemPictureEdit rMessageAvatar;

	private LabelLine lbInfoFacebookLocation;

	private LabelLine lbInfoFacebookEmail;

	private LabelLine lbInfoFacebookPhoneNumber;

	private LabelLine lbInfoFacebookName;

	private PictureEdit pictureInfoUser;

	private LabelControl labelControl3;

	private LabelControl labelControl2;

	private LabelControl labelControl1;

	private LabelControl labelControl4;

	private LabelLine lbInfoBirthday;

	private GridControlEx grdOrderItem;

	private GridViewEx gvOrderItem;

	private TextEditExPrice txtOrderTotal;

	private TextEditExPrice txtOrderFreight;

	private LabelLine lbOrderTotal;

	private LabelLine lbOrderLogisticAmount;

	private LookUpEditEx txtOrderCity;

	private LookUpEditEx txtOrderCountry;

	private TextEditEx txtOrderAddress;

	private TextEditEx txtOrderPhoneNumber;

	private TextEditEx txtOrderName;

	private GridColumnEx gcOrderItemName;

	private GridColumnEx gcOrderPriceBefDi;

	private RepositoryItemTextEditExPrice rPrice;

	private GridColumnEx gcOrderDiscPrcnt;

	private GridColumnEx gcOrderQuantity;

	private RepositoryItemCalcEdit rCalEdit;

	private TextEditEx txtOrderNote;

	private DateEditEx txtOrderDelDate;

	private SimpleButtonEx btnOrderCreateOrder;

	private LookUpEditEx txtOrderPriceList;

	private ContextMenuStrip contextMenuStripOrderItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit rOrderItem;

	private GridColumnEx gcOrderItemCode;

	private GridColumnEx gcOrderLineTotal;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private Timer timer1;

	private TileViewColumn tvcMessageUnread;

	public Author CurrentAuthor { get; set; }

	public Author ClientAuthor { get; set; }

	public frm_FacebookManager()
	{
		InitializeComponent();
	}

	private async void frm_FacebookManager_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			txtOrderTotal.Currency = GlobalData.MainCurrency;
			txtOrderFreight.Currency = GlobalData.MainCurrency;
			LoadDataPriceList();
			DataTable dataTable = HttpClient.BusinessMaster.LoadComboCountry();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
			}
			txtOrderCountry.Properties.DataSource = dataTable;
			radChatControl.ChatElement.SendMessage += ChatElement_SendMessage;
			grdMessage.Controls.OfType<VCrkScrollBar>().FirstOrDefault().ValueChanged += ScrollBar_ValueChanged;
			using (Image image = Image.FromFile(Application.StartupPath + "\\UnknowAvatar.png"))
			{
				using MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				byte[] inArray = memoryStream.ToArray();
				_defaultAvatar = Convert.ToBase64String(inArray);
			}
			DataTable dtAccessUser = HttpClient.FacebookManagerClient.GetDataAccessUser(GlobalData.UserName);
			bool flag = !dtAccessUser.IsNotNull();
			if (!flag)
			{
				flag = !(await CheckUserAccessToken(Function.ToString(dtAccessUser.Rows[0]["AccessToken"])));
			}
			if (flag)
			{
				frm_FacebookLoginDialog frm_FacebookLoginDialog2 = new frm_FacebookLoginDialog("607026383425731", "public_profile,manage_pages,read_page_mailboxes,publish_pages,pages_messaging");
				frm_FacebookLoginDialog2.ShowDialog(this);
				if (frm_FacebookLoginDialog2.FacebookOAuthResult != null && frm_FacebookLoginDialog2.FacebookOAuthResult.IsSuccess)
				{
					await GetUserToken(frm_FacebookLoginDialog2.FacebookOAuthResult);
				}
			}
			else
			{
				txtFacebookUserName.EditValue = Function.ToString(dtAccessUser.Rows[0]["F_UserName"]);
				_fUserId = Function.ToString(dtAccessUser.Rows[0]["F_UserId"]);
				_fUserAccessToken = Function.ToString(dtAccessUser.Rows[0]["AccessToken"]);
			}
			if (!string.IsNullOrEmpty(_fUserAccessToken))
			{
				await GetListAccessPage(GlobalData.UserName);
				if (!string.IsNullOrEmpty(_fPageAccessToken))
				{
					await LoadDataConversationPage(_pageId, _fPageAccessToken, refreshData: true);
				}
			}
			timer1.Start();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void ChatElement_SendMessage(object sender, SendMessageEventArgs e)
	{
		try
		{
			FacebookClient facebookClient = new FacebookClient(_fPageAccessToken);
			string text = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_ConvrId"));
			if (!string.IsNullOrEmpty(text))
			{
				string path = text + "/messages?message=";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (e.Message is ChatTextMessage chatTextMessage)
				{
					dictionary["message"] = chatTextMessage.Message;
					await facebookClient.PostTaskAsync(path, dictionary);
					radChatControl.ChatElement.AddMessage(new ChatTextMessage(chatTextMessage.Message, CurrentAuthor, DateTime.Now));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ScrollBar_ValueChanged(object sender, EventArgs e)
	{
		IScrollBar scrollBar = sender as IScrollBar;
		if (scrollBar.ViewInfo.VisibleValue >= scrollBar.ViewInfo.VisibleMaximum - 1 && !string.IsNullOrEmpty(_nextDataConverUrl))
		{
			LoadDataConversationPage(_pageId, _fPageAccessToken, refreshData: false);
		}
	}

	private async Task GetUserToken(FacebookOAuthResult facebookOAuthResult)
	{
		_ = 2;
		try
		{
			_fUserAccessToken = facebookOAuthResult.AccessToken;
			FacebookClient fb = new FacebookClient(_fUserAccessToken);
			dynamic userInfo = await fb.GetTaskAsync("me?field=id,name");
			await SaveUserAccessToken(_fUserAccessToken, userInfo);
			_fUserId = userInfo["id"];
			txtFacebookUserName.Text = userInfo["name"];
			await GetPageInfoFromFacebookUser(fb, _fUserId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetPageInfoFromFacebookUser(FacebookClient fb, string userId)
	{
		_ = 2;
		try
		{
			dynamic val = await fb.GetTaskAsync("me/accounts");
			DataTable dtFacebookPage = new DataTable
			{
				Columns = 
				{
					{
						"UserCode",
						typeof(string)
					},
					{
						"F_UserId",
						typeof(string)
					},
					{
						"F_PageId",
						typeof(string)
					},
					{
						"PageName",
						typeof(string)
					},
					{
						"AccessPageToken",
						typeof(string)
					},
					{
						"Actived",
						typeof(string)
					},
					{
						"LastAccessDate",
						typeof(DateTime)
					},
					{
						"LastAccessTime",
						typeof(int)
					},
					{
						"Avatar",
						typeof(string)
					}
				}
			};
			foreach (dynamic item in val.data.ToArray())
			{
				DataRow dr = dtFacebookPage.NewRow();
				dr["UserCode"] = GlobalData.UserName;
				dr["F_UserId"] = userId;
				dr["F_PageId"] = (object)item["id"];
				dr["PageName"] = (object)item["name"];
				dr["AccessPageToken"] = (object)item["access_token"];
				try
				{
					dynamic val2 = (await fb.GetTaskAsync(string.Format("/{0}/picture.width(64).height(64)", Function.ToString(item["id"])))).profile_pic;
					if ((!string.IsNullOrEmpty(val2)))
					{
						dynamic val3 = WebRequest.Create(val2);
						dynamic response = val3.GetResponse();
						using ((IDisposable)response)
						{
							dynamic responseStream = response.GetResponseStream();
							using ((IDisposable)responseStream)
							{
								dynamic val4 = Image.FromStream(responseStream);
								using ((IDisposable)val4)
								{
									using MemoryStream memoryStream = new MemoryStream();
									val4.Save(memoryStream, val4.RawFormat);
									string value = Convert.ToBase64String(memoryStream.ToArray());
									dr["Avatar"] = value;
								}
							}
						}
					}
				}
				catch (Exception)
				{
					dr["Avatar"] = _defaultAvatar;
				}
				dtFacebookPage.Rows.Add(dr);
			}
			dtFacebookPage.AcceptChanges();
			if (!dtFacebookPage.IsNotNull())
			{
				return;
			}
			try
			{
				HttpResult httpResult = await HttpClient.FacebookManagerClient.SavePageUserInfo(dtFacebookPage);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
				WinCommons.ErrorLineNumber(ex2);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.ErrorLineNumber(ex3);
		}
	}

	private async Task SaveUserAccessToken(string accessToken, dynamic userInfo)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("UserCode", typeof(string));
			dataTable.Columns.Add("F_UserId", typeof(string));
			dataTable.Columns.Add("F_UserName", typeof(string));
			dataTable.Columns.Add("AccessToken", typeof(string));
			dataTable.Columns.Add("LastAccessDate", typeof(DateTime));
			dataTable.Columns.Add("LastAccessTime", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["UserCode"] = GlobalData.UserName;
			dataRow["F_UserId"] = (object)userInfo["id"];
			dataRow["F_UserName"] = (object)userInfo["name"];
			dataRow["AccessToken"] = accessToken;
			dataRow["LastAccessDate"] = DateTime.Now.Date;
			dataRow["LastAccessTime"] = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			HttpResult httpResult = await HttpClient.FacebookManagerClient.SaveUserAccessToken(dataTable);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListAccessPage(string userCode)
	{
		DataTable dataTable = await HttpClient.FacebookManagerClient.GetListUserAccessPage(userCode);
		txtFacebookPage.Properties.DataSource = dataTable;
		if (dataTable.IsNotNull())
		{
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Actived"]).Equals("Y"));
			if (dataRow != null)
			{
				_fPageAccessToken = Function.ToString(dataRow["AccessPageToken"]);
				_pageId = Function.ToString(dataRow["F_PageId"]);
			}
			else
			{
				_fPageAccessToken = Function.ToString(dataTable.Rows[0]["AccessPageToken"]);
				_pageId = Function.ToString(dataTable.Rows[0]["F_PageId"]);
			}
		}
		txtFacebookPage.EditValue = _pageId;
	}

	private async Task LoadDataConversationPage(string pageId, string accessPageToken, bool refreshData)
	{
		_ = 2;
		try
		{
			if (_dtPageUser == null || _dtPageUser.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				return;
			}
			DataTable dtConverInbox = grdMessage.DataSourceTable;
			if (dtConverInbox == null || dtConverInbox.Columns.Count == 0)
			{
				dtConverInbox = new DataTable
				{
					Columns = 
					{
						{
							"F_ConvrId",
							typeof(string)
						},
						{
							"F_UserId",
							typeof(string)
						},
						{
							"Avatar",
							typeof(Image)
						},
						{
							"F_UserName",
							typeof(string)
						},
						{
							"UnreadCount",
							typeof(int)
						}
					}
				};
			}
			if (refreshData)
			{
				dtConverInbox.Rows.Clear();
			}
			bool getLastUpdateTime = false;
			FacebookClient fbPage = new FacebookClient(accessPageToken);
			dynamic val2;
			if (refreshData)
			{
				getLastUpdateTime = true;
				dynamic val = await fbPage.GetTaskAsync($"/{pageId}?fields=conversations.limit(20){{participants, unread_count, updated_time}}");
				val2 = val.conversations.data;
				if (val.conversations.paging != null && val.conversations.paging.next != null)
				{
					_nextDataConverUrl = val.conversations.paging.next;
				}
				else
				{
					_nextDataConverUrl = "";
				}
			}
			else
			{
				if (string.IsNullOrEmpty(_nextDataConverUrl))
				{
					return;
				}
				dynamic val3 = await fbPage.GetTaskAsync(_nextDataConverUrl);
				val2 = val3.data;
				if (val3.paging != null && val3.paging.next != null)
				{
					_nextDataConverUrl = val3.paging.next;
				}
				else
				{
					_nextDataConverUrl = "";
				}
			}
			bool isUpdateData = false;
			foreach (dynamic converData in val2.ToArray())
			{
				if (getLastUpdateTime)
				{
					_lastUpdateTime = DateTime.Parse(converData.updated_time).ToUniversalTime();
					getLastUpdateTime = false;
				}
				foreach (dynamic userData in converData.participants.data.ToArray())
				{
					dynamic userId = Function.ToString(userData.id);
					if (userId.Equals(pageId))
					{
						continue;
					}
					DataRow pageUser = _dtPageUser.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_UserId"]).Equals(userId));
					if (pageUser == null)
					{
						pageUser = _dtPageUser.NewRow();
						pageUser["F_PageId"] = pageId;
						pageUser["F_ConvrId"] = (object)converData.id;
						pageUser["F_UserId"] = (object)userData.id;
						pageUser["Avatar"] = _defaultAvatar;
						pageUser["F_UserName"] = (object)userData.name;
						_dtPageUser.Rows.Add(pageUser);
						isUpdateData = true;
					}
					string img = Function.ToString(pageUser["Avatar"]);
					if (string.IsNullOrEmpty(Function.ToString(pageUser["LinkAvatar"])))
					{
						try
						{
							dynamic val4 = (await fbPage.GetTaskAsync(string.Format("/{0}?fields=profile_pic.width(100).height(120)", userId))).profile_pic;
							if ((!string.IsNullOrEmpty(val4)))
							{
								dynamic val5 = WebRequest.Create(val4);
								dynamic response = val5.GetResponse();
								using ((IDisposable)response)
								{
									dynamic responseStream = response.GetResponseStream();
									using ((IDisposable)responseStream)
									{
										dynamic val6 = Image.FromStream(responseStream);
										using ((IDisposable)val6)
										{
											using MemoryStream memoryStream = new MemoryStream();
											val6.Save(memoryStream, val6.RawFormat);
											byte[] inArray = memoryStream.ToArray();
											img = (string)(pageUser["Avatar"] = Convert.ToBase64String(inArray));
											pageUser["LinkAvatar"] = (object)val4;
											isUpdateData = true;
										}
									}
								}
							}
						}
						catch (Exception)
						{
						}
					}
					DataRow dataRow = dtConverInbox.NewRow();
					dataRow["F_ConvrId"] = (object)converData.id;
					dataRow["F_UserId"] = (object)userData.id;
					dataRow["F_UserName"] = (object)userData.name;
					dataRow["Avatar"] = LoadImageFromBase64(img);
					dynamic val7 = Function.ParseInt(converData.unread_count);
					if (val7 > 0)
					{
						dataRow["UnreadCount"] = (object)val7;
					}
					dtConverInbox.Rows.Add(dataRow);
				}
			}
			_dtPageUser.AcceptChanges();
			dtConverInbox.AcceptChanges();
			if (isUpdateData)
			{
				HttpClient.FacebookManagerClient.SaveDataPageUser(pageId, _dtPageUser);
			}
			grdMessage.DataSource = dtConverInbox;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.None, ex2.Message);
		}
	}

	private Image LoadImageFromBase64(string imageBase64)
	{
		Image image = null;
		try
		{
			if (!string.IsNullOrEmpty(imageBase64))
			{
				using MemoryStream stream = new MemoryStream(Convert.FromBase64String(imageBase64));
				image = Image.FromStream(stream);
			}
			if (image == null)
			{
				image = Image.FromFile(Application.StartupPath + "\\UnknowAvatar.png");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return image;
	}

	private async Task GetNextMessage(string nextURL, FacebookClient fb, DataTable dtPageMessage, string pageId)
	{
		try
		{
			while (!string.IsNullOrEmpty(nextURL))
			{
				dynamic val = await fb.GetTaskAsync(nextURL);
				dynamic converId = Function.ToString(val.id);
				foreach (dynamic item in val.data)
				{
					DateTime dateTime = DateTime.Parse(Function.ToString(item.created_time)).ToUniversalTime();
					dynamic custId = Function.ToString(item.from.id);
					dynamic messageId = Function.ToString(item.id);
					if (dtPageMessage.AsEnumerable().Any((DataRow t) => Function.ToString(t["F_PageId"]).Equals(pageId) && Function.ToString(t["F_ConvrId"]).Equals(converId) && Function.ToString(t["F_CustId"]).Equals(custId) && Function.ToString(t["MessageId"]).Equals(messageId)))
					{
						dtPageMessage.AcceptChanges();
						return;
					}
					DataRow dataRow = dtPageMessage.NewRow();
					dataRow["F_PageId"] = pageId;
					dataRow["F_ConvrId"] = (object)converId;
					dataRow["F_CustId"] = (object)item.from.id;
					dataRow["MessageId"] = (object)item.id;
					dataRow["ConvrType"] = "M";
					dataRow["CustName"] = (object)item.from.name;
					dataRow["Message"] = (object)item.message;
					dataRow["CreateDate"] = dateTime.Date;
					dataRow["CreateTime"] = dateTime.Hour * 100 + dateTime.Minute;
					dtPageMessage.Rows.Add(dataRow);
				}
				nextURL = ((!((val.paging != null && val.paging.next != null) ? true : false)) ? "" : ((string)val.paging.next));
			}
			dtPageMessage.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<bool> CheckUserAccessToken(string userAccessToken)
	{
		try
		{
			await new FacebookClient().GetTaskAsync($"/me?access_token={userAccessToken}");
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	private async void btnLogin_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			frm_FacebookLoginDialog frm_FacebookLoginDialog2 = new frm_FacebookLoginDialog("607026383425731", "public_profile,manage_pages,read_page_mailboxes,publish_pages,pages_messaging");
			frm_FacebookLoginDialog2.ShowDialog(this);
			if (frm_FacebookLoginDialog2.FacebookOAuthResult != null && frm_FacebookLoginDialog2.FacebookOAuthResult.IsSuccess)
			{
				await GetUserToken(frm_FacebookLoginDialog2.FacebookOAuthResult);
			}
			if (!string.IsNullOrEmpty(_fUserAccessToken))
			{
				await GetListAccessPage(GlobalData.UserName);
				await LoadDataConversationPage(_pageId, _fPageAccessToken, refreshData: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileViewMessage_MouseWheel(object sender, MouseEventArgs e)
	{
	}

	private void txtFacebookPage_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			_dtPageUser = HttpClient.FacebookManagerClient.GetDataPageUser(Function.ToString(txtFacebookPage.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void tileViewMessage_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			_lastMessageId = "";
			tileViewMessage.SetFocusedRowCellValue("UnreadCount", DBNull.Value);
			radChatControl.HideOverlay();
			radChatControl.ChatElement.SuggestedActionsElement.ClearActions();
			radChatControl.ChatElement.MessagesViewElement.Items.Clear();
			DataRow dataRow = (txtFacebookPage.Properties.DataSource as DataTable).AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_PageId"]).Equals(Function.ToString(txtFacebookPage.EditValue)));
			CurrentAuthor = new Author(LoadImageFromBase64(Function.ToString(dataRow["Avatar"])), txtFacebookPage.Text);
			Image image = tileViewMessage.GetFocusedRowCellValue("Avatar") as Image;
			string text = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_UserName"));
			ClientAuthor = new Author(image, text);
			radChatControl.ChatElement.Author = CurrentAuthor;
			pictureInfoUser.Image = new Bitmap(image, new Size(100, 120));
			lbInfoFacebookName.Utext = text;
			await LoadInfoFacebookUser(Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_UserId")));
			await LoadDataMessageInConversation(Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_ConvrId")), _fPageAccessToken);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInfoFacebookUser(string facebookUserId)
	{
		_ = 1;
		try
		{
			_cardCode = "";
			LabelLine labelLine = lbInfoFacebookEmail;
			LabelLine labelLine2 = lbInfoFacebookPhoneNumber;
			LabelLine labelLine3 = lbInfoFacebookLocation;
			string text = (lbInfoBirthday.Utext = "Chưa có thông tin");
			string text3 = (labelLine3.Utext = text);
			string utext = (labelLine2.Utext = text3);
			labelLine.Utext = utext;
			TextEditEx textEditEx = txtOrderName;
			TextEditEx textEditEx2 = txtOrderAddress;
			LookUpEditEx lookUpEditEx = txtOrderCity;
			LookUpEditEx lookUpEditEx2 = txtOrderCountry;
			TextEditExPrice textEditExPrice = txtOrderFreight;
			TextEditEx textEditEx3 = txtOrderNote;
			TextEditEx textEditEx4 = txtOrderPhoneNumber;
			object obj = (txtOrderTotal.EditValue = null);
			object obj3 = (textEditEx4.EditValue = obj);
			object obj5 = (textEditEx3.EditValue = obj3);
			object obj7 = (textEditExPrice.EditValue = obj5);
			object obj9 = (lookUpEditEx2.EditValue = obj7);
			object obj11 = (lookUpEditEx.EditValue = obj9);
			object editValue = (textEditEx2.EditValue = obj11);
			textEditEx.EditValue = editValue;
			txtOrderDelDate.EditValue = DateTime.Now.Date;
			grdOrderItem.DataSourceTable.Rows.Clear();
			AddNewRowItem();
			string email = "";
			string adress = "";
			string country = "";
			string state = "";
			string phone = "";
			int priceList = -2;
			DataTable dataTable = await HttpClient.FacebookManagerClient.GetDataFacebookCustomer(facebookUserId);
			string text6;
			if (dataTable.IsNotNull())
			{
				email = Function.ToString(dataTable.Rows[0]["E_Mail"]);
				adress = Function.ToString(dataTable.Rows[0]["Street"]);
				country = Function.ToString(dataTable.Rows[0]["Country"]);
				state = Function.ToString(dataTable.Rows[0]["State"]);
				phone = Function.ToString(dataTable.Rows[0]["Phone1"]);
				text6 = Function.ToString(dataTable.Rows[0]["CardName"]);
				priceList = Function.ParseInt(dataTable.Rows[0]["ListNum"]);
				_cardCode = Function.ToString(dataTable.Rows[0]["CardCode"]);
			}
			else
			{
				dynamic val = await new FacebookClient(_fPageAccessToken).GetTaskAsync($"{facebookUserId}?fields=address,birthday,email,location");
				if ((!string.IsNullOrEmpty(Function.ToString(val.email))))
				{
					email = Function.ToString(val.email);
				}
				if (val.location != null && !string.IsNullOrEmpty(Function.ToString(val.location.name)))
				{
					adress = Function.ToString(val.location.name);
				}
				text6 = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_UserName"));
				DataTable dataTable2 = txtOrderPriceList.Properties.DataSource as DataTable;
				if (dataTable2.IsNotNull())
				{
					priceList = Function.ParseInt(dataTable2.Compute("MIN(Code)", "Code > 0"));
				}
			}
			txtOrderPriceList.EditValue = priceList;
			if (!string.IsNullOrEmpty(email))
			{
				lbInfoFacebookEmail.Utext = email;
			}
			if (!string.IsNullOrEmpty(adress))
			{
				lbInfoFacebookLocation.Utext = adress;
				txtOrderAddress.EditValue = adress;
			}
			if (!string.IsNullOrEmpty(country))
			{
				txtOrderCountry.EditValue = country;
			}
			if (!string.IsNullOrEmpty(state))
			{
				txtOrderCity.EditValue = state;
			}
			if (!string.IsNullOrEmpty(phone))
			{
				lbInfoFacebookPhoneNumber.Utext = phone;
				txtOrderPhoneNumber.EditValue = phone;
			}
			if (!string.IsNullOrEmpty(text6))
			{
				txtOrderName.EditValue = text6;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task LoadDataMessageInConversation(string converId, string accessPageToken)
	{
		dynamic val = await new FacebookClient(accessPageToken).GetTaskAsync($"/{converId}?fields=messages{{message,from{{id}},created_time,shares{{link}},attachments}}");
		List<ChatMessage> list = new List<ChatMessage>();
		bool flag = true;
		foreach (dynamic item in val.messages.data.ToArray())
		{
			if (flag)
			{
				_lastMessageId = Function.ToString(item.id);
				flag = false;
			}
			dynamic val2 = Function.ToString(item.from.id);
			dynamic val3 = Function.ToString(item.message);
			if ((!string.IsNullOrEmpty(val3)))
			{
				list.Add(new ChatTextMessage(Function.ToString(item.message), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
			}
			else if (item.attachments != null)
			{
				foreach (dynamic item2 in item.attachments.data.ToArray())
				{
					if (!((string)Function.ToString(item2.mime_type)).StartsWith("image"))
					{
						list.Add(new ChatTextMessage("", (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
						continue;
					}
					dynamic val4 = Function.ToString(item2.image_data.url);
					Image image = null;
					try
					{
						if ((!string.IsNullOrEmpty(val4)))
						{
							dynamic val5 = WebRequest.Create(val4);
							dynamic response = val5.GetResponse();
							using ((IDisposable)response)
							{
								dynamic responseStream = response.GetResponseStream();
								using ((IDisposable)responseStream)
								{
									image = Image.FromStream(responseStream);
								}
							}
						}
					}
					catch (Exception)
					{
					}
					list.Add(new ChatMediaMessage(image, new Size(Function.ParseInt(item2.image_data.width), Function.ParseInt(item2.image_data.height)), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
				}
			}
			else if (item.shares != null)
			{
				foreach (dynamic item3 in item.shares.data.ToArray())
				{
					list.Add(new ChatTextMessage(Function.ToString(item3.link), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
				}
			}
			else
			{
				list.Add(new ChatTextMessage("", (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
			}
		}
		for (int num = list.Count - 1; num > -1; num--)
		{
			radChatControl.ChatElement.AddMessage(list[num]);
		}
	}

	private async Task UpdateDataMessageInConversation(string converId, string accessPageToken)
	{
		dynamic val = await new FacebookClient(accessPageToken).GetTaskAsync($"/{converId}?fields=messages{{message,from{{id}},created_time,shares{{link}},attachments}}");
		List<ChatMessage> list = new List<ChatMessage>();
		bool flag = true;
		string lastMessageId = _lastMessageId;
		foreach (dynamic item in val.messages.data.ToArray())
		{
			if (_lastMessageId.Equals(Function.ToString(item.id)))
			{
				break;
			}
			if (flag)
			{
				lastMessageId = Function.ToString(item.id);
				flag = false;
			}
			dynamic val2 = Function.ToString(item.from.id);
			dynamic val3 = Function.ToString(item.message);
			if ((!string.IsNullOrEmpty(val3)))
			{
				list.Add(new ChatTextMessage(Function.ToString(item.message), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
			}
			else if (item.attachments != null)
			{
				foreach (dynamic item2 in item.attachments.data.ToArray())
				{
					if (!((string)Function.ToString(item2.mime_type)).StartsWith("image"))
					{
						list.Add(new ChatTextMessage("", (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
						continue;
					}
					dynamic val4 = Function.ToString(item2.image_data.url);
					Image image = null;
					try
					{
						if ((!string.IsNullOrEmpty(val4)))
						{
							dynamic val5 = WebRequest.Create(val4);
							dynamic response = val5.GetResponse();
							using ((IDisposable)response)
							{
								dynamic responseStream = response.GetResponseStream();
								using ((IDisposable)responseStream)
								{
									image = Image.FromStream(responseStream);
								}
							}
						}
					}
					catch (Exception)
					{
					}
					list.Add(new ChatMediaMessage(image, new Size(Function.ParseInt(item2.image_data.width), Function.ParseInt(item2.image_data.height)), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
				}
			}
			else if (item.shares != null)
			{
				foreach (dynamic item3 in item.shares.data.ToArray())
				{
					list.Add(new ChatTextMessage(Function.ToString(item3.link), (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
				}
			}
			else
			{
				list.Add(new ChatTextMessage("", (val2.Equals(_pageId)) ? CurrentAuthor : ClientAuthor, Function.ParseDateTimes(item.created_time)));
			}
		}
		for (int num = list.Count - 1; num > -1; num--)
		{
			radChatControl.ChatElement.AddMessage(list[num]);
		}
		_lastMessageId = lastMessageId;
	}

	private async void btnOrderCreateOrder_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			string userId = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_UserId"));
			if (string.IsNullOrEmpty(userId))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCustommer, "");
				return;
			}
			DataTable dtItem = grdOrderItem.DataSourceTable;
			if (!dtItem.IsNotNull() || dtItem.AsEnumerable().All((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
			{
				WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				DocumentsImpl doc = new DocumentsImpl
				{
					CardCode = _cardCode,
					CardName = txtOrderName.Text,
					BPLId = -1,
					DocStatus = "O",
					DocDate = DateTime.Now.Date,
					TaxDate = DateTime.Now.Date,
					Series = -1,
					Lines = new List<Document_LinesImpl>(),
					DocRate = 1.0,
					Comments = txtOrderNote.Text
				};
				int num = 0;
				for (int num2 = 0; num2 < dtItem.Rows.Count; num2++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dtItem.Rows[num2]["ItemCode"])))
					{
						if (string.IsNullOrEmpty(Function.ToString(dtItem.Rows[num2]["WhsCode"])))
						{
							WinCommons.Error(Enums.MessageCode.WarehouseCannotBeNull, "");
							return;
						}
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl
						{
							ItemCode = Function.ToString(dtItem.Rows[num2]["ItemCode"]),
							Dscription = Function.ToString(dtItem.Rows[num2]["Dscription"]),
							Quantity = Function.ParseDouble(dtItem.Rows[num2]["Quantity"]),
							Price = Function.ParseDouble(dtItem.Rows[num2]["Price"]),
							Rate = 1.0,
							LineNum = num,
							Currency = Function.ToString(dtItem.Rows[num2]["Currency"]),
							PriceBefDi = Function.ParseDouble(dtItem.Rows[num2]["PriceBefDi"]),
							WhsCode = Function.ToString(dtItem.Rows[num2]["WhsCode"]),
							UomCode = Function.ToString(dtItem.Rows[num2]["UomCode"]),
							UoMName = Function.ToString(dtItem.Rows[num2]["unitMsr"]),
							UseBaseUn = Function.ToString(dtItem.Rows[num2]["UseBaseUn"]),
							UomEntry = Function.ParseInt(dtItem.Rows[num2]["UomEntry"]),
							NumPerMsr = Function.ParseDouble(dtItem.Rows[num2]["NumPerMsr"]),
							ShipDate = Function.ParseDateTimes(dtItem.Rows[num2]["ShipDate"]),
							VatGroup = Function.ToString(dtItem.Rows[num2]["VatGroup"]),
							TaxPercentagePerRow = Function.ParseDouble(dtItem.Rows[num2]["VatPrcnt"]),
							Project = Function.ToString(dtItem.Rows[num2]["Project"])
						};
						if (Function.ParseDouble(txtOrderFreight.EditValue) > 0.0)
						{
							doc.Expenses = new List<DocumentsAdditionalExpensesImpl>
							{
								new DocumentsAdditionalExpensesImpl
								{
									ExpnsCode = 1,
									Status = "O",
									LineTotal = Function.ParseDouble(txtOrderFreight.EditValue)
								}
							};
						}
						if (string.IsNullOrEmpty(document_LinesImpl.Currency))
						{
							document_LinesImpl.Currency = GlobalData.MainCurrency;
						}
						num++;
						doc.Lines.Add(document_LinesImpl);
					}
				}
				doc.DocDueDate = Function.ParseDateTimes(txtOrderDelDate.EditValue);
				if (!(await doc.GetApprovalTemplate(new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales"), "3", this)))
				{
					WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
					return;
				}
				BPImpls bPImpls = new BPImpls();
				if (string.IsNullOrEmpty(_cardCode))
				{
					bPImpls.OldCardCode = "";
					bPImpls.CardCode = _cardCode;
					bPImpls.CardName = txtOrderName.Text;
					bPImpls.Phone1 = txtOrderPhoneNumber.Text;
					bPImpls.E_Mail = lbInfoFacebookEmail.Text;
					bPImpls.Balance = 0.0;
					bPImpls.DNotesBal = 0.0;
					bPImpls.OrdersBal = 0.0;
					bPImpls.Series = 1;
					bPImpls.CardType = "C";
					bPImpls.validFor = "N";
					bPImpls.frozenFor = "N";
					bPImpls.CreateDate = DateTime.Now.Date;
					bPImpls.UserSign = GlobalData.UserID;
					bPImpls.UserSign2 = GlobalData.UserID;
					bPImpls.ObjType = "2";
					bPImpls.Deleted = "";
					bPImpls.DocEntry = 0;
					bPImpls.ListUserFields = new List<UserFieldsImpl>
					{
						new UserFieldsImpl
						{
							Name = "U_FacebookID",
							Value = userId,
							Type = BoFieldTypes.db_Alpha
						}
					};
					List<AddressesImpl> addresses = new List<AddressesImpl>
					{
						new AddressesImpl
						{
							AddrType = "S",
							Address = "Ship To",
							Street = txtOrderAddress.Text,
							Country = Function.ToString(txtOrderCity.EditValue),
							State = Function.ToString(txtOrderCity.EditValue)
						}
					};
					bPImpls.Addresses = addresses;
					bPImpls.ShipToDef = "Ship To";
				}
				string text = await HttpClient.FacebookManagerClient.SaveFacebookOrder(doc, bPImpls);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				try
				{
					FacebookClient facebookClient = new FacebookClient(_fPageAccessToken);
					string text2 = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_ConvrId"));
					if (!string.IsNullOrEmpty(text2))
					{
						string path = text2 + "/messages?message=";
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						string messageText = "Đơn hàng đã được tạo:";
						for (int num3 = 0; num3 < dtItem.Rows.Count; num3++)
						{
							if (!string.IsNullOrEmpty(Function.ToString(dtItem.Rows[num3]["ItemCode"])))
							{
								messageText += string.Format("\r\nMặt hàng: {0}\r\nSố lượng: {1}\r\nĐơn giá: {2} {3}", Function.ToString(dtItem.Rows[num3][gcOrderItemName.FieldName]), Function.ParseDouble(dtItem.Rows[num3][gcOrderQuantity.FieldName]), Function.ToString(dtItem.Rows[num3][gcOrderPriceBefDi.FieldName]), Function.ToString(dtItem.Rows[num3]["Currency"]));
								decimal num4 = Function.ParseDecimal(dtItem.Rows[num3][gcOrderDiscPrcnt.FieldName]);
								if (num4 > 0m)
								{
									messageText += $"\r\nGiảm giá: {num4}%";
								}
							}
						}
						messageText = (string)(dictionary["message"] = messageText + $"\r\nPhí giao hàng: {txtOrderFreight.Text}\r\nTổng tiền thanh toán: {txtOrderTotal.Text}\r\nNgày giao hàng: {txtOrderDelDate.Text}\r\nGhi chú đơn hàng: {txtOrderNote.Text}\r\nTên khách hàng: {txtOrderName.Text}\r\nSDT: {txtOrderPhoneNumber.Text}\r\nĐịa chỉ giao hàng: {txtOrderAddress.Text}-{txtOrderCity.Text}-{txtOrderCountry.Text}");
						await facebookClient.PostTaskAsync(path, dictionary);
						radChatControl.ChatElement.AddMessage(new ChatTextMessage(messageText, CurrentAuthor, DateTime.Now));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
				grdOrderItem.DataSourceTable.Rows.Clear();
				AddNewRowItem();
				txtOrderFreight.EditValue = null;
				txtOrderTotal.EditValue = null;
				grdOrderItem.DataSourceTable.AcceptChanges();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void gvOrderItem_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DocumentLineTable documentLineTable = (DocumentLineTable)gvOrderItem.GridControl.DataSource;
			int num = Function.ParseInt(gvOrderItem.GetRowCellValue(e.RowHandle, "LineNum"));
			DocumentLineRow byNum = documentLineTable.GetByNum(num);
			if (e.Column.FieldName.Equals("ItemCode"))
			{
				string text = Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_UserId"));
				DataTable dataTable = HttpClient.Commons.ItemMaster(Function.ToString(e.Value), GlobalData.DefaultBranch, Enums.ItemObjectType.SellItem, text, Function.ParseDateTimes(txtOrderDelDate.EditValue));
				if (!dataTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.None, "Mặt hàng không tồn tại");
				}
				int rowHandle = e.RowHandle;
				DocumentLineTable documentLineTable2 = (DocumentLineTable)grdOrderItem.DataSource;
				WinFuntions.DeleteBomChildsItem(gvOrderItem, rowHandle, null, isChooseItem: true);
				grdOrderItem.DataSource = documentLineTable2;
				grdOrderItem.RefreshDataSource();
				DocumentLineRow line = documentLineTable2.GetNewRow();
				int num2 = num;
				IEnumerable<DocumentLineRow> source = from t in documentLineTable2.GetEnumerable()
					where Function.ToString(t["LineType"]) == "R"
					select t;
				if (source.Any())
				{
					int? num3 = source.Max((Func<DocumentLineRow, int?>)((DocumentLineRow x) => x.LineNum));
					num2 = ((num >= 0) ? num : (num3.HasValue ? (Function.ParseInt(num3) + 1) : 0));
				}
				line.LineNum = ((num2 >= 0) ? num2 : 0);
				documentLineTable2.Insert(rowHandle, line);
				documentLineTable2.AcceptChanges();
				line.DocEntry = -1;
				bool fromExcel = line.FromExcel;
				DataRow dataRow = dataTable.Rows[0];
				if (string.IsNullOrEmpty(line.ItemCode) && !fromExcel)
				{
					line.ItemCode = Function.ToString(dataRow["ItemCode"]);
				}
				if (string.IsNullOrEmpty(line.ItemDescription))
				{
					line.ItemDescription = Function.ToString(dataRow["ItemName"]);
				}
				if (Function.ParseDouble(line.Quantity).Equals(0.0) && !fromExcel)
				{
					line.Quantity = 1.0;
				}
				string text2 = "N";
				if (dataRow.IsExistsColumn("IsGrossPrc"))
				{
					text2 = Function.ToString(dataRow["IsGrossPrc"]);
				}
				line.RemainingOpenQuantity = line.Quantity;
				if (string.IsNullOrEmpty(Function.ToString(line.WarehouseCode)))
				{
					line.WarehouseCode = Function.ToString(dataRow["DefaultWarehouse"]);
				}
				line.ShipDate = Function.ParseDateTimes(txtOrderDelDate.EditValue);
				line.VatGroup = Function.ToString(dataRow["SalesVATGroup"]) ?? "";
				line.Rate = Function.ParseDouble(dataRow["RateSa"]);
				line.TaxPercentagePerRow = Function.ParseDouble(dataRow["RateSa"]);
				line.LineType = "R";
				line.LineVat = 0.0;
				line.LineStatusName = "O";
				line.UseBaseUnits = "N";
				line["SalPackUn"] = dataRow["SalPackUn"];
				line["PurPackUn"] = dataRow["PurPackUn"];
				line.UoMName = Function.ToString(dataRow["SalUnitMsr"]);
				line.UnitsOfMeasurment = Function.ParseDouble(dataRow["NumInSale"]);
				line["OpenInvQty"] = line.UnitsOfMeasurment * line.Quantity;
				line["InvQty"] = line.UnitsOfMeasurment * line.Quantity;
				double num4 = Function.ParseDouble(line["SalPackUn"]);
				if (Math.Abs(num4) < 1E-07)
				{
					num4 = 1.0;
				}
				double num5 = line.Quantity / num4;
				double num6 = Math.Truncate(num5);
				line["PackQty"] = ((num5 - num6 > 1E-07) ? (num6 + 1.0) : num6);
				line.InventoryQuantity = line.Quantity * line.UnitsOfMeasurment;
				line["UgpCode"] = Function.ParseInt(dataRow["UgpCode"]);
				if (Function.ParseInt(dataRow["UgpCode"]) == -1)
				{
					line.UoMCode = "Manual";
				}
				else
				{
					line.UoMCode = WinFuntions.GetUoMCodeByName(Function.ParseInt(line["UgpCode"]), line.UoMName);
				}
				double num7 = 0.0;
				string text3 = "";
				double discountPercent = 0.0;
				string value = "R";
				DataTable dataTable2 = HttpClient.Commons.DocumentPrice(line.ItemCode, text, Function.ParseDateTimes(txtOrderDelDate.EditValue), line.Quantity, line.UoMCode, Function.ParseInt(txtOrderPriceList.EditValue));
				if (dataTable2.IsNotNull())
				{
					num7 = Function.ParseDouble(dataTable2.Rows[0]["Price"]);
					text3 = Function.ToString(dataTable2.Rows[0]["Currency"]);
					discountPercent = Function.ParseDouble(dataTable2.Rows[0]["Discount"]);
					value = Function.ToString(dataTable2.Rows[0]["SpecPrice"]);
					if (Function.ParseInt(line["UgpCode"]) == -1)
					{
						num7 *= line.UnitsOfMeasurment;
					}
				}
				line["SpecPrice"] = value;
				line.DiscountPercent = discountPercent;
				if (text2 == "Y")
				{
					line.PriceBefDi = num7 / (1.0 + line.VatPrcnt / 100.0);
				}
				else
				{
					line.PriceBefDi = num7;
				}
				line.Price = line.PriceBefDi - line.PriceBefDi * line.DiscountPercent / 100.0;
				if (!string.IsNullOrEmpty(text3))
				{
					line.Currency = text3;
					line.RateCurrency = ExtenseionMethods.GetExchangeRate(text3, Function.ParseDateTimes(txtOrderDelDate.EditValue));
				}
				else
				{
					line.Currency = GlobalData.MainCurrency;
					line.RateCurrency = 1.0;
				}
				if (Function.ToString(dataRow["BOMTreeType"]) == "S" || Function.ToString(dataRow["BOMTreeType"]) == "T")
				{
					line.BOMTreeType = Function.ToString(dataRow["BOMTreeType"]);
					line.TreeType = Function.ToString(dataRow["BOMTreeType"]);
					DataTable dataTable3 = HttpClient.BillOfMasterials.SearchADoc(Function.ToString(dataRow["ItemCode"]));
					if (dataTable3.IsNotNull())
					{
						string text4 = Function.ToString(dataTable3.Rows[0]["OcrCode"]);
						if (!string.IsNullOrEmpty(text4))
						{
							line.OcrCode = text4;
						}
						string text5 = Function.ToString(dataTable3.Rows[0]["OcrCode2"]);
						if (!string.IsNullOrEmpty(text5))
						{
							line.OcrCode2 = text5;
						}
						string text6 = Function.ToString(dataTable3.Rows[0]["OcrCode3"]);
						if (!string.IsNullOrEmpty(text6))
						{
							line.OcrCode3 = text6;
						}
						string text7 = Function.ToString(dataTable3.Rows[0]["OcrCode4"]);
						if (!string.IsNullOrEmpty(text7))
						{
							line.OcrCode4 = text7;
						}
						string text8 = Function.ToString(dataTable3.Rows[0]["OcrCode5"]);
						if (!string.IsNullOrEmpty(text8))
						{
							line.OcrCode5 = text8;
						}
						string text9 = Function.ToString(dataTable3.Rows[0]["Project"]);
						if (!string.IsNullOrEmpty(text9))
						{
							line.Project = text9;
						}
					}
					List<Common.Table_ID_Code> list = new List<Common.Table_ID_Code>();
					DataTable childItemsInBom = WinFuntions.GetChildItemsInBom(Function.ToString(dataRow["ItemCode"]), list);
					int num8 = rowHandle;
					num8++;
					if (childItemsInBom != null && childItemsInBom.Rows.Count > 0)
					{
						foreach (DataRow bomitem in childItemsInBom.Rows)
						{
							DocumentLineRow newRow = documentLineTable2.GetNewRow();
							newRow.LineType = "R";
							int? num9 = ((IEnumerable<DocumentLineRow>)documentLineTable2.GetEnumerable()).Max((Func<DocumentLineRow, int?>)((DocumentLineRow x) => x.LineNum));
							int num10 = ((!num9.HasValue) ? 1 : (Function.ParseInt(num9) + 1));
							if (!list.Exists((Common.Table_ID_Code x) => x.Name == line.LineNum.ToString() && x.Code == Function.ToString(bomitem["ItemCode"])))
							{
								Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
								table_ID_Code.ID = Function.ParseDouble(bomitem["OnHand"]);
								table_ID_Code.Code = Function.ToString(bomitem["ItemCode"]);
								table_ID_Code.Name = line.LineNum.ToString();
								list.Add(table_ID_Code);
							}
							if (Function.ToString(dataRow["BOMTreeType"]) == "S")
							{
								newRow.BOMTreeType = line.LineNum.ToString();
								newRow.TreeType = "I";
							}
							else
							{
								newRow.BOMTreeType = "-1";
								newRow.TreeType = "N";
							}
							if (string.IsNullOrEmpty(Function.ToString(bomitem["Currency"])))
							{
								newRow.Currency = GlobalData.MainCurrency;
								newRow.RateCurrency = 1.0;
							}
							else
							{
								newRow.Currency = Function.ToString(bomitem["Currency"]);
								newRow.RateCurrency = ExtenseionMethods.GetExchangeRate(newRow.Currency, Function.ParseDateTimes(txtOrderDelDate.EditValue));
							}
							newRow["SalUnitMsr"] = Function.ToString(bomitem["SalUnitMsr"]);
							newRow["NumInSale"] = Function.ParseDouble(bomitem["NumInSale"]);
							newRow["BuyUnitMsr"] = Function.ToString(dataRow["BuyUnitMsr"]);
							newRow["NumInBuy"] = Function.ParseDouble(dataRow["NumInBuy"]);
							newRow.UnitsOfMeasurment = 1.0;
							newRow["UgpCode"] = Function.ParseInt(bomitem["UgpCode"]);
							newRow.UseBaseUnits = "Y";
							newRow.UoMName = Function.ToString(bomitem["InvntryUom"]);
							newRow["InvUnitMsr"] = Function.ToString(bomitem["InvntryUom"]);
							newRow["NumInInven"] = Function.ParseDouble(bomitem["NumInInven"]);
							newRow["OpenInvQty"] = 1;
							newRow["InvQty"] = 1;
							newRow["SlpCode"] = -1;
							newRow["OcrCode"] = bomitem["OcrCode"];
							newRow["OcrCode2"] = bomitem["OcrCode2"];
							newRow["OcrCode3"] = bomitem["OcrCode3"];
							newRow["OcrCode4"] = bomitem["OcrCode4"];
							newRow["OcrCode5"] = bomitem["OcrCode5"];
							newRow["FreeTxt"] = bomitem["Comment"];
							if (newRow.IsExistsColumn("ManBatchSerial"))
							{
								newRow["ManBatchSerial"] = "N";
							}
							if (Function.ParseInt(newRow["UgpCode"]) <= 0)
							{
								newRow.UoMCode = newRow.UoMName;
							}
							else
							{
								newRow.UoMCode = WinFuntions.GetUoMCodeByName(Function.ParseInt(newRow["UgpCode"]), newRow.UoMName);
							}
							newRow.Quantity = Function.ParseDouble(bomitem["OnHand"]);
							documentLineTable2.Rows.InsertAt(newRow, num8);
							documentLineTable2.AcceptChanges();
							grdOrderItem.DataSource = documentLineTable2;
							grdOrderItem.RefreshDataSource();
							gvOrderItem.SetRowCellValue(num8, "LineNum", num10);
							gvOrderItem.SetRowCellValue(num8, "DocEntry", -1);
							gvOrderItem.SetRowCellValue(num8, "ItemCode", Function.ToString(bomitem["ItemCode"]));
							gvOrderItem.SetRowCellValue(num8, "Dscription", Function.ToString(bomitem["ItemName"]));
							gvOrderItem.SetRowCellValue(num8, "ShipDate", txtOrderDelDate.EditValue);
							gvOrderItem.SetRowCellValue(num8, "UnitPrice", Function.ParseDouble(bomitem["Price"]));
							gvOrderItem.SetRowCellValue(num8, "PriceBefDi", Function.ParseDouble(bomitem["Price"]));
							gvOrderItem.SetRowCellValue(num8, "VatGroup", Function.ToString(bomitem["SalesVATGroup"]));
							gvOrderItem.SetRowCellValue(num8, "TaxCode", Function.ToString(bomitem["SalesVATGroup"]));
							gvOrderItem.SetRowCellValue(num8, "VatPrcnt", Function.ParseDouble(bomitem["RateSa"]));
							gvOrderItem.SetRowCellValue(num8, "OpenQty", 1);
							gvOrderItem.SetRowCellValue(num8, "LineStatus", "O");
							gvOrderItem.SetRowCellValue(num8, "Project", Function.ToString(bomitem["Project"]));
							gvOrderItem.SetRowCellValue(num8, "WhsCode", Function.ToString(bomitem["Warehouse"]));
							WinFuntions.SetUDFFieldValueBOMItemToDoc(line.ItemCode, Function.ParseInt(bomitem["ChildNum"]), gvOrderItem, num8);
							gvOrderItem.FocusedRowHandle = num8;
							num8++;
						}
					}
				}
				double num11 = ApzonCommon.LineTotal(line.Quantity, line.PriceBefDi, line.DiscountPercent);
				if (line.RateCurrency == 0.0)
				{
					line.LineTotal = num11 * 1.0;
				}
				else
				{
					line.LineTotal = num11 * Function.ParseDouble(line.RateCurrency);
				}
				double num12 = Function.ParseDouble(gvOrderItem.GetRowCellValue(e.RowHandle, "VatPrcnt"));
				double lineVat = line.LineTotal * num12;
				if (num12 >= 1.0)
				{
					lineVat = line.LineTotal * num12 / 100.0;
				}
				line.LineVat = lineVat;
				line["SalUnitMsr"] = Function.ToString(dataRow["SalUnitMsr"]);
				line["NumInSale"] = Function.ParseDouble(dataRow["NumInSale"]);
				line["BuyUnitMsr"] = Function.ToString(dataRow["BuyUnitMsr"]);
				line["NumInBuy"] = Function.ParseDouble(dataRow["NumInBuy"]);
				line["InvUnitMsr"] = Function.ToString(dataRow["InvntryUom"]);
				line["NumInInven"] = Function.ParseDouble(dataRow["NumInInven"]);
				if (line.IsExistsColumn("ManBatchSerial") && dataRow.IsExistsColumn("ManBatchSerial"))
				{
					line["ManBatchSerial"] = Function.ToString(dataRow["ManBatchSerial"]);
				}
				if (string.IsNullOrEmpty(Function.ToString(line["SlpCode"])) || Function.ToString(line["SlpCode"]).Equals("-1"))
				{
					line["SlpCode"] = -1;
				}
				line["SubCatNum"] = Function.ToString(dataRow["SubCatNum"]);
				line["QuantityOnStock"] = dataRow["OnStock"];
				line["QuantityOnWareHouse"] = dataRow["OnWhs"];
				line["IsCommited"] = dataRow["IsCommited"];
				line["IsCommitedInWareHouse"] = dataRow["IsCommitedInWhs"];
				line["OnOrder"] = dataRow["OnOrder"];
				line["OnOrderInWareHouse"] = dataRow["OnOrderInWhs"];
				line["LineVendor"] = dataRow["CardCode"];
				grdOrderItem.DataSource = documentLineTable2;
				grdOrderItem.RefreshDataSource();
				gvOrderItem.FocusedRowHandle = rowHandle;
				grdOrderItem.RefreshDataSource();
				gvOrderItem.ShowEditor();
				gvOrderItem.GridControl.DataSourceTable().AcceptChanges();
				AddNewRowItem();
			}
			else if (e.Column.FieldName.ToLower().Equals("quantity"))
			{
				byNum.InventoryQuantity = Function.ParseDouble(e.Value) * Function.ParseDouble(gvOrderItem.GetFocusedRowCellValue("NumPerMsr"));
				byNum.OpenInventoryQuantity = Function.ParseDouble(e.Value) * Function.ParseDouble(gvOrderItem.GetFocusedRowCellValue("NumPerMsr"));
				double num13 = Function.ParseDouble(byNum["SalPackUn"]);
				if (Math.Abs(num13) < 1E-07)
				{
					num13 = 1.0;
				}
				double num14 = Function.ParseDouble(e.Value) / num13;
				double num15 = Math.Truncate(num14);
				byNum["PackQty"] = ((num14 - num15 > 1E-07) ? (num15 + 1.0) : num15);
			}
			if (new string[4] { "Quantity", gcOrderPriceBefDi.FieldName, gcOrderDiscPrcnt.FieldName, "VatPrcnt" }.Contains(e.Column.FieldName))
			{
				double num16 = ApzonCommon.LineTotal(byNum.Quantity, byNum.PriceBefDi, byNum.DiscountPercent);
				byNum.RemainingOpenQuantity = byNum.Quantity;
				byNum.Price = byNum.PriceBefDi - byNum.PriceBefDi * byNum.DiscountPercent / 100.0;
				double num17 = ((Function.ParseDouble(gvOrderItem.GetRowCellValue(e.RowHandle, "Rate")) == 0.0) ? 1.0 : Function.ParseDouble(gvOrderItem.GetRowCellValue(e.RowHandle, "Rate")));
				int num18 = GlobalData.UnitPriceFormat.NumberDecimalDigits;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency));
					if (currencyCodesImpl != null)
					{
						num18 = ((currencyCodesImpl.Decimals == -1) ? num18 : currencyCodesImpl.Decimals);
					}
				}
				if (num17 == 0.0)
				{
					byNum.LineTotal = Function.ParseDouble(Math.Round(Function.ParseDecimal(num16), num18));
				}
				else
				{
					byNum.LineTotal = Function.ParseDouble(Math.Round(Function.ParseDecimal(num16 * Math.Pow(num17, GlobalData.ExchangeRatePow)), num18));
				}
			}
			SetDocTotal();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void SetDocTotal()
	{
		DocumentLineTable documentLineTable = (DocumentLineTable)grdOrderItem.DataSource;
		_ = GlobalData.MainCurrency;
		double num = 0.0;
		double num2 = 0.0;
		double freight = 0.0;
		double totalTax = 0.0;
		num = documentLineTable.GetEnumerable().Sum((DocumentLineRow x) => Function.ParseDouble(x.LineTotal) * (1.0 - x.DiscountPercent / 100.0));
		try
		{
			freight = documentLineTable.GetEnumerable().Sum((DocumentLineRow x) => Function.ParseDouble(x["LineTotal_F1"]) + Function.ParseDouble(x["LineTotal_F2"]) + Function.ParseDouble(x["LineTotal_F3"]));
			totalTax = documentLineTable.GetEnumerable().Sum((DocumentLineRow x) => x.LineVat + Function.ParseDouble(x["VatSum_F1"]) + Function.ParseDouble(x["VatSum_F2"]) + Function.ParseDouble(x["VatSum_F3"]));
		}
		catch (Exception)
		{
		}
		num2 = ApzonCommon.DocTotal(num, totalTax, 0.0, freight);
		txtOrderTotal.EditValue = Function.ParseDouble(num2) + Function.ParseDouble(txtOrderFreight.EditValue);
	}

	private void txtOrderCountry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txtOrderCity.EditValue = "";
			DataTable dataState = HttpClient.Commons.GetDataState(Function.ToString(txtOrderCountry.EditValue));
			if (dataState != null && dataState.Rows.Count > 0)
			{
				DataRow dataRow = dataState.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataState.Rows.InsertAt(dataRow, 0);
				dataState.AcceptChanges();
			}
			txtOrderCity.Properties.DataSource = dataState;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void AddNewRowItem()
	{
		DataTable dataTable = grdOrderItem.DataSourceTable;
		if (dataTable == null || dataTable.Columns.Count == 0)
		{
			dataTable = DocumentsHelper.Lines.GetClone();
		}
		if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = -1;
			object value = (dataRow["Dscription"] = string.Empty);
			dataRow["ItemCode"] = value;
			dataRow["LineNum"] = GetNewLineNum();
			dataRow["Quantity"] = 1;
			dataRow["Price"] = 0;
			dataRow["PQTReqdate"] = DateTime.Today;
			dataRow["ShipDate"] = DateTime.Today;
			dataRow["Currency"] = GlobalData.MainCurrency;
			dataRow["WhsCode"] = string.Empty;
			dataRow["BaseType"] = -1;
			dataRow["BaseEntry"] = 0;
			dataRow["LineStatus"] = "O";
			dataRow["AcctCode"] = "";
			dataRow["ManBtchSerial"] = "N";
			dataTable.Rows.Add(dataRow);
			grdOrderItem.DataSource = dataTable;
			grdOrderItem.DataSourceTable.AcceptChanges();
			grdOrderItem.RefreshDataSource();
		}
	}

	private int GetNewLineNum()
	{
		if (grdOrderItem.DataSourceTable.IsNotNull())
		{
			DataTable dataSourceTable = grdOrderItem.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return 0;
			}
			return Function.ParseInt(dataSourceTable.Compute("MAX(LineNum)", "")) + 1;
		}
		return 0;
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvOrderItem.DeleteDataSourceRowSelected();
			AddNewRowItem();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rOrderItemName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, "", "O", Enums.ItemObjectType.InventoryItem, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvOrderItem.SetFocusedRowCellValue("ItemCode", frm_CFL2.ChooseItem["ItemCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataPriceList()
	{
		try
		{
			DataTable dataSource = await HttpClient.Commons.GetPriceList();
			txtOrderPriceList.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_FacebookManager_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (e.FieldName.Equals("TotalExpns"))
			{
				SetDocTotal();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void timer1_Tick(object sender, EventArgs e)
	{
		timer1.Stop();
		try
		{
			await UpdateDataConversationPage(_pageId, _fPageAccessToken);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		timer1.Start();
	}

	private async Task UpdateDataConversationPage(string pageId, string accessPageToken)
	{
		DateTime updateTime = _lastUpdateTime;
		try
		{
			if (_dtPageUser == null || _dtPageUser.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				return;
			}
			DataTable dtConverInbox = grdMessage.DataSourceTable;
			if (dtConverInbox == null || dtConverInbox.Columns.Count == 0)
			{
				dtConverInbox = new DataTable
				{
					Columns = 
					{
						{
							"F_ConvrId",
							typeof(string)
						},
						{
							"F_UserId",
							typeof(string)
						},
						{
							"Avatar",
							typeof(Image)
						},
						{
							"F_UserName",
							typeof(string)
						},
						{
							"UnreadCount",
							typeof(int)
						}
					}
				};
			}
			FacebookClient fbPage = new FacebookClient(accessPageToken);
			dynamic lstUserConversation = await fbPage.GetTaskAsync($"/{pageId}?fields=conversations.limit(20){{participants, unread_count, updated_time}}");
			dynamic val = lstUserConversation.conversations.data;
			bool isUpdateData = false;
			bool isGetNextMessage = true;
			bool isUpdateTime = true;
			foreach (dynamic converData in val.ToArray())
			{
				dynamic val2 = DateTime.Parse(converData.updated_time).ToUniversalTime();
				if (val2 <= _lastUpdateTime)
				{
					break;
				}
				if (isUpdateTime)
				{
					updateTime = val2;
				}
				foreach (dynamic userData in converData.participants.data.ToArray())
				{
					dynamic userId = Function.ToString(userData.id);
					if (userId.Equals(pageId))
					{
						isGetNextMessage = false;
						continue;
					}
					DataRow pageUser = _dtPageUser.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_UserId"]).Equals(userId));
					if (pageUser == null)
					{
						pageUser = _dtPageUser.NewRow();
						pageUser["F_PageId"] = pageId;
						pageUser["F_ConvrId"] = (object)converData.id;
						pageUser["F_UserId"] = (object)userData.id;
						pageUser["Avatar"] = _defaultAvatar;
						pageUser["F_UserName"] = (object)userData.name;
						_dtPageUser.Rows.InsertAt(pageUser, 0);
						isUpdateData = true;
					}
					string img = Function.ToString(pageUser["Avatar"]);
					if (string.IsNullOrEmpty(Function.ToString(pageUser["LinkAvatar"])))
					{
						try
						{
							dynamic val3 = (await fbPage.GetTaskAsync(string.Format("/{0}?fields=profile_pic.width(100).height(120)", userId))).profile_pic;
							if ((!string.IsNullOrEmpty(val3)))
							{
								dynamic val4 = WebRequest.Create(val3);
								dynamic response = val4.GetResponse();
								using ((IDisposable)response)
								{
									dynamic responseStream = response.GetResponseStream();
									using ((IDisposable)responseStream)
									{
										dynamic val5 = Image.FromStream(responseStream);
										using ((IDisposable)val5)
										{
											using MemoryStream memoryStream = new MemoryStream();
											val5.Save(memoryStream, val5.RawFormat);
											byte[] inArray = memoryStream.ToArray();
											img = (string)(pageUser["Avatar"] = Convert.ToBase64String(inArray));
											pageUser["LinkAvatar"] = (object)val3;
											isUpdateData = true;
										}
									}
								}
							}
						}
						catch (Exception)
						{
						}
					}
					DataRow dataRow = dtConverInbox.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_ConvrId"]).Equals(Function.ToString(converData.id)));
					if (dataRow == null)
					{
						dataRow = dtConverInbox.NewRow();
						dataRow["F_ConvrId"] = (object)converData.id;
						dataRow["F_UserId"] = (object)userData.id;
						dataRow["F_UserName"] = (object)userData.name;
						dataRow["Avatar"] = LoadImageFromBase64(img);
						dtConverInbox.Rows.Add(dataRow);
					}
					if (Function.ToString(converData.id).Equals(Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_ConvrId"))))
					{
						UpdateDataMessageInConversation(Function.ToString(converData.id), _fPageAccessToken);
						continue;
					}
					dynamic val6 = Function.ParseInt(converData.unread_count);
					if (val6 > 0)
					{
						dataRow["UnreadCount"] = (object)val6;
					}
				}
			}
			if (isGetNextMessage && lstUserConversation.conversations.paging != null && lstUserConversation.conversations.paging.next != null)
			{
				isUpdateData = await UpdateDataNextConversationPage(pageId, accessPageToken, Function.ToString(lstUserConversation.conversations.paging.next), dtConverInbox);
			}
			_dtPageUser.AcceptChanges();
			dtConverInbox.AcceptChanges();
			if (isUpdateData)
			{
				HttpClient.FacebookManagerClient.SaveDataPageUser(pageId, _dtPageUser);
			}
			grdMessage.DataSource = dtConverInbox;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.None, ex2.Message);
		}
		_lastUpdateTime = updateTime;
	}

	private async Task<bool> UpdateDataNextConversationPage(string pageId, string accessPageToken, string nextConversationUrl, DataTable dtConverInbox)
	{
		bool isUpdateData = false;
		try
		{
			FacebookClient fbPage = new FacebookClient(accessPageToken);
			_lastUpdateTime = DateTime.Now;
			dynamic lstUserConversation = await fbPage.GetTaskAsync(nextConversationUrl);
			dynamic val = lstUserConversation.conversations.data;
			bool isGetNextMessage = true;
			foreach (dynamic converData in val.ToArray())
			{
				dynamic val2 = DateTime.Parse(converData.updated_time).ToUniversalTime();
				if (val2 <= _lastUpdateTime)
				{
					break;
				}
				foreach (dynamic userData in converData.participants.data.ToArray())
				{
					dynamic userId = Function.ToString(userData.id);
					if (userId.Equals(pageId))
					{
						isGetNextMessage = false;
						continue;
					}
					DataRow pageUser = _dtPageUser.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_UserId"]).Equals(userId));
					if (pageUser == null)
					{
						pageUser = _dtPageUser.NewRow();
						pageUser["F_PageId"] = pageId;
						pageUser["F_ConvrId"] = (object)converData.id;
						pageUser["F_UserId"] = (object)userData.id;
						pageUser["Avatar"] = _defaultAvatar;
						pageUser["F_UserName"] = (object)userData.name;
						_dtPageUser.Rows.InsertAt(pageUser, 0);
						isUpdateData = true;
					}
					string img = Function.ToString(pageUser["Avatar"]);
					if (string.IsNullOrEmpty(Function.ToString(pageUser["LinkAvatar"])))
					{
						try
						{
							dynamic val3 = (await fbPage.GetTaskAsync(string.Format("/{0}?fields=profile_pic.width(100).height(120)", userId))).profile_pic;
							if ((!string.IsNullOrEmpty(val3)))
							{
								dynamic val4 = WebRequest.Create(val3);
								dynamic response = val4.GetResponse();
								using ((IDisposable)response)
								{
									dynamic responseStream = response.GetResponseStream();
									using ((IDisposable)responseStream)
									{
										dynamic val5 = Image.FromStream(responseStream);
										using ((IDisposable)val5)
										{
											using MemoryStream memoryStream = new MemoryStream();
											val5.Save(memoryStream, val5.RawFormat);
											byte[] inArray = memoryStream.ToArray();
											img = (string)(pageUser["Avatar"] = Convert.ToBase64String(inArray));
											pageUser["LinkAvatar"] = (object)val3;
											isUpdateData = true;
										}
									}
								}
							}
						}
						catch (Exception)
						{
						}
					}
					DataRow dataRow = dtConverInbox.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["F_ConvrId"]).Equals(Function.ToString(converData.id)));
					if (dataRow == null)
					{
						dataRow = dtConverInbox.NewRow();
						dataRow["F_ConvrId"] = (object)converData.id;
						dataRow["F_UserId"] = (object)userData.id;
						dataRow["F_UserName"] = (object)userData.name;
						dataRow["Avatar"] = LoadImageFromBase64(img);
						dtConverInbox.Rows.Add(dataRow);
					}
					if (Function.ToString(converData.id).Equals(Function.ToString(tileViewMessage.GetFocusedRowCellValue("F_ConvrId"))))
					{
						UpdateDataMessageInConversation(Function.ToString(converData.id), _fPageAccessToken);
						continue;
					}
					dynamic val6 = Function.ParseInt(converData.unread_count);
					if (val6 > 0)
					{
						dataRow["UnreadCount"] = (object)val6;
					}
				}
			}
			if (isGetNextMessage && lstUserConversation.conversations.paging != null && lstUserConversation.conversations.paging.next != null)
			{
				dynamic val7 = await UpdateDataNextConversationPage(pageId, accessPageToken, Function.ToString(lstUserConversation.conversations.paging.next), dtConverInbox);
				if (val7)
				{
					isUpdateData = val7;
				}
			}
			_dtPageUser.AcceptChanges();
			dtConverInbox.AcceptChanges();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.None, ex2.Message);
		}
		return isUpdateData;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.FacebookCRM.frm_FacebookManager));
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.tvcMessageAvatar = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rMessageAvatar = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tvcMessageName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tvcMessageUnread = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.txtFacebookPage = new DevExpress.XtraEditors.LookUpEdit();
		this.btnCheck = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtFacebookUserName = new Apzon.Controls.Editor.TextEditEx();
		this.btnLogin = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnActive = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnAccess = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitUser = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabUser = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageAll = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdMessage = new Apzon.Controls.Grid.GridControlEx();
		this.tileViewMessage = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.xtraTabPageComment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitMesage = new DevExpress.XtraEditors.SplitContainerControl();
		this.radChatControl = new Telerik.WinControls.UI.RadChat();
		this.xtraTabOrder = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageCustomer = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.lbInfoBirthday = new Apzon.Controls.Controls.LabelLine();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.lbInfoFacebookLocation = new Apzon.Controls.Controls.LabelLine();
		this.lbInfoFacebookEmail = new Apzon.Controls.Controls.LabelLine();
		this.lbInfoFacebookPhoneNumber = new Apzon.Controls.Controls.LabelLine();
		this.lbInfoFacebookName = new Apzon.Controls.Controls.LabelLine();
		this.pictureInfoUser = new DevExpress.XtraEditors.PictureEdit();
		this.xtraTabPageOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtOrderPriceList = new Apzon.Controls.Editor.LookUpEditEx();
		this.btnOrderCreateOrder = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtOrderNote = new Apzon.Controls.Editor.TextEditEx();
		this.txtOrderDelDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtOrderTotal = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.txtOrderFreight = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.lbOrderTotal = new Apzon.Controls.Controls.LabelLine();
		this.lbOrderLogisticAmount = new Apzon.Controls.Controls.LabelLine();
		this.txtOrderCity = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtOrderCountry = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtOrderAddress = new Apzon.Controls.Editor.TextEditEx();
		this.txtOrderPhoneNumber = new Apzon.Controls.Editor.TextEditEx();
		this.txtOrderName = new Apzon.Controls.Editor.TextEditEx();
		this.grdOrderItem = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripOrderItem = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvOrderItem = new Apzon.Controls.Grid.GridViewEx();
		this.gcOrderItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rOrderItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcOrderQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rCalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcOrderPriceBefDi = new Apzon.Controls.Grid.GridColumnEx();
		this.rPrice = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcOrderDiscPrcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOrderItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOrderLineTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbFacebookUser = new Apzon.Controls.Controls.LabelLine();
		this.lbFacebookPage = new Apzon.Controls.Controls.LabelLine();
		this.radThemeManager1 = new Telerik.WinControls.RadThemeManager();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.rMessageAvatar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFacebookPage.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFacebookUserName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitUser).BeginInit();
		this.splitUser.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabUser).BeginInit();
		this.xtraTabUser.SuspendLayout();
		this.xtraTabPageAll.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdMessage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewMessage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitMesage).BeginInit();
		this.splitMesage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.radChatControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabOrder).BeginInit();
		this.xtraTabOrder.SuspendLayout();
		this.xtraTabPageCustomer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureInfoUser.Properties).BeginInit();
		this.xtraTabPageOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtOrderPriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderNote.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderDelDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderDelDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderFreight.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderCity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderCountry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderPhoneNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdOrderItem).BeginInit();
		this.contextMenuStripOrderItem.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvOrderItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOrderItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).BeginInit();
		base.SuspendLayout();
		this.tvcMessageAvatar.Caption = "Avatar";
		this.tvcMessageAvatar.ColumnEdit = this.rMessageAvatar;
		this.tvcMessageAvatar.FieldName = "Avatar";
		this.tvcMessageAvatar.Name = "tvcMessageAvatar";
		this.tvcMessageAvatar.Visible = true;
		this.tvcMessageAvatar.VisibleIndex = 0;
		this.rMessageAvatar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rMessageAvatar.Name = "rMessageAvatar";
		this.rMessageAvatar.ZoomAccelerationFactor = 1.0;
		this.tvcMessageName.Caption = "Name";
		this.tvcMessageName.FieldName = "F_UserName";
		this.tvcMessageName.Name = "tvcMessageName";
		this.tvcMessageName.Visible = true;
		this.tvcMessageName.VisibleIndex = 1;
		this.tvcMessageUnread.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 10f);
		this.tvcMessageUnread.AppearanceCell.ForeColor = System.Drawing.Color.Red;
		this.tvcMessageUnread.AppearanceCell.Options.UseFont = true;
		this.tvcMessageUnread.AppearanceCell.Options.UseForeColor = true;
		this.tvcMessageUnread.Caption = "Unread message";
		this.tvcMessageUnread.FieldName = "UnreadCount";
		this.tvcMessageUnread.Name = "tvcMessageUnread";
		this.tvcMessageUnread.Visible = true;
		this.tvcMessageUnread.VisibleIndex = 2;
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.txtFacebookPage.Location = new System.Drawing.Point(103, 36);
		this.txtFacebookPage.Name = "txtFacebookPage";
		this.txtFacebookPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFacebookPage.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("F_PageId", "Page Id", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PageName", "Page Name")
		});
		this.txtFacebookPage.Properties.DisplayMember = "PageName";
		this.txtFacebookPage.Properties.NullText = "";
		this.txtFacebookPage.Properties.ShowHeader = false;
		this.txtFacebookPage.Properties.ValueMember = "F_PageId";
		this.txtFacebookPage.Size = new System.Drawing.Size(200, 20);
		this.txtFacebookPage.TabIndex = 37;
		this.txtFacebookPage.EditValueChanged += new System.EventHandler(this.txtFacebookPage_EditValueChanged);
		this.btnCheck.FieldName = null;
		this.btnCheck.IsSearchControl = false;
		this.btnCheck.Location = new System.Drawing.Point(379, 8);
		this.btnCheck.Name = "btnCheck";
		this.btnCheck.Size = new System.Drawing.Size(61, 21);
		this.btnCheck.TabIndex = 44;
		this.btnCheck.Text = "Check";
		this.txtFacebookUserName.FieldName = null;
		this.txtFacebookUserName.IsSearchControl = false;
		this.txtFacebookUserName.Location = new System.Drawing.Point(103, 9);
		this.txtFacebookUserName.Name = "txtFacebookUserName";
		this.txtFacebookUserName.Properties.ReadOnly = true;
		this.txtFacebookUserName.Size = new System.Drawing.Size(200, 20);
		this.txtFacebookUserName.TabIndex = 39;
		this.btnLogin.FieldName = null;
		this.btnLogin.IsSearchControl = false;
		this.btnLogin.Location = new System.Drawing.Point(312, 8);
		this.btnLogin.Name = "btnLogin";
		this.btnLogin.Size = new System.Drawing.Size(61, 21);
		this.btnLogin.TabIndex = 38;
		this.btnLogin.Text = "Login";
		this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
		this.btnActive.Enabled = false;
		this.btnActive.FieldName = null;
		this.btnActive.IsSearchControl = false;
		this.btnActive.Location = new System.Drawing.Point(379, 35);
		this.btnActive.Name = "btnActive";
		this.btnActive.Size = new System.Drawing.Size(61, 21);
		this.btnActive.TabIndex = 40;
		this.btnActive.Text = "Active";
		this.btnAccess.Enabled = false;
		this.btnAccess.FieldName = null;
		this.btnAccess.IsSearchControl = false;
		this.btnAccess.Location = new System.Drawing.Point(312, 35);
		this.btnAccess.Name = "btnAccess";
		this.btnAccess.Size = new System.Drawing.Size(61, 21);
		this.btnAccess.TabIndex = 41;
		this.btnAccess.Text = "Access";
		this.splitUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitUser.Location = new System.Drawing.Point(1, 64);
		this.splitUser.Name = "splitUser";
		this.splitUser.Panel1.Controls.Add(this.xtraTabUser);
		this.splitUser.Panel1.Text = "Panel1";
		this.splitUser.Panel2.Controls.Add(this.splitMesage);
		this.splitUser.Panel2.Text = "Panel2";
		this.splitUser.Size = new System.Drawing.Size(1609, 581);
		this.splitUser.SplitterPosition = 219;
		this.splitUser.TabIndex = 46;
		this.splitUser.Text = "splitContainerControl1";
		this.xtraTabUser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabUser.IsSearchControl = false;
		this.xtraTabUser.Location = new System.Drawing.Point(0, 0);
		this.xtraTabUser.Name = "xtraTabUser";
		this.xtraTabUser.SelectedTabPage = this.xtraTabPageAll;
		this.xtraTabUser.Size = new System.Drawing.Size(219, 581);
		this.xtraTabUser.TabIndex = 0;
		this.xtraTabUser.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageAll, this.xtraTabPageComment });
		this.xtraTabPageAll.Controls.Add(this.grdMessage);
		this.xtraTabPageAll.FieldName = "";
		this.xtraTabPageAll.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageAll.Image");
		this.xtraTabPageAll.IsSearchControl = false;
		this.xtraTabPageAll.Name = "xtraTabPageAll";
		this.xtraTabPageAll.Size = new System.Drawing.Size(213, 534);
		this.xtraTabPageAll.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageAll.Tooltip = "All";
		this.grdMessage.AllowPasteExcel = false;
		this.grdMessage.ClipboardData = null;
		this.grdMessage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdMessage.Location = new System.Drawing.Point(0, 0);
		this.grdMessage.MainView = this.tileViewMessage;
		this.grdMessage.Name = "grdMessage";
		this.grdMessage.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rMessageAvatar });
		this.grdMessage.Size = new System.Drawing.Size(213, 534);
		this.grdMessage.TabIndex = 0;
		this.grdMessage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileViewMessage });
		this.tileViewMessage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.tvcMessageAvatar, this.tvcMessageName, this.tvcMessageUnread });
		this.tileViewMessage.GridControl = this.grdMessage;
		this.tileViewMessage.Name = "tileViewMessage";
		this.tileViewMessage.OptionsTiles.IndentBetweenGroups = 0;
		this.tileViewMessage.OptionsTiles.IndentBetweenItems = 0;
		this.tileViewMessage.OptionsTiles.ItemSize = new System.Drawing.Size(248, 64);
		this.tileViewMessage.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileViewMessage.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileViewMessage.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileViewMessage.OptionsTiles.RowCount = 0;
		tableColumnDefinition.Length.Value = 74.0;
		tableColumnDefinition2.Length.Value = 25.0;
		tableColumnDefinition3.Length.Value = 128.0;
		this.tileViewMessage.TileColumns.Add(tableColumnDefinition);
		this.tileViewMessage.TileColumns.Add(tableColumnDefinition2);
		this.tileViewMessage.TileColumns.Add(tableColumnDefinition3);
		this.tileViewMessage.TileRows.Add(obj);
		this.tileViewMessage.TileRows.Add(obj2);
		this.tileViewMessage.TileRows.Add(obj3);
		tableSpan.ColumnSpan = 2;
		tableSpan.RowSpan = 3;
		this.tileViewMessage.TileSpans.Add(tableSpan);
		tileViewItemElement.Column = this.tvcMessageAvatar;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.Text = "tvcMessageAvatar";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Column = this.tvcMessageName;
		tileViewItemElement2.ColumnIndex = 2;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 1;
		tileViewItemElement2.Text = "tvcMessageName";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.Appearance.Hovered.Font = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		tileViewItemElement3.Appearance.Hovered.ForeColor = System.Drawing.Color.Red;
		tileViewItemElement3.Appearance.Hovered.Options.UseFont = true;
		tileViewItemElement3.Appearance.Hovered.Options.UseForeColor = true;
		tileViewItemElement3.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		tileViewItemElement3.Appearance.Normal.ForeColor = System.Drawing.Color.Red;
		tileViewItemElement3.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement3.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement3.Column = this.tvcMessageUnread;
		tileViewItemElement3.ColumnIndex = 2;
		tileViewItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.Text = "tvcMessageUnread";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopRight;
		this.tileViewMessage.TileTemplate.Add(tileViewItemElement);
		this.tileViewMessage.TileTemplate.Add(tileViewItemElement2);
		this.tileViewMessage.TileTemplate.Add(tileViewItemElement3);
		this.tileViewMessage.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.tileViewMessage_FocusedRowObjectChanged);
		this.tileViewMessage.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.tileViewMessage_MouseWheel);
		this.xtraTabPageComment.FieldName = "";
		this.xtraTabPageComment.Image = (System.Drawing.Image)resources.GetObject("xtraTabPageComment.Image");
		this.xtraTabPageComment.IsSearchControl = false;
		this.xtraTabPageComment.Name = "xtraTabPageComment";
		this.xtraTabPageComment.Size = new System.Drawing.Size(213, 534);
		this.xtraTabPageComment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageComment.Tooltip = "Comments";
		this.splitMesage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitMesage.Location = new System.Drawing.Point(0, 0);
		this.splitMesage.Name = "splitMesage";
		this.splitMesage.Panel1.Controls.Add(this.radChatControl);
		this.splitMesage.Panel1.Text = "Panel1";
		this.splitMesage.Panel2.Controls.Add(this.xtraTabOrder);
		this.splitMesage.Panel2.Text = "Panel2";
		this.splitMesage.Size = new System.Drawing.Size(1385, 581);
		this.splitMesage.SplitterPosition = 967;
		this.splitMesage.TabIndex = 0;
		this.splitMesage.Text = "splitContainerControl2";
		this.radChatControl.AutoAddUserMessages = false;
		this.radChatControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
		this.radChatControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.radChatControl.Location = new System.Drawing.Point(0, 0);
		this.radChatControl.Name = "radChatControl";
		this.radChatControl.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 360, 500);
		this.radChatControl.Size = new System.Drawing.Size(967, 581);
		this.radChatControl.TabIndex = 1;
		this.radChatControl.TimeSeparatorInterval = System.TimeSpan.Parse("1.00:00:00");
		this.xtraTabOrder.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabOrder.IsSearchControl = false;
		this.xtraTabOrder.Location = new System.Drawing.Point(0, 0);
		this.xtraTabOrder.Name = "xtraTabOrder";
		this.xtraTabOrder.SelectedTabPage = this.xtraTabPageCustomer;
		this.xtraTabOrder.Size = new System.Drawing.Size(413, 581);
		this.xtraTabOrder.TabIndex = 0;
		this.xtraTabOrder.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageCustomer, this.xtraTabPageOrder });
		this.xtraTabPageCustomer.Controls.Add(this.labelControl4);
		this.xtraTabPageCustomer.Controls.Add(this.lbInfoBirthday);
		this.xtraTabPageCustomer.Controls.Add(this.labelControl3);
		this.xtraTabPageCustomer.Controls.Add(this.labelControl2);
		this.xtraTabPageCustomer.Controls.Add(this.labelControl1);
		this.xtraTabPageCustomer.Controls.Add(this.lbInfoFacebookLocation);
		this.xtraTabPageCustomer.Controls.Add(this.lbInfoFacebookEmail);
		this.xtraTabPageCustomer.Controls.Add(this.lbInfoFacebookPhoneNumber);
		this.xtraTabPageCustomer.Controls.Add(this.lbInfoFacebookName);
		this.xtraTabPageCustomer.Controls.Add(this.pictureInfoUser);
		this.xtraTabPageCustomer.FieldName = "";
		this.xtraTabPageCustomer.IsSearchControl = false;
		this.xtraTabPageCustomer.Name = "xtraTabPageCustomer";
		this.xtraTabPageCustomer.Size = new System.Drawing.Size(407, 553);
		this.xtraTabPageCustomer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageCustomer.Text = "Infomation";
		this.labelControl4.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl4.Appearance.Image");
		this.labelControl4.Appearance.Options.UseImage = true;
		this.labelControl4.Location = new System.Drawing.Point(111, 105);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(16, 16);
		this.labelControl4.TabIndex = 9;
		this.labelControl4.Visible = false;
		this.lbInfoBirthday.AutoSize = true;
		this.lbInfoBirthday.FieldName = null;
		this.lbInfoBirthday.HideWhenEmpty = false;
		this.lbInfoBirthday.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInfoBirthday.Location = new System.Drawing.Point(131, 104);
		this.lbInfoBirthday.Name = "lbInfoBirthday";
		this.lbInfoBirthday.ShowUnderLine = true;
		this.lbInfoBirthday.Size = new System.Drawing.Size(2836, 20);
		this.lbInfoBirthday.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInfoBirthday.TabIndex = 8;
		this.lbInfoBirthday.Utext = "Chưa có thông tin";
		this.lbInfoBirthday.Visible = false;
		this.labelControl3.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl3.Appearance.Image");
		this.labelControl3.Appearance.Options.UseImage = true;
		this.labelControl3.Location = new System.Drawing.Point(111, 79);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(16, 16);
		this.labelControl3.TabIndex = 7;
		this.labelControl2.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl2.Appearance.Image");
		this.labelControl2.Appearance.Options.UseImage = true;
		this.labelControl2.Location = new System.Drawing.Point(111, 54);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(16, 16);
		this.labelControl2.TabIndex = 6;
		this.labelControl1.Appearance.Image = (System.Drawing.Image)resources.GetObject("labelControl1.Appearance.Image");
		this.labelControl1.Appearance.Options.UseImage = true;
		this.labelControl1.Location = new System.Drawing.Point(111, 29);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(16, 16);
		this.labelControl1.TabIndex = 5;
		this.lbInfoFacebookLocation.AutoSize = true;
		this.lbInfoFacebookLocation.FieldName = null;
		this.lbInfoFacebookLocation.HideWhenEmpty = false;
		this.lbInfoFacebookLocation.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInfoFacebookLocation.Location = new System.Drawing.Point(131, 79);
		this.lbInfoFacebookLocation.Name = "lbInfoFacebookLocation";
		this.lbInfoFacebookLocation.ShowUnderLine = true;
		this.lbInfoFacebookLocation.Size = new System.Drawing.Size(2806, 20);
		this.lbInfoFacebookLocation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInfoFacebookLocation.TabIndex = 4;
		this.lbInfoFacebookLocation.Utext = "Chưa có thông tin";
		this.lbInfoFacebookEmail.AutoSize = true;
		this.lbInfoFacebookEmail.FieldName = null;
		this.lbInfoFacebookEmail.HideWhenEmpty = false;
		this.lbInfoFacebookEmail.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInfoFacebookEmail.Location = new System.Drawing.Point(131, 54);
		this.lbInfoFacebookEmail.Name = "lbInfoFacebookEmail";
		this.lbInfoFacebookEmail.ShowUnderLine = true;
		this.lbInfoFacebookEmail.Size = new System.Drawing.Size(2794, 20);
		this.lbInfoFacebookEmail.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInfoFacebookEmail.TabIndex = 3;
		this.lbInfoFacebookEmail.Utext = "Chưa có thông tin";
		this.lbInfoFacebookPhoneNumber.AutoSize = true;
		this.lbInfoFacebookPhoneNumber.FieldName = null;
		this.lbInfoFacebookPhoneNumber.HideWhenEmpty = false;
		this.lbInfoFacebookPhoneNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInfoFacebookPhoneNumber.Location = new System.Drawing.Point(131, 29);
		this.lbInfoFacebookPhoneNumber.Name = "lbInfoFacebookPhoneNumber";
		this.lbInfoFacebookPhoneNumber.ShowUnderLine = true;
		this.lbInfoFacebookPhoneNumber.Size = new System.Drawing.Size(2782, 20);
		this.lbInfoFacebookPhoneNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInfoFacebookPhoneNumber.TabIndex = 2;
		this.lbInfoFacebookPhoneNumber.Utext = "Chưa có thông tin";
		this.lbInfoFacebookName.AutoSize = true;
		this.lbInfoFacebookName.FieldName = null;
		this.lbInfoFacebookName.HideWhenEmpty = false;
		this.lbInfoFacebookName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInfoFacebookName.Location = new System.Drawing.Point(111, 4);
		this.lbInfoFacebookName.Name = "lbInfoFacebookName";
		this.lbInfoFacebookName.ShowUnderLine = true;
		this.lbInfoFacebookName.Size = new System.Drawing.Size(2770, 20);
		this.lbInfoFacebookName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInfoFacebookName.TabIndex = 1;
		this.lbInfoFacebookName.Utext = "Chưa có thông tin";
		this.pictureInfoUser.Cursor = System.Windows.Forms.Cursors.Default;
		this.pictureInfoUser.Location = new System.Drawing.Point(4, 4);
		this.pictureInfoUser.Name = "pictureInfoUser";
		this.pictureInfoUser.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureInfoUser.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.pictureInfoUser.Properties.ZoomAccelerationFactor = 1.0;
		this.pictureInfoUser.Size = new System.Drawing.Size(100, 120);
		this.pictureInfoUser.TabIndex = 0;
		this.xtraTabPageOrder.Controls.Add(this.txtOrderPriceList);
		this.xtraTabPageOrder.Controls.Add(this.btnOrderCreateOrder);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderNote);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderDelDate);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderTotal);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderFreight);
		this.xtraTabPageOrder.Controls.Add(this.lbOrderTotal);
		this.xtraTabPageOrder.Controls.Add(this.lbOrderLogisticAmount);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderCity);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderCountry);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderAddress);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderPhoneNumber);
		this.xtraTabPageOrder.Controls.Add(this.txtOrderName);
		this.xtraTabPageOrder.Controls.Add(this.grdOrderItem);
		this.xtraTabPageOrder.FieldName = "";
		this.xtraTabPageOrder.IsSearchControl = false;
		this.xtraTabPageOrder.Name = "xtraTabPageOrder";
		this.xtraTabPageOrder.Size = new System.Drawing.Size(407, 553);
		this.xtraTabPageOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageOrder.Text = "Order";
		this.txtOrderPriceList.FieldName = null;
		this.txtOrderPriceList.IsSearchControl = false;
		this.txtOrderPriceList.Location = new System.Drawing.Point(159, 87);
		this.txtOrderPriceList.Name = "txtOrderPriceList";
		this.txtOrderPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOrderPriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.txtOrderPriceList.Properties.DisplayMember = "Name";
		this.txtOrderPriceList.Properties.NullText = "Bảng giá";
		this.txtOrderPriceList.Properties.ValueMember = "Code";
		this.txtOrderPriceList.Size = new System.Drawing.Size(149, 20);
		this.txtOrderPriceList.TabIndex = 24;
		this.btnOrderCreateOrder.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOrderCreateOrder.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.btnOrderCreateOrder.FieldName = null;
		this.btnOrderCreateOrder.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOrderCreateOrder.ImageOptions.Image");
		this.btnOrderCreateOrder.IsSearchControl = false;
		this.btnOrderCreateOrder.Location = new System.Drawing.Point(295, 527);
		this.btnOrderCreateOrder.Name = "btnOrderCreateOrder";
		this.btnOrderCreateOrder.Size = new System.Drawing.Size(104, 23);
		this.btnOrderCreateOrder.TabIndex = 23;
		this.btnOrderCreateOrder.Text = "Tạo đơn hàng";
		this.btnOrderCreateOrder.Click += new System.EventHandler(this.btnOrderCreateOrder_Click);
		this.txtOrderNote.FieldName = null;
		this.txtOrderNote.IsSearchControl = false;
		this.txtOrderNote.Location = new System.Drawing.Point(4, 113);
		this.txtOrderNote.Name = "txtOrderNote";
		this.txtOrderNote.Properties.NullText = "Ghi chú đơn hàng";
		this.txtOrderNote.Size = new System.Drawing.Size(304, 20);
		this.txtOrderNote.TabIndex = 22;
		this.txtOrderDelDate.EditValue = null;
		this.txtOrderDelDate.FieldName = null;
		this.txtOrderDelDate.Location = new System.Drawing.Point(4, 87);
		this.txtOrderDelDate.Name = "txtOrderDelDate";
		this.txtOrderDelDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOrderDelDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOrderDelDate.Properties.IsSearchControl = false;
		this.txtOrderDelDate.Properties.NullText = "Ngày giao hàng";
		this.txtOrderDelDate.Size = new System.Drawing.Size(150, 20);
		this.txtOrderDelDate.TabIndex = 20;
		this.txtOrderTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtOrderTotal.Currency = "";
		this.txtOrderTotal.FieldName = null;
		this.txtOrderTotal.IsSearchControl = false;
		this.txtOrderTotal.Location = new System.Drawing.Point(251, 502);
		this.txtOrderTotal.Name = "txtOrderTotal";
		this.txtOrderTotal.Properties.Appearance.Options.UseTextOptions = true;
		this.txtOrderTotal.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtOrderTotal.Properties.NullText = "Tổng tiền";
		this.txtOrderTotal.Size = new System.Drawing.Size(150, 20);
		this.txtOrderTotal.TabIndex = 18;
		this.txtOrderFreight.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtOrderFreight.Currency = "";
		this.txtOrderFreight.FieldName = "TotalExpns";
		this.txtOrderFreight.IsSearchControl = false;
		this.txtOrderFreight.Location = new System.Drawing.Point(251, 474);
		this.txtOrderFreight.Name = "txtOrderFreight";
		this.txtOrderFreight.Properties.Appearance.Options.UseTextOptions = true;
		this.txtOrderFreight.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtOrderFreight.Properties.NullText = "Phí vận chuyển";
		this.txtOrderFreight.Size = new System.Drawing.Size(150, 20);
		this.txtOrderFreight.TabIndex = 17;
		this.lbOrderTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbOrderTotal.FieldName = null;
		this.lbOrderTotal.HideWhenEmpty = false;
		this.lbOrderTotal.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOrderTotal.Location = new System.Drawing.Point(146, 505);
		this.lbOrderTotal.Name = "lbOrderTotal";
		this.lbOrderTotal.ShowUnderLine = true;
		this.lbOrderTotal.Size = new System.Drawing.Size(104, 20);
		this.lbOrderTotal.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOrderTotal.TabIndex = 16;
		this.lbOrderTotal.Utext = "Tổng tiền";
		this.lbOrderLogisticAmount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbOrderLogisticAmount.FieldName = null;
		this.lbOrderLogisticAmount.HideWhenEmpty = false;
		this.lbOrderLogisticAmount.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOrderLogisticAmount.Location = new System.Drawing.Point(146, 477);
		this.lbOrderLogisticAmount.Name = "lbOrderLogisticAmount";
		this.lbOrderLogisticAmount.ShowUnderLine = true;
		this.lbOrderLogisticAmount.Size = new System.Drawing.Size(104, 20);
		this.lbOrderLogisticAmount.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOrderLogisticAmount.TabIndex = 15;
		this.lbOrderLogisticAmount.Utext = "Phí vận chuyển";
		this.txtOrderCity.FieldName = null;
		this.txtOrderCity.IsSearchControl = false;
		this.txtOrderCity.Location = new System.Drawing.Point(159, 61);
		this.txtOrderCity.Name = "txtOrderCity";
		this.txtOrderCity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOrderCity.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.txtOrderCity.Properties.DisplayMember = "Name";
		this.txtOrderCity.Properties.NullText = "Tỉnh";
		this.txtOrderCity.Properties.ShowHeader = false;
		this.txtOrderCity.Properties.ValueMember = "Code";
		this.txtOrderCity.Size = new System.Drawing.Size(149, 20);
		this.txtOrderCity.TabIndex = 12;
		this.txtOrderCountry.FieldName = null;
		this.txtOrderCountry.IsSearchControl = false;
		this.txtOrderCountry.Location = new System.Drawing.Point(4, 61);
		this.txtOrderCountry.Name = "txtOrderCountry";
		this.txtOrderCountry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOrderCountry.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.txtOrderCountry.Properties.DisplayMember = "Name";
		this.txtOrderCountry.Properties.NullText = "Nước";
		this.txtOrderCountry.Properties.ShowHeader = false;
		this.txtOrderCountry.Properties.ValueMember = "Code";
		this.txtOrderCountry.Size = new System.Drawing.Size(149, 20);
		this.txtOrderCountry.TabIndex = 11;
		this.txtOrderCountry.EditValueChanged += new System.EventHandler(this.txtOrderCountry_EditValueChanged);
		this.txtOrderAddress.FieldName = null;
		this.txtOrderAddress.IsSearchControl = false;
		this.txtOrderAddress.Location = new System.Drawing.Point(4, 34);
		this.txtOrderAddress.Name = "txtOrderAddress";
		this.txtOrderAddress.Properties.NullText = "Địa chỉ";
		this.txtOrderAddress.Size = new System.Drawing.Size(305, 20);
		this.txtOrderAddress.TabIndex = 10;
		this.txtOrderPhoneNumber.FieldName = null;
		this.txtOrderPhoneNumber.IsSearchControl = false;
		this.txtOrderPhoneNumber.Location = new System.Drawing.Point(159, 7);
		this.txtOrderPhoneNumber.Name = "txtOrderPhoneNumber";
		this.txtOrderPhoneNumber.Properties.NullText = "Số điện thoại";
		this.txtOrderPhoneNumber.Size = new System.Drawing.Size(150, 20);
		this.txtOrderPhoneNumber.TabIndex = 9;
		this.txtOrderName.FieldName = null;
		this.txtOrderName.IsSearchControl = false;
		this.txtOrderName.Location = new System.Drawing.Point(3, 7);
		this.txtOrderName.Name = "txtOrderName";
		this.txtOrderName.Properties.NullText = "Tên khách hàng";
		this.txtOrderName.Size = new System.Drawing.Size(150, 20);
		this.txtOrderName.TabIndex = 8;
		this.grdOrderItem.AllowPasteExcel = false;
		this.grdOrderItem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdOrderItem.ClipboardData = null;
		this.grdOrderItem.ContextMenuStrip = this.contextMenuStripOrderItem;
		this.grdOrderItem.Location = new System.Drawing.Point(2, 139);
		this.grdOrderItem.MainView = this.gvOrderItem;
		this.grdOrderItem.Name = "grdOrderItem";
		this.grdOrderItem.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rPrice, this.rCalEdit, this.rOrderItem });
		this.grdOrderItem.Size = new System.Drawing.Size(404, 329);
		this.grdOrderItem.TabIndex = 7;
		this.grdOrderItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvOrderItem });
		this.contextMenuStripOrderItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStripOrderItem.Name = "contextMenuStripOrderItem";
		this.contextMenuStripOrderItem.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvOrderItem.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvOrderItem.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvOrderItem.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvOrderItem.ColumnKey = null;
		this.gvOrderItem.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcOrderItemName, this.gcOrderQuantity, this.gcOrderPriceBefDi, this.gcOrderDiscPrcnt, this.gcOrderItemCode, this.gcOrderLineTotal });
		this.gvOrderItem.DisableFormSetting = false;
		this.gvOrderItem.DisplayException = false;
		this.gvOrderItem.EditColumn = null;
		this.gvOrderItem.FieldCurrency = null;
		this.gvOrderItem.FieldRate = null;
		this.gvOrderItem.FormId = null;
		this.gvOrderItem.GridControl = this.grdOrderItem;
		this.gvOrderItem.IndexEditRow = -1;
		this.gvOrderItem.IndicatorWidth = 35;
		this.gvOrderItem.IsApzTable = false;
		this.gvOrderItem.IsInsertNewRow = false;
		this.gvOrderItem.IsLoadingData = false;
		this.gvOrderItem.ItemId = null;
		this.gvOrderItem.LstColumnAllowBatch = null;
		this.gvOrderItem.LstColumnNotAllowBatch = null;
		this.gvOrderItem.LstColumNotSetting = null;
		this.gvOrderItem.Name = "gvOrderItem";
		this.gvOrderItem.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvOrderItem.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvOrderItem.OptionsPrint.AutoWidth = false;
		this.gvOrderItem.OptionsPrint.UsePrintStyles = false;
		this.gvOrderItem.OptionsView.BestFitMaxRowCount = 100;
		this.gvOrderItem.OptionsView.ColumnAutoWidth = false;
		this.gvOrderItem.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvOrderItem.OptionsView.ShowGroupedColumns = true;
		this.gvOrderItem.OptionsView.ShowGroupPanel = false;
		this.gvOrderItem.UseBatchMod = false;
		this.gvOrderItem.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvOrderItem_CellValueChanged);
		this.gcOrderItemName.Caption = "Mặt hàng";
		this.gcOrderItemName.ColumnEdit = this.rOrderItem;
		this.gcOrderItemName.CurrencyDateField = null;
		this.gcOrderItemName.CurrencyField = null;
		this.gcOrderItemName.CurrencyFixed = null;
		this.gcOrderItemName.CurrencyFixedRate = 0.0;
		this.gcOrderItemName.CurrencyRateField = null;
		this.gcOrderItemName.FieldName = "Dscription";
		this.gcOrderItemName.Name = "gcOrderItemName";
		this.gcOrderItemName.PrimaryKey = false;
		this.gcOrderItemName.UnitField = null;
		this.gcOrderItemName.Visible = true;
		this.gcOrderItemName.VisibleIndex = 0;
		this.rOrderItem.AutoHeight = false;
		this.rOrderItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rOrderItem.Name = "rOrderItem";
		this.rOrderItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rOrderItemName_ButtonClick);
		this.gcOrderQuantity.Caption = "Số lượng";
		this.gcOrderQuantity.ColumnEdit = this.rCalEdit;
		this.gcOrderQuantity.CurrencyDateField = null;
		this.gcOrderQuantity.CurrencyField = null;
		this.gcOrderQuantity.CurrencyFixed = null;
		this.gcOrderQuantity.CurrencyFixedRate = 0.0;
		this.gcOrderQuantity.CurrencyRateField = null;
		this.gcOrderQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcOrderQuantity.FieldName = "Quantity";
		this.gcOrderQuantity.Name = "gcOrderQuantity";
		this.gcOrderQuantity.PrimaryKey = false;
		this.gcOrderQuantity.UnitField = null;
		this.gcOrderQuantity.Visible = true;
		this.gcOrderQuantity.VisibleIndex = 1;
		this.rCalEdit.AutoHeight = false;
		this.rCalEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCalEdit.Name = "rCalEdit";
		this.gcOrderPriceBefDi.Caption = "Giá";
		this.gcOrderPriceBefDi.ColumnEdit = this.rPrice;
		this.gcOrderPriceBefDi.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcOrderPriceBefDi.CurrencyDateField = null;
		this.gcOrderPriceBefDi.CurrencyField = "Currency";
		this.gcOrderPriceBefDi.CurrencyFixed = null;
		this.gcOrderPriceBefDi.CurrencyFixedRate = 1.0;
		this.gcOrderPriceBefDi.CurrencyRateField = null;
		this.gcOrderPriceBefDi.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcOrderPriceBefDi.FieldName = "PriceBefDi";
		this.gcOrderPriceBefDi.Name = "gcOrderPriceBefDi";
		this.gcOrderPriceBefDi.PrimaryKey = false;
		this.gcOrderPriceBefDi.UnitField = null;
		this.gcOrderPriceBefDi.Visible = true;
		this.gcOrderPriceBefDi.VisibleIndex = 2;
		this.gcOrderPriceBefDi.Width = 56;
		this.rPrice.AutoHeight = false;
		this.rPrice.Name = "rPrice";
		this.gcOrderDiscPrcnt.Caption = "Khuyến mãi";
		this.gcOrderDiscPrcnt.ColumnEdit = this.rCalEdit;
		this.gcOrderDiscPrcnt.CurrencyDateField = null;
		this.gcOrderDiscPrcnt.CurrencyField = null;
		this.gcOrderDiscPrcnt.CurrencyFixed = null;
		this.gcOrderDiscPrcnt.CurrencyFixedRate = 0.0;
		this.gcOrderDiscPrcnt.CurrencyRateField = null;
		this.gcOrderDiscPrcnt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcOrderDiscPrcnt.FieldName = "DiscPrcnt";
		this.gcOrderDiscPrcnt.Name = "gcOrderDiscPrcnt";
		this.gcOrderDiscPrcnt.PrimaryKey = false;
		this.gcOrderDiscPrcnt.UnitField = null;
		this.gcOrderDiscPrcnt.Visible = true;
		this.gcOrderDiscPrcnt.VisibleIndex = 3;
		this.gcOrderItemCode.Caption = "Mã Hàng";
		this.gcOrderItemCode.CurrencyDateField = null;
		this.gcOrderItemCode.CurrencyField = null;
		this.gcOrderItemCode.CurrencyFixed = null;
		this.gcOrderItemCode.CurrencyFixedRate = 0.0;
		this.gcOrderItemCode.CurrencyRateField = null;
		this.gcOrderItemCode.FieldName = "ItemCode";
		this.gcOrderItemCode.Name = "gcOrderItemCode";
		this.gcOrderItemCode.PrimaryKey = false;
		this.gcOrderItemCode.UnitField = null;
		this.gcOrderLineTotal.Caption = "Tổng tiền";
		this.gcOrderLineTotal.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcOrderLineTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcOrderLineTotal.CurrencyDateField = null;
		this.gcOrderLineTotal.CurrencyField = "";
		this.gcOrderLineTotal.CurrencyFixed = null;
		this.gcOrderLineTotal.CurrencyFixedRate = 0.0;
		this.gcOrderLineTotal.CurrencyRateField = "";
		this.gcOrderLineTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcOrderLineTotal.FieldName = "LineTotal";
		this.gcOrderLineTotal.Name = "gcOrderLineTotal";
		this.gcOrderLineTotal.PrimaryKey = false;
		this.gcOrderLineTotal.UnitField = null;
		this.gcOrderLineTotal.Visible = true;
		this.gcOrderLineTotal.VisibleIndex = 4;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(1529, 651);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 47;
		this.btnCancel.Text = "Cancel";
		this.lbFacebookUser.FieldName = null;
		this.lbFacebookUser.HideWhenEmpty = false;
		this.lbFacebookUser.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFacebookUser.Location = new System.Drawing.Point(13, 12);
		this.lbFacebookUser.Name = "lbFacebookUser";
		this.lbFacebookUser.ShowUnderLine = true;
		this.lbFacebookUser.Size = new System.Drawing.Size(90, 20);
		this.lbFacebookUser.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFacebookUser.TabIndex = 48;
		this.lbFacebookUser.Utext = "Facebook User";
		this.lbFacebookPage.FieldName = null;
		this.lbFacebookPage.HideWhenEmpty = false;
		this.lbFacebookPage.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFacebookPage.Location = new System.Drawing.Point(13, 39);
		this.lbFacebookPage.Name = "lbFacebookPage";
		this.lbFacebookPage.ShowUnderLine = true;
		this.lbFacebookPage.Size = new System.Drawing.Size(90, 20);
		this.lbFacebookPage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFacebookPage.TabIndex = 49;
		this.lbFacebookPage.Utext = "Facebook Page";
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1611, 680);
		base.Controls.Add(this.lbFacebookPage);
		base.Controls.Add(this.lbFacebookUser);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.splitUser);
		base.Controls.Add(this.txtFacebookPage);
		base.Controls.Add(this.btnCheck);
		base.Controls.Add(this.txtFacebookUserName);
		base.Controls.Add(this.btnLogin);
		base.Controls.Add(this.btnActive);
		base.Controls.Add(this.btnAccess);
		base.Name = "frm_FacebookManager";
		base.ShowIcon = false;
		this.Text = "Management Facebook Channel";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_FacebookManager_OnControlValueChanged);
		base.Load += new System.EventHandler(this.frm_FacebookManager_Load);
		((System.ComponentModel.ISupportInitialize)this.rMessageAvatar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFacebookPage.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFacebookUserName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitUser).EndInit();
		this.splitUser.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabUser).EndInit();
		this.xtraTabUser.ResumeLayout(false);
		this.xtraTabPageAll.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdMessage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileViewMessage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitMesage).EndInit();
		this.splitMesage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.radChatControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabOrder).EndInit();
		this.xtraTabOrder.ResumeLayout(false);
		this.xtraTabPageCustomer.ResumeLayout(false);
		this.xtraTabPageCustomer.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureInfoUser.Properties).EndInit();
		this.xtraTabPageOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtOrderPriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderNote.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderDelDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderDelDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderFreight.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderCity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderCountry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderPhoneNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOrderName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdOrderItem).EndInit();
		this.contextMenuStripOrderItem.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvOrderItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOrderItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).EndInit();
		base.ResumeLayout(false);
	}
}
