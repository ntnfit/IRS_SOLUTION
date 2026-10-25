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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator.ApprovalProcess;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Administrator;

public class frm_AlertOverview : XtraFormEx
{
	public enum MailFolder
	{
		All = 0,
		Announcements = 1,
		ASP = 2,
		WinForms = 4,
		IDETools = 8,
		Frameworks = 0x10,
		Deleted = 0x20,
		Custom = 0x400
	}

	public enum MailType
	{
		Inbox,
		Deleted,
		Sent,
		Draft
	}

	public static string FormName = "frm_AlertOverview";

	private const string FormController = "AlertOverview";

	private IContainer components;

	private NavBarControl navBarControl1;

	private NavBarGroup navBarGroup1;

	private NavBarGroupControlContainer navBarGroupControlContainer1;

	private TreeList tv_Alert;

	private TreeListColumnEx colName;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private TreeListColumnEx colType;

	private TreeListColumnEx colFolder;

	private TreeListColumnEx colImageIndex;

	private ImageCollection icFolders;

	private TreeListColumnEx colData;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Alert;

	private ImageCollection icEditors;

	protected internal GridViewEx gv_Alert;

	private GridColumnEx gcPriority;

	private RepositoryItemImageComboBox rep_ImagePriority;

	private GridColumnEx gcIcon;

	private RepositoryItemImageComboBox rep_ImageIcon;

	private GridColumnEx gcAttachment;

	private RepositoryItemImageComboBox rep_ImageAttachment;

	private GridColumnEx gcSubject;

	private GridColumnEx gcFrom;

	private GridColumnEx gcDate;

	private GridColumnEx gcRead;

	private RepositoryItemImageComboBox repositoryItemImageComboBox4;

	private ImageList ilColumns;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private GridColumnEx gridColumn1;

	private static bool AllowControlTextColor => UserLookAndFeel.Default.ActiveSkinName != "Glass Oceans";

	public frm_AlertOverview()
	{
		InitializeComponent();
	}

	private async void frm_AlertOverview_Load(object sender, EventArgs e)
	{
		try
		{
			tv_Alert.AppendNode(new object[3]
			{
				"Inbox",
				MailType.Inbox,
				MailFolder.All
			}, null);
			tv_Alert.AppendNode(new object[4]
			{
				"OutBox",
				MailType.Sent,
				MailFolder.All,
				1
			}, null);
			tv_Alert.AppendNode(new object[4]
			{
				"Sent Message",
				MailType.Sent,
				MailFolder.All,
				2
			}, null);
			tv_Alert.ExpandAll();
			CreateMessagesList(tv_Alert.Nodes);
			tv_Alert.RowHeight = Math.Max(Convert.ToInt32(tv_Alert.Font.GetHeight()), icFolders.ImageSize.Height) + 5;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			int id = tv_Alert.FocusedNode.Id;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AlertOverview");
			DataTable dataSource = await helper.GetAsync("GetAlert", new
			{
				username = GlobalData.UserName,
				mailtype = id
			});
			grd_Alert.DataSource = dataSource;
			grd_Alert.RefreshDataSource();
			if (gv_Alert.Columns.Any((GridColumn t) => t.FieldName.Equals("RowCount")))
			{
				gv_Alert.Columns["RowCount"].DisplayFormat.FormatType = FormatType.Numeric;
				gv_Alert.Columns["RowCount"].DisplayFormat.FormatString = "{0:n" + GlobalData.QuantitiesFormat.CurrencyDecimalDigits + "}";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateMessagesList(TreeListNodes nodes)
	{
		try
		{
			foreach (TreeListNode node in nodes)
			{
				CreateMessagesForNode(node);
				CreateMessagesList(node.Nodes);
				if (GetNodeMailFolder(node) < 1024)
				{
					continue;
				}
				List<TreeListNode> list = new List<TreeListNode>();
				list.Add(node);
				GetAllChildren(node, list);
				List<Message> list2 = new List<Message>();
				foreach (TreeListNode item in list)
				{
					list2.AddRange((IEnumerable<Message>)item.GetValue(colData));
				}
				node.SetValue(colData, list2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal int GetNodeMailFolder(TreeListNode node)
	{
		try
		{
			return (int)node.GetValue(colFolder);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void GetAllChildren(TreeListNode node, List<TreeListNode> list)
	{
		try
		{
			if (!node.HasChildren)
			{
				return;
			}
			foreach (TreeListNode node2 in node.Nodes)
			{
				list.Add(node2);
				GetAllChildren(node2, list);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void treeList1_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		try
		{
			if (e.Column == colName)
			{
				string htmlTextColor = GetHtmlTextColor(tv_Alert.FocusedNode.Equals(e.Node));
				object value = e.Node.GetValue(colName);
				e.CellText = string.Format("<Color={1}>{0}", value, htmlTextColor);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateMessagesForNode(TreeListNode node)
	{
	}

	private static string GetHtmlTextColor(bool focused)
	{
		try
		{
			if (focused)
			{
				return ColorHelper.HtmlHighlightTextColor;
			}
			return AllowControlTextColor ? ColorHelper.HtmlControlTextColor : ColorHelper.HtmlControlTextColor2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	protected void LookAndFeelStyleChanged()
	{
		try
		{
			Color color = CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("Control");
			tv_Alert.Appearance.Empty.BackColor = color;
			tv_Alert.Appearance.Row.BackColor = color;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void tv_Alert_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Alert_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			grd_Data.DataSource = null;
			grd_Data.RefreshDataSource();
			gv_Data.Columns.Clear();
			int code = Function.ParseInt(gv_Alert.GetFocusedRowCellValue("Code"));
			gv_Alert.IsGroupRow(e.FocusedRowHandle);
			if (code > 0)
			{
				if (Function.ToString(gv_Alert.GetFocusedRowCellValue("WasRead")) == "N")
				{
					gv_Alert.SetFocusedRowCellValue("WasRead", "Y");
					await SaveReadMessageChanged(code, "Y");
				}
				WinCommons._OpenProcessing();
				DataSet dataSet;
				using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "AlertOverview"))
				{
					dataSet = await helper.GetAsync("GetAlertData", new { code });
				}
				if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
				{
					DataTable dataTable = dataSet.Tables[0];
					DataTable dataTable2 = dataSet.Tables[1];
					DataTable source = new DataTable();
					if (dataTable2.IsNotNull() && dataTable.IsNotNull() && dataTable.Columns.Count >= dataTable2.Rows.Count)
					{
						gv_Data.BeginUpdate();
						if (dataSet.Tables.Count >= 3)
						{
							source = dataSet.Tables[2];
						}
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							string fieldname = "";
							if (dataTable.Columns.Count >= i)
							{
								fieldname = Function.ToString(dataTable.Columns[i].ColumnName);
							}
							int loc = Function.ParseInt(dataTable2.Rows[i]["Location"]);
							GridColumnEx gridColumnEx = new GridColumnEx();
							gridColumnEx.Caption = Function.ToString(dataTable2.Rows[i]["ColName"]);
							gridColumnEx.VisibleIndex = loc;
							gridColumnEx.FieldName = fieldname;
							gridColumnEx.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
							gridColumnEx.Width = 75;
							gridColumnEx.OptionsColumn.ReadOnly = true;
							gridColumnEx.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
							gridColumnEx.Name = loc.ToString();
							GridColumnEx gridColumnEx2 = gridColumnEx;
							if (dataTable.Columns[i].DataType == typeof(DateTime) || dataTable.Columns[i].DataType == typeof(DateTime?))
							{
								gridColumnEx2.DisplayFormat.FormatType = FormatType.DateTime;
								string dateTimeFormat = GlobalData.DateTimeFormat;
								gridColumnEx2.DisplayFormat.FormatString = dateTimeFormat;
							}
							if (Function.ToString(dataTable2.Rows[i]["Link"]) == "Y")
							{
								RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
								repositoryItemButtonEdit.Buttons[0].IsLeft = true;
								repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
								repositoryItemButtonEdit.Buttons[0].ImageLocation = ImageLocation.MiddleCenter;
								repositoryItemButtonEdit.Buttons[0].Image = Resources.right;
								gridColumnEx2.ColumnEdit = repositoryItemButtonEdit;
								DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Location"]) == loc);
								string objtype = "";
								if (dataRow != null)
								{
									objtype = Function.ToString(dataRow["ObjType"]);
								}
								repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs events) =>
								{
									try
									{
										string text = Function.ToString(gv_Data.GetFocusedRowCellValue(fieldname));
										if (Function.ParseInt(objtype) != 0)
										{
											this.DrillDown((ApzObjectTypes)Function.ParseInt(objtype), (ApzObjectTypes)Function.ParseInt(objtype), text);
										}
										else
										{
											this.DrillDown(ApzObjectTypes.oUserDefinedObject, text, reserve: false, baseProduction: false, objtype);
										}
									}
									catch (Exception ex2)
									{
										Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
										WinCommons.ErrorLineNumber(ex2);
									}
								};
							}
							gv_Data.Columns.Add(gridColumnEx2);
						}
						gv_Data.EndUpdate();
						grd_Data.DataSource = dataTable;
						grd_Data.RefreshDataSource();
						gv_Data.BestFitColumns();
					}
					else
					{
						grd_Data.DataSource = dataTable;
						grd_Data.RefreshDataSource();
					}
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.NoDataFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task SaveReadMessageChanged(int code, string wasread)
	{
		try
		{
			Tuple<int, string, string> pr = new Tuple<int, string, string>(code, wasread, GlobalData.UserName);
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AlertOverview");
			await helper.PostAsync("SaveReadMessageChanged", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Alert_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Alert.GetFocusedRowCellValue("Code"));
			if (num == -1 && e.Button == MouseButtons.Left)
			{
				frm_WorkingProcess frm_WorkingProcess2 = new frm_WorkingProcess();
				frm_WorkingProcess2.MdiParent = MdiParent;
				frm_WorkingProcess2.Show();
			}
			if ((num == -1 || e.Column == gv_Alert.Columns["WasRead"]) && e.Button == MouseButtons.Left)
			{
				string text = Function.ToString(gv_Alert.GetFocusedRowCellValue("WasRead"));
				gv_Alert.SetFocusedRowCellValue("WasRead", (text == "Y") ? "N" : "Y");
				await SaveReadMessageChanged(num, (text == "Y") ? "N" : "Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Alert_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (e.FocusedRowHandle == -2147483647 || e.FocusedRowHandle == -2147483646)
			{
				return;
			}
			GridView gridView = (GridView)sender;
			if (e.FocusedRowHandle >= 0)
			{
				return;
			}
			int num;
			if (e.PrevFocusedRowHandle == int.MinValue)
			{
				num = 0;
			}
			else if (Control.MouseButtons == MouseButtons.Left || Control.MouseButtons == MouseButtons.Right)
			{
				num = e.PrevFocusedRowHandle;
			}
			else
			{
				int visibleIndex = gridView.GetVisibleIndex(e.PrevFocusedRowHandle);
				int visibleIndex2 = gridView.GetVisibleIndex(e.FocusedRowHandle);
				num = ((visibleIndex <= visibleIndex2) ? (e.PrevFocusedRowHandle + 1) : (e.PrevFocusedRowHandle - 1));
				if (num < 0)
				{
					num = 0;
				}
				if (num >= gridView.DataRowCount)
				{
					num = gridView.DataRowCount - 1;
				}
			}
			gridView.FocusedRowHandle = ((num >= 0) ? num : 0);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_AlertOverview));
		this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
		this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
		this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
		this.tv_Alert = new DevExpress.XtraTreeList.TreeList();
		this.colName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colType = new Apzon.Controls.Tree.TreeListColumnEx();
		this.colFolder = new Apzon.Controls.Tree.TreeListColumnEx();
		this.colImageIndex = new Apzon.Controls.Tree.TreeListColumnEx();
		this.colData = new Apzon.Controls.Tree.TreeListColumnEx();
		this.icFolders = new DevExpress.Utils.ImageCollection(this.components);
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Alert = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Alert = new Apzon.Controls.Grid.GridViewEx();
		this.gcPriority = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ImagePriority = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.icEditors = new DevExpress.Utils.ImageCollection(this.components);
		this.gcIcon = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ImageIcon = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.gcAttachment = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ImageAttachment = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.gcSubject = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRead = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemImageComboBox4 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.ilColumns = new System.Windows.Forms.ImageList(this.components);
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.navBarControl1).BeginInit();
		this.navBarControl1.SuspendLayout();
		this.navBarGroupControlContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Alert).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.icFolders).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Alert).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Alert).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImagePriority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.icEditors).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImageIcon).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImageAttachment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		base.SuspendLayout();
		this.navBarControl1.ActiveGroup = this.navBarGroup1;
		this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
		this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Left;
		this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[1] { this.navBarGroup1 });
		this.navBarControl1.Location = new System.Drawing.Point(0, 0);
		this.navBarControl1.Name = "navBarControl1";
		this.navBarControl1.OptionsNavPane.ExpandButtonMode = DevExpress.Utils.Controls.ExpandButtonMode.Inverted;
		this.navBarControl1.OptionsNavPane.ExpandedWidth = 139;
		this.navBarControl1.OptionsNavPane.ShowHeaderText = false;
		this.navBarControl1.OptionsNavPane.ShowOverflowButton = false;
		this.navBarControl1.OptionsNavPane.ShowOverflowPanel = false;
		this.navBarControl1.OptionsNavPane.ShowSplitter = false;
		this.navBarControl1.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane;
		this.navBarControl1.ShowIcons = DevExpress.Utils.DefaultBoolean.False;
		this.navBarControl1.Size = new System.Drawing.Size(139, 397);
		this.navBarControl1.StoreDefaultPaintStyleName = true;
		this.navBarControl1.TabIndex = 0;
		this.navBarControl1.Text = "navBarControl1";
		this.navBarGroup1.Caption = "Alert";
		this.navBarGroup1.ControlContainer = this.navBarGroupControlContainer1;
		this.navBarGroup1.Expanded = true;
		this.navBarGroup1.GroupClientHeight = 496;
		this.navBarGroup1.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
		this.navBarGroup1.Name = "navBarGroup1";
		this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
		this.navBarGroupControlContainer1.Controls.Add(this.tv_Alert);
		this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
		this.navBarGroupControlContainer1.Size = new System.Drawing.Size(139, 365);
		this.navBarGroupControlContainer1.TabIndex = 0;
		this.tv_Alert.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tv_Alert.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[5] { this.colName, this.colType, this.colFolder, this.colImageIndex, this.colData });
		this.tv_Alert.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Alert.Location = new System.Drawing.Point(0, 0);
		this.tv_Alert.Name = "tv_Alert";
		this.tv_Alert.OptionsView.ShowColumns = false;
		this.tv_Alert.OptionsView.ShowHorzLines = false;
		this.tv_Alert.OptionsView.ShowIndentAsRowStyle = true;
		this.tv_Alert.OptionsView.ShowIndicator = false;
		this.tv_Alert.OptionsView.ShowVertLines = false;
		this.tv_Alert.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemButtonEdit1 });
		this.tv_Alert.SelectImageList = this.icFolders;
		this.tv_Alert.Size = new System.Drawing.Size(139, 365);
		this.tv_Alert.TabIndex = 0;
		this.tv_Alert.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Alert_FocusedNodeChanged);
		this.tv_Alert.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList1_CustomDrawNodeCell);
		this.colName.Caption = "Name";
		this.colName.ColumnEdit = this.repositoryItemButtonEdit1;
		this.colName.CurrencyDateField = null;
		this.colName.CurrencyField = null;
		this.colName.CurrencyFixed = null;
		this.colName.CurrencyFixedRate = 0.0;
		this.colName.CurrencyRateField = null;
		this.colName.FieldName = "Name";
		this.colName.MinWidth = 34;
		this.colName.Name = "colName";
		this.colName.OptionsColumn.AllowFocus = false;
		this.colName.Visible = true;
		this.colName.VisibleIndex = 0;
		this.repositoryItemButtonEdit1.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.colType.CurrencyDateField = null;
		this.colType.CurrencyField = null;
		this.colType.CurrencyFixed = null;
		this.colType.CurrencyFixedRate = 0.0;
		this.colType.CurrencyRateField = null;
		this.colType.Name = "colType";
		this.colFolder.CurrencyDateField = null;
		this.colFolder.CurrencyField = null;
		this.colFolder.CurrencyFixed = null;
		this.colFolder.CurrencyFixedRate = 0.0;
		this.colFolder.CurrencyRateField = null;
		this.colFolder.Name = "colFolder";
		this.colImageIndex.Caption = "ImageIndex";
		this.colImageIndex.CurrencyDateField = null;
		this.colImageIndex.CurrencyField = null;
		this.colImageIndex.CurrencyFixed = null;
		this.colImageIndex.CurrencyFixedRate = 0.0;
		this.colImageIndex.CurrencyRateField = null;
		this.colImageIndex.FieldName = "ImageIndex";
		this.colImageIndex.Name = "colImageIndex";
		this.colData.Caption = "colData";
		this.colData.CurrencyDateField = null;
		this.colData.CurrencyField = null;
		this.colData.CurrencyFixed = null;
		this.colData.CurrencyFixedRate = 0.0;
		this.colData.CurrencyRateField = null;
		this.colData.FieldName = "colData";
		this.colData.Name = "colData";
		this.icFolders.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("icFolders.ImageStream");
		this.icFolders.Images.SetKeyName(0, "Inbox_16x16.png");
		this.icFolders.Images.SetKeyName(1, "513-20.png");
		this.icFolders.Images.SetKeyName(2, "SentItems_16x16.png");
		this.icFolders.Images.SetKeyName(3, "Drafts_16x16.png");
		this.icFolders.Images.SetKeyName(4, "DeletedItems_16x16.png");
		this.icFolders.Images.SetKeyName(5, "Customer.png");
		this.icFolders.Images.SetKeyName(6, "Announcements.png");
		this.icFolders.Images.SetKeyName(7, "ASP.png");
		this.icFolders.Images.SetKeyName(8, "IDE.png");
		this.icFolders.Images.SetKeyName(9, "Frameworks.png");
		this.icFolders.Images.SetKeyName(10, "WinForms_16x16.png");
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(139, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Alert);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Data);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.ScrollBarSmallChange = 1;
		this.splitContainerControl1.Size = new System.Drawing.Size(712, 397);
		this.splitContainerControl1.SplitterPosition = 360;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Alert.AllowPasteExcel = true;
		this.grd_Alert.ClipboardData = null;
		this.grd_Alert.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Alert.Location = new System.Drawing.Point(0, 0);
		this.grd_Alert.MainView = this.gv_Alert;
		this.grd_Alert.Name = "grd_Alert";
		this.grd_Alert.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_ImagePriority, this.rep_ImageIcon, this.rep_ImageAttachment, this.repositoryItemImageComboBox4 });
		this.grd_Alert.Size = new System.Drawing.Size(360, 397);
		this.grd_Alert.TabIndex = 0;
		this.grd_Alert.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Alert });
		this.gv_Alert.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Alert.ColumnKey = null;
		this.gv_Alert.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcPriority, this.gcIcon, this.gcAttachment, this.gcSubject, this.gcFrom, this.gcDate, this.gcRead, this.gridColumn1 });
		this.gv_Alert.DisableFormSetting = false;
		this.gv_Alert.DisplayException = false;
		this.gv_Alert.EditColumn = null;
		this.gv_Alert.FieldCurrency = null;
		this.gv_Alert.FieldRate = null;
		this.gv_Alert.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Alert.FormId = "oAlertOverview";
		this.gv_Alert.GridControl = this.grd_Alert;
		this.gv_Alert.GroupCount = 1;
		this.gv_Alert.GroupFormat = "[#image]{1} {2}";
		this.gv_Alert.IgnoreFilter = false;
		this.gv_Alert.Images = this.ilColumns;
		this.gv_Alert.IndexEditRow = -1;
		this.gv_Alert.IndicatorWidth = 35;
		this.gv_Alert.IsApzTable = false;
		this.gv_Alert.IsInsertNewRow = false;
		this.gv_Alert.IsLoadingData = false;
		this.gv_Alert.IsMappingContextAction = false;
		this.gv_Alert.ItemId = "OALERT";
		this.gv_Alert.LstColumnAllowBatch = null;
		this.gv_Alert.LstColumnNotAllowBatch = null;
		this.gv_Alert.LstColumnNotEditable = null;
		this.gv_Alert.LstColumNotSetting = null;
		this.gv_Alert.Name = "gv_Alert";
		this.gv_Alert.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Alert.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Alert.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Alert.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Alert.OptionsPrint.PrintHorzLines = false;
		this.gv_Alert.OptionsPrint.PrintVertLines = false;
		this.gv_Alert.OptionsSelection.MultiSelect = true;
		this.gv_Alert.OptionsView.ColumnAutoWidth = false;
		this.gv_Alert.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office2003;
		this.gv_Alert.OptionsView.ShowGroupedColumns = true;
		this.gv_Alert.OptionsView.ShowIndicator = false;
		this.gv_Alert.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Alert.PreviewFieldName = "PlainText";
		this.gv_Alert.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcDate, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.gv_Alert.UseBatchMod = false;
		this.gv_Alert.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_Alert_RowCellClick);
		this.gv_Alert.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Alert_FocusedRowChanged);
		this.gv_Alert.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Alert_FocusedRowObjectChanged);
		this.gcPriority.Caption = " Priority";
		this.gcPriority.ColumnEdit = this.rep_ImagePriority;
		this.gcPriority.CurrencyDateField = null;
		this.gcPriority.CurrencyField = null;
		this.gcPriority.CurrencyFixed = null;
		this.gcPriority.CurrencyFixedRate = 0.0;
		this.gcPriority.CurrencyRateField = null;
		this.gcPriority.FieldName = "Priority";
		this.gcPriority.ImageOptions.ImageIndex = 0;
		this.gcPriority.Name = "gcPriority";
		this.gcPriority.OptionsColumn.AllowFocus = false;
		this.gcPriority.OptionsColumn.AllowSize = false;
		this.gcPriority.OptionsColumn.FixedWidth = true;
		this.gcPriority.OptionsColumn.ReadOnly = true;
		this.gcPriority.OptionsColumn.ShowCaption = false;
		this.gcPriority.PrimaryKey = true;
		this.gcPriority.ToolTip = "Importance";
		this.gcPriority.UnitField = null;
		this.gcPriority.Visible = true;
		this.gcPriority.VisibleIndex = 0;
		this.gcPriority.Width = 28;
		this.rep_ImagePriority.AutoHeight = false;
		this.rep_ImagePriority.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.rep_ImagePriority.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[3]
		{
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Low", "0", 0),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Medium", "1", -1),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("High", "2", 1)
		});
		this.rep_ImagePriority.Name = "rep_ImagePriority";
		this.rep_ImagePriority.SmallImages = this.icEditors;
		this.icEditors.ImageSize = new System.Drawing.Size(13, 13);
		this.icEditors.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("icEditors.ImageStream");
		this.icEditors.Images.SetKeyName(0, "Low16x16.png");
		this.icEditors.Images.SetKeyName(1, "High16x16.png");
		this.icEditors.Images.SetKeyName(2, "letter.png");
		this.icEditors.Images.SetKeyName(3, "email.png");
		this.icEditors.Images.SetKeyName(4, "AttachmentObject_16x16.png");
		this.gcIcon.Caption = " Read";
		this.gcIcon.ColumnEdit = this.rep_ImageIcon;
		this.gcIcon.CurrencyDateField = null;
		this.gcIcon.CurrencyField = null;
		this.gcIcon.CurrencyFixed = null;
		this.gcIcon.CurrencyFixedRate = 0.0;
		this.gcIcon.CurrencyRateField = null;
		this.gcIcon.FieldName = "WasRead";
		this.gcIcon.ImageOptions.ImageIndex = 1;
		this.gcIcon.Name = "gcIcon";
		this.gcIcon.OptionsColumn.AllowFocus = false;
		this.gcIcon.OptionsColumn.AllowSize = false;
		this.gcIcon.OptionsColumn.FixedWidth = true;
		this.gcIcon.OptionsColumn.ReadOnly = true;
		this.gcIcon.OptionsColumn.ShowCaption = false;
		this.gcIcon.PrimaryKey = false;
		this.gcIcon.UnitField = null;
		this.gcIcon.Visible = true;
		this.gcIcon.VisibleIndex = 1;
		this.gcIcon.Width = 24;
		this.rep_ImageIcon.AutoHeight = false;
		this.rep_ImageIcon.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.rep_ImageIcon.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[2]
		{
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Read", "Y", 2),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Unread", "N", 3)
		});
		this.rep_ImageIcon.Name = "rep_ImageIcon";
		this.rep_ImageIcon.SmallImages = this.icEditors;
		this.gcAttachment.Caption = " Attachment";
		this.gcAttachment.ColumnEdit = this.rep_ImageAttachment;
		this.gcAttachment.CurrencyDateField = null;
		this.gcAttachment.CurrencyField = null;
		this.gcAttachment.CurrencyFixed = null;
		this.gcAttachment.CurrencyFixedRate = 0.0;
		this.gcAttachment.CurrencyRateField = null;
		this.gcAttachment.FieldName = "Attachment";
		this.gcAttachment.ImageOptions.ImageIndex = 2;
		this.gcAttachment.Name = "gcAttachment";
		this.gcAttachment.OptionsColumn.AllowFocus = false;
		this.gcAttachment.OptionsColumn.AllowSize = false;
		this.gcAttachment.OptionsColumn.FixedWidth = true;
		this.gcAttachment.OptionsColumn.ReadOnly = true;
		this.gcAttachment.OptionsColumn.ShowCaption = false;
		this.gcAttachment.PrimaryKey = false;
		this.gcAttachment.ToolTip = "Attachment";
		this.gcAttachment.UnitField = null;
		this.gcAttachment.Visible = true;
		this.gcAttachment.VisibleIndex = 2;
		this.gcAttachment.Width = 23;
		this.rep_ImageAttachment.AutoHeight = false;
		this.rep_ImageAttachment.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.rep_ImageAttachment.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[2]
		{
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Attachment", "1", 4),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Empty", "0", -1)
		});
		this.rep_ImageAttachment.Name = "rep_ImageAttachment";
		this.rep_ImageAttachment.SmallImages = this.icEditors;
		this.gcSubject.Caption = "Subject";
		this.gcSubject.CurrencyDateField = null;
		this.gcSubject.CurrencyField = null;
		this.gcSubject.CurrencyFixed = null;
		this.gcSubject.CurrencyFixedRate = 0.0;
		this.gcSubject.CurrencyRateField = null;
		this.gcSubject.FieldName = "Name";
		this.gcSubject.Name = "gcSubject";
		this.gcSubject.OptionsColumn.AllowEdit = false;
		this.gcSubject.OptionsColumn.ReadOnly = true;
		this.gcSubject.PrimaryKey = false;
		this.gcSubject.UnitField = null;
		this.gcSubject.Visible = true;
		this.gcSubject.VisibleIndex = 3;
		this.gcSubject.Width = 270;
		this.gcFrom.Caption = "From";
		this.gcFrom.CurrencyDateField = null;
		this.gcFrom.CurrencyField = null;
		this.gcFrom.CurrencyFixed = null;
		this.gcFrom.CurrencyFixedRate = 0.0;
		this.gcFrom.CurrencyRateField = null;
		this.gcFrom.FieldName = "FromUser";
		this.gcFrom.Name = "gcFrom";
		this.gcFrom.OptionsColumn.ReadOnly = true;
		this.gcFrom.PrimaryKey = false;
		this.gcFrom.UnitField = null;
		this.gcFrom.Width = 54;
		this.gcDate.Caption = "Received";
		this.gcDate.CurrencyDateField = null;
		this.gcDate.CurrencyField = null;
		this.gcDate.CurrencyFixed = null;
		this.gcDate.CurrencyFixedRate = 0.0;
		this.gcDate.CurrencyRateField = null;
		this.gcDate.DisplayFormat.FormatString = "MM.dd.yyyy HH:mm";
		this.gcDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDate.FieldName = "Received";
		this.gcDate.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gcDate.Name = "gcDate";
		this.gcDate.OptionsColumn.ReadOnly = true;
		this.gcDate.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gcDate.PrimaryKey = false;
		this.gcDate.UnitField = null;
		this.gcRead.Caption = "Read";
		this.gcRead.ColumnEdit = this.repositoryItemImageComboBox4;
		this.gcRead.CurrencyDateField = null;
		this.gcRead.CurrencyField = null;
		this.gcRead.CurrencyFixed = null;
		this.gcRead.CurrencyFixedRate = 0.0;
		this.gcRead.CurrencyRateField = null;
		this.gcRead.FieldName = "Read";
		this.gcRead.Name = "gcRead";
		this.gcRead.OptionsColumn.ReadOnly = true;
		this.gcRead.PrimaryKey = false;
		this.gcRead.UnitField = null;
		this.repositoryItemImageComboBox4.AutoHeight = false;
		this.repositoryItemImageComboBox4.Name = "repositoryItemImageComboBox4";
		this.repositoryItemImageComboBox4.SmallImages = this.icEditors;
		this.gridColumn1.Caption = "Total Record";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "RowCount";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 4;
		this.ilColumns.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("ilColumns.ImageStream");
		this.ilColumns.TransparentColor = System.Drawing.Color.Transparent;
		this.ilColumns.Images.SetKeyName(0, "Important_16x16.png");
		this.ilColumns.Images.SetKeyName(1, "MemoStyle.png");
		this.ilColumns.Images.SetKeyName(2, "icon-attachment-icon.png");
		this.ilColumns.Images.SetKeyName(3, "1481206593_eye.png");
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Data.Location = new System.Drawing.Point(0, 0);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(342, 397);
		this.grd_Data.TabIndex = 0;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oAlertOverview";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.UseBatchMod = false;
		base.Appearance.BackColor = System.Drawing.SystemColors.Control;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(851, 397);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.navBarControl1);
		base.FormId = "oAlertOverview";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AlertOverview";
		base.ShowInTaskbar = false;
		this.Text = "Alert Overview";
		base.Load += new System.EventHandler(this.frm_AlertOverview_Load);
		((System.ComponentModel.ISupportInitialize)this.navBarControl1).EndInit();
		this.navBarControl1.ResumeLayout(false);
		this.navBarGroupControlContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Alert).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.icFolders).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Alert).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Alert).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImagePriority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.icEditors).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImageIcon).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ImageAttachment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemImageComboBox4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		base.ResumeLayout(false);
	}
}
