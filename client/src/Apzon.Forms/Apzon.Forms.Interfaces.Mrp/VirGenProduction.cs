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
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Interfaces.Mrp;

public class VirGenProduction : XtraFormEx
{
	protected string Controler = "Schedule";

	private DataTable _tableDocPo;

	private DataTable _tableLinePo;

	protected readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lbSchedule;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl3;

	private PanelControl panelControl4;

	private PanelControl panelControl5;

	private GridControlEx grd_Close;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private LabelLine lbInputType;

	protected ButtonEditEx txt_Code;

	protected XtraTabControlEx tabControl;

	protected GridControlEx grd_Plan;

	protected GridViewEx gv_Plan;

	protected GridControlEx grd_Release;

	protected GridViewEx gv_Release;

	protected GridControlEx grd_Report;

	protected GridViewEx gv_Report;

	protected GridControlEx grd_Cancel;

	protected SimpleButtonEx Create_PO;

	protected LookUpEditEx txtInput;

	protected XtraTabPageEx tab_Plan;

	protected XtraTabPageEx tab_Release;

	protected XtraTabPageEx tab_Report;

	protected XtraTabPageEx tab_Cancel;

	protected GridViewEx gv_Cancel;

	protected XtraTabPageEx tab_Closed;

	protected GridViewEx gv_Close;

	protected SimpleButtonEx btnRealese;

	protected SimpleButtonEx btnReport;

	protected SimpleButtonEx btnCancel;

	protected SimpleButtonEx btnClose;

	protected RepositoryItemLookUpEdit rWhs;

	protected RepositoryItemLookUpEdit rTran;

	protected RepositoryItemLookUpEdit rSeri;

	protected RepositoryItemLookUpEdit rBP;

	protected RepositoryItemButtonEdit rItemCode;

	protected RepositoryItemButtonEdit rShift;

	protected RepositoryItemButtonEdit rOriginCode;

	private RepositoryItemLookUpEdit rOriginType;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				_formMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public VirGenProduction()
	{
		InitializeComponent();
		OnLoadAsync += frm_GenPOFromSchedule_Load;
	}

	public async Task MODE_FIND()
	{
		refreshToolStripMenuItem_Click(null, null);
	}

	protected void EnableDisableControl(bool isEnable)
	{
		try
		{
			txt_Code.ReadOnly = !isEnable;
			btnCancel.Enabled = isEnable;
			btnClose.Enabled = isEnable;
			btnRealese.Enabled = isEnable;
			Create_PO.Enabled = isEnable;
			btnReport.Enabled = isEnable;
			gv_Cancel.ReadOnly = !isEnable;
			gv_Close.ReadOnly = !isEnable;
			gv_Plan.ReadOnly = !isEnable;
			gv_Release.ReadOnly = !isEnable;
			gv_Report.ReadOnly = !isEnable;
			contextMenuStrip.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDataPO(string itemCode, string version, string sourceType, int sourceId, int sourceLine)
	{
		try
		{
			return await _httpClient.ScheduleClient.GetDataProductionOrder(itemCode, version, sourceType, sourceId, sourceLine);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return null;
	}

	private async Task CreatePoInSap()
	{
		_ = 2;
		try
		{
			if (_tableDocPo == null || _tableDocPo.Rows.Count == 0 || _tableLinePo == null || _tableLinePo.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.ServiceErrorReloadForm, "");
				return;
			}
			DataTable dt = grd_Plan.DataSourceTable;
			if (dt != null && dt.Rows.Count > 0)
			{
				dt.BeginLoadData();
				int[] lstCheck = gv_Plan.GetSelectedRows();
				if (lstCheck == null || lstCheck.Length == 0)
				{
					WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				}
				else
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						return;
					}
					WinCommons._OpenProcessing();
					List<ProductionOder_Impl_OWOR> lstPo = new List<ProductionOder_Impl_OWOR>();
					List<DataColumn> udf = (from DataColumn t in dt.Columns
						where t.ColumnName.StartsWith("U_")
						select (t)).ToList();
					for (int i = 0; i < lstCheck.Length; i++)
					{
						int dataSourceIndex = gv_Plan.GetDataSourceRowIndex(lstCheck[i]);
						DataTable dataTable = await GetDataPO(Function.ToString(dt.Rows[dataSourceIndex]["ItemCode"]), Function.ToString(dt.Rows[dataSourceIndex]["BomVersion"]), Function.ToString(txtInput.EditValue), Function.ParseInt(dt.Rows[dataSourceIndex]["U_OriginCode"]), Function.ParseInt(dt.Rows[dataSourceIndex]["U_OriginLine"]));
						if (dataTable == null || dataTable.Rows.Count == 0)
						{
							continue;
						}
						ProductionOder_Impl_OWOR productionOder_Impl_OWOR = new ProductionOder_Impl_OWOR
						{
							DocEntry = 0,
							Series = Function.ParseInt(dt.Rows[dataSourceIndex]["Series"]),
							Type = "S",
							Status = "P",
							ItemCode = Function.ToString(dt.Rows[dataSourceIndex]["ItemCode"]),
							ItemName = Function.ToString(dt.Rows[dataSourceIndex]["ItemName"]),
							Warehouse = Function.ToString(dt.Rows[dataSourceIndex]["Whse"]),
							PlannedQty = Function.ParseDouble(dt.Rows[dataSourceIndex]["Quantity"]),
							Project = Function.ToString(dt.Rows[dataSourceIndex]["Project"]),
							PostDate = Function.ParseDateTimes(dt.Rows[dataSourceIndex]["PostDate"]),
							DueDate = Function.ParseDateTimes(dt.Rows[dataSourceIndex]["DueDate"]),
							StartDate = Function.ParseDateTimes(dt.Rows[dataSourceIndex]["StartDate"]),
							CreateDate = DateTime.Now.Date,
							Comments = Function.ToString(dt.Rows[dataSourceIndex]["Remark"]),
							CardCode = (dt.ExistsColumn("CardCode") ? Function.ToString(dt.Rows[dataSourceIndex]["CardCode"]) : null),
							UserFields = new List<UserFieldsImpl>()
						};
						if (Function.ParseInt(dt.Rows[dataSourceIndex]["DemandType"]) == 17)
						{
							productionOder_Impl_OWOR.OriginType = "S";
							productionOder_Impl_OWOR.OriginAbs = Function.ParseInt(dt.Rows[dataSourceIndex]["DemandEntry"]);
						}
						if (udf.Count > 0)
						{
							for (int num = 0; num < udf.Count; num++)
							{
								if (_tableDocPo.Columns.Contains(udf[num].ColumnName))
								{
									UserFieldsImpl userFieldsImpl = new UserFieldsImpl
									{
										Name = udf[num].ColumnName
									};
									if (udf[num].DataType == typeof(DateTime))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Date;
									}
									else if (udf[num].DataType == typeof(double) || udf[num].DataType == typeof(decimal))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Numeric;
										userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
									}
									else if (udf[num].DataType == typeof(int) || udf[num].DataType == typeof(short) || udf[num].DataType == typeof(long))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									}
									else
									{
										userFieldsImpl.Type = BoFieldTypes.db_Alpha;
									}
									userFieldsImpl.Value = dt.Rows[dataSourceIndex][udf[num]];
									productionOder_Impl_OWOR.UserFields.Add(userFieldsImpl);
								}
							}
						}
						dataTable.BeginLoadData();
						productionOder_Impl_OWOR.Line_WOR1 = new List<ProductionOder_LineImpl_WOR1>();
						List<DataColumn> list = (from DataColumn t in dataTable.Columns
							where t.ColumnName.StartsWith("U_")
							select (t)).ToList();
						for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
						{
							ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = new ProductionOder_LineImpl_WOR1
							{
								LineNum = num2,
								ItemType = Function.ToString(dataTable.Rows[num2]["Type"]),
								ItemCode = Function.ToString(dataTable.Rows[num2]["Code"]),
								BaseQty = Function.ParseDouble(dataTable.Rows[num2]["Quantity"]),
								IssueType = Function.ToString(dataTable.Rows[num2]["IssueMthd"]),
								LineText = Function.ToString(dataTable.Rows[num2]["LineText"]),
								OcrCode = Function.ToString(dataTable.Rows[num2]["OcrCode"]),
								OcrCode2 = Function.ToString(dataTable.Rows[num2]["OcrCode2"]),
								OcrCode3 = Function.ToString(dataTable.Rows[num2]["OcrCode3"]),
								OcrCode4 = Function.ToString(dataTable.Rows[num2]["OcrCode4"]),
								OcrCode5 = Function.ToString(dataTable.Rows[num2]["OcrCode5"]),
								UomCode = Function.ToString(dataTable.Rows[num2]["IssueMthd"]),
								wareHouse = Function.ToString(dataTable.Rows[num2]["Warehouse"]),
								WipActCode = Function.ToString(dataTable.Rows[num2]["WipActCode"]),
								StartDate = Function.ParseDateTime(dataTable.Rows[num2]["StartDate"]),
								EndDate = Function.ParseDateTime(dataTable.Rows[num2]["EndDate"])
							};
							productionOder_LineImpl_WOR.Project = (string.IsNullOrEmpty(Function.ToString(dt.Rows[dataSourceIndex]["Project"])) ? Function.ToString(dataTable.Rows[num2]["Project"]) : Function.ToString(dt.Rows[dataSourceIndex]["Project"]));
							productionOder_LineImpl_WOR.PlannedQty = productionOder_LineImpl_WOR.BaseQty * productionOder_Impl_OWOR.PlannedQty;
							productionOder_LineImpl_WOR.UserFields = new List<UserFieldsImpl>();
							if (list.Count > 0)
							{
								for (int num3 = 0; num3 < list.Count; num3++)
								{
									if (_tableLinePo.Columns.Contains(list[num3].ColumnName))
									{
										UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl
										{
											Name = list[num3].ColumnName
										};
										if (list[num3].DataType == typeof(DateTime))
										{
											userFieldsImpl2.Type = BoFieldTypes.db_Date;
										}
										else if (list[num3].DataType == typeof(double) || list[num3].DataType == typeof(decimal))
										{
											userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
											userFieldsImpl2.SubType = BoFldSubTypes.st_Quantity;
										}
										else if (list[num3].DataType == typeof(int) || list[num3].DataType == typeof(short) || list[num3].DataType == typeof(long))
										{
											userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
										}
										else
										{
											userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
										}
										userFieldsImpl2.Value = dataTable.Rows[num2][list[num3]];
										productionOder_LineImpl_WOR.UserFields.Add(userFieldsImpl2);
									}
								}
							}
							productionOder_Impl_OWOR.Line_WOR1.Add(productionOder_LineImpl_WOR);
						}
						dataTable.EndLoadData();
						lstPo.Add(productionOder_Impl_OWOR);
					}
					dt.EndLoadData();
					HttpResult httpResult = await _httpClient.ProductionOrders.SaveList(lstPo);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						await LoadData();
						WinCommons.Success(Enums.MessageCode.CreateSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task CreatePoNoSap()
	{
		string mess = "";
		try
		{
			if (_tableDocPo == null || _tableDocPo.Rows.Count == 0 || _tableLinePo == null || _tableLinePo.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.ServiceErrorReloadForm, "");
				return;
			}
			DataTable dt = grd_Plan.DataSourceTable;
			if (dt != null && dt.Rows.Count > 0)
			{
				DateTime serverDate = await _httpClient.Commons.GetServerDate();
				dt.BeginLoadData();
				int[] lstCheck = gv_Plan.GetSelectedRows();
				if (lstCheck == null || lstCheck.Length == 0)
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseItem, "");
				}
				else
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						return;
					}
					WinCommons._OpenProcessing();
					List<DataColumn> udf = (from DataColumn t in dt.Columns
						where t.ColumnName.StartsWith("U_")
						select (t)).ToList();
					for (int i = 0; i < lstCheck.Length; i++)
					{
						int dataSourceIndex = gv_Plan.GetDataSourceRowIndex(lstCheck[i]);
						DataTable dataTable = await GetDataPO(Function.ToString(dt.Rows[dataSourceIndex]["ItemCode"]), Function.ToString(dt.Rows[dataSourceIndex]["BomVersion"]), Function.ToString(txtInput.EditValue), Function.ParseInt(dt.Rows[dataSourceIndex]["U_OriginCode"]), Function.ParseInt(dt.Rows[dataSourceIndex]["U_OriginLine"]));
						if (dataTable == null || dataTable.Rows.Count == 0)
						{
							mess += string.Format("Data bill of material {0} could not null", Function.ToString(dt.Rows[dataSourceIndex]["ItemCode"]));
							continue;
						}
						DataTable dataTable2 = _tableDocPo.Clone();
						DataTable dataTable3 = _tableLinePo.Clone();
						DataRow dataRow = dataTable2.NewRow();
						dataRow["DocEntry"] = 0;
						dataRow["Series"] = Function.ParseInt(dt.Rows[dataSourceIndex]["Series"]);
						dataRow["Type"] = "S";
						dataRow["Status"] = "P";
						dataRow["PIndicator"] = "Default";
						dataRow["ItemCode"] = Function.ToString(dt.Rows[dataSourceIndex]["ItemCode"]);
						dataRow["Warehouse"] = Function.ToString(dt.Rows[dataSourceIndex]["Whse"]);
						dataRow["PlannedQty"] = Function.ParseDouble(dt.Rows[dataSourceIndex]["Quantity"]);
						dataRow["Project"] = Function.ToString(dt.Rows[dataSourceIndex]["Project"]);
						dataRow["DueDate"] = Function.ParseDateTimes(dt.Rows[dataSourceIndex]["DueDate"]);
						dataRow["PostDate"] = Function.ParseDateTimes(dt.Rows[dataSourceIndex]["PostDate"]);
						dataRow["CreateDate"] = serverDate;
						dataRow["Comments"] = Function.ToString(dt.Rows[dataSourceIndex]["Remark"]);
						if (Function.ParseInt(dt.Rows[dataSourceIndex]["DemandType"]) == 17)
						{
							dataRow["OriginType"] = "S";
							dataRow["OriginAbs"] = Function.ParseInt(dt.Rows[dataSourceIndex]["DemandEntry"]);
						}
						if (udf.Count > 0)
						{
							for (int num = 0; num < udf.Count; num++)
							{
								if (dataTable2.Columns.Contains(udf[num].ColumnName))
								{
									dataRow[udf[num].ColumnName] = dt.Rows[dataSourceIndex][udf[num].ColumnName];
								}
							}
						}
						dataTable2.Rows.Add(dataRow);
						List<DataColumn> list = (from DataColumn t in dataTable.Columns
							where t.ColumnName.StartsWith("U_")
							select (t)).ToList();
						dataTable3.BeginLoadData();
						for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
						{
							DataRow dataRow2 = dataTable3.NewRow();
							dataRow2["LineNum"] = num2;
							dataRow2["ItemType"] = Function.ToString(dataTable.Rows[num2]["Type"]);
							dataRow2["ItemCode"] = Function.ToString(dataTable.Rows[num2]["Code"]);
							dataRow2["BaseQty"] = Function.ParseDouble(dataTable.Rows[num2]["Quantity"]);
							dataRow2["IssueType"] = Function.ToString(dataTable.Rows[num2]["IssueMthd"]);
							dataRow2["LineText"] = Function.ToString(dataTable.Rows[num2]["LineText"]);
							dataRow2["OcrCode"] = Function.ToString(dataTable.Rows[num2]["OcrCode"]);
							dataRow2["OcrCode2"] = Function.ToString(dataTable.Rows[num2]["OcrCode2"]);
							dataRow2["OcrCode3"] = Function.ToString(dataTable.Rows[num2]["OcrCode3"]);
							dataRow2["OcrCode4"] = Function.ToString(dataTable.Rows[num2]["OcrCode4"]);
							dataRow2["OcrCode5"] = Function.ToString(dataTable.Rows[num2]["OcrCode5"]);
							dataRow2["Project"] = (string.IsNullOrEmpty(Function.ToString(dt.Rows[dataSourceIndex]["Project"])) ? Function.ToString(dataTable.Rows[num2]["Project"]) : Function.ToString(dt.Rows[dataSourceIndex]["Project"]));
							dataRow2["PlannedQty"] = Function.ParseDouble(dataTable.Rows[num2]["Quantity"]) * Function.ParseDouble(dt.Rows[dataSourceIndex]["Quantity"]);
							dataRow2["UomCode"] = Function.ToString(dataTable.Rows[num2]["IssueMthd"]);
							dataRow2["wareHouse"] = (GlobalData.IsMultiBranch ? Function.ToString(dt.Rows[dataSourceIndex]["Whse"]) : Function.ToString(dataTable.Rows[num2]["Warehouse"]));
							dataRow2["WipActCode"] = Function.ToString(dataTable.Rows[num2]["WipActCode"]);
							dataRow2["StartDate"] = Function.ParseDateTime(dataTable.Rows[num2]["StartDate"]);
							dataRow2["EndDate"] = Function.ParseDateTime(dataTable.Rows[num2]["EndDate"]);
							if (list.Count > 0)
							{
								for (int num3 = 0; num3 < list.Count; num3++)
								{
									if (dataTable3.Columns.Contains(list[num3].ColumnName))
									{
										dataRow2[list[num3].ColumnName] = dataTable.Rows[num2][list[num3].ColumnName];
									}
								}
							}
							dataTable3.Rows.Add(dataRow2);
						}
						dataTable3.EndLoadData();
						DataSet dataSet = new DataSet();
						dataSet.Tables.Add(dataTable2);
						dataSet.Tables.Add(dataTable3);
						string text = mess;
						mess = text + (await _httpClient.ProductionOrders.SaveDataSet(dataSet)).Message;
					}
					dt.EndLoadData();
					await LoadData();
					if (string.IsNullOrEmpty(mess))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_SaveError + mess);
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetTablePo()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Schedule");
			DataSet dataSet = await http.GetAsync("GetTablePO");
			if (dataSet.Tables.Count == 2 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[1] != null && dataSet.Tables[1].Rows.Count > 0)
			{
				_tableDocPo = dataSet.Tables[0].Copy();
				_tableLinePo = dataSet.Tables[1].Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task SetDataWhs()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rWhs;
			repositoryItemLookUpEdit.DataSource = await _httpClient.ScheduleClient.GetDataWhs(Function.ToString(txtInput.EditValue), Function.ParseInt(txt_Code.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ReportSap()
	{
		if (grd_Report.DataSourceTable == null)
		{
			return;
		}
		DataTable dt = grd_Report.DataSourceTable;
		if (dt == null || dt.Rows.Count <= 0)
		{
			return;
		}
		int[] lstCheck = gv_Report.GetSelectedRows();
		if (lstCheck != null && lstCheck.Length != 0)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReportCompletePo, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			string mess = "";
			List<DataColumn> udf = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			for (int i = 0; i < lstCheck.Length; i++)
			{
				int dataSourceRowIndex = gv_Report.GetDataSourceRowIndex(lstCheck[i]);
				DocumentsImpl documentsImpl = new DocumentsImpl
				{
					DocEntry = 0,
					DocNum = 0,
					BPLId = Function.ParseInt(dt.Rows[dataSourceRowIndex]["BplID"]),
					DocDate = DateTime.Now.Date,
					DocDueDate = DateTime.Now.Date,
					ObjType = 59,
					Comments = "Receipt from production",
					GroupNum = -1,
					JrnlMemo = "Receipt from production",
					DocStatus = "O",
					Series = Function.ParseInt(dt.Rows[dataSourceRowIndex]["Series"]),
					DocRate = 1.0,
					DocType = "I",
					Printed = "N",
					DocCur = GlobalData.MainCurrency,
					UpdateDate = DateTime.Now.Date,
					CreateDate = DateTime.Now.Date,
					VatDate = DateTime.Now.Date,
					ReqDate = DateTime.Now.Date,
					Lines = new List<Document_LinesImpl>()
				};
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl
				{
					BaseEntry = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONo"]),
					BaseLine = -1,
					TranType = Function.ToString(dt.Rows[dataSourceRowIndex]["TranType"]),
					Remark = Function.ToString(dt.Rows[dataSourceRowIndex]["Remark"]),
					BaseType = 202,
					Quantity = Function.ParseDouble(dt.Rows[dataSourceRowIndex]["ReportQty"]),
					WhsCode = Function.ToString(dt.Rows[dataSourceRowIndex]["Whse"]),
					UserFields = new List<UserFieldsImpl>()
				};
				if (udf.Count > 0)
				{
					for (int num = 0; num < udf.Count; num++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl
						{
							Name = udf[num].ColumnName
						};
						if (udf[num].DataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (udf[num].DataType == typeof(double) || udf[num].DataType == typeof(decimal))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
						}
						else if (udf[num].DataType == typeof(int) || udf[num].DataType == typeof(short) || udf[num].DataType == typeof(long))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Value = dt.Rows[dataSourceRowIndex][udf[num]];
						document_LinesImpl.UserFields.Add(userFieldsImpl);
					}
				}
				documentsImpl.Lines.Add(document_LinesImpl);
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
				string text = mess;
				mess = text + (await http.PostAsync("SaveDoc", documentsImpl)).Message;
			}
			dt.EndLoadData();
			await LoadData();
			if (string.IsNullOrEmpty(mess))
			{
				WinCommons.Success(Enums.MessageCode.ReportSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, mess);
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
		}
	}

	private async Task ReportNoSap()
	{
		if (grd_Report.DataSourceTable == null)
		{
			return;
		}
		DataTable dt = grd_Report.DataSourceTable;
		if (dt == null || dt.Rows.Count <= 0)
		{
			return;
		}
		if (!DocumentsHelper.Document.IsNotNull() || !DocumentsHelper.Lines.IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			return;
		}
		int[] lstCheck = gv_Report.GetSelectedRows();
		if (lstCheck != null && lstCheck.Length != 0)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReportCompletePo, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			string mess = "";
			List<DataColumn> udf = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			for (int i = 0; i < lstCheck.Length; i++)
			{
				int dataSourceRowIndex = gv_Report.GetDataSourceRowIndex(lstCheck[i]);
				DataSet dataSet = new DataSet();
				DataTable dataTable = DocumentsHelper.Document.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocDate"] = DateTime.Now.Date;
				dataRow["DocDueDate"] = DateTime.Now.Date;
				dataRow["BplID"] = Function.ParseInt(dt.Rows[dataSourceRowIndex]["BplId"]);
				dataRow["ObjType"] = 59;
				dataRow["Comments"] = "Receipt from production";
				dataRow["GroupNum"] = -1;
				dataRow["JrnlMemo"] = "Receipt from production";
				dataRow["DocStatus"] = "O";
				dataRow["Series"] = Function.ParseInt(dt.Rows[dataSourceRowIndex]["Series"]);
				dataRow["DocRate"] = 1;
				dataRow["DocType"] = "I";
				dataRow["Printed"] = "N";
				dataRow["DocCur"] = GlobalData.MainCurrency;
				dataRow["UpdateDate"] = DateTime.Now.Date;
				dataRow["CreateDate"] = DateTime.Now.Date;
				dataRow["VatDate"] = DateTime.Now.Date;
				dataRow["ReqDate"] = DateTime.Now.Date;
				dataRow["Comments"] = Function.ToString(dt.Rows[dataSourceRowIndex]["Remark"]);
				dataTable.Rows.Add(dataRow);
				dataSet.Tables.Add(dataTable);
				DataTable dataTable2 = DocumentsHelper.Lines.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["ItemCode"] = dt.Rows[dataSourceRowIndex]["ItemCode"];
				dataRow2["Dscription"] = dt.Rows[dataSourceRowIndex]["ItemName"];
				dataRow2["BaseEntry"] = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONo"]);
				dataRow2["BaseType"] = 202;
				dataRow2["BaseRef"] = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONum"]).ToString();
				dataRow2["BaseLine"] = -1;
				dataRow2["Currency"] = GlobalData.MainCurrency;
				dataRow2["Price"] = Function.ParseDouble(dt.Rows[dataSourceRowIndex]["Price"]);
				dataRow2["Quantity"] = Function.ParseDouble(dt.Rows[dataSourceRowIndex]["ReportQty"]);
				dataRow2["WhsCode"] = Function.ToString(dt.Rows[dataSourceRowIndex]["Whse"]);
				dataRow2["TranType"] = Function.ToString(dt.Rows[dataSourceRowIndex]["TranType"]);
				dataRow2["ShipDate"] = Function.ToString(dt.Rows[dataSourceRowIndex]["DueDate"]);
				dataRow2["DocDate"] = DateTime.Now.Date;
				if (udf.Count > 0)
				{
					for (int num = 0; num < udf.Count; num++)
					{
						if (dataTable.Columns.Contains(udf[num].ColumnName))
						{
							dataRow2[udf[num].ColumnName] = dt.Rows[dataSourceRowIndex][udf[num].ColumnName];
						}
					}
				}
				dataTable2.Rows.Add(dataRow2);
				dataSet.Tables.Add(dataTable2);
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
				mess = (await http.PostAsync("SaveDataset", dataSet)).Message;
			}
			dt.EndLoadData();
			await LoadData();
			if (string.IsNullOrEmpty(mess))
			{
				WinCommons.Success(Enums.MessageCode.ReportSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, mess);
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
		}
	}

	private async Task GetBranch()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await http.GetAsync("GetBranchList", new
			{
				userCode = GlobalData.UserName
			});
			rBP.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetSeries()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await http.GetAsync("GetSeries", new
			{
				ObjType = (tabControl.SelectedTabPage.Name.Equals(tab_Report.Name) ? 59 : 202)
			});
			rSeri.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		string type;
		GridControl grd;
		switch (tabControl.SelectedTabPage.Name)
		{
		case "tab_Plan":
			type = "P";
			grd = grd_Plan;
			break;
		case "tab_Release":
			type = "R";
			grd = grd_Release;
			break;
		case "tab_Report":
			type = "E";
			grd = grd_Report;
			break;
		case "tab_Cancel":
			type = "C";
			grd = grd_Cancel;
			break;
		default:
			type = "L";
			grd = grd_Close;
			break;
		}
		DataTable dataTable;
		using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controler))
		{
			dataTable = await helper.GetAsync("GetDataGenProduction", new
			{
				id = Function.ToString(txt_Code.EditValue),
				type = type,
				inputType = Function.ToString(txtInput.EditValue)
			});
		}
		grd.DataSource = dataTable;
		grd.RefreshDataSource();
		if (!dataTable.IsNotNull())
		{
			WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
		}
	}

	private async Task<DataSet> GetDataStructure()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, Controler);
		return await http.GetAsync("GetDataStructureGenProduction");
	}

	private void LoadColumnGrid(DataSet ds)
	{
		if (ds == null || ds.Tables.Count < 5)
		{
			return;
		}
		gv_Plan.CreateColumByStructure(ds.Tables[0]);
		gv_Release.CreateColumByStructure(ds.Tables[1]);
		gv_Report.CreateColumByStructure(ds.Tables[2]);
		gv_Close.CreateColumByStructure(ds.Tables[3]);
		gv_Cancel.CreateColumByStructure(ds.Tables[4]);
		GridColumn gridColumn = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
		if (gridColumn != null)
		{
			gridColumn.ColumnEdit = rOriginType;
			gridColumn.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn2 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
		if (gridColumn2 != null)
		{
			gridColumn2.ColumnEdit = rOriginType;
			gridColumn2.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn3 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
		if (gridColumn3 != null)
		{
			gridColumn3.ColumnEdit = rOriginType;
			gridColumn3.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn4 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
		if (gridColumn4 != null)
		{
			gridColumn4.ColumnEdit = rOriginType;
			gridColumn4.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn5 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
		if (gridColumn5 != null)
		{
			gridColumn5.ColumnEdit = rOriginType;
			gridColumn5.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn6 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
		if (gridColumn6 != null)
		{
			gridColumn6.ColumnEdit = rOriginCode;
			gridColumn6.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn7 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
		if (gridColumn7 != null)
		{
			gridColumn7.ColumnEdit = rOriginCode;
			gridColumn7.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn8 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
		if (gridColumn8 != null)
		{
			gridColumn8.ColumnEdit = rOriginCode;
			gridColumn8.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn9 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
		if (gridColumn9 != null)
		{
			gridColumn9.ColumnEdit = rOriginCode;
			gridColumn9.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn10 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
		if (gridColumn10 != null)
		{
			gridColumn10.ColumnEdit = rOriginCode;
			gridColumn10.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn11 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
		if (gridColumn11 != null)
		{
			gridColumn11.ColumnEdit = rItemCode;
			gridColumn11.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn12 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
		if (gridColumn12 != null)
		{
			gridColumn12.ColumnEdit = rItemCode;
			gridColumn12.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn13 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
		if (gridColumn13 != null)
		{
			gridColumn13.ColumnEdit = rItemCode;
			gridColumn13.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn14 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
		if (gridColumn14 != null)
		{
			gridColumn14.ColumnEdit = rItemCode;
			gridColumn14.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn15 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
		if (gridColumn15 != null)
		{
			gridColumn15.ColumnEdit = rItemCode;
			gridColumn15.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn16 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Shift"));
		if (gridColumn16 != null)
		{
			gridColumn16.ColumnEdit = rShift;
			gridColumn16.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn17 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Shift"));
		if (gridColumn17 != null)
		{
			gridColumn17.ColumnEdit = rShift;
			gridColumn17.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn18 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Shift"));
		if (gridColumn18 != null)
		{
			gridColumn18.ColumnEdit = rShift;
			gridColumn18.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn19 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Shift"));
		if (gridColumn19 != null)
		{
			gridColumn19.ColumnEdit = rShift;
			gridColumn19.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn20 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Shift"));
		if (gridColumn20 != null)
		{
			gridColumn20.ColumnEdit = rShift;
			gridColumn20.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn21 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
		if (gridColumn21 != null)
		{
			gridColumn21.ColumnEdit = rBP;
			gridColumn21.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn22 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
		if (gridColumn22 != null)
		{
			gridColumn22.ColumnEdit = rBP;
			gridColumn22.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn23 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
		if (gridColumn23 != null)
		{
			gridColumn23.ColumnEdit = rBP;
			gridColumn23.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn24 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
		if (gridColumn24 != null)
		{
			gridColumn24.ColumnEdit = rBP;
			gridColumn24.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn25 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
		if (gridColumn25 != null)
		{
			gridColumn25.ColumnEdit = rBP;
			gridColumn25.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn26 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
		if (gridColumn26 != null)
		{
			gridColumn26.ColumnEdit = rSeri;
			gridColumn26.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn27 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
		if (gridColumn27 != null)
		{
			gridColumn27.ColumnEdit = rSeri;
			gridColumn27.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn28 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
		if (gridColumn28 != null)
		{
			gridColumn28.ColumnEdit = rSeri;
			gridColumn28.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn29 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
		if (gridColumn29 != null)
		{
			gridColumn29.ColumnEdit = rSeri;
			gridColumn29.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn30 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
		if (gridColumn30 != null)
		{
			gridColumn30.ColumnEdit = rSeri;
			gridColumn30.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn31 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Whse"));
		if (gridColumn31 != null)
		{
			gridColumn31.ColumnEdit = rWhs;
			gridColumn31.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn32 = gv_Release.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Whse"));
		if (gridColumn32 != null)
		{
			gridColumn32.ColumnEdit = rWhs;
			gridColumn32.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn33 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Whse"));
		if (gridColumn33 != null)
		{
			gridColumn33.ColumnEdit = rWhs;
			gridColumn33.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn34 = gv_Close.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Whse"));
		if (gridColumn34 != null)
		{
			gridColumn34.ColumnEdit = rWhs;
			gridColumn34.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn35 = gv_Cancel.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Whse"));
		if (gridColumn35 != null)
		{
			gridColumn35.ColumnEdit = rWhs;
			gridColumn35.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn36 = gv_Report.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("TranType"));
		if (gridColumn36 != null)
		{
			gridColumn36.ColumnEdit = rTran;
			gridColumn36.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		}
		GridColumn gridColumn37 = gv_Plan.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Project"));
		if (gridColumn37 == null)
		{
			return;
		}
		RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit
		{
			AllowMouseWheel = false
		};
		repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs ev) =>
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Project, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRowView dataRowView = frm_ChooseFromList2.ChooseItem;
				gv_Plan.SetRowCellValue(gv_Plan.FocusedRowHandle, "Project", Function.ToString(dataRowView["PrjCode"]));
			}
		};
		gridColumn37.ColumnEdit = repositoryItemButtonEdit;
		gridColumn37.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
	}

	protected virtual void LoadDatasourceType()
	{
		List<Common.Table_Code> dataSource = new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Name = "Schedule",
				Code = "S"
			},
			new Common.Table_Code
			{
				Name = "Capacity",
				Code = "C"
			}
		};
		txtInput.Properties.DataSource = dataSource;
		txtInput.EditValue = "S";
	}

	private async Task frm_GenPOFromSchedule_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			await GetTablePo();
			await GetBranch();
			await GetSeries();
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Complete",
					Code = "C"
				},
				new Common.Table_Code
				{
					Name = "Reject",
					Code = "R"
				}
			};
			rTran.DataSource = dataSource;
			LoadDatasourceType();
			rOriginType.DataSource = WinFuntions.GetListOriginType();
			LoadColumnGrid(await GetDataStructure());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Plan_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	protected virtual void txt_Schedule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, false, Function.ToString(txt_Code.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					ScheduleStatus = "A",
					ScheduleType = "O",
					IsGenCapacity = (Function.ToString(txtInput.EditValue).Equals("S") ? "N" : "Y")
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txt_Code.EditValue = Function.ToString(frm_CFL2.ChooseItem["PSCode"]);
					tabControl.Focus();
				}
				return;
			}
			int num = Function.ParseInt(txt_Code.EditValue);
			if (num != 0)
			{
				if (Function.ToString(txtInput.EditValue).Equals("S"))
				{
					this.DrillDown(ApzObjectTypes.oSchedule, ApzObjectTypes.oSchedule, Function.ToString(num));
				}
				else
				{
					this.DrillDown(ApzObjectTypes.oCapacity, ApzObjectTypes.oCapacity, Function.ToString(num));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await GetSeries();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		FormMode = Enums.FormViewModeType.View;
	}

	private void gv_Release_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Report_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Cancel_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Close_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	protected async void txt_Code_Validating(object sender, CancelEventArgs e)
	{
		if (grd_Cancel == null)
		{
			return;
		}
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (sender != null && !string.IsNullOrEmpty(Function.ToString(txt_Code.EditValue)))
			{
				string text;
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, Controler))
				{
					text = await http.GetAsync("CheckInputData", new
					{
						type = Function.ToString(txtInput.EditValue),
						code = Function.ParseInt(txt_Code.EditValue),
						genType = 'O'
					});
				}
				if (!string.IsNullOrEmpty(text))
				{
					txt_Code.EditValue = null;
					grd_Cancel.DataSourceTable.Rows.Clear();
					grd_Close.DataSourceTable.Rows.Clear();
					grd_Plan.DataSourceTable.Rows.Clear();
					grd_Release.DataSourceTable.Rows.Clear();
					grd_Report.DataSourceTable.Rows.Clear();
					WinCommons.Error(Enums.MessageCode.None, text);
					FormMode = Enums.FormViewModeType.View;
					txt_Code.Focus();
					WinCommons._CloseProcessing();
					return;
				}
			}
			await SetDataWhs();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async void Create_PO_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (!GlobalData.LoginSap)
			{
				await CreatePoNoSap();
			}
			else
			{
				await CreatePoInSap();
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

	private async void btnRealese_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (grd_Release.DataSourceTable != null && grd_Release.DataSource is DataTable dt && dt.Rows.Count > 0)
			{
				dt.BeginLoadData();
				int[] lstCheck = gv_Release.GetSelectedRows();
				if (lstCheck != null && lstCheck.Length != 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReleaseProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						return;
					}
					WinCommons._OpenProcessing();
					FormMode = Enums.FormViewModeType.Busy;
					string mess = "";
					for (int i = 0; i < lstCheck.Length; i++)
					{
						int dataSourceRowIndex = gv_Release.GetDataSourceRowIndex(lstCheck[i]);
						ProductionOder_Impl_OWOR pr = new ProductionOder_Impl_OWOR
						{
							DocEntry = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONo"]),
							Status = "R"
						};
						using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
						mess = await helper.PostParamsAsJsonAsync("UpdateStatus", pr);
					}
					dt.EndLoadData();
					await LoadData();
					if (string.IsNullOrEmpty(mess))
					{
						WinCommons.Success(Enums.MessageCode.ReleaseSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, mess);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
				}
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

	private async void btnReport_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (!GlobalData.LoginSap)
			{
				await ReportNoSap();
			}
			else
			{
				await ReportSap();
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

	private async void btnCancel_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (grd_Cancel.DataSourceTable != null)
			{
				DataTable dt = grd_Cancel.DataSourceTable;
				if (dt != null && dt.Rows.Count > 0)
				{
					dt.BeginLoadData();
					int[] lstCheck = gv_Cancel.GetSelectedRows();
					if (lstCheck != null && lstCheck.Length != 0)
					{
						if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelSelectProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
						{
							return;
						}
						WinCommons._OpenProcessing();
						FormMode = Enums.FormViewModeType.Busy;
						string mess = "";
						for (int i = 0; i < lstCheck.Length; i++)
						{
							int dataSourceRowIndex = gv_Cancel.GetDataSourceRowIndex(lstCheck[i]);
							ProductionOder_Impl_OWOR pr = new ProductionOder_Impl_OWOR
							{
								DocEntry = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONo"]),
								Status = "C"
							};
							using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Schedule");
							mess = await helper.PostParamsAsJsonAsync("UpdateStatus", pr);
						}
						dt.EndLoadData();
						await LoadData();
						if (string.IsNullOrEmpty(mess))
						{
							WinCommons.Success(Enums.MessageCode.CancelSuccess, "");
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, mess);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
					}
				}
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

	private async void btnClose_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (grd_Close.DataSourceTable != null)
			{
				DataTable dt = grd_Close.DataSourceTable;
				if (dt != null && dt.Rows.Count > 0)
				{
					dt.BeginLoadData();
					int[] lstCheck = gv_Close.GetSelectedRows();
					if (lstCheck != null && lstCheck.Length != 0)
					{
						if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseProductionOrder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
						{
							return;
						}
						WinCommons._OpenProcessing();
						FormMode = Enums.FormViewModeType.View;
						string mess = "";
						for (int i = 0; i < lstCheck.Length; i++)
						{
							int dataSourceRowIndex = gv_Close.GetDataSourceRowIndex(lstCheck[i]);
							ProductionOder_Impl_OWOR pr = new ProductionOder_Impl_OWOR
							{
								DocEntry = Function.ParseInt(dt.Rows[dataSourceRowIndex]["PONo"]),
								Status = "L"
							};
							using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
							mess = await helper.PostParamsAsJsonAsync("UpdateStatus", pr);
						}
						dt.EndLoadData();
						await LoadData();
						if (string.IsNullOrEmpty(mess))
						{
							await LoadData();
							WinCommons.Success(Enums.MessageCode.CloseSuccess, "");
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, mess);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
					}
				}
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

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async void txtInput_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void tabControl_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Busy)
		{
			e.Cancel = true;
		}
	}

	private void rWhs_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				string text = tabControl.SelectedTabPage.Name switch
				{
					"tab_Plan" => Function.ToString(gv_Plan.GetFocusedRowCellValue("Whse")), 
					"tab_Release" => Function.ToString(gv_Release.GetFocusedRowCellValue("Whse")), 
					"tab_Report" => Function.ToString(gv_Report.GetFocusedRowCellValue("Whse")), 
					"tab_Cancel" => Function.ToString(gv_Cancel.GetFocusedRowCellValue("Whse")), 
					_ => Function.ToString(gv_Close.GetFocusedRowCellValue("Whse")), 
				};
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

	private void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = tabControl.SelectedTabPage.Name switch
			{
				"tab_Plan" => Function.ToString(gv_Plan.GetFocusedRowCellValue("ItemCode")), 
				"tab_Release" => Function.ToString(gv_Release.GetFocusedRowCellValue("ItemCode")), 
				"tab_Report" => Function.ToString(gv_Report.GetFocusedRowCellValue("ItemCode")), 
				"tab_Cancel" => Function.ToString(gv_Cancel.GetFocusedRowCellValue("ItemCode")), 
				_ => Function.ToString(gv_Close.GetFocusedRowCellValue("ItemCode")), 
			};
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rShift_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = tabControl.SelectedTabPage.Name switch
			{
				"tab_Plan" => Function.ToString(gv_Plan.GetFocusedRowCellValue("Shift")), 
				"tab_Release" => Function.ToString(gv_Release.GetFocusedRowCellValue("Shift")), 
				"tab_Report" => Function.ToString(gv_Report.GetFocusedRowCellValue("Shift")), 
				"tab_Cancel" => Function.ToString(gv_Cancel.GetFocusedRowCellValue("Shift")), 
				_ => Function.ToString(gv_Close.GetFocusedRowCellValue("Shift")), 
			};
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oShift, ApzObjectTypes.oShift, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rSchedule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			GridView gridView;
			if (tabControl.SelectedTabPage.Name.Equals(tab_Plan.Name))
			{
				gridView = gv_Plan;
			}
			else if (tabControl.SelectedTabPage.Name.Equals(tab_Release.Name))
			{
				gridView = gv_Release;
			}
			else if (tabControl.SelectedTabPage.Name.Equals(tab_Report.Name))
			{
				gridView = gv_Report;
			}
			else
			{
				gridView = ((!tabControl.SelectedTabPage.Name.Equals(tab_Closed.Name)) ? gv_Cancel : gv_Close);
			}
			this.DrillDownOrigin(Function.ToString(gridView.GetFocusedRowCellValue("U_OriginType")), Function.ToString(gridView.GetFocusedRowCellValue("U_OriginCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.Mrp.VirGenProduction));
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
		this.lbSchedule = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.ButtonEditEx();
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Plan = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.Create_PO = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Plan = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Plan = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Release = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btnRealese = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Release = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Release = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btnReport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Report = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Report = new Apzon.Controls.Grid.GridViewEx();
		this.rWhs = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rTran = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rSeri = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rBP = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rShift = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rOriginCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rOriginType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_Cancel = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Cancel = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Cancel = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Closed = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.btnClose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Close = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Close = new Apzon.Controls.Grid.GridViewEx();
		this.lbInputType = new Apzon.Controls.Controls.LabelLine();
		this.txtInput = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.tab_Plan.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Plan).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Plan).BeginInit();
		this.tab_Release.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Release).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).BeginInit();
		this.tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTran).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSeri).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rShift).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).BeginInit();
		this.tab_Cancel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Cancel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Cancel).BeginInit();
		this.tab_Closed.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Close).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Close).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInput.Properties).BeginInit();
		base.SuspendLayout();
		this.lbSchedule.EditControl = null;
		this.lbSchedule.EditValue = "Schedule Code";
		this.lbSchedule.FieldName = "lbScheCode";
		this.lbSchedule.HideWhenEmpty = false;
		this.lbSchedule.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSchedule.Location = new System.Drawing.Point(209, 8);
		this.lbSchedule.Name = "lbSchedule";
		this.lbSchedule.ReadOnly = false;
		this.lbSchedule.SettingInVisible = false;
		this.lbSchedule.SettingReadonly = false;
		this.lbSchedule.ShowUnderLine = true;
		this.lbSchedule.Size = new System.Drawing.Size(80, 20);
		this.lbSchedule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSchedule.TabIndex = 0;
		this.lbSchedule.Utext = "Schedule Code";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "ScheCode";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(293, 6);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Code.Properties.AppearanceFocused.Options.UseBackColor = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_Code.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(115, 20);
		this.txt_Code.TabIndex = 2;
		this.txt_Code.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Schedule_ButtonClick);
		this.txt_Code.Validating += new System.ComponentModel.CancelEventHandler(this.txt_Code_Validating);
		this.tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(-4, 32);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.tab_Plan;
		this.tabControl.Size = new System.Drawing.Size(556, 470);
		this.tabControl.TabIndex = 0;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tab_Plan, this.tab_Release, this.tab_Report, this.tab_Cancel, this.tab_Closed });
		this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_SelectedPageChanged);
		this.tabControl.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.tabControl_SelectedPageChanging);
		this.tab_Plan.Controls.Add(this.panelControl1);
		this.tab_Plan.EditControl = null;
		this.tab_Plan.EditValue = "Plan";
		this.tab_Plan.FieldName = null;
		this.tab_Plan.IsSearchControl = false;
		this.tab_Plan.Name = "tab_Plan";
		this.tab_Plan.ReadOnly = false;
		this.tab_Plan.SettingInVisible = false;
		this.tab_Plan.SettingReadonly = false;
		this.tab_Plan.Size = new System.Drawing.Size(554, 445);
		this.tab_Plan.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Plan.Text = "Plan";
		this.panelControl1.Controls.Add(this.Create_PO);
		this.panelControl1.Controls.Add(this.grd_Plan);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(554, 445);
		this.panelControl1.TabIndex = 0;
		this.Create_PO.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.Create_PO.EditControl = null;
		this.Create_PO.EditValue = "Create Production Order";
		this.Create_PO.FieldName = "btnSave";
		this.Create_PO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("Create_PO.ImageOptions.Image");
		this.Create_PO.IsSearchControl = false;
		this.Create_PO.Location = new System.Drawing.Point(10, 419);
		this.Create_PO.Name = "Create_PO";
		this.Create_PO.ReadOnly = false;
		this.Create_PO.SettingInVisible = false;
		this.Create_PO.SettingReadonly = false;
		this.Create_PO.Size = new System.Drawing.Size(163, 20);
		this.Create_PO.TabIndex = 3;
		this.Create_PO.Text = "Create Production Order";
		this.Create_PO.Click += new System.EventHandler(this.Create_PO_Click);
		this.grd_Plan.AllowPasteExcel = true;
		this.grd_Plan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Plan.ClipboardData = null;
		this.grd_Plan.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Plan.Location = new System.Drawing.Point(-1, 0);
		this.grd_Plan.MainView = this.gv_Plan;
		this.grd_Plan.Name = "grd_Plan";
		this.grd_Plan.Size = new System.Drawing.Size(560, 412);
		this.grd_Plan.TabIndex = 0;
		this.grd_Plan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Plan });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.refreshToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(114, 26);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gv_Plan.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
		this.gv_Plan.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gv_Plan.Appearance.FocusedRow.BackColor = System.Drawing.Color.OldLace;
		this.gv_Plan.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Plan.Appearance.SelectedRow.BackColor = System.Drawing.Color.PapayaWhip;
		this.gv_Plan.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Plan.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Plan.ColumnKey = null;
		this.gv_Plan.DisableFormSetting = false;
		this.gv_Plan.DisplayException = false;
		this.gv_Plan.EditColumn = null;
		this.gv_Plan.FieldCurrency = null;
		this.gv_Plan.FieldRate = null;
		this.gv_Plan.FormId = "oMrpGenDocument";
		this.gv_Plan.GridControl = this.grd_Plan;
		this.gv_Plan.IgnoreFilter = false;
		this.gv_Plan.IndexEditRow = -1;
		this.gv_Plan.IndicatorWidth = 40;
		this.gv_Plan.IsApzTable = false;
		this.gv_Plan.IsInsertNewRow = false;
		this.gv_Plan.IsLoadingData = false;
		this.gv_Plan.IsMappingContextAction = false;
		this.gv_Plan.ItemId = "OPLAN";
		this.gv_Plan.LstColumnAllowBatch = null;
		this.gv_Plan.LstColumnNotAllowBatch = null;
		this.gv_Plan.LstColumnNotEditable = null;
		this.gv_Plan.LstColumNotSetting = null;
		this.gv_Plan.Name = "gv_Plan";
		this.gv_Plan.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Plan.OptionsSelection.MultiSelect = true;
		this.gv_Plan.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Plan.OptionsView.ColumnAutoWidth = false;
		this.gv_Plan.OptionsView.ShowGroupPanel = false;
		this.gv_Plan.UseBatchMod = false;
		this.gv_Plan.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Plan_CustomDrawRowIndicator);
		this.tab_Release.Controls.Add(this.panelControl2);
		this.tab_Release.EditControl = null;
		this.tab_Release.EditValue = "Release";
		this.tab_Release.FieldName = null;
		this.tab_Release.IsSearchControl = false;
		this.tab_Release.Name = "tab_Release";
		this.tab_Release.ReadOnly = false;
		this.tab_Release.SettingInVisible = false;
		this.tab_Release.SettingReadonly = false;
		this.tab_Release.Size = new System.Drawing.Size(554, 445);
		this.tab_Release.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Release.Text = "Release";
		this.panelControl2.Controls.Add(this.btnRealese);
		this.panelControl2.Controls.Add(this.grd_Release);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(554, 445);
		this.panelControl2.TabIndex = 1;
		this.btnRealese.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnRealese.EditControl = null;
		this.btnRealese.EditValue = "Release";
		this.btnRealese.FieldName = null;
		this.btnRealese.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRealese.ImageOptions.Image");
		this.btnRealese.IsSearchControl = false;
		this.btnRealese.Location = new System.Drawing.Point(12, 419);
		this.btnRealese.Name = "btnRealese";
		this.btnRealese.ReadOnly = false;
		this.btnRealese.SettingInVisible = false;
		this.btnRealese.SettingReadonly = false;
		this.btnRealese.Size = new System.Drawing.Size(75, 20);
		this.btnRealese.TabIndex = 4;
		this.btnRealese.Text = "Release";
		this.btnRealese.Click += new System.EventHandler(this.btnRealese_Click);
		this.grd_Release.AllowPasteExcel = false;
		this.grd_Release.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Release.ClipboardData = null;
		this.grd_Release.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Release.Location = new System.Drawing.Point(0, 0);
		this.grd_Release.MainView = this.gv_Release;
		this.grd_Release.Name = "grd_Release";
		this.grd_Release.Size = new System.Drawing.Size(554, 412);
		this.grd_Release.TabIndex = 2;
		this.grd_Release.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Release });
		this.gv_Release.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Release.ColumnKey = null;
		this.gv_Release.DisableFormSetting = false;
		this.gv_Release.DisplayException = false;
		this.gv_Release.EditColumn = null;
		this.gv_Release.FieldCurrency = null;
		this.gv_Release.FieldRate = null;
		this.gv_Release.FormId = "oMrpGenDocument";
		this.gv_Release.GridControl = this.grd_Release;
		this.gv_Release.IgnoreFilter = false;
		this.gv_Release.IndexEditRow = -1;
		this.gv_Release.IndicatorWidth = 40;
		this.gv_Release.IsApzTable = false;
		this.gv_Release.IsInsertNewRow = false;
		this.gv_Release.IsLoadingData = false;
		this.gv_Release.IsMappingContextAction = false;
		this.gv_Release.ItemId = "ORELEASE";
		this.gv_Release.LstColumnAllowBatch = null;
		this.gv_Release.LstColumnNotAllowBatch = null;
		this.gv_Release.LstColumnNotEditable = null;
		this.gv_Release.LstColumNotSetting = null;
		this.gv_Release.Name = "gv_Release";
		this.gv_Release.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Release.OptionsSelection.MultiSelect = true;
		this.gv_Release.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Release.OptionsView.ColumnAutoWidth = false;
		this.gv_Release.OptionsView.ShowGroupPanel = false;
		this.gv_Release.UseBatchMod = false;
		this.gv_Release.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Release_CustomDrawRowIndicator);
		this.tab_Report.Controls.Add(this.panelControl3);
		this.tab_Report.EditControl = null;
		this.tab_Report.EditValue = "Report";
		this.tab_Report.FieldName = null;
		this.tab_Report.IsSearchControl = false;
		this.tab_Report.Name = "tab_Report";
		this.tab_Report.ReadOnly = false;
		this.tab_Report.SettingInVisible = false;
		this.tab_Report.SettingReadonly = false;
		this.tab_Report.Size = new System.Drawing.Size(554, 445);
		this.tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Report.Text = "Report";
		this.panelControl3.Controls.Add(this.btnReport);
		this.panelControl3.Controls.Add(this.grd_Report);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(554, 445);
		this.panelControl3.TabIndex = 1;
		this.btnReport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnReport.EditControl = null;
		this.btnReport.EditValue = "Report";
		this.btnReport.FieldName = null;
		this.btnReport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnReport.ImageOptions.Image");
		this.btnReport.IsSearchControl = false;
		this.btnReport.Location = new System.Drawing.Point(14, 419);
		this.btnReport.Name = "btnReport";
		this.btnReport.ReadOnly = false;
		this.btnReport.SettingInVisible = false;
		this.btnReport.SettingReadonly = false;
		this.btnReport.Size = new System.Drawing.Size(75, 20);
		this.btnReport.TabIndex = 5;
		this.btnReport.Text = "Report";
		this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
		this.grd_Report.AllowPasteExcel = false;
		this.grd_Report.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Report.ClipboardData = null;
		this.grd_Report.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Report.Location = new System.Drawing.Point(0, 0);
		this.grd_Report.MainView = this.gv_Report;
		this.grd_Report.Name = "grd_Report";
		this.grd_Report.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rWhs, this.rTran, this.rSeri, this.rBP, this.rItemCode, this.rShift, this.rOriginCode, this.rOriginType });
		this.grd_Report.Size = new System.Drawing.Size(554, 412);
		this.grd_Report.TabIndex = 4;
		this.grd_Report.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Report });
		this.gv_Report.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Report.ColumnKey = null;
		this.gv_Report.DisableFormSetting = false;
		this.gv_Report.DisplayException = false;
		this.gv_Report.EditColumn = null;
		this.gv_Report.FieldCurrency = null;
		this.gv_Report.FieldRate = null;
		this.gv_Report.FormId = "oMrpGenDocument";
		this.gv_Report.GridControl = this.grd_Report;
		this.gv_Report.IgnoreFilter = false;
		this.gv_Report.IndexEditRow = -1;
		this.gv_Report.IndicatorWidth = 40;
		this.gv_Report.IsApzTable = false;
		this.gv_Report.IsInsertNewRow = false;
		this.gv_Report.IsLoadingData = false;
		this.gv_Report.IsMappingContextAction = false;
		this.gv_Report.ItemId = "OREPORT";
		this.gv_Report.LstColumnAllowBatch = null;
		this.gv_Report.LstColumnNotAllowBatch = null;
		this.gv_Report.LstColumnNotEditable = null;
		this.gv_Report.LstColumNotSetting = null;
		this.gv_Report.Name = "gv_Report";
		this.gv_Report.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Report.OptionsSelection.MultiSelect = true;
		this.gv_Report.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Report.OptionsView.ColumnAutoWidth = false;
		this.gv_Report.OptionsView.ShowGroupPanel = false;
		this.gv_Report.UseBatchMod = false;
		this.gv_Report.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Report_CustomDrawRowIndicator);
		this.rWhs.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhs.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("WhsName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("WhsCode", "Name2")
		});
		this.rWhs.DisplayMember = "WhsCode";
		this.rWhs.Name = "rWhs";
		this.rWhs.NullText = "";
		this.rWhs.ShowFooter = false;
		this.rWhs.ShowHeader = false;
		this.rWhs.ValueMember = "WhsCode";
		this.rWhs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhs_ButtonClick);
		this.rTran.AutoHeight = false;
		this.rTran.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTran.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rTran.DisplayMember = "Name";
		this.rTran.Name = "rTran";
		this.rTran.NullText = "";
		this.rTran.ShowFooter = false;
		this.rTran.ShowHeader = false;
		this.rTran.ValueMember = "Code";
		this.rSeri.AutoHeight = false;
		this.rSeri.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rSeri.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rSeri.DisplayMember = "Name";
		this.rSeri.Name = "rSeri";
		this.rSeri.NullText = "";
		this.rSeri.ShowFooter = false;
		this.rSeri.ShowHeader = false;
		this.rSeri.ValueMember = "Value";
		this.rBP.AutoHeight = false;
		this.rBP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBP.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rBP.DisplayMember = "Name";
		this.rBP.Name = "rBP";
		this.rBP.NullText = "";
		this.rBP.ShowFooter = false;
		this.rBP.ShowHeader = false;
		this.rBP.ValueMember = "Value";
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.rShift.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rShift.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rShift.Name = "rShift";
		this.rShift.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rShift_ButtonClick);
		this.rOriginCode.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rOriginCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rOriginCode.Name = "rOriginCode";
		this.rOriginCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rSchedule_ButtonClick);
		this.rOriginType.AutoHeight = false;
		this.rOriginType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOriginType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.rOriginType.DisplayMember = "Name";
		this.rOriginType.Name = "rOriginType";
		this.rOriginType.NullText = "";
		this.rOriginType.ShowHeader = false;
		this.rOriginType.ValueMember = "Code";
		this.tab_Cancel.Controls.Add(this.panelControl4);
		this.tab_Cancel.EditControl = null;
		this.tab_Cancel.EditValue = "Cancel";
		this.tab_Cancel.FieldName = null;
		this.tab_Cancel.IsSearchControl = false;
		this.tab_Cancel.Name = "tab_Cancel";
		this.tab_Cancel.ReadOnly = false;
		this.tab_Cancel.SettingInVisible = false;
		this.tab_Cancel.SettingReadonly = false;
		this.tab_Cancel.Size = new System.Drawing.Size(554, 445);
		this.tab_Cancel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Cancel.Text = "Cancel";
		this.panelControl4.Controls.Add(this.btnCancel);
		this.panelControl4.Controls.Add(this.grd_Cancel);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(554, 445);
		this.panelControl4.TabIndex = 1;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(11, 419);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 6;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Cancel.AllowPasteExcel = false;
		this.grd_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Cancel.ClipboardData = null;
		this.grd_Cancel.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Cancel.Location = new System.Drawing.Point(0, 0);
		this.grd_Cancel.MainView = this.gv_Cancel;
		this.grd_Cancel.Name = "grd_Cancel";
		this.grd_Cancel.Size = new System.Drawing.Size(554, 412);
		this.grd_Cancel.TabIndex = 6;
		this.grd_Cancel.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Cancel });
		this.gv_Cancel.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Cancel.ColumnKey = null;
		this.gv_Cancel.DisableFormSetting = false;
		this.gv_Cancel.DisplayException = false;
		this.gv_Cancel.EditColumn = null;
		this.gv_Cancel.FieldCurrency = null;
		this.gv_Cancel.FieldRate = null;
		this.gv_Cancel.FormId = "oMrpGenDocument";
		this.gv_Cancel.GridControl = this.grd_Cancel;
		this.gv_Cancel.IgnoreFilter = false;
		this.gv_Cancel.IndexEditRow = -1;
		this.gv_Cancel.IndicatorWidth = 40;
		this.gv_Cancel.IsApzTable = false;
		this.gv_Cancel.IsInsertNewRow = false;
		this.gv_Cancel.IsLoadingData = false;
		this.gv_Cancel.IsMappingContextAction = false;
		this.gv_Cancel.ItemId = "OCANCEL";
		this.gv_Cancel.LstColumnAllowBatch = null;
		this.gv_Cancel.LstColumnNotAllowBatch = null;
		this.gv_Cancel.LstColumnNotEditable = null;
		this.gv_Cancel.LstColumNotSetting = null;
		this.gv_Cancel.Name = "gv_Cancel";
		this.gv_Cancel.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Cancel.OptionsSelection.MultiSelect = true;
		this.gv_Cancel.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Cancel.OptionsView.ColumnAutoWidth = false;
		this.gv_Cancel.OptionsView.ShowGroupPanel = false;
		this.gv_Cancel.UseBatchMod = false;
		this.gv_Cancel.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Cancel_CustomDrawRowIndicator);
		this.tab_Closed.Controls.Add(this.panelControl5);
		this.tab_Closed.EditControl = null;
		this.tab_Closed.EditValue = "Close";
		this.tab_Closed.FieldName = null;
		this.tab_Closed.IsSearchControl = false;
		this.tab_Closed.Name = "tab_Closed";
		this.tab_Closed.ReadOnly = false;
		this.tab_Closed.SettingInVisible = false;
		this.tab_Closed.SettingReadonly = false;
		this.tab_Closed.Size = new System.Drawing.Size(554, 445);
		this.tab_Closed.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Closed.Text = "Close";
		this.panelControl5.Controls.Add(this.btnClose);
		this.panelControl5.Controls.Add(this.grd_Close);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(554, 445);
		this.panelControl5.TabIndex = 1;
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnClose.EditControl = null;
		this.btnClose.EditValue = "Close";
		this.btnClose.FieldName = null;
		this.btnClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnClose.ImageOptions.Image");
		this.btnClose.IsSearchControl = false;
		this.btnClose.Location = new System.Drawing.Point(13, 419);
		this.btnClose.Name = "btnClose";
		this.btnClose.ReadOnly = false;
		this.btnClose.SettingInVisible = false;
		this.btnClose.SettingReadonly = false;
		this.btnClose.Size = new System.Drawing.Size(75, 20);
		this.btnClose.TabIndex = 7;
		this.btnClose.Text = "Close";
		this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
		this.grd_Close.AllowPasteExcel = false;
		this.grd_Close.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Close.ClipboardData = null;
		this.grd_Close.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Close.Location = new System.Drawing.Point(0, 0);
		this.grd_Close.MainView = this.gv_Close;
		this.grd_Close.Name = "grd_Close";
		this.grd_Close.Size = new System.Drawing.Size(554, 412);
		this.grd_Close.TabIndex = 8;
		this.grd_Close.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Close });
		this.gv_Close.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Close.ColumnKey = null;
		this.gv_Close.DisableFormSetting = false;
		this.gv_Close.DisplayException = false;
		this.gv_Close.EditColumn = null;
		this.gv_Close.FieldCurrency = null;
		this.gv_Close.FieldRate = null;
		this.gv_Close.FormId = "oMrpGenDocument";
		this.gv_Close.GridControl = this.grd_Close;
		this.gv_Close.IgnoreFilter = false;
		this.gv_Close.IndexEditRow = -1;
		this.gv_Close.IndicatorWidth = 40;
		this.gv_Close.IsApzTable = false;
		this.gv_Close.IsInsertNewRow = false;
		this.gv_Close.IsLoadingData = false;
		this.gv_Close.IsMappingContextAction = false;
		this.gv_Close.ItemId = "OCLOSE";
		this.gv_Close.LstColumnAllowBatch = null;
		this.gv_Close.LstColumnNotAllowBatch = null;
		this.gv_Close.LstColumnNotEditable = null;
		this.gv_Close.LstColumNotSetting = null;
		this.gv_Close.Name = "gv_Close";
		this.gv_Close.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Close.OptionsSelection.MultiSelect = true;
		this.gv_Close.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Close.OptionsView.ColumnAutoWidth = false;
		this.gv_Close.OptionsView.ShowGroupPanel = false;
		this.gv_Close.UseBatchMod = false;
		this.gv_Close.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Close_CustomDrawRowIndicator);
		this.lbInputType.EditControl = null;
		this.lbInputType.EditValue = "Input From";
		this.lbInputType.FieldName = "lbInputType";
		this.lbInputType.HideWhenEmpty = false;
		this.lbInputType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInputType.Location = new System.Drawing.Point(6, 8);
		this.lbInputType.Name = "lbInputType";
		this.lbInputType.ReadOnly = false;
		this.lbInputType.SettingInVisible = false;
		this.lbInputType.SettingReadonly = false;
		this.lbInputType.ShowUnderLine = true;
		this.lbInputType.Size = new System.Drawing.Size(80, 20);
		this.lbInputType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInputType.TabIndex = 0;
		this.lbInputType.Utext = "Input From";
		this.txtInput.EditControl = null;
		this.txtInput.FieldName = "InputType";
		this.txtInput.IsSearchControl = false;
		this.txtInput.Location = new System.Drawing.Point(86, 5);
		this.txtInput.Name = "txtInput";
		this.txtInput.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtInput.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtInput.Properties.DisplayMember = "Name";
		this.txtInput.Properties.NullText = "";
		this.txtInput.Properties.ShowFooter = false;
		this.txtInput.Properties.ShowHeader = false;
		this.txtInput.Properties.ValueMember = "Code";
		this.txtInput.SettingInVisible = false;
		this.txtInput.SettingReadonly = false;
		this.txtInput.Size = new System.Drawing.Size(115, 20);
		this.txtInput.TabIndex = 1;
		this.txtInput.EditValueChanged += new System.EventHandler(this.txtInput_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(548, 497);
		base.Controls.Add(this.lbInputType);
		base.Controls.Add(this.tabControl);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.lbSchedule);
		base.Controls.Add(this.txtInput);
		base.FormId = "oGenProduction";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirGenProduction";
		this.Text = "Production Order From Schedule";
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.tab_Plan.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Plan).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Plan).EndInit();
		this.tab_Release.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Release).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).EndInit();
		this.tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTran).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSeri).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rShift).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).EndInit();
		this.tab_Cancel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Cancel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Cancel).EndInit();
		this.tab_Closed.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Close).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Close).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInput.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
