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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Widget;

public class frm_Widget : XtraFormEx
{
	private const string TableHeaderName = "APZ_OWID";

	private const string TableDocumentsName = "APZ_WID1";

	private const string TablePagesName = "APZ_WID2";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Action AfterSave;

	private IContainer components;

	private Document document_SalesProcessModel;

	private TileBar tileBar;

	private NavigationFrame navFrame;

	private NavigationPage navigationPage1;

	private UcWidgetViewer ucWidgetViewer1;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx simpleButton2;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private DockManager dockManager1;

	private DockPanel dockPanel1;

	private ControlContainer dockPanel1_Container;

	private SimpleButtonEx btn_AddGroup;

	private TextEditEx txt_Group;

	private TextEditEx txt_Page;

	private ColorEdit colorEdit_BackColor;

	private SimpleButtonEx btn_AddPage;

	private SimpleButtonEx btn_ChooseItems;

	private GroupControl groupControl1;

	private LabelControl labelControl4;

	private TextEditEx txt_Caption;

	private CheckEditEx chk_Active;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private LabelControl labelControl5;

	private ButtonEditCFL btn_FatherMenu;

	private SimpleButtonEx btn_RemovePage;

	private SimpleButtonEx btn_RemoveGroup;

	public int WidgetEntry { get; set; }

	public bool ShowProperties { get; set; }

	private DataTable DtDocuments { get; set; }

	private DataTable DtHeader { get; set; }

	private DataTable DtPages { get; set; }

	private DataTable DtChooseItem { get; set; }

	private bool _isLoading { get; set; }

	public frm_Widget()
	{
		InitializeComponent();
		try
		{
			ucWidgetViewer1.WidgetView.QueryControl += widget_QueryControl;
			ucWidgetViewer1.WidgetView.Documents.Changed += Documents_Changed;
			ucWidgetViewer1.WidgetView.EndStackGroupDragging += widgetView1_EndStackGroupDragging;
			ucWidgetViewer1.WidgetView.EndSizing += widgetView1_EndSizing;
			ucWidgetViewer1.WidgetView.BeginSizing += widgetView1_BeginSizing;
			ucWidgetViewer1.WidgetView.EndDragging += widgetView1_EndDragging;
			ucWidgetViewer1.WidgetView.DocumentClosed += widgetView1_DocumentClosed;
			tileBar.SelectedItemChanged += TileBar_SelectedItemChanged;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void widgetView1_DocumentClosed(object sender, DocumentEventArgs e)
	{
		try
		{
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				XtraFormEx.EnableDirectionalControl(Enums.DirectionalControl.WidgetSave, isEnable: true);
				btn_OK.Text = Resources.ButtonAdd;
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				XtraFormEx.EnableDirectionalControl(Enums.DirectionalControl.WidgetSave, isEnable: true);
				btn_OK.Text = Resources.ButtonUpdate;
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				XtraFormEx.EnableDirectionalControl(Enums.DirectionalControl.WidgetSave, isEnable: false);
				btn_OK.Text = Resources.ButtonOK;
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
				break;
			case Enums.FormViewModeType.Busy:
				XtraFormEx.EnableDirectionalControl(Enums.DirectionalControl.WidgetSave, isEnable: false);
				btn_OK.Enabled = false;
				btn_Cancel.Enabled = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TileBar_SelectedItemChanged(object sender, TileItemEventArgs e)
	{
		try
		{
			if (e.Item != null)
			{
				txt_Group.EditValue = e.Item.Group.Text;
				txt_Page.EditValue = e.Item.Text;
				colorEdit_BackColor.Color = e.Item.AppearanceItem.Normal.BackColor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeViewModeToUpdate()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void widgetView1_BeginSizing(object sender, LayoutBeginSizingEventArgs e)
	{
		ChangeViewModeToUpdate();
	}

	private void widgetView1_EndDragging(object sender, EventArgs e)
	{
		ChangeViewModeToUpdate();
	}

	private void widgetView1_EndStackGroupDragging(object sender, StackGroupDraggingEventArgs e)
	{
		ChangeViewModeToUpdate();
	}

	private void widgetView1_EndSizing(object sender, LayoutEndSizingEventArgs e)
	{
		ChangeViewModeToUpdate();
	}

	private void Documents_Changed(object sender, EventArgs e)
	{
		ChangeViewModeToUpdate();
	}

	private void widget_QueryControl(object sender, DevExpress.XtraBars.Docking2010.Views.QueryControlEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (!string.IsNullOrEmpty(e.Document.ControlTypeName))
			{
				if (Function.ParseInt(e.Document.ControlTypeName) == 7000030)
				{
					UcDashboardViewer control = new UcDashboardViewer(Function.ParseInt(e.Document.ControlName));
					e.Control = control;
				}
				else if (Type.GetType(e.Document.ControlTypeName) != null && Activator.CreateInstance(Type.GetType(e.Document.ControlTypeName)) is Control control2)
				{
					e.Control = control2;
				}
				if (e.Document is Document)
				{
					if (((Document)e.Document).Width == 0)
					{
						((Document)e.Document).Width = 400;
					}
					if (((Document)e.Document).Height == 0)
					{
						((Document)e.Document).Height = 400;
					}
				}
			}
			else
			{
				e.Control = new System.Windows.Forms.UserControl();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			e.Control = new System.Windows.Forms.UserControl();
		}
		WinCommons._CloseProcessing();
	}

	public void WIDGET_EDIT()
	{
		try
		{
			frm_WidgetAddItem frm_WidgetAddItem2 = new frm_WidgetAddItem();
			DataTable dataTable = DtChooseItem.Clone();
			foreach (BaseDocument item in (IEnumerable<BaseDocument>)ucWidgetViewer1.WidgetView.Documents)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ObjCode"] = Function.ToString(item.ControlName);
				dataRow["Name"] = item.Caption;
				dataRow["ObjType"] = Function.ToString(item.ControlTypeName);
				dataTable.Rows.Add(dataRow);
			}
			frm_WidgetAddItem2.WidgetSource = dataTable;
			frm_WidgetAddItem2.UpdateDocument = (Action<string, string, bool>)Delegate.Combine(frm_WidgetAddItem2.UpdateDocument, new Action<string, string, bool>(UpdateDocument));
			frm_WidgetAddItem2.MdiParent = MdiParent;
			frm_WidgetAddItem2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UpdateDocument(string objtype, string objcode, bool visible)
	{
		try
		{
			if (navFrame.SelectedPage is NavigationPage { Controls: var controls } && controls.Count > 0 && controls[0] is UcWidgetViewer ucWidgetViewer)
			{
				BaseDocument baseDocument = ucWidgetViewer.WidgetView.Documents.FirstOrDefault((BaseDocument t) => t.ControlTypeName == objtype && t.ControlName == objcode);
				if (baseDocument != null && baseDocument.Control != null && !visible)
				{
					ucWidgetViewer.WidgetView.RemoveDocument(baseDocument.Control);
					ChangeViewModeToUpdate();
				}
				if ((baseDocument == null) & visible)
				{
					baseDocument = new Document
					{
						ControlName = objcode,
						ControlTypeName = objtype
					};
					ucWidgetViewer.WidgetView.Documents.Add(baseDocument);
					ChangeViewModeToUpdate();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void WIDGET_TABLE()
	{
		try
		{
			ucWidgetViewer1.WidgetView.LayoutMode = LayoutMode.TableLayout;
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void WIDGET_STACK()
	{
		try
		{
			ucWidgetViewer1.WidgetView.LayoutMode = LayoutMode.StackLayout;
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void WIDGET_FLOW()
	{
		try
		{
			ucWidgetViewer1.WidgetView.LayoutMode = LayoutMode.FlowLayout;
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void WIDGET_FREE()
	{
		try
		{
			ucWidgetViewer1.WidgetView.LayoutMode = LayoutMode.FreeLayout;
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void WIDGET_SAVE()
	{
		try
		{
			DataTable dataTable = DtDocuments.Clone();
			dataTable.TableName = "APZ_WID1";
			foreach (Document item in (IEnumerable<BaseDocument>)ucWidgetViewer1.WidgetView.Documents)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = WidgetEntry;
				dataRow["ObjType"] = item.ControlTypeName;
				dataRow["ObjCode"] = item.ControlName;
				dataRow["Caption"] = item.Caption;
				dataRow["ColumnIndex"] = item.ColumnIndex;
				dataRow["ColumnSpan"] = item.ColumnSpan;
				dataRow["Height"] = item.Height;
				dataRow["Width"] = item.Width;
				dataRow["RowIndex"] = item.RowIndex;
				dataRow["RowSpan"] = item.RowSpan;
				dataRow["ToolTip"] = item.ToolTip;
				dataRow["FreeLayoutHeight"] = item.FreeLayoutHeight.UnitValue;
				dataRow["FreeLayoutWidth"] = item.FreeLayoutWidth.UnitValue;
				_ = ucWidgetViewer1.WidgetView.LayoutMode;
				dataTable.Rows.Add(dataRow);
			}
			if (DtHeader != null)
			{
				DataTable dataTable2 = DtHeader.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["AbsEntry"] = WidgetEntry;
				dataRow2["Caption"] = Text;
				dataRow2["LayoutMode"] = (int)ucWidgetViewer1.WidgetView.LayoutMode;
				dataTable2.TableName = "APZ_OWID";
				dataTable2.Rows.Add(dataRow2);
				DataSet dataSet = new DataSet();
				dataSet.Tables.AddRange(new DataTable[2]
				{
					dataTable2.Copy(),
					dataTable.Copy()
				});
				HttpResult httpResult = await _httpClient.Widget.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CanNotGetDataHeader, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_SalesWidget_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			dockPanel1.Visibility = ((!ShowProperties) ? DockVisibility.Hidden : DockVisibility.Visible);
			DtDocuments = await _httpClient.Widget.GetDocument(WidgetEntry);
			DtHeader = await _httpClient.Widget.GetHeader(WidgetEntry);
			DtPages = await _httpClient.Widget.GetPages(WidgetEntry);
			navFrame.SelectedPage = navigationPage1;
			btn_FatherMenu.Properties.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			btn_FatherMenu.Properties.DisplayMember = "Name";
			btn_FatherMenu.Properties.ValueMember = "Code";
			btn_FatherMenu.Properties.StoredProcedure = "Apz_Cfl_GetFatherMenu";
			btn_FatherMenu.Properties.RefreshDataSource();
			btn_FatherMenu.Properties.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL properties = btn_FatherMenu.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm._tableType = "2";
			}));
			btn_FatherMenu.ReadOnly = true;
			if (WidgetEntry == 0)
			{
				FormMode = Enums.FormViewModeType.Add;
				AddGroup("Untitled group", "Untitled page");
			}
			if (DtHeader.IsNotNull())
			{
				FormMode = Enums.FormViewModeType.View;
				_isLoading = true;
				txt_Caption.EditValue = Function.ToString(DtHeader.Rows[0]["Caption"]);
				btn_FatherMenu.EditValue = Function.ToString(DtHeader.Rows[0]["FatherMenu"]);
				if (DtPages.IsNotNull())
				{
					foreach (var item in from t in DtPages.AsEnumerable()
						group t by Function.ToString(t["GroupText"]) into t
						select new
						{
							Key = t.Key,
							Items = t
						})
					{
						TileBarGroup tileBarGroup = new TileBarGroup();
						tileBarGroup.Text = Function.ToString(item.Key);
						foreach (DataRow item2 in item.Items)
						{
							int pageId = Function.ParseInt(item2["PageId"]);
							TileBarItem tileBarItem = new TileBarItem();
							tileBarItem.AppearanceItem.Normal.BackColor = Color.FromArgb(Function.ParseInt(item2["BackColor"]));
							tileBarItem.AppearanceItem.Normal.Options.UseBackColor = true;
							tileBarItem.DropDownOptions.BeakColor = Color.Empty;
							TileItemElement tileItemElement = new TileItemElement();
							tileItemElement.Text = Function.ToString(item2["PageText"]);
							tileBarItem.Elements.Add(tileItemElement);
							tileBarItem.Id = pageId;
							tileBarItem.ItemSize = TileBarItemSize.Wide;
							tileBarGroup.Items.Add(tileBarItem);
							tileBar.Groups.Add(tileBarGroup);
							NavigationPage navigationPage = new NavigationPage();
							navigationPage.Name = pageId.ToString();
							EnumerableRowCollection<DataRow> enumerableRowCollection = from t in DtDocuments.AsEnumerable()
								where Function.ParseInt(t["PageId"]) == pageId
								select t;
							UcWidgetViewer ucWidgetViewer = new UcWidgetViewer();
							ucWidgetViewer.DocumentClosed = (Action<DocumentEventArgs>)Delegate.Combine(ucWidgetViewer.DocumentClosed, (Action<DocumentEventArgs>)((DocumentEventArgs ev) =>
							{
								try
								{
									ChangeViewModeToUpdate();
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", ex2);
									WinCommons.ErrorLineNumber(ex2);
								}
							}));
							ucWidgetViewer.Dock = DockStyle.Fill;
							ucWidgetViewer.Location = new Point(0, 0);
							ucWidgetViewer.Size = new Size(971, 630);
							ucWidgetViewer.TabIndex = 0;
							ucWidgetViewer.WidgetEntry = WidgetEntry;
							foreach (DataRow item3 in enumerableRowCollection)
							{
								string controlName = Function.ToString(item3["ObjCode"]);
								string controlTypeName = Function.ToString(item3["ObjType"]);
								Document document = new Document
								{
									ControlName = controlName,
									ControlTypeName = controlTypeName
								};
								document.Caption = Function.ToString(item3["Caption"]);
								document.ColumnIndex = Function.ParseInt(item3["ColumnIndex"]);
								document.ColumnSpan = Function.ParseInt(item3["ColumnSpan"]);
								document.Height = Function.ParseInt(item3["Height"]);
								document.Width = Function.ParseInt(item3["Width"]);
								document.RowIndex = Function.ParseInt(item3["RowIndex"]);
								document.RowSpan = Function.ParseInt(item3["RowSpan"]);
								document.ToolTip = Function.ToString(item3["ToolTip"]);
								document.FreeLayoutHeight.UnitType = LengthUnitType.Pixel;
								document.FreeLayoutHeight.UnitValue = Function.ParseInt(item3["FreeLayoutHeight"]);
								document.FreeLayoutWidth.UnitType = LengthUnitType.Pixel;
								document.FreeLayoutWidth.UnitValue = Function.ParseInt(item3["FreeLayoutWidth"]);
								ucWidgetViewer.WidgetView.Documents.Add(document);
							}
							navigationPage.Controls.Add(ucWidgetViewer);
							navFrame.Controls.Add(navigationPage);
							navFrame.Pages.Add(navigationPage);
						}
					}
				}
				SelectFirstItem();
				txt_Caption.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void SelectFirstItem()
	{
		if (tileBar.Groups.FirstOrDefault()?.Items.FirstOrDefault() is TileItem selectedItem)
		{
			tileBar.SelectedItem = selectedItem;
		}
	}

	private void btn_AddGroup_Click(object sender, EventArgs e)
	{
		try
		{
			AddGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddGroup(string groupText = "", string firstPageText = "")
	{
		int num = navFrame.Pages.Select((NavigationPageBase t) => Function.ParseInt(t.Name)).Max();
		TileBarGroup tileBarGroup = new TileBarGroup();
		tileBarGroup.Text = groupText;
		TileBarItem tileBarItem = new TileBarItem();
		tileBarItem.AppearanceItem.Normal.BackColor = Color.Chocolate;
		tileBarItem.AppearanceItem.Normal.Options.UseBackColor = true;
		tileBarItem.DropDownOptions.BeakColor = Color.Empty;
		TileItemElement tileItemElement = new TileItemElement();
		tileItemElement.Text = firstPageText;
		tileBarItem.Elements.Add(tileItemElement);
		tileBarItem.Id = num + 1;
		tileBarItem.ItemSize = TileBarItemSize.Wide;
		tileBarGroup.Items.Add(tileBarItem);
		tileBar.Groups.Add(tileBarGroup);
		tileBar.SelectedItem = tileBarItem;
		UcWidgetViewer ucWidgetViewer = new UcWidgetViewer();
		ucWidgetViewer.Dock = DockStyle.Fill;
		ucWidgetViewer.Location = new Point(0, 0);
		ucWidgetViewer.Size = new Size(971, 630);
		ucWidgetViewer.TabIndex = 0;
		ucWidgetViewer.WidgetEntry = -1;
		NavigationPage navigationPage = new NavigationPage();
		navigationPage.Name = (num + 1).ToString();
		navigationPage.Controls.Add(ucWidgetViewer);
		navFrame.Controls.Add(navigationPage);
		navFrame.Pages.Add(navigationPage);
		navFrame.SelectedPage = navigationPage;
		txt_Group.Focus();
	}

	private void tileBar_ItemClick(object sender, TileItemEventArgs e)
	{
	}

	private void txt_Group_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (tileBar.SelectedItem != null && tileBar.SelectedItem.Group != null)
			{
				tileBar.SelectedItem.Group.Text = txt_Group.Text;
			}
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Page_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (tileBar.SelectedItem != null)
			{
				tileBar.SelectedItem.Text = txt_Page.Text;
			}
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void colorEdit_BackColor_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (tileBar.SelectedItem != null)
			{
				tileBar.SelectedItem.AppearanceItem.Normal.BackColor = colorEdit_BackColor.Color;
			}
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AddPage_Click(object sender, EventArgs e)
	{
		try
		{
			if (tileBar.SelectedItem != null && tileBar.SelectedItem.Group != null)
			{
				int num = navFrame.Pages.Select((NavigationPageBase t) => Function.ParseInt(t.Name)).Max();
				TileBarItem tileBarItem = new TileBarItem();
				tileBarItem.Id = num + 1;
				tileBarItem.AppearanceItem.Normal.BackColor = Color.DarkGray;
				tileBarItem.AppearanceItem.Normal.Options.UseBackColor = true;
				tileBarItem.DropDownOptions.BeakColor = Color.Empty;
				TileItemElement element = new TileItemElement
				{
					Text = ""
				};
				tileBarItem.Elements.Add(element);
				tileBarItem.ItemSize = TileBarItemSize.Wide;
				tileBar.SelectedItem.Group.Items.Add(tileBarItem);
				tileBar.SelectedItem = tileBarItem;
				UcWidgetViewer ucWidgetViewer = new UcWidgetViewer();
				ucWidgetViewer.Dock = DockStyle.Fill;
				ucWidgetViewer.Location = new Point(0, 0);
				ucWidgetViewer.Size = new Size(971, 630);
				ucWidgetViewer.TabIndex = 0;
				ucWidgetViewer.WidgetEntry = -1;
				NavigationPage navigationPage = new NavigationPage();
				navFrame.Controls.Add(navigationPage);
				navigationPage.Name = (num + 1).ToString();
				navigationPage.Controls.Add(ucWidgetViewer);
				navFrame.Pages.Add(navigationPage);
				navFrame.SelectedPage = navigationPage;
				txt_Page.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ChooseItems_Click(object sender, EventArgs e)
	{
		try
		{
			frm_WidgetAddItem frm_WidgetAddItem2 = new frm_WidgetAddItem(multiselect: true, Enums.WidgetItemType.All);
			frm_WidgetAddItem2.SelectItem = (Action<DataRow[]>)Delegate.Combine(frm_WidgetAddItem2.SelectItem, new Action<DataRow[]>(Widget_SelectItem));
			frm_WidgetAddItem2.UpdateDocument = (Action<string, string, bool>)Delegate.Combine(frm_WidgetAddItem2.UpdateDocument, new Action<string, string, bool>(UpdateDocument));
			frm_WidgetAddItem2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Widget_SelectItem(DataRow[] obj)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Caption_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			Text = txt_Caption.Text;
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			string value = Function.ToString(txt_Caption.EditValue);
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.CaptionIsMissing, "");
				txt_Caption.Focus();
				return;
			}
			if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataTable = DtHeader.Clone();
			dataTable.TableName = "APZ_OWID";
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = WidgetEntry;
			dataRow["Caption"] = value;
			dataRow["LayoutMode"] = 1;
			dataRow["FatherMenu"] = Function.ToString(btn_FatherMenu.EditValue);
			dataTable.Rows.Add(dataRow);
			DataTable dataTable2 = DtPages.Clone();
			dataTable2.TableName = "APZ_WID2";
			DataTable dataTable3 = DtDocuments.Clone();
			dataTable3.TableName = "APZ_WID1";
			int num = 0;
			foreach (TileBarGroup group in tileBar.Groups)
			{
				string value2 = Function.ToString(group.Text);
				foreach (ITileItem item2 in group.Items)
				{
					TileBarItem item = item2 as TileBarItem;
					if (item == null)
					{
						continue;
					}
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["AbsEntry"] = WidgetEntry;
					dataRow2["PageId"] = item.Id;
					TileItemElement tileItemElement = item.Elements.FirstOrDefault();
					if (tileItemElement != null)
					{
						dataRow2["PageText"] = tileItemElement.Text;
					}
					dataRow2["BackColor"] = item.AppearanceItem.Normal.BackColor.ToArgb();
					dataRow2["GroupText"] = value2;
					dataTable2.Rows.Add(dataRow2);
					foreach (NavigationPageBase item3 in navFrame.Pages.Where((NavigationPageBase t) => t.Name == item.Id.ToString()))
					{
						if (item3.Controls.Count <= 0 || !(item3.Controls[0] is UcWidgetViewer ucWidgetViewer))
						{
							continue;
						}
						foreach (BaseDocument item4 in (IEnumerable<BaseDocument>)ucWidgetViewer.WidgetView.Documents)
						{
							if (item4 is Document document)
							{
								DataRow dataRow3 = dataTable3.NewRow();
								dataRow3["AbsEntry"] = WidgetEntry;
								dataRow3["ItemId"] = num;
								dataRow3["PageId"] = item.Id;
								dataRow3["ObjType"] = document.ControlTypeName;
								dataRow3["ObjCode"] = document.ControlName;
								dataRow3["Caption"] = document.Caption;
								dataRow3["ColumnIndex"] = document.ColumnIndex;
								dataRow3["ColumnSpan"] = document.ColumnSpan;
								dataRow3["Height"] = document.Height;
								dataRow3["Width"] = document.Width;
								dataRow3["RowIndex"] = document.RowIndex;
								dataRow3["RowSpan"] = document.RowSpan;
								dataRow3["ToolTip"] = document.ToolTip;
								dataRow3["FreeLayoutHeight"] = document.FreeLayoutHeight.UnitValue;
								dataRow3["FreeLayoutWidth"] = document.FreeLayoutWidth.UnitValue;
								dataTable3.Rows.Add(dataRow3);
								num++;
							}
						}
					}
				}
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[3]
			{
				dataTable.Copy(),
				dataTable3.Copy(),
				dataTable2.Copy()
			});
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.Widget.Save(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				WinCommons._CloseProcessing();
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			FormMode = Enums.FormViewModeType.View;
			if (AfterSave != null)
			{
				AfterSave();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_FatherMenu_Properties_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Active_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewModeToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileBar_SelectedItemChanged_1(object sender, TileItemEventArgs e)
	{
		try
		{
			int id = e.Item.Id;
			if (navFrame.Pages.AsEnumerable().FirstOrDefault((NavigationPageBase t) => t.Name == id.ToString()) is NavigationPage selectedPage)
			{
				navFrame.SelectedPage = selectedPage;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_RemovePage_Click(object sender, EventArgs e)
	{
		try
		{
			TileBarItem selecteditem = tileBar.SelectedItem as TileBarItem;
			if (selecteditem == null)
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectItemToRemove, "");
			}
			else if (navFrame.SelectedPage is NavigationPage { Name: var name } navigationPage)
			{
				if (name != selecteditem.Id.ToString())
				{
					WinCommons.Error(Enums.MessageCode.PageIsNotMatch, "");
					return;
				}
				navFrame.Pages.Remove(navigationPage);
				TileGroup tileGroup = tileBar.Groups.FirstOrDefault((TileGroup t) => t.Items.Contains(selecteditem));
				if (tileGroup != null)
				{
					tileGroup.Items.Remove(selecteditem);
					ChangeViewModeToUpdate();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectAPage, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_RemoveGroup_Click(object sender, EventArgs e)
	{
		try
		{
			try
			{
				TileBarItem selecteditem = tileBar.SelectedItem as TileBarItem;
				if (selecteditem == null)
				{
					WinCommons.Warning(Enums.MessageCode.PleaseSelectItemToRemove, "");
				}
				else if (navFrame.SelectedPage is NavigationPage { Name: var name })
				{
					if (name != selecteditem.Id.ToString())
					{
						WinCommons.Error(Enums.MessageCode.PageIsNotMatch, "");
						return;
					}
					TileGroup tileGroup = tileBar.Groups.FirstOrDefault((TileGroup t) => t.Items.Contains(selecteditem));
					if (tileGroup == null)
					{
						return;
					}
					foreach (ITileItem item2 in tileGroup.Items)
					{
						TileBarItem item = item2 as TileBarItem;
						if (item != null)
						{
							NavigationPageBase navigationPageBase = navFrame.Pages.FirstOrDefault((NavigationPageBase t) => t.Name == item.Id.ToString());
							if (navigationPageBase != null)
							{
								navFrame.Pages.Remove(navigationPageBase);
							}
						}
					}
					tileBar.Groups.Remove(tileGroup);
					ChangeViewModeToUpdate();
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.PleaseSelectAPage, "");
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_Widget));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.document_SalesProcessModel = new DevExpress.XtraBars.Docking2010.Views.Widget.Document(this.components);
		this.tileBar = new DevExpress.XtraBars.Navigation.TileBar();
		this.navFrame = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucWidgetViewer1 = new Apzon.Forms.Widget.UcWidgetViewer();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
		this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
		this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.btn_RemoveGroup = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_RemovePage = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.btn_FatherMenu = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.txt_Caption = new Apzon.Controls.Editor.TextEditEx();
		this.btn_ChooseItems = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_AddPage = new Apzon.Controls.Controls.SimpleButtonEx();
		this.colorEdit_BackColor = new DevExpress.XtraEditors.ColorEdit();
		this.txt_Page = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Group = new Apzon.Controls.Editor.TextEditEx();
		this.btn_AddGroup = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.document_SalesProcessModel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navFrame).BeginInit();
		this.navFrame.SuspendLayout();
		this.navigationPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).BeginInit();
		this.dockPanel1.SuspendLayout();
		this.dockPanel1_Container.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_FatherMenu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.colorEdit_BackColor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Page.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Group.Properties).BeginInit();
		base.SuspendLayout();
		this.document_SalesProcessModel.Caption = "Sales A/R Process Model";
		this.document_SalesProcessModel.ColumnSpan = 4;
		this.document_SalesProcessModel.ControlName = "ucSalesProcessModel";
		this.document_SalesProcessModel.ControlTypeName = "Apzon.Forms.Widget.ucSalesProcessModel";
		this.document_SalesProcessModel.RowSpan = 4;
		this.tileBar.AllowGlyphSkinning = true;
		this.tileBar.AllowSelectedItem = true;
		this.tileBar.AppearanceGroupText.ForeColor = System.Drawing.Color.FromArgb(140, 140, 140);
		this.tileBar.AppearanceGroupText.Options.UseForeColor = true;
		this.tileBar.BackColor = System.Drawing.Color.FromArgb(232, 232, 232);
		this.tileBar.Cursor = System.Windows.Forms.Cursors.Default;
		this.tileBar.Dock = System.Windows.Forms.DockStyle.Top;
		this.tileBar.DropDownButtonWidth = 30;
		this.tileBar.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar.IndentBetweenGroups = 10;
		this.tileBar.IndentBetweenItems = 10;
		this.tileBar.ItemPadding = new System.Windows.Forms.Padding(5);
		this.tileBar.ItemSize = 35;
		this.tileBar.Location = new System.Drawing.Point(242, 0);
		this.tileBar.MaxId = 12;
		this.tileBar.Name = "tileBar";
		this.tileBar.Padding = new System.Windows.Forms.Padding(5);
		this.tileBar.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar.SelectionBorderWidth = 2;
		this.tileBar.SelectionColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.tileBar.SelectionColorMode = DevExpress.XtraBars.Navigation.SelectionColorMode.UseItemBackColor;
		this.tileBar.Size = new System.Drawing.Size(971, 77);
		this.tileBar.TabIndex = 4;
		this.tileBar.Text = "tileBar";
		this.tileBar.VerticalContentAlignment = DevExpress.Utils.VertAlignment.Bottom;
		this.tileBar.WideTileWidth = 150;
		this.tileBar.SelectedItemChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBar_SelectedItemChanged_1);
		this.tileBar.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBar_ItemClick);
		this.navFrame.Controls.Add(this.navigationPage1);
		this.navFrame.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navFrame.Location = new System.Drawing.Point(242, 77);
		this.navFrame.Name = "navFrame";
		this.navFrame.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[1] { this.navigationPage1 });
		this.navFrame.SelectedPage = this.navigationPage1;
		this.navFrame.Size = new System.Drawing.Size(971, 630);
		this.navFrame.TabIndex = 5;
		this.navFrame.Text = "navigationFrame1";
		this.navigationPage1.Controls.Add(this.ucWidgetViewer1);
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(971, 630);
		this.ucWidgetViewer1.ControlResized = null;
		this.ucWidgetViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucWidgetViewer1.DocumentClosed = null;
		this.ucWidgetViewer1.Location = new System.Drawing.Point(0, 0);
		this.ucWidgetViewer1.Name = "ucWidgetViewer1";
		this.ucWidgetViewer1.Size = new System.Drawing.Size(971, 630);
		this.ucWidgetViewer1.TabIndex = 0;
		this.ucWidgetViewer1.WidgetEntry = -1;
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Add Group";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(1293, 8);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(75, 23);
		this.simpleButton1.TabIndex = 6;
		this.simpleButton1.Text = "Add Group";
		this.simpleButton2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Add Tab";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(1293, 37);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 23);
		this.simpleButton2.TabIndex = 7;
		this.simpleButton2.Text = "Add Tab";
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.DockManager = this.dockManager1;
		this.barManager1.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1213, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 707);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1213, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 707);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1213, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 707);
		this.dockManager1.DockingOptions.ShowCloseButton = false;
		this.dockManager1.Form = this;
		this.dockManager1.MenuManager = this.barManager1;
		this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[1] { this.dockPanel1 });
		this.dockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.dockPanel1.Controls.Add(this.dockPanel1_Container);
		this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
		this.dockPanel1.ID = new System.Guid("d80a8687-0183-4c2d-aba1-7bd02da71ef6");
		this.dockPanel1.Location = new System.Drawing.Point(0, 0);
		this.dockPanel1.Name = "dockPanel1";
		this.dockPanel1.OriginalSize = new System.Drawing.Size(242, 200);
		this.dockPanel1.Size = new System.Drawing.Size(242, 707);
		this.dockPanel1.Text = "Properies";
		this.dockPanel1_Container.Controls.Add(this.btn_RemoveGroup);
		this.dockPanel1_Container.Controls.Add(this.btn_RemovePage);
		this.dockPanel1_Container.Controls.Add(this.btn_Cancel);
		this.dockPanel1_Container.Controls.Add(this.btn_OK);
		this.dockPanel1_Container.Controls.Add(this.labelLine1);
		this.dockPanel1_Container.Controls.Add(this.groupControl1);
		this.dockPanel1_Container.Controls.Add(this.btn_ChooseItems);
		this.dockPanel1_Container.Controls.Add(this.btn_AddPage);
		this.dockPanel1_Container.Controls.Add(this.colorEdit_BackColor);
		this.dockPanel1_Container.Controls.Add(this.txt_Page);
		this.dockPanel1_Container.Controls.Add(this.txt_Group);
		this.dockPanel1_Container.Controls.Add(this.btn_AddGroup);
		this.dockPanel1_Container.Location = new System.Drawing.Point(3, 26);
		this.dockPanel1_Container.Name = "dockPanel1_Container";
		this.dockPanel1_Container.Size = new System.Drawing.Size(235, 678);
		this.dockPanel1_Container.TabIndex = 0;
		this.btn_RemoveGroup.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_RemoveGroup.EditControl = null;
		this.btn_RemoveGroup.EditValue = "Remove Group";
		this.btn_RemoveGroup.FieldName = null;
		this.btn_RemoveGroup.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_RemoveGroup.ImageOptions.Image");
		this.btn_RemoveGroup.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_RemoveGroup.IsSearchControl = false;
		this.btn_RemoveGroup.Location = new System.Drawing.Point(89, 164);
		this.btn_RemoveGroup.Name = "btn_RemoveGroup";
		this.btn_RemoveGroup.ReadOnly = false;
		this.btn_RemoveGroup.SettingInVisible = false;
		this.btn_RemoveGroup.SettingReadonly = false;
		this.btn_RemoveGroup.Size = new System.Drawing.Size(78, 51);
		this.btn_RemoveGroup.TabIndex = 11;
		this.btn_RemoveGroup.Text = "Remove Group";
		this.btn_RemoveGroup.Click += new System.EventHandler(this.btn_RemoveGroup_Click);
		this.btn_RemovePage.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_RemovePage.EditControl = null;
		this.btn_RemovePage.EditValue = "Remove Page";
		this.btn_RemovePage.FieldName = null;
		this.btn_RemovePage.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_RemovePage.ImageOptions.Image");
		this.btn_RemovePage.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_RemovePage.IsSearchControl = false;
		this.btn_RemovePage.Location = new System.Drawing.Point(86, 285);
		this.btn_RemovePage.Name = "btn_RemovePage";
		this.btn_RemovePage.ReadOnly = false;
		this.btn_RemovePage.SettingInVisible = false;
		this.btn_RemovePage.SettingReadonly = false;
		this.btn_RemovePage.Size = new System.Drawing.Size(81, 51);
		this.btn_RemovePage.TabIndex = 11;
		this.btn_RemovePage.Text = "Remove Page";
		this.btn_RemovePage.Click += new System.EventHandler(this.btn_RemovePage_Click);
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(95, 647);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 15;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(14, 647);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 14;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(6, 621);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(227, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 13;
		this.labelLine1.Utext = "";
		this.groupControl1.Controls.Add(this.labelControl5);
		this.groupControl1.Controls.Add(this.btn_FatherMenu);
		this.groupControl1.Controls.Add(this.chk_Active);
		this.groupControl1.Controls.Add(this.labelControl4);
		this.groupControl1.Controls.Add(this.txt_Caption);
		this.groupControl1.Location = new System.Drawing.Point(3, 3);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(227, 147);
		this.groupControl1.TabIndex = 12;
		this.groupControl1.Text = "Main Infomation";
		this.labelControl5.Location = new System.Drawing.Point(14, 75);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(61, 13);
		this.labelControl5.TabIndex = 11;
		this.labelControl5.Text = "Father Menu";
		this.btn_FatherMenu.EditControl = null;
		this.btn_FatherMenu.FieldName = null;
		this.btn_FatherMenu.IsSearchControl = false;
		this.btn_FatherMenu.Location = new System.Drawing.Point(14, 95);
		this.btn_FatherMenu.Name = "btn_FatherMenu";
		this.btn_FatherMenu.Properties.AutoLoadDataSource = true;
		this.btn_FatherMenu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_FatherMenu.Properties.ContainerControl = null;
		this.btn_FatherMenu.Properties.DisplayMember = "";
		this.btn_FatherMenu.Properties.DisplayMember2 = "";
		this.btn_FatherMenu.Properties.LoadDataSourceWhenEnter = false;
		this.btn_FatherMenu.Properties.SeparateCharacterForMulti = ';';
		this.btn_FatherMenu.Properties.SeparateString = ", ";
		this.btn_FatherMenu.Properties.UsingAutoCompleteBox = false;
		this.btn_FatherMenu.Properties.UsingDataSource = true;
		this.btn_FatherMenu.Properties.ValidateWhenNoDataSource = false;
		this.btn_FatherMenu.Properties.ValueMember = "";
		this.btn_FatherMenu.Properties.EditValueChanged += new System.EventHandler(this.btn_FatherMenu_Properties_EditValueChanged);
		this.btn_FatherMenu.SettingInVisible = false;
		this.btn_FatherMenu.SettingReadonly = false;
		this.btn_FatherMenu.Size = new System.Drawing.Size(209, 22);
		this.btn_FatherMenu.TabIndex = 10;
		this.chk_Active.EditControl = null;
		this.chk_Active.EditValue = "Y";
		this.chk_Active.FieldName = null;
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(14, 121);
		this.chk_Active.MenuManager = this.barManager1;
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 5;
		this.chk_Active.CheckedChanged += new System.EventHandler(this.chk_Active_CheckedChanged);
		this.labelControl4.Location = new System.Drawing.Point(14, 32);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(74, 13);
		this.labelControl4.TabIndex = 2;
		this.labelControl4.Text = "Widget Caption";
		this.txt_Caption.EditControl = null;
		this.txt_Caption.FieldName = null;
		this.txt_Caption.IsSearchControl = false;
		this.txt_Caption.Location = new System.Drawing.Point(14, 51);
		this.txt_Caption.MenuManager = this.barManager1;
		this.txt_Caption.Name = "txt_Caption";
		this.txt_Caption.SettingInVisible = false;
		this.txt_Caption.SettingReadonly = false;
		this.txt_Caption.Size = new System.Drawing.Size(209, 20);
		this.txt_Caption.TabIndex = 3;
		this.txt_Caption.EditValueChanged += new System.EventHandler(this.txt_Caption_EditValueChanged);
		this.btn_ChooseItems.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_ChooseItems.EditControl = null;
		this.btn_ChooseItems.EditValue = "Choose Items";
		this.btn_ChooseItems.FieldName = null;
		this.btn_ChooseItems.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChooseItems.ImageOptions.Image");
		this.btn_ChooseItems.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_ChooseItems.IsSearchControl = false;
		this.btn_ChooseItems.Location = new System.Drawing.Point(17, 446);
		this.btn_ChooseItems.Name = "btn_ChooseItems";
		this.btn_ChooseItems.ReadOnly = false;
		this.btn_ChooseItems.SettingInVisible = false;
		this.btn_ChooseItems.SettingReadonly = false;
		this.btn_ChooseItems.Size = new System.Drawing.Size(84, 51);
		this.btn_ChooseItems.TabIndex = 11;
		this.btn_ChooseItems.Text = "Choose Items";
		this.btn_ChooseItems.Click += new System.EventHandler(this.btn_ChooseItems_Click);
		this.btn_AddPage.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_AddPage.EditControl = null;
		this.btn_AddPage.EditValue = "Add Page";
		this.btn_AddPage.FieldName = null;
		this.btn_AddPage.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddPage.ImageOptions.Image");
		this.btn_AddPage.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_AddPage.IsSearchControl = false;
		this.btn_AddPage.Location = new System.Drawing.Point(11, 285);
		this.btn_AddPage.Name = "btn_AddPage";
		this.btn_AddPage.ReadOnly = false;
		this.btn_AddPage.SettingInVisible = false;
		this.btn_AddPage.SettingReadonly = false;
		this.btn_AddPage.Size = new System.Drawing.Size(67, 51);
		this.btn_AddPage.TabIndex = 5;
		this.btn_AddPage.Text = "Add Page";
		this.btn_AddPage.Click += new System.EventHandler(this.btn_AddPage_Click);
		this.colorEdit_BackColor.EditValue = System.Drawing.Color.Empty;
		this.colorEdit_BackColor.Location = new System.Drawing.Point(14, 368);
		this.colorEdit_BackColor.MenuManager = this.barManager1;
		this.colorEdit_BackColor.Name = "colorEdit_BackColor";
		this.colorEdit_BackColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.colorEdit_BackColor.Size = new System.Drawing.Size(209, 20);
		this.colorEdit_BackColor.TabIndex = 3;
		this.colorEdit_BackColor.EditValueChanged += new System.EventHandler(this.colorEdit_BackColor_EditValueChanged);
		this.txt_Page.EditControl = null;
		this.txt_Page.FieldName = null;
		this.txt_Page.IsSearchControl = false;
		this.txt_Page.Location = new System.Drawing.Point(14, 342);
		this.txt_Page.MenuManager = this.barManager1;
		this.txt_Page.Name = "txt_Page";
		this.txt_Page.SettingInVisible = false;
		this.txt_Page.SettingReadonly = false;
		this.txt_Page.Size = new System.Drawing.Size(209, 20);
		this.txt_Page.TabIndex = 2;
		this.txt_Page.EditValueChanged += new System.EventHandler(this.txt_Page_EditValueChanged);
		this.txt_Group.EditControl = null;
		this.txt_Group.FieldName = null;
		this.txt_Group.IsSearchControl = false;
		this.txt_Group.Location = new System.Drawing.Point(14, 221);
		this.txt_Group.MenuManager = this.barManager1;
		this.txt_Group.Name = "txt_Group";
		this.txt_Group.SettingInVisible = false;
		this.txt_Group.SettingReadonly = false;
		this.txt_Group.Size = new System.Drawing.Size(209, 20);
		this.txt_Group.TabIndex = 1;
		this.txt_Group.EditValueChanged += new System.EventHandler(this.txt_Group_EditValueChanged);
		this.btn_AddGroup.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_AddGroup.EditControl = null;
		this.btn_AddGroup.EditValue = "Add Group";
		this.btn_AddGroup.FieldName = null;
		this.btn_AddGroup.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddGroup.ImageOptions.Image");
		this.btn_AddGroup.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_AddGroup.IsSearchControl = false;
		this.btn_AddGroup.Location = new System.Drawing.Point(14, 164);
		this.btn_AddGroup.Name = "btn_AddGroup";
		this.btn_AddGroup.ReadOnly = false;
		this.btn_AddGroup.SettingInVisible = false;
		this.btn_AddGroup.SettingReadonly = false;
		this.btn_AddGroup.Size = new System.Drawing.Size(67, 51);
		this.btn_AddGroup.TabIndex = 4;
		this.btn_AddGroup.Text = "Add Group";
		this.btn_AddGroup.Click += new System.EventHandler(this.btn_AddGroup_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1213, 707);
		base.Controls.Add(this.navFrame);
		base.Controls.Add(this.tileBar);
		base.Controls.Add(this.dockPanel1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Widget.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Widget";
		base.ShowInTaskbar = false;
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_SalesWidget_Load);
		((System.ComponentModel.ISupportInitialize)this.document_SalesProcessModel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navFrame).EndInit();
		this.navFrame.ResumeLayout(false);
		this.navigationPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dockManager1).EndInit();
		this.dockPanel1.ResumeLayout(false);
		this.dockPanel1_Container.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		this.groupControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_FatherMenu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.colorEdit_BackColor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Page.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Group.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
