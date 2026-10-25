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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.MTO_Manufacturing;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.RoutingAndResource;

public class frm_Routing : XtraFormEx
{
	private bool addRow;

	private int lineNum;

	private int rowHandle = -1;

	private bool edittingRow;

	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private ButtonEditEx btnRouCode;

	private SimpleButtonEx btSearch;

	private SplitContainerControl splitContainerControl1;

	private GroupControl grc_General;

	private GridControlEx grdDocs;

	private GridViewEx gvDocs;

	private GridColumnEx clRouCode;

	private GridColumnEx clRouName;

	private GridColumnEx clStatus;

	private GridColumnEx clvalidFrom;

	private GridColumnEx clvalidTo;

	private GridColumnEx clCardCode;

	private GridColumnEx clCardName;

	private GridColumnEx clLotSize;

	private GridColumnEx clType;

	private GridColumnEx clRemark;

	private GroupControl grc_RoutingDetail;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx clOpCode;

	private GridColumnEx clResCode;

	private GridColumnEx clKeyCode;

	private GridColumnEx clQuantity;

	private GridColumnEx clOpName;

	private GridColumnEx clUoM;

	private GridColumnEx clSetuptime;

	private GridColumnEx clMachineTime;

	private GridColumnEx clLaborTime;

	private GridColumnEx clPRT;

	private GridColumnEx clBOMCode;

	private GridColumnEx clNextOp;

	private GridColumnEx clComments;

	private LabelLine lb_Type;

	private LabelLine lb_Status;

	private TextEditEx btnRouName;

	private LabelLine lb_RoutingName;

	private LabelLine lb_RoutingCode;

	private GridColumnEx clSetupUoM;

	private GridColumnEx clMachineUoM;

	private GridColumnEx clLaborUoM;

	private GridColumnEx clModelCode;

	private GridColumnEx colRouCode;

	private GridColumnEx LineNum;

	private ContextMenuStrip contextMenuDoc;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit repControlKey;

	private RepositoryItemButtonEdit rep_btnBPCode;

	private RepositoryItemButtonEdit repModelCode;

	private RepositoryItemButtonEdit repResCode;

	private RepositoryItemComboBox repMechanicUOM;

	private ContextMenuStrip contextMenuDocs;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private GridColumnEx clPrevOp;

	private GridColumnEx clSubOpCode;

	private RepositoryItemButtonEdit repOpCode;

	private RepositoryItemButtonEdit repSubOpCode;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private RepositoryItemCheckEdit repPRT;

	private RepositoryItemButtonEdit repNextOp;

	private RepositoryItemButtonEdit repPrevOp;

	private RepositoryItemLookUpEdit repUOM;

	private SimpleButtonEx btn_NewAssign;

	private RepositoryItemButtonEdit repBOM;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private RepositoryItemLookUpEdit replokUp_Status;

	private RepositoryItemLookUpEdit replokUpType;

	private LookUpEdit lokStatus;

	private LookUpEdit lokType;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemLookUpEdit repUOM1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	public frm_Routing()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Routing"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Routing" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_frmRouting;
		btSearch.Text = Apzon.Commons.Properties.Resources.ButtonSearch;
		btSave.Text = Apzon.Commons.Properties.Resources.ButtonSave;
		btCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
		btRefresh.Text = Apzon.Commons.Properties.Resources.ButtonRefresh;
		btn_NewAssign.Text = Apzon.Commons.Properties.Resources.ButtonNewAssignment;
		deleteRowToolStripMenuItem.Text = Apzon.Commons.Properties.Resources.ctx_Delete;
		insertRowToolStripMenuItem.Text = Apzon.Commons.Properties.Resources.ctx_InsertRowBelow;
		duplicateToolStripMenuItem.Text = Apzon.Commons.Properties.Resources.ctx_Duplicate;
		deleteRowToolStripMenuItem1.Text = Apzon.Commons.Properties.Resources.ctx_Delete;
		lb_RoutingCode.Utext = Apzon.Commons.Properties.Resources.lb_RoutingCode;
		lb_RoutingName.Utext = Apzon.Commons.Properties.Resources.lb_RoutingName;
		lb_Status.Utext = Apzon.Commons.Properties.Resources.lb_Status;
		lb_Type.Utext = Apzon.Commons.Properties.Resources.lb_Type;
		clRouCode.Caption = Apzon.Commons.Properties.Resources.lb_RoutingCode;
		clRouName.Caption = Apzon.Commons.Properties.Resources.lb_RoutingName;
		clStatus.Caption = Apzon.Commons.Properties.Resources.lb_Status;
		clvalidFrom.Caption = Apzon.Commons.Properties.Resources.lb_ActiveFrom;
		clvalidTo.Caption = Apzon.Commons.Properties.Resources.lb_ActiveTo;
		clCardCode.Caption = Apzon.Commons.Properties.Resources.lb_BPCode;
		clCardName.Caption = Apzon.Commons.Properties.Resources.lb_BPName;
		clLotSize.Caption = Apzon.Commons.Properties.Resources.lb_LotSize;
		clRemark.Caption = Apzon.Commons.Properties.Resources.lb_Remark;
		clModelCode.Caption = Apzon.Commons.Properties.Resources.lb_ModelCode;
		clOpCode.Caption = Apzon.Commons.Properties.Resources.lb_OpCode;
		clOpName.Caption = Apzon.Commons.Properties.Resources.lb_Description;
		clSubOpCode.Caption = Apzon.Commons.Properties.Resources.lb_SubOpCode;
		clResCode.Caption = Apzon.Commons.Properties.Resources.lb_WorkCenter;
		clKeyCode.Caption = Apzon.Commons.Properties.Resources.lb_CtrlKey;
		clQuantity.Caption = Apzon.Commons.Properties.Resources.lb_BaseQty;
		clUoM.Caption = Apzon.Commons.Properties.Resources.lb_BaseUoM;
		clSetuptime.Caption = Apzon.Commons.Properties.Resources.lb_SetupTime;
		clSetupUoM.Caption = Apzon.Commons.Properties.Resources.lb_SetupUoM;
		clMachineTime.Caption = Apzon.Commons.Properties.Resources.lb_MachineTime;
		clMachineUoM.Caption = Apzon.Commons.Properties.Resources.lb_MachineUoM;
		clLaborTime.Caption = Apzon.Commons.Properties.Resources.lb_LaborTime;
		clLaborUoM.Caption = Apzon.Commons.Properties.Resources.lb_LaborUoM;
		clBOMCode.Caption = Apzon.Commons.Properties.Resources.lb_BOM;
		clNextOp.Caption = Apzon.Commons.Properties.Resources.lb_NextOp;
		clPrevOp.Caption = Apzon.Commons.Properties.Resources.lb_PrevOp;
		clComments.Caption = Apzon.Commons.Properties.Resources.lb_Comment;
	}

	private void frm_Routing_Load(object sender, EventArgs e)
	{
		try
		{
			LoadComponent();
			LoadDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadComponent()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Cuộn",
				ID = "C"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Tờ",
				ID = "T"
			});
			repUOM1.DataSource = list;
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "Hour",
				ID = "H"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Minutes",
				ID = "M"
			});
			repUOM.DataSource = list2;
			List<Common.Table_ID> list3 = new List<Common.Table_ID>();
			list3.Add(new Common.Table_ID
			{
				Name = "Active",
				ID = "Y"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Inactive",
				ID = "N"
			});
			replokUp_Status.DataSource = list3;
			lokStatus.Properties.DataSource = list3;
			List<Common.Table_ID> list4 = new List<Common.Table_ID>();
			list4.Add(new Common.Table_ID
			{
				Name = "Special",
				ID = "S"
			});
			list4.Add(new Common.Table_ID
			{
				Name = "Normal",
				ID = "N"
			});
			replokUpType.DataSource = list4;
			lokType.Properties.DataSource = list4;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadDocs()
	{
		DataTable dataTable = await GetDocs();
		dataTable.AcceptChanges();
		grdDocs.DataSource = dataTable;
		grdDocs.RefreshDataSource();
		AddNewRowDocs();
	}

	private void DeleteRowEndEmpty()
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteRowEndEmptyDoc()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowDoc()
	{
		try
		{
			string value = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "RouCode"));
			if (!(grdDoc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("RouCode", typeof(string));
				dataTable2.Columns.Add("LineNum", typeof(int));
				dataTable2.Columns.Add("OpCode", typeof(string));
				dataTable2.Columns.Add("ResCode", typeof(string));
				dataTable2.Columns.Add("KeyCode", typeof(string));
				dataTable2.Columns.Add("Quantity", typeof(int));
				dataTable2.Columns.Add("UoM", typeof(string));
				dataTable2.Columns.Add("SubOpCode", typeof(string));
				dataTable2.Columns.Add("OpName", typeof(string));
				dataTable2.Columns.Add("SetupTime", typeof(int));
				dataTable2.Columns.Add("SetupUoM", typeof(string));
				dataTable2.Columns.Add("MachineTime", typeof(int));
				dataTable2.Columns.Add("MachineUoM", typeof(string));
				dataTable2.Columns.Add("LaborTime", typeof(int));
				dataTable2.Columns.Add("LaborUoM", typeof(string));
				dataTable2.Columns.Add("PRT", typeof(string));
				dataTable2.Columns.Add("BOMCode", typeof(string));
				dataTable2.Columns.Add("PrevOp", typeof(string));
				dataTable2.Columns.Add("NextOp", typeof(string));
				dataTable2.Columns.Add("Comments", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["RouCode"] = value;
				lineNum++;
				dataRow["LineNum"] = lineNum;
				dataRow["OpCode"] = "";
				dataRow["ResCode"] = "";
				dataRow["KeyCode"] = "";
				dataRow["Quantity"] = 0;
				dataRow["UoM"] = "H";
				dataRow["SubOpCode"] = "";
				dataRow["OpName"] = "";
				dataRow["SetupTime"] = 0;
				dataRow["SetupUoM"] = "H";
				dataRow["MachineTime"] = 0;
				dataRow["MachineUoM"] = "H";
				dataRow["LaborTime"] = 0;
				dataRow["LaborUoM"] = "H";
				dataRow["PRT"] = "Y";
				dataRow["BOMCode"] = "";
				dataRow["PrevOp"] = "";
				dataRow["NextOp"] = "";
				dataRow["Comments"] = "";
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
				grdDoc.DataSource = dataTable2;
				grdDocs.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.RowCount - 1, "OpCode"))) || !string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.RowCount - 1, "SubOpCode"))))
			{
				if (!dataTable.Columns.Contains("RouCode"))
				{
					dataTable.Columns.Add("RouCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("LineNum"))
				{
					dataTable.Columns.Add("LineNum", typeof(int));
				}
				if (!dataTable.Columns.Contains("OpCode"))
				{
					dataTable.Columns.Add("OpCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("ResCode"))
				{
					dataTable.Columns.Add("ResCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("KeyCode"))
				{
					dataTable.Columns.Add("KeyCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("KeyCode"))
				{
					dataTable.Columns.Add("KeyCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("Quantity"))
				{
					dataTable.Columns.Add("Quantity", typeof(int));
				}
				if (!dataTable.Columns.Contains("UoM"))
				{
					dataTable.Columns.Add("UoM", typeof(string));
				}
				if (!dataTable.Columns.Contains("SubOpCode"))
				{
					dataTable.Columns.Add("SubOpCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("OpName"))
				{
					dataTable.Columns.Add("OpName", typeof(string));
				}
				if (!dataTable.Columns.Contains("SetupTime"))
				{
					dataTable.Columns.Add("SetupTime", typeof(int));
				}
				if (!dataTable.Columns.Contains("SetupUoM"))
				{
					dataTable.Columns.Add("SetupUoM", typeof(string));
				}
				if (!dataTable.Columns.Contains("MachineTime"))
				{
					dataTable.Columns.Add("MachineTime", typeof(int));
				}
				if (!dataTable.Columns.Contains("MachineUoM"))
				{
					dataTable.Columns.Add("MachineUoM", typeof(string));
				}
				if (!dataTable.Columns.Contains("LaborTime"))
				{
					dataTable.Columns.Add("LaborTime", typeof(int));
				}
				if (!dataTable.Columns.Contains("LaborUoM"))
				{
					dataTable.Columns.Add("LaborUoM", typeof(string));
				}
				if (!dataTable.Columns.Contains("PRT"))
				{
					dataTable.Columns.Add("PRT", typeof(string));
				}
				if (!dataTable.Columns.Contains("BOMCode"))
				{
					dataTable.Columns.Add("BOMCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("PrevOp"))
				{
					dataTable.Columns.Add("PrevOp", typeof(string));
				}
				if (!dataTable.Columns.Contains("NextOp"))
				{
					dataTable.Columns.Add("NextOp", typeof(string));
				}
				if (!dataTable.Columns.Contains("Comments"))
				{
					dataTable.Columns.Add("Comments", typeof(string));
				}
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["RouCode"] = value;
				lineNum++;
				dataRow2["LineNum"] = lineNum;
				dataRow2["OpCode"] = "";
				dataRow2["ResCode"] = "";
				dataRow2["KeyCode"] = "";
				dataRow2["Quantity"] = 0;
				dataRow2["UoM"] = "H";
				dataRow2["SubOpCode"] = "";
				dataRow2["OpName"] = "";
				dataRow2["SetupTime"] = 0;
				dataRow2["SetupUoM"] = "H";
				dataRow2["MachineTime"] = 0;
				dataRow2["MachineUoM"] = "H";
				dataRow2["LaborTime"] = 0;
				dataRow2["LaborUoM"] = "H";
				dataRow2["PRT"] = "";
				dataRow2["BOMCode"] = "";
				dataRow2["PrevOp"] = "";
				dataRow2["NextOp"] = "";
				dataRow2["Comments"] = "";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InsertRowDoc()
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.RowCount - 1, "OpCode"))))
			{
				DataTable dataTable = grdDoc.DataSource as DataTable;
				DataRow dataRow = dataTable.NewRow();
				string value = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "RouCode"));
				dataRow["RouCode"] = value;
				lineNum++;
				dataRow["LineNum"] = lineNum;
				dataRow["OpCode"] = "";
				dataRow["ResCode"] = "";
				dataRow["KeyCode"] = "";
				dataRow["Quantity"] = 0;
				dataRow["UoM"] = "";
				dataRow["SubOpCode"] = "";
				dataRow["OpName"] = "";
				dataRow["SetupTime"] = 0;
				dataRow["SetupUoM"] = "";
				dataRow["MachineTime"] = 0;
				dataRow["MachineUoM"] = "";
				dataRow["LaborTime"] = 0;
				dataRow["LaborUoM"] = "";
				dataRow["PRT"] = "";
				dataRow["BOMCode"] = "";
				dataRow["PrevOp"] = "";
				dataRow["NextOp"] = "";
				dataRow["Comments"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetDocs()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Routing");
			DataTable dataTable = await _helper.GetDataTableAsync("GetDocs");
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void AddNewRowDocs()
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			if (dataTable == null || dataTable.Count() == 0)
			{
				if (!dataTable.Columns.Contains("RouCode"))
				{
					dataTable.Columns.Add("RouCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("RouName"))
				{
					dataTable.Columns.Add("RouName", typeof(string));
				}
				if (!dataTable.Columns.Contains("Status"))
				{
					dataTable.Columns.Add("Status", typeof(string));
				}
				if (!dataTable.Columns.Contains("validFrom"))
				{
					dataTable.Columns.Add("validFrom", typeof(DateTime));
				}
				if (!dataTable.Columns.Contains("validTo"))
				{
					dataTable.Columns.Add("validTo", typeof(DateTime));
				}
				if (!dataTable.Columns.Contains("Type"))
				{
					dataTable.Columns.Add("Type", typeof(string));
				}
				if (!dataTable.Columns.Contains("CardCode"))
				{
					dataTable.Columns.Add("CardCode", typeof(string));
				}
				if (!dataTable.Columns.Contains("CardName"))
				{
					dataTable.Columns.Add("CardName", typeof(string));
				}
				if (!dataTable.Columns.Contains("LotSize"))
				{
					dataTable.Columns.Add("LotSize", typeof(decimal));
				}
				if (!dataTable.Columns.Contains("Remark"))
				{
					dataTable.Columns.Add("Remark", typeof(string));
				}
				if (!dataTable.Columns.Contains("ModelCode"))
				{
					dataTable.Columns.Add("ModelCode", typeof(string));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["RouCode"] = "";
				dataRow["RouName"] = "";
				dataRow["Status"] = "Y";
				dataRow["validFrom"] = DateTime.Today;
				dataRow["validTo"] = DateTime.Today;
				dataRow["Type"] = "N";
				dataRow["CardCode"] = "";
				dataRow["CardName"] = "";
				dataRow["LotSize"] = 0;
				dataRow["Remark"] = "";
				dataRow["ModelCode"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
				gvDocs.FocusedRowHandle = dataTable.Rows.Count - 1;
				GridColumn focusedColumn = gvDocs.Columns.Where((GridColumn p) => p.FieldName == "RouCode").FirstOrDefault();
				gvDocs.FocusedColumn = focusedColumn;
				gvDocs.ShowEditor();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Count() - 1]["RouCode"])))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["RouCode"] = "";
				dataRow2["RouName"] = "";
				dataRow2["Status"] = "Y";
				dataRow2["validFrom"] = DateTime.Today;
				dataRow2["validTo"] = DateTime.Today;
				dataRow2["Type"] = "N";
				dataRow2["CardCode"] = "";
				dataRow2["CardName"] = "";
				dataRow2["LotSize"] = 0;
				dataRow2["Remark"] = "";
				dataRow2["ModelCode"] = "";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grdDocs.DataSource = dataTable;
				grdDocs.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DuplicateDocs()
	{
		if (grdDocs.DataSource is DataTable dataTable)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["RouCode"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "RouCode");
			dataRow["RouName"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "RouName");
			dataRow["Status"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Status");
			dataRow["validFrom"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "validFrom");
			dataRow["validTo"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "validTo");
			dataRow["Type"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Type");
			dataRow["CardCode"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "CardCode");
			dataRow["CardName"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "CardName");
			dataRow["LotSize"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "LotSize");
			dataRow["Remark"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Remark");
			dataRow["ModelCode"] = gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "ModelCode");
			dataTable.Rows.InsertAt(dataRow, gvDocs.RowCount - 1);
			dataTable.AcceptChanges();
			grdDocs.DataSource = dataTable;
			grdDocs.RefreshDataSource();
		}
	}

	private async void GetDoc()
	{
		_ = 1;
		try
		{
			grdDoc.DataSource = null;
			string roucode = Function.ToString(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "RouCode"));
			WinCommons._OpenProcessing();
			lineNum = await GetMaxLineNum(roucode);
			WinCommons._CloseProcessing();
			DataTable dataTable = await AwaitGetDoc(roucode);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				AddNewRowDoc();
				return;
			}
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			AddNewRowDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> AwaitGetDoc(string roucode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Routing");
			DataTable dataTable = await _helper.GetDataTableAsync("GetDoc", new { roucode });
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void Save()
	{
	}

	private async void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("RouCode", typeof(string));
			dataTable.Columns.Add("RouName", typeof(string));
			dataTable.Columns.Add("Status", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["RouCode"] = (string.IsNullOrEmpty(Function.ToString(btnRouCode.EditValue)) ? "" : btnRouCode.EditValue);
			dataRow["RouName"] = (string.IsNullOrEmpty(Function.ToString(btnRouName.EditValue)) ? "" : btnRouName.EditValue);
			dataRow["Status"] = (string.IsNullOrEmpty(Function.ToString(lokStatus.EditValue)) ? "" : lokStatus.EditValue);
			dataRow["Type"] = (string.IsNullOrEmpty(Function.ToString(lokType.EditValue)) ? "" : lokType.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataSource = await AwaitSearch(dataTable);
			grdDocs.DataSource = dataSource;
			grdDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable CreatTableDocToSave(DataTable dtdoc)
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (!dataTable.Columns.Contains("RouCode"))
			{
				dataTable.Columns.Add("RouCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("LineNum"))
			{
				dataTable.Columns.Add("LineNum", typeof(int));
			}
			if (!dataTable.Columns.Contains("OpCode"))
			{
				dataTable.Columns.Add("OpCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("KeyCode"))
			{
				dataTable.Columns.Add("KeyCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("Quantity"))
			{
				dataTable.Columns.Add("Quantity", typeof(int));
			}
			if (!dataTable.Columns.Contains("UoM"))
			{
				dataTable.Columns.Add("UoM", typeof(string));
			}
			if (!dataTable.Columns.Contains("SubOpCode"))
			{
				dataTable.Columns.Add("SubOpCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("ResCode"))
			{
				dataTable.Columns.Add("ResCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("OpName"))
			{
				dataTable.Columns.Add("OpName", typeof(string));
			}
			if (!dataTable.Columns.Contains("BOMCode"))
			{
				dataTable.Columns.Add("BOMCode", typeof(string));
			}
			if (!dataTable.Columns.Contains("SetupTime"))
			{
				dataTable.Columns.Add("SetupTime", typeof(int));
			}
			if (!dataTable.Columns.Contains("SetupUoM"))
			{
				dataTable.Columns.Add("SetupUoM", typeof(string));
			}
			if (!dataTable.Columns.Contains("MachineTime"))
			{
				dataTable.Columns.Add("MachineTime", typeof(int));
			}
			if (!dataTable.Columns.Contains("MachineUoM"))
			{
				dataTable.Columns.Add("MachineUoM", typeof(string));
			}
			if (!dataTable.Columns.Contains("LaborTime"))
			{
				dataTable.Columns.Add("LaborTime", typeof(int));
			}
			if (!dataTable.Columns.Contains("LaborUoM"))
			{
				dataTable.Columns.Add("LaborUoM", typeof(string));
			}
			if (!dataTable.Columns.Contains("Comments"))
			{
				dataTable.Columns.Add("Comments", typeof(string));
			}
			if (!dataTable.Columns.Contains("PRT"))
			{
				dataTable.Columns.Add("PRT", typeof(string));
			}
			if (!dataTable.Columns.Contains("PrevOp"))
			{
				dataTable.Columns.Add("PrevOp", typeof(string));
			}
			if (!dataTable.Columns.Contains("NextOp"))
			{
				dataTable.Columns.Add("NextOp", typeof(string));
			}
			for (int i = 0; i < dtdoc.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["RouCode"] = dtdoc.Rows[i]["RouCode"];
				dataRow["LineNum"] = i;
				dataRow["OpCode"] = dtdoc.Rows[i]["OpCode"];
				dataRow["KeyCode"] = dtdoc.Rows[i]["KeyCode"];
				dataRow["Quantity"] = dtdoc.Rows[i]["Quantity"];
				dataRow["UoM"] = dtdoc.Rows[i]["UoM"];
				dataRow["SubOpCode"] = dtdoc.Rows[i]["SubOpCode"];
				dataRow["ResCode"] = dtdoc.Rows[i]["ResCode"];
				dataRow["OpName"] = dtdoc.Rows[i]["OpName"];
				dataRow["BOMCode"] = dtdoc.Rows[i]["BOMCode"];
				dataRow["SetupTime"] = dtdoc.Rows[i]["SetupTime"];
				dataRow["SetupUoM"] = dtdoc.Rows[i]["SetupUoM"];
				dataRow["MachineTime"] = dtdoc.Rows[i]["MachineTime"];
				dataRow["MachineUoM"] = dtdoc.Rows[i]["MachineUoM"];
				dataRow["LaborTime"] = dtdoc.Rows[i]["LaborTime"];
				dataRow["LaborUoM"] = dtdoc.Rows[i]["LaborUoM"];
				dataRow["Comments"] = dtdoc.Rows[i]["Comments"];
				dataRow["PRT"] = dtdoc.Rows[i]["PRT"];
				dataRow["PrevOp"] = dtdoc.Rows[i]["PrevOp"];
				dataRow["NextOp"] = dtdoc.Rows[i]["NextOp"];
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<string> WaitSaveData(DataSet dtFather)
	{
		try
		{
			using HttpClientHelper<RoutingImpl> _helper = new HttpClientHelper<RoutingImpl>(Configuration.ApiServer, "Routing");
			return await _helper.PostParamsAsJsonAsync("SaveDocs", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async Task<int> GetMaxLineNum(string roucode)
	{
		try
		{
			int result;
			using (HttpClientHelper<RoutingImpl> _helper = new HttpClientHelper<RoutingImpl>(Configuration.ApiServer, "Routing"))
			{
				result = await _helper.GetIntAsync("GetMaxLineNum", new { roucode });
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async Task<DataTable> AwaitSearch(DataTable dtsearch)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Routing");
			return await _helper.PostAsync("SearchField", dtsearch);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetSupOpCode()
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "Operation");
			return await _helper.GetDataTableAsync("GetSuoOpCode");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			edittingRow = true;
			DeleteRowDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DeleteDocs(string ids)
	{
		try
		{
			using HttpClientHelper<RoutingImpl> _helper = new HttpClientHelper<RoutingImpl>(Configuration.ApiServer, "Routing");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocs", ids);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DeleteRowDocEmpty(string ids)
	{
		try
		{
			using HttpClientHelper<RoutingImpl> _helper = new HttpClientHelper<RoutingImpl>(Configuration.ApiServer, "Routing");
			string text = await _helper.PostParamsAsJsonAsync("DeleteDocsAlone", ids);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> DeleteDoc(DataTable dts)
	{
		try
		{
			string result;
			using (HttpClientHelper<ControlKeyImpl> _helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "Routing"))
			{
				result = await _helper.PostParamsAsJsonAsync("DeleteDoc", dts);
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void DeleteRowDoc()
	{
		try
		{
			DataTable dt = (DataTable)grdDoc.DataSource;
			if (dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			string value = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "RouCode"));
			int num = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "LineNum"));
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("RouCode", typeof(string));
			dataTable.Columns.Add("LineNum", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["RouCode"] = value;
			dataRow["LineNum"] = num;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (await DeleteDoc(dataTable) != "")
			{
				dt.Rows.RemoveAt(gvDoc.FocusedRowHandle);
				dt.AcceptChanges();
				if (dt.Rows.Count > 0)
				{
					grdDoc.DataSource = dt;
					grdDoc.RefreshDataSource();
				}
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				GetDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (gvDocs.FocusedRowHandle != rowHandle && edittingRow)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					gvDocs.FocusedRowHandle = rowHandle;
				}
				else
				{
					rowHandle = gvDocs.FocusedRowHandle;
					edittingRow = false;
					GetDoc();
				}
			}
			if ((gvDocs.FocusedRowHandle != rowHandle || !edittingRow) && gvDocs.FocusedRowHandle != rowHandle && !edittingRow)
			{
				rowHandle = gvDocs.FocusedRowHandle;
				GetDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repControlKey_Click(object sender, EventArgs e)
	{
	}

	private void rep_btnBPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDocs.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: true, null);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItems[0];
				DataRow dataRow = dataTable.Rows[gvDocs.GetDataSourceRowIndex(gvDocs.FocusedRowHandle)];
				dataRow["CardCode"] = businessPartnersImpl.CardCode;
				dataRow["CardName"] = businessPartnersImpl.CardName;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = businessPartnersImpl.CardCode;
				for (int i = 1; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRow dataRow2 = dataTable.NewRow();
					BusinessPartnersImpl businessPartnersImpl2 = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItems[i];
					dataRow2["CardCode"] = businessPartnersImpl2.CardCode;
					dataRow2["CardName"] = businessPartnersImpl2.CardName;
					dataTable.Rows.Add(dataRow2);
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repModelCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private async void repResCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Resource, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				GetResCodeImpl getResCodeImpl = (GetResCodeImpl)frm_ChooseFromList2.ChooseItems[0];
				dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)]["ResCode"] = getResCodeImpl.ResCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = getResCodeImpl.ResCode;
				string value = await GetCtrlKeyByResCode(getResCodeImpl.ResCode);
				if (!string.IsNullOrEmpty(value))
				{
					gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "KeyCode", value);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> GetCtrlKeyByResCode(string resCode)
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "ProductionPlanning");
			return await _helper.PostAsync("GetCtrlKeyByResCode", resCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "Error";
		}
	}

	private void addNewRowToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		try
		{
			AddNewRowDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvDocs.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvDoc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.Name = "repositoryItemTextEditReadOnly";
			repositoryItemTextEdit.ReadOnly = true;
			for (int i = 0; i < gvDoc.RowCount; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "OpCode"))) && e.RowHandle == i && e.Column.FieldName == "SubOpCode")
				{
					e.RepositoryItem = repositoryItemTextEdit;
				}
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "SubOpCode"))) && e.RowHandle == i && e.Column.FieldName == "OpCode")
				{
					e.RepositoryItem = repositoryItemTextEdit;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingRow = true;
			if (!(e.Column.FieldName == "OpCode") && !(e.Column.FieldName == "SubOpCode"))
			{
				return;
			}
			for (int i = 0; i < gvDoc.RowCount; i++)
			{
				if (i != 0 && i != gvDoc.RowCount)
				{
					if (e.RowHandle == i && (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode"))
					{
						if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "OpCode"))))
						{
							gvDoc.SetRowCellValue(i - 1, "NextOp", gvDoc.GetRowCellValue(i, "OpCode"));
							gvDoc.SetRowCellValue(i + 1, "PrevOp", gvDoc.GetRowCellValue(i, "OpCode"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "SubOpCode"))))
						{
							gvDoc.SetRowCellValue(i - 1, "NextOp", gvDoc.GetRowCellValue(i, "SubOpCode"));
							gvDoc.SetRowCellValue(i + 1, "PrevOp", gvDoc.GetRowCellValue(i, "SubOpCode"));
						}
					}
				}
				else if (i == 0)
				{
					if (e.RowHandle == i && (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode"))
					{
						if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "OpCode"))))
						{
							gvDoc.SetRowCellValue(i + 1, "PrevOp", gvDoc.GetRowCellValue(i, "OpCode"));
						}
						if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "SubOpCode"))))
						{
							gvDoc.SetRowCellValue(i + 1, "PrevOp", gvDoc.GetRowCellValue(i, "SubOpCode"));
						}
					}
				}
				else if (e.RowHandle == i && (e.Column.FieldName == "OpCode" || e.Column.FieldName == "SubOpCode"))
				{
					if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "OpCode"))))
					{
						gvDoc.SetRowCellValue(i - 1, "NextOp", gvDoc.GetRowCellValue(i, "OpCode"));
					}
					if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(i, "SubOpCode"))))
					{
						gvDoc.SetRowCellValue(i - 1, "NextOp", gvDoc.GetRowCellValue(i, "SubOpCode"));
					}
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("OpCode"))) || !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("SubOpCode"))))
			{
				AddNewRowDoc();
			}
			else
			{
				DeleteRowEndEmptyDoc();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DuplicateDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("RouCode"))))
			{
				if (DialogResult.Yes != XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo))
				{
					return;
				}
				edittingRow = false;
				DataTable dataTable = grdDocs.DataSource as DataTable;
				DataTable dataTable2 = grdDoc.DataSource as DataTable;
				dataTable.Columns.Remove("CardName");
				dataTable.AcceptChanges();
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable.Count(); i++)
					{
						if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["RouCode"])))
						{
							dataTable.Rows.RemoveAt(i);
							dataTable2.AcceptChanges();
							i--;
						}
					}
					dataTable.AcceptChanges();
				}
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					for (int j = 0; j < dataTable2.Count(); j++)
					{
						if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["OpCode"])) && string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["SubOpCode"])))
						{
							dataTable2.Rows.RemoveAt(j);
							dataTable2.AcceptChanges();
							j--;
						}
					}
				}
				DataTable dataTable3 = CreatTableDocToSave(dataTable2);
				DataSet ds = new DataSet();
				ds.Tables.AddRange(new DataTable[2] { dataTable, dataTable3 });
				ds.AcceptChanges();
				WinCommons._OpenProcessing();
				string text = await WaitSaveData(ds);
				ds.Reset();
				ds.Clear();
				if (text == "")
				{
					LoadDocs();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.RowIsSelectedHaveRoutCodeEmpty, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingRow = true;
			if (e.Column.FieldName == "RouCode")
			{
				if (string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("RouCode"))))
				{
					DeleteRowEndEmpty();
				}
				else
				{
					AddNewRowDocs();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gvDocs.GetSelectedRows();
			if (gvDocs.GetSelectedRows() == null || gvDocs.GetSelectedRows().Count() == 0)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoNoRowSelected, Enums.MessageCode.SystemMessage);
			}
			else
			{
				for (int i = 0; i < selectedRows.Count(); i++)
				{
					if (string.IsNullOrEmpty(Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "RouCode"))))
					{
						WinCommons.Error(Enums.MessageCode.RowIsSelectedHaveRoutCodeEmpty, "");
						return;
					}
				}
				DataTable dt = grdDoc.DataSource as DataTable;
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					string text = "'";
					int[] array = selectedRows;
					foreach (int num in array)
					{
						text = text + gvDocs.GetRowCellValue(num, "RouCode").ToString() + " ','";
					}
					if (text.Length > 0)
					{
						text = text.Remove(text.Length - 2);
						if (dt.Count() > 1)
						{
							DeleteDocs(text);
						}
						else
						{
							DeleteRowDocEmpty(text);
						}
					}
				}
			}
			LoadDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repControlKey_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ControlKey, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ControlKeyImpl controlKeyImpl = (ControlKeyImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)]["KeyCode"] = controlKeyImpl.KeyCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = controlKeyImpl.KeyCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void repOpCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDocs.GetFocusedRowCellValue("RouCode"))))
			{
				DataTable dataTable = grdDoc.DataSource as DataTable;
				ButtonEdit buttonEdit = sender as ButtonEdit;
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Operation, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
					DataRow dataRow = dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)];
					dataRow["OpCode"] = operationImpl.Code;
					dataRow["OpName"] = operationImpl.Name;
					dataTable.AcceptChanges();
					buttonEdit.EditValue = operationImpl.Code;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseRoutingCodeBeforeChooseOpCode, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void repSubOpCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OperationByStatus, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)];
				dataRow["SubOpCode"] = operationImpl.Code;
				dataRow["OpName"] = operationImpl.Name;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = operationImpl.Code;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			edittingRow = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repNextOp_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Operation, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)]["NextOp"] = operationImpl.Code;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = operationImpl.Code;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btn_NewAssign_Click(object sender, EventArgs e)
	{
		try
		{
			frm_AssignComponent frm_AssignComponent2 = new frm_AssignComponent();
			frm_AssignComponent2.MdiParent = MdiParent;
			frm_AssignComponent2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repBOM_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.BOM, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dataTable = grdDoc.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				ApzBillOfMaterials apzBillOfMaterials = (ApzBillOfMaterials)frm_ChooseFromList2.ChooseItems[0];
				dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)]["BOMCode"] = apzBillOfMaterials.Code;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = apzBillOfMaterials.Code;
				for (int i = 1; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					ApzBillOfMaterials apzBillOfMaterials2 = (ApzBillOfMaterials)frm_ChooseFromList2.ChooseItems[i];
					dataRow["BOMCode"] = apzBillOfMaterials2.Code;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repPrevOp_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Operation, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				OperationImpl operationImpl = (OperationImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)]["PrevOp"] = operationImpl.Code;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = operationImpl.Code;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void repBOM_Click(object sender, EventArgs e)
	{
	}

	private void btnRouCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Routing, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				GetListRoutingImpl getListRoutingImpl = (GetListRoutingImpl)frm_ChooseFromList2.ChooseItems[0];
				btnRouCode.Text = getListRoutingImpl.RouCode;
				btnRouName.Text = getListRoutingImpl.RouName;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Routing_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (edittingRow)
			{
				if (XtraMessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				edittingRow = false;
			}
		}
		catch (Exception ex)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_Routing));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btn_NewAssign = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnRouCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grc_General = new DevExpress.XtraEditors.GroupControl();
		this.grdDocs = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuDocs = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocs = new Apzon.Controls.Grid.GridViewEx();
		this.clRouCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clRouName = new Apzon.Controls.Grid.GridColumnEx();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.replokUp_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clvalidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.clvalidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.clType = new Apzon.Controls.Grid.GridColumnEx();
		this.replokUpType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnBPCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.clLotSize = new Apzon.Controls.Grid.GridColumnEx();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.clModelCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repModelCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.grc_RoutingDetail = new DevExpress.XtraEditors.GroupControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.colRouCode = new Apzon.Controls.Grid.GridColumnEx();
		this.LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.clOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repOpCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repResCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clKeyCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.clUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOM1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clSubOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repSubOpCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clOpName = new Apzon.Controls.Grid.GridColumnEx();
		this.clSetuptime = new Apzon.Controls.Grid.GridColumnEx();
		this.clSetupUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clMachineTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clMachineUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clLaborTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clLaborUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clPRT = new Apzon.Controls.Grid.GridColumnEx();
		this.repPRT = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clBOMCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repBOM = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clNextOp = new Apzon.Controls.Grid.GridColumnEx();
		this.repNextOp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clPrevOp = new Apzon.Controls.Grid.GridColumnEx();
		this.repPrevOp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clComments = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.repControlKey = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repMechanicUOM = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.lb_Type = new Apzon.Controls.Controls.LabelLine();
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.btnRouName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_RoutingName = new Apzon.Controls.Controls.LabelLine();
		this.lb_RoutingCode = new Apzon.Controls.Controls.LabelLine();
		this.lokStatus = new DevExpress.XtraEditors.LookUpEdit();
		this.lokType = new DevExpress.XtraEditors.LookUpEdit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btnRouCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grc_General).BeginInit();
		this.grc_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDocs).BeginInit();
		this.contextMenuDocs.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.replokUp_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.replokUpType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnBPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repModelCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grc_RoutingDetail).BeginInit();
		this.grc_RoutingDetail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repSubOpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repPRT).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repBOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repNextOp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repPrevOp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repControlKey).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repMechanicUOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnRouName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lokStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lokType.Properties).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.btn_NewAssign);
		this.panel1.Controls.Add(this.btRefresh);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 362);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(845, 33);
		this.panel1.TabIndex = 61;
		this.panel1.Tag = "ss";
		this.btn_NewAssign.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewAssign.EditControl = null;
		this.btn_NewAssign.EditValue = "New Assignment";
		this.btn_NewAssign.FieldName = null;
		this.btn_NewAssign.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewAssign.ImageOptions.Image");
		this.btn_NewAssign.IsSearchControl = false;
		this.btn_NewAssign.Location = new System.Drawing.Point(678, 7);
		this.btn_NewAssign.Name = "btn_NewAssign";
		this.btn_NewAssign.ReadOnly = false;
		this.btn_NewAssign.SettingInVisible = false;
		this.btn_NewAssign.SettingReadonly = false;
		this.btn_NewAssign.Size = new System.Drawing.Size(155, 19);
		this.btn_NewAssign.TabIndex = 45;
		this.btn_NewAssign.Text = "New Assignment";
		this.btn_NewAssign.Click += new System.EventHandler(this.btn_NewAssign_Click);
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(214, 8);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(99, 19);
		this.btRefresh.TabIndex = 44;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(111, 8);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(7, 8);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Save";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		this.btnRouCode.EditControl = null;
		this.btnRouCode.FieldName = null;
		this.btnRouCode.IsSearchControl = false;
		this.btnRouCode.Location = new System.Drawing.Point(122, 5);
		this.btnRouCode.Name = "btnRouCode";
		this.btnRouCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.btnRouCode.Properties.Appearance.Options.UseFont = true;
		this.btnRouCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnRouCode.SettingInVisible = false;
		this.btnRouCode.SettingReadonly = false;
		this.btnRouCode.Size = new System.Drawing.Size(147, 18);
		this.btnRouCode.TabIndex = 59;
		this.btnRouCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnRouCode_ButtonClick);
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(548, 27);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(99, 19);
		this.btSearch.TabIndex = 58;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 58);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grc_General);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grc_RoutingDetail);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(843, 299);
		this.splitContainerControl1.SplitterPosition = 148;
		this.splitContainerControl1.TabIndex = 57;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grc_General.Controls.Add(this.grdDocs);
		this.grc_General.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grc_General.Location = new System.Drawing.Point(0, 0);
		this.grc_General.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grc_General.Name = "grc_General";
		this.grc_General.Size = new System.Drawing.Size(843, 148);
		this.grc_General.TabIndex = 0;
		this.grc_General.Text = "General";
		this.grdDocs.AllowPasteExcel = true;
		this.grdDocs.ClipboardData = null;
		this.grdDocs.ContextMenuStrip = this.contextMenuDocs;
		this.grdDocs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDocs.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Location = new System.Drawing.Point(2, 23);
		this.grdDocs.MainView = this.gvDocs;
		this.grdDocs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDocs.Name = "grdDocs";
		this.grdDocs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_btnBPCode, this.repModelCode, this.replokUp_Status, this.replokUpType, this.repositoryItemMemoExEdit1 });
		this.grdDocs.Size = new System.Drawing.Size(839, 123);
		this.grdDocs.TabIndex = 0;
		this.grdDocs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocs });
		this.contextMenuDocs.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateToolStripMenuItem, this.deleteRowToolStripMenuItem1 });
		this.contextMenuDocs.Name = "contextMenuDocs";
		this.contextMenuDocs.Size = new System.Drawing.Size(134, 48);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gvDocs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocs.ColumnKey = null;
		this.gvDocs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.clRouCode, this.clRouName, this.clStatus, this.clvalidFrom, this.clvalidTo, this.clType, this.clCardCode, this.clCardName, this.clLotSize, this.clRemark,
			this.clModelCode
		});
		this.gvDocs.DisableFormSetting = false;
		this.gvDocs.DisplayException = false;
		this.gvDocs.EditColumn = null;
		this.gvDocs.FieldCurrency = null;
		this.gvDocs.FieldRate = null;
		this.gvDocs.FormId = null;
		this.gvDocs.GridControl = this.grdDocs;
		this.gvDocs.IgnoreFilter = false;
		this.gvDocs.IndexEditRow = -1;
		this.gvDocs.IndicatorWidth = 40;
		this.gvDocs.IsApzTable = false;
		this.gvDocs.IsInsertNewRow = false;
		this.gvDocs.IsLoadingData = false;
		this.gvDocs.IsMappingContextAction = false;
		this.gvDocs.ItemId = null;
		this.gvDocs.LstColumnAllowBatch = null;
		this.gvDocs.LstColumnNotAllowBatch = null;
		this.gvDocs.LstColumnNotEditable = null;
		this.gvDocs.LstColumNotSetting = null;
		this.gvDocs.Name = "gvDocs";
		this.gvDocs.OptionsSelection.MultiSelect = true;
		this.gvDocs.OptionsView.ColumnAutoWidth = false;
		this.gvDocs.OptionsView.ShowGroupPanel = false;
		this.gvDocs.UseBatchMod = false;
		this.gvDocs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocs_CustomDrawRowIndicator);
		this.gvDocs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDocs_RowStyle);
		this.gvDocs.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDocs_FocusedRowObjectChanged);
		this.gvDocs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocs_CellValueChanged);
		this.clRouCode.Caption = "Routing Code";
		this.clRouCode.CurrencyDateField = null;
		this.clRouCode.CurrencyField = null;
		this.clRouCode.CurrencyFixed = null;
		this.clRouCode.CurrencyFixedRate = 0.0;
		this.clRouCode.CurrencyRateField = null;
		this.clRouCode.FieldName = "RouCode";
		this.clRouCode.Name = "clRouCode";
		this.clRouCode.PrimaryKey = true;
		this.clRouCode.UnitField = null;
		this.clRouCode.Visible = true;
		this.clRouCode.VisibleIndex = 0;
		this.clRouCode.Width = 105;
		this.clRouName.Caption = "Routing Name";
		this.clRouName.CurrencyDateField = null;
		this.clRouName.CurrencyField = null;
		this.clRouName.CurrencyFixed = null;
		this.clRouName.CurrencyFixedRate = 0.0;
		this.clRouName.CurrencyRateField = null;
		this.clRouName.FieldName = "RouName";
		this.clRouName.Name = "clRouName";
		this.clRouName.PrimaryKey = false;
		this.clRouName.UnitField = null;
		this.clRouName.Visible = true;
		this.clRouName.VisibleIndex = 1;
		this.clRouName.Width = 149;
		this.clStatus.Caption = "Status";
		this.clStatus.ColumnEdit = this.replokUp_Status;
		this.clStatus.CurrencyDateField = null;
		this.clStatus.CurrencyField = null;
		this.clStatus.CurrencyFixed = null;
		this.clStatus.CurrencyFixedRate = 0.0;
		this.clStatus.CurrencyRateField = null;
		this.clStatus.FieldName = "Status";
		this.clStatus.Name = "clStatus";
		this.clStatus.PrimaryKey = false;
		this.clStatus.UnitField = null;
		this.clStatus.Visible = true;
		this.clStatus.VisibleIndex = 2;
		this.clStatus.Width = 69;
		this.replokUp_Status.AutoHeight = false;
		this.replokUp_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.replokUp_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.replokUp_Status.DisplayMember = "Name";
		this.replokUp_Status.Name = "replokUp_Status";
		this.replokUp_Status.ValueMember = "ID";
		this.clvalidFrom.Caption = "Active From";
		this.clvalidFrom.CurrencyDateField = null;
		this.clvalidFrom.CurrencyField = null;
		this.clvalidFrom.CurrencyFixed = null;
		this.clvalidFrom.CurrencyFixedRate = 0.0;
		this.clvalidFrom.CurrencyRateField = null;
		this.clvalidFrom.FieldName = "validFrom";
		this.clvalidFrom.Name = "clvalidFrom";
		this.clvalidFrom.PrimaryKey = false;
		this.clvalidFrom.UnitField = null;
		this.clvalidFrom.Visible = true;
		this.clvalidFrom.VisibleIndex = 3;
		this.clvalidFrom.Width = 89;
		this.clvalidTo.Caption = "Active To";
		this.clvalidTo.CurrencyDateField = null;
		this.clvalidTo.CurrencyField = null;
		this.clvalidTo.CurrencyFixed = null;
		this.clvalidTo.CurrencyFixedRate = 0.0;
		this.clvalidTo.CurrencyRateField = null;
		this.clvalidTo.FieldName = "validTo";
		this.clvalidTo.Name = "clvalidTo";
		this.clvalidTo.PrimaryKey = false;
		this.clvalidTo.UnitField = null;
		this.clvalidTo.Visible = true;
		this.clvalidTo.VisibleIndex = 4;
		this.clvalidTo.Width = 90;
		this.clType.Caption = "Type";
		this.clType.ColumnEdit = this.replokUpType;
		this.clType.CurrencyDateField = null;
		this.clType.CurrencyField = null;
		this.clType.CurrencyFixed = null;
		this.clType.CurrencyFixedRate = 0.0;
		this.clType.CurrencyRateField = null;
		this.clType.FieldName = "Type";
		this.clType.Name = "clType";
		this.clType.PrimaryKey = false;
		this.clType.UnitField = null;
		this.clType.Visible = true;
		this.clType.VisibleIndex = 8;
		this.replokUpType.AutoHeight = false;
		this.replokUpType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.replokUpType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.replokUpType.DisplayMember = "Name";
		this.replokUpType.Name = "replokUpType";
		this.replokUpType.ValueMember = "ID";
		this.clCardCode.Caption = "BP Code";
		this.clCardCode.ColumnEdit = this.rep_btnBPCode;
		this.clCardCode.CurrencyDateField = null;
		this.clCardCode.CurrencyField = null;
		this.clCardCode.CurrencyFixed = null;
		this.clCardCode.CurrencyFixedRate = 0.0;
		this.clCardCode.CurrencyRateField = null;
		this.clCardCode.FieldName = "CardCode";
		this.clCardCode.Name = "clCardCode";
		this.clCardCode.PrimaryKey = false;
		this.clCardCode.UnitField = null;
		this.clCardCode.Visible = true;
		this.clCardCode.VisibleIndex = 5;
		this.clCardCode.Width = 104;
		this.rep_btnBPCode.AutoHeight = false;
		this.rep_btnBPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_btnBPCode.Name = "rep_btnBPCode";
		this.rep_btnBPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnBPCode_ButtonClick);
		this.clCardName.Caption = "BP Name";
		this.clCardName.CurrencyDateField = null;
		this.clCardName.CurrencyField = null;
		this.clCardName.CurrencyFixed = null;
		this.clCardName.CurrencyFixedRate = 0.0;
		this.clCardName.CurrencyRateField = null;
		this.clCardName.FieldName = "CardName";
		this.clCardName.Name = "clCardName";
		this.clCardName.PrimaryKey = false;
		this.clCardName.UnitField = null;
		this.clCardName.Visible = true;
		this.clCardName.VisibleIndex = 6;
		this.clCardName.Width = 144;
		this.clLotSize.Caption = "Lot Size";
		this.clLotSize.CurrencyDateField = null;
		this.clLotSize.CurrencyField = null;
		this.clLotSize.CurrencyFixed = null;
		this.clLotSize.CurrencyFixedRate = 0.0;
		this.clLotSize.CurrencyRateField = null;
		this.clLotSize.FieldName = "LotSize";
		this.clLotSize.Name = "clLotSize";
		this.clLotSize.PrimaryKey = false;
		this.clLotSize.UnitField = null;
		this.clLotSize.Visible = true;
		this.clLotSize.VisibleIndex = 7;
		this.clRemark.Caption = "Remark";
		this.clRemark.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.clRemark.CurrencyDateField = null;
		this.clRemark.CurrencyField = null;
		this.clRemark.CurrencyFixed = null;
		this.clRemark.CurrencyFixedRate = 0.0;
		this.clRemark.CurrencyRateField = null;
		this.clRemark.FieldName = "Remark";
		this.clRemark.Name = "clRemark";
		this.clRemark.PrimaryKey = false;
		this.clRemark.UnitField = null;
		this.clRemark.Visible = true;
		this.clRemark.VisibleIndex = 9;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.clModelCode.Caption = "ModelCode";
		this.clModelCode.CurrencyDateField = null;
		this.clModelCode.CurrencyField = null;
		this.clModelCode.CurrencyFixed = null;
		this.clModelCode.CurrencyFixedRate = 0.0;
		this.clModelCode.CurrencyRateField = null;
		this.clModelCode.FieldName = "ModelCode";
		this.clModelCode.Name = "clModelCode";
		this.clModelCode.PrimaryKey = false;
		this.clModelCode.UnitField = null;
		this.repModelCode.AutoHeight = false;
		this.repModelCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repModelCode.Name = "repModelCode";
		this.repModelCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repModelCode_ButtonClick);
		this.grc_RoutingDetail.Controls.Add(this.grdDoc);
		this.grc_RoutingDetail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grc_RoutingDetail.Location = new System.Drawing.Point(0, 0);
		this.grc_RoutingDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grc_RoutingDetail.Name = "grc_RoutingDetail";
		this.grc_RoutingDetail.Size = new System.Drawing.Size(843, 141);
		this.grc_RoutingDetail.TabIndex = 0;
		this.grc_RoutingDetail.Text = "Routing Detail";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuDoc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDoc.Location = new System.Drawing.Point(2, 23);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.repControlKey, this.repResCode, this.repMechanicUOM, this.repOpCode, this.repSubOpCode, this.repPRT, this.repNextOp, this.repPrevOp, this.repUOM, this.repBOM,
			this.repositoryItemButtonEdit1, this.repositoryItemMemoEdit1, this.repUOM1
		});
		this.grdDoc.Size = new System.Drawing.Size(839, 116);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteRowToolStripMenuItem, this.insertRowToolStripMenuItem });
		this.contextMenuDoc.Name = "contextMenuStrip1";
		this.contextMenuDoc.Size = new System.Drawing.Size(165, 48);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.insertRowToolStripMenuItem.Text = "Insert Row Below";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[20]
		{
			this.colRouCode, this.LineNum, this.clOpCode, this.clResCode, this.clKeyCode, this.clQuantity, this.clUoM, this.clSubOpCode, this.clOpName, this.clSetuptime,
			this.clSetupUoM, this.clMachineTime, this.clMachineUoM, this.clLaborTime, this.clLaborUoM, this.clPRT, this.clBOMCode, this.clNextOp, this.clPrevOp, this.clComments
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDoc_RowStyle);
		this.gvDoc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEdit);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.colRouCode.Caption = "RouCode";
		this.colRouCode.CurrencyDateField = null;
		this.colRouCode.CurrencyField = null;
		this.colRouCode.CurrencyFixed = null;
		this.colRouCode.CurrencyFixedRate = 0.0;
		this.colRouCode.CurrencyRateField = null;
		this.colRouCode.FieldName = "RouCode";
		this.colRouCode.Name = "colRouCode";
		this.colRouCode.PrimaryKey = true;
		this.colRouCode.UnitField = null;
		this.LineNum.Caption = "LineNum";
		this.LineNum.CurrencyDateField = null;
		this.LineNum.CurrencyField = null;
		this.LineNum.CurrencyFixed = null;
		this.LineNum.CurrencyFixedRate = 0.0;
		this.LineNum.CurrencyRateField = null;
		this.LineNum.FieldName = "LineNum";
		this.LineNum.Name = "LineNum";
		this.LineNum.OptionsColumn.ReadOnly = true;
		this.LineNum.PrimaryKey = false;
		this.LineNum.UnitField = null;
		this.clOpCode.Caption = "Op Code";
		this.clOpCode.ColumnEdit = this.repOpCode;
		this.clOpCode.CurrencyDateField = null;
		this.clOpCode.CurrencyField = null;
		this.clOpCode.CurrencyFixed = null;
		this.clOpCode.CurrencyFixedRate = 0.0;
		this.clOpCode.CurrencyRateField = null;
		this.clOpCode.FieldName = "OpCode";
		this.clOpCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clOpCode.Name = "clOpCode";
		this.clOpCode.PrimaryKey = false;
		this.clOpCode.UnitField = null;
		this.clOpCode.Visible = true;
		this.clOpCode.VisibleIndex = 0;
		this.clOpCode.Width = 97;
		this.repOpCode.AutoHeight = false;
		this.repOpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOpCode.Name = "repOpCode";
		this.repOpCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOpCode_ButtonClick);
		this.clResCode.Caption = "Work Center";
		this.clResCode.ColumnEdit = this.repResCode;
		this.clResCode.CurrencyDateField = null;
		this.clResCode.CurrencyField = null;
		this.clResCode.CurrencyFixed = null;
		this.clResCode.CurrencyFixedRate = 0.0;
		this.clResCode.CurrencyRateField = null;
		this.clResCode.FieldName = "ResCode";
		this.clResCode.Name = "clResCode";
		this.clResCode.PrimaryKey = false;
		this.clResCode.UnitField = null;
		this.clResCode.Visible = true;
		this.clResCode.VisibleIndex = 3;
		this.clResCode.Width = 108;
		this.repResCode.AutoHeight = false;
		this.repResCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repResCode.Name = "repResCode";
		this.repResCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repResCode_ButtonClick);
		this.clKeyCode.Caption = "Ctrl Key";
		this.clKeyCode.CurrencyDateField = null;
		this.clKeyCode.CurrencyField = null;
		this.clKeyCode.CurrencyFixed = null;
		this.clKeyCode.CurrencyFixedRate = 0.0;
		this.clKeyCode.CurrencyRateField = null;
		this.clKeyCode.FieldName = "KeyCode";
		this.clKeyCode.Name = "clKeyCode";
		this.clKeyCode.PrimaryKey = false;
		this.clKeyCode.UnitField = null;
		this.clKeyCode.Visible = true;
		this.clKeyCode.VisibleIndex = 4;
		this.clKeyCode.Width = 78;
		this.clQuantity.Caption = "Base Qty";
		this.clQuantity.CurrencyDateField = null;
		this.clQuantity.CurrencyField = null;
		this.clQuantity.CurrencyFixed = null;
		this.clQuantity.CurrencyFixedRate = 0.0;
		this.clQuantity.CurrencyRateField = null;
		this.clQuantity.FieldName = "Quantity";
		this.clQuantity.Name = "clQuantity";
		this.clQuantity.PrimaryKey = false;
		this.clQuantity.UnitField = null;
		this.clQuantity.Visible = true;
		this.clQuantity.VisibleIndex = 5;
		this.clQuantity.Width = 92;
		this.clUoM.Caption = "Base UoM";
		this.clUoM.ColumnEdit = this.repUOM1;
		this.clUoM.CurrencyDateField = null;
		this.clUoM.CurrencyField = null;
		this.clUoM.CurrencyFixed = null;
		this.clUoM.CurrencyFixedRate = 0.0;
		this.clUoM.CurrencyRateField = null;
		this.clUoM.FieldName = "UoM";
		this.clUoM.Name = "clUoM";
		this.clUoM.PrimaryKey = false;
		this.clUoM.UnitField = null;
		this.clUoM.Visible = true;
		this.clUoM.VisibleIndex = 6;
		this.clUoM.Width = 72;
		this.repUOM1.AutoHeight = false;
		this.repUOM1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOM1.DisplayMember = "Name";
		this.repUOM1.Name = "repUOM1";
		this.repUOM1.ValueMember = "ID";
		this.clSubOpCode.Caption = "SubOpCode";
		this.clSubOpCode.ColumnEdit = this.repSubOpCode;
		this.clSubOpCode.CurrencyDateField = null;
		this.clSubOpCode.CurrencyField = null;
		this.clSubOpCode.CurrencyFixed = null;
		this.clSubOpCode.CurrencyFixedRate = 0.0;
		this.clSubOpCode.CurrencyRateField = null;
		this.clSubOpCode.FieldName = "SubOpCode";
		this.clSubOpCode.Name = "clSubOpCode";
		this.clSubOpCode.PrimaryKey = false;
		this.clSubOpCode.UnitField = null;
		this.clSubOpCode.Visible = true;
		this.clSubOpCode.VisibleIndex = 2;
		this.repSubOpCode.AutoHeight = false;
		this.repSubOpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repSubOpCode.Name = "repSubOpCode";
		this.repSubOpCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repSubOpCode_ButtonClick);
		this.clOpName.Caption = "Description";
		this.clOpName.CurrencyDateField = null;
		this.clOpName.CurrencyField = null;
		this.clOpName.CurrencyFixed = null;
		this.clOpName.CurrencyFixedRate = 0.0;
		this.clOpName.CurrencyRateField = null;
		this.clOpName.FieldName = "OpName";
		this.clOpName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.clOpName.Name = "clOpName";
		this.clOpName.OptionsColumn.ReadOnly = true;
		this.clOpName.PrimaryKey = false;
		this.clOpName.UnitField = null;
		this.clOpName.Visible = true;
		this.clOpName.VisibleIndex = 1;
		this.clOpName.Width = 182;
		this.clSetuptime.Caption = "Setup Time";
		this.clSetuptime.CurrencyDateField = null;
		this.clSetuptime.CurrencyField = null;
		this.clSetuptime.CurrencyFixed = null;
		this.clSetuptime.CurrencyFixedRate = 0.0;
		this.clSetuptime.CurrencyRateField = null;
		this.clSetuptime.FieldName = "SetupTime";
		this.clSetuptime.Name = "clSetuptime";
		this.clSetuptime.PrimaryKey = false;
		this.clSetuptime.UnitField = null;
		this.clSetuptime.Visible = true;
		this.clSetuptime.VisibleIndex = 7;
		this.clSetuptime.Width = 93;
		this.clSetupUoM.Caption = "Setup UoM";
		this.clSetupUoM.ColumnEdit = this.repUOM;
		this.clSetupUoM.CurrencyDateField = null;
		this.clSetupUoM.CurrencyField = null;
		this.clSetupUoM.CurrencyFixed = null;
		this.clSetupUoM.CurrencyFixedRate = 0.0;
		this.clSetupUoM.CurrencyRateField = null;
		this.clSetupUoM.FieldName = "SetupUoM";
		this.clSetupUoM.Name = "clSetupUoM";
		this.clSetupUoM.PrimaryKey = false;
		this.clSetupUoM.UnitField = null;
		this.clSetupUoM.Visible = true;
		this.clSetupUoM.VisibleIndex = 8;
		this.repUOM.AutoHeight = false;
		this.repUOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOM.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repUOM.DisplayMember = "Name";
		this.repUOM.Name = "repUOM";
		this.repUOM.NullText = "";
		this.repUOM.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.repUOM.ValueMember = "ID";
		this.clMachineTime.Caption = "Machine Time";
		this.clMachineTime.CurrencyDateField = null;
		this.clMachineTime.CurrencyField = null;
		this.clMachineTime.CurrencyFixed = null;
		this.clMachineTime.CurrencyFixedRate = 0.0;
		this.clMachineTime.CurrencyRateField = null;
		this.clMachineTime.FieldName = "MachineTime";
		this.clMachineTime.Name = "clMachineTime";
		this.clMachineTime.PrimaryKey = false;
		this.clMachineTime.UnitField = null;
		this.clMachineTime.Visible = true;
		this.clMachineTime.VisibleIndex = 9;
		this.clMachineUoM.Caption = "Machine UoM";
		this.clMachineUoM.ColumnEdit = this.repUOM;
		this.clMachineUoM.CurrencyDateField = null;
		this.clMachineUoM.CurrencyField = null;
		this.clMachineUoM.CurrencyFixed = null;
		this.clMachineUoM.CurrencyFixedRate = 0.0;
		this.clMachineUoM.CurrencyRateField = null;
		this.clMachineUoM.FieldName = "MachineUoM";
		this.clMachineUoM.Name = "clMachineUoM";
		this.clMachineUoM.PrimaryKey = false;
		this.clMachineUoM.UnitField = null;
		this.clMachineUoM.Visible = true;
		this.clMachineUoM.VisibleIndex = 10;
		this.clLaborTime.Caption = "Labor Time";
		this.clLaborTime.CurrencyDateField = null;
		this.clLaborTime.CurrencyField = null;
		this.clLaborTime.CurrencyFixed = null;
		this.clLaborTime.CurrencyFixedRate = 0.0;
		this.clLaborTime.CurrencyRateField = null;
		this.clLaborTime.FieldName = "LaborTime";
		this.clLaborTime.Name = "clLaborTime";
		this.clLaborTime.PrimaryKey = false;
		this.clLaborTime.UnitField = null;
		this.clLaborTime.Visible = true;
		this.clLaborTime.VisibleIndex = 11;
		this.clLaborUoM.Caption = "Labor UoM";
		this.clLaborUoM.ColumnEdit = this.repUOM;
		this.clLaborUoM.CurrencyDateField = null;
		this.clLaborUoM.CurrencyField = null;
		this.clLaborUoM.CurrencyFixed = null;
		this.clLaborUoM.CurrencyFixedRate = 0.0;
		this.clLaborUoM.CurrencyRateField = null;
		this.clLaborUoM.FieldName = "LaborUoM";
		this.clLaborUoM.Name = "clLaborUoM";
		this.clLaborUoM.PrimaryKey = false;
		this.clLaborUoM.UnitField = null;
		this.clLaborUoM.Visible = true;
		this.clLaborUoM.VisibleIndex = 12;
		this.clPRT.Caption = "PRT";
		this.clPRT.ColumnEdit = this.repPRT;
		this.clPRT.CurrencyDateField = null;
		this.clPRT.CurrencyField = null;
		this.clPRT.CurrencyFixed = null;
		this.clPRT.CurrencyFixedRate = 0.0;
		this.clPRT.CurrencyRateField = null;
		this.clPRT.FieldName = "PRT";
		this.clPRT.Name = "clPRT";
		this.clPRT.PrimaryKey = false;
		this.clPRT.UnitField = null;
		this.clPRT.Visible = true;
		this.clPRT.VisibleIndex = 13;
		this.repPRT.AutoHeight = false;
		this.repPRT.Name = "repPRT";
		this.repPRT.ValueChecked = "Y";
		this.repPRT.ValueUnchecked = "N";
		this.clBOMCode.Caption = "BOM";
		this.clBOMCode.ColumnEdit = this.repBOM;
		this.clBOMCode.CurrencyDateField = null;
		this.clBOMCode.CurrencyField = null;
		this.clBOMCode.CurrencyFixed = null;
		this.clBOMCode.CurrencyFixedRate = 0.0;
		this.clBOMCode.CurrencyRateField = null;
		this.clBOMCode.FieldName = "BOMCode";
		this.clBOMCode.Name = "clBOMCode";
		this.clBOMCode.PrimaryKey = false;
		this.clBOMCode.UnitField = null;
		this.clBOMCode.Visible = true;
		this.clBOMCode.VisibleIndex = 14;
		this.repBOM.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.repBOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinLeft, "", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repBOM.Name = "repBOM";
		this.repBOM.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repBOM_ButtonClick);
		this.repBOM.Click += new System.EventHandler(this.repBOM_Click);
		this.clNextOp.Caption = "Next Op";
		this.clNextOp.ColumnEdit = this.repNextOp;
		this.clNextOp.CurrencyDateField = null;
		this.clNextOp.CurrencyField = null;
		this.clNextOp.CurrencyFixed = null;
		this.clNextOp.CurrencyFixedRate = 0.0;
		this.clNextOp.CurrencyRateField = null;
		this.clNextOp.FieldName = "NextOp";
		this.clNextOp.Name = "clNextOp";
		this.clNextOp.OptionsColumn.ReadOnly = true;
		this.clNextOp.PrimaryKey = false;
		this.clNextOp.UnitField = null;
		this.clNextOp.Visible = true;
		this.clNextOp.VisibleIndex = 15;
		this.repNextOp.AutoHeight = false;
		this.repNextOp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repNextOp.Name = "repNextOp";
		this.repNextOp.ReadOnly = true;
		this.repNextOp.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repNextOp_ButtonClick);
		this.clPrevOp.Caption = "PrevOp";
		this.clPrevOp.ColumnEdit = this.repPrevOp;
		this.clPrevOp.CurrencyDateField = null;
		this.clPrevOp.CurrencyField = null;
		this.clPrevOp.CurrencyFixed = null;
		this.clPrevOp.CurrencyFixedRate = 0.0;
		this.clPrevOp.CurrencyRateField = null;
		this.clPrevOp.FieldName = "PrevOp";
		this.clPrevOp.Name = "clPrevOp";
		this.clPrevOp.OptionsColumn.ReadOnly = true;
		this.clPrevOp.PrimaryKey = false;
		this.clPrevOp.UnitField = null;
		this.clPrevOp.Visible = true;
		this.clPrevOp.VisibleIndex = 16;
		this.repPrevOp.AutoHeight = false;
		this.repPrevOp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repPrevOp.Name = "repPrevOp";
		this.repPrevOp.ReadOnly = true;
		this.repPrevOp.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repPrevOp_ButtonClick);
		this.clComments.Caption = "Comment";
		this.clComments.ColumnEdit = this.repositoryItemMemoEdit1;
		this.clComments.CurrencyDateField = null;
		this.clComments.CurrencyField = null;
		this.clComments.CurrencyFixed = null;
		this.clComments.CurrencyFixedRate = 0.0;
		this.clComments.CurrencyRateField = null;
		this.clComments.FieldName = "Comments";
		this.clComments.Name = "clComments";
		this.clComments.PrimaryKey = false;
		this.clComments.UnitField = null;
		this.clComments.Visible = true;
		this.clComments.VisibleIndex = 17;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.repControlKey.AutoHeight = false;
		this.repControlKey.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repControlKey.Name = "repControlKey";
		this.repControlKey.ReadOnly = true;
		this.repControlKey.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repControlKey_ButtonClick);
		this.repControlKey.Click += new System.EventHandler(this.repControlKey_Click);
		this.repMechanicUOM.AutoHeight = false;
		this.repMechanicUOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repMechanicUOM.Items.AddRange(new object[3] { "M", "H", "D" });
		this.repMechanicUOM.Name = "repMechanicUOM";
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.lb_Type.EditControl = null;
		this.lb_Type.EditValue = "Type";
		this.lb_Type.FieldName = null;
		this.lb_Type.HideWhenEmpty = false;
		this.lb_Type.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Type.Location = new System.Drawing.Point(281, 32);
		this.lb_Type.Name = "lb_Type";
		this.lb_Type.ReadOnly = false;
		this.lb_Type.SettingInVisible = false;
		this.lb_Type.SettingReadonly = false;
		this.lb_Type.ShowUnderLine = true;
		this.lb_Type.Size = new System.Drawing.Size(114, 20);
		this.lb_Type.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Type.TabIndex = 55;
		this.lb_Type.Utext = "Type";
		this.lb_Status.EditControl = null;
		this.lb_Status.EditValue = "Status";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(281, 7);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(114, 20);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 54;
		this.lb_Status.Utext = "Status";
		this.btnRouName.EditControl = null;
		this.btnRouName.FieldName = null;
		this.btnRouName.IsSearchControl = false;
		this.btnRouName.Location = new System.Drawing.Point(122, 28);
		this.btnRouName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btnRouName.Name = "btnRouName";
		this.btnRouName.SettingInVisible = false;
		this.btnRouName.SettingReadonly = false;
		this.btnRouName.Size = new System.Drawing.Size(147, 20);
		this.btnRouName.TabIndex = 53;
		this.lb_RoutingName.EditControl = null;
		this.lb_RoutingName.EditValue = "Routing Name";
		this.lb_RoutingName.FieldName = null;
		this.lb_RoutingName.HideWhenEmpty = false;
		this.lb_RoutingName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RoutingName.Location = new System.Drawing.Point(7, 32);
		this.lb_RoutingName.Name = "lb_RoutingName";
		this.lb_RoutingName.ReadOnly = false;
		this.lb_RoutingName.SettingInVisible = false;
		this.lb_RoutingName.SettingReadonly = false;
		this.lb_RoutingName.ShowUnderLine = true;
		this.lb_RoutingName.Size = new System.Drawing.Size(117, 20);
		this.lb_RoutingName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RoutingName.TabIndex = 52;
		this.lb_RoutingName.Utext = "Routing Name";
		this.lb_RoutingCode.EditControl = null;
		this.lb_RoutingCode.EditValue = "Routing Code";
		this.lb_RoutingCode.FieldName = null;
		this.lb_RoutingCode.HideWhenEmpty = false;
		this.lb_RoutingCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RoutingCode.Location = new System.Drawing.Point(7, 7);
		this.lb_RoutingCode.Name = "lb_RoutingCode";
		this.lb_RoutingCode.ReadOnly = false;
		this.lb_RoutingCode.SettingInVisible = false;
		this.lb_RoutingCode.SettingReadonly = false;
		this.lb_RoutingCode.ShowUnderLine = true;
		this.lb_RoutingCode.Size = new System.Drawing.Size(117, 20);
		this.lb_RoutingCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RoutingCode.TabIndex = 51;
		this.lb_RoutingCode.Utext = "Routing Code";
		this.lokStatus.Location = new System.Drawing.Point(396, 3);
		this.lokStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lokStatus.Name = "lokStatus";
		this.lokStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lokStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lokStatus.Properties.DisplayMember = "Name";
		this.lokStatus.Properties.NullText = "";
		this.lokStatus.Properties.PopupSizeable = false;
		this.lokStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.lokStatus.Properties.ValueMember = "ID";
		this.lokStatus.Size = new System.Drawing.Size(147, 20);
		this.lokStatus.TabIndex = 56;
		this.lokType.Location = new System.Drawing.Point(396, 28);
		this.lokType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lokType.Name = "lokType";
		this.lokType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lokType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lokType.Properties.DisplayMember = "Name";
		this.lokType.Properties.NullText = "";
		this.lokType.Properties.PopupSizeable = false;
		this.lokType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.lokType.Properties.ValueMember = "ID";
		this.lokType.Size = new System.Drawing.Size(147, 20);
		this.lokType.TabIndex = 62;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(845, 395);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.btnRouCode);
		base.Controls.Add(this.btSearch);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.lb_Type);
		base.Controls.Add(this.lb_Status);
		base.Controls.Add(this.btnRouName);
		base.Controls.Add(this.lb_RoutingName);
		base.Controls.Add(this.lb_RoutingCode);
		base.Controls.Add(this.lokStatus);
		base.Controls.Add(this.lokType);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_Routing";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Routing";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Routing_FormClosing);
		base.Load += new System.EventHandler(this.frm_Routing_Load);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btnRouCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grc_General).EndInit();
		this.grc_General.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDocs).EndInit();
		this.contextMenuDocs.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.replokUp_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.replokUpType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnBPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repModelCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grc_RoutingDetail).EndInit();
		this.grc_RoutingDetail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repSubOpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repPRT).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repBOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repNextOp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repPrevOp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repControlKey).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repMechanicUOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnRouName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lokStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lokType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
