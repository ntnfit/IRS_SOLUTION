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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_BinLocMasterData : XtraFormEx
{
	public const string FormController = "BinLocMaster";

	private bool _isLoading;

	private bool _addConfirm;

	public int AbsEntry;

	private IContainer components;

	private GridControlEx grdBinLocMaster;

	private GridViewEx gvBinLocMaster;

	private GridColumnEx colWarehouseCode;

	private GridColumnEx colBinCode;

	private GridColumnEx colSL1Code;

	private GridColumnEx colSL2Code;

	private GridColumnEx colSL3Code;

	private GridControlEx grdBinProp;

	private GridViewEx gvBinProp;

	private LabelControl lblProp;

	private GridColumnEx colInactive;

	private GridColumnEx colReceiveBin;

	private GridColumnEx colNoAutoAllc;

	private GridColumnEx colDescr;

	private GridColumnEx colWeight;

	private GridColumnEx colItemQty;

	private GridColumnEx colNoItems;

	private GridColumnEx colNoBaSe;

	private GridColumnEx colAltSortCode;

	private GridColumnEx colBarCode;

	private GridColumnEx colMinQty;

	private GridColumnEx colMaxQty;

	private GridColumnEx colMaxWeight;

	private GridColumnEx colItmRtrict;

	private GridColumnEx colUoMRtrict;

	private GridColumnEx colBatchRes;

	private GridColumnEx colTranRest;

	private GridColumnEx colLastUpd;

	private GridColumnEx colReason;

	private RepositoryItemCheckEdit ceInactive;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private RepositoryItemButtonEdit rep_WhsCode;

	private RepositoryItemButtonEdit rep_SLCode;

	private GridColumnEx colSL4Code;

	private SplitContainerControl splitContainerControl1;

	private RepositoryItemLookUpEdit rep_ItmRstrict;

	private RepositoryItemLookUpEdit rep_UoMRtrict;

	private RepositoryItemLookUpEdit rep_BatchRtrict;

	private RepositoryItemLookUpEdit rep_TranRstrict;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit rep_Attribute;

	private GridColumnEx colAttr1;

	private GridColumnEx colAttr2;

	private GridColumnEx colAttr3;

	private GridColumnEx colAttr4;

	private GridColumnEx colAttr5;

	private GridColumnEx colAttr6;

	private GridColumnEx colAttr7;

	private GridColumnEx colAttr8;

	private GridColumnEx colAttr9;

	private GridColumnEx colAttr10;

	private GridColumnEx gc_ItemRestrictionChoose;

	private GridColumnEx gc_UoMRestrictionChoose;

	private GridColumnEx gc_SingleBatch;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			addNewToolStripMenuItem.Visible = false;
			deleteRowToolStripMenuItem.Visible = true;
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			addNewToolStripMenuItem.Visible = true;
			deleteRowToolStripMenuItem.Visible = false;
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			addNewToolStripMenuItem.Visible = true;
			deleteRowToolStripMenuItem.Visible = true;
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		default:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_BinLocMasterData()
	{
		InitializeComponent();
	}

	private void SetDataSourceLookupEdit()
	{
		var dataSource = new Dictionary<int, string>
		{
			{ 0, "None" },
			{ 1, "Specific Item" },
			{ 2, "Single Item Only" },
			{ 3, "Specific Item Group" },
			{ 4, "Single Item Group Only" }
		}.Select((KeyValuePair<int, string> r) => new
		{
			ID = r.Key,
			Name = r.Value
		});
		rep_ItmRstrict.DataSource = dataSource;
		var dataSource2 = new Dictionary<int, string>
		{
			{ 0, "None" },
			{ 1, "Specific UoM" },
			{ 2, "Single UoM Only" },
			{ 3, "Specific UoM Group" },
			{ 4, "Single UoM Group Only" }
		}.Select((KeyValuePair<int, string> r) => new
		{
			ID = r.Key,
			Name = r.Value
		});
		rep_UoMRtrict.DataSource = dataSource2;
		var dataSource3 = new Dictionary<string, string>
		{
			{ "N", "None" },
			{ "Y", "Single Batch" }
		}.Select((KeyValuePair<string, string> r) => new
		{
			Code = r.Key,
			Name = r.Value
		});
		rep_BatchRtrict.DataSource = dataSource3;
		var dataSource4 = new Dictionary<int, string>
		{
			{ 0, "None" },
			{ 1, "All Transactions" },
			{ 2, "Inbound Transactions" },
			{ 3, "Out Transactions" },
			{ 4, "All Except Inventory Transfer And Counting Transactions" }
		}.Select((KeyValuePair<int, string> r) => new
		{
			ID = r.Key,
			Name = r.Value
		});
		rep_TranRstrict.DataSource = dataSource4;
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster"))
			{
				DataTable dataSource = await hp.GetAsync("GetDataBinLocation", new
				{
					absEntry = AbsEntry
				});
				int focusedRowHandle = gvBinLocMaster.FocusedRowHandle;
				grdBinLocMaster.DataSource = dataSource;
				grdBinLocMaster.RefreshDataSource();
				if (gvBinLocMaster.FocusedRowHandle.Equals(focusedRowHandle))
				{
					await LoadProperties();
				}
			}
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task LoadProperties()
	{
		try
		{
			int absEntry = Function.ParseInt(gvBinLocMaster.GetFocusedRowCellValue("AbsEntry"));
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
			DataTable dataTable = await hp.GetAsync("GetBinLocProperties", new { absEntry });
			grdBinProp.DataSource = dataTable;
			grdBinProp.RefreshDataSource();
			if (!dataTable.IsNotNull())
			{
				colReason.OptionsColumn.ReadOnly = false;
				return;
			}
			colReason.OptionsColumn.ReadOnly = Function.ParseInt(dataTable.Rows[0]["RtrictType"]) == 0;
			if (Function.ParseInt(dataTable.Rows[0]["ItmRtrictT"]) >= 0)
			{
				ItemRestrictionsType type = ItemRestrictionsType.None;
				switch (Function.ParseInt(dataTable.Rows[0]["ItmRtrictT"]))
				{
				case 0:
					type = ItemRestrictionsType.None;
					break;
				case 1:
					type = ItemRestrictionsType.SpecificItem;
					break;
				case 2:
					type = ItemRestrictionsType.SingleItemOnly;
					break;
				case 3:
					type = ItemRestrictionsType.SpecificItemGroup;
					break;
				case 4:
					type = ItemRestrictionsType.SingleItemGroupOnly;
					break;
				}
				LoadItemRestrictionsColumn(type);
			}
			if (Function.ParseInt(dataTable.Rows[0]["UoMRtrict"]) >= 0)
			{
				UoMRestrictionsType type2 = UoMRestrictionsType.None;
				switch (Function.ParseInt(dataTable.Rows[0]["UoMRtrict"]))
				{
				case 0:
					type2 = UoMRestrictionsType.None;
					break;
				case 1:
					type2 = UoMRestrictionsType.SpecificUoM;
					break;
				case 2:
					type2 = UoMRestrictionsType.SingleUoMOnly;
					break;
				case 3:
					type2 = UoMRestrictionsType.SpecificUoMGroup;
					break;
				case 4:
					type2 = UoMRestrictionsType.SingleUoMGroupOnly;
					break;
				}
				LoadUoMRestrictionsColumn(type2);
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["SngBatch"])))
			{
				if (Function.ToString(dataTable.Rows[0]["SngBatch"]).Equals("Y"))
				{
					LoadSingleBatchColumn(BatchRestrictionsType.SingleBatch);
				}
				else
				{
					LoadSingleBatchColumn(BatchRestrictionsType.None);
				}
			}
			if (Function.ParseInt(dataTable.Rows[0]["RtrictType"]) >= 0)
			{
				if (Function.ParseInt(dataTable.Rows[0]["RtrictType"]) > 0)
				{
					colReason.OptionsColumn.ReadOnly = false;
					colReason.OptionsColumn.AllowEdit = true;
				}
				else
				{
					colReason.OptionsColumn.ReadOnly = true;
					colReason.OptionsColumn.AllowEdit = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadBinLocField()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> hp_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup"))
			{
				DataTable dataTable = await hp_1.GetAsync("GetAllWHSub");
				EnumerableRowCollection<int> source = from r in dataTable.AsEnumerable()
					select Function.ParseInt(r["FldNum"]);
				GridColumnEx[] array = new GridColumnEx[4] { colSL1Code, colSL2Code, colSL3Code, colSL4Code };
				for (int num = 0; num < 4; num++)
				{
					if (source.Contains(num + 1))
					{
						array[num].Visible = true;
						array[num].VisibleIndex = num + 1;
						array[num].Caption = Function.ToString(dataTable.Rows[num]["DispName"]);
					}
					else
					{
						array[num].Visible = false;
					}
				}
			}
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AttrCodeSetup");
			DataTable dataTable2 = await hp.GetAsync("GetAttribute");
			EnumerableRowCollection<int> source2 = from r in dataTable2.AsEnumerable()
				select Function.ParseInt(r["FldNum"]);
			GridColumnEx[] array2 = new GridColumnEx[10] { colAttr1, colAttr2, colAttr3, colAttr4, colAttr5, colAttr6, colAttr7, colAttr8, colAttr9, colAttr10 };
			if (!dataTable2.IsNotNull())
			{
				return;
			}
			for (int num2 = 0; num2 < 10; num2++)
			{
				if (source2.Contains(num2 + 1))
				{
					array2[num2].Visible = true;
					array2[num2].Caption = Function.ToString(dataTable2.Rows[num2]["DispName"]);
				}
				else
				{
					array2[num2].Visible = false;
				}
				if (num2 < dataTable2.Rows.Count)
				{
					array2[num2].Tag = Function.ParseInt(dataTable2.Rows[num2]["AbsEntry"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewRow(DataTable data = null)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				return;
			}
			if (data == null)
			{
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocMaster");
				data = await hp.GetAsync("GetTableStructure");
				DataRow dataRow = data.NewRow();
				object obj = (dataRow["UoMRtrict"] = 0);
				object value = (dataRow["RtrictType"] = obj);
				dataRow["ItmRtrictT"] = value;
				object obj4 = (dataRow["ReceiveBin"] = "N");
				obj = (dataRow["NoAutoAllc"] = obj4);
				value = (dataRow["Disabled"] = obj);
				dataRow["SngBatch"] = value;
				data.Rows.Add(dataRow);
			}
			else
			{
				DataRow dataRow2 = data.Rows[0];
				DataRow dataRow3 = data.Rows[0];
				DataRow dataRow4 = data.Rows[0];
				DataRow dataRow5 = data.Rows[0];
				object obj8 = (data.Rows[0]["NoOfBaSe"] = DBNull.Value);
				object obj4 = (dataRow5["NoOfItems"] = obj8);
				object obj = (dataRow4["ItmWght"] = obj4);
				object value = (dataRow3["ItemQty"] = obj);
				dataRow2["AbsEntry"] = value;
			}
			data.AcceptChanges();
			grdBinLocMaster.DataSource = data;
			grdBinLocMaster.RefreshDataSource();
			grdBinProp.DataSource = data;
			grdBinProp.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Add;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task<string> Save()
	{
		try
		{
			DataTable dataTable = ((FormMode != Enums.FormViewModeType.Add) ? grdBinProp.DataSourceTable : (grdBinLocMaster.DataSource as DataTable));
			if (!dataTable.IsNotNull())
			{
				return "No data to save";
			}
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "BinLocMaster");
			return await hp.PostAsync("Save", new Tuple<DataTable, string>(dataTable, (FormMode == Enums.FormViewModeType.Add) ? "A" : "U"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private async Task Remove(int absEntry)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "BinLocMaster");
			string text = await hp.PostAsync("Remove", absEntry);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				await LoadData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.Cancel)
		{
			_isLoading = true;
			await LoadData();
		}
	}

	public async Task REFRESH()
	{
		_isLoading = true;
		await AddNewRow();
	}

	private string GetBinSeperator(string whsCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BinLocMaster");
			return httpClientHelper.Get("GetBinSeperatorOfWarehouse", new { whsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "-";
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		btnOk.Enabled = isEnable;
		GridViewEx gridViewEx = gvBinLocMaster;
		bool readOnly = (gvBinProp.ReadOnly = !isEnable);
		gridViewEx.ReadOnly = readOnly;
	}

	private RepositoryItemButtonEdit LoadChooseFromListControl(ApzObjectTypes objType, Enums.TypeCFL typeCfl)
	{
		try
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Buttons.Add(new EditorButton
			{
				Kind = ButtonPredefines.Search
			});
			repositoryItemButtonEdit.Buttons[0].IsLeft = true;
			repositoryItemButtonEdit.Buttons[0].ImageOptions.Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			repositoryItemButtonEdit.Buttons[0].Click += (object s, EventArgs e) =>
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvBinProp.GetRowCellValue(gvBinProp.FocusedRowHandle, gvBinProp.FocusedColumn.FieldName))))
				{
					WinCommons._OpenProcessing();
					this.DrillDown(objType, objType, Function.ToString(gvBinProp.GetRowCellValue(gvBinProp.FocusedRowHandle, gvBinProp.FocusedColumn.FieldName)));
					WinCommons._CloseProcessing();
				}
			};
			repositoryItemButtonEdit.Buttons[1].Click += (object s, EventArgs e) =>
			{
				using frm_CFL frm_CFL2 = new frm_CFL(typeCfl, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					if (objType == ApzObjectTypes.oUoM)
					{
						gvBinProp.SetRowCellValue(gvBinProp.FocusedRowHandle, gvBinProp.FocusedColumn.FieldName, frm_CFL2.ChooseItem[2]);
					}
					else
					{
						gvBinProp.SetRowCellValue(gvBinProp.FocusedRowHandle, gvBinProp.FocusedColumn.FieldName, frm_CFL2.ChooseItem[0]);
					}
				}
			};
			return repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new RepositoryItemButtonEdit();
		}
	}

	private RepositoryItemLookUpEdit LoadLookupEditControl(ApzObjectTypes objType)
	{
		try
		{
			if (objType != ApzObjectTypes.oItemGroups && objType != ApzObjectTypes.oUoMGroups)
			{
				return null;
			}
			string controler = ((objType == ApzObjectTypes.oItemGroups) ? "ItemGroups" : "UnitOfMeasureGroups");
			string action = ((objType == ApzObjectTypes.oItemGroups) ? "GetItemGroup" : "LoadDoc");
			string empty = string.Empty;
			string empty2 = string.Empty;
			DataTable dataSource = new DataTable();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, controler))
			{
				dataSource = httpClientHelper.Get(action);
			}
			if (objType == ApzObjectTypes.oItemGroups)
			{
				empty = "ITMSGRPNAM";
				empty2 = "ITMSGRPCOD";
			}
			else
			{
				empty = "UgpName";
				empty2 = "UgpEntry";
			}
			repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo(empty));
			repositoryItemLookUpEdit.ShowHeader = false;
			repositoryItemLookUpEdit.DisplayMember = empty;
			repositoryItemLookUpEdit.ValueMember = empty2;
			repositoryItemLookUpEdit.NullText = string.Empty;
			repositoryItemLookUpEdit.DataSource = dataSource;
			return repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return null;
		}
	}

	private void LoadItemRestrictionsColumn(ItemRestrictionsType type)
	{
		try
		{
			switch (type)
			{
			case ItemRestrictionsType.None:
				gc_ItemRestrictionChoose.FieldName = string.Empty;
				gc_ItemRestrictionChoose.ColumnEdit = null;
				gc_ItemRestrictionChoose.Visible = false;
				break;
			case ItemRestrictionsType.SpecificItem:
				gc_ItemRestrictionChoose.FieldName = "SpcItmCode";
				gc_ItemRestrictionChoose.Caption = "Item";
				gc_ItemRestrictionChoose.ColumnEdit = null;
				gc_ItemRestrictionChoose.ColumnEdit = LoadChooseFromListControl(ApzObjectTypes.oItems, Enums.TypeCFL.Items);
				gc_ItemRestrictionChoose.OptionsColumn.ReadOnly = false;
				gc_ItemRestrictionChoose.OptionsColumn.AllowEdit = true;
				gc_ItemRestrictionChoose.Visible = true;
				break;
			case ItemRestrictionsType.SingleItemGroupOnly:
				gc_ItemRestrictionChoose.FieldName = "SpcItmGrpC";
				gc_ItemRestrictionChoose.Caption = "Current Item Group";
				gc_ItemRestrictionChoose.ColumnEdit = null;
				gc_ItemRestrictionChoose.OptionsColumn.ReadOnly = true;
				gc_ItemRestrictionChoose.OptionsColumn.AllowEdit = false;
				gc_ItemRestrictionChoose.Visible = true;
				break;
			case ItemRestrictionsType.SingleItemOnly:
				gc_ItemRestrictionChoose.FieldName = "SpcItmCode";
				gc_ItemRestrictionChoose.Caption = "Current Item";
				gc_ItemRestrictionChoose.FieldName = "SpcItmCode";
				gc_ItemRestrictionChoose.ColumnEdit = null;
				gc_ItemRestrictionChoose.OptionsColumn.ReadOnly = true;
				gc_ItemRestrictionChoose.OptionsColumn.AllowEdit = false;
				gc_ItemRestrictionChoose.Visible = true;
				break;
			case ItemRestrictionsType.SpecificItemGroup:
				gc_ItemRestrictionChoose.FieldName = "SpcItmGrpC";
				gc_ItemRestrictionChoose.Caption = "Item Group";
				gc_ItemRestrictionChoose.ColumnEdit = null;
				gc_ItemRestrictionChoose.ColumnEdit = LoadLookupEditControl(ApzObjectTypes.oItemGroups);
				gc_ItemRestrictionChoose.OptionsColumn.ReadOnly = false;
				gc_ItemRestrictionChoose.OptionsColumn.AllowEdit = true;
				gc_ItemRestrictionChoose.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadUoMRestrictionsColumn(UoMRestrictionsType type)
	{
		try
		{
			switch (type)
			{
			case UoMRestrictionsType.None:
				gc_UoMRestrictionChoose.FieldName = string.Empty;
				gc_UoMRestrictionChoose.ColumnEdit = null;
				gc_UoMRestrictionChoose.Visible = false;
				break;
			case UoMRestrictionsType.SpecificUoM:
				gc_UoMRestrictionChoose.FieldName = "SpcUoMCode";
				gc_UoMRestrictionChoose.Caption = "UoM";
				gc_UoMRestrictionChoose.OptionsColumn.ReadOnly = false;
				gc_UoMRestrictionChoose.OptionsColumn.AllowEdit = true;
				gc_UoMRestrictionChoose.ColumnEdit = null;
				gc_UoMRestrictionChoose.ColumnEdit = LoadChooseFromListControl(ApzObjectTypes.oUoM, Enums.TypeCFL.ListUoMs);
				gc_UoMRestrictionChoose.Visible = true;
				break;
			case UoMRestrictionsType.SingleUoMGroupOnly:
				gc_UoMRestrictionChoose.FieldName = "SpcUoMGrpC";
				gc_UoMRestrictionChoose.Caption = "Current UoM Group";
				gc_UoMRestrictionChoose.ColumnEdit = null;
				gc_UoMRestrictionChoose.OptionsColumn.ReadOnly = true;
				gc_UoMRestrictionChoose.OptionsColumn.AllowEdit = false;
				gc_UoMRestrictionChoose.Visible = true;
				break;
			case UoMRestrictionsType.SingleUoMOnly:
				gc_UoMRestrictionChoose.Caption = "Current UoM";
				gc_UoMRestrictionChoose.FieldName = "SngUoMCode";
				gc_UoMRestrictionChoose.ColumnEdit = null;
				gc_UoMRestrictionChoose.OptionsColumn.ReadOnly = true;
				gc_UoMRestrictionChoose.OptionsColumn.AllowEdit = false;
				gc_UoMRestrictionChoose.Visible = true;
				break;
			case UoMRestrictionsType.SpecificUoMGroup:
				gc_UoMRestrictionChoose.FieldName = "SpcUGPCode";
				gc_UoMRestrictionChoose.Caption = "UoM Group";
				gc_UoMRestrictionChoose.ColumnEdit = null;
				gc_UoMRestrictionChoose.ColumnEdit = LoadLookupEditControl(ApzObjectTypes.oUoMGroups);
				gc_UoMRestrictionChoose.OptionsColumn.ReadOnly = false;
				gc_UoMRestrictionChoose.OptionsColumn.AllowEdit = true;
				gc_UoMRestrictionChoose.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadSingleBatchColumn(BatchRestrictionsType type)
	{
		try
		{
			gc_SingleBatch.OptionsColumn.ReadOnly = true;
			gc_SingleBatch.OptionsColumn.AllowEdit = false;
			gc_SingleBatch.Caption = "Current Batch";
			if (type == BatchRestrictionsType.None)
			{
				gc_SingleBatch.Visible = false;
			}
			else
			{
				gc_SingleBatch.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetBinPropertiesValue(int rowHandle, string fieldName, object value)
	{
		try
		{
			gvBinProp.SetRowCellValue(rowHandle, fieldName, value);
			gvBinProp.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_BinLocMasterData_Load(object sender, EventArgs e)
	{
		SetDataSourceLookupEdit();
		await LoadBinLocField();
		await LoadData();
	}

	private async void gvBinLocMaster_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvBinLocMaster.GetFocusedRowCellValue("AbsEntry")) != 0 && FormMode == Enums.FormViewModeType.View)
			{
				await LoadProperties();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void rep_WhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gvBinLocMaster.GetFocusedRowCellValue("WhsCode"));
				if (!string.IsNullOrEmpty(text))
				{
					frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
					frm_WareHouses2._whsSearchCode = text;
					frm_WareHouses2.MdiParent = MdiParent;
					frm_WareHouses2.Show();
				}
			}
			else
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WarehouseBinActivated, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvBinLocMaster.SetFocusedRowCellValue("WhsCode", Function.ToString(frm_CFL2.ChooseItem["WhsCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void rep_SLCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gvBinLocMaster.GetFocusedValue());
				if (!string.IsNullOrEmpty(text))
				{
					frm_WHSubSetup frm_WHSubSetup2 = new frm_WHSubSetup();
					frm_WHSubSetup2.SlCode = text;
					frm_WHSubSetup2.WhFldAbs = gvBinLocMaster.FocusedColumn.VisibleIndex;
					frm_WHSubSetup2.MdiParent = MdiParent;
					frm_WHSubSetup2.Show();
				}
			}
			else
			{
				int visibleIndex = gvBinLocMaster.FocusedColumn.VisibleIndex;
				List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>
				{
					new Common.ParameterValue("fldAbs", visibleIndex)
				};
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WarehouseSublevel, false, "", "", Enums.ItemObjectType.NoItemType, lstpr, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvBinLocMaster.SetFocusedValue(Function.ToString(frm_CFL2.ChooseItem["SLCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		EnableDisableControl(isEnable: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons._OpenProcessing();
				string text = await Save();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					if (FormMode == Enums.FormViewModeType.Add)
					{
						_addConfirm = false;
						await AddNewRow();
					}
					else
					{
						_isLoading = true;
						await LoadData();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(isEnable: true);
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		Close();
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_isLoading = true;
		await AddNewRow();
	}

	private async void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow focusedDataRow = gvBinLocMaster.GetFocusedDataRow();
			DataTable dataTable = grdBinProp.DataSource as DataTable;
			if (focusedDataRow == null || !dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataFound, "");
				return;
			}
			foreach (object column in focusedDataRow.Table.Columns)
			{
				dataTable.Rows[0][column.ToString()] = focusedDataRow[column.ToString()];
			}
			dataTable.AcceptChanges();
			_isLoading = true;
			await AddNewRow(dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH_DATA();
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvBinLocMaster.GetFocusedDataRow() != null && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int absEntry = Function.ParseInt(gvBinLocMaster.GetFocusedRowCellValue("AbsEntry"));
				if (FormMode == Enums.FormViewModeType.Add)
				{
					AddNewRow();
				}
				else
				{
					await Remove(absEntry);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void gvBinLocMaster_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column == colBinCode)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				_addConfirm = true;
			}
			string text = Function.ToString(gvBinLocMaster.GetFocusedRowCellValue("WhsCode"));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string binSeperator = GetBinSeperator(text);
			for (int i = 1; i <= 4; i++)
			{
				string text2 = Function.ToString(gvBinLocMaster.GetFocusedRowCellValue("SL" + i + "Code"));
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + binSeperator + text2;
				}
			}
			gvBinLocMaster.SetFocusedRowCellValue(colBinCode, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void gvBinProp_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		int num = -1;
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		else if (FormMode == Enums.FormViewModeType.Add)
		{
			_addConfirm = true;
		}
		_ = e.Column;
		_ = colTranRest;
		if (Function.ParseInt(gvBinProp.GetFocusedValue()) == 0)
		{
			colReason.OptionsColumn.ReadOnly = true;
		}
		else
		{
			colReason.OptionsColumn.ReadOnly = false;
		}
		if (e.Column.FieldName.Equals(colItmRtrict.FieldName))
		{
			switch (Function.ParseInt(gvBinProp.GetRowCellValue(e.RowHandle, colItmRtrict.FieldName)))
			{
			case 0:
				LoadItemRestrictionsColumn(ItemRestrictionsType.None);
				break;
			case 1:
				LoadItemRestrictionsColumn(ItemRestrictionsType.SpecificItem);
				break;
			case 2:
				LoadItemRestrictionsColumn(ItemRestrictionsType.SingleItemOnly);
				break;
			case 3:
				LoadItemRestrictionsColumn(ItemRestrictionsType.SpecificItemGroup);
				break;
			case 4:
				LoadItemRestrictionsColumn(ItemRestrictionsType.SingleItemGroupOnly);
				break;
			}
		}
		else if (e.Column.FieldName.Equals(colUoMRtrict.FieldName))
		{
			switch (Function.ParseInt(gvBinProp.GetRowCellValue(e.RowHandle, colUoMRtrict.FieldName)))
			{
			case 0:
				LoadUoMRestrictionsColumn(UoMRestrictionsType.None);
				break;
			case 1:
				LoadUoMRestrictionsColumn(UoMRestrictionsType.SpecificUoM);
				break;
			case 2:
				LoadUoMRestrictionsColumn(UoMRestrictionsType.SingleUoMOnly);
				break;
			case 3:
				LoadUoMRestrictionsColumn(UoMRestrictionsType.SpecificUoMGroup);
				break;
			case 4:
				LoadUoMRestrictionsColumn(UoMRestrictionsType.SingleUoMGroupOnly);
				break;
			}
		}
		else if (e.Column.FieldName.Equals(colBatchRes.FieldName))
		{
			string text = Function.ToString(gvBinProp.GetRowCellValue(e.RowHandle, colBatchRes.FieldName));
			if (!(text == "Y"))
			{
				if (text == "N")
				{
					LoadSingleBatchColumn(BatchRestrictionsType.None);
				}
			}
			else
			{
				LoadSingleBatchColumn(BatchRestrictionsType.SingleBatch);
			}
		}
		else if (e.Column.FieldName.Equals(colTranRest.FieldName))
		{
			num = Function.ParseInt(gvBinProp.GetRowCellValue(e.RowHandle, colTranRest.FieldName));
			if (num > 0)
			{
				colReason.OptionsColumn.ReadOnly = false;
				colReason.OptionsColumn.AllowEdit = true;
			}
			else
			{
				colReason.OptionsColumn.ReadOnly = true;
				colReason.OptionsColumn.AllowEdit = false;
				SetBinPropertiesValue(e.RowHandle, "RtrictResn", null);
			}
		}
	}

	private async void gvBinLocMaster_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (_isLoading)
		{
			e.Allow = false;
		}
		else if (FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Allow = false;
				return;
			}
			_isLoading = true;
			await LoadData();
		}
	}

	private async void frm_BinLocMasterData_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Update && (FormMode != Enums.FormViewModeType.Add || !_addConfirm))
		{
			return;
		}
		switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel))
		{
		case DialogResult.Yes:
		{
			e.Cancel = true;
			EnableDisableControl(isEnable: false);
			WinCommons._OpenProcessing();
			string text = await Save();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				EnableDisableControl(isEnable: true);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				Close();
			}
			break;
		}
		case DialogResult.Cancel:
			e.Cancel = true;
			break;
		}
	}

	private void rep_Attribute_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = (int)gvBinProp.FocusedColumn.Tag;
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gvBinProp.GetFocusedValue());
				if (!string.IsNullOrEmpty(text))
				{
					frm_AttrCodeSetup frm_AttrCodeSetup2 = new frm_AttrCodeSetup();
					frm_AttrCodeSetup2.AttrValue = text;
					frm_AttrCodeSetup2.FldNum = num - 4;
					frm_AttrCodeSetup2.MdiParent = MdiParent;
					frm_AttrCodeSetup2.Show();
				}
			}
			else
			{
				List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>
				{
					new Common.ParameterValue("fldAbs", num)
				};
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.BinLocAttribute, false, "", "", Enums.ItemObjectType.NoItemType, lstpr, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvBinProp.SetFocusedValue(Function.ToString(frm_CFL2.ChooseItem["AttrValue"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grdBinLocMaster = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvBinLocMaster = new Apzon.Controls.Grid.GridViewEx();
		this.colWarehouseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colSL1Code = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SLCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colSL2Code = new Apzon.Controls.Grid.GridColumnEx();
		this.colSL3Code = new Apzon.Controls.Grid.GridColumnEx();
		this.colSL4Code = new Apzon.Controls.Grid.GridColumnEx();
		this.colBinCode = new Apzon.Controls.Grid.GridColumnEx();
		this.grdBinProp = new Apzon.Controls.Grid.GridControlEx();
		this.gvBinProp = new Apzon.Controls.Grid.GridViewEx();
		this.colInactive = new Apzon.Controls.Grid.GridColumnEx();
		this.ceInactive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.colReceiveBin = new Apzon.Controls.Grid.GridColumnEx();
		this.colNoAutoAllc = new Apzon.Controls.Grid.GridColumnEx();
		this.colDescr = new Apzon.Controls.Grid.GridColumnEx();
		this.colWeight = new Apzon.Controls.Grid.GridColumnEx();
		this.colItemQty = new Apzon.Controls.Grid.GridColumnEx();
		this.colNoItems = new Apzon.Controls.Grid.GridColumnEx();
		this.colNoBaSe = new Apzon.Controls.Grid.GridColumnEx();
		this.colAltSortCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colBarCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colMinQty = new Apzon.Controls.Grid.GridColumnEx();
		this.colMaxQty = new Apzon.Controls.Grid.GridColumnEx();
		this.colMaxWeight = new Apzon.Controls.Grid.GridColumnEx();
		this.colItmRtrict = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItmRstrict = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ItemRestrictionChoose = new Apzon.Controls.Grid.GridColumnEx();
		this.colUoMRtrict = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UoMRtrict = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_UoMRestrictionChoose = new Apzon.Controls.Grid.GridColumnEx();
		this.colBatchRes = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BatchRtrict = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_SingleBatch = new Apzon.Controls.Grid.GridColumnEx();
		this.colTranRest = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TranRstrict = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colLastUpd = new Apzon.Controls.Grid.GridColumnEx();
		this.colReason = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Attribute = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colAttr2 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr3 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr4 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr5 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr6 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr7 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr8 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr9 = new Apzon.Controls.Grid.GridColumnEx();
		this.colAttr10 = new Apzon.Controls.Grid.GridColumnEx();
		this.lblProp = new DevExpress.XtraEditors.LabelControl();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.grdBinLocMaster).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvBinLocMaster).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SLCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdBinProp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinProp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ceInactive).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItmRstrict).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UoMRtrict).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchRtrict).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TranRstrict).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Attribute).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.grdBinLocMaster.AllowPasteExcel = true;
		this.grdBinLocMaster.ClipboardData = null;
		this.grdBinLocMaster.ContextMenuStrip = this.contextMenuStrip1;
		this.grdBinLocMaster.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdBinLocMaster.Location = new System.Drawing.Point(0, 0);
		this.grdBinLocMaster.MainView = this.gvBinLocMaster;
		this.grdBinLocMaster.Name = "grdBinLocMaster";
		this.grdBinLocMaster.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_WhsCode, this.rep_SLCode });
		this.grdBinLocMaster.Size = new System.Drawing.Size(867, 266);
		this.grdBinLocMaster.TabIndex = 0;
		this.grdBinLocMaster.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBinLocMaster });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.duplicateRowToolStripMenuItem, this.refreshToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(151, 92);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvBinLocMaster.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBinLocMaster.ColumnKey = null;
		this.gvBinLocMaster.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.colWarehouseCode, this.colSL1Code, this.colSL2Code, this.colSL3Code, this.colSL4Code, this.colBinCode });
		this.gvBinLocMaster.DisableFormSetting = false;
		this.gvBinLocMaster.DisplayException = false;
		this.gvBinLocMaster.EditColumn = null;
		this.gvBinLocMaster.FieldCurrency = null;
		this.gvBinLocMaster.FieldRate = null;
		this.gvBinLocMaster.FormId = null;
		this.gvBinLocMaster.GridControl = this.grdBinLocMaster;
		this.gvBinLocMaster.IgnoreFilter = false;
		this.gvBinLocMaster.IndexEditRow = -1;
		this.gvBinLocMaster.IndicatorWidth = 35;
		this.gvBinLocMaster.IsApzTable = false;
		this.gvBinLocMaster.IsInsertNewRow = false;
		this.gvBinLocMaster.IsLoadingData = false;
		this.gvBinLocMaster.IsMappingContextAction = false;
		this.gvBinLocMaster.ItemId = null;
		this.gvBinLocMaster.LstColumnAllowBatch = null;
		this.gvBinLocMaster.LstColumnNotAllowBatch = null;
		this.gvBinLocMaster.LstColumnNotEditable = null;
		this.gvBinLocMaster.LstColumNotSetting = null;
		this.gvBinLocMaster.Name = "gvBinLocMaster";
		this.gvBinLocMaster.OptionsCustomization.AllowColumnMoving = false;
		this.gvBinLocMaster.OptionsCustomization.AllowQuickHideColumns = false;
		this.gvBinLocMaster.OptionsView.ShowGroupPanel = false;
		this.gvBinLocMaster.UseBatchMod = false;
		this.gvBinLocMaster.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvBinLocMaster_FocusedRowObjectChanged);
		this.gvBinLocMaster.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBinLocMaster_CellValueChanged);
		this.gvBinLocMaster.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvBinLocMaster_BeforeLeaveRow);
		this.colWarehouseCode.AppearanceCell.Options.UseTextOptions = true;
		this.colWarehouseCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colWarehouseCode.Caption = "Warehouse";
		this.colWarehouseCode.ColumnEdit = this.rep_WhsCode;
		this.colWarehouseCode.CurrencyDateField = null;
		this.colWarehouseCode.CurrencyField = null;
		this.colWarehouseCode.CurrencyFixed = null;
		this.colWarehouseCode.CurrencyFixedRate = 0.0;
		this.colWarehouseCode.CurrencyRateField = null;
		this.colWarehouseCode.FieldName = "WhsCode";
		this.colWarehouseCode.Name = "colWarehouseCode";
		this.colWarehouseCode.PrimaryKey = true;
		this.colWarehouseCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colWarehouseCode.UnitField = null;
		this.colWarehouseCode.Visible = true;
		this.colWarehouseCode.VisibleIndex = 0;
		this.colWarehouseCode.Width = 164;
		this.rep_WhsCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions2.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_WhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WhsCode_ButtonClick);
		this.colSL1Code.AppearanceCell.Options.UseTextOptions = true;
		this.colSL1Code.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colSL1Code.ColumnEdit = this.rep_SLCode;
		this.colSL1Code.CurrencyDateField = null;
		this.colSL1Code.CurrencyField = null;
		this.colSL1Code.CurrencyFixed = null;
		this.colSL1Code.CurrencyFixedRate = 0.0;
		this.colSL1Code.CurrencyRateField = null;
		this.colSL1Code.FieldName = "SL1Code";
		this.colSL1Code.Name = "colSL1Code";
		this.colSL1Code.PrimaryKey = false;
		this.colSL1Code.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colSL1Code.UnitField = null;
		this.colSL1Code.Visible = true;
		this.colSL1Code.VisibleIndex = 1;
		this.colSL1Code.Width = 151;
		this.rep_SLCode.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions4.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.rep_SLCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_SLCode.Name = "rep_SLCode";
		this.rep_SLCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_SLCode_ButtonClick);
		this.colSL2Code.AppearanceCell.Options.UseTextOptions = true;
		this.colSL2Code.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colSL2Code.ColumnEdit = this.rep_SLCode;
		this.colSL2Code.CurrencyDateField = null;
		this.colSL2Code.CurrencyField = null;
		this.colSL2Code.CurrencyFixed = null;
		this.colSL2Code.CurrencyFixedRate = 0.0;
		this.colSL2Code.CurrencyRateField = null;
		this.colSL2Code.FieldName = "SL2Code";
		this.colSL2Code.Name = "colSL2Code";
		this.colSL2Code.PrimaryKey = false;
		this.colSL2Code.UnitField = null;
		this.colSL2Code.Width = 149;
		this.colSL3Code.AppearanceCell.Options.UseTextOptions = true;
		this.colSL3Code.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colSL3Code.ColumnEdit = this.rep_SLCode;
		this.colSL3Code.CurrencyDateField = null;
		this.colSL3Code.CurrencyField = null;
		this.colSL3Code.CurrencyFixed = null;
		this.colSL3Code.CurrencyFixedRate = 0.0;
		this.colSL3Code.CurrencyRateField = null;
		this.colSL3Code.FieldName = "SL3Code";
		this.colSL3Code.Name = "colSL3Code";
		this.colSL3Code.PrimaryKey = false;
		this.colSL3Code.UnitField = null;
		this.colSL3Code.Width = 156;
		this.colSL4Code.ColumnEdit = this.rep_SLCode;
		this.colSL4Code.CurrencyDateField = null;
		this.colSL4Code.CurrencyField = null;
		this.colSL4Code.CurrencyFixed = null;
		this.colSL4Code.CurrencyFixedRate = 0.0;
		this.colSL4Code.CurrencyRateField = null;
		this.colSL4Code.FieldName = "SL4Code";
		this.colSL4Code.Name = "colSL4Code";
		this.colSL4Code.PrimaryKey = false;
		this.colSL4Code.UnitField = null;
		this.colBinCode.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colBinCode.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colBinCode.AppearanceCell.Options.UseBackColor = true;
		this.colBinCode.AppearanceCell.Options.UseTextOptions = true;
		this.colBinCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colBinCode.Caption = "Bin Location Code";
		this.colBinCode.CurrencyDateField = null;
		this.colBinCode.CurrencyField = null;
		this.colBinCode.CurrencyFixed = null;
		this.colBinCode.CurrencyFixedRate = 0.0;
		this.colBinCode.CurrencyRateField = null;
		this.colBinCode.FieldName = "BinCode";
		this.colBinCode.Name = "colBinCode";
		this.colBinCode.OptionsColumn.AllowEdit = false;
		this.colBinCode.OptionsColumn.AllowFocus = false;
		this.colBinCode.PrimaryKey = false;
		this.colBinCode.UnitField = null;
		this.colBinCode.Visible = true;
		this.colBinCode.VisibleIndex = 2;
		this.colBinCode.Width = 221;
		this.grdBinProp.AllowPasteExcel = true;
		this.grdBinProp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdBinProp.ClipboardData = null;
		this.grdBinProp.Location = new System.Drawing.Point(0, 20);
		this.grdBinProp.MainView = this.gvBinProp;
		this.grdBinProp.Name = "grdBinProp";
		this.grdBinProp.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.ceInactive, this.rep_ItmRstrict, this.rep_UoMRtrict, this.rep_BatchRtrict, this.rep_TranRstrict, this.rep_Attribute });
		this.grdBinProp.Size = new System.Drawing.Size(867, 128);
		this.grdBinProp.TabIndex = 1;
		this.grdBinProp.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBinProp });
		this.gvBinProp.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBinProp.ColumnKey = null;
		this.gvBinProp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[32]
		{
			this.colInactive, this.colReceiveBin, this.colNoAutoAllc, this.colDescr, this.colWeight, this.colItemQty, this.colNoItems, this.colNoBaSe, this.colAltSortCode, this.colBarCode,
			this.colMinQty, this.colMaxQty, this.colMaxWeight, this.colItmRtrict, this.gc_ItemRestrictionChoose, this.colUoMRtrict, this.gc_UoMRestrictionChoose, this.colBatchRes, this.gc_SingleBatch, this.colTranRest,
			this.colLastUpd, this.colReason, this.colAttr1, this.colAttr2, this.colAttr3, this.colAttr4, this.colAttr5, this.colAttr6, this.colAttr7, this.colAttr8,
			this.colAttr9, this.colAttr10
		});
		this.gvBinProp.DisableFormSetting = false;
		this.gvBinProp.DisplayException = false;
		this.gvBinProp.EditColumn = null;
		this.gvBinProp.FieldCurrency = null;
		this.gvBinProp.FieldRate = null;
		this.gvBinProp.FormId = null;
		this.gvBinProp.GridControl = this.grdBinProp;
		this.gvBinProp.IgnoreFilter = false;
		this.gvBinProp.IndexEditRow = -1;
		this.gvBinProp.IndicatorWidth = 35;
		this.gvBinProp.IsApzTable = false;
		this.gvBinProp.IsInsertNewRow = false;
		this.gvBinProp.IsLoadingData = false;
		this.gvBinProp.IsMappingContextAction = false;
		this.gvBinProp.ItemId = null;
		this.gvBinProp.LstColumnAllowBatch = null;
		this.gvBinProp.LstColumnNotAllowBatch = null;
		this.gvBinProp.LstColumnNotEditable = null;
		this.gvBinProp.LstColumNotSetting = null;
		this.gvBinProp.Name = "gvBinProp";
		this.gvBinProp.OptionsView.ColumnAutoWidth = false;
		this.gvBinProp.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
		this.gvBinProp.OptionsView.ShowGroupPanel = false;
		this.gvBinProp.OptionsView.ShowIndicator = false;
		this.gvBinProp.UseBatchMod = false;
		this.gvBinProp.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBinProp_CellValueChanged);
		this.colInactive.Caption = "Inactive";
		this.colInactive.ColumnEdit = this.ceInactive;
		this.colInactive.CurrencyDateField = null;
		this.colInactive.CurrencyField = null;
		this.colInactive.CurrencyFixed = null;
		this.colInactive.CurrencyFixedRate = 0.0;
		this.colInactive.CurrencyRateField = null;
		this.colInactive.FieldName = "Disabled";
		this.colInactive.Name = "colInactive";
		this.colInactive.PrimaryKey = true;
		this.colInactive.UnitField = null;
		this.colInactive.Visible = true;
		this.colInactive.VisibleIndex = 0;
		this.ceInactive.AutoHeight = false;
		this.ceInactive.Name = "ceInactive";
		this.ceInactive.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ceInactive.ValueChecked = "Y";
		this.ceInactive.ValueUnchecked = "N";
		this.colReceiveBin.Caption = "Receiving Bin Location";
		this.colReceiveBin.ColumnEdit = this.ceInactive;
		this.colReceiveBin.CurrencyDateField = null;
		this.colReceiveBin.CurrencyField = null;
		this.colReceiveBin.CurrencyFixed = null;
		this.colReceiveBin.CurrencyFixedRate = 0.0;
		this.colReceiveBin.CurrencyRateField = null;
		this.colReceiveBin.FieldName = "ReceiveBin";
		this.colReceiveBin.Name = "colReceiveBin";
		this.colReceiveBin.PrimaryKey = false;
		this.colReceiveBin.UnitField = null;
		this.colReceiveBin.Visible = true;
		this.colReceiveBin.VisibleIndex = 1;
		this.colNoAutoAllc.Caption = "Exclude from Auto. Alloc. on Issue";
		this.colNoAutoAllc.ColumnEdit = this.ceInactive;
		this.colNoAutoAllc.CurrencyDateField = null;
		this.colNoAutoAllc.CurrencyField = null;
		this.colNoAutoAllc.CurrencyFixed = null;
		this.colNoAutoAllc.CurrencyFixedRate = 0.0;
		this.colNoAutoAllc.CurrencyRateField = null;
		this.colNoAutoAllc.FieldName = "NoAutoAllc";
		this.colNoAutoAllc.Name = "colNoAutoAllc";
		this.colNoAutoAllc.PrimaryKey = false;
		this.colNoAutoAllc.UnitField = null;
		this.colNoAutoAllc.Visible = true;
		this.colNoAutoAllc.VisibleIndex = 2;
		this.colDescr.Caption = "Description";
		this.colDescr.CurrencyDateField = null;
		this.colDescr.CurrencyField = null;
		this.colDescr.CurrencyFixed = null;
		this.colDescr.CurrencyFixedRate = 0.0;
		this.colDescr.CurrencyRateField = null;
		this.colDescr.FieldName = "Descr";
		this.colDescr.Name = "colDescr";
		this.colDescr.PrimaryKey = false;
		this.colDescr.UnitField = null;
		this.colDescr.Visible = true;
		this.colDescr.VisibleIndex = 3;
		this.colWeight.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colWeight.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colWeight.AppearanceCell.Options.UseBackColor = true;
		this.colWeight.Caption = "Item Weight";
		this.colWeight.CurrencyDateField = null;
		this.colWeight.CurrencyField = null;
		this.colWeight.CurrencyFixed = null;
		this.colWeight.CurrencyFixedRate = 0.0;
		this.colWeight.CurrencyRateField = null;
		this.colWeight.FieldName = "ItmWght";
		this.colWeight.Name = "colWeight";
		this.colWeight.OptionsColumn.AllowEdit = false;
		this.colWeight.OptionsColumn.AllowFocus = false;
		this.colWeight.PrimaryKey = false;
		this.colWeight.UnitField = null;
		this.colWeight.Visible = true;
		this.colWeight.VisibleIndex = 4;
		this.colItemQty.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colItemQty.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colItemQty.AppearanceCell.Options.UseBackColor = true;
		this.colItemQty.Caption = "Item Qty";
		this.colItemQty.CurrencyDateField = null;
		this.colItemQty.CurrencyField = null;
		this.colItemQty.CurrencyFixed = null;
		this.colItemQty.CurrencyFixedRate = 0.0;
		this.colItemQty.CurrencyRateField = null;
		this.colItemQty.FieldName = "ItemQty";
		this.colItemQty.Name = "colItemQty";
		this.colItemQty.OptionsColumn.AllowEdit = false;
		this.colItemQty.OptionsColumn.AllowFocus = false;
		this.colItemQty.PrimaryKey = false;
		this.colItemQty.UnitField = null;
		this.colItemQty.Visible = true;
		this.colItemQty.VisibleIndex = 5;
		this.colNoItems.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colNoItems.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colNoItems.AppearanceCell.Options.UseBackColor = true;
		this.colNoItems.Caption = "No. of Items";
		this.colNoItems.CurrencyDateField = null;
		this.colNoItems.CurrencyField = null;
		this.colNoItems.CurrencyFixed = null;
		this.colNoItems.CurrencyFixedRate = 0.0;
		this.colNoItems.CurrencyRateField = null;
		this.colNoItems.FieldName = "NoOfItems";
		this.colNoItems.Name = "colNoItems";
		this.colNoItems.OptionsColumn.AllowEdit = false;
		this.colNoItems.OptionsColumn.AllowFocus = false;
		this.colNoItems.PrimaryKey = false;
		this.colNoItems.UnitField = null;
		this.colNoItems.Visible = true;
		this.colNoItems.VisibleIndex = 6;
		this.colNoBaSe.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colNoBaSe.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colNoBaSe.AppearanceCell.Options.UseBackColor = true;
		this.colNoBaSe.Caption = "No. of Batches/Serials";
		this.colNoBaSe.CurrencyDateField = null;
		this.colNoBaSe.CurrencyField = null;
		this.colNoBaSe.CurrencyFixed = null;
		this.colNoBaSe.CurrencyFixedRate = 0.0;
		this.colNoBaSe.CurrencyRateField = null;
		this.colNoBaSe.FieldName = "NoOfBaSe";
		this.colNoBaSe.Name = "colNoBaSe";
		this.colNoBaSe.OptionsColumn.AllowEdit = false;
		this.colNoBaSe.OptionsColumn.AllowFocus = false;
		this.colNoBaSe.PrimaryKey = false;
		this.colNoBaSe.UnitField = null;
		this.colNoBaSe.Visible = true;
		this.colNoBaSe.VisibleIndex = 7;
		this.colAltSortCode.Caption = "Alternative Sort Code";
		this.colAltSortCode.CurrencyDateField = null;
		this.colAltSortCode.CurrencyField = null;
		this.colAltSortCode.CurrencyFixed = null;
		this.colAltSortCode.CurrencyFixedRate = 0.0;
		this.colAltSortCode.CurrencyRateField = null;
		this.colAltSortCode.FieldName = "AltSortCod";
		this.colAltSortCode.Name = "colAltSortCode";
		this.colAltSortCode.PrimaryKey = false;
		this.colAltSortCode.UnitField = null;
		this.colAltSortCode.Visible = true;
		this.colAltSortCode.VisibleIndex = 8;
		this.colBarCode.Caption = "Bar Code";
		this.colBarCode.CurrencyDateField = null;
		this.colBarCode.CurrencyField = null;
		this.colBarCode.CurrencyFixed = null;
		this.colBarCode.CurrencyFixedRate = 0.0;
		this.colBarCode.CurrencyRateField = null;
		this.colBarCode.FieldName = "BarCode";
		this.colBarCode.Name = "colBarCode";
		this.colBarCode.PrimaryKey = false;
		this.colBarCode.UnitField = null;
		this.colBarCode.Visible = true;
		this.colBarCode.VisibleIndex = 9;
		this.colMinQty.Caption = "Minimum Qty";
		this.colMinQty.CurrencyDateField = null;
		this.colMinQty.CurrencyField = null;
		this.colMinQty.CurrencyFixed = null;
		this.colMinQty.CurrencyFixedRate = 0.0;
		this.colMinQty.CurrencyRateField = null;
		this.colMinQty.FieldName = "MinLevel";
		this.colMinQty.Name = "colMinQty";
		this.colMinQty.PrimaryKey = false;
		this.colMinQty.UnitField = null;
		this.colMinQty.Visible = true;
		this.colMinQty.VisibleIndex = 10;
		this.colMaxQty.Caption = "Maximum Qty";
		this.colMaxQty.CurrencyDateField = null;
		this.colMaxQty.CurrencyField = null;
		this.colMaxQty.CurrencyFixed = null;
		this.colMaxQty.CurrencyFixedRate = 0.0;
		this.colMaxQty.CurrencyRateField = null;
		this.colMaxQty.FieldName = "MaxLevel";
		this.colMaxQty.Name = "colMaxQty";
		this.colMaxQty.PrimaryKey = false;
		this.colMaxQty.UnitField = null;
		this.colMaxQty.Visible = true;
		this.colMaxQty.VisibleIndex = 11;
		this.colMaxWeight.Caption = "Maximum Weight";
		this.colMaxWeight.CurrencyDateField = null;
		this.colMaxWeight.CurrencyField = null;
		this.colMaxWeight.CurrencyFixed = null;
		this.colMaxWeight.CurrencyFixedRate = 0.0;
		this.colMaxWeight.CurrencyRateField = null;
		this.colMaxWeight.Name = "colMaxWeight";
		this.colMaxWeight.PrimaryKey = false;
		this.colMaxWeight.UnitField = null;
		this.colMaxWeight.Visible = true;
		this.colMaxWeight.VisibleIndex = 12;
		this.colItmRtrict.Caption = "Item Restrictions";
		this.colItmRtrict.ColumnEdit = this.rep_ItmRstrict;
		this.colItmRtrict.CurrencyDateField = null;
		this.colItmRtrict.CurrencyField = null;
		this.colItmRtrict.CurrencyFixed = null;
		this.colItmRtrict.CurrencyFixedRate = 0.0;
		this.colItmRtrict.CurrencyRateField = null;
		this.colItmRtrict.FieldName = "ItmRtrictT";
		this.colItmRtrict.Name = "colItmRtrict";
		this.colItmRtrict.PrimaryKey = false;
		this.colItmRtrict.UnitField = null;
		this.colItmRtrict.Visible = true;
		this.colItmRtrict.VisibleIndex = 13;
		this.rep_ItmRstrict.AutoHeight = false;
		this.rep_ItmRstrict.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ItmRstrict.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.rep_ItmRstrict.DisplayMember = "Name";
		this.rep_ItmRstrict.Name = "rep_ItmRstrict";
		this.rep_ItmRstrict.NullText = "";
		this.rep_ItmRstrict.ShowHeader = false;
		this.rep_ItmRstrict.ValueMember = "ID";
		this.gc_ItemRestrictionChoose.CurrencyDateField = null;
		this.gc_ItemRestrictionChoose.CurrencyField = null;
		this.gc_ItemRestrictionChoose.CurrencyFixed = null;
		this.gc_ItemRestrictionChoose.CurrencyFixedRate = 0.0;
		this.gc_ItemRestrictionChoose.CurrencyRateField = null;
		this.gc_ItemRestrictionChoose.Name = "gc_ItemRestrictionChoose";
		this.gc_ItemRestrictionChoose.PrimaryKey = false;
		this.gc_ItemRestrictionChoose.UnitField = null;
		this.colUoMRtrict.Caption = "UoM Restrictions";
		this.colUoMRtrict.ColumnEdit = this.rep_UoMRtrict;
		this.colUoMRtrict.CurrencyDateField = null;
		this.colUoMRtrict.CurrencyField = null;
		this.colUoMRtrict.CurrencyFixed = null;
		this.colUoMRtrict.CurrencyFixedRate = 0.0;
		this.colUoMRtrict.CurrencyRateField = null;
		this.colUoMRtrict.FieldName = "UoMRtrict";
		this.colUoMRtrict.Name = "colUoMRtrict";
		this.colUoMRtrict.PrimaryKey = false;
		this.colUoMRtrict.UnitField = null;
		this.colUoMRtrict.Visible = true;
		this.colUoMRtrict.VisibleIndex = 14;
		this.rep_UoMRtrict.AutoHeight = false;
		this.rep_UoMRtrict.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UoMRtrict.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name4")
		});
		this.rep_UoMRtrict.DisplayMember = "Name";
		this.rep_UoMRtrict.Name = "rep_UoMRtrict";
		this.rep_UoMRtrict.NullText = "";
		this.rep_UoMRtrict.ShowHeader = false;
		this.rep_UoMRtrict.ValueMember = "ID";
		this.gc_UoMRestrictionChoose.CurrencyDateField = null;
		this.gc_UoMRestrictionChoose.CurrencyField = null;
		this.gc_UoMRestrictionChoose.CurrencyFixed = null;
		this.gc_UoMRestrictionChoose.CurrencyFixedRate = 0.0;
		this.gc_UoMRestrictionChoose.CurrencyRateField = null;
		this.gc_UoMRestrictionChoose.Name = "gc_UoMRestrictionChoose";
		this.gc_UoMRestrictionChoose.PrimaryKey = false;
		this.gc_UoMRestrictionChoose.UnitField = null;
		this.colBatchRes.Caption = "Batch Restriction";
		this.colBatchRes.ColumnEdit = this.rep_BatchRtrict;
		this.colBatchRes.CurrencyDateField = null;
		this.colBatchRes.CurrencyField = null;
		this.colBatchRes.CurrencyFixed = null;
		this.colBatchRes.CurrencyFixedRate = 0.0;
		this.colBatchRes.CurrencyRateField = null;
		this.colBatchRes.FieldName = "SngBatch";
		this.colBatchRes.Name = "colBatchRes";
		this.colBatchRes.PrimaryKey = false;
		this.colBatchRes.UnitField = null;
		this.colBatchRes.Visible = true;
		this.colBatchRes.VisibleIndex = 15;
		this.rep_BatchRtrict.AutoHeight = false;
		this.rep_BatchRtrict.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_BatchRtrict.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name5", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name6")
		});
		this.rep_BatchRtrict.DisplayMember = "Name";
		this.rep_BatchRtrict.Name = "rep_BatchRtrict";
		this.rep_BatchRtrict.NullText = "";
		this.rep_BatchRtrict.ShowHeader = false;
		this.rep_BatchRtrict.ValueMember = "Code";
		this.gc_SingleBatch.CurrencyDateField = null;
		this.gc_SingleBatch.CurrencyField = null;
		this.gc_SingleBatch.CurrencyFixed = null;
		this.gc_SingleBatch.CurrencyFixedRate = 0.0;
		this.gc_SingleBatch.CurrencyRateField = null;
		this.gc_SingleBatch.Name = "gc_SingleBatch";
		this.gc_SingleBatch.PrimaryKey = false;
		this.gc_SingleBatch.UnitField = null;
		this.colTranRest.Caption = "Transaction Restrictions";
		this.colTranRest.ColumnEdit = this.rep_TranRstrict;
		this.colTranRest.CurrencyDateField = null;
		this.colTranRest.CurrencyField = null;
		this.colTranRest.CurrencyFixed = null;
		this.colTranRest.CurrencyFixedRate = 0.0;
		this.colTranRest.CurrencyRateField = null;
		this.colTranRest.FieldName = "RtrictType";
		this.colTranRest.Name = "colTranRest";
		this.colTranRest.PrimaryKey = false;
		this.colTranRest.UnitField = null;
		this.colTranRest.Visible = true;
		this.colTranRest.VisibleIndex = 16;
		this.rep_TranRstrict.AutoHeight = false;
		this.rep_TranRstrict.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TranRstrict.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name7", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name8")
		});
		this.rep_TranRstrict.DisplayMember = "Name";
		this.rep_TranRstrict.Name = "rep_TranRstrict";
		this.rep_TranRstrict.NullText = "";
		this.rep_TranRstrict.ShowHeader = false;
		this.rep_TranRstrict.ValueMember = "ID";
		this.colLastUpd.Caption = "Last Updated On";
		this.colLastUpd.CurrencyDateField = null;
		this.colLastUpd.CurrencyField = null;
		this.colLastUpd.CurrencyFixed = null;
		this.colLastUpd.CurrencyFixedRate = 0.0;
		this.colLastUpd.CurrencyRateField = null;
		this.colLastUpd.FieldName = "RtrictDate";
		this.colLastUpd.Name = "colLastUpd";
		this.colLastUpd.OptionsColumn.AllowEdit = false;
		this.colLastUpd.OptionsColumn.AllowFocus = false;
		this.colLastUpd.PrimaryKey = false;
		this.colLastUpd.UnitField = null;
		this.colLastUpd.Visible = true;
		this.colLastUpd.VisibleIndex = 17;
		this.colReason.Caption = "Reason";
		this.colReason.CurrencyDateField = null;
		this.colReason.CurrencyField = null;
		this.colReason.CurrencyFixed = null;
		this.colReason.CurrencyFixedRate = 0.0;
		this.colReason.CurrencyRateField = null;
		this.colReason.FieldName = "RtrictResn";
		this.colReason.Name = "colReason";
		this.colReason.PrimaryKey = false;
		this.colReason.UnitField = null;
		this.colReason.Visible = true;
		this.colReason.VisibleIndex = 18;
		this.colAttr1.Caption = "gridColumnEx1";
		this.colAttr1.ColumnEdit = this.rep_Attribute;
		this.colAttr1.CurrencyDateField = null;
		this.colAttr1.CurrencyField = null;
		this.colAttr1.CurrencyFixed = null;
		this.colAttr1.CurrencyFixedRate = 0.0;
		this.colAttr1.CurrencyRateField = null;
		this.colAttr1.FieldName = "Attr1Val";
		this.colAttr1.Name = "colAttr1";
		this.colAttr1.PrimaryKey = false;
		this.colAttr1.UnitField = null;
		this.rep_Attribute.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions6.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.rep_Attribute.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Attribute.Name = "rep_Attribute";
		this.rep_Attribute.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Attribute_ButtonClick);
		this.colAttr2.Caption = "gridColumnEx2";
		this.colAttr2.ColumnEdit = this.rep_Attribute;
		this.colAttr2.CurrencyDateField = null;
		this.colAttr2.CurrencyField = null;
		this.colAttr2.CurrencyFixed = null;
		this.colAttr2.CurrencyFixedRate = 0.0;
		this.colAttr2.CurrencyRateField = null;
		this.colAttr2.FieldName = "Attr2Val";
		this.colAttr2.Name = "colAttr2";
		this.colAttr2.PrimaryKey = false;
		this.colAttr2.UnitField = null;
		this.colAttr3.Caption = "gridColumnEx3";
		this.colAttr3.ColumnEdit = this.rep_Attribute;
		this.colAttr3.CurrencyDateField = null;
		this.colAttr3.CurrencyField = null;
		this.colAttr3.CurrencyFixed = null;
		this.colAttr3.CurrencyFixedRate = 0.0;
		this.colAttr3.CurrencyRateField = null;
		this.colAttr3.FieldName = "Attr3Val";
		this.colAttr3.Name = "colAttr3";
		this.colAttr3.PrimaryKey = false;
		this.colAttr3.UnitField = null;
		this.colAttr4.Caption = "gridColumnEx4";
		this.colAttr4.ColumnEdit = this.rep_Attribute;
		this.colAttr4.CurrencyDateField = null;
		this.colAttr4.CurrencyField = null;
		this.colAttr4.CurrencyFixed = null;
		this.colAttr4.CurrencyFixedRate = 0.0;
		this.colAttr4.CurrencyRateField = null;
		this.colAttr4.FieldName = "Attr4Val";
		this.colAttr4.Name = "colAttr4";
		this.colAttr4.PrimaryKey = false;
		this.colAttr4.UnitField = null;
		this.colAttr5.Caption = "gridColumnEx5";
		this.colAttr5.ColumnEdit = this.rep_Attribute;
		this.colAttr5.CurrencyDateField = null;
		this.colAttr5.CurrencyField = null;
		this.colAttr5.CurrencyFixed = null;
		this.colAttr5.CurrencyFixedRate = 0.0;
		this.colAttr5.CurrencyRateField = null;
		this.colAttr5.FieldName = "Attr5Val";
		this.colAttr5.Name = "colAttr5";
		this.colAttr5.PrimaryKey = false;
		this.colAttr5.UnitField = null;
		this.colAttr6.Caption = "gridColumnEx6";
		this.colAttr6.ColumnEdit = this.rep_Attribute;
		this.colAttr6.CurrencyDateField = null;
		this.colAttr6.CurrencyField = null;
		this.colAttr6.CurrencyFixed = null;
		this.colAttr6.CurrencyFixedRate = 0.0;
		this.colAttr6.CurrencyRateField = null;
		this.colAttr6.FieldName = "Attr6Val";
		this.colAttr6.Name = "colAttr6";
		this.colAttr6.PrimaryKey = false;
		this.colAttr6.UnitField = null;
		this.colAttr7.Caption = "gridColumnEx7";
		this.colAttr7.ColumnEdit = this.rep_Attribute;
		this.colAttr7.CurrencyDateField = null;
		this.colAttr7.CurrencyField = null;
		this.colAttr7.CurrencyFixed = null;
		this.colAttr7.CurrencyFixedRate = 0.0;
		this.colAttr7.CurrencyRateField = null;
		this.colAttr7.FieldName = "Attr7Val";
		this.colAttr7.Name = "colAttr7";
		this.colAttr7.PrimaryKey = false;
		this.colAttr7.UnitField = null;
		this.colAttr8.Caption = "gridColumnEx8";
		this.colAttr8.ColumnEdit = this.rep_Attribute;
		this.colAttr8.CurrencyDateField = null;
		this.colAttr8.CurrencyField = null;
		this.colAttr8.CurrencyFixed = null;
		this.colAttr8.CurrencyFixedRate = 0.0;
		this.colAttr8.CurrencyRateField = null;
		this.colAttr8.FieldName = "Attr8Val";
		this.colAttr8.Name = "colAttr8";
		this.colAttr8.PrimaryKey = false;
		this.colAttr8.UnitField = null;
		this.colAttr9.Caption = "gridColumnEx9";
		this.colAttr9.ColumnEdit = this.rep_Attribute;
		this.colAttr9.CurrencyDateField = null;
		this.colAttr9.CurrencyField = null;
		this.colAttr9.CurrencyFixed = null;
		this.colAttr9.CurrencyFixedRate = 0.0;
		this.colAttr9.CurrencyRateField = null;
		this.colAttr9.FieldName = "Attr9Val";
		this.colAttr9.Name = "colAttr9";
		this.colAttr9.PrimaryKey = false;
		this.colAttr9.UnitField = null;
		this.colAttr10.Caption = "gridColumnEx10";
		this.colAttr10.ColumnEdit = this.rep_Attribute;
		this.colAttr10.CurrencyDateField = null;
		this.colAttr10.CurrencyField = null;
		this.colAttr10.CurrencyFixed = null;
		this.colAttr10.CurrencyFixedRate = 0.0;
		this.colAttr10.CurrencyRateField = null;
		this.colAttr10.FieldName = "Attr10Val";
		this.colAttr10.Name = "colAttr10";
		this.colAttr10.PrimaryKey = false;
		this.colAttr10.UnitField = null;
		this.lblProp.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblProp.Appearance.Options.UseFont = true;
		this.lblProp.Location = new System.Drawing.Point(12, 1);
		this.lblProp.Name = "lblProp";
		this.lblProp.Size = new System.Drawing.Size(109, 13);
		this.lblProp.TabIndex = 2;
		this.lblProp.Text = "Bin Location Properties";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 425);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 425);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 3;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdBinLocMaster);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.lblProp);
		this.splitContainerControl1.Panel2.Controls.Add(this.grdBinProp);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(867, 419);
		this.splitContainerControl1.SplitterPosition = 266;
		this.splitContainerControl1.TabIndex = 5;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(867, 460);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.FormId = "oBinLocation";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BinLocMasterData";
		this.Text = "Bin Location Master Data";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BinLocMasterData_FormClosing);
		base.Load += new System.EventHandler(this.frm_BinLocMasterData_Load);
		((System.ComponentModel.ISupportInitialize)this.grdBinLocMaster).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvBinLocMaster).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SLCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdBinProp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinProp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ceInactive).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItmRstrict).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UoMRtrict).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BatchRtrict).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TranRstrict).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Attribute).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
