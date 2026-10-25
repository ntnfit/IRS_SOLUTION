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
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Forms.Administrator.ApprovalProcess;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;

namespace Apzon.Forms.UserControls;

public class TileViewPagingEx : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoaded;

	private int _currentPage = 1;

	private int _pageIndex;

	private List<DataTable> _lstDataSource = new List<DataTable>();

	private string _myReqText = "My Request";

	private string _apprText = "Approval Request";

	private string _createdText = "Created";

	public DataTable _dtSourceTmp = new DataTable();

	public DataTable _dtSource = new DataTable();

	public DataTable dtMenu = new DataTable();

	public bool AllowCheckItem;

	public bool DocTotalVisible = true;

	public bool RequestTypeVisible = true;

	public bool IsSettingChanged;

	public string CurrentRequest = string.Empty;

	public List<DataRow> ItemsSelected = new List<DataRow>();

	public TileView TileView;

	public GridControlEx GridControl;

	public LookUpEditEx RequestTypeControl;

	public LookUpEditEx SearchTypeControl;

	public ButtonEditEx TextSearchControl;

	public GridColumnCollection Columns;

	private IContainer components;

	private ImageCollection img_Lists;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem mnu_TooltipSetting;

	private ToolTipController toolTipController1;

	private SimpleButtonEx btn_FirstRecord;

	private SimpleButtonEx btn_Previous;

	private SimpleButtonEx btn_Last;

	private SimpleButtonEx btn_Next;

	private LabelControlEx lbl_PageNav;

	private LookUpEditEx lk_RequestType;

	private TileView tv_Templates;

	private TileViewColumn tc_ValueCheck;

	private TileViewColumn tc_WddCode;

	private TileViewColumn tc_WtmCode;

	private TileViewColumn tc_WtmName;

	private TileViewColumn tc_DocNo;

	private TileViewColumn tc_DocType;

	private TileViewColumn tc_UserID;

	private TileViewColumn tc_UserRequest;

	private TileViewColumn tc_Status;

	private TileViewColumn tc_Remarks;

	private TileViewColumn tc_PostingDate;

	private TileViewColumn tc_PostingTime;

	private TileViewColumn tc_RequestType;

	private TileViewColumn tc_CardCode;

	private TileViewColumn tc_CardName;

	private TileViewColumn tc_DocDueDate;

	private TileViewColumn tc_DocTotal;

	private TileViewColumn tc_DocCur;

	private GridControlEx grd_Templates;

	private ButtonEditEx btn_TextSearch;

	private LookUpEditEx lk_SearchType;

	private TablePanel tablePanel1;

	private RepositoryItemCheckEdit rep_ItmCheck;

	private TileViewColumn tc_DraftNo;

	private TileViewColumn tc_DraftType;

	private TileViewColumn tc_ApprovalReq;

	private TileViewColumn tc_MyReq;

	private TileViewColumn tc_Created;

	private TileViewColumn tc_Pending;

	private TileViewColumn tc_Approved;

	private TileViewColumn tc_Rejected;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	public TileViewPagingEx()
	{
		InitializeComponent();
		InitializeControl();
	}

	public void InitializeControl()
	{
		try
		{
			lk_SearchType.Properties.DataSource = new Dictionary<int, string>
			{
				{ 0, "User" },
				{ 1, "Draft No" },
				{ 3, "Request Type" }
			};
			lk_RequestType.Properties.DataSource = new Dictionary<string, string>
			{
				{ "P", "Approval Request" },
				{ "M", "My Request" }
			};
			TileView = tv_Templates;
			GridControl = grd_Templates;
			lk_SearchType.ItemIndex = 0;
			lk_RequestType.ItemIndex = 0;
			RequestTypeControl = lk_RequestType;
			SearchTypeControl = lk_SearchType;
			TextSearchControl = btn_TextSearch;
			Columns = tv_Templates.Columns;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GenerateSourceToList(int splitPageNum)
	{
		try
		{
			_lstDataSource = new List<DataTable>();
			for (int i = 0; i < splitPageNum; i++)
			{
				if (GridControl.DataSourceTable.Rows.Count == 0)
				{
					break;
				}
				DataTable dataTable = GridControl.DataSourceTable.Copy().Clone();
				for (int j = 0; j < splitPageNum; j++)
				{
					if (j < GridControl.DataSourceTable.Rows.Count)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow.ItemArray = GridControl.DataSourceTable.Rows[j].ItemArray;
						GridControl.DataSourceTable.Rows[j]["Cloned"] = "Y";
						dataTable.Rows.Add(dataRow);
					}
				}
				DataRow[] array = GridControl.DataSourceTable.Select("Cloned = 'Y'");
				foreach (DataRow row in array)
				{
					GridControl.DataSourceTable.Rows.Remove(row);
				}
				GridControl.RefreshDataSource();
				_lstDataSource.Add(dataTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SplitDocumentPage(int splitPageNum)
	{
		try
		{
			if (!GridControl.DataSourceTable.IsNotNull())
			{
				_lstDataSource = new List<DataTable>();
				ShowPagingInfo(GridControl.DataSourceTable, "F");
				return;
			}
			if (!GridControl.DataSourceTable.Columns.Contains("Cloned"))
			{
				GridControl.DataSourceTable.Columns.Add("Cloned", typeof(string));
			}
			GenerateSourceToList(splitPageNum);
			if (_lstDataSource.Count > 0)
			{
				GridControl.DataSource = _lstDataSource[0];
				GridControl.RefreshDataSource();
				ShowPagingInfo(GridControl.DataSourceTable, "F");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowPagingInfo(DataTable dtSource, string type)
	{
		int num = 0;
		if (!dtSource.IsNotNull())
		{
			_pageIndex = 0;
			num = 1;
		}
		else
		{
			switch (type)
			{
			case "F":
				_pageIndex = 0;
				_currentPage = 1;
				break;
			case "P":
				_pageIndex--;
				_currentPage--;
				break;
			case "N":
				_pageIndex++;
				_currentPage++;
				break;
			case "L":
				_pageIndex = _lstDataSource.Count - 1;
				_currentPage = _lstDataSource.Count;
				break;
			}
			num = _lstDataSource.Count;
		}
		lbl_PageNav.Text = "Page " + _currentPage + " of " + num;
	}

	public void SettingMenuData(DataTable dtSource, DataTable dtMenu)
	{
		try
		{
			if (!dtSource.IsNotNull() || !dtMenu.IsNotNull())
			{
				return;
			}
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (DataRow row in dtMenu.Rows)
			{
				if (Function.ToString(row["MnuType"]).Equals("P"))
				{
					text = text + "'" + Function.ToString(row["DocType"]) + "',";
				}
				else
				{
					text2 = text2 + "'" + Function.ToString(row["DocType"]) + "',";
				}
			}
			string filterExpression = string.Empty;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				filterExpression = $"WtmName in ({text}) and DocType in ({text2})";
			}
			else if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				filterExpression = $"WtmName in ({text})";
			}
			else if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				filterExpression = $"DocType in ({text2})";
			}
			DataTable dataSource = dtSource.Copy().Select(filterExpression).CopyToDataTable();
			SetDataSource(dataSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SettingTileViewItem(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return;
			}
			_apprText = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_ApprovalReq"))[10]);
			_myReqText = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_MyReq"))[10]);
			_createdText = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_Created"))[10]);
			string caption = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_DraftNo"))[10]);
			string caption2 = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_DocNo"))[10]);
			string caption3 = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_DraftType"))[10]);
			string caption4 = Function.ToString(dt.Copy().AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["ColumnId"]).Equals("tc_DocType"))[10]);
			foreach (DataRow row in dt.Rows)
			{
				bool flag = Function.ParseBool(row["Visible"]);
				string text = Function.ToString(row["ColumnId"]);
				string caption5 = Function.ToString(row["ColumnName"]);
				switch (text)
				{
				case "tc_CardName":
					tc_CardName.Caption = caption5;
					tc_CardName.Visible = flag;
					break;
				case "tc_Remarks":
					tc_Remarks.Caption = caption5;
					tc_Remarks.Visible = flag;
					break;
				case "tc_DraftType":
				case "tc_DocType":
					tc_DraftType.Caption = caption3;
					tc_DocType.Caption = caption4;
					tc_DocType.Visible = flag;
					tc_DraftType.Visible = flag;
					break;
				case "tc_DocNo":
					tc_DocNo.Caption = caption2;
					tc_DocNo.Visible = flag;
					break;
				case "tc_DraftNo":
					tc_DraftNo.Caption = caption;
					tc_DraftNo.Visible = flag;
					break;
				case "tc_DocDueDate":
					tc_DocDueDate.Caption = caption5;
					tc_DocDueDate.Visible = flag;
					break;
				case "tc_DocTotal":
					tc_DocTotal.Caption = caption5;
					tc_DocTotal.Visible = flag;
					tc_DocCur.Visible = flag;
					DocTotalVisible = flag;
					break;
				case "tc_PostingDate":
					tc_PostingDate.Caption = caption5;
					tc_PostingDate.Visible = flag;
					break;
				case "tc_RequestType":
					tc_RequestType.Visible = flag;
					RequestTypeVisible = flag;
					break;
				case "tc_Pending":
					tc_Pending.Caption = caption5;
					break;
				case "tc_Approved":
					tc_Approved.Caption = caption5;
					break;
				case "tc_Rejected":
					tc_Rejected.Caption = caption5;
					break;
				case "tc_UserRequest":
					tc_UserRequest.Visible = flag;
					break;
				case "tc_Status":
					tc_Status.Visible = flag;
					tc_Pending.Visible = flag;
					tc_Approved.Visible = flag;
					tc_Rejected.Visible = flag;
					break;
				}
			}
			tv_Templates.BeginUpdate();
			tv_Templates.RefreshData();
			tv_Templates.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task ShowWorkingProcessSetting(string type)
	{
		try
		{
			using frm_WorkingProcessSetting frm_WorkingProcessSetting2 = new frm_WorkingProcessSetting(type, grd_Templates.DataSourceTable.IsNotNull())
			{
				StartPosition = FormStartPosition.CenterParent
			};
			frm_WorkingProcessSetting2.ShowDialog(this);
			IsSettingChanged = frm_WorkingProcessSetting2.IsChanged;
			if (!IsSettingChanged)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable templateData = _httpClient.WorkingProcessClient.GetTemplateData(GlobalData.UserID, "A", CurrentRequest);
			DataSet setting = _httpClient.WorkingProcessClient.GetSetting(GlobalData.UserID);
			dtMenu = setting.Tables[0];
			if (!setting.Tables[0].IsNotNull() && !setting.Tables[1].IsNotNull() && !setting.Tables[2].IsNotNull())
			{
				return;
			}
			SettingMenuData(templateData, dtMenu);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public void SetDataSource(DataTable dt)
	{
		try
		{
			_lstDataSource = new List<DataTable>();
			if (!dt.IsNotNull())
			{
				GridControl.DataSource = dt;
				GridControl.RefreshDataSource();
				ShowPagingInfo(dt, "F");
			}
			else
			{
				dt.AcceptChanges();
				GridControl.DataSource = dt;
				SplitDocumentPage(15);
				GridControl.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void NavigationPage(string type)
	{
		try
		{
			if (_lstDataSource.Count == 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			switch (type)
			{
			case "F":
				GridControl.DataSource = _lstDataSource[0];
				break;
			case "P":
				if (_currentPage == 1)
				{
					WinCommons._CloseProcessing();
					return;
				}
				GridControl.DataSource = _lstDataSource[_pageIndex - 1];
				break;
			case "N":
				if (_currentPage == _lstDataSource.Count)
				{
					WinCommons._CloseProcessing();
					return;
				}
				GridControl.DataSource = _lstDataSource[_pageIndex + 1];
				break;
			case "L":
				GridControl.DataSource = _lstDataSource[_lstDataSource.Count - 1];
				break;
			}
			GridControl.RefreshDataSource();
			ShowPagingInfo(GridControl.DataSourceTable, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public bool IsDocumentCreated()
	{
		return Function.ToString(tv_Templates.GetRowCellValue(tv_Templates.FocusedRowHandle, "IsDraft")).Equals("N");
	}

	public void SetItemCheckValue(int rowHandle, string value)
	{
		try
		{
			tv_Templates.SetRowCellValue(rowHandle, "ItemCheck", value);
			tv_Templates.RefreshData();
			DataRow dataRow = tv_Templates.GetDataRow(rowHandle);
			if (ItemsSelected.Contains(dataRow) || value.Equals("False"))
			{
				ItemsSelected.Remove(dataRow);
			}
			if (value.Equals("True"))
			{
				ItemsSelected.Add(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void ClearItemsSelected()
	{
		ItemsSelected.Clear();
	}

	private void SearchData(int searchType)
	{
		try
		{
			string text = btn_TextSearch.Text.Trim().ToLower();
			DataView dataView = new DataView(_dtSource.Copy());
			if (string.IsNullOrEmpty(text))
			{
				SetDataSource(dataView.ToTable());
			}
			else if (!string.IsNullOrEmpty(text) && dataView.ToTable().IsNotNull())
			{
				switch (searchType)
				{
				case 0:
					dataView.RowFilter = string.Format("UserCode = '{0}' or UserName = '{0}'", text);
					break;
				case 1:
					dataView.RowFilter = $"DocNo like '%{text}%'";
					break;
				}
				SetDataSource(dataView.ToTable());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Templates_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		try
		{
			if (!(sender is TileView tileView) || tv_Templates == null || tv_Templates.RowCount <= 0 || e.Item.Elements.Count <= 0)
			{
				return;
			}
			string arg = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "UserName"));
			string text = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "Status"));
			string text2 = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "IsDraft"));
			string text3 = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "ItemCheck"));
			string text4 = Function.ParseDecimal(tileView.GetRowCellValue(e.RowHandle, "DocTotal")).ToString("#,##,#");
			string text5 = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "DocNo"));
			string text6 = Function.ToString(tileView.GetRowCellValue(e.RowHandle, "Remarks"));
			TileItemElement tileItemElement = e.Item.Elements[1];
			string text7;
			if (!tc_UserRequest.Visible)
			{
				text7 = string.Empty;
			}
			else
			{
				string text8 = (e.Item.Elements[1].Text = $"                      {arg}");
				text7 = text8;
			}
			tileItemElement.Text = text7;
			if (tc_DraftNo.Visible || tc_DocNo.Visible)
			{
				if (text2.Equals("N"))
				{
					e.Item.Elements[6].Text = tc_DocNo.Caption + ": " + Function.ToString(text5);
				}
				else
				{
					e.Item.Elements[6].Text = tc_DraftNo.Caption + ": " + Function.ToString(text5);
				}
			}
			else
			{
				e.Item.Elements[6].Text = string.Empty;
			}
			if (tc_Remarks.Visible)
			{
				if (string.IsNullOrEmpty(Function.ToString(text6)))
				{
					e.Item.Elements[2].Text = tc_Remarks.Caption + ": empty";
				}
			}
			else
			{
				e.Item.Elements[2].Text = string.Empty;
			}
			if (string.IsNullOrEmpty(text4))
			{
				text4 = "0";
			}
			e.Item.Elements[8].Text = (DocTotalVisible ? $"                            {text4} {e.Item.Elements[11].Text}" : string.Empty);
			if (tc_Status.Visible)
			{
				switch (text)
				{
				case "Y":
					e.Item.Elements[5].Image = img_Lists.Images[0];
					e.Item.Elements[5].Appearance.Normal.ForeColor = Color.Green;
					e.Item.Elements[5].Text = tc_Approved.Caption;
					break;
				case "N":
					e.Item.Elements[5].Image = img_Lists.Images[1];
					e.Item.Elements[5].Appearance.Normal.ForeColor = Color.Red;
					e.Item.Elements[5].Text = tc_Rejected.Caption;
					break;
				case "W":
					e.Item.Elements[5].Image = img_Lists.Images[2];
					e.Item.Elements[5].Appearance.Normal.ForeColor = Color.DarkBlue;
					e.Item.Elements[5].Text = tc_Pending.Caption;
					break;
				}
			}
			if (CurrentRequest.Equals("M"))
			{
				e.Item.Elements[7].Appearance.Normal.ForeColor = Color.Green;
				e.Item.Elements[7].Text = (RequestTypeVisible ? _myReqText : string.Empty);
			}
			else if (CurrentRequest.Equals("P"))
			{
				e.Item.Elements[7].Appearance.Normal.ForeColor = Color.Red;
				e.Item.Elements[7].Text = (RequestTypeVisible ? _apprText : string.Empty);
			}
			if (text2.Equals("N"))
			{
				e.Item.Elements[7].Appearance.Normal.ForeColor = Color.Blue;
				e.Item.Elements[7].Text = (RequestTypeVisible ? _createdText : string.Empty);
			}
			if (text3.Equals("True"))
			{
				e.Item.AppearanceItem.Normal.BackColor = Color.FromArgb(254, 234, 163);
				e.Item.AppearanceItem.Hovered.BackColor = Color.FromArgb(254, 234, 163);
			}
			else
			{
				e.Item.AppearanceItem.Normal.BackColor = Color.White;
				e.Item.AppearanceItem.Hovered.BackColor = Color.FromArgb(254, 234, 163);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Templates_ContextButtonClick(object sender, ContextItemClickEventArgs e)
	{
		try
		{
			if (e.Item != null && e.DataItem != null)
			{
				CheckContextButton checkContextButton = e.Item as CheckContextButton;
				int rowHandle = (e.DataItem as TileViewItem).RowHandle;
				tv_Templates.FocusedRowHandle = rowHandle;
				if (AllowCheckItem)
				{
					SetItemCheckValue(rowHandle, checkContextButton.Checked.ToString());
				}
				else
				{
					SetItemCheckValue(rowHandle, "False");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Templates_ContextButtonCustomize(object sender, TileViewContextButtonCustomizeEventArgs e)
	{
		try
		{
			if (e.Item is CheckContextButton checkContextButton)
			{
				checkContextButton.Checked = Function.ParseBool(tv_Templates.GetRowCellValue(e.RowHandle, tc_ValueCheck));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Templates_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
	{
		try
		{
			if (!_isLoaded && !grd_Templates.DataSourceTable.IsNotNull())
			{
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				e.Graphics.DrawString("No document found", new Font("Segoe UI", 8.25f, FontStyle.Regular), SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), stringFormat);
				tv_Templates.RefreshEditor(updateEditorValue: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Templates_MouseDown(object sender, MouseEventArgs e)
	{
		contextMenuStrip1.Enabled = grd_Templates.DataSourceTable.IsNotNull();
	}

	private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (e.SelectedControl != grd_Templates)
			{
				return;
			}
			TileViewHitInfo tileViewHitInfo = tv_Templates.CalcHitInfo(e.ControlMousePosition);
			DataRow dataRow = tv_Templates.GetDataRow(tileViewHitInfo.RowHandle);
			if (tileViewHitInfo.HitTest != TileControlHitTest.Item)
			{
				return;
			}
			ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo(tileViewHitInfo.ItemInfo, "");
			toolTipControlInfo.ToolTipType = ToolTipType.SuperTip;
			toolTipControlInfo.SuperTip = new SuperToolTip();
			int docEntry = Function.ParseInt(dataRow["DocNo"]);
			int wddCode = Function.ParseInt(dataRow["WddCode"]);
			int wtmCode = Function.ParseInt(dataRow["WtmCode"]);
			foreach (DataRow row in _httpClient.WorkingProcessClient.GetTooltipData(wddCode, wtmCode, docEntry, GlobalData.UserID).Rows)
			{
				string text = Function.ToString(row["DispText"]) + ": " + Function.ToString(row["Value"]);
				toolTipControlInfo.SuperTip.Items.Add(text);
			}
			e.Info = toolTipControlInfo;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Next_Click(object sender, EventArgs e)
	{
		NavigationPage("N");
	}

	private void btn_FirstRecord_Click(object sender, EventArgs e)
	{
		NavigationPage("F");
	}

	private void btn_Previous_Click(object sender, EventArgs e)
	{
		NavigationPage("P");
	}

	private void btn_Last_Click(object sender, EventArgs e)
	{
		NavigationPage("L");
	}

	private void lk_SearchType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(lk_SearchType.EditValue) == 3)
			{
				btn_TextSearch.Visible = false;
				lk_RequestType.Visible = true;
				lk_RequestType.ItemIndex = 0;
				lk_RequestType_EditValueChanged(sender, e);
				lk_RequestType.Focus();
			}
			else
			{
				lk_RequestType.Visible = false;
				btn_TextSearch.Visible = true;
				btn_TextSearch.EditValue = string.Empty;
				btn_TextSearch.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_TextSearch_TextChanged(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		SearchData(Function.ParseInt(lk_SearchType.EditValue));
		WinCommons._CloseProcessing();
	}

	private void mnu_TooltipSetting_Click(object sender, EventArgs e)
	{
		ShowWorkingProcessSetting("T");
	}

	private void btn_TextSearch_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL((Function.ParseInt(lk_SearchType.EditValue) == 0) ? Enums.TypeCFL.Users : Enums.TypeCFL.DraftNo, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					btn_TextSearch.EditValue = frm_CFL2.ChooseItem[0];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void lk_RequestType_EditValueChanged(object sender, EventArgs e)
	{
		CurrentRequest = ((lk_RequestType.ItemIndex == 0) ? "P" : "M");
	}

	private void tv_Templates_ItemClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			int focusedRowHandle = tv_Templates.FocusedRowHandle;
			string text = Function.ToString(tv_Templates.GetRowCellValue(focusedRowHandle, "Status"));
			string text2 = Function.ToString(tv_Templates.GetRowCellValue(focusedRowHandle, "IsDraft"));
			string text3 = Function.ToString(tv_Templates.GetRowCellValue(focusedRowHandle, "ItemCheck"));
			Function.ParseInt(tv_Templates.GetRowCellValue(focusedRowHandle, "OwnerID"));
			if (text.Equals("N") || text2.Equals("N"))
			{
				e.Item.Enabled = false;
				SetItemCheckValue(focusedRowHandle, "False");
				AllowCheckItem = false;
			}
			else if (CurrentRequest.Equals("M"))
			{
				if (text.Equals("Y"))
				{
					SetItemCheckValue(focusedRowHandle, text3.Equals("False") ? "True" : "False");
					AllowCheckItem = true;
				}
				else
				{
					e.Item.Enabled = false;
					SetItemCheckValue(focusedRowHandle, "False");
					AllowCheckItem = false;
				}
			}
			else
			{
				SetItemCheckValue(focusedRowHandle, text3.Equals("False") ? "True" : "False");
				AllowCheckItem = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_TextSearch_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			WinCommons._OpenProcessing();
			SearchData(Function.ParseInt(lk_SearchType.EditValue));
			WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.TileViewPagingEx));
		DevExpress.Utils.CheckContextButton checkContextButton = new DevExpress.Utils.CheckContextButton();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement8 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement9 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement10 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement11 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement12 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.tc_UserRequest = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Remarks = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DraftType = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_PostingDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tc_DraftNo = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_RequestType = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DocTotal = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_CardName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DocDueDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DocCur = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DocType = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_DocNo = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rep_ItmCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.mnu_TooltipSetting = new System.Windows.Forms.ToolStripMenuItem();
		this.img_Lists = new DevExpress.Utils.ImageCollection(this.components);
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.btn_FirstRecord = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Previous = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Last = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Next = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_PageNav = new Apzon.Controls.Controls.LabelControlEx();
		this.lk_RequestType = new Apzon.Controls.Editor.LookUpEditEx();
		this.tv_Templates = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tc_ValueCheck = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_WddCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_WtmCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_WtmName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_UserID = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Status = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_PostingTime = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_ApprovalReq = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_MyReq = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Created = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_CardCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Pending = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Approved = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tc_Rejected = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.grd_Templates = new Apzon.Controls.Grid.GridControlEx();
		this.btn_TextSearch = new Apzon.Controls.Editor.ButtonEditEx();
		this.lk_SearchType = new Apzon.Controls.Editor.LookUpEditEx();
		this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItmCheck).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.img_Lists).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_RequestType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Templates).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Templates).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_TextSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_SearchType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablePanel1).BeginInit();
		this.tablePanel1.SuspendLayout();
		base.SuspendLayout();
		this.tc_UserRequest.Caption = "User Request";
		this.tc_UserRequest.FieldName = "UserName";
		this.tc_UserRequest.Name = "tc_UserRequest";
		this.tc_UserRequest.Visible = true;
		this.tc_UserRequest.VisibleIndex = 4;
		this.tc_Remarks.Caption = "Remarks";
		this.tc_Remarks.FieldName = "Remarks";
		this.tc_Remarks.Name = "tc_Remarks";
		this.tc_Remarks.OptionsColumn.ShowCaption = true;
		this.tc_Remarks.Visible = true;
		this.tc_Remarks.VisibleIndex = 9;
		this.tc_DraftType.Caption = "Draft Type";
		this.tc_DraftType.FieldName = "DocType";
		this.tc_DraftType.Name = "tc_DraftType";
		this.tc_DraftType.OptionsColumn.ShowCaption = true;
		this.tc_DraftType.Visible = true;
		this.tc_DraftType.VisibleIndex = 6;
		this.tc_PostingDate.Caption = "Posting Date";
		this.tc_PostingDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.tc_PostingDate.FieldName = "DocDate";
		this.tc_PostingDate.Name = "tc_PostingDate";
		this.tc_PostingDate.OptionsColumn.ShowCaption = true;
		this.tc_PostingDate.Visible = true;
		this.tc_PostingDate.VisibleIndex = 10;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.tc_DraftNo.Caption = "Draft No";
		this.tc_DraftNo.FieldName = "DocNo";
		this.tc_DraftNo.Name = "tc_DraftNo";
		this.tc_DraftNo.OptionsColumn.ShowCaption = true;
		this.tc_DraftNo.Visible = true;
		this.tc_DraftNo.VisibleIndex = 7;
		this.tc_RequestType.Caption = "Request Type";
		this.tc_RequestType.Name = "tc_RequestType";
		this.tc_RequestType.Visible = true;
		this.tc_RequestType.VisibleIndex = 13;
		this.tc_DocTotal.Caption = "Total";
		this.tc_DocTotal.FieldName = "DocTotal";
		this.tc_DocTotal.Name = "tc_DocTotal";
		this.tc_DocTotal.OptionsColumn.ShowCaption = true;
		this.tc_DocTotal.Visible = true;
		this.tc_DocTotal.VisibleIndex = 20;
		this.tc_CardName.Caption = "Card Name";
		this.tc_CardName.FieldName = "CardName";
		this.tc_CardName.Name = "tc_CardName";
		this.tc_CardName.OptionsColumn.ShowCaption = true;
		this.tc_CardName.Visible = true;
		this.tc_CardName.VisibleIndex = 18;
		this.tc_DocDueDate.Caption = "Due Date";
		this.tc_DocDueDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.tc_DocDueDate.FieldName = "DocDueDate";
		this.tc_DocDueDate.Name = "tc_DocDueDate";
		this.tc_DocDueDate.OptionsColumn.ShowCaption = true;
		this.tc_DocDueDate.Visible = true;
		this.tc_DocDueDate.VisibleIndex = 19;
		this.tc_DocCur.Caption = "Currency";
		this.tc_DocCur.FieldName = "DocCur";
		this.tc_DocCur.Name = "tc_DocCur";
		this.tc_DocCur.Visible = true;
		this.tc_DocCur.VisibleIndex = 21;
		this.tc_DocType.Caption = "Type";
		this.tc_DocType.FieldName = "DocType";
		this.tc_DocType.Name = "tc_DocType";
		this.tc_DocType.OptionsColumn.ShowCaption = true;
		this.tc_DocType.Visible = true;
		this.tc_DocType.VisibleIndex = 5;
		this.tc_DocNo.Caption = "Doc No";
		this.tc_DocNo.FieldName = "DocNo";
		this.tc_DocNo.Name = "tc_DocNo";
		this.tc_DocNo.OptionsColumn.ShowCaption = true;
		this.tc_DocNo.Visible = true;
		this.tc_DocNo.VisibleIndex = 12;
		this.rep_ItmCheck.Caption = "";
		this.rep_ItmCheck.Name = "rep_ItmCheck";
		this.rep_ItmCheck.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_ItmCheck.ReadOnly = true;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.mnu_TooltipSetting });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(155, 26);
		this.mnu_TooltipSetting.Name = "mnu_TooltipSetting";
		this.mnu_TooltipSetting.Size = new System.Drawing.Size(154, 22);
		this.mnu_TooltipSetting.Text = "Tooltip Settting";
		this.mnu_TooltipSetting.Click += new System.EventHandler(this.mnu_TooltipSetting_Click);
		this.img_Lists.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("img_Lists.ImageStream");
		this.img_Lists.Images.SetKeyName(0, "approved.png");
		this.img_Lists.Images.SetKeyName(1, "reject.png");
		this.img_Lists.Images.SetKeyName(2, "pending.png");
		this.img_Lists.Images.SetKeyName(3, "setting.png");
		this.img_Lists.Images.SetKeyName(4, "cancel.png");
		this.toolTipController1.AutoPopDelay = 30000;
		this.toolTipController1.KeepWhileHovered = true;
		this.toolTipController1.RoundRadius = 10;
		this.toolTipController1.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
		this.btn_FirstRecord.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tablePanel1.SetColumn(this.btn_FirstRecord, 0);
		this.btn_FirstRecord.EditControl = null;
		this.btn_FirstRecord.EditValue = "";
		this.btn_FirstRecord.FieldName = null;
		this.btn_FirstRecord.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
		this.btn_FirstRecord.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_FirstRecord.ImageOptions.Image");
		this.btn_FirstRecord.IsSearchControl = false;
		this.btn_FirstRecord.Location = new System.Drawing.Point(3, 3);
		this.btn_FirstRecord.Name = "btn_FirstRecord";
		this.btn_FirstRecord.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_FirstRecord.ReadOnly = false;
		this.tablePanel1.SetRow(this.btn_FirstRecord, 0);
		this.btn_FirstRecord.SettingInVisible = false;
		this.btn_FirstRecord.SettingReadonly = false;
		this.btn_FirstRecord.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btn_FirstRecord.Size = new System.Drawing.Size(23, 20);
		this.btn_FirstRecord.TabIndex = 10;
		this.btn_FirstRecord.Click += new System.EventHandler(this.btn_FirstRecord_Click);
		this.btn_Previous.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tablePanel1.SetColumn(this.btn_Previous, 1);
		this.btn_Previous.EditControl = null;
		this.btn_Previous.EditValue = "";
		this.btn_Previous.FieldName = null;
		this.btn_Previous.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Previous.ImageOptions.Image");
		this.btn_Previous.IsSearchControl = false;
		this.btn_Previous.Location = new System.Drawing.Point(32, 3);
		this.btn_Previous.Name = "btn_Previous";
		this.btn_Previous.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_Previous.ReadOnly = false;
		this.tablePanel1.SetRow(this.btn_Previous, 0);
		this.btn_Previous.SettingInVisible = false;
		this.btn_Previous.SettingReadonly = false;
		this.btn_Previous.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btn_Previous.Size = new System.Drawing.Size(23, 20);
		this.btn_Previous.TabIndex = 11;
		this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
		this.btn_Last.AutoSize = true;
		this.tablePanel1.SetColumn(this.btn_Last, 4);
		this.btn_Last.EditControl = null;
		this.btn_Last.EditValue = "";
		this.btn_Last.FieldName = null;
		this.btn_Last.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Last.ImageOptions.Image");
		this.btn_Last.IsSearchControl = false;
		this.btn_Last.Location = new System.Drawing.Point(151, 3);
		this.btn_Last.Name = "btn_Last";
		this.btn_Last.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_Last.ReadOnly = false;
		this.tablePanel1.SetRow(this.btn_Last, 0);
		this.btn_Last.SettingInVisible = false;
		this.btn_Last.SettingReadonly = false;
		this.btn_Last.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btn_Last.Size = new System.Drawing.Size(22, 20);
		this.btn_Last.TabIndex = 12;
		this.btn_Last.Click += new System.EventHandler(this.btn_Last_Click);
		this.btn_Next.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tablePanel1.SetColumn(this.btn_Next, 3);
		this.btn_Next.EditControl = null;
		this.btn_Next.EditValue = "";
		this.btn_Next.FieldName = null;
		this.btn_Next.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Next.ImageOptions.Image");
		this.btn_Next.IsSearchControl = false;
		this.btn_Next.Location = new System.Drawing.Point(122, 3);
		this.btn_Next.Name = "btn_Next";
		this.btn_Next.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_Next.ReadOnly = false;
		this.tablePanel1.SetRow(this.btn_Next, 0);
		this.btn_Next.SettingInVisible = false;
		this.btn_Next.SettingReadonly = false;
		this.btn_Next.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btn_Next.Size = new System.Drawing.Size(23, 20);
		this.btn_Next.TabIndex = 13;
		this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
		this.lbl_PageNav.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tablePanel1.SetColumn(this.lbl_PageNav, 2);
		this.lbl_PageNav.EditControl = null;
		this.lbl_PageNav.EditValue = "Page 1 of 1";
		this.lbl_PageNav.FieldName = null;
		this.lbl_PageNav.Location = new System.Drawing.Point(61, 6);
		this.lbl_PageNav.Name = "lbl_PageNav";
		this.lbl_PageNav.ReadOnly = false;
		this.tablePanel1.SetRow(this.lbl_PageNav, 0);
		this.lbl_PageNav.SettingInVisible = false;
		this.lbl_PageNav.SettingReadonly = false;
		this.lbl_PageNav.Size = new System.Drawing.Size(55, 13);
		this.lbl_PageNav.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PageNav.TabIndex = 14;
		this.lbl_PageNav.Text = "Page 1 of 1";
		this.lk_RequestType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lk_RequestType.EditControl = null;
		this.lk_RequestType.FieldName = null;
		this.lk_RequestType.IsSearchControl = false;
		this.lk_RequestType.Location = new System.Drawing.Point(109, 3);
		this.lk_RequestType.Name = "lk_RequestType";
		this.lk_RequestType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_RequestType.Properties.Appearance.Options.UseFont = true;
		this.lk_RequestType.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_RequestType.Properties.AppearanceFocused.Options.UseFont = true;
		this.lk_RequestType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_RequestType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Key", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value")
		});
		this.lk_RequestType.Properties.DisplayMember = "Value";
		this.lk_RequestType.Properties.NullText = "";
		this.lk_RequestType.Properties.ShowHeader = false;
		this.lk_RequestType.Properties.ValueMember = "Key";
		this.lk_RequestType.SettingInVisible = false;
		this.lk_RequestType.SettingReadonly = false;
		this.lk_RequestType.Size = new System.Drawing.Size(398, 20);
		this.lk_RequestType.TabIndex = 8;
		this.lk_RequestType.Visible = false;
		this.lk_RequestType.EditValueChanged += new System.EventHandler(this.lk_RequestType_EditValueChanged);
		this.tv_Templates.Appearance.Group.BackColor = System.Drawing.Color.LightSalmon;
		this.tv_Templates.Appearance.Group.Options.UseBackColor = true;
		this.tv_Templates.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[26]
		{
			this.tc_ValueCheck, this.tc_WddCode, this.tc_WtmCode, this.tc_WtmName, this.tc_DocNo, this.tc_DocType, this.tc_DraftNo, this.tc_DraftType, this.tc_UserID, this.tc_UserRequest,
			this.tc_Status, this.tc_Remarks, this.tc_PostingDate, this.tc_PostingTime, this.tc_RequestType, this.tc_ApprovalReq, this.tc_MyReq, this.tc_Created, this.tc_CardCode, this.tc_CardName,
			this.tc_DocDueDate, this.tc_DocTotal, this.tc_DocCur, this.tc_Pending, this.tc_Approved, this.tc_Rejected
		});
		checkContextButton.AlignmentOptions.Panel = DevExpress.Utils.ContextItemPanel.Left;
		checkContextButton.AlignmentOptions.Position = DevExpress.Utils.ContextItemPosition.Center;
		checkContextButton.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
		checkContextButton.AppearanceNormal.BackColor = System.Drawing.Color.Gray;
		checkContextButton.AppearanceNormal.Options.UseBackColor = true;
		checkContextButton.Caption = null;
		checkContextButton.Id = new System.Guid("fe6d26b5-5f6d-4dea-8d5c-2e1e4d2d7ba4");
		checkContextButton.Name = "chk_ItemCheck";
		checkContextButton.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tv_Templates.ContextButtons.Add(checkContextButton);
		this.tv_Templates.GridControl = this.grd_Templates;
		this.tv_Templates.Name = "tv_Templates";
		this.tv_Templates.OptionsDragDrop.ShowDropIndicators = false;
		this.tv_Templates.OptionsKanban.GroupFooterButton.Visible = DevExpress.Utils.DefaultBoolean.True;
		this.tv_Templates.OptionsTiles.AllowItemHover = true;
		this.tv_Templates.OptionsTiles.IndentBetweenGroups = 0;
		this.tv_Templates.OptionsTiles.IndentBetweenItems = 0;
		this.tv_Templates.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tv_Templates.OptionsTiles.ItemSize = new System.Drawing.Size(748, 106);
		this.tv_Templates.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tv_Templates.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tv_Templates.OptionsTiles.Padding = new System.Windows.Forms.Padding(3);
		this.tv_Templates.OptionsTiles.RowCount = 0;
		tableColumnDefinition.Length.Value = 45.0;
		tableColumnDefinition2.Length.Value = 497.0;
		tableColumnDefinition3.Length.Value = 206.0;
		this.tv_Templates.TileColumns.Add(tableColumnDefinition);
		this.tv_Templates.TileColumns.Add(tableColumnDefinition2);
		this.tv_Templates.TileColumns.Add(tableColumnDefinition3);
		tableRowDefinition.Length.Value = 27.0;
		tableRowDefinition2.Length.Value = 29.0;
		tableRowDefinition3.Length.Value = 24.0;
		tableRowDefinition4.Length.Value = 26.0;
		this.tv_Templates.TileRows.Add(tableRowDefinition);
		this.tv_Templates.TileRows.Add(tableRowDefinition2);
		this.tv_Templates.TileRows.Add(tableRowDefinition3);
		this.tv_Templates.TileRows.Add(tableRowDefinition4);
		tableSpan.RowSpan = 3;
		this.tv_Templates.TileSpans.Add(tableSpan);
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.Squeeze;
		tileViewItemElement.Name = "tc_Check";
		tileViewItemElement.Text = "";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement2.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement2.Column = this.tc_UserRequest;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "tc_UserRequest";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.Column = this.tc_Remarks;
		tileViewItemElement3.ColumnIndex = 1;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.RowIndex = 2;
		tileViewItemElement3.Text = "tc_Remarks";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.TextLocation = new System.Drawing.Point(2, 0);
		tileViewItemElement4.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement4.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement4.Column = this.tc_DraftType;
		tileViewItemElement4.ColumnIndex = 1;
		tileViewItemElement4.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement4.RowIndex = 3;
		tileViewItemElement4.Text = "tc_DraftType";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement4.TextLocation = new System.Drawing.Point(2, 0);
		tileViewItemElement5.Column = this.tc_PostingDate;
		tileViewItemElement5.ColumnIndex = 2;
		tileViewItemElement5.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement5.RowIndex = 1;
		tileViewItemElement5.Text = "tc_PostingDate";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement6.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI Semibold", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement6.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement6.Appearance.Normal.Options.UseImage = true;
		tileViewItemElement6.ColumnIndex = 2;
		tileViewItemElement6.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement6.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.Squeeze;
		tileViewItemElement6.ImageOptions.ImageSize = new System.Drawing.Size(16, 16);
		tileViewItemElement6.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileViewItemElement6.RowIndex = 3;
		tileViewItemElement6.Text = "tc_Status";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement7.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement7.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement7.Column = this.tc_DraftNo;
		tileViewItemElement7.ColumnIndex = 2;
		tileViewItemElement7.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement7.Text = "tc_DraftNo";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement8.Column = this.tc_RequestType;
		tileViewItemElement8.ColumnIndex = 1;
		tileViewItemElement8.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement8.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement8.MaxWidth = 150;
		tileViewItemElement8.Name = "tle_RequestType";
		tileViewItemElement8.Text = "tc_RequestType";
		tileViewItemElement8.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement8.TextLocation = new System.Drawing.Point(2, 0);
		tileViewItemElement9.Column = this.tc_DocTotal;
		tileViewItemElement9.ColumnIndex = 1;
		tileViewItemElement9.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement9.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement9.RowIndex = 3;
		tileViewItemElement9.Text = "tc_DocTotal";
		tileViewItemElement9.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.Column = this.tc_CardName;
		tileViewItemElement10.ColumnIndex = 1;
		tileViewItemElement10.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement10.RowIndex = 1;
		tileViewItemElement10.StretchHorizontal = true;
		tileViewItemElement10.Text = "tc_CardName";
		tileViewItemElement10.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement10.TextLocation = new System.Drawing.Point(2, 0);
		tileViewItemElement11.Column = this.tc_DocDueDate;
		tileViewItemElement11.ColumnIndex = 2;
		tileViewItemElement11.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement11.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement11.RowIndex = 2;
		tileViewItemElement11.Text = "tc_DocDueDate";
		tileViewItemElement11.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement12.Column = this.tc_DocCur;
		tileViewItemElement12.ColumnIndex = 1;
		tileViewItemElement12.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement12.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement12.ImageVisible = false;
		tileViewItemElement12.RowIndex = 3;
		tileViewItemElement12.Text = "tc_DocCur";
		tileViewItemElement12.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		tileViewItemElement12.TextVisible = false;
		this.tv_Templates.TileTemplate.Add(tileViewItemElement);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement2);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement3);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement4);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement5);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement6);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement7);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement8);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement9);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement10);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement11);
		this.tv_Templates.TileTemplate.Add(tileViewItemElement12);
		this.tv_Templates.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.tv_Templates_ItemClick);
		this.tv_Templates.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.tv_Templates_ContextButtonClick);
		this.tv_Templates.ContextButtonCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewContextButtonCustomizeEventHandler(this.tv_Templates_ContextButtonCustomize);
		this.tv_Templates.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tv_Templates_ItemCustomize);
		this.tv_Templates.CustomDrawEmptyForeground += new DevExpress.XtraGrid.Views.Base.CustomDrawEventHandler(this.tv_Templates_CustomDrawEmptyForeground);
		this.tc_ValueCheck.Caption = "Item Check";
		this.tc_ValueCheck.ColumnEdit = this.rep_ItmCheck;
		this.tc_ValueCheck.FieldName = "ItemCheck";
		this.tc_ValueCheck.Name = "tc_ValueCheck";
		this.tc_ValueCheck.OptionsColumn.AllowEdit = false;
		this.tc_ValueCheck.OptionsColumn.AllowFocus = false;
		this.tc_ValueCheck.OptionsColumn.ReadOnly = true;
		this.tc_ValueCheck.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.tc_ValueCheck.Visible = true;
		this.tc_ValueCheck.VisibleIndex = 0;
		this.tc_WddCode.Caption = "WddCode";
		this.tc_WddCode.FieldName = "WddCode";
		this.tc_WddCode.Name = "tc_WddCode";
		this.tc_WddCode.Visible = true;
		this.tc_WddCode.VisibleIndex = 1;
		this.tc_WtmCode.FieldName = "WtmCode";
		this.tc_WtmCode.Name = "tc_WtmCode";
		this.tc_WtmCode.OptionsColumn.ShowCaption = true;
		this.tc_WtmCode.Visible = true;
		this.tc_WtmCode.VisibleIndex = 2;
		this.tc_WtmName.Caption = "Template Name";
		this.tc_WtmName.FieldName = "WtmName";
		this.tc_WtmName.Name = "tc_WtmName";
		this.tc_WtmName.OptionsColumn.ShowCaption = true;
		this.tc_UserID.FieldName = "UserID";
		this.tc_UserID.Name = "tc_UserID";
		this.tc_UserID.Visible = true;
		this.tc_UserID.VisibleIndex = 3;
		this.tc_Status.Caption = "Status";
		this.tc_Status.FieldName = "Status";
		this.tc_Status.Name = "tc_Status";
		this.tc_Status.Visible = true;
		this.tc_Status.VisibleIndex = 8;
		this.tc_PostingTime.Caption = "Update Time";
		this.tc_PostingTime.FieldName = "UpdateTime";
		this.tc_PostingTime.Name = "tc_PostingTime";
		this.tc_PostingTime.Visible = true;
		this.tc_PostingTime.VisibleIndex = 11;
		this.tc_ApprovalReq.Caption = "Approval Request";
		this.tc_ApprovalReq.Name = "tc_ApprovalReq";
		this.tc_ApprovalReq.Visible = true;
		this.tc_ApprovalReq.VisibleIndex = 15;
		this.tc_MyReq.Caption = "My Request";
		this.tc_MyReq.Name = "tc_MyReq";
		this.tc_MyReq.Visible = true;
		this.tc_MyReq.VisibleIndex = 16;
		this.tc_Created.Caption = "Created";
		this.tc_Created.Name = "tc_Created";
		this.tc_Created.Visible = true;
		this.tc_Created.VisibleIndex = 17;
		this.tc_CardCode.Caption = "Card Code";
		this.tc_CardCode.FieldName = "CardCode";
		this.tc_CardCode.Name = "tc_CardCode";
		this.tc_CardCode.OptionsColumn.ShowCaption = true;
		this.tc_CardCode.Visible = true;
		this.tc_CardCode.VisibleIndex = 14;
		this.tc_Pending.Caption = "Pending";
		this.tc_Pending.Name = "tc_Pending";
		this.tc_Approved.Caption = "Approved";
		this.tc_Approved.Name = "tc_Approved";
		this.tc_Rejected.Caption = "Rejected";
		this.tc_Rejected.Name = "tc_Rejected";
		this.grd_Templates.AllowPasteExcel = false;
		this.grd_Templates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Templates.ClipboardData = null;
		this.grd_Templates.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Templates.Location = new System.Drawing.Point(0, 27);
		this.grd_Templates.MainView = this.tv_Templates;
		this.grd_Templates.Name = "grd_Templates";
		this.grd_Templates.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemDateEditEx1 });
		this.grd_Templates.Size = new System.Drawing.Size(509, 518);
		this.grd_Templates.TabIndex = 5;
		this.grd_Templates.ToolTipController = this.toolTipController1;
		this.grd_Templates.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tv_Templates });
		this.grd_Templates.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grd_Templates_MouseDown);
		this.btn_TextSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_TextSearch.EditControl = null;
		this.btn_TextSearch.FieldName = null;
		this.btn_TextSearch.IsSearchControl = false;
		this.btn_TextSearch.Location = new System.Drawing.Point(109, 3);
		this.btn_TextSearch.Name = "btn_TextSearch";
		this.btn_TextSearch.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_TextSearch.Properties.Appearance.Options.UseFont = true;
		this.btn_TextSearch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_TextSearch.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_TextSearch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_TextSearch.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_TextSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_TextSearch.SettingInVisible = false;
		this.btn_TextSearch.SettingReadonly = false;
		this.btn_TextSearch.Size = new System.Drawing.Size(401, 20);
		this.btn_TextSearch.TabIndex = 7;
		this.btn_TextSearch.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_TextSearch_ButtonClick);
		this.btn_TextSearch.TextChanged += new System.EventHandler(this.btn_TextSearch_TextChanged);
		this.btn_TextSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btn_TextSearch_KeyDown);
		this.lk_SearchType.EditControl = null;
		this.lk_SearchType.FieldName = null;
		this.lk_SearchType.IsSearchControl = false;
		this.lk_SearchType.Location = new System.Drawing.Point(1, 3);
		this.lk_SearchType.Name = "lk_SearchType";
		this.lk_SearchType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_SearchType.Properties.Appearance.Options.UseFont = true;
		this.lk_SearchType.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_SearchType.Properties.AppearanceFocused.Options.UseFont = true;
		this.lk_SearchType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_SearchType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Key", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value")
		});
		this.lk_SearchType.Properties.DisplayMember = "Value";
		this.lk_SearchType.Properties.NullText = "";
		this.lk_SearchType.Properties.ShowHeader = false;
		this.lk_SearchType.Properties.ValueMember = "Key";
		this.lk_SearchType.SettingInVisible = false;
		this.lk_SearchType.SettingReadonly = false;
		this.lk_SearchType.Size = new System.Drawing.Size(107, 20);
		this.lk_SearchType.TabIndex = 6;
		this.lk_SearchType.EditValueChanged += new System.EventHandler(this.lk_SearchType_EditValueChanged);
		this.tablePanel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tablePanel1.AutoSize = true;
		this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 13f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 13f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 13f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 15.9f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 114.48f));
		this.tablePanel1.Controls.Add(this.btn_FirstRecord);
		this.tablePanel1.Controls.Add(this.lbl_PageNav);
		this.tablePanel1.Controls.Add(this.btn_Next);
		this.tablePanel1.Controls.Add(this.btn_Last);
		this.tablePanel1.Controls.Add(this.btn_Previous);
		this.tablePanel1.Location = new System.Drawing.Point(164, 547);
		this.tablePanel1.Name = "tablePanel1";
		this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26f));
		this.tablePanel1.Size = new System.Drawing.Size(177, 26);
		this.tablePanel1.TabIndex = 15;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.Controls.Add(this.tablePanel1);
		base.Controls.Add(this.lk_SearchType);
		base.Controls.Add(this.btn_TextSearch);
		base.Controls.Add(this.grd_Templates);
		base.Controls.Add(this.lk_RequestType);
		base.Name = "TileViewPagingEx";
		base.Size = new System.Drawing.Size(510, 573);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItmCheck).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.img_Lists).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_RequestType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tv_Templates).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Templates).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_TextSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_SearchType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablePanel1).EndInit();
		this.tablePanel1.ResumeLayout(false);
		this.tablePanel1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
