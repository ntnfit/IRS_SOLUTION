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
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.Mrp;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using SAPbobsCOM;

namespace Apzon.Forms.Mrps;

public class frm_GenTranferRequestFromMrp : VirMrpGenDocument
{
	public frm_GenTranferRequestFromMrp()
	{
		InitComponents();
		SetItemUomType = Enums.ItemUomType.InventoryUom;
	}

	public void InitComponents()
	{
		Controller = "Schedule";
		Text = "Transfer From Mrp";
		FormMode = Enums.FormViewModeType.View;
		string formId = (gvDoc.FormId = "GenTranfer");
		FormId = formId;
		gvDoc.ItemId = "Lines";
	}

	protected override void LoadButtonSave()
	{
		try
		{
			base.LoadButtonSave();
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			dXPopupMenu.Items.Add(new DXMenuItem("Inventory Transfer", async (object s, EventArgs ev) =>
			{
				await Save(Enums.MrpGenDocumentType.Document);
			}));
			dXPopupMenu.Items.Add(new DXMenuItem("Transfer Request", async (object s, EventArgs ev) =>
			{
				await Save(Enums.MrpGenDocumentType.Request);
			}));
			btnSave.DropDownControl = dXPopupMenu;
			btnSave.Text = "Transfer Request";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task GetSeries()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
			DataTable dataSource = await http.GetAsync("GetSeriesOrderAndRequest", new
			{
				type = Function.ToString(67)
			});
			rSeri.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task LoadDataFromSchedule()
	{
		_ = 1;
		try
		{
			await base.LoadDataFromSchedule();
			if (Function.ParseInt(txtCode.EditValue) > 0)
			{
				using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
				DataTable dataSource = await http.GetAsync("GetDataInventoryTransferFormSchedule", new
				{
					input = Function.ToString(txtInput.EditValue),
					code = Function.ParseInt(txtCode.EditValue),
					user = GlobalData.UserCodeLogin
				});
				grdDoc.DataSource = dataSource;
				grdDoc.RefreshDataSource();
				grdDoc.DataSourceTable.AcceptChanges();
			}
			else
			{
				grdDoc.DataSource = null;
				grdDoc.RefreshDataSource();
				grdDoc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task Save(Enums.MrpGenDocumentType type = (Enums.MrpGenDocumentType)0)
	{
		_ = 2;
		try
		{
			if (type == 0)
			{
				type = Enums.MrpGenDocumentType.Request;
			}
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				if (XtraMessageBoxEx.Show((type == Enums.MrpGenDocumentType.Document) ? Enums.MessageCode.ConfirmCreateInventoryTransfer : Enums.MessageCode.ConfirmCreateTransferRequest, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				string text = ((!GlobalData.LoginSap) ? (await SaveDataTable(type)) : (await SaveDi(type)));
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				await LoadDataFromSchedule();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
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

	private async Task<string> SaveDi(Enums.MrpGenDocumentType type)
	{
		try
		{
			string mess = "";
			DocumentsImpl doc = new DocumentsImpl();
			int[] selectedRows = gvDoc.GetSelectedRows();
			DataTable dt = grdDoc.DataSourceTable.Clone();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				dt.ImportRow(gvDoc.GetDataRow(selectedRows[i]));
			}
			List<DataColumn> udf = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			doc.ObjType = ((type == Enums.MrpGenDocumentType.Document) ? 67 : 1250000001);
			doc.UpdateDate = DateTime.Now.Date;
			doc.UserSign = Function.ParseInt(GlobalData.UserSignLogin);
			for (int i2 = 0; i2 < dt.Rows.Count; i2++)
			{
				string value = Function.ToString(dt.Rows[i2]["CardCode"]);
				doc.CardCode = Function.ToString(dt.Rows[i2]["CardCode"]);
				doc.CardName = Function.ToString(dt.Rows[i2]["CardName"]);
				doc.DocStatus = "O";
				doc.DocDate = DateTime.Now;
				doc.TaxDate = DateTime.Now;
				doc.UserSign = GlobalData.UserID;
				doc.Series = Function.ParseInt(dt.Rows[i2]["Series"]);
				doc.OriginType = "R";
				doc.Lines = new List<Document_LinesImpl>();
				doc.DocRate = 1.0;
				DateTime dateTime = Function.ParseDateTimes(dt.Rows[i2]["ShipDate"]);
				int num = 0;
				for (int num2 = i2; num2 < dt.Rows.Count; num2++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num2]["FromWhsCod"])))
					{
						mess += "You must choose warehouse!";
					}
					else if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num2]["WhsCode"])))
					{
						mess += "You must choose To warehouse!";
					}
					else
					{
						if (!Function.ToString(dt.Rows[num2]["CardCode"]).Equals(value))
						{
							continue;
						}
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl
						{
							ItemCode = Function.ToString(dt.Rows[num2]["ItemCode"]),
							Dscription = Function.ToString(dt.Rows[num2]["Dscription"]),
							Quantity = Function.ParseDouble(dt.Rows[num2]["Quantity"]),
							Rate = 1.0,
							LineNum = num
						};
						num++;
						document_LinesImpl.WhsCode = Function.ToString(dt.Rows[num2]["WhsCode"]);
						document_LinesImpl.FromWhsCod = Function.ToString(dt.Rows[num2]["FromWhsCod"]);
						document_LinesImpl.UomCode = Function.ToString(dt.Rows[i2]["UomCode"]);
						document_LinesImpl.UoMName = Function.ToString(dt.Rows[i2]["unitMsr"]);
						document_LinesImpl.UseBaseUn = Function.ToString(dt.Rows[i2]["UseBaseUn"]);
						document_LinesImpl.UomEntry = Function.ParseInt(dt.Rows[i2]["UomEntry"]);
						document_LinesImpl.NumPerMsr = Function.ParseDouble(dt.Rows[i2]["NumPerMsr"]);
						document_LinesImpl.Project = Function.ToString(dt.Rows[num2]["Project"]);
						document_LinesImpl.UserFields = new List<UserFieldsImpl>();
						if (udf.Count > 0)
						{
							for (int num3 = 0; num3 < udf.Count; num3++)
							{
								if (DocumentsHelper.Lines.Columns.Contains(udf[num3].ColumnName))
								{
									UserFieldsImpl userFieldsImpl = new UserFieldsImpl
									{
										Name = udf[num3].ColumnName
									};
									if (udf[num3].DataType == typeof(DateTime))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Date;
									}
									else if (udf[num3].DataType == typeof(double) || udf[num3].DataType == typeof(decimal))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Numeric;
										userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
									}
									else if (udf[num3].DataType == typeof(int) || udf[num3].DataType == typeof(short) || udf[num3].DataType == typeof(long))
									{
										userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									}
									else
									{
										userFieldsImpl.Type = BoFieldTypes.db_Alpha;
									}
									userFieldsImpl.Value = dt.Rows[num2][udf[num3]];
									document_LinesImpl.UserFields.Add(userFieldsImpl);
								}
							}
						}
						if (Function.ParseDateTimes(dt.Rows[num2]["ShipDate"]) < dateTime)
						{
							dateTime = Function.ParseDateTimes(dt.Rows[num2]["ShipDate"]);
						}
						doc.Lines.Add(document_LinesImpl);
						dt.Rows.RemoveAt(num2);
						num2--;
					}
				}
				doc.DocDueDate = dateTime;
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, (Enums.MrpGenDocumentType.Document == type) ? "InventoryTranfer" : "TranferRequest");
				HttpResult httpResult = await http.PostAsync("SaveDoc", doc);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					mess = mess + " " + httpResult.Message;
				}
			}
			return mess;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task<string> SaveDataTable(Enums.MrpGenDocumentType type)
	{
		try
		{
			if (!DocumentsHelper.Document.IsNotNull() || !DocumentsHelper.Lines.IsNotNull())
			{
				return "Could not get structure";
			}
			string mess = "";
			int[] selectedRows = gvDoc.GetSelectedRows();
			DataTable dt = grdDoc.DataSourceTable.Clone();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				dt.ImportRow(gvDoc.GetDataRow(selectedRows[i]));
			}
			List<string> colum = (from DataColumn t in DocumentsHelper.Lines.Columns
				select t.ColumnName).ToList();
			List<string> scurceColum = (from DataColumn t in dt.Columns
				where colum.Contains(t.ColumnName)
				select t.ColumnName).ToList();
			for (int i2 = 0; i2 < dt.Rows.Count; i2++)
			{
				string text = Function.ToString(dt.Rows[i2]["CardCode"]);
				DataTable dataTable = DocumentsHelper.Document.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["DocNum"] = 0;
				dataRow["CardCode"] = text;
				dataRow["CardName"] = Function.ToString(dt.Rows[i2]["CardName"]);
				dataRow["BPLId"] = Function.ParseInt(dt.Rows[i2]["BplID"]);
				dataRow["DocStatus"] = "O";
				dataRow["DocDate"] = DateTime.Now;
				dataRow["TaxDate"] = DateTime.Now;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["Series"] = ((type == Enums.MrpGenDocumentType.Document) ? Function.ParseInt(dt.Rows[i2]["Series"]) : Function.ParseInt(dt.Rows[i2]["SeriesRequest"]));
				dataRow["DocRate"] = 1;
				dataRow["OriginType"] = "R";
				dataRow["Instance"] = 0;
				dataRow["Segment"] = 0;
				dataRow["PIndicator"] = "Default";
				dataRow["DocSubType"] = "--";
				DateTime dateTime = Function.ParseDateTimes(dt.Rows[i2]["ShipDate"]);
				int num = 0;
				DataTable dataTable2 = DocumentsHelper.Lines.Clone();
				for (int num2 = i2; num2 < dt.Rows.Count; num2++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num2]["FromWhsCod"])))
					{
						mess += "You must choose warehouse!";
					}
					else if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num2]["WhsCode"])))
					{
						mess += "You must choose To warehouse!";
					}
					else if (Function.ToString(dt.Rows[num2]["CardCode"]).Equals(text))
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2["DocEntry"] = 0;
						dataRow2["LineNum"] = num;
						dataRow2["Rate"] = 1;
						num++;
						for (int num3 = 0; num3 < scurceColum.Count; num3++)
						{
							dataRow2[scurceColum[num3]] = Function.ToString(dt.Rows[num2][scurceColum[num3]]);
						}
						if (string.IsNullOrEmpty(Function.ToString(dataRow2["Currency"])))
						{
							dataRow2["Currency"] = GlobalData.MainCurrency;
						}
						if (Function.ParseDateTimes(dt.Rows[num2]["ShipDate"]) < dateTime)
						{
							dateTime = Function.ParseDateTimes(dt.Rows[num2]["ShipDate"]);
						}
						dataTable2.Rows.Add(dataRow2);
						dt.Rows.RemoveAt(num2);
						num2--;
					}
				}
				dataRow["DocDueDate"] = dateTime;
				dataTable.Rows.Add(dataRow);
				if (dataTable.Rows.Count == 0 || dataTable2.Rows.Count == 0)
				{
					mess = mess + "Data line" + text + " could not be null!";
					continue;
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(dataTable2);
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, (type == Enums.MrpGenDocumentType.Document) ? "InventoryTranfer" : "TranferRequest");
				HttpResult httpResult = await http.PostAsync("SaveDataSet", dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					mess = mess + " " + httpResult.Message;
				}
			}
			return mess;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override void ChooseBussinessParter()
	{
		try
		{
			base.ChooseBussinessParter();
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vendor, false, null, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "CardCode", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "CardName", Function.ToString(frm_CFL2.ChooseItem["CardName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task<DataTable> GetStructureGrid()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return await http.GetAsync("GetStructureGenTransfer");
	}

	protected override void Code_ChooseFromList(object sender, ButtonPressedEventArgs e)
	{
		if (Function.ToString(txtInput.EditValue) == "S")
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					ScheduleStatus = "A",
					ScheduleType = "P;T"
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txtCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["PSCode"]);
				}
			}
			else
			{
				int num = Function.ParseInt(txtCode.EditValue);
				if (num != 0)
				{
					this.DrillDown(ApzObjectTypes.oSchedule, ApzObjectTypes.oSchedule, Function.ToString(num));
				}
			}
		}
		else if (e.Button.Index == 0)
		{
			frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.MRP, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MrpStatus = "A",
				MrpType = "4"
			};
			frm_CFL3.ShowDialog(this);
			if (frm_CFL3.ChooseItem != null)
			{
				txtCode.EditValue = Function.ToString(frm_CFL3.ChooseItem["MrpCode"]);
			}
		}
		else if (Function.ParseInt(txtCode.EditValue) != 0)
		{
			frm_MRP frm_MRP2 = new frm_MRP(Function.ParseInt(txtCode.EditValue));
			frm_MRP2.MdiParent = MdiParent;
			frm_MRP2.Show();
		}
	}

	protected override async Task<string> GetDataCheckInput()
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, Controller);
		return await http.GetAsync("CheckInputData", new
		{
			type = Function.ToString(txtInput.EditValue),
			code = Function.ParseInt(txtCode.EditValue),
			genType = "T"
		});
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_GenTranferRequestFromMrp));
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtInput.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rBran).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rSeri).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rCarCode).BeginInit();
		base.SuspendLayout();
		base.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		base.txtInput.EditValue = "S";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(506, 404);
		base.Name = "frm_GenTranferRequestFromMrp";
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtInput.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rBran).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rSeri).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rCarCode).EndInit();
		base.ResumeLayout(false);
	}
}
