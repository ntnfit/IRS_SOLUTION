using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.FormTemplate;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.FormTemplate;

public class frm_TempateControlGrid : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _objectId;

	private DataTable _openDataSource;

	private DataSet _wizardData = new DataSet();

	private DataTable _wizardDoc = new DataTable();

	private DataTable _wizardContainer = new DataTable();

	private DataTable _wizardControls = new DataTable();

	private DataTable _wizardValidValue = new DataTable();

	private readonly DataTable _validValueStruct = new DataTable();

	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControl layoutControl2;

	private LayoutControlGroup layoutControlGroup2;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControl Tab_Main;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private GridControlEx gridControlEx1;

	private GridViewEx gridViewEx1;

	private GridColumnEx gridColumnEx1;

	public int TemplateId { get; set; }

	public frm_TempateControlGrid()
	{
		InitializeComponent();
		OnLoadAsync += frm_PurchaseRequestControl_Load;
	}

	public override void RaiseControlValueChanged(ControlChangedEventArgs e)
	{
		base.RaiseControlValueChanged(e);
		try
		{
			_ = from t in _wizardContainer.AsEnumerable()
				select Function.ToString(t["VisibleExpression"]);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_TempateControlGrid(int templateId)
	{
		InitializeComponent();
		OnLoadAsync += frm_PurchaseRequestControl_Load;
		TemplateId = templateId;
	}

	public frm_TempateControlGrid(int templateId, int objectId)
	{
		InitializeComponent();
		OnLoadAsync += frm_PurchaseRequestControl_Load;
		TemplateId = templateId;
		_objectId = objectId;
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_OK.Text = Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_OK.Text = Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_PurchaseRequestControl_Load(object sender, EventArgs e)
	{
		try
		{
			_validValueStruct.Columns.Add("Code", typeof(string));
			_validValueStruct.Columns.Add("Name", typeof(string));
			if (TemplateId != 0)
			{
				FormId = "W" + TemplateId;
			}
			_wizardData = await _httpClient.FormWizard.GetById(TemplateId);
			Text = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["Caption"]);
			if (Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["AcceptButton"]) == "Y")
			{
				btn_OK.Visible = true;
				AcceptButton = btn_OK;
			}
			else
			{
				btn_OK.Visible = false;
				AcceptButton = null;
			}
			if (Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["CancelButton"]) == "Y")
			{
				btn_Cancel.Visible = true;
				CancelButton = btn_Cancel;
			}
			else
			{
				btn_Cancel.Visible = false;
				CancelButton = null;
			}
			if (_wizardData.Tables.Contains("FWZ1"))
			{
				_wizardContainer = _wizardData.Tables["FWZ1"];
				DataRow header = _wizardContainer.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ContainerType"]) == "F");
				if (header != null)
				{
					if (_wizardData.Tables.Contains("FWZ2"))
					{
						DataRow[] array = (from t in _wizardData.Tables["FWZ2"].AsEnumerable()
							where Function.ParseInt(t["ContainerId"]) == Function.ParseInt(header["ContainerId"])
							select t).ToArray();
						if (array.Length != 0)
						{
							foreach (DataRow item in array.OrderBy((DataRow t) => Function.ParseInt(t["VisIndex"])))
							{
								bool flag = Function.ToString(item["Visible"]) != "N";
								LayoutControlItem layoutControlItem = ((Function.ToString(item["IsLeft"]) == "N") ? layoutControl2.AddItem() : layoutControl1.AddItem());
								layoutControlItem.Text = Function.ToString(item["Caption"]);
								layoutControlItem.TextToControlDistance = 10;
								layoutControlItem.TextAlignMode = TextAlignModeItem.CustomSize;
								layoutControlItem.TextSize = new Size(110, 25);
								layoutControlItem.AppearanceItemCaption.Font = new Font(layoutControlItem.AppearanceItemCaption.Font.FontFamily, layoutControlItem.AppearanceItemCaption.Font.Size, FontStyle.Regular);
								AddControlToLayoutItem(item, layoutControlItem);
								layoutControlItem.Visibility = ((!flag) ? LayoutVisibility.Never : LayoutVisibility.Always);
								layoutControlItem.Name = "layout_" + Function.ToString(item["FieldName"]);
							}
							int num = (from t in array.AsEnumerable()
								where Function.ToString(t["IsLeft"]) != "Y"
								select t).ToArray().Length;
							int num2 = array.Length - num;
							int num3 = ((num2 > num) ? num2 : num);
							splitContainerControl1.SplitterPosition = (num3 + 1) * 27;
						}
					}
					DataTable dataTable = await GetData(header);
					dataTable.Rows.Add(dataTable.NewRow());
					DataSource = dataTable;
				}
				DataRow[] array2 = (from t in _wizardContainer.AsEnumerable()
					where Function.ToString(t["ContainerType"]) == "T"
					select t).ToArray();
				if (array2.Length != 0)
				{
					DataRow[] array3 = array2;
					foreach (DataRow dataRow in array3)
					{
						int containerId = Function.ParseInt(dataRow["ContainerId"]);
						string text = Function.ToString(dataRow["ItemId"]);
						GridControlEx grd = new GridControlEx
						{
							AllowPasteExcel = true
						};
						string viewCaption = Function.ToString(dataRow["Caption"]);
						grd.Dock = DockStyle.Fill;
						grd.Size = new Size(966, 385);
						GridViewEx gv = new GridViewEx();
						grd.ViewCollection.AddRange(new BaseView[1] { gv });
						gv.GridControl = grd;
						grd.MainView = gv;
						gv.FormId = FormId;
						gv.ItemId = (string.IsNullOrEmpty(text) ? ("W" + containerId) : text);
						gv.ViewCaption = viewCaption;
						gv.Name = gv.ItemId;
						gv.RowCellStyle += Gv_RowCellStyle;
						gv.Tag = containerId;
						DataRow[] array4 = (from t in _wizardData.Tables["FWZ2"].AsEnumerable()
							where Function.ParseInt(t["ContainerId"]) == containerId
							select t).ToArray();
						gv.BeginUpdate();
						DataRow[] array5 = array4;
						foreach (DataRow dataRow2 in array5)
						{
							string text2 = Function.ToString(dataRow2["PrimaryKey"]);
							GridColumnEx gridColumnEx = new GridColumnEx();
							gridColumnEx.FieldName = Function.ToString(dataRow2["FieldName"]);
							gridColumnEx.Caption = Function.ToString(dataRow2["Caption"]);
							gridColumnEx.ShowUnboundExpressionMenu = true;
							gridColumnEx.Visible = Function.ToString(dataRow2["Visible"]) == "Y";
							gridColumnEx.Width = 100;
							gridColumnEx.PrimaryKey = text2 == "Y";
							gridColumnEx.OptionsColumn.ReadOnly = Function.ToString(dataRow2["IsReadOnly"]) == "Y";
							gridColumnEx.OptionsColumn.AllowEdit = Function.ToString(dataRow2["Editable"]) == "Y";
							GridColumnEx gridColumnEx2 = gridColumnEx;
							if (gridColumnEx2.Visible)
							{
								gridColumnEx2.VisibleIndex = Function.ParseInt(dataRow2["VisIndex"]);
							}
							string text3 = Function.ToString(dataRow2["DataType"]);
							Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(dataRow2["Editor"]);
							Enums.TypeCFL typeCFL = (Enums.TypeCFL)Function.ParseInt(dataRow2["CFLType"]);
							if (!(text3 == "A"))
							{
								if (text3 == "B")
								{
									gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Amount;
								}
							}
							else
							{
								switch (formTemplateEditorType)
								{
								case Enums.FormTemplateEditorType.ChooseFromList:
								{
									RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL("ItemCode", "ItemCode");
									repositoryItemButtonEditCFL.TypeCFL = typeCFL;
									gridColumnEx2.ColumnEdit = repositoryItemButtonEditCFL;
									break;
								}
								case Enums.FormTemplateEditorType.CheckEdit:
								{
									RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
									repositoryItemCheckEdit.ValueChecked = "Y";
									repositoryItemCheckEdit.ValueUnchecked = "N";
									repositoryItemCheckEdit.ValueGrayed = "N";
									gridColumnEx2.ColumnEdit = repositoryItemCheckEdit;
									break;
								}
								}
							}
							gv.Columns.Add(gridColumnEx2);
						}
						gv.CellValueChangedAsync += async (CellValueChangedEventArgs ev) =>
						{
							await Gv_CellValueChangedAsync(gv, ev);
						};
						gv.CellValueChanged += Gv_CellValueChanged;
						gv.EndUpdate();
						XtraTabPageEx xtraTabPageEx = new XtraTabPageEx();
						xtraTabPageEx.Size = new Size(966, 385);
						xtraTabPageEx.SuspendLayout();
						xtraTabPageEx.Controls.Add(grd);
						xtraTabPageEx.SourceType = Enums.DataSourceType.TittleValue;
						xtraTabPageEx.Text = viewCaption;
						Tab_Main.TabPages.Add(xtraTabPageEx);
						xtraTabPageEx.ResumeLayout(performLayout: false);
						DataTable dataTable2 = await GetData(dataRow);
						dataTable2.Rows.Add(dataTable2.NewRow());
						grd.DataSource = dataTable2;
						grd.RefreshDataSource();
					}
				}
			}
			if (_objectId != 0)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly)
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

	private void Gv_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private async Task Gv_CellValueChangedAsync(GridViewEx gv, CellValueChangedEventArgs e)
	{
		try
		{
			GridColumnEx col = e.Column as GridColumnEx;
			if (col != null && col.PrimaryKey && gv.GridControl.DataSource is DataTable dataTable && dataTable.Columns.Count > 0 && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t[col.FieldName]))) == null)
			{
				dataTable.Rows.Add(dataTable.NewRow());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetData(DataRow header)
	{
		string qtxt = Function.ToString(header["DataString"]);
		DataTable dataTable = await _httpClient.Cfl.GetQueryParams(qtxt);
		List<QueryParamater> list = new List<QueryParamater>();
		foreach (DataRow row in dataTable.Rows)
		{
			QueryParamater queryParamater = new QueryParamater
			{
				ParaName = Function.ToString(row["Para"])
			};
			queryParamater.Value = _objectId;
			list.Add(queryParamater);
		}
		DataSet dataSet = _httpClient.Commons.ExecuteSql(qtxt, list);
		if (dataSet != null && dataSet.Tables.Count > 1)
		{
			DataTable dataTable2 = dataSet.Tables["Message"];
			if (dataTable2.IsNotNull())
			{
				string text = Function.ToString(dataTable2.Rows[0]["MsgType"]);
				string msg = Function.ToString(dataTable2.Rows[0]["Message"]);
				if (text == "5xx")
				{
					WinCommons.Error(Enums.MessageCode.None, msg);
				}
			}
			return dataSet.Tables[1];
		}
		WinCommons.Error(Enums.MessageCode.CannotGetData, "");
		return new DataTable();
	}

	private void AddControlToLayoutItem(DataRow item, LayoutControlItem layoutItem)
	{
		try
		{
			int num = layoutControl1.Width - 50;
			string text = Function.ToString(item["DataType"]);
			Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(item["Editor"]);
			string text2 = Function.ToString(item["FieldName"]);
			bool flag = Function.ToString(item["PrimaryKey"]) == "Y";
			switch (text)
			{
			case "A":
				switch (formTemplateEditorType)
				{
				case Enums.FormTemplateEditorType.ChooseFromList:
				{
					ButtonEditCFL buttonEditCFL = new ButtonEditCFL();
					buttonEditCFL.FieldName = text2;
					buttonEditCFL.Name = text2;
					buttonEditCFL.StyleController = layoutControl1;
					layoutItem.Control = buttonEditCFL;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize3 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize3;
					Enums.TypeCFL typeCFL = (Enums.TypeCFL)Function.ParseInt(item["CFLType"]);
					buttonEditCFL.Properties.TypeCFL = typeCFL;
					if (flag)
					{
						buttonEditCFL.EditValue = _objectId;
					}
					break;
				}
				case Enums.FormTemplateEditorType.ValidValue:
				{
					LookUpEditEx lookUpEditEx = new LookUpEditEx();
					lookUpEditEx.Name = text2;
					lookUpEditEx.FieldName = text2;
					lookUpEditEx.StyleController = layoutControl1;
					layoutItem.Control = lookUpEditEx;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize4 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize4;
					if (Function.ToString(item["ValidType"]) == "F")
					{
						DataRow[] array = (from t in _wizardData.Tables["FWZ3"].AsEnumerable()
							where Function.ParseInt(t["LineId"]) == Function.ParseInt(item["LineId"])
							select t).ToArray();
						if (array.Length != 0)
						{
							DataTable dataTable = _validValueStruct.Clone();
							DataRow[] array2 = array;
							foreach (DataRow dataRow in array2)
							{
								DataRow dataRow2 = dataTable.NewRow();
								dataRow2["Code"] = Function.ToString(dataRow["Code"]);
								dataRow2["Name"] = Function.ToString(dataRow["Name"]);
								dataTable.Rows.Add(dataRow2);
							}
							lookUpEditEx.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
						}
					}
					if (flag)
					{
						lookUpEditEx.EditValue = _objectId;
					}
					break;
				}
				default:
				{
					TextEditEx textEditEx = new TextEditEx();
					textEditEx.FieldName = text2;
					textEditEx.Name = text2;
					textEditEx.StyleController = layoutControl1;
					layoutItem.Control = textEditEx;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize2 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize2;
					if (flag)
					{
						textEditEx.EditValue = _objectId;
					}
					break;
				}
				case Enums.FormTemplateEditorType.CheckEdit:
					break;
				}
				break;
			case "D":
			{
				DateEditEx dateEditEx = new DateEditEx();
				dateEditEx.Name = text2;
				dateEditEx.FieldName = text2;
				dateEditEx.Properties.DisplayFormat.FormatType = FormatType.DateTime;
				dateEditEx.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				dateEditEx.StyleController = layoutControl1;
				layoutItem.Control = dateEditEx;
				dateEditEx.StyleController = layoutControl1;
				layoutItem.Control = dateEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize6 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize6;
				break;
			}
			case "F":
			{
				CalcEditEx calcEditEx2 = new CalcEditEx();
				calcEditEx2.FieldName = text2;
				calcEditEx2.Name = text2;
				calcEditEx2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEditEx2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				calcEditEx2.StyleController = layoutControl1;
				layoutItem.Control = calcEditEx2;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize7 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize7;
				if (flag)
				{
					calcEditEx2.EditValue = _objectId;
				}
				break;
			}
			case "M":
			{
				MemoExEditEx memoExEditEx = new MemoExEditEx();
				memoExEditEx.FieldName = text2;
				memoExEditEx.Name = text2;
				memoExEditEx.StyleController = layoutControl1;
				layoutItem.Control = memoExEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize5 = (layoutItem.MaxSize = new Size(num, 220));
				layoutItem.MinSize = minSize5;
				if (flag)
				{
					memoExEditEx.EditValue = _objectId;
				}
				break;
			}
			case "N":
			{
				CalcEditEx calcEditEx = new CalcEditEx();
				calcEditEx.FieldName = text2;
				calcEditEx.Name = text2;
				calcEditEx.StyleController = layoutControl1;
				calcEditEx.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEditEx.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				layoutItem.Control = calcEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize;
				if (flag)
				{
					calcEditEx.EditValue = _objectId;
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override bool SetVisibleControl(string fieldName, bool isVisible)
	{
		try
		{
			foreach (object item in layoutControl1.Items)
			{
				if (item is LayoutItem)
				{
					LayoutItem layoutItem = item as LayoutItem;
					if (layoutItem.Name == "layout_" + fieldName)
					{
						layoutItem.Visibility = ((!isVisible) ? LayoutVisibility.Never : LayoutVisibility.Always);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return isVisible;
	}

	private void AddControlToLayoutItem(ControlField item, LayoutControlItem layoutItem)
	{
		try
		{
			int num = layoutControl1.Width - 50;
			switch (item.Type)
			{
			case BoFieldTypes.db_Alpha:
				if (item.Editor == Enums.FormTemplateEditorType.ChooseFromList)
				{
					ButtonEditCFL buttonEditCFL = new ButtonEditCFL();
					buttonEditCFL.Name = item.Name;
					buttonEditCFL.StyleController = layoutControl1;
					layoutItem.Control = buttonEditCFL;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize5 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize5;
				}
				else if (item.Editor == Enums.FormTemplateEditorType.ValidValue)
				{
					LookUpEditEx lookUpEditEx = new LookUpEditEx();
					lookUpEditEx.Name = item.Name;
					lookUpEditEx.StyleController = layoutControl1;
					layoutItem.Control = lookUpEditEx;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize6 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize6;
					if (item.ValidValues != null)
					{
						lookUpEditEx.SetDataSource(item.ValidValues);
					}
				}
				else
				{
					TextEdit textEdit = new TextEdit();
					textEdit.Name = item.Name;
					textEdit.StyleController = layoutControl1;
					layoutItem.Control = textEdit;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize7 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize7;
				}
				break;
			case BoFieldTypes.db_Date:
			{
				DateEdit dateEdit = new DateEdit();
				dateEdit.Name = item.Name;
				dateEdit.Properties.DisplayFormat.FormatType = FormatType.DateTime;
				dateEdit.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				dateEdit.StyleController = layoutControl1;
				layoutItem.Control = dateEdit;
				dateEdit.StyleController = layoutControl1;
				layoutItem.Control = dateEdit;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize4 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize4;
				break;
			}
			case BoFieldTypes.db_Float:
			{
				CalcEdit calcEdit = new CalcEdit();
				calcEdit.Name = item.Name;
				calcEdit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEdit.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				calcEdit.StyleController = layoutControl1;
				layoutItem.Control = calcEdit;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize3 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize3;
				break;
			}
			case BoFieldTypes.db_Memo:
			{
				MemoExEdit memoExEdit = new MemoExEdit();
				memoExEdit.Name = item.Name;
				memoExEdit.StyleController = layoutControl1;
				layoutItem.Control = memoExEdit;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize2 = (layoutItem.MaxSize = new Size(num, 220));
				layoutItem.MinSize = minSize2;
				break;
			}
			case BoFieldTypes.db_Numeric:
			{
				CalcEditEx calcEditEx = new CalcEditEx();
				calcEditEx.Name = item.Name;
				calcEditEx.StyleController = layoutControl1;
				calcEditEx.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEditEx.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				layoutItem.Control = calcEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void AfterAction(HttpResult ret, bool resetData = false)
	{
		try
		{
			if (!string.IsNullOrEmpty(ret.Message))
			{
				return;
			}
			DataSource = _openDataSource;
			foreach (XtraTabPageEx tabPage in Tab_Main.TabPages)
			{
				foreach (Control control in tabPage.Controls)
				{
					if (control is GridControlEx)
					{
						DataTable dataSourceTable = (control as GridControlEx).DataSourceTable;
						dataSourceTable.Rows.Clear();
						dataSourceTable.AcceptChanges();
						dataSourceTable.Rows.Add(dataSourceTable.NewRow());
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_TempateControlGrid_Shown(object sender, EventArgs e)
	{
		try
		{
			Size = new Size(Size.Width + 1, Size.Height);
			splitContainerControl1.SplitterPosition += 1;
			if (DataSource is DataTable dataTable)
			{
				_openDataSource = dataTable.Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.FormTemplate.frm_TempateControlGrid));
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.Tab_Main = new DevExpress.XtraTab.XtraTabControl();
		this.gridControlEx1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Tab_Main).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl1.Location = new System.Drawing.Point(12, 3);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(380, 270);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(380, 270);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl2.Location = new System.Drawing.Point(495, 3);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.Root = this.layoutControlGroup2;
		this.layoutControl2.Size = new System.Drawing.Size(380, 270);
		this.layoutControl2.TabIndex = 1;
		this.layoutControl2.Text = "layoutControl2";
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Name = "layoutControlGroup1";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(380, 270);
		this.layoutControlGroup2.TextVisible = false;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl1);
		this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Cancel);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_OK);
		this.splitContainerControl1.Panel2.Controls.Add(this.Tab_Main);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(966, 711);
		this.splitContainerControl1.SplitterPosition = 286;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Close";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(871, 389);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(83, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Close";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(10, 389);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Visible = false;
		this.Tab_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.Tab_Main.Location = new System.Drawing.Point(0, 0);
		this.Tab_Main.Name = "Tab_Main";
		this.Tab_Main.Size = new System.Drawing.Size(966, 386);
		this.Tab_Main.TabIndex = 0;
		this.gridControlEx1.AllowPasteExcel = false;
		this.gridControlEx1.ClipboardData = null;
		this.gridControlEx1.Location = new System.Drawing.Point(189, 104);
		this.gridControlEx1.MainView = this.gridViewEx1;
		this.gridControlEx1.Name = "gridControlEx1";
		this.gridControlEx1.Size = new System.Drawing.Size(400, 200);
		this.gridControlEx1.TabIndex = 0;
		this.gridControlEx1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridViewEx1 });
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx1 });
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "oTemplateControlGrid";
		this.gridViewEx1.GridControl = this.gridControlEx1;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = "GRID1";
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.gridColumnEx1.Caption = "gridColumnEx1";
		this.gridColumnEx1.CflType = Apzon.Commons.Enums.Enums.TypeCFL.ListItems;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(966, 711);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oTemplateControlGrid";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TempateControlGrid";
		base.Shown += new System.EventHandler(this.frm_TempateControlGrid_Shown);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Tab_Main).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		base.ResumeLayout(false);
	}
}
