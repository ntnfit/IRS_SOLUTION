using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper.HubHelpers;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Microsoft.AspNet.SignalR.Client;

namespace Apzon.Forms.UserControls;

public class ucComment : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool saveComment;

	private bool clearUnread;

	private bool refreshComment;

	private DataTable dtCLG1 = new DataTable();

	private DataTable dtUnread = new DataTable();

	private Dictionary<int, string> dictionUnread = new Dictionary<int, string>();

	private int rowHandle = -1;

	private Color userNameColor = Color.FromArgb(29, 114, 115);

	private int _id;

	private bool autoGridSize = true;

	private HubClientHelper hubConnect;

	private List<int> additionNotifyUser = new List<int>();

	private ApzObjectTypes objectType;

	private IContainer components;

	public MemoEditEx memoComment;

	public CustomGridControl grd_Comment;

	public CustomGridView gv_Comment;

	private GridColumn gc_ID;

	private GridColumn gc_CmtCode;

	private GridColumn gc_UserName;

	private GridColumn gc_CreatedTime;

	private GridColumn gc_Delete;

	public RepositoryItemButtonEdit rep_bte_Delete;

	public RepositoryItemTextEdit rep_txt_Comment;

	public RepositoryItemMemoEdit rep_memo_Comment;

	public RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private GridColumn gc_Type;

	private GridColumn gc_Content;

	private GridColumn gc_UserSign;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	public List<int> AdditionNotifyUser
	{
		get
		{
			return additionNotifyUser;
		}
		set
		{
			additionNotifyUser = value;
		}
	}

	[DefaultValue(true)]
	public bool AutoGridSize
	{
		get
		{
			return autoGridSize;
		}
		set
		{
			autoGridSize = value;
		}
	}

	public ApzObjectTypes ObjectType
	{
		get
		{
			return objectType;
		}
		set
		{
			objectType = value;
		}
	}

	public string ObjectTypeString
	{
		get
		{
			int num = (int)objectType;
			string result = "";
			if (num != 0)
			{
				result = Function.ToString(num);
			}
			return result;
		}
	}

	public event EventHandler BeforeSendComment;

	public ucComment()
	{
		try
		{
			InitializeComponent();
			gv_Comment.Columns["CmtCode"].SortOrder = ColumnSortOrder.Ascending;
			gv_Comment.OptionsView.ShowColumnHeaders = false;
			gv_Comment.OptionsView.ShowGroupPanel = false;
			gv_Comment.OptionsView.ShowHorizontalLines = DefaultBoolean.False;
			gv_Comment.OptionsView.ShowVerticalLines = DefaultBoolean.False;
			gv_Comment.OptionsView.ShowIndicator = false;
			gv_Comment.OptionsView.ShowPreview = true;
			gv_Comment.OptionsView.ShowVerticalLines = DefaultBoolean.False;
			gv_Comment.OptionsView.ColumnAutoWidth = true;
			gv_Comment.Appearance.Preview.ForeColor = Color.FromArgb(38, 38, 38);
			gv_Comment.Appearance.Preview.Options.UseForeColor = true;
			gv_Comment.OptionsView.AutoCalcPreviewLineCount = true;
			gv_Comment.FocusRectStyle = DrawFocusRectStyle.None;
			gv_Comment.OptionsSelection.EnableAppearanceFocusedCell = false;
			gv_Comment.OptionsSelection.EnableAppearanceFocusedRow = false;
			gv_Comment.OptionsSelection.EnableAppearanceHideSelection = false;
			userNameColor = Color.FromArgb(46, 89, 111);
			gc_UserName.AppearanceCell.ForeColor = userNameColor;
			gv_Comment.Appearance.Preview.ForeColor = Color.FromArgb(68, 68, 80);
			gv_Comment.Appearance.FocusedRow.ForeColor = userNameColor;
			gv_Comment.Appearance.FocusedCell.ForeColor = userNameColor;
			Font font = new Font("Tahoma", 8.25f);
			gv_Comment.Appearance.Preview.Font = font;
			repositoryItemMemoEdit1.Appearance.Font = font;
		}
		catch (Exception)
		{
		}
	}

	private async void ucComment_Load(object sender, EventArgs e)
	{
		try
		{
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				Form form = FindForm();
				if (form != null)
				{
					hubConnect = new HubClientHelper(form);
					HubConnection hubConnection = hubConnect.HubConnection;
					hubConnection.CreateHubProxy("NotificationClient").On<NotifiedObj>("UpdateNotification", UpdateNotification);
					await hubConnection.Start();
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
		BeginInvoke((Action)(async () =>
		{
			try
			{
				if (obj.TranType == Enums.TransactionType.M && !obj.CreateUserCode.Equals(GlobalData.UserName))
				{
					await RefreshComment(_id);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}));
	}

	public async Task RefreshComment(int Id)
	{
		if (refreshComment)
		{
			return;
		}
		try
		{
			await LoadTableStructure();
			_id = Id;
			if (_id > 0 && !string.IsNullOrEmpty(ObjectTypeString))
			{
				refreshComment = true;
				int userID = GlobalData.UserID;
				DataSet dataSet = await _httpClient.SubActivities.LoadComment(_id, ObjectTypeString, userID);
				if (dataSet.IsNotNull())
				{
					grd_Comment.DataSource = dataSet.Tables[0];
					if (dataSet.Tables[1].IsNotNull())
					{
						dtUnread = dataSet.Tables[1];
					}
					else
					{
						dtUnread.Clear();
					}
				}
				else
				{
					ClearData();
				}
			}
			else
			{
				ClearData();
			}
			(grd_Comment.DataSource as DataTable).AcceptChanges();
			grd_Comment.RefreshDataSource();
			gv_Comment.MoveLastVisible();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			refreshComment = false;
		}
	}

	private void ClearData()
	{
		grd_Comment.DataSource = dtCLG1.Clone();
		dtUnread.Clear();
	}

	private async Task LoadTableStructure()
	{
		_ = 1;
		try
		{
			if (dtCLG1.Columns.Count <= 0)
			{
				dtCLG1 = await WinFuntions.GetTableStructureAsync("APZ_CLG1");
			}
			if (dtUnread.Columns.Count <= 0)
			{
				dtUnread = await WinFuntions.GetTableStructureAsync("APZ_CLG3");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public async Task RemoveCommentById(int Id)
	{
		try
		{
			if (Id != 0 && Id > 0 && !string.IsNullOrEmpty(ObjectTypeString))
			{
				string text = await _httpClient.SubActivities.RemoveCommentById(Id, ObjectTypeString);
				if (!(text == ""))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text.ToString());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void RemoveComment(int rowHandle)
	{
		try
		{
			int id = Function.ParseInt(gv_Comment.GetRowCellValue(rowHandle, "CmtCode"));
			string text = await _httpClient.SubActivities.DeleteComment(id);
			if (string.IsNullOrWhiteSpace(text))
			{
				gv_Comment.BeginUpdate();
				gv_Comment.DeleteRow(rowHandle);
				gv_Comment.EndUpdate();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task SaveComment(string commentText = "")
	{
		_ = 1;
		try
		{
			await LoadTableStructure();
			if (commentText == "")
			{
				commentText = Function.ToString(memoComment.Text);
			}
			if (string.IsNullOrEmpty(commentText) || !dtCLG1.HasColumn())
			{
				return;
			}
			dtCLG1.Clear();
			DataRow dataRow = dtCLG1.NewRow();
			dataRow["Id"] = _id;
			dataRow["ObjType"] = ObjectTypeString;
			dataRow["CmtCode"] = 0;
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["UserName"] = GlobalData.UserNameLogin;
			dataRow["CreatedTime"] = DateTime.Now;
			dataRow["Content"] = commentText;
			dtCLG1.Rows.Add(dataRow);
			dtCLG1.AcceptChanges();
			object[] drCopy = dtCLG1.Rows[0].ItemArray;
			if (dtCLG1.IsNotNull())
			{
				if (BeforeSendComment != null)
				{
					BeforeSendComment(this, EventArgs.Empty);
				}
				HttpResult httpResult = await _httpClient.SubActivities.SaveComment(dtCLG1, additionNotifyUser);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					DataTable obj = grd_Comment.DataSource as DataTable;
					DataRow dataRow2 = obj.NewRow();
					dataRow2.ItemArray = drCopy;
					int num = Function.ParseInt(httpResult.Content);
					dataRow2["CmtCode"] = num;
					obj.Rows.Add(dataRow2);
					obj.AcceptChanges();
					gv_Comment.MoveLastVisible();
					memoComment.Text = string.Empty;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					memoComment.Text = commentText;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			additionNotifyUser.Clear();
		}
	}

	private void toolStripMenuItem_DeleteCM_Click(object sender, EventArgs e)
	{
		int focusedRowHandle = gv_Comment.FocusedRowHandle;
		TryRemoveComment(focusedRowHandle);
	}

	private void TryRemoveComment(int rowHandle)
	{
		try
		{
			if (Function.ParseInt(gv_Comment.GetRowCellValue(rowHandle, "CmtCode")) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(gv_Comment.GetFocusedRowCellValue("UserSign"));
			if (num != 0)
			{
				int num2 = Function.ParseInt(GlobalData.UserID);
				if (num == num2 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					RemoveComment(rowHandle);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void memoComment_KeyDown(object sender, KeyEventArgs e)
	{
		if (saveComment)
		{
			return;
		}
		try
		{
			if (e.KeyCode == Keys.Return && e.Shift)
			{
				saveComment = true;
				await SaveComment();
			}
		}
		finally
		{
			saveComment = false;
		}
	}

	private void rep_bte_Delete_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if ((sender as BaseEdit).Parent is GridControl gridControl)
			{
				GridView gridView = (GridView)gridControl.MainView;
				if (gridView != null)
				{
					int focusedRowHandle = gridView.FocusedRowHandle;
					TryRemoveComment(focusedRowHandle);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_Comment_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void gv_Comment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void gv_Comment_RowCountChanged(object sender, EventArgs e)
	{
		try
		{
			if (autoGridSize)
			{
				UpdateGridSize(sender as GridView);
			}
		}
		catch (Exception)
		{
		}
	}

	private void UpdateGridSize(GridView view)
	{
		try
		{
			if (view == null)
			{
				return;
			}
			GridViewInfo gridViewInfo = (GridViewInfo)view.GetViewInfo();
			if (gridViewInfo != null)
			{
				int num = gridViewInfo.CalcRealViewHeight(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
				int num2 = Height - 42;
				if (num > num2)
				{
					num = num2;
				}
				view.GridControl.Height = num;
				view.LayoutChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Comment_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			GridColumn column = e.Column;
			if (column != null && column.ColumnType == typeof(DateTime))
			{
				e.DisplayText = WinFuntions.FormatCreatedDate(Function.ToString(e.Value));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Comment_SizeChanged(object sender, EventArgs e)
	{
		if (autoGridSize)
		{
			UpdateGridSize(gv_Comment);
		}
	}

	private void gv_Comment_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridView obj = sender as GridView;
			GridHitInfo gridHitInfo = obj.CalcHitInfo(e.Location);
			if (gridHitInfo.InRowCell)
			{
				rowHandle = gridHitInfo.RowHandle;
			}
			else
			{
				rowHandle = -1;
			}
			obj.Invalidate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Comment_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Delete" || e.RowHandle != rowHandle)
			{
				return;
			}
			int num = Function.ParseInt(gv_Comment.GetRowCellValue(e.RowHandle, "UserSign"));
			if (num == 0)
			{
				return;
			}
			int num2 = Function.ParseInt(GlobalData.UserID);
			if (num == num2)
			{
				ButtonEditViewInfo buttonEditViewInfo = (e.Cell as GridCellInfo).ViewInfo as ButtonEditViewInfo;
				if (e.Column.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
				{
					EditorButton button = repositoryItemButtonEdit.Buttons[0];
					EditorButtonObjectInfoArgs editorButtonObjectInfoArgs = new EditorButtonObjectInfoArgs(e.Cache, button, e.Appearance);
					editorButtonObjectInfoArgs.Bounds = new Rectangle(e.Bounds.Right - 17, e.Bounds.Y, 17, e.Bounds.Height);
					buttonEditViewInfo.EditorButtonPainter.DrawObject(editorButtonObjectInfoArgs);
					Rectangle bounds = e.Bounds;
					bounds.Offset(2, 0);
					e.Appearance.DrawString(e.Cache, e.DisplayText, bounds);
					e.Handled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Comment_RowCellClick(object sender, RowCellClickEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Delete")
			{
				TryRemoveComment(e.RowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repositoryItemMemoEdit1_FormatEditValue(object sender, ConvertEditValueEventArgs e)
	{
	}

	private void gv_Comment_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "UserName")
			{
				return;
			}
			int num = Function.ParseInt(gv_Comment.GetRowCellValue(e.RowHandle, "UserSign"));
			if (num != 0)
			{
				int num2 = Function.ParseInt(GlobalData.UserID);
				if (num == num2)
				{
					Color color = Color.FromArgb(204, 59, 74);
					color = Color.FromArgb(162, 120, 58);
					color = Color.FromArgb(162, 115, 100);
					e.Appearance.ForeColor = color;
				}
				else
				{
					Color foreColor = Color.FromArgb(58, 125, 160);
					e.Appearance.ForeColor = foreColor;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Comment_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			int cmtCode = Function.ParseInt(gv_Comment.GetRowCellValue(e.RowHandle, "CmtCode"));
			DataRow dataRow = dtUnread.AsEnumerable().FirstOrDefault((DataRow dr) => Function.ParseInt(dr["CmtCode"]) == cmtCode);
			if (dataRow != null && Function.ToString(dataRow["Unread"]) == "Y")
			{
				e.Appearance.BackColor = Color.FromArgb(240, 230, 200);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Comment_GotFocus(object sender, EventArgs e)
	{
		ClearUnreadComment();
	}

	private void memoComment_Enter(object sender, EventArgs e)
	{
		ClearUnreadComment();
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		RefreshComment(_id);
	}

	private async Task ClearUnreadComment()
	{
		if (clearUnread)
		{
			return;
		}
		try
		{
			if (!dtUnread.IsNotNull())
			{
				return;
			}
			clearUnread = true;
			foreach (DataRow item in dtUnread.AsEnumerable())
			{
				item["Unread"] = "N";
			}
			dtUnread.AcceptChanges();
			await _httpClient.SubActivities.ClearUnreadComment(dtUnread);
			dtUnread.Clear();
			gv_Comment.LayoutChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			clearUnread = false;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (hubConnect != null)
		{
			hubConnect.Dispose();
		}
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.memoComment = new Apzon.Controls.Editor.MemoEditEx();
		this.grd_Comment = new Apzon.Controls.Grid.CustomGridControl();
		this.gv_Comment = new Apzon.Controls.Grid.CustomGridView();
		this.gc_ID = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_Type = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_CmtCode = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_UserSign = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_UserName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_CreatedTime = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_Content = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
		this.rep_bte_Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_txt_Comment = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.memoComment.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Delete).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_txt_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.memoComment.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoComment.FieldName = null;
		this.memoComment.IsSearchControl = false;
		this.memoComment.Location = new System.Drawing.Point(0, 310);
		this.memoComment.Name = "memoComment";
		this.memoComment.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.memoComment.Properties.Appearance.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.memoComment.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(48, 48, 48);
		this.memoComment.Properties.Appearance.Options.UseBackColor = true;
		this.memoComment.Properties.Appearance.Options.UseFont = true;
		this.memoComment.Properties.Appearance.Options.UseForeColor = true;
		this.memoComment.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.memoComment.Properties.NullValuePrompt = "Add a comment (Shift+Enter) ...  ";
		this.memoComment.Size = new System.Drawing.Size(462, 36);
		this.memoComment.TabIndex = 3;
		this.memoComment.Enter += new System.EventHandler(this.memoComment_Enter);
		this.memoComment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.memoComment_KeyDown);
		this.grd_Comment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Comment.Location = new System.Drawing.Point(0, 0);
		this.grd_Comment.MainView = this.gv_Comment;
		this.grd_Comment.Name = "grd_Comment";
		this.grd_Comment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_bte_Delete, this.rep_txt_Comment, this.repositoryItemMemoEdit1 });
		this.grd_Comment.Size = new System.Drawing.Size(462, 304);
		this.grd_Comment.TabIndex = 4;
		this.grd_Comment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Comment });
		this.grd_Comment.SizeChanged += new System.EventHandler(this.grd_Comment_SizeChanged);
		this.gv_Comment.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.gv_Comment.Appearance.Preview.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.gv_Comment.Appearance.Preview.ForeColor = System.Drawing.Color.FromArgb(48, 48, 60);
		this.gv_Comment.Appearance.Preview.Options.UseForeColor = true;
		this.gv_Comment.Appearance.Row.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.gv_Comment.Appearance.Row.Options.UseBackColor = true;
		this.gv_Comment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_Comment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clChartOfAccounts;
		this.gv_Comment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gc_ID, this.gc_Type, this.gc_CmtCode, this.gc_UserSign, this.gc_UserName, this.gc_CreatedTime, this.gc_Content, this.gc_Delete });
		this.gv_Comment.GridControl = this.grd_Comment;
		this.gv_Comment.IndicatorWidth = 35;
		this.gv_Comment.IsApzTable = false;
		this.gv_Comment.Name = "gv_Comment";
		this.gv_Comment.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
		this.gv_Comment.OptionsView.AutoCalcPreviewLineCount = true;
		this.gv_Comment.OptionsView.RowAutoHeight = true;
		this.gv_Comment.OptionsView.ShowGroupPanel = false;
		this.gv_Comment.OptionsView.ShowPreview = true;
		this.gv_Comment.PreviewFieldName = "Content";
		this.gv_Comment.PreviewIndent = 5;
		this.gv_Comment.PreviewRowEdit = null;
		this.gv_Comment.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_Comment_RowCellClick);
		this.gv_Comment.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gv_Comment_CustomDrawCell);
		this.gv_Comment.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Comment_RowCellStyle);
		this.gv_Comment.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Comment_RowStyle);
		this.gv_Comment.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Comment_CustomRowCellEdit);
		this.gv_Comment.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Comment_CustomRowCellEditForEditing);
		this.gv_Comment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Comment_CustomColumnDisplayText);
		this.gv_Comment.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Comment_MouseMove);
		this.gv_Comment.GotFocus += new System.EventHandler(this.gv_Comment_GotFocus);
		this.gv_Comment.RowCountChanged += new System.EventHandler(this.gv_Comment_RowCountChanged);
		this.gc_ID.Caption = "ID";
		this.gc_ID.FieldName = "Id";
		this.gc_ID.Name = "gc_ID";
		this.gc_ID.OptionsColumn.AllowEdit = false;
		this.gc_Type.Caption = "ObjType";
		this.gc_Type.FieldName = "ObjType";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.OptionsColumn.AllowEdit = false;
		this.gc_CmtCode.Caption = "ClgCode";
		this.gc_CmtCode.FieldName = "CmtCode";
		this.gc_CmtCode.Name = "gc_CmtCode";
		this.gc_CmtCode.OptionsColumn.AllowEdit = false;
		this.gc_UserSign.Caption = "gridColumn1";
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.OptionsColumn.AllowEdit = false;
		this.gc_UserName.AppearanceCell.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.gc_UserName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(31, 99, 151);
		this.gc_UserName.AppearanceCell.Options.UseFont = true;
		this.gc_UserName.AppearanceCell.Options.UseForeColor = true;
		this.gc_UserName.Caption = "UserSign";
		this.gc_UserName.FieldName = "UserName";
		this.gc_UserName.Name = "gc_UserName";
		this.gc_UserName.OptionsColumn.AllowEdit = false;
		this.gc_UserName.Visible = true;
		this.gc_UserName.VisibleIndex = 0;
		this.gc_UserName.Width = 290;
		this.gc_CreatedTime.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.gc_CreatedTime.AppearanceCell.ForeColor = System.Drawing.SystemColors.ControlDark;
		this.gc_CreatedTime.AppearanceCell.Options.UseFont = true;
		this.gc_CreatedTime.AppearanceCell.Options.UseForeColor = true;
		this.gc_CreatedTime.AppearanceCell.Options.UseTextOptions = true;
		this.gc_CreatedTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_CreatedTime.Caption = "CMTime";
		this.gc_CreatedTime.FieldName = "CreatedTime";
		this.gc_CreatedTime.Name = "gc_CreatedTime";
		this.gc_CreatedTime.OptionsColumn.AllowEdit = false;
		this.gc_CreatedTime.Visible = true;
		this.gc_CreatedTime.VisibleIndex = 1;
		this.gc_CreatedTime.Width = 114;
		this.gc_Content.AppearanceCell.Font = new System.Drawing.Font("Verdana", 8.25f);
		this.gc_Content.AppearanceCell.Options.UseFont = true;
		this.gc_Content.Caption = "Comment";
		this.gc_Content.FieldName = "Content";
		this.gc_Content.Name = "gc_Content";
		this.gc_Content.OptionsColumn.AllowEdit = false;
		this.gc_Delete.Caption = "Delete";
		this.gc_Delete.ColumnEdit = this.rep_bte_Delete;
		this.gc_Delete.FieldName = "Delete";
		this.gc_Delete.Name = "gc_Delete";
		this.gc_Delete.OptionsColumn.FixedWidth = true;
		this.gc_Delete.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_Delete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.gc_Delete.Visible = true;
		this.gc_Delete.VisibleIndex = 2;
		this.gc_Delete.Width = 23;
		this.rep_bte_Delete.AutoHeight = false;
		this.rep_bte_Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Clear)
		});
		this.rep_bte_Delete.Name = "rep_bte_Delete";
		this.rep_bte_Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_bte_Delete.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_Delete_ButtonPressed);
		this.rep_txt_Comment.AutoHeight = false;
		this.rep_txt_Comment.Name = "rep_txt_Comment";
		this.rep_txt_Comment.ReadOnly = true;
		this.repositoryItemMemoEdit1.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.repositoryItemMemoEdit1.Appearance.Options.UseBackColor = true;
		this.repositoryItemMemoEdit1.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.repositoryItemMemoEdit1.AppearanceFocused.Options.UseBackColor = true;
		this.repositoryItemMemoEdit1.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
		this.repositoryItemMemoEdit1.AppearanceReadOnly.Options.UseBackColor = true;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.repositoryItemMemoEdit1.ReadOnly = true;
		this.repositoryItemMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.repositoryItemMemoEdit1.FormatEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(this.repositoryItemMemoEdit1_FormatEditValue);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 26);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.grd_Comment);
		base.Controls.Add(this.memoComment);
		base.Name = "ucComment";
		base.Size = new System.Drawing.Size(462, 346);
		base.Load += new System.EventHandler(this.ucComment_Load);
		((System.ComponentModel.ISupportInitialize)this.memoComment.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Delete).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_txt_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
