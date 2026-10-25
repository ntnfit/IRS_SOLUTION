using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Mrps;

public class frm_AdvanceForecast : XtraFormEx
{
	private DataTable _dtSettingSap;

	private const string OkString = "Ok";

	private const string SaveString = "Save";

	private const string UpdateString = "Update";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oAdvanceForecast);

	private DataTable _dataPrice = new DataTable();

	private string _itemCode;

	private string _whsCode;

	private DateTime? _dueDate;

	private bool _viewTypeChange;

	private IContainer components;

	private PanelControl pc_pannel;

	private SimpleButtonEx btnSearch;

	private TextEditEx txt_FCCode;

	private LabelLine lb_View;

	private LabelLine lb_BpCode;

	private SimpleButtonEx btn_GenerateForecast;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private SplitContainerControl scc_SplitContainerControl;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcForecastName;

	private GridColumnEx gcStartDate;

	private GridColumnEx gcEndDate;

	private GridColumnEx gcView;

	private RepositoryItemDateEdit rDate;

	private RepositoryItemLookUpEdit rView;

	private ContextMenuStrip contextMenuStrip_Doc;

	private ContextMenuStrip contextMenuStrip_Line;

	private GridControlEx grdLineQuantity;

	private GridViewEx gvLineQuantity;

	private GridColumnEx gcItemCode;

	private RepositoryItemButtonEdit rItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcWhsCode;

	private RepositoryItemButtonEdit rWhsCode;

	private ToolStripMenuItem deleteToolStripMenuItem_Doc;

	private ToolStripMenuItem addToolStripMenuItem_Doc;

	private ToolStripMenuItem deleteToolStripMenuStrip_Line;

	private GridSplitContainer gridSplitContainer2;

	private GridColumnEx gcDate;

	private GridColumnEx gcQuantity;

	private GridColumnEx gcAbsID;

	private GridColumnEx gcLineID;

	private LookUpEdit txtView;

	private RepositoryItemCalcEdit rCal;

	private GridColumnEx gcBPCode;

	private RepositoryItemButtonEdit rBPCode;

	private GridColumnEx gcSplCode;

	private RepositoryItemLookUpEdit rSpl;

	private GridColumnEx gcLineBP;

	private RepositoryItemButtonEdit rLineBP;

	private GridColumnEx gcLineSpl;

	private RepositoryItemLookUpEdit rLineSpl;

	private GridColumnEx gcCode;

	private XtraTabControlEx xtraTabForcast;

	private XtraTabPageEx TabQuantity;

	private XtraTabPageEx tabPrice;

	private GridControlEx grdLinePrice;

	private GridViewEx gvLinePrice;

	private GridColumnEx gcItemCodeLinePrice;

	private RepositoryItemButtonEdit rItemCodeLinePrice;

	private GridColumnEx gcItemNameLinePrice;

	private GridColumnEx gcWhsCodeLinePrice;

	private RepositoryItemButtonEdit rWhsCodeLinePrice;

	private GridColumnEx gcDateLinePrice;

	private GridColumnEx gcQuantityLinePrice;

	private GridColumnEx gcIdLinePrice;

	private GridColumnEx gcLineIdPrice;

	private GridColumnEx gcBpCodeLinePrice;

	private RepositoryItemButtonEdit rBpCodeLinePrice;

	private GridColumnEx gcSaleEmployeLinePrice;

	private RepositoryItemLookUpEdit rSaleLinePrice;

	private GridColumnEx gcLinePrice;

	private GridColumnEx gcPriceLinePrice;

	private GridColumnEx gcDocPriceList;

	private RepositoryItemLookUpEdit rPriceList;

	private GridColumnEx gcOriginType;

	private RepositoryItemLookUpEdit rOriginType;

	private GridColumnEx gcBpCatalog;

	private RepositoryItemButtonEdit rBpCatalog;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private ToolStripMenuItem duplicateToolStripMenuItem_Doc;

	private GridColumnEx gcUom;

	private GridColumnEx gcUoMLinePrice;

	private GridColumnEx gcCurrency;

	private RepositoryItemLookUpEdit rCurrency;

	private GridColumnEx gcLineCurrency;

	public Enums.FormViewModeType OpenFormViewMode { get; internal set; }

	public DataTable OpenFormDocument { get; set; }

	public DataTable OpenFormLines { get; set; }

	public ContextActionMappingImpl[] MappingDocument { get; set; }

	public ContextActionMappingImpl[] MappingLines { get; set; }

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
			if (FormAuth != Enums.FormAuthorities.Full && value != Enums.FormViewModeType.Find)
			{
				_formMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				contextMenuStrip_Doc.Enabled = false;
				btn_Save.Text = "Save";
				btn_GenerateForecast.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
			{
				EnableDisableControl(isEnable: false);
				gcForecastName.OptionsColumn.ReadOnly = false;
				gvDoc.ReadOnly = false;
				gcView.OptionsColumn.ReadOnly = true;
				gcView.AppearanceCell.BackColor = Color.FromArgb(224, 224, 224);
				GridViewEx gridViewEx2 = gvLineQuantity;
				bool readOnly = (gvLinePrice.ReadOnly = false);
				gridViewEx2.ReadOnly = readOnly;
				rItemCode.Buttons[0].Visible = true;
				rWhsCode.Buttons[0].Visible = true;
				rBPCode.Buttons[0].Visible = true;
				rLineBP.Buttons[0].Visible = true;
				rItemCode.Buttons[0].Visible = true;
				rWhsCode.Buttons[0].Visible = true;
				rBPCode.Buttons[0].Visible = true;
				rLineBP.Buttons[0].Visible = true;
				btn_Save.Enabled = true;
				contextMenuStrip_Doc.Enabled = true;
				contextMenuStrip_Line.Enabled = true;
				btn_GenerateForecast.Enabled = true;
				btn_Save.Text = "Ok";
				btnSearch.Enabled = true;
				btn_Cancel.Enabled = true;
				btn_GenerateForecast.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				gcView.OptionsColumn.ReadOnly = true;
				btn_Save.Text = "Update";
				btn_GenerateForecast.Enabled = false;
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Find:
			{
				EnableDisableControl(isEnable: false);
				gvDoc.ReadOnly = false;
				GridViewEx gridViewEx = gvLineQuantity;
				bool readOnly = (gvLinePrice.ReadOnly = false);
				gridViewEx.ReadOnly = readOnly;
				btn_Save.Enabled = true;
				contextMenuStrip_Doc.Enabled = true;
				contextMenuStrip_Line.Enabled = true;
				btn_GenerateForecast.Enabled = true;
				btn_Save.Text = "Ok";
				btnSearch.Enabled = true;
				btn_Cancel.Enabled = true;
				btn_GenerateForecast.Enabled = false;
				break;
			}
			}
		}
		catch
		{
		}
	}

	public frm_AdvanceForecast()
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceForecast_Load;
		FormMode = Enums.FormViewModeType.View;
	}

	public frm_AdvanceForecast(string forecastCode)
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceForecast_Load;
		FormMode = Enums.FormViewModeType.View;
		txt_FCCode.EditValue = forecastCode;
	}

	public frm_AdvanceForecast(string forecastCode, string itemCode, string whsCode, DateTime? dueDate)
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceForecast_Load;
		FormMode = Enums.FormViewModeType.View;
		txt_FCCode.EditValue = forecastCode;
		_itemCode = itemCode;
		_whsCode = whsCode;
		_dueDate = dueDate;
	}

	private async Task frm_AdvanceForecast_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			gvLineQuantity.LstColumNotSetting = new List<string> { "Currency", "UnitPrice", "Quantity" };
			gvLinePrice.LstColumNotSetting = new List<string> { "UnitPrice", "Quantity" };
			rCurrency.DataSource = GlobalData.Currencies;
			FormMode = Enums.FormViewModeType.Busy;
			_dtSettingSap = await GetSettingSap();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Name"] = "";
			dataRow["Value"] = "";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Name"] = "Daily";
			dataRow["Value"] = "D";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Name"] = "Weekly";
			dataRow["Value"] = "W";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Name"] = "Monthly";
			dataRow["Value"] = "M";
			dataTable.Rows.Add(dataRow);
			rView.DataSource = dataTable;
			txtView.Properties.DataSource = dataTable;
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "Standard",
				Code = "S"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Advance",
				Code = "A"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Excel",
				Code = "E"
			});
			rOriginType.DataSource = list;
			LoadSaleCode();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rPriceList;
			repositoryItemLookUpEdit.DataSource = await _httpClient.Commons.GetPriceList();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["APZ_OFCT"] = gvDoc,
				["APZ_FCT1"] = gvLineQuantity
			};
			List<Formular> formulars = await WinFuntions.GetFormulas(8000005);
			gvDoc.InitFormular(formulars, tblFormular);
			gvLineQuantity.InitFormular(formulars, tblFormular);
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gvDoc, "APZ_OFCT", tblFormular), WinFuntions.LoadUdfColumns(gvLineQuantity, "APZ_FCT1", tblFormular));
			if (OpenFormViewMode == Enums.FormViewModeType.Add)
			{
				await AddNewForecast();
				if (OpenFormDocument.IsNotNull())
				{
					for (int i = 0; i < MappingDocument.Length; i++)
					{
						if (string.IsNullOrEmpty(MappingDocument[i].TargetField))
						{
							continue;
						}
						GridColumn gridColumn = gvDoc.Columns[MappingDocument[i].TargetField];
						if (gridColumn != null)
						{
							object obj = null;
							if (!string.IsNullOrEmpty(MappingDocument[i].SourceField))
							{
								obj = ((!(gridColumn.ColumnType == typeof(DateTime))) ? OpenFormDocument.Rows[0][MappingDocument[i].SourceField] : ((object)Function.ParseDateTime(OpenFormDocument.Rows[0][MappingDocument[i].SourceField])));
							}
							if (string.IsNullOrEmpty(Function.ToString(obj)))
							{
								obj = MappingDocument[i].DfltValue;
							}
							gvDoc.SetFocusedRowCellValue(gridColumn, obj);
						}
					}
				}
				if (OpenFormLines.IsNotNull())
				{
					ContextActionMappingImpl mapDate = MappingLines.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "Date");
					if (mapDate != null)
					{
						DateTime? dateTime = OpenFormLines.AsEnumerable().Min((DataRow t) => Function.ParseDateTime(t[mapDate.SourceField]));
						DateTime? dateTime2 = OpenFormLines.AsEnumerable().Max((DataRow t) => Function.ParseDateTime(t[mapDate.SourceField]));
						gvDoc.SetFocusedRowCellValue("StartDate", dateTime ?? dateTime2 ?? DateTime.Today);
						gvDoc.SetFocusedRowCellValue("EndDate", dateTime2 ?? dateTime ?? DateTime.Today);
					}
					ContextActionMappingImpl contextActionMappingImpl = MappingLines.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "ItemCode");
					if (contextActionMappingImpl != null)
					{
						for (int num = 0; num < OpenFormLines.Rows.Count; num++)
						{
							AddNewRowLine();
							int num2 = gvLineQuantity.RowCount - 1;
							gvLineQuantity.FocusedRowHandle = num2;
							if (gvLineQuantity.FocusedRowHandle != num2)
							{
								return;
							}
							gvLineQuantity.SetFocusedRowCellValue("ItemCode", OpenFormLines.Rows[num][contextActionMappingImpl.SourceField]);
							for (int num3 = 0; num3 < MappingLines.Length; num3++)
							{
								object obj2 = OpenFormLines.Rows[num][MappingLines[num3].SourceField];
								if (string.IsNullOrEmpty(Function.ToString(obj2)))
								{
									obj2 = MappingLines[num3].DfltValue;
								}
								if (MappingLines[num3].TargetField == "Quantity")
								{
									if (mapDate != null)
									{
										DateTime? dateTime3 = Function.ParseDateTime(OpenFormLines.Rows[num][mapDate.SourceField]);
										if (dateTime3.HasValue)
										{
											DateTime value = dateTime3.Value;
											gvLineQuantity.SetFocusedRowCellValue("Q" + value.ToString("yyyy.MM.dd"), obj2);
										}
									}
								}
								else
								{
									gvLineQuantity.SetFocusedRowCellValue(MappingLines[num3].TargetField, obj2);
								}
							}
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.MappingItemCodeIsMissing, "");
					}
				}
			}
			else
			{
				LoadData(Enums.NavigateType.Current);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, Function.ToString(gridViewEx.GetFocusedRowCellValue(gcItemCode.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ListMandatoryField = "CardCode,SubCatNum";
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					WinCommons._OpenProcessing();
					InsertItemFromList(frm_CFL2.ChooseItems, gridViewEx);
					FormMode = ((FormMode == Enums.FormViewModeType.View) ? Enums.FormViewModeType.Update : FormMode);
				}
			}
			else
			{
				string text = Function.ToString(gridViewEx.GetFocusedRowCellValue(gcItemCode.FieldName));
				if (!string.IsNullOrWhiteSpace(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void rWhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, Function.ToString(gridViewEx.GetFocusedRowCellValue(gcWhsCode.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gridViewEx.SetFocusedRowCellValue(gcWhsCode.FieldName, frm_CFL2.ChooseItem["WhsCode"]);
				}
			}
			else
			{
				string text = Function.ToString(gridViewEx.GetFocusedRowCellValue(gcWhsCode.FieldName));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuStrip_Line_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formViewModeType = FormMode;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
				FormMode = Enums.FormViewModeType.Busy;
				WinCommons._OpenProcessing();
				gridViewEx.DeleteSelectedRows();
				gridViewEx.RefreshData();
				gridViewEx.GridControl.RefreshDataSource();
				gridViewEx.GridControl.DataSourceTable().AcceptChanges();
				AddNewRowLine();
				if (formViewModeType == Enums.FormViewModeType.View || formViewModeType == Enums.FormViewModeType.Find)
				{
					formViewModeType = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = formViewModeType;
		WinCommons._CloseProcessing();
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Code"));
			txt_FCCode.EditValue = num;
			grdLineQuantity.DataSourceTable.Rows.Clear();
			gvLineQuantity.ShowLoadingPanel();
			grdLinePrice.DataSourceTable.Rows.Clear();
			gvLinePrice.ShowLoadingPanel();
			DataSet dataLine = GetDataLine(num);
			LoadPivotGrid(dataLine);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			gvLineQuantity.HideLoadingPanel();
			gvLinePrice.HideLoadingPanel();
		}
	}

	private async void addToolStripMenuItem_Doc_Click(object sender, EventArgs e)
	{
		try
		{
			await AddNewForecast();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_Doc_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formMode = FormMode;
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("Code"))) && (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Find) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				DeleteForecast(Function.ParseInt(gvDoc.GetFocusedRowCellValue("Code")));
				formMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = formMode;
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gvLineQuantity_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
	{
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ((e.Column.FieldName == gcStartDate.FieldName || e.Column.FieldName == gcEndDate.FieldName) && !_viewTypeChange)
			{
				int type = ((e.Column == gcStartDate) ? 1 : 2);
				DateTime date = Function.ParseDateTimes(e.Value).Date;
				date = GetEditDate(date, Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, gcView)), e.RowHandle, type);
				grdDoc.DataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(e.RowHandle)][gvDoc.FocusedColumn.FieldName] = date;
				if (gvDoc.FocusedColumn.FieldName.Equals(gcStartDate.FieldName) && date > Function.ParseDateTimes(gvDoc.GetRowCellValue(e.RowHandle, gcEndDate)))
				{
					grdDoc.DataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(e.RowHandle)][gcEndDate.FieldName] = date;
				}
				else if (gvDoc.FocusedColumn.FieldName.Equals(gcEndDate.FieldName) && date < Function.ParseDateTimes(gvDoc.GetRowCellValue(e.RowHandle, gcStartDate.FieldName)))
				{
					grdDoc.DataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(e.RowHandle)][gcStartDate.FieldName] = date;
				}
				grdDoc.RefreshDataSource();
				grdDoc.DataSourceTable.AcceptChanges();
				List<ColumnsModel> listColumns = GetListColumns(Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, gcView)));
				CreatePivotColumns(listColumns);
				ChangeDataSource(listColumns);
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView))
			{
				return;
			}
			LineCellValueChange(gridView, gvDoc, e);
			if (e.Column.FieldName.Equals(gcItemCode.FieldName))
			{
				gridView.FocusedColumn = gcWhsCode;
				AddNewRowLine();
			}
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public void LineCellValueChange(GridView line, GridView doc, CellValueChangedEventArgs e)
	{
		int dataSourceRowIndex = line.GetDataSourceRowIndex(e.RowHandle);
		DataTable dataTable = line.GridControl.DataSourceTable();
		if (e.Column.FieldName.Equals("ItemCode"))
		{
			string text = Function.ToString(e.Value);
			dataTable.Rows[dataSourceRowIndex]["ItemCode"] = text;
			if (string.IsNullOrEmpty(text))
			{
				dataTable.Rows[dataSourceRowIndex]["ItemName"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["InvntryUom"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["WhsCode"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex][gcBpCatalog.FieldName] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["BPCode"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["SplCode"] = DBNull.Value;
				return;
			}
			DataTable aDoc = _httpClient.Items.GetADoc(text);
			if (aDoc != null && aDoc.Rows.Count > 0)
			{
				dataTable.Rows[dataSourceRowIndex]["ItemName"] = aDoc.Rows[0]["ItemName"];
				dataTable.Rows[dataSourceRowIndex]["InvntryUom"] = aDoc.Rows[0]["InvntryUom"];
				dataTable.Rows[dataSourceRowIndex]["WhsCode"] = aDoc.Rows[0]["DfltWH"];
				dataTable.Rows[dataSourceRowIndex][gcBpCatalog.FieldName] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["BPCode"] = Function.ToString(doc.GetRowCellValue(e.RowHandle, "BPCode"));
				dataTable.Rows[dataSourceRowIndex]["SplCode"] = Function.ParseInt(doc.GetRowCellValue(e.RowHandle, "SplCode"));
			}
			else
			{
				dataTable.Rows[dataSourceRowIndex]["ItemName"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["InvntryUom"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["WhsCode"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex][gcBpCatalog.FieldName] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["BPCode"] = DBNull.Value;
				dataTable.Rows[dataSourceRowIndex]["SplCode"] = DBNull.Value;
				WinCommons.Error(Enums.MessageCode.ItemCodeDoesNotExists, "");
			}
			double num = 0.0;
			DataRow[] array = _dataPrice.Select($"ItemCode = '{text}'");
			if (array.Length != 0)
			{
				num = Function.ParseDouble(array[0]["Price"]);
			}
			dataTable.Rows[dataSourceRowIndex]["Price"] = num;
		}
		else if (e.Column.FieldName.Equals(gcLineBP.FieldName))
		{
			dataTable.Rows[dataSourceRowIndex][gcBpCatalog.FieldName] = DBNull.Value;
		}
	}

	private void gvLine_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (sender is GridView gridView && e.RowHandle == gridView.FocusedRowHandle)
			{
				e.Appearance.BackColor = Color.Beige;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.RowHandle == gvDoc.FocusedRowHandle)
		{
			e.Appearance.BackColor = Color.Beige;
		}
	}

	private void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void grdLine_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinCommons._OpenProcessing();
				GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
				ExtenseionMethods.PasteFromExcelToDataTable(gridViewEx.GridControl, gridViewEx, new List<string> { gcItemCode.FieldName });
				AddNewRowLine();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gvDoc_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			gvDoc.FocusedRowHandle = -2147483646;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gvDoc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			e.ExceptionMode = ExceptionMode.Ignore;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rBPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, Function.ToString(gvDoc.GetFocusedRowCellValue(gcBPCode.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem == null)
				{
					return;
				}
				string value = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
				gvDoc.SetFocusedRowCellValue(gcBPCode.FieldName, value);
				grdDoc.RefreshDataSource();
				grdDoc.DataSourceTable.AcceptChanges();
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyToAllLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				DataTable dataSourceTable = grdLineQuantity.DataSourceTable;
				if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataSourceTable.Rows.Count; i++)
					{
						dataSourceTable.Rows[i][gcLineBP.FieldName] = value;
					}
					dataSourceTable.AcceptChanges();
					grdLineQuantity.RefreshDataSource();
				}
			}
			else
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gcBPCode.FieldName));
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.DrillDown(ApzObjectTypes.oAdvanceForecast, ApzObjectTypes.oBusinessPartners, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rLineBP_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, Function.ToString(gridViewEx.GetFocusedRowCellValue(gcLineBP.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gridViewEx.SetFocusedRowCellValue(gcLineBP.FieldName, frm_CFL2.ChooseItem["CardCode"]);
					RefreshDataGrid();
				}
			}
			else
			{
				string text = Function.ToString(gridViewEx.GetFocusedRowCellValue(gcLineBP.FieldName));
				if (!string.IsNullOrWhiteSpace(text))
				{
					this.DrillDown(ApzObjectTypes.oAdvanceForecast, ApzObjectTypes.oBusinessPartners, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.Name.Length > 1 && Function.ParseInt(e.Column.Name.Substring(1)) > 0 && Function.ParseDouble(e.Value).Equals(0.0))
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_AdvanceForecast_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_GenerateForecast_Click(object sender, EventArgs e)
	{
		try
		{
			frm_InputGenerateForecast frm_InputGenerateForecast2 = new frm_InputGenerateForecast();
			frm_InputGenerateForecast2.ShowDialog(this);
			if (frm_InputGenerateForecast2.DialogResult != DialogResult.OK)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			InputGenerateForecastImpl input = frm_InputGenerateForecast2.Input;
			input.ViewBy = Function.ToString(gvDoc.GetFocusedRowCellValue(gcView));
			input.FromDate = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue(gcStartDate));
			input.ToDate = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue(gcEndDate));
			LoadPivotGrid(await _httpClient.AdvanceForecastClient.GenerateForecast(input));
			gvDoc.SetFocusedRowCellValue("OriginType", "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void rBpCatalog_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
			int focusedRowHandle = gridViewEx.FocusedRowHandle;
			string itemCode = Function.ToString(gridViewEx.GetRowCellValue(focusedRowHandle, "ItemCode"));
			string bpCode = Function.ToString(gridViewEx.GetRowCellValue(focusedRowHandle, "CardCode"));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.BpCatalog, true, Function.ToString(gridViewEx.GetFocusedRowCellValue("SubCatNum")), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				BpCode = bpCode,
				ItemCode = itemCode
			};
			frm_CFL2.ShowDialog(this);
			WinCommons._OpenProcessing();
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				WinCommons._OpenProcessing();
				InsertItemFromList(frm_CFL2.ChooseItems, gridViewEx);
				FormMode = ((FormMode == Enums.FormViewModeType.View) ? Enums.FormViewModeType.Update : FormMode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				await AddNewForecast();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		WinCommons._OpenProcessing();
		try
		{
			LoadData(Enums.NavigateType.Current);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public new async Task FORMSETTING()
	{
		try
		{
			EditorContainer activeGridControl = ActiveControl.GetActiveGridControl();
			if (activeGridControl != null)
			{
				if (activeGridControl.Name.Equals(grdDoc.Name))
				{
					gvDoc.ShowFormSetting(this);
					return;
				}
				GridControlEx gridControl = (activeGridControl.Name.Equals(grdLineQuantity.Name) ? grdLineQuantity : grdLinePrice);
				ShowLineFormSetting(gridControl);
			}
			else
			{
				ShowSettingForm();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowLineFormSetting(GridControlEx gridControl)
	{
		try
		{
			if (!(gridControl.MainView is GridViewEx gridViewEx))
			{
				return;
			}
			List<string> list = gridViewEx.LstColumNotSetting ?? new List<string>();
			string value = (gridViewEx.Name.Equals(gvLineQuantity.Name) ? Function.ToString("APZ_FCT1") : Function.ToString("PFCT1"));
			DataTable ret = new DataTable();
			ret.Columns.Add("FormId", typeof(string));
			ret.Columns.Add("ItemId", typeof(string));
			ret.Columns.Add("ColumnId", typeof(string));
			ret.Columns.Add("ColumnName", typeof(string));
			ret.Columns.Add("UserSign", typeof(string));
			ret.Columns.Add("Width", typeof(int));
			ret.Columns.Add("Visible", typeof(bool));
			ret.Columns.Add("VisIndex", typeof(int));
			ret.Columns.Add("Editable", typeof(bool));
			ret.Columns.Add("IsReadOnly", typeof(bool));
			ret.Columns.Add("DataType", typeof(string));
			ret.Columns.Add("Fixed", typeof(string));
			ret.Columns.Add("AllowChSet", typeof(bool));
			ret.Columns.Add("VisSetting", typeof(bool));
			ret.Columns.Add("IsSystem", typeof(string));
			ret.Columns.Add("LngCode", typeof(string));
			ret.Columns.Add("NeverEditable", typeof(bool));
			ret.Columns.Add("GroupIndex", typeof(int));
			foreach (GridColumnEx column2 in gridViewEx.Columns)
			{
				if (!Function.ParseDateTime(column2.FieldName.Substring(1)).HasValue && !list.Contains(column2.FieldName))
				{
					DataRow dataRow = ret.NewRow();
					dataRow["FormId"] = FormId;
					dataRow["ItemId"] = value;
					dataRow["ColumnId"] = column2.FieldName;
					dataRow["ColumnName"] = column2.Caption;
					dataRow["UserSign"] = GlobalData.UserName;
					dataRow["Width"] = column2.Width;
					dataRow["Visible"] = column2.Visible;
					dataRow["VisIndex"] = column2.AbsoluteIndex;
					if (gridViewEx.LstColumnNotEditable != null && gridViewEx.LstColumnNotEditable.Contains(column2.FieldName))
					{
						dataRow["Editable"] = false;
						dataRow["NeverEditable"] = true;
					}
					else
					{
						dataRow["Editable"] = column2.Visible && !column2.OptionsColumn.ReadOnly;
						dataRow["NeverEditable"] = false;
					}
					dataRow["IsReadOnly"] = column2.OptionsColumn.ReadOnly;
					dataRow["DataType"] = column2.ColumnType.ToString();
					DataRow dataRow2 = dataRow;
					string value2;
					if (column2.Fixed == FixedStyle.Left)
					{
						value2 = "L";
					}
					else
					{
						value2 = ((column2.Fixed == FixedStyle.Right) ? "R" : "N");
					}
					dataRow2["Fixed"] = value2;
					dataRow["AllowChSet"] = column2.AllowChSet;
					dataRow["VisSetting"] = true;
					dataRow["IsSystem"] = "Y";
					dataRow["LngCode"] = "-";
					dataRow["GroupIndex"] = column2.GroupIndex;
					ret.Rows.Add(dataRow);
				}
			}
			GridControl gridControl2 = gridViewEx.GridControl;
			if (gridControl2.DataSource != null)
			{
				DataColumn[] array = (from DataColumn t in gridControl2.DataSourceTable().Columns
					where ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(t.ColumnName)) == null
					select t).ToArray();
				foreach (DataColumn column in array)
				{
					if (!list.Contains(column.ColumnName) && ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(column.ColumnName)) == null && column.ColumnName.StartsWith("U_"))
					{
						DataRow dataRow3 = ret.NewRow();
						dataRow3["FormId"] = FormId;
						dataRow3["ItemId"] = value;
						dataRow3["ColumnId"] = column.ColumnName;
						dataRow3["ColumnName"] = column.ColumnName;
						dataRow3["UserSign"] = GlobalData.UserName;
						dataRow3["Width"] = 100;
						dataRow3["Visible"] = false;
						dataRow3["VisIndex"] = ret.Rows.Count;
						dataRow3["Editable"] = false;
						dataRow3["IsReadOnly"] = true;
						dataRow3["DataType"] = column.DataType.Name;
						dataRow3["Fixed"] = "N";
						dataRow3["AllowChSet"] = true;
						dataRow3["VisSetting"] = true;
						dataRow3["IsSystem"] = "N";
						dataRow3["LngCode"] = "-";
						dataRow3["NeverEditable"] = gridViewEx.LstColumnNotEditable != null && gridViewEx.LstColumnNotEditable.Contains(column.ColumnName);
						dataRow3["GroupIndex"] = -1;
						ret.Rows.Add(dataRow3);
					}
				}
			}
			frm_FormSetting frm_FormSetting2 = new frm_FormSetting();
			frm_FormSetting2.DtSource = ret;
			frm_FormSetting2.MdiParent = MdiParent;
			frm_FormSetting2.Grid = gridControl;
			frm_FormSetting2.Item = (gridViewEx.Name.Equals(gvLineQuantity.Name) ? new Tuple<GridViewEx, string>(gvLineQuantity, "APZ_FCT1") : new Tuple<GridViewEx, string>(gvLinePrice, "PFCT1"));
			frm_FormSetting2.FormSettingId = FormId;
			frm_FormSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSaleCode()
	{
		try
		{
			DataTable dataSource = await _httpClient.Commons.GetSaleEmployee();
			rSpl.DataSource = dataSource;
			rLineSpl.DataSource = dataSource;
			rSaleLinePrice.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DateTime GetEditDate(DateTime dateEdit, string viewType, int rowHandle, int type)
	{
		try
		{
			switch (viewType)
			{
			case "W":
			{
				if (type == 1)
				{
					DayOfWeek dayOfWeek = dateEdit.DayOfWeek;
					if (_dtSettingSap == null || _dtSettingSap.Rows.Count == 0 || string.IsNullOrEmpty(Function.ToString(_dtSettingSap.Rows[0]["WndTo"])))
					{
						return dateEdit.Date.AddDays((double)((dayOfWeek != DayOfWeek.Sunday) ? (1 - dayOfWeek) : ((DayOfWeek)(-6))));
					}
					return dateEdit.Date.AddDays((double)((Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek <= DayOfWeek.Sunday) ? (Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek) : (Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek - 7)));
				}
				if (Function.ParseDateTimes(dateEdit).Date < Function.ParseDateTimes(gvDoc.GetRowCellValue(rowHandle, gcStartDate.FieldName)).Date)
				{
					dateEdit = Function.ParseDateTimes(gvDoc.GetRowCellValue(rowHandle, gcStartDate.FieldName)).Date;
				}
				DayOfWeek dayOfWeek2 = dateEdit.DayOfWeek;
				if (_dtSettingSap == null || _dtSettingSap.Rows.Count == 0 || string.IsNullOrEmpty(Function.ToString(_dtSettingSap.Rows[0]["WndTo"])))
				{
					return dateEdit.Date.AddDays((double)((dayOfWeek2 != DayOfWeek.Sunday) ? (7 - dayOfWeek2) : DayOfWeek.Sunday));
				}
				return dateEdit.Date.AddDays((double)(((Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek2 <= DayOfWeek.Sunday) ? (Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek2) : (Function.ParseInt(_dtSettingSap.Rows[0]["WndTo"]) - dayOfWeek2 - 7)) + 6));
			}
			case "M":
				if (type == 1)
				{
					return dateEdit.Date.AddDays(-Function.ParseDateTimes(dateEdit).Day + 1);
				}
				if (Function.ParseDateTimes(dateEdit).Date < Function.ParseDateTimes(gvDoc.GetRowCellValue(rowHandle, gcStartDate.FieldName)).Date)
				{
					dateEdit = Function.ParseDateTimes(gvDoc.GetRowCellValue(rowHandle, gcStartDate.FieldName)).Date;
				}
				return new DateTime(dateEdit.Year, dateEdit.Month, 1).AddMonths(1).AddDays(-1.0);
			case "D":
				return (Function.ParseDateTimes(dateEdit).Date < DateTime.Now.Date) ? DateTime.Now.Date : Function.ParseDateTimes(dateEdit).Date;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return DateTime.Now.Date;
	}

	private async Task Save()
	{
		Enums.FormViewModeType oldView = FormMode;
		try
		{
			FormMode = Enums.FormViewModeType.Busy;
			DataTable dataTable = UnPivotData();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				FormMode = oldView;
				return;
			}
			DataSet dataSet = new DataSet();
			DataTable dataTable2 = grdDoc.DataSourceTable.Clone();
			DataRow dataRow = dataTable2.NewRow();
			dataRow.ItemArray = gvDoc.GetFocusedDataRow().ItemArray;
			dataTable2.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable2);
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = ((oldView != Enums.FormViewModeType.Add) ? (await _httpClient.AdvanceForecastClient.Update(dataSet)) : (await _httpClient.AdvanceForecastClient.Save(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				WinCommons.Success((oldView == Enums.FormViewModeType.Add) ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				txt_FCCode.EditValue = "";
				LoadData(Enums.NavigateType.Current);
				WinCommons._CloseProcessing();
				return;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
	}

	private DataTable UnPivotData()
	{
		try
		{
			DataTable dataTable = grdLineQuantity.DataSourceTable.Copy();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i][gcItemCode.FieldName])))
				{
					dataTable.Rows.RemoveAt(i);
					i--;
				}
				else if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i][gcWhsCode.FieldName])))
				{
					WinCommons.Error(Enums.MessageCode.WarehouseCannotBeNull, "");
					gvLineQuantity.FocusedColumn = gcWhsCode;
					gvLineQuantity.FocusedRowHandle = i;
					gvLineQuantity.ShowEditor();
					return null;
				}
			}
			dataTable.AcceptChanges();
			DataTable dataTable2 = grdLinePrice.DataSourceTable.Copy();
			if (dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
				gvLineQuantity.FocusedColumn = gcItemCode;
				gvLineQuantity.ShowEditor();
				return null;
			}
			var anon = (from x in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(x["ItemCode"]))
				group x by new
				{
					ItemCode = Function.ToString(x["ItemCode"]),
					WhsCode = Function.ToString(x["Whscode"]),
					BpCode = Function.ToString(x["BPCode"])
				} into g
				where g.Count() > 1
				select g.Key).FirstOrDefault();
			if (!string.IsNullOrEmpty(anon?.ItemCode))
			{
				WinCommons.Error(Enums.MessageCode.CannotDuplicateKeyItemCodeWhsCodeWithValue, "", anon.ItemCode, anon.WhsCode, anon.BpCode);
				return null;
			}
			DataView dataView = new DataView(dataTable);
			List<string> collection = (from DataColumn t in dataTable.Columns
				where t.ColumnName.StartsWith("U_")
				select t.ColumnName).ToList();
			List<string> list = new List<string> { "Code", "LineID", "ItemCode", "ItemName", "InvntryUom", "WhsCode", "BPCode", "SplCode", "SubCatNum", "Currency" };
			list.AddRange(collection);
			DataTable dataTable3 = dataView.ToTable(distinct: true, list.ToArray());
			dataTable3.Columns.Add("Date", typeof(DateTime));
			dataTable3.Columns.Add("Quantity", typeof(double));
			dataTable3.Columns.Add("UnitPrice", typeof(decimal));
			DataTable dataTable4 = dataTable3.Clone();
			List<string> list2 = (from DataColumn t in dataTable.Columns
				where t.ColumnName.StartsWith("Q") && t.ColumnName.Contains(".")
				select t.ColumnName).ToList();
			if (!list2.Any())
			{
				WinCommons.Error(Enums.MessageCode.CannotProcessDateData, "");
				return null;
			}
			int num = 0;
			for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
			{
				string itemCode = Function.ToString(dataTable.Rows[num2]["ItemCode"]);
				string whsCode = Function.ToString(dataTable.Rows[num2]["WhsCode"]);
				string bpCode = Function.ToString(dataTable.Rows[num2]["BPCode"]);
				string splCode = Function.ToString(dataTable.Rows[num2]["SplCode"]);
				DataRow dataRow = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(itemCode) && Function.ToString(t["WhsCode"]).Equals(whsCode) && Function.ToString(t["BPCode"]).Equals(bpCode) && Function.ToString(t["SplCode"]).Equals(splCode));
				if (dataRow != null)
				{
					for (int num3 = 0; num3 < list2.Count; num3++)
					{
						DataRow dataRow2 = dataTable4.NewRow();
						dataRow2.ItemArray = dataRow.ItemArray;
						dataRow2["Date"] = Function.ParseDateTime(list2[num3].Substring(1), "yyyy.MM.dd");
						dataRow2["Quantity"] = dataTable.Rows[num2][list2[num3]];
						dataRow2["UnitPrice"] = dataTable2.Rows[num2]["P" + list2[num3].Substring(1)];
						dataRow2["Currency"] = dataTable2.Rows[num2]["Currency"];
						dataRow2["LineID"] = ++num;
						dataTable4.Rows.Add(dataRow2);
					}
				}
			}
			dataTable4.AcceptChanges();
			return dataTable4;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return null;
	}

	private async Task DeleteForecast(int code)
	{
		HttpResult httpResult = await _httpClient.AdvanceForecastClient.Delete(code);
		if (string.IsNullOrEmpty(httpResult.Message))
		{
			txt_FCCode.EditValue = "";
			LoadData(Enums.NavigateType.Current);
			WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
	}

	private void SetDataRevenue()
	{
		try
		{
			DataTable dataSourceTable = grdLineQuantity.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				double num = 0.0;
				string arg = Function.ToString(dataSourceTable.Rows[i][gcItemCode.FieldName]);
				if (_dataPrice.IsNotNull())
				{
					DataRow[] array = _dataPrice.Select($"ItemCode = '{arg}'");
					if (array.Length != 0)
					{
						num = Function.ParseDouble(array[0]["Price"]);
					}
				}
				for (int j = 0; j < gvLineQuantity.Columns.Count; j++)
				{
					string name = gvLineQuantity.Columns[j].Name;
					if (name.Length >= 2 && Function.ParseInt(name.Substring(1)) != 0 && !name.Substring(0, 1).Equals("P"))
					{
						string columnName = "P" + gvLineQuantity.Columns[j].FieldName.Substring(1);
						dataSourceTable.Rows[i][columnName] = num * Function.ParseDouble(dataSourceTable.Rows[i][gvLineQuantity.Columns[j].FieldName]);
						dataSourceTable.Rows[i]["Price"] = num;
					}
				}
			}
			dataSourceTable.AcceptChanges();
			grdLineQuantity.DataSource = dataSourceTable;
			grdLinePrice.DataSource = dataSourceTable;
			RefreshDataGrid();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InsertItemFromList(List<DataRow> lst, GridViewEx gv)
	{
		if (lst == null || lst.Count == 0)
		{
			return;
		}
		int focusedRowHandle = gv.FocusedRowHandle;
		int focusedDataSourceRowIndex = gv.GetFocusedDataSourceRowIndex();
		gv.DeleteRow(focusedRowHandle);
		gv.RefreshData();
		RefreshDataGrid();
		GridColumn focusedColumn = gv.FocusedColumn;
		DataTable dataTable = gv.GridControl.DataSourceTable();
		foreach (DataRow item in lst)
		{
			if (item == null)
			{
				continue;
			}
			gv.FocusedColumn = focusedColumn;
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, focusedDataSourceRowIndex);
			dataTable.AcceptChanges();
			gv.GridControl.DataSource = dataTable;
			int num = gv.FindRowHandleByDataRow(row);
			if (num >= 0)
			{
				gv.FocusedRowHandle = num;
				gv.SetFocusedRowCellValue(gcItemCode.FieldName, item["ItemCode"]);
				if (!string.IsNullOrEmpty(Function.ToString(item["SubCatNum"])))
				{
					gv.SetFocusedRowCellValue(gcLineBP.FieldName, item["CardCode"]);
				}
			}
		}
		RefreshDataGrid();
		AddNewRowLine();
	}

	private void ChangeDataSource(List<ColumnsModel> lst)
	{
		try
		{
			DataTable dataSourceTable = grdLineQuantity.DataSourceTable;
			if (lst == null || dataSourceTable.Columns.Count == 0)
			{
				return;
			}
			List<GridColumn> list = gvLineQuantity.Columns.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				string name = list[i].Name;
				if (name.Length <= 1 || Function.ParseInt(name.Substring(1)) == 0)
				{
					lst.Add(new ColumnsModel
					{
						ColumnFieldName = list[i].FieldName,
						ColumnName = list[i].Name,
						ColumnCaption = list[i].Caption
					});
				}
			}
			for (int j = 0; j < dataSourceTable.Columns.Count; j++)
			{
				string name2 = dataSourceTable.Columns[j].ColumnName;
				if (!lst.Exists((ColumnsModel t) => t.ColumnFieldName == name2))
				{
					if (!name2.StartsWith("U_"))
					{
						dataSourceTable.Columns.RemoveAt(j);
						j--;
					}
				}
				else
				{
					lst.Remove(lst.Find((ColumnsModel t) => t.ColumnFieldName == name2));
				}
			}
			for (int num = 0; num < lst.Count; num++)
			{
				string columnFieldName = lst[num].ColumnFieldName;
				dataSourceTable.Columns.Add(columnFieldName, columnFieldName.Equals("Date") ? typeof(DateTime) : typeof(decimal));
			}
			grdLineQuantity.DataSource = dataSourceTable;
			grdLinePrice.DataSource = dataSourceTable;
			RefreshDataGrid();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetSettingSap()
	{
		try
		{
			return await _httpClient.AdvanceForecastClient.GetSettingSap();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void LoadData(Enums.NavigateType type)
	{
		try
		{
			DataTable dataDoc = _httpClient.AdvanceForecastClient.GetDataDoc(Function.ToString(txt_FCCode.EditValue), Function.ToString(txtView.EditValue), type);
			grdDoc.DataSource = dataDoc;
			grdDoc.RefreshDataSource();
			if (gvDoc.FocusedRowHandle < 0)
			{
				DataSet dataLine = GetDataLine(Function.ParseInt(gvDoc.GetFocusedRowCellValue("Code")));
				LoadPivotGrid(dataLine);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataSet GetDataLine(int code)
	{
		DataSet dataSet = new DataSet();
		try
		{
			DataSet dataLine = _httpClient.AdvanceForecastClient.GetDataLine(code);
			if (dataLine == null || dataLine.Tables.Count != 2)
			{
				return dataSet;
			}
			DataTable dataTable = dataLine.Tables[0];
			DataTable dataTable2 = dataLine.Tables[1];
			if (dataTable.IsNotNull())
			{
				List<string> list = (from DataColumn t in dataTable.Columns
					where t.ColumnName.StartsWith("U_")
					select t.ColumnName).ToList();
				List<string> list2 = new List<string> { "Code", "ItemCode", "ItemName", "InvntryUom", "WhsCode", "BPCode", "SplCode", "SubCatNum", "Currency" };
				list2.AddRange(list);
				DataTable dataTable3 = new DataView(dataTable).ToTable(distinct: true, list2.ToArray());
				dataTable3.Columns.Add("LineID", typeof(int));
				List<DateTime> list3 = (from t in dataTable.AsEnumerable()
					select Function.ParseDateTimes(t["Date"])).Distinct().ToList();
				for (int num = 0; num < list3.Count; num++)
				{
					dataTable3.Columns.Add(string.Format("Q{0}", list3[num].ToString("yyyy.MM.dd")), typeof(double));
					dataTable3.Columns.Add(string.Format("P{0}", list3[num].ToString("yyyy.MM.dd")), typeof(double));
				}
				for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
				{
					string itemCode = Function.ToString(dataTable3.Rows[num2]["ItemCode"]);
					string whsCode = Function.ToString(dataTable3.Rows[num2]["WhsCode"]);
					string bpCode = Function.ToString(dataTable3.Rows[num2]["BPCode"]);
					string splCode = Function.ToString(dataTable3.Rows[num2]["SplCode"]);
					List<DataRow> list4 = (from t in dataTable.AsEnumerable()
						where Function.ToString(t["ItemCode"]).Equals(itemCode) && Function.ToString(t["WhsCode"]).Equals(whsCode) && Function.ToString(t["BPCode"]).Equals(bpCode) && Function.ToString(t["SplCode"]).Equals(splCode)
						select t).ToList();
					if (list4.Any())
					{
						for (int num3 = 0; num3 < list.Count; num3++)
						{
							string columnName = list[num3];
							dataTable3.Rows[num2][columnName] = list4[0][columnName];
						}
						dataTable3.Rows[num2]["Currency"] = list4[0]["Currency"];
						for (int num4 = 0; num4 < list4.Count; num4++)
						{
							string columnName2 = string.Format("Q{0}", Function.ParseDateTimes(list4[num4]["Date"]).ToString("yyyy.MM.dd"));
							string columnName3 = string.Format("P{0}", Function.ParseDateTimes(list4[num4]["Date"]).ToString("yyyy.MM.dd"));
							dataTable3.Rows[num2][columnName2] = list4[num4]["Quantity"];
							dataTable3.Rows[num2][columnName3] = list4[num4]["UnitPrice"];
						}
					}
				}
				dataSet.Tables.Add(dataTable3);
				dataSet.Tables.Add(dataTable2.Copy());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataSet;
	}

	private void AddNewRowLine()
	{
		if (!CheckBlanRowLine())
		{
			DataTable dataTable = grdLineQuantity.DataSourceTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = _httpClient.AdvanceForecastClient.GetColumnLine();
				dataTable.AcceptChanges();
			}
			else
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
			grdLineQuantity.DataSource = dataTable;
			grdLinePrice.DataSource = dataTable;
			RefreshDataGrid();
		}
	}

	private void RefreshDataGrid()
	{
		grdLineQuantity.RefreshDataSource();
		grdLineQuantity.DataSourceTable.AcceptChanges();
		grdLinePrice.RefreshDataSource();
		grdLinePrice.DataSourceTable.AcceptChanges();
	}

	private bool CheckBlanRowLine()
	{
		try
		{
			if (gvDoc.FocusedRowHandle < 0)
			{
				return true;
			}
			RefreshDataGrid();
			DataTable dataSourceTable = grdLineQuantity.DataSourceTable;
			if (dataSourceTable.Rows.Count == 0)
			{
				return false;
			}
			return dataSourceTable.Select().ToList().Exists((DataRow t) => string.IsNullOrEmpty(Function.ToString(t[gcItemCode.FieldName])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private void LoadPivotGrid(DataSet dt)
	{
		try
		{
			List<ColumnsModel> listColumns = GetListColumns(Function.ToString(gvDoc.GetFocusedRowCellValue(gcView)));
			CreatePivotColumns(listColumns);
			if (dt == null || dt.Tables.Count < 2)
			{
				grdLineQuantity.DataSourceTable.Rows.Clear();
				grdLinePrice.DataSourceTable.Rows.Clear();
			}
			else
			{
				grdLineQuantity.DataSource = dt.Tables[0];
				grdLinePrice.DataSource = dt.Tables[0];
				_dataPrice = dt.Tables[1];
			}
			RefreshDataGrid();
			ChangeDataSource(listColumns);
			AddNewRowLine();
			if (_dueDate.HasValue)
			{
				GridViewEx gridViewEx = (xtraTabForcast.SelectedTabPage.Name.Equals(TabQuantity.Name) ? gvLineQuantity : gvLinePrice);
				DataRow dataRow = gridViewEx.GridControl.DataSourceTable().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(_itemCode) && Function.ToString(t["WhsCode"]).Equals(_whsCode));
				if (dataRow != null)
				{
					gridViewEx.FocusedRowHandle = gridViewEx.FindRowHandleByDataRow(dataRow);
				}
				GridColumn gridColumn = gridViewEx.Columns.FirstOrDefault((GridColumn t) => t.FieldName.Length == 11 && t.FieldName.Substring(1).Equals(Function.ParseDateTimes(_dueDate).ToString("yyyy.MM.dd")));
				if (gridColumn != null)
				{
					gridViewEx.FocusedColumn = gridColumn;
					gridViewEx.ShowEditor();
				}
				_dueDate = null;
				_itemCode = (_whsCode = "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<ColumnsModel> GetListColumns(string viewType, bool isPivot = true)
	{
		List<ColumnsModel> list = new List<ColumnsModel>();
		int dateType = Function.ParseInt(GlobalData.DateTimeFormat);
		string charType = GlobalData.DateSeparatorFormat ?? " ";
		if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcStartDate.FieldName))) && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcEndDate.FieldName))))
		{
			switch (viewType)
			{
			case "D":
			{
				DateTime dateTime2 = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcStartDate.FieldName));
				while (dateTime2 <= Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcEndDate.FieldName)))
				{
					list.Add(new ColumnsModel
					{
						ColumnCaption = (isPivot ? Function.DateToStringSap(dateTime2, dateType, charType) : Function.ToString(dateTime2)),
						ColumnName = (list.Count + 1).ToString(),
						ColumnFieldName = "Q" + dateTime2.Date.ToString("yyyy.MM.dd")
					});
					list.Add(new ColumnsModel
					{
						ColumnCaption = (isPivot ? Function.DateToStringSap(dateTime2, dateType, charType) : Function.ToString(dateTime2)),
						ColumnName = (list.Count + 1).ToString(),
						ColumnFieldName = "P" + dateTime2.Date.ToString("yyyy.MM.dd")
					});
					dateTime2 = dateTime2.AddDays(1.0);
				}
				break;
			}
			case "W":
			{
				DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
				if (currentInfo != null)
				{
					Calendar calendar = currentInfo.Calendar;
					DateTime dateTime3 = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcStartDate.FieldName)).Date;
					while (dateTime3 <= Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcEndDate.FieldName)).Date)
					{
						list.Add(new ColumnsModel
						{
							ColumnCaption = (isPivot ? Function.ToString(calendar.GetWeekOfYear(dateTime3, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek)) : Function.ToString(dateTime3)),
							ColumnName = (list.Count + 1).ToString(),
							ColumnFieldName = "Q" + dateTime3.Date.ToString("yyyy.MM.dd")
						});
						list.Add(new ColumnsModel
						{
							ColumnCaption = (isPivot ? Function.ToString(calendar.GetWeekOfYear(dateTime3, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek)) : Function.ToString(dateTime3)),
							ColumnName = (list.Count + 1).ToString(),
							ColumnFieldName = "P" + dateTime3.Date.ToString("yyyy.MM.dd")
						});
						dateTime3 = dateTime3.AddDays(7.0);
					}
				}
				break;
			}
			case "M":
			{
				DateTime dateTime = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcStartDate.FieldName));
				while (dateTime <= Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcEndDate.FieldName)))
				{
					list.Add(new ColumnsModel
					{
						ColumnCaption = (isPivot ? Function.DateToStringSap(dateTime, dateType, charType, isDate: false) : Function.ToString(dateTime)),
						ColumnName = (list.Count + 1).ToString(),
						ColumnFieldName = "Q" + dateTime.Date.ToString("yyyy.MM.dd")
					});
					list.Add(new ColumnsModel
					{
						ColumnCaption = (isPivot ? Function.DateToStringSap(dateTime, dateType, charType, isDate: false) : Function.ToString(dateTime)),
						ColumnName = (list.Count + 1).ToString(),
						ColumnFieldName = "P" + dateTime.Date.ToString("yyyy.MM.dd")
					});
					dateTime = dateTime.AddMonths(1);
				}
				break;
			}
			}
		}
		return list;
	}

	private void RemoveColumns(GridViewEx gv)
	{
		for (int i = 0; i < gv.Columns.Count; i++)
		{
			string name = gv.Columns[i].Name;
			if (name.Length > 1 && Function.ParseInt(name.Substring(1)) > 0)
			{
				gv.Columns.RemoveAt(i);
				i--;
			}
		}
	}

	private void AddNewColumnPivot(GridViewEx gv, List<ColumnsModel> lst, string prefix = "")
	{
		List<GridColumnEx> list = new List<GridColumnEx>();
		if (prefix == "Q")
		{
			for (int i = 0; i < lst.Count; i++)
			{
				if (Function.ParseInt(lst[i].ColumnName) > 0 && lst[i].ColumnFieldName.Substring(0, 1).Equals(prefix))
				{
					GridColumnEx item = new GridColumnEx
					{
						Caption = lst[i].ColumnCaption,
						Name = prefix + lst[i].ColumnName,
						FieldName = lst[i].ColumnFieldName,
						ColumnEdit = rCal,
						Visible = true,
						Width = 100,
						DisplayFormatType = Enums.DisplayFormatType.Qty
					};
					list.Add(item);
				}
			}
		}
		else if (prefix == "P")
		{
			for (int j = 0; j < lst.Count; j++)
			{
				if (Function.ParseInt(lst[j].ColumnName) > 0 && lst[j].ColumnFieldName.Substring(0, 1).Equals(prefix))
				{
					GridColumnEx item2 = new GridColumnEx
					{
						Caption = lst[j].ColumnCaption,
						Name = prefix + lst[j].ColumnName,
						FieldName = lst[j].ColumnFieldName,
						ColumnEdit = rCal,
						Visible = true,
						Width = 100,
						DisplayFormatType = Enums.DisplayFormatType.Price,
						Currency = Enums.CurrencyColumn.Custom,
						CurrencyField = "Currency"
					};
					list.Add(item2);
				}
			}
		}
		if (lst.Count > 0)
		{
			GridColumnCollection columns = gv.Columns;
			GridColumn[] columns2 = list.ToArray();
			columns.AddRange(columns2);
		}
	}

	private void CreatePivotColumns(List<ColumnsModel> lst)
	{
		try
		{
			if (lst != null)
			{
				RemoveColumns(gvLineQuantity);
				RemoveColumns(gvLinePrice);
				AddNewColumnPivot(gvLineQuantity, lst, "Q");
				AddNewColumnPivot(gvLinePrice, lst, "P");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			rItemCode.Buttons[0].Visible = isEnable;
			rWhsCode.Buttons[0].Visible = isEnable;
			rBPCode.Buttons[0].Visible = isEnable;
			rLineBP.Buttons[0].Visible = isEnable;
			rCal.Buttons[0].Visible = isEnable;
			rItemCodeLinePrice.Buttons[0].Visible = isEnable;
			rWhsCodeLinePrice.Buttons[0].Visible = isEnable;
			rBpCodeLinePrice.Buttons[0].Visible = isEnable;
			rSaleLinePrice.Buttons[0].Visible = isEnable;
			btnSearch.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			btn_GenerateForecast.Enabled = isEnable;
			btn_Save.Enabled = isEnable;
			btn_GenerateForecast.Enabled = isEnable;
			contextMenuStrip_Doc.Enabled = isEnable;
			contextMenuStrip_Line.Enabled = isEnable;
			gvDoc.ReadOnly = !isEnable;
			GridViewEx gridViewEx = gvLineQuantity;
			bool readOnly = (gvLinePrice.ReadOnly = !isEnable);
			gridViewEx.ReadOnly = readOnly;
			gcForecastName.OptionsColumn.ReadOnly = !isEnable;
			gcView.OptionsColumn.ReadOnly = !isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNewForecast()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			DataTable dtDoc = grdDoc.DataSourceTable.Clone();
			if (dtDoc.Columns.Count == 0)
			{
				dtDoc = await _httpClient.AdvanceForecastClient.GetColumnDoc();
			}
			dtDoc.Rows.Clear();
			DataRow row = dtDoc.NewRow();
			row[gcStartDate.FieldName] = DateTime.Now.Date;
			DataRow dataRow = row;
			dataRow["Code"] = await GetNewIdToAddNew();
			row[gcEndDate.FieldName] = DateTime.Now.Date;
			row[gcView.FieldName] = "D";
			row["OriginType"] = "S";
			dtDoc.Rows.Add(row);
			if (_dataPrice == null || _dataPrice.Columns.Count < 2)
			{
				_dataPrice = new DataTable();
				_dataPrice.Columns.Add(new DataColumn("ItemCode", typeof(string)));
				_dataPrice.Columns.Add(new DataColumn("Price", typeof(double)));
			}
			_dataPrice.Rows.Clear();
			_dataPrice.AcceptChanges();
			dtDoc.AcceptChanges();
			grdDoc.DataSource = dtDoc;
			grdDoc.RefreshDataSource();
			grdDoc.DataSourceTable.AcceptChanges();
			gvDoc.FocusedColumn = gcForecastName;
			gvDoc.FocusedRowHandle = 0;
			gvDoc.ShowEditor();
			CreatePivotColumns(GetListColumns(Function.ToString(gvDoc.GetFocusedRowCellValue(gcView))));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void GetDataPrice(int priceList)
	{
		try
		{
			_dataPrice = _httpClient.AdvanceForecastClient.GetDataPrice(priceList);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> GetNewIdToAddNew()
	{
		try
		{
			HttpResult httpResult = await _httpClient.AdvanceForecastClient.GetNewIdForecast();
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return Function.ParseInt(httpResult.Content);
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	protected override void NoAuthSetControlSearch()
	{
		TextEditEx textEditEx = txt_FCCode;
		bool readOnly = (txtView.ReadOnly = false);
		textEditEx.ReadOnly = readOnly;
		btnSearch.Enabled = true;
	}

	private async void duplicateToolStripMenuItem_Doc_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Add;
			int num = await GetNewIdToAddNew();
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			DataTable dataTable = grdLinePrice.DataSourceTable.Copy();
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dataSourceTable.Rows[0];
			DataTable dataTable2 = grdDoc.DataSourceTable.Copy().Clone();
			dataRow["Code"] = num;
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow.AcceptChanges();
			dataTable2.Rows.Add(dataRow.ItemArray);
			dataTable2.AcceptChanges();
			grdDoc.DataSource = dataTable2;
			gvDoc.FocusedColumn = gcForecastName;
			gvDoc.FocusedRowHandle = 0;
			gvDoc.ShowEditor();
			foreach (DataRow row in dataTable.Rows)
			{
				row["Code"] = num;
			}
			dataTable.AcceptChanges();
			grdLineQuantity.DataSource = dataTable;
			grdLineQuantity.RefreshDataSource();
			grdLinePrice.DataSource = dataTable;
			grdLinePrice.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void rView_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rSpl_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyToAllLine, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int num = Function.ParseInt(e.NewValue);
				DataTable dataSourceTable = grdLineQuantity.DataSourceTable;
				if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataSourceTable.Rows.Count; i++)
					{
						dataSourceTable.Rows[i][gcLineSpl.FieldName] = num;
					}
					dataSourceTable.AcceptChanges();
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rPriceList_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void rView_EditValueChanged(object sender, EventArgs e)
	{
		if ((sender as LookUpEdit)?.GetSelectedDataRow() is DataRowView dataRowView)
		{
			DateTime dateEdit = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcStartDate));
			DateTime dateEdit2 = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue(gcEndDate));
			string viewType = Function.ToString(Function.ToString(dataRowView["Value"]));
			_viewTypeChange = true;
			gvDoc.SetFocusedRowCellValue(gcStartDate, GetEditDate(dateEdit, viewType, gvDoc.FocusedRowHandle, 1));
			gvDoc.SetFocusedRowCellValue(gcEndDate, GetEditDate(dateEdit2, viewType, gvDoc.FocusedRowHandle, 2));
			_viewTypeChange = false;
			CreatePivotColumns(GetListColumns(viewType));
			ChangeDataSource(GetListColumns(viewType));
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			gvDoc.FocusedColumn = gcStartDate;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_AdvanceForecast));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.pc_pannel = new DevExpress.XtraEditors.PanelControl();
		this.btn_GenerateForecast = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.scc_SplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcForecastName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcView = new Apzon.Controls.Grid.GridColumnEx();
		this.rView = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBPCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rBPCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcSplCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rSpl = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcOriginType = new Apzon.Controls.Grid.GridColumnEx();
		this.rOriginType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabForcast = new Apzon.Controls.Controls.XtraTabControlEx();
		this.TabQuantity = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdLineQuantity = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuStrip_Line = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLineQuantity = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUom = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rWhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcBpCatalog = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpCatalog = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAbsID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBP = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineBP = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineSpl = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineSpl = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tabPrice = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdLinePrice = new Apzon.Controls.Grid.GridControlEx();
		this.gvLinePrice = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCodeLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCodeLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemNameLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUoMLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWhsCodeLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rWhsCodeLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDateLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPriceLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantityLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIdLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineIdPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBpCodeLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpCodeLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcSaleEmployeLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rSaleLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_FCCode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_View = new Apzon.Controls.Controls.LabelLine();
		this.lb_BpCode = new Apzon.Controls.Controls.LabelLine();
		this.txtView = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pc_pannel).BeginInit();
		this.pc_pannel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.scc_SplitContainerControl).BeginInit();
		this.scc_SplitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSpl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabForcast).BeginInit();
		this.xtraTabForcast.SuspendLayout();
		this.TabQuantity.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLineQuantity).BeginInit();
		this.contextMenuStrip_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLineQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCatalog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		this.tabPrice.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCodeLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCodeLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCodeLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSaleLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FCCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtView.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.pc_pannel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pc_pannel.Controls.Add(this.btn_GenerateForecast);
		this.pc_pannel.Controls.Add(this.btn_Cancel);
		this.pc_pannel.Controls.Add(this.btn_Save);
		this.pc_pannel.Controls.Add(this.scc_SplitContainerControl);
		this.pc_pannel.Controls.Add(this.btnSearch);
		this.pc_pannel.Controls.Add(this.txt_FCCode);
		this.pc_pannel.Controls.Add(this.lb_View);
		this.pc_pannel.Controls.Add(this.lb_BpCode);
		this.pc_pannel.Controls.Add(this.txtView);
		this.pc_pannel.Location = new System.Drawing.Point(0, 1);
		this.pc_pannel.Name = "pc_pannel";
		this.pc_pannel.Size = new System.Drawing.Size(871, 460);
		this.pc_pannel.TabIndex = 0;
		this.btn_GenerateForecast.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_GenerateForecast.EditControl = null;
		this.btn_GenerateForecast.EditValue = "Generate Forecast";
		this.btn_GenerateForecast.FieldName = null;
		this.btn_GenerateForecast.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_GenerateForecast.ImageOptions.Image");
		this.btn_GenerateForecast.IsSearchControl = false;
		this.btn_GenerateForecast.Location = new System.Drawing.Point(735, 436);
		this.btn_GenerateForecast.Name = "btn_GenerateForecast";
		this.btn_GenerateForecast.ReadOnly = false;
		this.btn_GenerateForecast.SettingInVisible = false;
		this.btn_GenerateForecast.SettingReadonly = false;
		this.btn_GenerateForecast.Size = new System.Drawing.Size(132, 20);
		this.btn_GenerateForecast.TabIndex = 8;
		this.btn_GenerateForecast.Text = "Generate Forecast";
		this.btn_GenerateForecast.Click += new System.EventHandler(this.btn_GenerateForecast_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 436);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 6;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(7, 436);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 5;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.scc_SplitContainerControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.scc_SplitContainerControl.Horizontal = false;
		this.scc_SplitContainerControl.Location = new System.Drawing.Point(3, 29);
		this.scc_SplitContainerControl.Name = "scc_SplitContainerControl";
		this.scc_SplitContainerControl.Panel1.Controls.Add(this.gridSplitContainer2);
		this.scc_SplitContainerControl.Panel1.Text = "Panel1";
		this.scc_SplitContainerControl.Panel2.Controls.Add(this.xtraTabForcast);
		this.scc_SplitContainerControl.Panel2.Text = "Panel2";
		this.scc_SplitContainerControl.Size = new System.Drawing.Size(865, 401);
		this.scc_SplitContainerControl.SplitterPosition = 200;
		this.scc_SplitContainerControl.TabIndex = 0;
		this.scc_SplitContainerControl.Text = "splitContainerControl1";
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grdDoc;
		this.gridSplitContainer2.Location = new System.Drawing.Point(0, 1);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grdDoc);
		this.gridSplitContainer2.Size = new System.Drawing.Size(865, 198);
		this.gridSplitContainer2.TabIndex = 0;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip_Doc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rDate, this.rView, this.rBPCode, this.rSpl, this.rPriceList, this.rOriginType });
		this.grdDoc.Size = new System.Drawing.Size(865, 198);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem_Doc, this.deleteToolStripMenuItem_Doc, this.duplicateToolStripMenuItem_Doc });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(125, 70);
		this.addToolStripMenuItem_Doc.Name = "addToolStripMenuItem_Doc";
		this.addToolStripMenuItem_Doc.Size = new System.Drawing.Size(124, 22);
		this.addToolStripMenuItem_Doc.Text = "Add New";
		this.addToolStripMenuItem_Doc.Click += new System.EventHandler(this.addToolStripMenuItem_Doc_Click);
		this.deleteToolStripMenuItem_Doc.Name = "deleteToolStripMenuItem_Doc";
		this.deleteToolStripMenuItem_Doc.Size = new System.Drawing.Size(124, 22);
		this.deleteToolStripMenuItem_Doc.Text = "Delete";
		this.deleteToolStripMenuItem_Doc.Click += new System.EventHandler(this.deleteToolStripMenuItem_Doc_Click);
		this.duplicateToolStripMenuItem_Doc.Name = "duplicateToolStripMenuItem_Doc";
		this.duplicateToolStripMenuItem_Doc.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem_Doc.Text = "Duplicate";
		this.duplicateToolStripMenuItem_Doc.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Doc_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.Appearance.SelectedRow.BackColor = System.Drawing.Color.White;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcCode, this.gcForecastName, this.gcStartDate, this.gcEndDate, this.gcView, this.gcBPCode, this.gcSplCode, this.gcDocPriceList, this.gcOriginType });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oAdvanceForecast";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "APZ_OFCT";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsFind.AllowFindPanel = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.ColumnFilterChanged += new System.EventHandler(this.gvDoc_ColumnFilterChanged);
		this.gvDoc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gvDoc_InvalidValueException);
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.OptionsColumn.ReadOnly = true;
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcCode.Width = 77;
		this.gcForecastName.Caption = "Forecast Name";
		this.gcForecastName.CurrencyDateField = null;
		this.gcForecastName.CurrencyField = null;
		this.gcForecastName.CurrencyFixed = null;
		this.gcForecastName.CurrencyFixedRate = 0.0;
		this.gcForecastName.CurrencyRateField = null;
		this.gcForecastName.FieldName = "Name";
		this.gcForecastName.Name = "gcForecastName";
		this.gcForecastName.PrimaryKey = false;
		this.gcForecastName.UnitField = null;
		this.gcForecastName.Visible = true;
		this.gcForecastName.VisibleIndex = 1;
		this.gcForecastName.Width = 121;
		this.gcStartDate.Caption = "Start Date";
		this.gcStartDate.ColumnEdit = this.rDate;
		this.gcStartDate.CurrencyDateField = null;
		this.gcStartDate.CurrencyField = null;
		this.gcStartDate.CurrencyFixed = null;
		this.gcStartDate.CurrencyFixedRate = 0.0;
		this.gcStartDate.CurrencyRateField = null;
		this.gcStartDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcStartDate.FieldName = "StartDate";
		this.gcStartDate.Name = "gcStartDate";
		this.gcStartDate.PrimaryKey = false;
		this.gcStartDate.UnitField = null;
		this.gcStartDate.Visible = true;
		this.gcStartDate.VisibleIndex = 2;
		this.gcStartDate.Width = 115;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.Name = "rDate";
		this.gcEndDate.Caption = "End Date";
		this.gcEndDate.ColumnEdit = this.rDate;
		this.gcEndDate.CurrencyDateField = null;
		this.gcEndDate.CurrencyField = null;
		this.gcEndDate.CurrencyFixed = null;
		this.gcEndDate.CurrencyFixedRate = 0.0;
		this.gcEndDate.CurrencyRateField = null;
		this.gcEndDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcEndDate.FieldName = "EndDate";
		this.gcEndDate.Name = "gcEndDate";
		this.gcEndDate.PrimaryKey = false;
		this.gcEndDate.UnitField = null;
		this.gcEndDate.Visible = true;
		this.gcEndDate.VisibleIndex = 3;
		this.gcEndDate.Width = 128;
		this.gcView.Caption = "View";
		this.gcView.ColumnEdit = this.rView;
		this.gcView.CurrencyDateField = null;
		this.gcView.CurrencyField = null;
		this.gcView.CurrencyFixed = null;
		this.gcView.CurrencyFixedRate = 0.0;
		this.gcView.CurrencyRateField = null;
		this.gcView.FieldName = "FormView";
		this.gcView.Name = "gcView";
		this.gcView.PrimaryKey = false;
		this.gcView.UnitField = null;
		this.gcView.Visible = true;
		this.gcView.VisibleIndex = 4;
		this.gcView.Width = 114;
		this.rView.AutoHeight = false;
		this.rView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rView.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rView.DisplayMember = "Name";
		this.rView.Name = "rView";
		this.rView.NullText = "";
		this.rView.ShowFooter = false;
		this.rView.ShowHeader = false;
		this.rView.ValueMember = "Value";
		this.rView.EditValueChanged += new System.EventHandler(this.rView_EditValueChanged);
		this.rView.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rView_EditValueChanging);
		this.gcBPCode.Caption = "BP Code";
		this.gcBPCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.gcBPCode.ColumnEdit = this.rBPCode;
		this.gcBPCode.CurrencyDateField = null;
		this.gcBPCode.CurrencyField = null;
		this.gcBPCode.CurrencyFixed = null;
		this.gcBPCode.CurrencyFixedRate = 0.0;
		this.gcBPCode.CurrencyRateField = null;
		this.gcBPCode.FieldName = "BPCode";
		this.gcBPCode.Name = "gcBPCode";
		this.gcBPCode.PrimaryKey = false;
		this.gcBPCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBPCode.UnitField = null;
		this.gcBPCode.Visible = true;
		this.gcBPCode.VisibleIndex = 5;
		this.gcBPCode.Width = 67;
		this.rBPCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rBPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBPCode.Name = "rBPCode";
		this.rBPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBPCode_ButtonClick);
		this.gcSplCode.Caption = "Sale Employee";
		this.gcSplCode.ColumnEdit = this.rSpl;
		this.gcSplCode.CurrencyDateField = null;
		this.gcSplCode.CurrencyField = null;
		this.gcSplCode.CurrencyFixed = null;
		this.gcSplCode.CurrencyFixedRate = 0.0;
		this.gcSplCode.CurrencyRateField = null;
		this.gcSplCode.FieldName = "SplCode";
		this.gcSplCode.Name = "gcSplCode";
		this.gcSplCode.PrimaryKey = false;
		this.gcSplCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcSplCode.UnitField = null;
		this.gcSplCode.Visible = true;
		this.gcSplCode.VisibleIndex = 6;
		this.gcSplCode.Width = 86;
		this.rSpl.AutoHeight = false;
		this.rSpl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rSpl.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rSpl.DisplayMember = "SlpName";
		this.rSpl.Name = "rSpl";
		this.rSpl.NullText = "";
		this.rSpl.ShowFooter = false;
		this.rSpl.ShowHeader = false;
		this.rSpl.ValueMember = "SlpCode";
		this.rSpl.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rSpl_EditValueChanging);
		this.gcDocPriceList.Caption = "Price List";
		this.gcDocPriceList.ColumnEdit = this.rPriceList;
		this.gcDocPriceList.CurrencyDateField = null;
		this.gcDocPriceList.CurrencyField = null;
		this.gcDocPriceList.CurrencyFixed = null;
		this.gcDocPriceList.CurrencyFixedRate = 0.0;
		this.gcDocPriceList.CurrencyRateField = null;
		this.gcDocPriceList.FieldName = "PriceList";
		this.gcDocPriceList.Name = "gcDocPriceList";
		this.gcDocPriceList.PrimaryKey = false;
		this.gcDocPriceList.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDocPriceList.UnitField = null;
		this.gcDocPriceList.Visible = true;
		this.gcDocPriceList.VisibleIndex = 7;
		this.gcDocPriceList.Width = 77;
		this.rPriceList.AutoHeight = false;
		this.rPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rPriceList.DisplayMember = "Name";
		this.rPriceList.Name = "rPriceList";
		this.rPriceList.NullText = "";
		this.rPriceList.ValueMember = "Code";
		this.rPriceList.EditValueChanged += new System.EventHandler(this.rPriceList_EditValueChanged);
		this.gcOriginType.Caption = "Origin Type";
		this.gcOriginType.ColumnEdit = this.rOriginType;
		this.gcOriginType.CurrencyDateField = null;
		this.gcOriginType.CurrencyField = null;
		this.gcOriginType.CurrencyFixed = null;
		this.gcOriginType.CurrencyFixedRate = 0.0;
		this.gcOriginType.CurrencyRateField = null;
		this.gcOriginType.FieldName = "OriginType";
		this.gcOriginType.Name = "gcOriginType";
		this.gcOriginType.OptionsColumn.ReadOnly = true;
		this.gcOriginType.PrimaryKey = false;
		this.gcOriginType.UnitField = null;
		this.gcOriginType.Visible = true;
		this.gcOriginType.VisibleIndex = 8;
		this.gcOriginType.Width = 48;
		this.rOriginType.AutoHeight = false;
		this.rOriginType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOriginType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2")
		});
		this.rOriginType.DisplayMember = "Name";
		this.rOriginType.Name = "rOriginType";
		this.rOriginType.NullText = "";
		this.rOriginType.ShowHeader = false;
		this.rOriginType.ValueMember = "Code";
		this.xtraTabForcast.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabForcast.IsSearchControl = false;
		this.xtraTabForcast.Location = new System.Drawing.Point(0, 0);
		this.xtraTabForcast.Name = "xtraTabForcast";
		this.xtraTabForcast.SelectedTabPage = this.TabQuantity;
		this.xtraTabForcast.Size = new System.Drawing.Size(865, 191);
		this.xtraTabForcast.TabIndex = 1;
		this.xtraTabForcast.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.TabQuantity, this.tabPrice });
		this.TabQuantity.Controls.Add(this.grdLineQuantity);
		this.TabQuantity.EditControl = null;
		this.TabQuantity.EditValue = "Quantity Forecast";
		this.TabQuantity.FieldName = "TabQuantity";
		this.TabQuantity.IsSearchControl = false;
		this.TabQuantity.Name = "TabQuantity";
		this.TabQuantity.ReadOnly = false;
		this.TabQuantity.SettingInVisible = false;
		this.TabQuantity.SettingReadonly = false;
		this.TabQuantity.Size = new System.Drawing.Size(863, 166);
		this.TabQuantity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.TabQuantity.Text = "Quantity Forecast";
		this.grdLineQuantity.AllowPasteExcel = true;
		this.grdLineQuantity.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grdLineQuantity.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLineQuantity.Location = new System.Drawing.Point(0, 0);
		this.grdLineQuantity.MainView = this.gvLineQuantity;
		this.grdLineQuantity.Name = "grdLineQuantity";
		this.grdLineQuantity.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rItemCode, this.rWhsCode, this.rCal, this.rLineBP, this.rLineSpl, this.rBpCatalog });
		this.grdLineQuantity.Size = new System.Drawing.Size(863, 166);
		this.grdLineQuantity.TabIndex = 0;
		this.grdLineQuantity.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLineQuantity });
		this.grdLineQuantity.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdLine_EditorKeyDown);
		this.contextMenuStrip_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuStrip_Line });
		this.contextMenuStrip_Line.Name = "contextMenuStrip_Line";
		this.contextMenuStrip_Line.Size = new System.Drawing.Size(134, 26);
		this.deleteToolStripMenuStrip_Line.Name = "deleteToolStripMenuStrip_Line";
		this.deleteToolStripMenuStrip_Line.Size = new System.Drawing.Size(133, 22);
		this.deleteToolStripMenuStrip_Line.Text = "Delete Row";
		this.deleteToolStripMenuStrip_Line.Click += new System.EventHandler(this.deleteToolStripMenuStrip_Line_Click);
		this.gvLineQuantity.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvLineQuantity.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvLineQuantity.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvLineQuantity.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gvLineQuantity.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLineQuantity.ColumnKey = "ItemCode";
		this.gvLineQuantity.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcItemCode, this.gcItemName, this.gcUom, this.gcWhsCode, this.gcBpCatalog, this.gcDate, this.gcLinePrice, this.gcQuantity, this.gcAbsID, this.gcLineID,
			this.gcLineBP, this.gcLineSpl, this.gcLineCurrency
		});
		this.gvLineQuantity.DisableFormSetting = false;
		this.gvLineQuantity.DisplayException = false;
		this.gvLineQuantity.FieldCurrency = null;
		this.gvLineQuantity.FieldRate = null;
		this.gvLineQuantity.FormId = "oAdvanceForecast";
		this.gvLineQuantity.GridControl = this.grdLineQuantity;
		this.gvLineQuantity.IndicatorWidth = 50;
		this.gvLineQuantity.IsApzTable = false;
		this.gvLineQuantity.IsInsertNewRow = false;
		this.gvLineQuantity.IsLoadingData = false;
		this.gvLineQuantity.ItemId = "APZ_FCT1";
		this.gvLineQuantity.LstColumnAllowBatch = null;
		this.gvLineQuantity.LstColumnNotAllowBatch = null;
		this.gvLineQuantity.LstColumnNotEditable = new string[2] { "Price", "UoM" };
		this.gvLineQuantity.LstColumNotSetting = null;
		this.gvLineQuantity.Name = "gvLineQuantity";
		this.gvLineQuantity.OptionsFind.AllowFindPanel = false;
		this.gvLineQuantity.OptionsSelection.MultiSelect = true;
		this.gvLineQuantity.OptionsView.ColumnAutoWidth = false;
		this.gvLineQuantity.OptionsView.ShowGroupPanel = false;
		this.gvLineQuantity.UseBatchMod = false;
		this.gvLineQuantity.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvLine_RowCellStyle);
		this.gvLineQuantity.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gvLineQuantity_FocusedColumnChanged);
		this.gvLineQuantity.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanged);
		this.gvLineQuantity.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvLine_CustomColumnDisplayText);
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 94;
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcItemName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gcItemName.AppearanceCell.Options.UseBackColor = true;
		this.gcItemName.Caption = "Item Description";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 88;
		this.gcUom.Caption = "Uom Name";
		this.gcUom.CurrencyDateField = null;
		this.gcUom.CurrencyField = null;
		this.gcUom.CurrencyFixed = null;
		this.gcUom.CurrencyFixedRate = 0.0;
		this.gcUom.CurrencyRateField = null;
		this.gcUom.FieldName = "InvntryUom";
		this.gcUom.Name = "gcUom";
		this.gcUom.OptionsColumn.AllowEdit = false;
		this.gcUom.PrimaryKey = false;
		this.gcUom.UnitField = null;
		this.gcWhsCode.Caption = "WhsCode";
		this.gcWhsCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcWhsCode.ColumnEdit = this.rWhsCode;
		this.gcWhsCode.CurrencyDateField = null;
		this.gcWhsCode.CurrencyField = null;
		this.gcWhsCode.CurrencyFixed = null;
		this.gcWhsCode.CurrencyFixedRate = 0.0;
		this.gcWhsCode.CurrencyRateField = null;
		this.gcWhsCode.FieldName = "WhsCode";
		this.gcWhsCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcWhsCode.Name = "gcWhsCode";
		this.gcWhsCode.PrimaryKey = false;
		this.gcWhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWhsCode.UnitField = null;
		this.gcWhsCode.Visible = true;
		this.gcWhsCode.VisibleIndex = 2;
		this.gcWhsCode.Width = 95;
		this.rWhsCode.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rWhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhsCode.Name = "rWhsCode";
		this.rWhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhsCode_ButtonClick);
		this.gcBpCatalog.Caption = "Bp Catalog";
		this.gcBpCatalog.ColumnEdit = this.rBpCatalog;
		this.gcBpCatalog.CurrencyDateField = null;
		this.gcBpCatalog.CurrencyField = null;
		this.gcBpCatalog.CurrencyFixed = null;
		this.gcBpCatalog.CurrencyFixedRate = 0.0;
		this.gcBpCatalog.CurrencyRateField = null;
		this.gcBpCatalog.FieldName = "SubCatNum";
		this.gcBpCatalog.Name = "gcBpCatalog";
		this.gcBpCatalog.PrimaryKey = false;
		this.gcBpCatalog.UnitField = null;
		this.gcBpCatalog.Visible = true;
		this.gcBpCatalog.VisibleIndex = 3;
		this.rBpCatalog.AutoHeight = false;
		this.rBpCatalog.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rBpCatalog.Name = "rBpCatalog";
		this.rBpCatalog.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBpCatalog_ButtonClick);
		this.gcDate.Caption = "Date";
		this.gcDate.CurrencyDateField = null;
		this.gcDate.CurrencyField = null;
		this.gcDate.CurrencyFixed = null;
		this.gcDate.CurrencyFixedRate = 0.0;
		this.gcDate.CurrencyRateField = null;
		this.gcDate.FieldName = "Date";
		this.gcDate.Name = "gcDate";
		this.gcDate.PrimaryKey = false;
		this.gcDate.UnitField = null;
		this.gcLinePrice.Caption = "Price";
		this.gcLinePrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcLinePrice.CurrencyDateField = null;
		this.gcLinePrice.CurrencyField = null;
		this.gcLinePrice.CurrencyFixed = null;
		this.gcLinePrice.CurrencyFixedRate = 0.0;
		this.gcLinePrice.CurrencyRateField = null;
		this.gcLinePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLinePrice.FieldName = "Price";
		this.gcLinePrice.Name = "gcLinePrice";
		this.gcLinePrice.OptionsColumn.AllowEdit = false;
		this.gcLinePrice.OptionsColumn.ReadOnly = true;
		this.gcLinePrice.PrimaryKey = false;
		this.gcLinePrice.UnitField = null;
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		this.gcAbsID.Caption = "AbsID";
		this.gcAbsID.CurrencyDateField = null;
		this.gcAbsID.CurrencyField = null;
		this.gcAbsID.CurrencyFixed = null;
		this.gcAbsID.CurrencyFixedRate = 0.0;
		this.gcAbsID.CurrencyRateField = null;
		this.gcAbsID.FieldName = "Code";
		this.gcAbsID.Name = "gcAbsID";
		this.gcAbsID.PrimaryKey = false;
		this.gcAbsID.UnitField = null;
		this.gcLineID.Caption = "LineID";
		this.gcLineID.CurrencyDateField = null;
		this.gcLineID.CurrencyField = null;
		this.gcLineID.CurrencyFixed = null;
		this.gcLineID.CurrencyFixedRate = 0.0;
		this.gcLineID.CurrencyRateField = null;
		this.gcLineID.FieldName = "LineID";
		this.gcLineID.Name = "gcLineID";
		this.gcLineID.PrimaryKey = false;
		this.gcLineID.UnitField = null;
		this.gcLineBP.Caption = "BP Code";
		this.gcLineBP.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.gcLineBP.ColumnEdit = this.rLineBP;
		this.gcLineBP.CurrencyDateField = null;
		this.gcLineBP.CurrencyField = null;
		this.gcLineBP.CurrencyFixed = null;
		this.gcLineBP.CurrencyFixedRate = 0.0;
		this.gcLineBP.CurrencyRateField = null;
		this.gcLineBP.FieldName = "BPCode";
		this.gcLineBP.Name = "gcLineBP";
		this.gcLineBP.PrimaryKey = false;
		this.gcLineBP.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineBP.UnitField = null;
		this.gcLineBP.Visible = true;
		this.gcLineBP.VisibleIndex = 4;
		this.gcLineBP.Width = 96;
		this.rLineBP.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rLineBP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineBP.Name = "rLineBP";
		this.rLineBP.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineBP_ButtonClick);
		this.gcLineSpl.Caption = "Sales employee";
		this.gcLineSpl.ColumnEdit = this.rLineSpl;
		this.gcLineSpl.CurrencyDateField = null;
		this.gcLineSpl.CurrencyField = null;
		this.gcLineSpl.CurrencyFixed = null;
		this.gcLineSpl.CurrencyFixedRate = 0.0;
		this.gcLineSpl.CurrencyRateField = null;
		this.gcLineSpl.FieldName = "SplCode";
		this.gcLineSpl.Name = "gcLineSpl";
		this.gcLineSpl.PrimaryKey = false;
		this.gcLineSpl.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLineSpl.UnitField = null;
		this.gcLineSpl.Visible = true;
		this.gcLineSpl.VisibleIndex = 5;
		this.gcLineSpl.Width = 100;
		this.rLineSpl.AutoHeight = false;
		this.rLineSpl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineSpl.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name11"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Name12", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineSpl.DisplayMember = "SlpName";
		this.rLineSpl.Name = "rLineSpl";
		this.rLineSpl.NullText = "";
		this.rLineSpl.ShowFooter = false;
		this.rLineSpl.ShowHeader = false;
		this.rLineSpl.ValueMember = "SlpCode";
		this.gcLineCurrency.Caption = "Currency";
		this.gcLineCurrency.CurrencyDateField = null;
		this.gcLineCurrency.CurrencyField = null;
		this.gcLineCurrency.CurrencyFixed = null;
		this.gcLineCurrency.CurrencyFixedRate = 0.0;
		this.gcLineCurrency.CurrencyRateField = null;
		this.gcLineCurrency.FieldName = "Currency";
		this.gcLineCurrency.Name = "gcLineCurrency";
		this.gcLineCurrency.PrimaryKey = false;
		this.gcLineCurrency.UnitField = null;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.tabPrice.Controls.Add(this.grdLinePrice);
		this.tabPrice.EditControl = null;
		this.tabPrice.EditValue = "Revenue Forecast";
		this.tabPrice.FieldName = "tabPrice";
		this.tabPrice.IsSearchControl = false;
		this.tabPrice.Name = "tabPrice";
		this.tabPrice.ReadOnly = false;
		this.tabPrice.SettingInVisible = false;
		this.tabPrice.SettingReadonly = false;
		this.tabPrice.Size = new System.Drawing.Size(863, 166);
		this.tabPrice.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPrice.Text = "Revenue Forecast";
		this.grdLinePrice.AllowPasteExcel = false;
		this.grdLinePrice.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grdLinePrice.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLinePrice.Location = new System.Drawing.Point(0, 0);
		this.grdLinePrice.MainView = this.gvLinePrice;
		this.grdLinePrice.Name = "grdLinePrice";
		this.grdLinePrice.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rItemCodeLinePrice, this.rWhsCodeLinePrice, this.rBpCodeLinePrice, this.rSaleLinePrice, this.rCurrency });
		this.grdLinePrice.Size = new System.Drawing.Size(863, 166);
		this.grdLinePrice.TabIndex = 1;
		this.grdLinePrice.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLinePrice });
		this.gvLinePrice.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvLinePrice.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvLinePrice.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvLinePrice.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gvLinePrice.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLinePrice.ColumnKey = "ItemCode";
		this.gvLinePrice.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gcItemCodeLinePrice, this.gcItemNameLinePrice, this.gcUoMLinePrice, this.gcWhsCodeLinePrice, this.gcDateLinePrice, this.gcPriceLinePrice, this.gcQuantityLinePrice, this.gcIdLinePrice, this.gcLineIdPrice, this.gcBpCodeLinePrice,
			this.gcSaleEmployeLinePrice, this.gcCurrency
		});
		this.gvLinePrice.DisableFormSetting = false;
		this.gvLinePrice.DisplayException = false;
		this.gvLinePrice.FieldCurrency = null;
		this.gvLinePrice.FieldRate = null;
		this.gvLinePrice.FormId = "oAdvanceForecast";
		this.gvLinePrice.GridControl = this.grdLinePrice;
		this.gvLinePrice.IndicatorWidth = 50;
		this.gvLinePrice.IsApzTable = false;
		this.gvLinePrice.IsInsertNewRow = false;
		this.gvLinePrice.IsLoadingData = false;
		this.gvLinePrice.ItemId = "PFCT1";
		this.gvLinePrice.LstColumnAllowBatch = null;
		this.gvLinePrice.LstColumnNotAllowBatch = null;
		this.gvLinePrice.LstColumnNotEditable = new string[2] { "Price", "UoM" };
		this.gvLinePrice.LstColumNotSetting = null;
		this.gvLinePrice.Name = "gvLinePrice";
		this.gvLinePrice.OptionsFind.AllowFindPanel = false;
		this.gvLinePrice.OptionsSelection.MultiSelect = true;
		this.gvLinePrice.OptionsView.ColumnAutoWidth = false;
		this.gvLinePrice.OptionsView.ShowGroupPanel = false;
		this.gvLinePrice.UseBatchMod = false;
		this.gvLinePrice.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvLine_RowCellStyle);
		this.gvLinePrice.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanged);
		this.gvLinePrice.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvLine_CustomColumnDisplayText);
		this.gcItemCodeLinePrice.Caption = "Item No.";
		this.gcItemCodeLinePrice.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gcItemCodeLinePrice.ColumnEdit = this.rItemCodeLinePrice;
		this.gcItemCodeLinePrice.CurrencyDateField = null;
		this.gcItemCodeLinePrice.CurrencyField = null;
		this.gcItemCodeLinePrice.CurrencyFixed = null;
		this.gcItemCodeLinePrice.CurrencyFixedRate = 0.0;
		this.gcItemCodeLinePrice.CurrencyRateField = null;
		this.gcItemCodeLinePrice.FieldName = "ItemCode";
		this.gcItemCodeLinePrice.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCodeLinePrice.Name = "gcItemCodeLinePrice";
		this.gcItemCodeLinePrice.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcItemCodeLinePrice.PrimaryKey = false;
		this.gcItemCodeLinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCodeLinePrice.UnitField = null;
		this.gcItemCodeLinePrice.Visible = true;
		this.gcItemCodeLinePrice.VisibleIndex = 0;
		this.gcItemCodeLinePrice.Width = 94;
		this.rItemCodeLinePrice.AutoHeight = false;
		this.rItemCodeLinePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCodeLinePrice.Name = "rItemCodeLinePrice";
		this.rItemCodeLinePrice.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcItemNameLinePrice.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gcItemNameLinePrice.AppearanceCell.Options.UseBackColor = true;
		this.gcItemNameLinePrice.Caption = "Item Description";
		this.gcItemNameLinePrice.CurrencyDateField = null;
		this.gcItemNameLinePrice.CurrencyField = null;
		this.gcItemNameLinePrice.CurrencyFixed = null;
		this.gcItemNameLinePrice.CurrencyFixedRate = 0.0;
		this.gcItemNameLinePrice.CurrencyRateField = null;
		this.gcItemNameLinePrice.FieldName = "ItemName";
		this.gcItemNameLinePrice.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemNameLinePrice.Name = "gcItemNameLinePrice";
		this.gcItemNameLinePrice.OptionsColumn.ReadOnly = true;
		this.gcItemNameLinePrice.PrimaryKey = false;
		this.gcItemNameLinePrice.UnitField = null;
		this.gcItemNameLinePrice.Visible = true;
		this.gcItemNameLinePrice.VisibleIndex = 1;
		this.gcItemNameLinePrice.Width = 88;
		this.gcUoMLinePrice.Caption = "Uom Name";
		this.gcUoMLinePrice.CurrencyDateField = null;
		this.gcUoMLinePrice.CurrencyField = null;
		this.gcUoMLinePrice.CurrencyFixed = null;
		this.gcUoMLinePrice.CurrencyFixedRate = 0.0;
		this.gcUoMLinePrice.CurrencyRateField = null;
		this.gcUoMLinePrice.FieldName = "InvntryUom";
		this.gcUoMLinePrice.Name = "gcUoMLinePrice";
		this.gcUoMLinePrice.OptionsColumn.AllowEdit = false;
		this.gcUoMLinePrice.PrimaryKey = false;
		this.gcUoMLinePrice.UnitField = null;
		this.gcWhsCodeLinePrice.Caption = "WhsCode";
		this.gcWhsCodeLinePrice.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.gcWhsCodeLinePrice.ColumnEdit = this.rWhsCodeLinePrice;
		this.gcWhsCodeLinePrice.CurrencyDateField = null;
		this.gcWhsCodeLinePrice.CurrencyField = null;
		this.gcWhsCodeLinePrice.CurrencyFixed = null;
		this.gcWhsCodeLinePrice.CurrencyFixedRate = 0.0;
		this.gcWhsCodeLinePrice.CurrencyRateField = null;
		this.gcWhsCodeLinePrice.FieldName = "WhsCode";
		this.gcWhsCodeLinePrice.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcWhsCodeLinePrice.Name = "gcWhsCodeLinePrice";
		this.gcWhsCodeLinePrice.PrimaryKey = false;
		this.gcWhsCodeLinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWhsCodeLinePrice.UnitField = null;
		this.gcWhsCodeLinePrice.Visible = true;
		this.gcWhsCodeLinePrice.VisibleIndex = 2;
		this.gcWhsCodeLinePrice.Width = 95;
		this.rWhsCodeLinePrice.AutoHeight = false;
		this.rWhsCodeLinePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhsCodeLinePrice.Name = "rWhsCodeLinePrice";
		this.rWhsCodeLinePrice.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhsCode_ButtonClick);
		this.gcDateLinePrice.Caption = "Date";
		this.gcDateLinePrice.CurrencyDateField = null;
		this.gcDateLinePrice.CurrencyField = null;
		this.gcDateLinePrice.CurrencyFixed = null;
		this.gcDateLinePrice.CurrencyFixedRate = 0.0;
		this.gcDateLinePrice.CurrencyRateField = null;
		this.gcDateLinePrice.FieldName = "Date";
		this.gcDateLinePrice.Name = "gcDateLinePrice";
		this.gcDateLinePrice.PrimaryKey = false;
		this.gcDateLinePrice.UnitField = null;
		this.gcPriceLinePrice.Caption = "Price";
		this.gcPriceLinePrice.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcPriceLinePrice.CurrencyDateField = null;
		this.gcPriceLinePrice.CurrencyField = null;
		this.gcPriceLinePrice.CurrencyFixed = null;
		this.gcPriceLinePrice.CurrencyFixedRate = 0.0;
		this.gcPriceLinePrice.CurrencyRateField = null;
		this.gcPriceLinePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPriceLinePrice.FieldName = "Price";
		this.gcPriceLinePrice.Name = "gcPriceLinePrice";
		this.gcPriceLinePrice.OptionsColumn.AllowEdit = false;
		this.gcPriceLinePrice.OptionsColumn.ReadOnly = true;
		this.gcPriceLinePrice.PrimaryKey = false;
		this.gcPriceLinePrice.UnitField = null;
		this.gcQuantityLinePrice.Caption = "Quantity";
		this.gcQuantityLinePrice.CurrencyDateField = null;
		this.gcQuantityLinePrice.CurrencyField = null;
		this.gcQuantityLinePrice.CurrencyFixed = null;
		this.gcQuantityLinePrice.CurrencyFixedRate = 0.0;
		this.gcQuantityLinePrice.CurrencyRateField = null;
		this.gcQuantityLinePrice.FieldName = "Quantity";
		this.gcQuantityLinePrice.Name = "gcQuantityLinePrice";
		this.gcQuantityLinePrice.PrimaryKey = false;
		this.gcQuantityLinePrice.UnitField = null;
		this.gcIdLinePrice.Caption = "AbsID";
		this.gcIdLinePrice.CurrencyDateField = null;
		this.gcIdLinePrice.CurrencyField = null;
		this.gcIdLinePrice.CurrencyFixed = null;
		this.gcIdLinePrice.CurrencyFixedRate = 0.0;
		this.gcIdLinePrice.CurrencyRateField = null;
		this.gcIdLinePrice.FieldName = "Code";
		this.gcIdLinePrice.Name = "gcIdLinePrice";
		this.gcIdLinePrice.PrimaryKey = false;
		this.gcIdLinePrice.UnitField = null;
		this.gcLineIdPrice.Caption = "LineID";
		this.gcLineIdPrice.CurrencyDateField = null;
		this.gcLineIdPrice.CurrencyField = null;
		this.gcLineIdPrice.CurrencyFixed = null;
		this.gcLineIdPrice.CurrencyFixedRate = 0.0;
		this.gcLineIdPrice.CurrencyRateField = null;
		this.gcLineIdPrice.FieldName = "LineID";
		this.gcLineIdPrice.Name = "gcLineIdPrice";
		this.gcLineIdPrice.PrimaryKey = false;
		this.gcLineIdPrice.UnitField = null;
		this.gcBpCodeLinePrice.Caption = "BP Code";
		this.gcBpCodeLinePrice.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.gcBpCodeLinePrice.ColumnEdit = this.rBpCodeLinePrice;
		this.gcBpCodeLinePrice.CurrencyDateField = null;
		this.gcBpCodeLinePrice.CurrencyField = null;
		this.gcBpCodeLinePrice.CurrencyFixed = null;
		this.gcBpCodeLinePrice.CurrencyFixedRate = 0.0;
		this.gcBpCodeLinePrice.CurrencyRateField = null;
		this.gcBpCodeLinePrice.FieldName = "BPCode";
		this.gcBpCodeLinePrice.Name = "gcBpCodeLinePrice";
		this.gcBpCodeLinePrice.PrimaryKey = false;
		this.gcBpCodeLinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBpCodeLinePrice.UnitField = null;
		this.gcBpCodeLinePrice.Visible = true;
		this.gcBpCodeLinePrice.VisibleIndex = 3;
		this.gcBpCodeLinePrice.Width = 96;
		this.rBpCodeLinePrice.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rBpCodeLinePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBpCodeLinePrice.Name = "rBpCodeLinePrice";
		this.rBpCodeLinePrice.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineBP_ButtonClick);
		this.gcSaleEmployeLinePrice.Caption = "Sales employee";
		this.gcSaleEmployeLinePrice.ColumnEdit = this.rSaleLinePrice;
		this.gcSaleEmployeLinePrice.CurrencyDateField = null;
		this.gcSaleEmployeLinePrice.CurrencyField = null;
		this.gcSaleEmployeLinePrice.CurrencyFixed = null;
		this.gcSaleEmployeLinePrice.CurrencyFixedRate = 0.0;
		this.gcSaleEmployeLinePrice.CurrencyRateField = null;
		this.gcSaleEmployeLinePrice.FieldName = "SplCode";
		this.gcSaleEmployeLinePrice.Name = "gcSaleEmployeLinePrice";
		this.gcSaleEmployeLinePrice.PrimaryKey = false;
		this.gcSaleEmployeLinePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcSaleEmployeLinePrice.UnitField = null;
		this.gcSaleEmployeLinePrice.Visible = true;
		this.gcSaleEmployeLinePrice.VisibleIndex = 4;
		this.gcSaleEmployeLinePrice.Width = 100;
		this.rSaleLinePrice.AutoHeight = false;
		this.rSaleLinePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rSaleLinePrice.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name11"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Name12", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rSaleLinePrice.DisplayMember = "SlpName";
		this.rSaleLinePrice.Name = "rSaleLinePrice";
		this.rSaleLinePrice.NullText = "";
		this.rSaleLinePrice.ShowFooter = false;
		this.rSaleLinePrice.ShowHeader = false;
		this.rSaleLinePrice.ValueMember = "SlpCode";
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.ColumnEdit = this.rCurrency;
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		this.rCurrency.AutoHeight = false;
		this.rCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCurrency.DisplayMember = "Code";
		this.rCurrency.Name = "rCurrency";
		this.rCurrency.NullText = "";
		this.rCurrency.ValueMember = "Code";
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(408, 3);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(70, 20);
		this.btnSearch.TabIndex = 4;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.txt_FCCode.EditControl = null;
		this.txt_FCCode.FieldName = null;
		this.txt_FCCode.IsSearchControl = false;
		this.txt_FCCode.Location = new System.Drawing.Point(92, 3);
		this.txt_FCCode.Name = "txt_FCCode";
		this.txt_FCCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_FCCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_FCCode.SettingInVisible = false;
		this.txt_FCCode.SettingReadonly = false;
		this.txt_FCCode.Size = new System.Drawing.Size(120, 20);
		this.txt_FCCode.TabIndex = 1;
		this.lb_View.EditControl = null;
		this.lb_View.EditValue = "View";
		this.lb_View.FieldName = null;
		this.lb_View.HideWhenEmpty = false;
		this.lb_View.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_View.Location = new System.Drawing.Point(220, 5);
		this.lb_View.Name = "lb_View";
		this.lb_View.ReadOnly = false;
		this.lb_View.SettingInVisible = false;
		this.lb_View.SettingReadonly = false;
		this.lb_View.ShowUnderLine = true;
		this.lb_View.Size = new System.Drawing.Size(60, 20);
		this.lb_View.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_View.TabIndex = 0;
		this.lb_View.Utext = "View";
		this.lb_BpCode.EditControl = null;
		this.lb_BpCode.EditValue = "Forecast Code";
		this.lb_BpCode.FieldName = null;
		this.lb_BpCode.HideWhenEmpty = false;
		this.lb_BpCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BpCode.Location = new System.Drawing.Point(6, 5);
		this.lb_BpCode.Name = "lb_BpCode";
		this.lb_BpCode.ReadOnly = false;
		this.lb_BpCode.SettingInVisible = false;
		this.lb_BpCode.SettingReadonly = false;
		this.lb_BpCode.ShowUnderLine = true;
		this.lb_BpCode.Size = new System.Drawing.Size(90, 20);
		this.lb_BpCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BpCode.TabIndex = 0;
		this.lb_BpCode.Utext = "Forecast Code";
		this.txtView.Location = new System.Drawing.Point(279, 3);
		this.txtView.Name = "txtView";
		this.txtView.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtView.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtView.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name4")
		});
		this.txtView.Properties.DisplayMember = "Name";
		this.txtView.Properties.NullText = "";
		this.txtView.Properties.ShowFooter = false;
		this.txtView.Properties.ShowHeader = false;
		this.txtView.Properties.ValueMember = "Value";
		this.txtView.Size = new System.Drawing.Size(120, 20);
		this.txtView.TabIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(872, 462);
		base.Controls.Add(this.pc_pannel);
		base.FormId = "oAdvanceForecast";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_AdvanceForecast.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdvanceForecast";
		this.Text = "Forecast";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AdvanceForecast_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pc_pannel).EndInit();
		this.pc_pannel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.scc_SplitContainerControl).EndInit();
		this.scc_SplitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSpl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabForcast).EndInit();
		this.xtraTabForcast.ResumeLayout(false);
		this.TabQuantity.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLineQuantity).EndInit();
		this.contextMenuStrip_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLineQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCatalog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineBP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineSpl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		this.tabPrice.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCodeLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCodeLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCodeLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSaleLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FCCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtView.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
