using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Forms.Widget;
using Apzon.HttpClientHelper;
using DevExpress.DashboardCommon;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraTab;

namespace Apzon.Forms.Dashboard;

public class frm_DashboardManager : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isModified;

	private DevExpress.DashboardCommon.Dashboard _dashboard;

	private int _focusedCatId = -1;

	private IContainer components;

	private GridControlEx grd_Main;

	private TileView titleView_Main;

	private TileViewColumn col_Layout;

	private TileViewColumn col_Name;

	private TileViewColumn col_Category;

	private ImageCollection imageCollection1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem ctm_NewDashboard;

	private NavigationFrame navigationFrame1;

	private NavigationPage npage_Main;

	private NavigationPage npage_Viewer;

	private UcDashboardViewer ucDashboardViewer1;

	private SimpleButtonEx btn_Back;

	private SimpleButtonEx btn_EditDesign;

	private LabelControl lbl_DashboardName;

	private SimpleButtonEx btn_NewCat;

	private SimpleButtonEx btn_NewDashboard;

	private NavigationPage npage_NewDashboard;

	private TextEditEx txt_Caption;

	private LabelLine labelLine1;

	private GroupControl groupControl1;

	private XtraTabControlEx tab_Source;

	private XtraTabPageEx tab_X;

	private XtraTabPageEx tab_Query;

	private LabelLine labelLine2;

	private RadioGroupEx rdg_Source;

	private XtraTabPageEx tab_Report;

	private XtraTabPageEx tab_Form;

	private LabelLine labelLine3;

	private ButtonEditEx btn_BrowseFile;

	private ButtonEditCFL bte_DataFrom;

	private LookUpEdit look_DataFrom;

	private LabelLine lbl_DataFrom;

	private ButtonEditCFL bte_RptString;

	private LabelLine labelLine4;

	private MemoExEdit memo_DataString;

	private SimpleButtonEx btn_EditDesignDashboard;

	private SimpleButtonEx btn_Back2;

	private SimpleButtonEx btn_Add;

	private AutoCompleteTextBox txt_Search;

	private ButtonEditEx txt_GroupText;

	private LabelLine labelLine5;

	private LookUpEdit look_Category;

	private GridControlEx grd_UdrParams;

	private GridViewEx gv_UdrParams;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemLookUpEdit rep_UdrParamDataType;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private LabelControlEx lbl_DashboardCaption;

	private ToolStripMenuItem ctm_Edit;

	private ToolStripMenuItem ctm_Remove;

	private ToolStripSeparator toolStripSeparator1;

	private SimpleButtonEx btn_Edit;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
				btn_Add.Enabled = true;
				btn_Back.Enabled = true;
				btn_Back2.Enabled = true;
				btn_EditDesign.Enabled = true;
				btn_EditDesignDashboard.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Add.Enabled = true;
				btn_Back.Enabled = true;
				btn_Back2.Enabled = true;
				btn_EditDesign.Enabled = true;
				btn_EditDesignDashboard.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				_isModified = false;
				btn_Add.Enabled = true;
				btn_Back.Enabled = true;
				btn_Back2.Enabled = true;
				btn_EditDesign.Enabled = true;
				btn_EditDesignDashboard.Enabled = true;
				break;
			case Enums.FormViewModeType.Busy:
				btn_Add.Enabled = false;
				btn_Back.Enabled = false;
				btn_Back2.Enabled = false;
				btn_EditDesign.Enabled = false;
				btn_EditDesignDashboard.Enabled = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_DashboardManager()
	{
		InitializeComponent();
	}

	private async void frm_DashboardManager_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadAllData();
			look_DataFrom.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Defined Query" },
				{ "T", "Query Text" }
			});
			bte_DataFrom.Properties.DisplayMember = "QName";
			bte_DataFrom.Properties.ValueMember = "IntrnalKey";
			bte_DataFrom.Properties.TypeCFL = Enums.TypeCFL.Queries;
			bte_DataFrom.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties = bte_DataFrom.Properties;
			properties.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties.ItemSelected, new Action<DataRow, GridView>(DataFrom_ItemSelected));
			bte_RptString.Properties.DisplayMember = "Name";
			bte_RptString.Properties.ValueMember = "Code";
			bte_RptString.Properties.TypeCFL = Enums.TypeCFL.UserDefinedReport;
			bte_RptString.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties2 = bte_RptString.Properties;
			properties2.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties2.ItemSelected, new Action<DataRow, GridView>(Report_ItemSelected));
			tab_Source.ShowTabHeader = DefaultBoolean.False;
			txt_GroupText.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Report_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			string text = Function.ToString(row["Code"]);
			if (!string.IsNullOrEmpty(text))
			{
				DataTable dataSource = await _httpClient.Udr.GetParamsByCode(text);
				grd_UdrParams.DataSource = dataSource;
				grd_UdrParams.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadAllData()
	{
		DataTable dataSource = await _httpClient.DashboardDetail.LoadAll();
		grd_Main.DataSource = dataSource;
		grd_Main.RefreshDataSource();
	}

	private async void DataFrom_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			Function.ToString(row["QString"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void titleView_Main_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		try
		{
			if (e.Item.Elements[0] is TileViewItemElement tileViewItemElement && !string.IsNullOrEmpty(tileViewItemElement.Text))
			{
				string value = Function.ToString(tileViewItemElement);
				if (!string.IsNullOrEmpty(value))
				{
					XElement xElement = XDocument.Parse(value).Descendants("Items").Elements()
						.FirstOrDefault();
					if (xElement != null)
					{
						switch (xElement.Name.LocalName.ToLower())
						{
						case "grid":
						case "pivot":
							e.Item.ImageIndex = 0;
							break;
						case "chart":
							e.Item.ImageIndex = 1;
							break;
						case "scatterchart":
							e.Item.ImageIndex = 2;
							break;
						case "pie":
							e.Item.ImageIndex = 3;
							break;
						case "gauge":
							e.Item.ImageIndex = 4;
							break;
						case "card":
							e.Item.ImageIndex = 5;
							break;
						case "piemap":
						case "geopointmap":
						case "bubblemap":
						case "choroplethmap":
							e.Item.ImageIndex = 6;
							break;
						default:
							e.Item.ImageIndex = 1;
							break;
						}
					}
				}
				else
				{
					e.Item.ImageIndex = 7;
				}
			}
			else
			{
				e.Item.ImageIndex = 7;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void titleView_Main_BeforeItemDrag(object sender, BeforeItemDragEventArgs e)
	{
		try
		{
			e.Cancel = IsEmptyItem(e.RowHandle);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool IsEmptyItem(int rowHandle)
	{
		try
		{
			if (titleView_Main.GetRow(rowHandle) == null)
			{
				return false;
			}
			return Function.ParseInt(titleView_Main.GetRowCellValue(rowHandle, "Id")) < 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private void titleView_Main_ItemDrop(object sender, ItemDropEventArgs e)
	{
		try
		{
			object groupColumnValue = e.GroupColumnValue;
			if (!e.PrevGroupColumnValue.Equals(groupColumnValue))
			{
				titleView_Main.BeginDataUpdate();
				DataTable dataSourceTable = grd_Main.DataSourceTable;
				dataSourceTable.Rows[e.ListSourceRowIndex]["CatId"] = Function.ParseInt(groupColumnValue);
				dataSourceTable.AcceptChanges();
				titleView_Main.EndDataUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void titleView_Main_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column == col_Category)
			{
				object obj = grd_Main.DataSourceTable.Rows[e.ListSourceRowIndex]["Category"];
				e.DisplayText = Function.ToString(obj);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void titleView_Main_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(titleView_Main.GetRowCellValue(e.Item.RowHandle, "Layout"))))
			{
				int num = Function.ParseInt(titleView_Main.GetRowCellValue(e.Item.RowHandle, "Id"));
				if (num != 0)
				{
					await ucDashboardViewer1.SetDashboardId(num);
					navigationFrame1.SelectedPageIndex = 1;
					lbl_DashboardName.Text = Function.ToString(titleView_Main.GetRowCellValue(e.Item.RowHandle, "Name"));
				}
			}
			else
			{
				btn_NewDashboard_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Back_Click(object sender, EventArgs e)
	{
		await BackToMain();
	}

	private async Task BackToMain()
	{
		try
		{
			if (_isModified)
			{
				await LoadAllData();
			}
			navigationFrame1.SelectedPageIndex = 0;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Edit_Click(object sender, EventArgs e)
	{
		try
		{
			if (_dashboard == null)
			{
				_dashboard = new DevExpress.DashboardCommon.Dashboard();
			}
			string text = Function.ToString(titleView_Main.GetFocusedRowCellValue("Layout"));
			int itemClickedId = Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id"));
			_dashboard.LoadFromXDocument(XDocument.Parse(text));
			DevExpress.DashboardCommon.Dashboard dash = this.LoadDashboardFromXml(_dashboard.SaveToXDocument());
			DataTable dataTable = await _httpClient.DashboardDetail.GetById(itemClickedId);
			if (Function.ToString(dataTable.Rows[0]["SourceType"]) == "R")
			{
				string code = Function.ToString(dataTable.Rows[0]["UdrCode"]);
				DataTable dataTable2 = await _httpClient.Udr.GetQueryByCode(code);
				if (dataTable2.IsNotNull())
				{
					DataTable dataTable3 = DashboardHelper.GetDashboardSourceByReport(storename: Function.ToString(dataTable2.Rows[0]["StoreName"]), drPr: await _httpClient.DashboardDetail.GetParams(itemClickedId));
					foreach (IDashboardDataSource dataSource in dash.DataSources)
					{
						if (dataSource is DashboardObjectDataSource)
						{
							((DashboardObjectDataSource)dataSource).DataSource = dataTable3.Copy();
						}
					}
				}
			}
			frm_DashboardDesigner frm = new frm_DashboardDesigner(dash);
			frm_DashboardDesigner frm_DashboardDesigner2 = frm;
			frm_DashboardDesigner2.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner2.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
			{
				await SaveEditedLayout(obj, frm);
			}));
			frm.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveEditedLayout(DevExpress.DashboardCommon.Dashboard obj, frm_DashboardDesigner frm)
	{
		WinCommons._OpenProcessing();
		try
		{
			int itemClickedId = Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id"));
			await SaveEditedDashboard(obj, frm, closeAfterSave: false);
			string layout = _dashboard.SaveToXDocument().ToString();
			HttpResult httpResult = _httpClient.DashboardDetail.UpdateLayout(itemClickedId, layout);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				WinCommons._CloseProcessing();
				return;
			}
			await ucDashboardViewer1.SetDashboardId(itemClickedId);
			frm.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
	}

	private void txt_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			titleView_Main.ActiveFilterString = string.Format("Name LIKE '%{0}%' OR Category LIKE '%{0}%'", txt_Search.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_NewDashboard_Click(object sender, EventArgs e)
	{
		try
		{
			navigationFrame1.SelectedPageIndex = 2;
			var array = (from t in grd_Main.DataSourceTable.AsEnumerable()
				select new
				{
					Id = Function.ToString(t["CatId"]),
					Name = Function.ToString(t["Category"])
				}).Distinct().ToArray();
			if (array.Length != 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("Id", typeof(string));
				dataTable.Columns.Add("Name", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["Id"] = "-1";
				dataRow["Name"] = "";
				dataTable.Rows.Add(dataRow);
				var array2 = array;
				foreach (var anon in array2)
				{
					dataRow = dataTable.NewRow();
					dataRow["Id"] = anon.Id;
					dataRow["Name"] = anon.Name;
					dataTable.Rows.Add(dataRow);
				}
				look_Category.Properties.DataSource = dataTable;
				look_Category.EditValue = "-1";
				txt_Caption.EditValue = "";
				rdg_Source.SelectedIndex = 0;
				bte_RptString.Text = "";
				look_DataFrom.EditValue = "";
				bte_DataFrom.EditValue = "";
				memo_DataString.EditValue = "";
				bte_RptString.EditValue = "";
			}
			txt_Caption.Focus();
			_dashboard = null;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rdg_Source_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			tab_Source.SelectedTabPageIndex = rdg_Source.SelectedIndex;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_BrowseFile_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".xml",
				Multiselect = false,
				Filter = "Dashboard Layout (.xml)|*.xml"
			};
			openFileDialog.ShowDialog(this);
			if (!string.IsNullOrEmpty(openFileDialog.FileName))
			{
				btn_BrowseFile.EditValue = openFileDialog.FileName;
				XDocument document = XDocument.Parse(File.ReadAllText(Function.ToString(btn_BrowseFile.EditValue)));
				_dashboard = new DevExpress.DashboardCommon.Dashboard();
				_dashboard.LoadFromXDocument(document);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void look_DataFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				string dataFrom = Function.ToString(look_DataFrom.EditValue);
				VisibleDataString(dataFrom);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void VisibleDataString(string dataFrom)
	{
		if (!(dataFrom == "Q"))
		{
			if (dataFrom == "T")
			{
				memo_DataString.Visible = true;
				memo_DataString.Left = bte_DataFrom.Left;
				bte_DataFrom.Visible = false;
			}
		}
		else
		{
			memo_DataString.Visible = false;
			bte_DataFrom.Visible = true;
		}
	}

	private void txt_Caption_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			lbl_DashboardCaption.Text = txt_Caption.Text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Back2_Click(object sender, EventArgs e)
	{
		try
		{
			if (_dashboard == null || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await BackToMain();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_EditDesignDashboard_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			new DevExpress.DashboardCommon.Dashboard();
			DevExpress.DashboardCommon.Dashboard dash = ((_dashboard == null) ? null : _dashboard);
			string text = Function.ToString(rdg_Source.EditValue);
			DataTable rs = new DataTable();
			if (!(text == "R"))
			{
				if (text == "Q")
				{
					string text2 = Function.ToString(look_DataFrom.EditValue);
					string sql;
					if (!(text2 == "Q"))
					{
						if (!(text2 == "T"))
						{
							WinCommons.Warning(Enums.MessageCode.None, "Choose type of query");
							return;
						}
						sql = memo_DataString.Text;
					}
					else
					{
						int queryId = Function.ParseInt(bte_DataFrom.EditValue);
						sql = await _httpClient.Queries.GetQueryText(queryId);
					}
					DataSet dataSet = _httpClient.Commons.ExecuteSql(sql);
					if (dataSet.Tables.Count == 2 && dataSet.Tables[1].Columns.Count > 0)
					{
						rs = dataSet.Tables[1];
					}
				}
			}
			else
			{
				string code = Function.ToString(bte_RptString.EditValue);
				DataTable dataTable = await _httpClient.Udr.GetQueryByCode(code);
				if (dataTable.IsNotNull())
				{
					string storename = Function.ToString(dataTable.Rows[0]["StoreName"]);
					DataTable dataSourceTable = grd_UdrParams.DataSourceTable;
					rs = DashboardHelper.GetDashboardSourceByReport(dataSourceTable, storename);
				}
			}
			string text3 = Function.ToString(txt_Caption.EditValue);
			rs.TableName = (string.IsNullOrEmpty(text3) ? "Table" : text3);
			if (rs.Columns.Count > 0)
			{
				if (dash == null)
				{
					DataSet dataSet2 = new DataSet();
					dataSet2.Tables.Add(rs.Copy());
					frm_DashboardDesigner frmDesign = new frm_DashboardDesigner(dataSet2, text3);
					frm_DashboardDesigner frm_DashboardDesigner2 = frmDesign;
					frm_DashboardDesigner2.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner2.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
					{
						await SaveEditedDashboard(obj, frmDesign);
					}));
					frmDesign.ShowDialog();
					WinCommons._CloseProcessing();
					return;
				}
				foreach (IDashboardDataSource dataSource in dash.DataSources)
				{
					if (dataSource is DashboardObjectDataSource)
					{
						((DashboardObjectDataSource)dataSource).DataSource = rs.Copy();
					}
				}
			}
			if (dash != null)
			{
				frm_DashboardDesigner frm = new frm_DashboardDesigner(dash)
				{
					MdiParent = MdiParent
				};
				frm_DashboardDesigner frm_DashboardDesigner3 = frm;
				frm_DashboardDesigner3.SaveEditedDashboard = (Action<DevExpress.DashboardCommon.Dashboard>)Delegate.Combine(frm_DashboardDesigner3.SaveEditedDashboard, (Action<DevExpress.DashboardCommon.Dashboard>)(async (DevExpress.DashboardCommon.Dashboard obj) =>
				{
					await SaveEditedDashboard(obj, frm);
				}));
				frm.Show();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.ExecuteQueryError, "");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
	}

	private async Task SaveEditedDashboard(DevExpress.DashboardCommon.Dashboard obj, frm_DashboardDesigner frm, bool closeAfterSave = true)
	{
		try
		{
			_isModified = true;
			DevExpress.DashboardCommon.Dashboard edited = new DevExpress.DashboardCommon.Dashboard();
			edited.LoadFromXDocument(obj.SaveToXDocument());
			if (_dashboard != null)
			{
				foreach (IDashboardDataSource dataSource in _dashboard.DataSources)
				{
					foreach (DashboardItem item in edited.Items.Where((DashboardItem t) => t is DataDashboardItem && ((DataDashboardItem)t).DataSource == edited.DataSources[0]))
					{
						if (item is DataDashboardItem dataDashboardItem && dataDashboardItem.DataSource.Name != dataSource.Name)
						{
							dataDashboardItem.DataSource = dataSource;
						}
					}
					dataSource.CalculatedFields.AddRange(edited.DataSources[0].CalculatedFields.ToArray());
					dataSource.Filter = edited.DataSources[0].Filter;
					if (edited.DataSources.Count > 1)
					{
						edited.DataSources.RemoveAt(0);
					}
				}
			}
			_dashboard = edited;
			if (closeAfterSave)
			{
				frm.Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Add_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			int num = Function.ParseInt(look_Category.EditValue);
			if (num <= 0)
			{
				WinCommons.Warning(Enums.MessageCode.CategoryCannotBeEmpty, "");
				FormMode = currentMode;
				return;
			}
			DataTable dataTable = grd_Main.DataSourceTable.Clone();
			dataTable.TableName = "APZ_DAB1";
			DataRow dataRow = dataTable.NewRow();
			dataRow["CatId"] = num;
			dataRow["Name"] = Function.ToString(txt_Caption.EditValue);
			XDocument xDocument = _dashboard.SaveToXDocument();
			dataRow["Layout"] = xDocument.ToString();
			dataRow["SourceType"] = Function.ToString(rdg_Source.EditValue);
			dataRow["UdrCode"] = Function.ToString(bte_RptString.EditValue);
			dataRow["Id"] = ((currentMode != Enums.FormViewModeType.Add) ? Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id")) : 0);
			string text = Function.ToString(look_DataFrom.EditValue);
			dataRow["Query"] = (text.Equals("T") ? memo_DataString.EditValue : bte_DataFrom.EditValue);
			dataRow["SqlType"] = text;
			dataTable.Rows.Add(dataRow);
			DataTable dataSourceTable = grd_UdrParams.DataSourceTable;
			if (!dataSourceTable.ExistsColumn("Id"))
			{
				dataSourceTable.Columns.Add("Id", typeof(int));
			}
			dataSourceTable.TableName = "APZ_DAB4";
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[2]
			{
				dataTable.Copy(),
				dataSourceTable.Copy()
			});
			HttpResult httpResult = await _httpClient.DashboardDetail.SaveDataSet(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isModified = true;
				await BackToMain();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currentMode;
	}

	private void titleView_Main_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			TileViewHitInfo tileViewHitInfo = titleView_Main.CalcHitInfo(e.Location);
			if ((e.Button & MouseButtons.Left) != 0 && tileViewHitInfo.HitTest == TileControlHitTest.GroupCaption)
			{
				string editValue = tileViewHitInfo.GroupInfo.Group.Text;
				txt_GroupText.Visible = true;
				txt_GroupText.Width = tileViewHitInfo.GroupInfo.Bounds.Width;
				txt_GroupText.Location = new Point(tileViewHitInfo.GroupInfo.Bounds.X, tileViewHitInfo.GroupInfo.Bounds.Y + 5);
				txt_GroupText.EditValue = editValue;
				txt_GroupText.Focus();
				int rowHandle = titleView_Main.CalcHitInfo(txt_GroupText.Location).Item.RowHandle;
				_focusedCatId = Function.ParseInt(titleView_Main.GetRowCellValue(rowHandle, "CatId"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_GroupText_Leave(object sender, EventArgs e)
	{
		try
		{
			UpdateGroupCaption();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UpdateGroupCaption()
	{
		DataTable dataSourceTable = grd_Main.DataSourceTable;
		if (dataSourceTable.IsNotNull() && _focusedCatId != -1)
		{
			string text = Function.ToString(txt_GroupText.Text).Trim();
			DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["CatId"]) == _focusedCatId);
			if (dataRow != null)
			{
				if (Function.ToString(dataRow["Category"]).Trim() != text && !string.IsNullOrEmpty(text))
				{
					PostGroupCaption(text, dataSourceTable);
				}
				else
				{
					HideGroupText();
				}
			}
			else
			{
				PostGroupCaption(text, dataSourceTable);
			}
		}
		else
		{
			HideGroupText();
		}
	}

	private void PostGroupCaption(string newvalue, DataTable source)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Id", typeof(int));
		dataTable.Columns.Add("Caption", typeof(string));
		DataRow dataRow = dataTable.NewRow();
		dataRow["Id"] = _focusedCatId;
		dataRow["Caption"] = newvalue;
		dataTable.Rows.Add(dataRow);
		HttpResult httpResult = _httpClient.CategoriesDABClient.UpdateCaption(dataTable);
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			return;
		}
		foreach (DataRow item in from t in source.AsEnumerable()
			where Function.ParseInt(t["CatId"]) == _focusedCatId
			select t)
		{
			item["Category"] = newvalue;
			item["Id"] = Function.ParseInt(httpResult.Content);
		}
		source.AcceptChanges();
		HideGroupText();
	}

	private void HideGroupText()
	{
		txt_GroupText.Visible = false;
		txt_GroupText.Text = "";
		_focusedCatId = -1;
	}

	private void txt_GroupText_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				UpdateGroupCaption();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_DashboardManager_SizeChanged(object sender, EventArgs e)
	{
		try
		{
			HideGroupText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NewCat_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Main.DataSourceTable;
			int num = 1;
			if (dataSourceTable.IsNotNull())
			{
				num = Function.ParseInt(dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["CatId"]))) + 1;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["CatId"] = num;
			dataRow["Category"] = "Untitled";
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			TileGroupViewInfoCollection groups = ((ITileControl)titleView_Main.GetViewInfo()).ViewInfo.Groups;
			if (groups != null)
			{
				TileGroupViewInfo tileGroupViewInfo = groups[groups.Count - 1];
				txt_GroupText.Visible = true;
				txt_GroupText.Width = 150;
				txt_GroupText.Left = tileGroupViewInfo.Bounds.X;
				txt_GroupText.Top = tileGroupViewInfo.Bounds.Y + 5;
				txt_GroupText.EditValue = "";
				txt_GroupText.Focus();
				_focusedCatId = num;
			}
			int num2 = titleView_Main.LocateByValue("CatId", Function.ParseInt(num));
			titleView_Main.FocusedRowHandle = num2;
			titleView_Main.MakeRowVisible(num2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void titleView_Main_ShownEditor(object sender, EventArgs e)
	{
	}

	private void txt_GroupText_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (_focusedCatId <= 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveCatalogAndDashboard, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			HttpResult httpResult = _httpClient.CategoriesDABClient.RemoveCategory(_focusedCatId);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			DataTable dataSourceTable = grd_Main.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				if (Function.ParseInt(dataSourceTable.Rows[i]["CatId"]) == _focusedCatId)
				{
					dataSourceTable.Rows.RemoveAt(i);
					i--;
				}
			}
			dataSourceTable.AcceptChanges();
			HideGroupText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_UdrParams_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_NewDashboard_Click(object sender, EventArgs e)
	{
		try
		{
			btn_NewDashboard_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ctm_Remove_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveDashboard, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			int[] selectedRows = titleView_Main.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				WinCommons._OpenProcessing();
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					int itemClickedId = Function.ParseInt(titleView_Main.GetRowCellValue(rowHandle, "Id"));
					HttpResult httpResult = await _httpClient.DashboardDetail.Remove(itemClickedId);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						WinCommons._CloseProcessing();
						return;
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				await LoadAllData();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.PleaseChooseItemForThisAction, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void Btn_Edit_Click_1(object sender, EventArgs e)
	{
		try
		{
			int id = Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id"));
			await ShowEditDashboardPage(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ShowEditDashboardPage(int id)
	{
		DataTable dataTable = await _httpClient.DashboardDetail.GetById(id);
		if (dataTable.IsNotNull())
		{
			navigationFrame1.SelectedPageIndex = 2;
			var array = (from t in grd_Main.DataSourceTable.AsEnumerable()
				select new
				{
					Id = Function.ToString(t["CatId"]),
					Name = Function.ToString(t["Category"])
				}).Distinct().ToArray();
			if (array.Length != 0)
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("Id", typeof(string));
				dataTable2.Columns.Add("Name", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["Id"] = "-1";
				dataRow["Name"] = "";
				dataTable2.Rows.Add(dataRow);
				var array2 = array;
				foreach (var anon in array2)
				{
					dataRow = dataTable2.NewRow();
					dataRow["Id"] = anon.Id;
					dataRow["Name"] = anon.Name;
					dataTable2.Rows.Add(dataRow);
				}
				look_Category.Properties.DataSource = dataTable2;
			}
			look_Category.EditValue = Function.ToString(dataTable.Rows[0]["CatID"]);
			txt_Caption.EditValue = Function.ToString(dataTable.Rows[0]["Name"]);
			rdg_Source.EditValue = Function.ToString(dataTable.Rows[0]["SourceType"]);
			bte_RptString.Text = Function.ToString(dataTable.Rows[0]["Query"]);
			look_DataFrom.EditValue = Function.ToString(dataTable.Rows[0]["SqlType"]);
			bte_DataFrom.EditValue = Function.ToString(dataTable.Rows[0]["Query"]);
			memo_DataString.EditValue = Function.ToString(dataTable.Rows[0]["Query"]);
			bte_RptString.EditValue = Function.ToString(dataTable.Rows[0]["UdrCode"]);
			if (_dashboard == null)
			{
				_dashboard = new DevExpress.DashboardCommon.Dashboard();
			}
			int itemClickedId = Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id"));
			string text = Function.ToString(titleView_Main.GetFocusedRowCellValue("Layout"));
			_dashboard.LoadFromXDocument(XDocument.Parse(text));
			DevExpress.DashboardCommon.Dashboard dash = this.LoadDashboardFromXml(_dashboard.SaveToXDocument());
			if (Function.ToString(dataTable.Rows[0]["SourceType"]) == "R")
			{
				string code = Function.ToString(dataTable.Rows[0]["UdrCode"]);
				DataTable dataTable3 = await _httpClient.Udr.GetQueryByCode(code);
				if (dataTable3.IsNotNull())
				{
					DataTable dataTable4 = DashboardHelper.GetDashboardSourceByReport(storename: Function.ToString(dataTable3.Rows[0]["StoreName"]), drPr: await _httpClient.DashboardDetail.GetParams(itemClickedId));
					foreach (IDashboardDataSource dataSource2 in dash.DataSources)
					{
						if (dataSource2 is DashboardObjectDataSource)
						{
							((DashboardObjectDataSource)dataSource2).DataSource = dataTable4.Copy();
						}
					}
				}
			}
			_dashboard = dash;
			DataTable dataSource = await _httpClient.DashboardDetail.GetParams(id);
			grd_UdrParams.DataSource = dataSource;
			grd_UdrParams.RefreshDataSource();
			txt_Caption.Focus();
			FormMode = Enums.FormViewModeType.Update;
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
	}

	private async void ctm_Edit_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			int id = Function.ParseInt(titleView_Main.GetFocusedRowCellValue("Id"));
			await ShowEditDashboardPage(id);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DashboardManager));
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.col_Layout = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_Name = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.grd_Main = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_NewDashboard = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Edit = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.ctm_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.titleView_Main = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.col_Category = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.npage_Main = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.txt_GroupText = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_Search = new Apzon.Forms.UserControls.AutoCompleteTextBox();
		this.btn_NewCat = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NewDashboard = new Apzon.Controls.Controls.SimpleButtonEx();
		this.npage_Viewer = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.btn_Edit = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_DashboardName = new DevExpress.XtraEditors.LabelControl();
		this.btn_EditDesign = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Back = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ucDashboardViewer1 = new Apzon.Forms.Widget.UcDashboardViewer();
		this.npage_NewDashboard = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.lbl_DashboardCaption = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditDesignDashboard = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Back2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.look_Category = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Source = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_X = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_BrowseFile = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Query = new Apzon.Controls.Controls.XtraTabPageEx();
		this.memo_DataString = new DevExpress.XtraEditors.MemoExEdit();
		this.bte_DataFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.look_DataFrom = new DevExpress.XtraEditors.LookUpEdit();
		this.lbl_DataFrom = new Apzon.Controls.Controls.LabelLine();
		this.tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_UdrParams = new Apzon.Controls.Grid.GridControlEx();
		this.gv_UdrParams = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UdrParamDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_RptString = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Form = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.rdg_Source = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Caption = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Main).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.titleView_Main).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.npage_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_GroupText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		this.npage_Viewer.SuspendLayout();
		this.npage_NewDashboard.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Source).BeginInit();
		this.tab_Source.SuspendLayout();
		this.tab_X.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_BrowseFile.Properties).BeginInit();
		this.tab_Query.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memo_DataString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DataFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_DataFrom.Properties).BeginInit();
		this.tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_UdrParams).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UdrParams).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UdrParamDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RptString.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Source.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).BeginInit();
		base.SuspendLayout();
		this.col_Layout.Caption = "Layout";
		this.col_Layout.FieldName = "Layout";
		this.col_Layout.Name = "col_Layout";
		this.col_Layout.Visible = true;
		this.col_Layout.VisibleIndex = 0;
		this.col_Name.Caption = "Name";
		this.col_Name.FieldName = "Name";
		this.col_Name.Name = "col_Name";
		this.col_Name.Visible = true;
		this.col_Name.VisibleIndex = 1;
		this.grd_Main.AllowPasteExcel = false;
		this.grd_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Main.ClipboardData = null;
		this.grd_Main.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Main.Location = new System.Drawing.Point(0, 54);
		this.grd_Main.MainView = this.titleView_Main;
		this.grd_Main.Name = "grd_Main";
		this.grd_Main.Size = new System.Drawing.Size(884, 557);
		this.grd_Main.TabIndex = 3;
		this.grd_Main.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.titleView_Main });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.ctm_NewDashboard, this.ctm_Edit, this.toolStripSeparator1, this.ctm_Remove });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(196, 76);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.ctm_NewDashboard.Image = (System.Drawing.Image)resources.GetObject("ctm_NewDashboard.Image");
		this.ctm_NewDashboard.Name = "ctm_NewDashboard";
		this.ctm_NewDashboard.Size = new System.Drawing.Size(195, 22);
		this.ctm_NewDashboard.Text = "Create New Dashboard";
		this.ctm_NewDashboard.Click += new System.EventHandler(this.ctm_NewDashboard_Click);
		this.ctm_Edit.Image = (System.Drawing.Image)resources.GetObject("ctm_Edit.Image");
		this.ctm_Edit.Name = "ctm_Edit";
		this.ctm_Edit.Size = new System.Drawing.Size(195, 22);
		this.ctm_Edit.Text = "Edit";
		this.ctm_Edit.Click += new System.EventHandler(this.ctm_Edit_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
		this.ctm_Remove.Image = (System.Drawing.Image)resources.GetObject("ctm_Remove.Image");
		this.ctm_Remove.Name = "ctm_Remove";
		this.ctm_Remove.Size = new System.Drawing.Size(195, 22);
		this.ctm_Remove.Text = "Remove";
		this.ctm_Remove.Click += new System.EventHandler(this.ctm_Remove_Click);
		this.titleView_Main.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.titleView_Main.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.col_Layout, this.col_Name, this.col_Category });
		this.titleView_Main.ColumnSet.GroupColumn = this.col_Category;
		this.titleView_Main.GridControl = this.grd_Main;
		this.titleView_Main.Images = this.imageCollection1;
		this.titleView_Main.Name = "titleView_Main";
		this.titleView_Main.OptionsBehavior.AllowSmoothScrolling = true;
		this.titleView_Main.OptionsDragDrop.AllowDrag = true;
		this.titleView_Main.OptionsTiles.AllowItemHover = true;
		this.titleView_Main.OptionsTiles.ColumnCount = 4;
		this.titleView_Main.OptionsTiles.HighlightFocusedTileOnGridLoad = true;
		this.titleView_Main.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.titleView_Main.OptionsTiles.ItemBorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Never;
		this.titleView_Main.OptionsTiles.ItemSize = new System.Drawing.Size(175, 175);
		this.titleView_Main.OptionsTiles.RowCount = 4;
		this.titleView_Main.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.col_Category, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition.Length.Value = 163.0;
		this.titleView_Main.TileColumns.Add(tableColumnDefinition);
		tableRowDefinition.Length.Value = 120.0;
		tableRowDefinition2.Length.Value = 36.0;
		this.titleView_Main.TileRows.Add(tableRowDefinition);
		this.titleView_Main.TileRows.Add(tableRowDefinition2);
		tileViewItemElement.Column = this.col_Layout;
		tileViewItemElement.Text = "col_Image";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.TextVisible = false;
		tileViewItemElement2.Column = this.col_Name;
		tileViewItemElement2.RowIndex = 1;
		tileViewItemElement2.Text = "col_Name";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.titleView_Main.TileTemplate.Add(tileViewItemElement);
		this.titleView_Main.TileTemplate.Add(tileViewItemElement2);
		this.titleView_Main.BeforeItemDrag += new DevExpress.XtraGrid.Views.Tile.TileViewBeforeItemDragEventHandler(this.titleView_Main_BeforeItemDrag);
		this.titleView_Main.ItemDrop += new DevExpress.XtraGrid.Views.Tile.TileViewItemDropEventHandler(this.titleView_Main_ItemDrop);
		this.titleView_Main.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.titleView_Main_ItemClick);
		this.titleView_Main.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.titleView_Main_ItemCustomize);
		this.titleView_Main.ShownEditor += new System.EventHandler(this.titleView_Main_ShownEditor);
		this.titleView_Main.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.titleView_Main_CustomColumnDisplayText);
		this.titleView_Main.MouseUp += new System.Windows.Forms.MouseEventHandler(this.titleView_Main_MouseUp);
		this.col_Category.Caption = "Category";
		this.col_Category.FieldName = "CatId";
		this.col_Category.Name = "col_Category";
		this.col_Category.Visible = true;
		this.col_Category.VisibleIndex = 2;
		this.imageCollection1.ImageSize = new System.Drawing.Size(100, 100);
		this.imageCollection1.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imageCollection1.ImageStream");
		this.imageCollection1.Images.SetKeyName(0, "icon-grid");
		this.imageCollection1.Images.SetKeyName(1, "icon-chart");
		this.imageCollection1.Images.SetKeyName(2, "icon-scatter");
		this.imageCollection1.Images.SetKeyName(3, "icon-pies");
		this.imageCollection1.Images.SetKeyName(4, "icon-gauges");
		this.imageCollection1.Images.SetKeyName(5, "icon-card");
		this.imageCollection1.Images.SetKeyName(6, "icon-map");
		this.navigationFrame1.Controls.Add(this.npage_Main);
		this.navigationFrame1.Controls.Add(this.npage_Viewer);
		this.navigationFrame1.Controls.Add(this.npage_NewDashboard);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[3] { this.npage_Main, this.npage_Viewer, this.npage_NewDashboard });
		this.navigationFrame1.SelectedPage = this.npage_Main;
		this.navigationFrame1.Size = new System.Drawing.Size(884, 611);
		this.navigationFrame1.TabIndex = 6;
		this.navigationFrame1.Text = "navigationFrame1";
		this.navigationFrame1.TransitionAnimationProperties.FrameCount = 500;
		this.navigationFrame1.TransitionAnimationProperties.FrameInterval = 500;
		this.npage_Main.Controls.Add(this.txt_GroupText);
		this.npage_Main.Controls.Add(this.txt_Search);
		this.npage_Main.Controls.Add(this.btn_NewCat);
		this.npage_Main.Controls.Add(this.btn_NewDashboard);
		this.npage_Main.Controls.Add(this.grd_Main);
		this.npage_Main.Name = "npage_Main";
		this.npage_Main.Size = new System.Drawing.Size(884, 611);
		this.txt_GroupText.EditControl = null;
		this.txt_GroupText.FieldName = null;
		this.txt_GroupText.IsSearchControl = false;
		this.txt_GroupText.Location = new System.Drawing.Point(300, 10);
		this.txt_GroupText.Name = "txt_GroupText";
		this.txt_GroupText.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18f);
		this.txt_GroupText.Properties.Appearance.Options.UseFont = true;
		this.txt_GroupText.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_GroupText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Remove this category", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_GroupText.SettingInVisible = false;
		this.txt_GroupText.SettingReadonly = false;
		this.txt_GroupText.Size = new System.Drawing.Size(201, 38);
		this.txt_GroupText.TabIndex = 15;
		this.txt_GroupText.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_GroupText_ButtonClick);
		this.txt_GroupText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_GroupText_KeyDown);
		this.txt_GroupText.Leave += new System.EventHandler(this.txt_GroupText_Leave);
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(12, 12);
		this.txt_Search.Name = "txt_Search";
		this.txt_Search.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.txt_Search.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14f);
		this.txt_Search.Properties.Appearance.Options.UseBackColor = true;
		this.txt_Search.Properties.Appearance.Options.UseFont = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Search.Properties.NullText = "Search";
		this.txt_Search.Size = new System.Drawing.Size(282, 30);
		this.txt_Search.TabIndex = 14;
		this.txt_Search.Values = null;
		this.txt_Search.EditValueChanged += new System.EventHandler(this.txt_Search_EditValueChanged);
		this.btn_NewCat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewCat.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_NewCat.EditControl = null;
		this.btn_NewCat.EditValue = "New Category";
		this.btn_NewCat.FieldName = null;
		this.btn_NewCat.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewCat.ImageOptions.Image");
		this.btn_NewCat.IsSearchControl = false;
		this.btn_NewCat.Location = new System.Drawing.Point(627, 6);
		this.btn_NewCat.Name = "btn_NewCat";
		this.btn_NewCat.ReadOnly = false;
		this.btn_NewCat.SettingInVisible = false;
		this.btn_NewCat.SettingReadonly = false;
		this.btn_NewCat.Size = new System.Drawing.Size(117, 42);
		this.btn_NewCat.TabIndex = 6;
		this.btn_NewCat.Text = "New Category";
		this.btn_NewCat.ToolTip = "Back to List of Dashboards";
		this.btn_NewCat.Click += new System.EventHandler(this.btn_NewCat_Click);
		this.btn_NewDashboard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewDashboard.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_NewDashboard.EditControl = null;
		this.btn_NewDashboard.EditValue = "New Dashboard";
		this.btn_NewDashboard.FieldName = null;
		this.btn_NewDashboard.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewDashboard.ImageOptions.Image");
		this.btn_NewDashboard.IsSearchControl = false;
		this.btn_NewDashboard.Location = new System.Drawing.Point(750, 6);
		this.btn_NewDashboard.Name = "btn_NewDashboard";
		this.btn_NewDashboard.ReadOnly = false;
		this.btn_NewDashboard.SettingInVisible = false;
		this.btn_NewDashboard.SettingReadonly = false;
		this.btn_NewDashboard.Size = new System.Drawing.Size(131, 42);
		this.btn_NewDashboard.TabIndex = 5;
		this.btn_NewDashboard.Text = "New Dashboard";
		this.btn_NewDashboard.ToolTip = "Back to List of Dashboards";
		this.btn_NewDashboard.Click += new System.EventHandler(this.btn_NewDashboard_Click);
		this.npage_Viewer.Caption = "npage_Viewer";
		this.npage_Viewer.Controls.Add(this.btn_Edit);
		this.npage_Viewer.Controls.Add(this.lbl_DashboardName);
		this.npage_Viewer.Controls.Add(this.btn_EditDesign);
		this.npage_Viewer.Controls.Add(this.btn_Back);
		this.npage_Viewer.Controls.Add(this.ucDashboardViewer1);
		this.npage_Viewer.Name = "npage_Viewer";
		this.npage_Viewer.Size = new System.Drawing.Size(884, 611);
		this.btn_Edit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Edit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Edit.EditControl = null;
		this.btn_Edit.EditValue = "Edit";
		this.btn_Edit.FieldName = null;
		this.btn_Edit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Edit.ImageOptions.Image");
		this.btn_Edit.IsSearchControl = false;
		this.btn_Edit.Location = new System.Drawing.Point(636, 3);
		this.btn_Edit.Name = "btn_Edit";
		this.btn_Edit.ReadOnly = false;
		this.btn_Edit.SettingInVisible = false;
		this.btn_Edit.SettingReadonly = false;
		this.btn_Edit.Size = new System.Drawing.Size(117, 42);
		this.btn_Edit.TabIndex = 4;
		this.btn_Edit.Text = "Edit";
		this.btn_Edit.ToolTip = "Back to List of Dashboards";
		this.btn_Edit.Click += new System.EventHandler(this.Btn_Edit_Click_1);
		this.lbl_DashboardName.Appearance.Font = new System.Drawing.Font("Tahoma", 12f);
		this.lbl_DashboardName.Appearance.ForeColor = System.Drawing.Color.FromArgb(0, 0, 64);
		this.lbl_DashboardName.Appearance.Options.UseFont = true;
		this.lbl_DashboardName.Appearance.Options.UseForeColor = true;
		this.lbl_DashboardName.Location = new System.Drawing.Point(70, 20);
		this.lbl_DashboardName.Name = "lbl_DashboardName";
		this.lbl_DashboardName.Size = new System.Drawing.Size(113, 19);
		this.lbl_DashboardName.TabIndex = 3;
		this.lbl_DashboardName.Text = "Unknown Name";
		this.btn_EditDesign.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditDesign.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_EditDesign.EditControl = null;
		this.btn_EditDesign.EditValue = "Edit Design";
		this.btn_EditDesign.FieldName = null;
		this.btn_EditDesign.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditDesign.ImageOptions.Image");
		this.btn_EditDesign.IsSearchControl = false;
		this.btn_EditDesign.Location = new System.Drawing.Point(759, 6);
		this.btn_EditDesign.Name = "btn_EditDesign";
		this.btn_EditDesign.ReadOnly = false;
		this.btn_EditDesign.SettingInVisible = false;
		this.btn_EditDesign.SettingReadonly = false;
		this.btn_EditDesign.Size = new System.Drawing.Size(117, 42);
		this.btn_EditDesign.TabIndex = 2;
		this.btn_EditDesign.Text = "Edit Design";
		this.btn_EditDesign.ToolTip = "Back to List of Dashboards";
		this.btn_EditDesign.Click += new System.EventHandler(this.btn_Edit_Click);
		this.btn_Back.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Back.EditControl = null;
		this.btn_Back.EditValue = "";
		this.btn_Back.FieldName = null;
		this.btn_Back.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Back.ImageOptions.Image");
		this.btn_Back.IsSearchControl = false;
		this.btn_Back.Location = new System.Drawing.Point(10, 6);
		this.btn_Back.Name = "btn_Back";
		this.btn_Back.ReadOnly = false;
		this.btn_Back.SettingInVisible = false;
		this.btn_Back.SettingReadonly = false;
		this.btn_Back.Size = new System.Drawing.Size(45, 42);
		this.btn_Back.TabIndex = 1;
		this.btn_Back.ToolTip = "Back to List of Dashboards";
		this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
		this.ucDashboardViewer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucDashboardViewer1.DashboardName = null;
		this.ucDashboardViewer1.Location = new System.Drawing.Point(3, 50);
		this.ucDashboardViewer1.Name = "ucDashboardViewer1";
		this.ucDashboardViewer1.Size = new System.Drawing.Size(878, 558);
		this.ucDashboardViewer1.TabIndex = 0;
		this.npage_NewDashboard.Caption = "npage_NewDashboard";
		this.npage_NewDashboard.Controls.Add(this.lbl_DashboardCaption);
		this.npage_NewDashboard.Controls.Add(this.btn_Add);
		this.npage_NewDashboard.Controls.Add(this.btn_EditDesignDashboard);
		this.npage_NewDashboard.Controls.Add(this.btn_Back2);
		this.npage_NewDashboard.Controls.Add(this.groupControl1);
		this.npage_NewDashboard.Name = "npage_NewDashboard";
		this.npage_NewDashboard.Size = new System.Drawing.Size(884, 611);
		this.lbl_DashboardCaption.Appearance.Font = new System.Drawing.Font("Tahoma", 12.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_DashboardCaption.Appearance.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.lbl_DashboardCaption.Appearance.Options.UseFont = true;
		this.lbl_DashboardCaption.Appearance.Options.UseForeColor = true;
		this.lbl_DashboardCaption.EditControl = null;
		this.lbl_DashboardCaption.EditValue = "";
		this.lbl_DashboardCaption.FieldName = null;
		this.lbl_DashboardCaption.Location = new System.Drawing.Point(65, 16);
		this.lbl_DashboardCaption.Name = "lbl_DashboardCaption";
		this.lbl_DashboardCaption.ReadOnly = false;
		this.lbl_DashboardCaption.SettingInVisible = false;
		this.lbl_DashboardCaption.SettingReadonly = false;
		this.lbl_DashboardCaption.Size = new System.Drawing.Size(0, 21);
		this.lbl_DashboardCaption.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DashboardCaption.TabIndex = 6;
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Add.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Add.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "Save";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(631, 5);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(117, 42);
		this.btn_Add.TabIndex = 5;
		this.btn_Add.Text = "Save";
		this.btn_Add.ToolTip = "Save changes";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_EditDesignDashboard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditDesignDashboard.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_EditDesignDashboard.EditControl = null;
		this.btn_EditDesignDashboard.EditValue = "Edit Design";
		this.btn_EditDesignDashboard.FieldName = null;
		this.btn_EditDesignDashboard.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditDesignDashboard.ImageOptions.Image");
		this.btn_EditDesignDashboard.IsSearchControl = false;
		this.btn_EditDesignDashboard.Location = new System.Drawing.Point(754, 5);
		this.btn_EditDesignDashboard.Name = "btn_EditDesignDashboard";
		this.btn_EditDesignDashboard.ReadOnly = false;
		this.btn_EditDesignDashboard.SettingInVisible = false;
		this.btn_EditDesignDashboard.SettingReadonly = false;
		this.btn_EditDesignDashboard.Size = new System.Drawing.Size(117, 42);
		this.btn_EditDesignDashboard.TabIndex = 4;
		this.btn_EditDesignDashboard.Text = "Edit Design";
		this.btn_EditDesignDashboard.ToolTip = "Edit Design";
		this.btn_EditDesignDashboard.Click += new System.EventHandler(this.btn_EditDesignDashboard_Click);
		this.btn_Back2.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Back2.EditControl = null;
		this.btn_Back2.EditValue = "";
		this.btn_Back2.FieldName = null;
		this.btn_Back2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Back2.ImageOptions.Image");
		this.btn_Back2.IsSearchControl = false;
		this.btn_Back2.Location = new System.Drawing.Point(5, 5);
		this.btn_Back2.Name = "btn_Back2";
		this.btn_Back2.ReadOnly = false;
		this.btn_Back2.SettingInVisible = false;
		this.btn_Back2.SettingReadonly = false;
		this.btn_Back2.Size = new System.Drawing.Size(45, 42);
		this.btn_Back2.TabIndex = 3;
		this.btn_Back2.ToolTip = "Back to List of Dashboards";
		this.btn_Back2.Click += new System.EventHandler(this.btn_Back2_Click);
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.groupControl1.Appearance.Options.UseBackColor = true;
		this.groupControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.groupControl1.Controls.Add(this.look_Category);
		this.groupControl1.Controls.Add(this.labelLine5);
		this.groupControl1.Controls.Add(this.tab_Source);
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.rdg_Source);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.txt_Caption);
		this.groupControl1.Location = new System.Drawing.Point(0, 53);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(884, 558);
		this.groupControl1.TabIndex = 2;
		this.groupControl1.Text = "New Dashboard - Information";
		this.look_Category.Location = new System.Drawing.Point(388, 29);
		this.look_Category.Name = "look_Category";
		this.look_Category.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Category.Properties.DisplayMember = "Name";
		this.look_Category.Properties.NullText = "";
		this.look_Category.Properties.ValueMember = "Id";
		this.look_Category.Size = new System.Drawing.Size(211, 20);
		this.look_Category.TabIndex = 6;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Category";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(308, 33);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 5;
		this.labelLine5.Utext = "Category";
		this.tab_Source.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Source.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.tab_Source.Appearance.Options.UseBackColor = true;
		this.tab_Source.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tab_Source.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tab_Source.IsSearchControl = false;
		this.tab_Source.Location = new System.Drawing.Point(0, 142);
		this.tab_Source.Name = "tab_Source";
		this.tab_Source.SelectedTabPage = this.tab_X;
		this.tab_Source.Size = new System.Drawing.Size(884, 416);
		this.tab_Source.TabIndex = 4;
		this.tab_Source.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tab_X, this.tab_Query, this.tab_Report, this.tab_Form });
		this.tab_X.Controls.Add(this.btn_BrowseFile);
		this.tab_X.Controls.Add(this.labelLine3);
		this.tab_X.EditControl = null;
		this.tab_X.EditValue = "XML";
		this.tab_X.FieldName = null;
		this.tab_X.IsSearchControl = false;
		this.tab_X.Name = "tab_X";
		this.tab_X.ReadOnly = false;
		this.tab_X.SettingInVisible = false;
		this.tab_X.SettingReadonly = false;
		this.tab_X.Size = new System.Drawing.Size(882, 391);
		this.tab_X.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_X.Text = "XML";
		this.btn_BrowseFile.EditControl = null;
		this.btn_BrowseFile.FieldName = null;
		this.btn_BrowseFile.IsSearchControl = false;
		this.btn_BrowseFile.Location = new System.Drawing.Point(88, 4);
		this.btn_BrowseFile.Name = "btn_BrowseFile";
		this.btn_BrowseFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.btn_BrowseFile.SettingInVisible = false;
		this.btn_BrowseFile.SettingReadonly = false;
		this.btn_BrowseFile.Size = new System.Drawing.Size(337, 20);
		this.btn_BrowseFile.TabIndex = 1;
		this.btn_BrowseFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_BrowseFile_ButtonClick);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Browse file";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Browse file";
		this.tab_Query.Controls.Add(this.memo_DataString);
		this.tab_Query.Controls.Add(this.bte_DataFrom);
		this.tab_Query.Controls.Add(this.look_DataFrom);
		this.tab_Query.Controls.Add(this.lbl_DataFrom);
		this.tab_Query.EditControl = null;
		this.tab_Query.EditValue = "Query";
		this.tab_Query.FieldName = null;
		this.tab_Query.IsSearchControl = false;
		this.tab_Query.Name = "tab_Query";
		this.tab_Query.ReadOnly = false;
		this.tab_Query.SettingInVisible = false;
		this.tab_Query.SettingReadonly = false;
		this.tab_Query.Size = new System.Drawing.Size(882, 391);
		this.tab_Query.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Query.Text = "Query";
		this.memo_DataString.Location = new System.Drawing.Point(428, 3);
		this.memo_DataString.Name = "memo_DataString";
		this.memo_DataString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_DataString.Properties.ShowIcon = false;
		this.memo_DataString.Size = new System.Drawing.Size(230, 20);
		this.memo_DataString.TabIndex = 7;
		this.memo_DataString.Visible = false;
		this.bte_DataFrom.EditControl = null;
		this.bte_DataFrom.FieldName = null;
		this.bte_DataFrom.IsSearchControl = false;
		this.bte_DataFrom.Location = new System.Drawing.Point(192, 3);
		this.bte_DataFrom.Name = "bte_DataFrom";
		this.bte_DataFrom.Properties.AutoLoadDataSource = true;
		this.bte_DataFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_DataFrom.Properties.ContainerControl = null;
		this.bte_DataFrom.Properties.DisplayMember = "";
		this.bte_DataFrom.Properties.DisplayMember2 = "";
		this.bte_DataFrom.Properties.LoadDataSourceWhenEnter = false;
		this.bte_DataFrom.Properties.SeparateCharacterForMulti = ';';
		this.bte_DataFrom.Properties.SeparateString = ", ";
		this.bte_DataFrom.Properties.UsingAutoCompleteBox = false;
		this.bte_DataFrom.Properties.UsingDataSource = true;
		this.bte_DataFrom.Properties.ValidateWhenNoDataSource = false;
		this.bte_DataFrom.Properties.ValueMember = "";
		this.bte_DataFrom.SettingInVisible = false;
		this.bte_DataFrom.SettingReadonly = false;
		this.bte_DataFrom.Size = new System.Drawing.Size(230, 20);
		this.bte_DataFrom.TabIndex = 5;
		this.look_DataFrom.Location = new System.Drawing.Point(86, 3);
		this.look_DataFrom.Name = "look_DataFrom";
		this.look_DataFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_DataFrom.Properties.NullText = "";
		this.look_DataFrom.Size = new System.Drawing.Size(100, 20);
		this.look_DataFrom.TabIndex = 4;
		this.look_DataFrom.EditValueChanged += new System.EventHandler(this.look_DataFrom_EditValueChanged);
		this.lbl_DataFrom.EditControl = null;
		this.lbl_DataFrom.EditValue = "Data From";
		this.lbl_DataFrom.FieldName = null;
		this.lbl_DataFrom.HideWhenEmpty = false;
		this.lbl_DataFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DataFrom.Location = new System.Drawing.Point(9, 6);
		this.lbl_DataFrom.Name = "lbl_DataFrom";
		this.lbl_DataFrom.ReadOnly = false;
		this.lbl_DataFrom.SettingInVisible = false;
		this.lbl_DataFrom.SettingReadonly = false;
		this.lbl_DataFrom.ShowUnderLine = true;
		this.lbl_DataFrom.Size = new System.Drawing.Size(80, 20);
		this.lbl_DataFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DataFrom.TabIndex = 3;
		this.lbl_DataFrom.Utext = "Data From";
		this.tab_Report.Controls.Add(this.grd_UdrParams);
		this.tab_Report.Controls.Add(this.bte_RptString);
		this.tab_Report.Controls.Add(this.labelLine4);
		this.tab_Report.EditControl = null;
		this.tab_Report.EditValue = "Report";
		this.tab_Report.FieldName = null;
		this.tab_Report.IsSearchControl = false;
		this.tab_Report.Name = "tab_Report";
		this.tab_Report.ReadOnly = false;
		this.tab_Report.SettingInVisible = false;
		this.tab_Report.SettingReadonly = false;
		this.tab_Report.Size = new System.Drawing.Size(882, 391);
		this.tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Report.Text = "Report";
		this.grd_UdrParams.AllowPasteExcel = false;
		this.grd_UdrParams.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_UdrParams.ClipboardData = null;
		this.grd_UdrParams.Location = new System.Drawing.Point(9, 27);
		this.grd_UdrParams.MainView = this.gv_UdrParams;
		this.grd_UdrParams.Name = "grd_UdrParams";
		this.grd_UdrParams.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_UdrParamDataType });
		this.grd_UdrParams.Size = new System.Drawing.Size(867, 325);
		this.grd_UdrParams.TabIndex = 7;
		this.grd_UdrParams.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_UdrParams });
		this.gv_UdrParams.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_UdrParams.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_UdrParams.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_UdrParams.ColumnKey = null;
		this.gv_UdrParams.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx11, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx10, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8,
			this.gridColumnEx9, this.gridColumnEx12, this.gridColumnEx13
		});
		this.gv_UdrParams.DisableFormSetting = false;
		this.gv_UdrParams.DisplayException = false;
		this.gv_UdrParams.EditColumn = null;
		this.gv_UdrParams.FieldCurrency = null;
		this.gv_UdrParams.FieldRate = null;
		this.gv_UdrParams.FormId = "oDasdboardManager";
		this.gv_UdrParams.GridControl = this.grd_UdrParams;
		this.gv_UdrParams.IgnoreFilter = false;
		this.gv_UdrParams.IndexEditRow = -1;
		this.gv_UdrParams.IndicatorWidth = 35;
		this.gv_UdrParams.IsApzTable = false;
		this.gv_UdrParams.IsInsertNewRow = false;
		this.gv_UdrParams.IsLoadingData = false;
		this.gv_UdrParams.IsMappingContextAction = false;
		this.gv_UdrParams.ItemId = "OPARAMATER";
		this.gv_UdrParams.LstColumnAllowBatch = null;
		this.gv_UdrParams.LstColumnNotAllowBatch = null;
		this.gv_UdrParams.LstColumnNotEditable = null;
		this.gv_UdrParams.LstColumNotSetting = null;
		this.gv_UdrParams.Name = "gv_UdrParams";
		this.gv_UdrParams.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_UdrParams.OptionsPrint.AutoWidth = false;
		this.gv_UdrParams.OptionsView.BestFitMaxRowCount = 100;
		this.gv_UdrParams.OptionsView.ColumnAutoWidth = false;
		this.gv_UdrParams.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_UdrParams.OptionsView.ShowGroupPanel = false;
		this.gv_UdrParams.OptionsView.ShowViewCaption = true;
		this.gv_UdrParams.UseBatchMod = false;
		this.gv_UdrParams.ViewCaption = "Parameters";
		this.gv_UdrParams.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_UdrParams_RowCellStyle);
		this.gridColumnEx1.Caption = "Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Width = 100;
		this.gridColumnEx2.Caption = "LineNum";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "LineNum";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Width = 100;
		this.gridColumnEx11.Caption = "Param Name";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ParamName";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 0;
		this.gridColumnEx11.Width = 134;
		this.gridColumnEx3.Caption = "Label";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Label";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx3.Width = 125;
		this.gridColumnEx4.Caption = "Type";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Type";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 134;
		this.gridColumnEx10.Caption = "DataType";
		this.gridColumnEx10.ColumnEdit = this.rep_UdrParamDataType;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 3;
		this.gridColumnEx10.Width = 128;
		this.rep_UdrParamDataType.AutoHeight = false;
		this.rep_UdrParamDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UdrParamDataType.Name = "rep_UdrParamDataType";
		this.rep_UdrParamDataType.NullText = "";
		this.gridColumnEx5.Caption = "Format String";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "FormatString";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 100;
		this.gridColumnEx6.Caption = "SubType";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SubType";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Width = 100;
		this.gridColumnEx7.Caption = "Default Value";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DefaultValue";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 5;
		this.gridColumnEx7.Width = 147;
		this.gridColumnEx8.Caption = "ToolTip";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ToolTip";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Width = 100;
		this.gridColumnEx9.Caption = "AllowMultiple";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "AllowMultiple";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Width = 100;
		this.gridColumnEx12.Caption = "Visible";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Visible";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.OptionsColumn.ReadOnly = true;
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Width = 100;
		this.gridColumnEx13.Caption = "AllowEdit";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "AllowEdit";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Width = 100;
		this.bte_RptString.EditControl = null;
		this.bte_RptString.FieldName = null;
		this.bte_RptString.IsSearchControl = false;
		this.bte_RptString.Location = new System.Drawing.Point(90, 3);
		this.bte_RptString.Name = "bte_RptString";
		this.bte_RptString.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.bte_RptString.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_RptString.Properties.ContainerControl = null;
		this.bte_RptString.Properties.DisplayMember = "";
		this.bte_RptString.Properties.DisplayMember2 = "";
		this.bte_RptString.Properties.LoadDataSourceWhenEnter = false;
		this.bte_RptString.Properties.SeparateCharacterForMulti = ';';
		this.bte_RptString.Properties.SeparateString = ", ";
		this.bte_RptString.Properties.UsingAutoCompleteBox = false;
		this.bte_RptString.Properties.UsingDataSource = true;
		this.bte_RptString.Properties.ValidateWhenNoDataSource = false;
		this.bte_RptString.Properties.ValueMember = "";
		this.bte_RptString.SettingInVisible = false;
		this.bte_RptString.SettingReadonly = false;
		this.bte_RptString.Size = new System.Drawing.Size(230, 20);
		this.bte_RptString.TabIndex = 6;
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Report Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(9, 6);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 4;
		this.labelLine4.Utext = "Report Name";
		this.tab_Form.EditControl = null;
		this.tab_Form.EditValue = "Forms";
		this.tab_Form.FieldName = null;
		this.tab_Form.IsSearchControl = false;
		this.tab_Form.Name = "tab_Form";
		this.tab_Form.ReadOnly = false;
		this.tab_Form.SettingInVisible = false;
		this.tab_Form.SettingReadonly = false;
		this.tab_Form.Size = new System.Drawing.Size(882, 391);
		this.tab_Form.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Form.Text = "Forms";
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Source";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(10, 59);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "Source";
		this.rdg_Source.EditControl = null;
		this.rdg_Source.EditValue = "X";
		this.rdg_Source.FieldName = null;
		this.rdg_Source.IsSearchControl = false;
		this.rdg_Source.Location = new System.Drawing.Point(91, 55);
		this.rdg_Source.Name = "rdg_Source";
		this.rdg_Source.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_Source.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_Source.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_Source.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("X", "Import From Xml File"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Q", "User-Defined Query"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("R", "User-Defined Report")
		});
		this.rdg_Source.SettingInVisible = false;
		this.rdg_Source.SettingReadonly = false;
		this.rdg_Source.Size = new System.Drawing.Size(211, 81);
		this.rdg_Source.TabIndex = 2;
		this.rdg_Source.SelectedIndexChanged += new System.EventHandler(this.rdg_Source_SelectedIndexChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Caption";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 33);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Caption";
		this.txt_Caption.EditControl = null;
		this.txt_Caption.FieldName = null;
		this.txt_Caption.IsSearchControl = false;
		this.txt_Caption.Location = new System.Drawing.Point(91, 29);
		this.txt_Caption.Name = "txt_Caption";
		this.txt_Caption.SettingInVisible = false;
		this.txt_Caption.SettingReadonly = false;
		this.txt_Caption.Size = new System.Drawing.Size(211, 20);
		this.txt_Caption.TabIndex = 1;
		this.txt_Caption.EditValueChanged += new System.EventHandler(this.txt_Caption_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(884, 611);
		base.Controls.Add(this.navigationFrame1);
		base.FormId = "oDasdboardManager";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DashboardManager.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DashboardManager";
		base.ShowInTaskbar = false;
		this.Text = "Dashboard Management";
		base.Load += new System.EventHandler(this.frm_DashboardManager_Load);
		base.SizeChanged += new System.EventHandler(this.frm_DashboardManager_SizeChanged);
		((System.ComponentModel.ISupportInitialize)this.grd_Main).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.titleView_Main).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageCollection1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.npage_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_GroupText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		this.npage_Viewer.ResumeLayout(false);
		this.npage_Viewer.PerformLayout();
		this.npage_NewDashboard.ResumeLayout(false);
		this.npage_NewDashboard.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Source).EndInit();
		this.tab_Source.ResumeLayout(false);
		this.tab_X.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_BrowseFile.Properties).EndInit();
		this.tab_Query.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memo_DataString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_DataFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_DataFrom.Properties).EndInit();
		this.tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_UdrParams).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UdrParams).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UdrParamDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RptString.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Source.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
