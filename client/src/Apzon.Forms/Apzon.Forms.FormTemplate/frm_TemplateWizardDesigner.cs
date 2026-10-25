using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;

namespace Apzon.Forms.FormTemplate;

public class frm_TemplateWizardDesigner : XtraFormEx
{
	public DataSet DsFormTemplate;

	private string _containerType;

	private int _stackGroup;

	private GridHitInfo _downHitInfo;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Container;

	private GridViewEx gv_Container;

	private SplitContainerControl splitContainerControl2;

	private GridColumnEx gridColumnEx1;

	private SimpleButton btn_AddField;

	private SimpleButton btn_Remove;

	private SimpleButton btn_InsertField;

	private GridControlEx grd_Fields;

	private GridViewEx gv_Fields;

	private VGridControl vgrd_Properties;

	private RepositoryItemLookUpEdit rep_DataType;

	private RepositoryItemLookUpEdit rep_SubType;

	private RepositoryItemCheckEdit rep_IsLeft;

	private RepositoryItemLookUpEdit rep_Editor;

	private RepositoryItemLookUpEdit rep_CFLType;

	private RepositoryItemLookUpEdit rep_ValidType;

	private RepositoryItemCheckEdit rep_KeyNewRow;

	private EditorRow row_Name;

	private EditorRow row_Caption;

	private EditorRow row_DataType;

	private EditorRow row_SubType;

	private EditorRow row_IsLeft;

	private EditorRow row_Editor;

	private EditorRow row_CFLType;

	private EditorRow row_ValidType;

	private EditorRow row_ValueMember;

	private EditorRow row_DisplayMember;

	private EditorRow row_ValidQuery;

	private EditorRow row_KeyNewRow;

	private EditorRow row_Visible;

	private EditorRow row_VisibleIndex;

	private EditorRow row_DefaultValue;

	private EditorRow row_IsReadOnly;

	private EditorRow row_Editable;

	private EditorRow row_PrimaryKey;

	private CategoryRow category_General;

	private CategoryRow category_Editor;

	private CategoryRow category_appearance;

	private GridColumnEx gridColumnEx2;

	private SimpleButton btn_RemoveContainer;

	private SimpleButton btn_InsertContainer;

	private SimpleButton btn_AddContainer;

	private SimpleButton btn_ContainerDown;

	private SimpleButton btn_ContainerUp;

	private SimpleButton btn_FieldDown;

	private SimpleButton btn_FieldUp;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemCheckEdit rep_ShowCaption;

	private RepositoryItemLookUpEdit rep_DisplayType;

	private RepositoryItemMemoExEdit rep_CFLQuery;

	private EditorRow row_CFLQuery;

	private CategoryRow category_Cfl;

	private CategoryRow category_Valid;

	private CategoryRow category_Member;

	private LabelControlEx lbl_ToolTip;

	private SplitContainerControl splitContainerControl_Properties;

	private GridControlEx grd_ValidValue;

	private GridViewEx gv_ValidValue;

	private GridColumnEx gcCode;

	private GridColumnEx gcName;

	private RepositoryItemLookUpEdit rep_DrillDownType;

	private EditorRow row_DrillDownType;

	private EditorRow row_Mandatory;

	private EditorRow row_PasswordChar;

	private EditorRow row_RadioGroupVertical;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEdit rep_SumType;

	private EditorRow row_SumType;

	private EditorRow row_DisFormat;

	private RepositoryItemCheckEdit rep_MultiSelect;

	private EditorRow row_Mitiselect;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx5;

	private SimpleButtonEx btn_OK;

	private EditorRow row_ParentFieldName;

	private RepositoryItemCheckEdit rep_IsParentField;

	public frm_TemplateWizardDesigner()
	{
		InitializeComponent();
	}

	public frm_TemplateWizardDesigner(DataSet dsFormTemplate, string containerType, int stackgroup)
	{
		InitializeComponent();
		DsFormTemplate = dsFormTemplate;
		_containerType = containerType;
		_stackGroup = stackgroup;
	}

	private void frm_TemplateWizardDesigner_Load(object sender, EventArgs e)
	{
		try
		{
			lbl_ToolTip.Text = "";
			rep_DataType.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Alpha" },
				{ "M", "Memo" },
				{ "N", "Numeric" },
				{ "D", "Date" },
				{ "B", "Float" }
			});
			rep_SubType.SetDataSource(new Dictionary<string, string>
			{
				{ "S", "Sum" },
				{ "P", "Price" },
				{ "Q", "Quantity" },
				{ "%", "Percentage" },
				{ "?", "Address" },
				{ "T", "Time" },
				{ "R", "Rate" },
				{ "M", "Measurement" },
				{ "B", "Link" },
				{ "I", "Image" },
				{ "", "None" }
			});
			rep_SumType.SetDataSource(new Dictionary<string, string>
			{
				{ "N", "None" },
				{ "S", "Sum" },
				{ "C", "Count" },
				{ "X", "Max" },
				{ "M", "Min" },
				{ "A", "Average" }
			}, hideCodeColumn: true);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (Enums.FormTemplateEditorType value in Enum.GetValues(typeof(Enums.FormTemplateEditorType)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)value;
				dataRow["Name"] = Function.SpaceBeforeCapital(Function.ToString(value));
				dataTable.Rows.Add(dataRow);
			}
			rep_Editor.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
			rep_ValidType.SetDataSource(new Dictionary<string, string>
			{
				{ "F", "Fixed List" },
				{ "T", "Query Text" }
			});
			DataTable dataTable2 = dataTable.Clone();
			foreach (Enums.TypeCFL value2 in Enum.GetValues(typeof(Enums.TypeCFL)))
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["Code"] = (int)value2;
				dataRow2["Name"] = Function.SpaceBeforeCapital(Function.ToString(value2));
				dataTable2.Rows.Add(dataRow2);
			}
			rep_CFLType.SetDataSource(dataTable2, "Name", "Code", emptyRow: true);
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Code", typeof(string));
			dataTable3.Columns.Add("Name", typeof(string));
			foreach (ApzObjectTypes value3 in Enum.GetValues(typeof(ApzObjectTypes)))
			{
				DataRow dataRow3 = dataTable3.NewRow();
				int num = (int)value3;
				dataRow3["Code"] = num.ToString();
				string text = Function.ToString(value3);
				dataRow3["Name"] = Function.SpaceBeforeCapital(text.Substring(1, text.Length - 1));
				dataTable3.Rows.Add(dataRow3);
			}
			rep_DrillDownType.SetDataSource(dataTable3, "Name", "Code", emptyRow: true);
			rep_DisplayType.SetDataSource(new Dictionary<string, string>
			{
				{ "1", "GridView" },
				{ "2", "TreeView" }
			});
			DataTable dataTable4 = DsFormTemplate.Tables["FWZ1"];
			DsFormTemplate.Tables["FWZ2"].Columns["LineId"].AutoIncrement = true;
			DsFormTemplate.Tables["FWZ2"].Columns["LineId"].AutoIncrementSeed = (DsFormTemplate.Tables["FWZ2"].IsNotNull() ? (DsFormTemplate.Tables["FWZ2"].AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineId"])) + 1) : 1);
			DsFormTemplate.Tables["FWZ2"].Columns["LineId"].AutoIncrementStep = 1L;
			EnumerableRowCollection<DataRow> source = from t in dataTable4.AsEnumerable()
				where Function.ToString(t["ContainerType"]) == _containerType && Function.ParseInt(t["StackGroup"]) == _stackGroup
				select t;
			DataTable dataTable5 = dataTable4.Clone();
			if (source.ToArray().Length != 0)
			{
				dataTable5 = source.CopyToDataTable();
			}
			dataTable5.Columns["ContainerId"].AutoIncrement = true;
			dataTable5.Columns["ContainerId"].AutoIncrementSeed = (dataTable4.IsNotNull() ? (dataTable4.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["ContainerId"])) + 1) : 1);
			dataTable5.Columns["ContainerId"].AutoIncrementStep = 1L;
			dataTable5.Columns["ShowCaption"].DefaultValue = "Y";
			dataTable5.Columns["AddCtxMenu"].DefaultValue = "Y";
			dataTable5.Columns["Type"].DefaultValue = 1;
			if (!dataTable5.IsNotNull())
			{
				DataRow dataRow4 = dataTable5.NewRow();
				dataRow4["ShowCaption"] = "Y";
				dataRow4["AddCtxMenu"] = "Y";
				dataTable5.Rows.Add(dataRow4);
			}
			grd_Container.DataSource = dataTable5;
			grd_Container.RefreshDataSource();
			if (_containerType == "F")
			{
				SimpleButton simpleButton = btn_AddContainer;
				SimpleButton simpleButton2 = btn_InsertContainer;
				SimpleButton simpleButton3 = btn_RemoveContainer;
				SimpleButton simpleButton4 = btn_ContainerUp;
				bool flag = (btn_ContainerDown.Enabled = false);
				bool flag3 = (simpleButton4.Enabled = flag);
				bool flag5 = (simpleButton3.Enabled = flag3);
				bool enabled = (simpleButton2.Enabled = flag5);
				simpleButton.Enabled = enabled;
				gv_Container.ColumnsEx["AddCtxMenu"].Visible = false;
				EditorRow editorRow = row_SumType;
				enabled = (row_DisFormat.Visible = false);
				editorRow.Visible = enabled;
			}
			else
			{
				gv_Container.ColumnsEx["AddCtxMenu"].Visible = true;
				EditorRow editorRow2 = row_SumType;
				bool enabled = (row_DisFormat.Visible = true);
				editorRow2.Visible = enabled;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Container_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int containerId = Function.ParseInt(gv_Container.GetFocusedRowCellValue("ContainerId"));
			row_ParentFieldName.Visible = Function.ParseInt(gv_Container.GetFocusedRowCellValue("Type")) != 1;
			InsertDataFields(grd_Fields.DataSourceTable.Copy());
			DataRow[] array = (from t in DsFormTemplate.Tables["FWZ2"].AsEnumerable()
				where Function.ParseInt(t["ContainerId"]) == containerId
				select t).ToArray();
			DataTable dataTable = DsFormTemplate.Tables["FWZ2"].Clone();
			if (array.Length != 0)
			{
				dataTable = array.CopyToDataTable();
				dataTable.Columns["LineId"].AutoIncrement = true;
				dataTable.Columns["LineId"].AutoIncrementSeed = (DsFormTemplate.Tables["FWZ2"].IsNotNull() ? (DsFormTemplate.Tables["FWZ2"].AsEnumerable().Max((DataRow t) => Function.ParseLong(t["LineId"])) + 1) : 1);
				dataTable.Columns["LineId"].AutoIncrementStep = 1L;
			}
			else
			{
				DataRow row = NewField(dataTable, containerId);
				dataTable.Rows.Add(row);
			}
			grd_Fields.DataSource = dataTable;
			grd_Fields.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataRow NewField(DataTable dt, int containerId)
	{
		try
		{
			DataRow dataRow = dt.NewRow();
			dataRow["ContainerId"] = containerId;
			dataRow["DataType"] = "A";
			dataRow["SubType"] = "";
			dataRow["IsLeft"] = "Y";
			if (dt.IsNotNull())
			{
				dataRow["VisIndex"] = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisIndex"])) + 1;
			}
			else
			{
				dataRow["VisIndex"] = 0;
			}
			dataRow["Editable"] = "Y";
			dataRow["Visible"] = "Y";
			dataRow["IsReadOnly"] = "N";
			return dataRow;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private void gv_Fields_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		FieldsFocusedRowObjectChanged();
	}

	private void FieldsFocusedRowObjectChanged()
	{
		try
		{
			InsertDataValidValues(grd_ValidValue.DataSourceTable.Copy());
			DataRow focusedDataRow = gv_Fields.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				vgrd_Properties.DataSource = new DataRow[1] { focusedDataRow }.CopyToDataTable();
				vgrd_Properties.RefreshDataSource();
				int lineId = Function.ParseInt(focusedDataRow["LineId"]);
				EnumerableRowCollection<DataRow> source = from t in DsFormTemplate.Tables["FWZ3"].AsEnumerable()
					where Function.ParseInt(t["LineId"]) == lineId
					select t;
				DataTable dataTable = (source.Any() ? source.CopyToDataTable() : DsFormTemplate.Tables["FWZ3"].Clone());
				DataRow dataRow = dataTable.NewRow();
				dataRow["LineId"] = focusedDataRow["LineId"];
				dataTable.Rows.Add(dataRow);
				grd_ValidValue.DataSource = dataTable;
				grd_ValidValue.RefreshDataSource();
				string validType = Function.ToString(focusedDataRow["ValidType"]);
				ShowHideValidValues(validType);
				Enums.FormTemplateEditorType editor = (Enums.FormTemplateEditorType)Function.ParseInt(focusedDataRow["Editor"]);
				ShowHideEditor(editor);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void vgrd_Properties_RowChanged(object sender, RowChangedEventArgs e)
	{
	}

	private void vgrd_Properties_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
	{
		try
		{
			string fieldName = e.Row.Properties.FieldName;
			if (!string.IsNullOrEmpty(fieldName))
			{
				DataTable dataSourceTable = grd_Fields.DataSourceTable;
				DataRow focusedDataRow = gv_Fields.GetFocusedDataRow();
				switch (fieldName)
				{
				case "VisIndex":
					MoveRow(Function.ParseInt(focusedDataRow[fieldName]), Function.ParseInt(e.Value), gv_Fields);
					break;
				case "Editor":
				{
					Enums.FormTemplateEditorType editor = (Enums.FormTemplateEditorType)Function.ParseInt(e.Value);
					ShowHideEditor(editor);
					break;
				}
				case "ValidType":
				{
					string validType = Function.ToString(e.Value);
					ShowHideValidValues(validType);
					break;
				}
				}
				focusedDataRow[fieldName] = e.Value;
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowHideValidValues(string validType)
	{
		if (validType == "F")
		{
			splitContainerControl_Properties.PanelVisibility = SplitPanelVisibility.Both;
			gv_ValidValue.ViewCaption = "Valid Values";
			gcCode.Caption = "Code";
			gcName.Caption = "Name";
		}
		else
		{
			splitContainerControl_Properties.PanelVisibility = SplitPanelVisibility.Panel1;
		}
	}

	private void ShowHideEditor(Enums.FormTemplateEditorType editor)
	{
		if (editor == Enums.FormTemplateEditorType.RadioGroup)
		{
			row_RadioGroupVertical.Visible = true;
			splitContainerControl_Properties.PanelVisibility = SplitPanelVisibility.Both;
			gv_ValidValue.ViewCaption = "List Radio Button";
			gcCode.Caption = "Value";
			gcName.Caption = "Description";
		}
		else
		{
			row_RadioGroupVertical.Visible = false;
			splitContainerControl_Properties.PanelVisibility = SplitPanelVisibility.Panel1;
		}
		row_CFLQuery.Visible = editor == Enums.FormTemplateEditorType.ChooseFromList;
		row_CFLType.Visible = editor == Enums.FormTemplateEditorType.ChooseFromList;
		category_Cfl.Visible = editor == Enums.FormTemplateEditorType.ChooseFromList;
		row_ValidQuery.Visible = editor == Enums.FormTemplateEditorType.ValidValue;
		row_ValidType.Visible = editor == Enums.FormTemplateEditorType.ValidValue;
		category_Valid.Visible = editor == Enums.FormTemplateEditorType.ValidValue;
		EditorRow editorRow = row_DisplayMember;
		bool visible = (row_ValueMember.Visible = editor == Enums.FormTemplateEditorType.ChooseFromList || editor == Enums.FormTemplateEditorType.ValidValue);
		editorRow.Visible = visible;
	}

	private void gv_Fields_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			_downHitInfo = null;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
				if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
				{
					_downHitInfo = gridHitInfo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Fields_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Button == MouseButtons.Left && _downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(_downHitInfo.HitPoint.X - dragSize.Width / 2, _downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridViewEx?.GridControl.DoDragDrop(_downHitInfo, DragDropEffects.All);
					_downHitInfo = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Fields_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
			{
				GridHitInfo gridHitInfo2 = mainView.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Fields_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2, gridViewEx);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow, GridViewEx gv)
	{
		try
		{
			if (sourceRow != targetRow && gv.GridControl is GridControlEx { DataSourceTable: var dataSourceTable } gridControlEx)
			{
				DataRow dataRow = dataSourceTable.Rows[sourceRow];
				DataRow dataRow2 = dataSourceTable.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				dataSourceTable.Rows.Remove(dataRow);
				dataSourceTable.Rows.InsertAt(dataRow2, targetRow);
				dataSourceTable.AcceptChanges();
				gv.FocusedRowHandle = targetRow;
				gv.MakeRowVisible(targetRow);
				RefreshVisIndex(gridControlEx);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_FieldUp_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Fields.FocusedRowHandle;
			MoveRow(focusedRowHandle, focusedRowHandle - 1, gv_Fields);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_FieldDown_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Fields.FocusedRowHandle;
			MoveRow(focusedRowHandle, focusedRowHandle + 1, gv_Fields);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AddField_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Fields.DataSourceTable;
			DataRow row = NewField(dataSourceTable, Function.ParseInt(gv_Container.GetFocusedRowCellValue("ContainerId")));
			dataSourceTable.Rows.Add(row);
			dataSourceTable.AcceptChanges();
			gv_Fields.FocusedRowHandle = gv_Fields.FindRowHandleByDataRow(row);
			gv_Fields.MakeRowVisible(gv_Fields.FocusedRowHandle);
			RefreshVisIndex(grd_Fields);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_InsertField_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Fields.DataSourceTable;
			DataRow row = NewField(dataSourceTable, Function.ParseInt(gv_Container.GetFocusedRowCellValue("ContainerId")));
			dataSourceTable.AcceptChanges();
			dataSourceTable.Rows.InsertAt(row, gv_Fields.FocusedRowHandle);
			gv_Fields.FocusedRowHandle = gv_Fields.FindRowHandleByDataRow(row);
			gv_Fields.MakeRowVisible(gv_Fields.FocusedRowHandle);
			RefreshVisIndex(grd_Fields);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Fields.DeleteSelectedRows();
			grd_Fields.DataSourceTable.AcceptChanges();
			FieldsFocusedRowObjectChanged();
			RefreshVisIndex(grd_Fields);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshVisIndex(GridControlEx grd)
	{
		try
		{
			DataTable dataSourceTable = grd.DataSourceTable;
			if (dataSourceTable.ExistsColumn("VisIndex") && dataSourceTable.IsNotNull())
			{
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					dataSourceTable.Rows[i]["VisIndex"] = i;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AddContainer_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Container.DataSourceTable;
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.Rows.Add(row);
			dataSourceTable.AcceptChanges();
			gv_Container.FocusedRowHandle = gv_Container.FindRowHandleByDataRow(row);
			gv_Container.MakeRowVisible(gv_Container.FocusedRowHandle);
			RefreshVisIndex(grd_Container);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_InsertContainer_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Container.DataSourceTable;
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.AcceptChanges();
			dataSourceTable.Rows.InsertAt(row, gv_Container.FocusedRowHandle);
			gv_Container.FocusedRowHandle = gv_Container.FindRowHandleByDataRow(row);
			gv_Container.MakeRowVisible(gv_Container.FocusedRowHandle);
			RefreshVisIndex(grd_Container);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_RemoveContainer_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Container.DeleteSelectedRows();
			grd_Container.DataSourceTable.AcceptChanges();
			FieldsFocusedRowObjectChanged();
			RefreshVisIndex(grd_Container);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			InsertDataValidValues(grd_ValidValue.DataSourceTable.Copy());
			InsertDataContainer(grd_Container.DataSourceTable.Copy());
			InsertDataFields(grd_Fields.DataSourceTable.Copy());
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InsertDataContainer(DataTable dt)
	{
		if (DsFormTemplate.Tables["FWZ1"] == null || DsFormTemplate.Tables["FWZ1"].Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CannotGetDataContainer, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = DsFormTemplate.Tables["FWZ1"].Select("ContainerType='" + _containerType + "' AND StackGroup='" + _stackGroup + "'");
			for (int i = 0; i < array.Length; i++)
			{
				DsFormTemplate.Tables["FWZ1"].Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["ContainerId"])))
				{
					DataRow dataRow = DsFormTemplate.Tables["FWZ1"].NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					dataRow["ContainerType"] = _containerType;
					dataRow["StackGroup"] = _stackGroup;
					DsFormTemplate.Tables["FWZ1"].Rows.Add(dataRow);
				}
			}
			DsFormTemplate.Tables["FWZ1"].AcceptChanges();
		}
	}

	private void InsertDataValidValues(DataTable dt)
	{
		if (DsFormTemplate.Tables["FWZ3"] == null || DsFormTemplate.Tables["FWZ3"].Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CannotGetData, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			int num = Function.ParseInt(dt.Rows[0]["LineId"]);
			DataRow[] array = DsFormTemplate.Tables["FWZ3"].Select("LineId='" + num + "'");
			for (int i = 0; i < array.Length; i++)
			{
				DsFormTemplate.Tables["FWZ3"].Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["Code"])) || !string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["Name"])))
				{
					DataRow dataRow = DsFormTemplate.Tables["FWZ3"].NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					dataRow["VisOrder"] = j;
					DsFormTemplate.Tables["FWZ3"].Rows.Add(dataRow);
				}
			}
			DsFormTemplate.Tables["FWZ3"].AcceptChanges();
		}
	}

	private void InsertDataFields(DataTable dt)
	{
		if (DsFormTemplate.Tables["FWZ2"] == null || DsFormTemplate.Tables["FWZ2"].Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CannotGetData, "");
		}
		else
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return;
			}
			int num = Function.ParseInt(dt.Rows[0]["ContainerId"]);
			DataRow[] array = DsFormTemplate.Tables["FWZ2"].Select("ContainerId='" + num + "'");
			for (int i = 0; i < array.Length; i++)
			{
				DsFormTemplate.Tables["FWZ2"].Rows.Remove(array[i]);
			}
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[j]["FieldName"])))
				{
					DataRow dataRow = DsFormTemplate.Tables["FWZ2"].NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					dataRow["ContainerId"] = num;
					DsFormTemplate.Tables["FWZ2"].Rows.Add(dataRow);
				}
			}
			DsFormTemplate.Tables["FWZ2"].AcceptChanges();
		}
	}

	private void vgrd_Properties_RecordCellStyle(object sender, GetCustomRowCellStyleEventArgs e)
	{
		try
		{
			switch (e.Row.Properties.FieldName)
			{
			case "CFLQuery":
				if (Function.ParseInt(vgrd_Properties.GetCellValue(row_CFLType, 0)) != 9999)
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				break;
			case "ValidQuery":
			{
				string text = Function.ToString(vgrd_Properties.GetCellValue(row_ValidType, 0));
				if (text != "Q" && text != "T")
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				break;
			}
			case "ValueMember":
			case "DisplayMember":
			{
				Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(vgrd_Properties.GetCellValue(row_Editor, 0));
				if (formTemplateEditorType != Enums.FormTemplateEditorType.ChooseFromList && formTemplateEditorType != Enums.FormTemplateEditorType.ValidValue)
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void vgrd_Properties_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			BaseRow focusedRow = vgrd_Properties.FocusedRow;
			if (focusedRow == null)
			{
				return;
			}
			switch (focusedRow.Properties.FieldName)
			{
			case "CFLQuery":
				if (Function.ParseInt(vgrd_Properties.GetCellValue(row_CFLType, 0)) != 9999)
				{
					e.Cancel = true;
				}
				break;
			case "ValidQuery":
			{
				string text = Function.ToString(vgrd_Properties.GetCellValue(row_ValidType, 0));
				if (text != "Q" && text != "T")
				{
					e.Cancel = true;
				}
				break;
			}
			case "ValueMember":
			case "DisplayMember":
			{
				Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(vgrd_Properties.GetCellValue(row_Editor, 0));
				if (formTemplateEditorType != Enums.FormTemplateEditorType.ChooseFromList && formTemplateEditorType != Enums.FormTemplateEditorType.ValidValue)
				{
					e.Cancel = true;
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void vgrd_Properties_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e)
	{
		try
		{
			if (e.Row != null)
			{
				switch (e.Row.Properties.FieldName)
				{
				case "FieldName":
					lbl_ToolTip.Text = "Do not use white space and special characters";
					break;
				case "CFLType":
					lbl_ToolTip.Text = "You can select \"User Defined Data\", then you can define \"Choose From List\" by entering query";
					break;
				case "ValidType":
					lbl_ToolTip.Text = "You can select \"Query Text\", then you can define \"Valid Values\" by entering query, select \"Fixed List\", then you can define by collection";
					break;
				default:
					lbl_ToolTip.Text = "";
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Fields_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Fields.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			int dataSourceRowIndex = gv_Fields.GetDataSourceRowIndex(e.RowHandle);
			if (dataSourceTable.Rows.Count > dataSourceRowIndex && dataSourceRowIndex >= 0)
			{
				if (!(Function.ToString(dataSourceTable.Rows[dataSourceRowIndex]["Visible"]) == "Y"))
				{
					e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
				}
				bool flag = Function.ToString(dataSourceTable.Rows[dataSourceRowIndex]["IsReadOnly"]) == "Y";
				bool flag2 = Function.ToString(dataSourceTable.Rows[dataSourceRowIndex]["Editable"]) == "Y";
				if (flag || !flag2)
				{
					e.Appearance.BackColor = GridColor.GridReadOnly;
				}
				if (Function.ToString(dataSourceTable.Rows[dataSourceRowIndex]["PrimaryKey"]) == "Y")
				{
					e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ValidValue_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_ValidValue.DataSourceTable;
			if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Code"])) && string.IsNullOrEmpty(Function.ToString(t["Name"]))) == null)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["LineId"] = grd_Fields.DataSourceTable.Rows[gv_Fields.GetDataSourceRowIndex(gv_Fields.FocusedRowHandle)]["LineId"];
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Fields_Click(object sender, EventArgs e)
	{
	}

	private void gv_Container_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Type")
			{
				if (Function.ParseInt(e.Value) == 2)
				{
					row_ParentFieldName.Visible = true;
				}
				else
				{
					row_ParentFieldName.Visible = false;
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.FormTemplate.frm_TemplateWizardDesigner));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_ContainerDown = new DevExpress.XtraEditors.SimpleButton();
		this.btn_ContainerUp = new DevExpress.XtraEditors.SimpleButton();
		this.btn_RemoveContainer = new DevExpress.XtraEditors.SimpleButton();
		this.btn_InsertContainer = new DevExpress.XtraEditors.SimpleButton();
		this.btn_AddContainer = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Container = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Container = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ShowCaption = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DisplayType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_FieldDown = new DevExpress.XtraEditors.SimpleButton();
		this.btn_FieldUp = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Fields = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Fields = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Remove = new DevExpress.XtraEditors.SimpleButton();
		this.btn_InsertField = new DevExpress.XtraEditors.SimpleButton();
		this.btn_AddField = new DevExpress.XtraEditors.SimpleButton();
		this.splitContainerControl_Properties = new DevExpress.XtraEditors.SplitContainerControl();
		this.vgrd_Properties = new DevExpress.XtraVerticalGrid.VGridControl();
		this.rep_DataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_SubType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_IsLeft = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_Editor = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_CFLType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_ValidType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_KeyNewRow = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_CFLQuery = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.rep_DrillDownType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_SumType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_MultiSelect = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.category_General = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_Name = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Caption = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DataType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_SubType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_KeyNewRow = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_PrimaryKey = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DrillDownType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_ParentFieldName = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category_Editor = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_Editor = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_RadioGroupVertical = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_PasswordChar = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_SumType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DisFormat = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category_Cfl = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_CFLType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_CFLQuery = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Mitiselect = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category_Valid = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_ValidType = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_ValidQuery = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category_Member = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_ValueMember = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DisplayMember = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Editable = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_IsReadOnly = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Mandatory = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.category_appearance = new DevExpress.XtraVerticalGrid.Rows.CategoryRow();
		this.row_IsLeft = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_Visible = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_VisibleIndex = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row_DefaultValue = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.grd_ValidValue = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ValidValue = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.lbl_ToolTip = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.rep_IsParentField = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Container).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Container).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ShowCaption).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DisplayType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Fields).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Fields).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl_Properties).BeginInit();
		this.splitContainerControl_Properties.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.vgrd_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SubType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsLeft).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Editor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFLType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_KeyNewRow).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFLQuery).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MultiSelect).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ValidValue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ValidValue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsParentField).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 3);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_ContainerDown);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_ContainerUp);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_RemoveContainer);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_InsertContainer);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_AddContainer);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Container);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1035, 401);
		this.splitContainerControl1.SplitterPosition = 410;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_ContainerDown.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_ContainerDown.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ContainerDown.ImageOptions.Image");
		this.btn_ContainerDown.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_ContainerDown.Location = new System.Drawing.Point(162, 4);
		this.btn_ContainerDown.Name = "btn_ContainerDown";
		this.btn_ContainerDown.Size = new System.Drawing.Size(30, 23);
		this.btn_ContainerDown.TabIndex = 7;
		this.btn_ContainerDown.ToolTip = "Move toward End";
		this.btn_ContainerUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_ContainerUp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ContainerUp.ImageOptions.Image");
		this.btn_ContainerUp.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_ContainerUp.Location = new System.Drawing.Point(126, 4);
		this.btn_ContainerUp.Name = "btn_ContainerUp";
		this.btn_ContainerUp.Size = new System.Drawing.Size(30, 23);
		this.btn_ContainerUp.TabIndex = 6;
		this.btn_ContainerUp.ToolTip = "Move toward Beginning";
		this.btn_RemoveContainer.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_RemoveContainer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_RemoveContainer.ImageOptions.Image");
		this.btn_RemoveContainer.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_RemoveContainer.Location = new System.Drawing.Point(75, 3);
		this.btn_RemoveContainer.Name = "btn_RemoveContainer";
		this.btn_RemoveContainer.Size = new System.Drawing.Size(30, 23);
		this.btn_RemoveContainer.TabIndex = 5;
		this.btn_RemoveContainer.ToolTip = "Remove Container";
		this.btn_RemoveContainer.Click += new System.EventHandler(this.btn_RemoveContainer_Click);
		this.btn_InsertContainer.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_InsertContainer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_InsertContainer.ImageOptions.Image");
		this.btn_InsertContainer.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_InsertContainer.Location = new System.Drawing.Point(39, 4);
		this.btn_InsertContainer.Name = "btn_InsertContainer";
		this.btn_InsertContainer.Size = new System.Drawing.Size(30, 23);
		this.btn_InsertContainer.TabIndex = 4;
		this.btn_InsertContainer.ToolTip = "Insert Container";
		this.btn_InsertContainer.Click += new System.EventHandler(this.btn_InsertContainer_Click);
		this.btn_AddContainer.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_AddContainer.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddContainer.ImageOptions.Image");
		this.btn_AddContainer.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_AddContainer.Location = new System.Drawing.Point(3, 4);
		this.btn_AddContainer.Name = "btn_AddContainer";
		this.btn_AddContainer.Size = new System.Drawing.Size(30, 23);
		this.btn_AddContainer.TabIndex = 3;
		this.btn_AddContainer.ToolTip = "Add Container";
		this.btn_AddContainer.Click += new System.EventHandler(this.btn_AddContainer_Click);
		this.grd_Container.AllowDrop = true;
		this.grd_Container.AllowPasteExcel = true;
		this.grd_Container.ClipboardData = null;
		this.grd_Container.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Container.Location = new System.Drawing.Point(0, 0);
		this.grd_Container.MainView = this.gv_Container;
		this.grd_Container.Name = "grd_Container";
		this.grd_Container.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ShowCaption, this.rep_DisplayType });
		this.grd_Container.Size = new System.Drawing.Size(410, 401);
		this.grd_Container.TabIndex = 0;
		this.grd_Container.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Container });
		this.grd_Container.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_Fields_DragDrop);
		this.grd_Container.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_Fields_DragOver);
		this.gv_Container.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Container.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Container.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Container.ColumnKey = null;
		this.gv_Container.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx1, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx7, this.gridColumnEx6, this.gridColumnEx5 });
		this.gv_Container.DisableFormSetting = false;
		this.gv_Container.DisplayException = false;
		this.gv_Container.EditColumn = null;
		this.gv_Container.FieldCurrency = null;
		this.gv_Container.FieldRate = null;
		this.gv_Container.FormId = "oTemplateWizardDesign";
		this.gv_Container.GridControl = this.grd_Container;
		this.gv_Container.HasFormSetting = true;
		this.gv_Container.IgnoreFilter = false;
		this.gv_Container.IndexEditRow = -1;
		this.gv_Container.IndicatorWidth = 35;
		this.gv_Container.IsApzTable = false;
		this.gv_Container.IsInsertNewRow = false;
		this.gv_Container.IsLoadingData = false;
		this.gv_Container.IsMappingContextAction = false;
		this.gv_Container.ItemId = "OCONTAINER";
		this.gv_Container.LstColumnAllowBatch = null;
		this.gv_Container.LstColumnNotAllowBatch = null;
		this.gv_Container.LstColumnNotEditable = null;
		this.gv_Container.LstColumNotSetting = null;
		this.gv_Container.LstOldUDF = null;
		this.gv_Container.Name = "gv_Container";
		this.gv_Container.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Container.OptionsPrint.AutoWidth = false;
		this.gv_Container.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Container.OptionsView.ColumnAutoWidth = false;
		this.gv_Container.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Container.OptionsView.ShowGroupPanel = false;
		this.gv_Container.OptionsView.ShowViewCaption = true;
		this.gv_Container.ReadOnly = false;
		this.gv_Container.SettingReadonly = false;
		this.gv_Container.UseBatchMod = false;
		this.gv_Container.ViewCaption = "Container";
		this.gv_Container.ViewCaptionHeight = 30;
		this.gv_Container.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Container_FocusedRowObjectChanged);
		this.gv_Container.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Container_CellValueChanged);
		this.gv_Container.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Fields_MouseDown);
		this.gv_Container.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Fields_MouseMove);
		this.gridColumnEx1.Caption = "Caption";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Caption";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 120;
		this.gridColumnEx3.Caption = "ItemId";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ItemId";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Show Caption";
		this.gridColumnEx4.ColumnEdit = this.rep_ShowCaption;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ShowCaption";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 86;
		this.rep_ShowCaption.AutoHeight = false;
		this.rep_ShowCaption.Name = "rep_ShowCaption";
		this.rep_ShowCaption.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_ShowCaption.ValueChecked = "Y";
		this.rep_ShowCaption.ValueGrayed = "N";
		this.rep_ShowCaption.ValueUnchecked = "N";
		this.gridColumnEx7.Caption = "Add Context Menu";
		this.gridColumnEx7.ColumnEdit = this.rep_ShowCaption;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "AddCtxMenu";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 3;
		this.gridColumnEx7.Width = 100;
		this.gridColumnEx6.Caption = "Show Checkbox Select";
		this.gridColumnEx6.ColumnEdit = this.rep_ShowCaption;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "ShowCheckCol";
		this.gridColumnEx6.MinWidth = 21;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 120;
		this.gridColumnEx5.Caption = "Display Type";
		this.gridColumnEx5.ColumnEdit = this.rep_DisplayType;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Type";
		this.gridColumnEx5.MinWidth = 21;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 120;
		this.rep_DisplayType.Name = "rep_DisplayType";
		this.rep_DisplayType.NullText = "";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_FieldDown);
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_FieldUp);
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Fields);
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_Remove);
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_InsertField);
		this.splitContainerControl2.Panel1.Controls.Add(this.btn_AddField);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.splitContainerControl_Properties);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(615, 401);
		this.splitContainerControl2.SplitterPosition = 259;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.btn_FieldDown.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_FieldDown.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_FieldDown.ImageOptions.Image");
		this.btn_FieldDown.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_FieldDown.Location = new System.Drawing.Point(168, 4);
		this.btn_FieldDown.Name = "btn_FieldDown";
		this.btn_FieldDown.Size = new System.Drawing.Size(30, 23);
		this.btn_FieldDown.TabIndex = 9;
		this.btn_FieldDown.ToolTip = "Move toward End";
		this.btn_FieldDown.Click += new System.EventHandler(this.btn_FieldDown_Click);
		this.btn_FieldUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_FieldUp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_FieldUp.ImageOptions.Image");
		this.btn_FieldUp.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_FieldUp.Location = new System.Drawing.Point(132, 4);
		this.btn_FieldUp.Name = "btn_FieldUp";
		this.btn_FieldUp.Size = new System.Drawing.Size(30, 23);
		this.btn_FieldUp.TabIndex = 8;
		this.btn_FieldUp.ToolTip = "Move toward Beginning";
		this.btn_FieldUp.Click += new System.EventHandler(this.btn_FieldUp_Click);
		this.grd_Fields.AllowDrop = true;
		this.grd_Fields.AllowPasteExcel = true;
		this.grd_Fields.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Fields.ClipboardData = null;
		this.grd_Fields.Location = new System.Drawing.Point(4, 33);
		this.grd_Fields.MainView = this.gv_Fields;
		this.grd_Fields.Name = "grd_Fields";
		this.grd_Fields.Size = new System.Drawing.Size(251, 367);
		this.grd_Fields.TabIndex = 3;
		this.grd_Fields.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Fields });
		this.grd_Fields.Click += new System.EventHandler(this.grd_Fields_Click);
		this.grd_Fields.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_Fields_DragDrop);
		this.grd_Fields.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_Fields_DragOver);
		this.gv_Fields.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Fields.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Fields.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Fields.ColumnKey = null;
		this.gv_Fields.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx2 });
		this.gv_Fields.DisableFormSetting = false;
		this.gv_Fields.DisplayException = false;
		this.gv_Fields.EditColumn = null;
		this.gv_Fields.FieldCurrency = null;
		this.gv_Fields.FieldRate = null;
		this.gv_Fields.FormId = "oTemplateWizardDesign";
		this.gv_Fields.GridControl = this.grd_Fields;
		this.gv_Fields.HasFormSetting = true;
		this.gv_Fields.IgnoreFilter = false;
		this.gv_Fields.IndexEditRow = -1;
		this.gv_Fields.IndicatorWidth = 35;
		this.gv_Fields.IsApzTable = false;
		this.gv_Fields.IsInsertNewRow = false;
		this.gv_Fields.IsLoadingData = false;
		this.gv_Fields.IsMappingContextAction = false;
		this.gv_Fields.ItemId = "OFIELD";
		this.gv_Fields.LstColumnAllowBatch = null;
		this.gv_Fields.LstColumnNotAllowBatch = null;
		this.gv_Fields.LstColumnNotEditable = null;
		this.gv_Fields.LstColumNotSetting = null;
		this.gv_Fields.LstOldUDF = null;
		this.gv_Fields.Name = "gv_Fields";
		this.gv_Fields.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Fields.OptionsCustomization.AllowSort = false;
		this.gv_Fields.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Fields.OptionsView.ColumnAutoWidth = false;
		this.gv_Fields.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Fields.OptionsView.ShowColumnHeaders = false;
		this.gv_Fields.OptionsView.ShowGroupPanel = false;
		this.gv_Fields.OptionsView.ShowViewCaption = true;
		this.gv_Fields.ReadOnly = false;
		this.gv_Fields.SettingReadonly = false;
		this.gv_Fields.UseBatchMod = false;
		this.gv_Fields.ViewCaption = "Fields";
		this.gv_Fields.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Fields_RowCellStyle);
		this.gv_Fields.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Fields_FocusedRowObjectChanged);
		this.gv_Fields.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Fields_MouseDown);
		this.gv_Fields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Fields_MouseMove);
		this.gridColumnEx2.Caption = "Name (Caption)";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "gridColumnEx2";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.AllowEdit = false;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnboundExpression = "ISNULL(FieldName, '')+'('+ISNULL(Caption, '')+')'";
		this.gridColumnEx2.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.Width = 213;
		this.btn_Remove.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Remove.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Remove.ImageOptions.Image");
		this.btn_Remove.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_Remove.Location = new System.Drawing.Point(76, 3);
		this.btn_Remove.Name = "btn_Remove";
		this.btn_Remove.Size = new System.Drawing.Size(30, 23);
		this.btn_Remove.TabIndex = 2;
		this.btn_Remove.ToolTip = "Remove Field";
		this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
		this.btn_InsertField.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_InsertField.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_InsertField.ImageOptions.Image");
		this.btn_InsertField.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_InsertField.Location = new System.Drawing.Point(40, 4);
		this.btn_InsertField.Name = "btn_InsertField";
		this.btn_InsertField.Size = new System.Drawing.Size(30, 23);
		this.btn_InsertField.TabIndex = 1;
		this.btn_InsertField.ToolTip = "Insert Field";
		this.btn_InsertField.Click += new System.EventHandler(this.btn_InsertField_Click);
		this.btn_AddField.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_AddField.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddField.ImageOptions.Image");
		this.btn_AddField.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_AddField.Location = new System.Drawing.Point(4, 4);
		this.btn_AddField.Name = "btn_AddField";
		this.btn_AddField.Size = new System.Drawing.Size(30, 23);
		this.btn_AddField.TabIndex = 0;
		this.btn_AddField.ToolTip = "Add Field";
		this.btn_AddField.Click += new System.EventHandler(this.btn_AddField_Click);
		this.splitContainerControl_Properties.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl_Properties.Horizontal = false;
		this.splitContainerControl_Properties.Location = new System.Drawing.Point(10, 33);
		this.splitContainerControl_Properties.Name = "splitContainerControl_Properties";
		this.splitContainerControl_Properties.Panel1.Controls.Add(this.vgrd_Properties);
		this.splitContainerControl_Properties.Panel1.Text = "Panel1";
		this.splitContainerControl_Properties.Panel2.Controls.Add(this.grd_ValidValue);
		this.splitContainerControl_Properties.Panel2.Text = "Panel2";
		this.splitContainerControl_Properties.Size = new System.Drawing.Size(336, 367);
		this.splitContainerControl_Properties.SplitterPosition = 475;
		this.splitContainerControl_Properties.TabIndex = 1;
		this.splitContainerControl_Properties.Text = "splitContainerControl3";
		this.vgrd_Properties.Appearance.Category.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.vgrd_Properties.Appearance.Category.ForeColor = System.Drawing.Color.FromArgb(128, 64, 64);
		this.vgrd_Properties.Appearance.Category.Options.UseFont = true;
		this.vgrd_Properties.Appearance.Category.Options.UseForeColor = true;
		this.vgrd_Properties.Cursor = System.Windows.Forms.Cursors.Default;
		this.vgrd_Properties.Dock = System.Windows.Forms.DockStyle.Fill;
		this.vgrd_Properties.Location = new System.Drawing.Point(0, 0);
		this.vgrd_Properties.Name = "vgrd_Properties";
		this.vgrd_Properties.RecordWidth = 281;
		this.vgrd_Properties.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.rep_DataType, this.rep_SubType, this.rep_IsLeft, this.rep_Editor, this.rep_CFLType, this.rep_ValidType, this.rep_KeyNewRow, this.rep_CFLQuery, this.rep_DrillDownType, this.rep_SumType,
			this.rep_MultiSelect, this.rep_IsParentField
		});
		this.vgrd_Properties.RowHeaderWidth = 159;
		this.vgrd_Properties.Rows.AddRange(new DevExpress.XtraVerticalGrid.Rows.BaseRow[33]
		{
			this.category_General, this.row_Name, this.row_Caption, this.row_DataType, this.row_SubType, this.row_KeyNewRow, this.row_PrimaryKey, this.row_DrillDownType, this.row_ParentFieldName, this.category_Editor,
			this.row_Editor, this.row_RadioGroupVertical, this.row_PasswordChar, this.row_SumType, this.row_DisFormat, this.category_Cfl, this.row_CFLType, this.row_CFLQuery, this.row_Mitiselect, this.category_Valid,
			this.row_ValidType, this.row_ValidQuery, this.category_Member, this.row_ValueMember, this.row_DisplayMember, this.row_Editable, this.row_IsReadOnly, this.row_Mandatory, this.category_appearance, this.row_IsLeft,
			this.row_Visible, this.row_VisibleIndex, this.row_DefaultValue
		});
		this.vgrd_Properties.Size = new System.Drawing.Size(336, 357);
		this.vgrd_Properties.TabIndex = 0;
		this.vgrd_Properties.RowChanged += new DevExpress.XtraVerticalGrid.Events.RowChangedEventHandler(this.vgrd_Properties_RowChanged);
		this.vgrd_Properties.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(this.vgrd_Properties_FocusedRowChanged);
		this.vgrd_Properties.RecordCellStyle += new DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventHandler(this.vgrd_Properties_RecordCellStyle);
		this.vgrd_Properties.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.vgrd_Properties_ShowingEditor);
		this.vgrd_Properties.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.vgrd_Properties_CellValueChanged);
		this.rep_DataType.AutoHeight = false;
		this.rep_DataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DataType.Name = "rep_DataType";
		this.rep_DataType.NullText = "";
		this.rep_SubType.AutoHeight = false;
		this.rep_SubType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SubType.Name = "rep_SubType";
		this.rep_SubType.NullText = "";
		this.rep_IsLeft.AutoHeight = false;
		this.rep_IsLeft.Name = "rep_IsLeft";
		this.rep_IsLeft.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_IsLeft.ValueChecked = "Y";
		this.rep_IsLeft.ValueGrayed = "N";
		this.rep_IsLeft.ValueUnchecked = "N";
		this.rep_Editor.AutoHeight = false;
		this.rep_Editor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Editor.Name = "rep_Editor";
		this.rep_Editor.NullText = "";
		this.rep_CFLType.AutoHeight = false;
		this.rep_CFLType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CFLType.Name = "rep_CFLType";
		this.rep_CFLType.NullText = "";
		this.rep_ValidType.AutoHeight = false;
		this.rep_ValidType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ValidType.Name = "rep_ValidType";
		this.rep_ValidType.NullText = "";
		this.rep_KeyNewRow.AutoHeight = false;
		this.rep_KeyNewRow.Name = "rep_KeyNewRow";
		this.rep_KeyNewRow.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_KeyNewRow.ValueChecked = "Y";
		this.rep_KeyNewRow.ValueGrayed = "N";
		this.rep_KeyNewRow.ValueUnchecked = "N";
		this.rep_CFLQuery.AutoHeight = false;
		this.rep_CFLQuery.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CFLQuery.Name = "rep_CFLQuery";
		this.rep_CFLQuery.ShowIcon = false;
		this.rep_DrillDownType.AutoHeight = false;
		this.rep_DrillDownType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DrillDownType.Name = "rep_DrillDownType";
		this.rep_DrillDownType.NullText = "";
		this.rep_SumType.AutoHeight = false;
		this.rep_SumType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SumType.Name = "rep_SumType";
		this.rep_SumType.NullText = "";
		this.rep_SumType.ShowHeader = false;
		this.rep_MultiSelect.AutoHeight = false;
		this.rep_MultiSelect.Name = "rep_MultiSelect";
		this.rep_MultiSelect.ValueChecked = "Y";
		this.rep_MultiSelect.ValueGrayed = "N";
		this.rep_MultiSelect.ValueUnchecked = "N";
		this.category_General.Height = 17;
		this.category_General.Name = "category_General";
		this.category_General.Properties.Caption = "General";
		this.row_Name.Height = 25;
		this.row_Name.Name = "row_Name";
		this.row_Name.Properties.Caption = "Name";
		this.row_Name.Properties.FieldName = "FieldName";
		this.row_Caption.Height = 25;
		this.row_Caption.Name = "row_Caption";
		this.row_Caption.Properties.Caption = "Caption";
		this.row_Caption.Properties.FieldName = "Caption";
		this.row_DataType.Height = 25;
		this.row_DataType.Name = "row_DataType";
		this.row_DataType.Properties.Caption = "Data Type";
		this.row_DataType.Properties.FieldName = "DataType";
		this.row_DataType.Properties.RowEdit = this.rep_DataType;
		this.row_SubType.Height = 25;
		this.row_SubType.Name = "row_SubType";
		this.row_SubType.Properties.Caption = "Sub. Type";
		this.row_SubType.Properties.FieldName = "SubType";
		this.row_SubType.Properties.RowEdit = this.rep_SubType;
		this.row_KeyNewRow.Height = 25;
		this.row_KeyNewRow.Name = "row_KeyNewRow";
		this.row_KeyNewRow.Properties.Caption = "Key New Row";
		this.row_KeyNewRow.Properties.FieldName = "KeyNewRow";
		this.row_KeyNewRow.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_KeyNewRow.Visible = false;
		this.row_PrimaryKey.Height = 25;
		this.row_PrimaryKey.Name = "row_PrimaryKey";
		this.row_PrimaryKey.Properties.Caption = "Primary Key";
		this.row_PrimaryKey.Properties.FieldName = "PrimaryKey";
		this.row_PrimaryKey.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_DrillDownType.Height = 25;
		this.row_DrillDownType.Name = "row_DrillDownType";
		this.row_DrillDownType.Properties.Caption = "Drill Down";
		this.row_DrillDownType.Properties.FieldName = "DrillDownType";
		this.row_DrillDownType.Properties.RowEdit = this.rep_DrillDownType;
		this.row_DrillDownType.Properties.ToolTip = "Drill Down Type";
		this.row_ParentFieldName.Height = 25;
		this.row_ParentFieldName.Name = "row_ParentFieldName";
		this.row_ParentFieldName.Properties.Caption = "Is ParentField";
		this.row_ParentFieldName.Properties.FieldName = "IsParentFieldName";
		this.row_ParentFieldName.Properties.RowEdit = this.rep_IsParentField;
		this.row_ParentFieldName.Visible = false;
		this.category_Editor.Height = 18;
		this.category_Editor.Name = "category_Editor";
		this.category_Editor.Properties.Caption = "Option Editor";
		this.row_Editor.Height = 25;
		this.row_Editor.Name = "row_Editor";
		this.row_Editor.Properties.Caption = "Editor";
		this.row_Editor.Properties.FieldName = "Editor";
		this.row_Editor.Properties.RowEdit = this.rep_Editor;
		this.row_RadioGroupVertical.Height = 25;
		this.row_RadioGroupVertical.Name = "row_RadioGroupVertical";
		this.row_RadioGroupVertical.Properties.Caption = "Vertical Radio Group";
		this.row_RadioGroupVertical.Properties.FieldName = "RGVertical";
		this.row_RadioGroupVertical.Properties.RowEdit = this.rep_IsLeft;
		this.row_PasswordChar.Height = 25;
		this.row_PasswordChar.Name = "row_PasswordChar";
		this.row_PasswordChar.Properties.Caption = "Password Char";
		this.row_PasswordChar.Properties.FieldName = "PasswordChar";
		this.row_SumType.Height = 25;
		this.row_SumType.Name = "row_SumType";
		this.row_SumType.Properties.Caption = "Summary Type";
		this.row_SumType.Properties.FieldName = "SumType";
		this.row_SumType.Properties.RowEdit = this.rep_SumType;
		this.row_DisFormat.Height = 25;
		this.row_DisFormat.Name = "row_DisFormat";
		this.row_DisFormat.Properties.Caption = "Display Format";
		this.row_DisFormat.Properties.FieldName = "DisplayFormat";
		this.category_Cfl.Name = "category_Cfl";
		this.category_Cfl.Properties.Caption = "Choose From List";
		this.row_CFLType.Height = 25;
		this.row_CFLType.Name = "row_CFLType";
		this.row_CFLType.Properties.Caption = "Choose From List Type";
		this.row_CFLType.Properties.FieldName = "CFLType";
		this.row_CFLType.Properties.RowEdit = this.rep_CFLType;
		this.row_CFLQuery.Height = 25;
		this.row_CFLQuery.Name = "row_CFLQuery";
		this.row_CFLQuery.Properties.Caption = "Choose From List Query";
		this.row_CFLQuery.Properties.FieldName = "CFLQuery";
		this.row_CFLQuery.Properties.RowEdit = this.rep_CFLQuery;
		this.row_Mitiselect.Height = 25;
		this.row_Mitiselect.Name = "row_Mitiselect";
		this.row_Mitiselect.Properties.Caption = "Choose from list Multiselect";
		this.row_Mitiselect.Properties.FieldName = "MultiSelect";
		this.row_Mitiselect.Properties.RowEdit = this.rep_MultiSelect;
		this.category_Valid.Name = "category_Valid";
		this.category_Valid.Properties.Caption = "Valid Values";
		this.row_ValidType.Height = 25;
		this.row_ValidType.Name = "row_ValidType";
		this.row_ValidType.Properties.Caption = "Valid value Type";
		this.row_ValidType.Properties.FieldName = "ValidType";
		this.row_ValidType.Properties.RowEdit = this.rep_ValidType;
		this.row_ValidQuery.Height = 25;
		this.row_ValidQuery.Name = "row_ValidQuery";
		this.row_ValidQuery.Properties.Caption = "Valid Value Query";
		this.row_ValidQuery.Properties.FieldName = "ValidQuery";
		this.row_ValidQuery.Properties.RowEdit = this.rep_CFLQuery;
		this.category_Member.Height = 25;
		this.category_Member.Name = "category_Member";
		this.category_Member.Properties.Caption = "Member";
		this.row_ValueMember.Height = 25;
		this.row_ValueMember.Name = "row_ValueMember";
		this.row_ValueMember.Properties.Caption = "Value Member";
		this.row_ValueMember.Properties.FieldName = "ValueMember";
		this.row_DisplayMember.Height = 25;
		this.row_DisplayMember.Name = "row_DisplayMember";
		this.row_DisplayMember.Properties.Caption = "Display Member";
		this.row_DisplayMember.Properties.FieldName = "DisplayMember";
		this.row_Editable.Height = 25;
		this.row_Editable.Name = "row_Editable";
		this.row_Editable.Properties.Caption = "Editable";
		this.row_Editable.Properties.FieldName = "Editable";
		this.row_Editable.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_IsReadOnly.Height = 25;
		this.row_IsReadOnly.Name = "row_IsReadOnly";
		this.row_IsReadOnly.Properties.Caption = "Is Read Only";
		this.row_IsReadOnly.Properties.FieldName = "IsReadOnly";
		this.row_IsReadOnly.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_Mandatory.Name = "row_Mandatory";
		this.row_Mandatory.Properties.FieldName = "Mandatory";
		this.row_Mandatory.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_Mandatory.Visible = false;
		this.category_appearance.Name = "category_appearance";
		this.category_appearance.Properties.Caption = "Appearance";
		this.row_IsLeft.Height = 25;
		this.row_IsLeft.Name = "row_IsLeft";
		this.row_IsLeft.Properties.Caption = "Is Left";
		this.row_IsLeft.Properties.FieldName = "IsLeft";
		this.row_IsLeft.Properties.RowEdit = this.rep_IsLeft;
		this.row_Visible.Height = 25;
		this.row_Visible.Name = "row_Visible";
		this.row_Visible.Properties.Caption = "Visible";
		this.row_Visible.Properties.FieldName = "Visible";
		this.row_Visible.Properties.RowEdit = this.rep_KeyNewRow;
		this.row_VisibleIndex.Height = 25;
		this.row_VisibleIndex.Name = "row_VisibleIndex";
		this.row_VisibleIndex.Properties.Caption = "Visible Index";
		this.row_VisibleIndex.Properties.FieldName = "VisIndex";
		this.row_DefaultValue.Height = 25;
		this.row_DefaultValue.Name = "row_DefaultValue";
		this.row_DefaultValue.Properties.Caption = "Default Value";
		this.row_DefaultValue.Properties.FieldName = "DefaultValue";
		this.grd_ValidValue.AllowPasteExcel = true;
		this.grd_ValidValue.ClipboardData = null;
		this.grd_ValidValue.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ValidValue.Location = new System.Drawing.Point(0, 0);
		this.grd_ValidValue.MainView = this.gv_ValidValue;
		this.grd_ValidValue.Name = "grd_ValidValue";
		this.grd_ValidValue.Size = new System.Drawing.Size(0, 0);
		this.grd_ValidValue.TabIndex = 0;
		this.grd_ValidValue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ValidValue });
		this.gv_ValidValue.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_ValidValue.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ValidValue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ValidValue.ColumnKey = null;
		this.gv_ValidValue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcCode, this.gcName });
		this.gv_ValidValue.DisableFormSetting = true;
		this.gv_ValidValue.DisplayException = false;
		this.gv_ValidValue.EditColumn = null;
		this.gv_ValidValue.FieldCurrency = null;
		this.gv_ValidValue.FieldRate = null;
		this.gv_ValidValue.FormId = "oTemplateWizardDesign";
		this.gv_ValidValue.GridControl = this.grd_ValidValue;
		this.gv_ValidValue.HasFormSetting = true;
		this.gv_ValidValue.IgnoreFilter = false;
		this.gv_ValidValue.IndexEditRow = -1;
		this.gv_ValidValue.IndicatorWidth = 35;
		this.gv_ValidValue.IsApzTable = false;
		this.gv_ValidValue.IsInsertNewRow = false;
		this.gv_ValidValue.IsLoadingData = false;
		this.gv_ValidValue.IsMappingContextAction = false;
		this.gv_ValidValue.ItemId = "OVALIDVALUE";
		this.gv_ValidValue.LstColumnAllowBatch = null;
		this.gv_ValidValue.LstColumnNotAllowBatch = null;
		this.gv_ValidValue.LstColumnNotEditable = null;
		this.gv_ValidValue.LstColumNotSetting = null;
		this.gv_ValidValue.LstOldUDF = null;
		this.gv_ValidValue.Name = "gv_ValidValue";
		this.gv_ValidValue.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ValidValue.OptionsPrint.AutoWidth = false;
		this.gv_ValidValue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ValidValue.OptionsView.ColumnAutoWidth = false;
		this.gv_ValidValue.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ValidValue.OptionsView.ShowGroupPanel = false;
		this.gv_ValidValue.OptionsView.ShowViewCaption = true;
		this.gv_ValidValue.ReadOnly = false;
		this.gv_ValidValue.SettingReadonly = false;
		this.gv_ValidValue.UseBatchMod = false;
		this.gv_ValidValue.ViewCaption = "Valid Values";
		this.gv_ValidValue.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ValidValue_CellValueChanged);
		this.gcCode.Caption = "Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
		this.gcCode.Width = 100;
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Width = 324;
		this.lbl_ToolTip.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lbl_ToolTip.Appearance.ForeColor = System.Drawing.Color.FromArgb(192, 64, 0);
		this.lbl_ToolTip.Appearance.Options.UseFont = true;
		this.lbl_ToolTip.Appearance.Options.UseForeColor = true;
		this.lbl_ToolTip.EditControl = null;
		this.lbl_ToolTip.EditValue = "Tooltip";
		this.lbl_ToolTip.FieldName = null;
		this.lbl_ToolTip.Location = new System.Drawing.Point(105, 665);
		this.lbl_ToolTip.Name = "lbl_ToolTip";
		this.lbl_ToolTip.ReadOnly = false;
		this.lbl_ToolTip.SettingInVisible = false;
		this.lbl_ToolTip.SettingReadonly = false;
		this.lbl_ToolTip.Size = new System.Drawing.Size(39, 13);
		this.lbl_ToolTip.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ToolTip.TabIndex = 2;
		this.lbl_ToolTip.Text = "Tooltip";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 410);
		this.btn_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(81, 24);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.rep_IsParentField.AutoHeight = false;
		this.rep_IsParentField.Name = "rep_IsParentField";
		this.rep_IsParentField.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_IsParentField.ValueChecked = "Y";
		this.rep_IsParentField.ValueGrayed = "N";
		this.rep_IsParentField.ValueUnchecked = "N";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1036, 438);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.lbl_ToolTip);
		base.Controls.Add(this.splitContainerControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oTemplateWizardDesign";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TemplateWizardDesigner";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Template Wizard Designer";
		base.Load += new System.EventHandler(this.frm_TemplateWizardDesigner_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Container).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Container).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ShowCaption).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DisplayType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Fields).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Fields).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl_Properties).EndInit();
		this.splitContainerControl_Properties.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.vgrd_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SubType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsLeft).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Editor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFLType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ValidType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_KeyNewRow).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFLQuery).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SumType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MultiSelect).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ValidValue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ValidValue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsParentField).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
