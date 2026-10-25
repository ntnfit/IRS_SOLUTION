using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using Apzon.Forms.Purchase;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using SAPbobsCOM;

namespace Apzon.Forms.Interfaces.Mrp;

public class VirGenPurchase : VirMrpGenDocument
{
	public VirGenPurchase()
	{
		SetItemUomType = Enums.ItemUomType.PurchaseUom;
	}

	protected override void LoadButtonSave()
	{
		try
		{
			base.LoadButtonSave();
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			dXPopupMenu.Items.Add(new DXMenuItem("Purchase Order", async (object s, EventArgs ev) =>
			{
				await Save(Enums.MrpGenDocumentType.Document);
			}));
			dXPopupMenu.Items.Add(new DXMenuItem("Purchase Request", async (object s, EventArgs ev) =>
			{
				await Save(Enums.MrpGenDocumentType.Request);
			}));
			btnSave.DropDownControl = dXPopupMenu;
			btnSave.Text = (Function.ToString(txtInput.EditValue).Equals("S") ? "Purchase Order" : "Purchase Request");
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
				type = Function.ToString(22)
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
				DataTable dataSource = await http.GetAsync("GetDataGenPurchase", new
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
		_ = 3;
		try
		{
			if (type == 0)
			{
				type = (btnSave.Text.Equals("Purchase Order") ? Enums.MrpGenDocumentType.Document : Enums.MrpGenDocumentType.Request);
			}
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				if (XtraMessageBoxEx.Show((type == Enums.MrpGenDocumentType.Document) ? Enums.MessageCode.ConfirmCreatePurchaseOrder : Enums.MessageCode.ConfirmCreatePurchaseRepuest, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				string text;
				if (GlobalData.LoginSap)
				{
					if (Function.ToString(GlobalData.AdditionalGeneralSettings.Tables[0].Rows[0]["CheckOpenFormPR"]).Equals("Y") && type == Enums.MrpGenDocumentType.Request)
					{
						await SaveRequest();
						gvDoc.DeleteSelectedRows();
						FormMode = Enums.FormViewModeType.View;
						return;
					}
					text = await SaveDi(type);
				}
				else
				{
					text = await SaveDataTable(type);
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await LoadDataFromSchedule();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
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
		_ = 3;
		try
		{
			string mess = "";
			int[] selectedRows = gvDoc.GetSelectedRows();
			DataTable dt = grdDoc.DataSourceTable.Clone();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				dt.ImportRow(gvDoc.GetDataRow(selectedRows[i]));
			}
			DateTime serverDate = await HttpClientHelperFactory.GetHttpClient().Commons.GetServerDate();
			List<DataColumn> udf = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			if (type == Enums.MrpGenDocumentType.Document)
			{
				List<DocumentsImpl> lstDocument = new List<DocumentsImpl>();
				int i2;
				for (i2 = 0; i2 < dt.Rows.Count; i2++)
				{
					string text = Function.ToString(dt.Rows[i2]["CardCode"]);
					if (string.IsNullOrEmpty(text))
					{
						return "You must choose cardcode!";
					}
					DocumentsImpl doc = new DocumentsImpl
					{
						CardCode = text,
						CardName = Function.ToString(dt.Rows[i2]["CardName"]),
						BPLId = Function.ParseInt(dt.Rows[i2]["BplID"]),
						DocStatus = "O",
						DocDate = Function.ParseDateTimes(dt.Rows[i2]["DocDate"]),
						TaxDate = serverDate,
						Series = Function.ParseInt(dt.Rows[i2]["Series"]),
						Lines = new List<Document_LinesImpl>(),
						DocRate = 1.0,
						OriginType = "R"
					};
					DateTime dateTime = Function.ParseDateTimes(dt.Rows[i2]["ShipDate"]);
					int num = 0;
					for (int num2 = i2; num2 < dt.Rows.Count; num2++)
					{
						if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num2]["WhsCode"])))
						{
							return "You must choose warehouse!";
						}
						if (!Function.ToString(dt.Rows[num2]["CardCode"]).Equals(text))
						{
							continue;
						}
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl
						{
							ItemCode = Function.ToString(dt.Rows[num2]["ItemCode"]),
							Dscription = Function.ToString(dt.Rows[num2]["Dscription"]),
							Quantity = Function.ParseDouble(dt.Rows[num2]["Quantity"]),
							Price = Function.ParseDouble(dt.Rows[num2]["Price"]),
							Rate = 1.0,
							LineNum = num,
							Currency = Function.ToString(dt.Rows[num2]["Currency"]),
							PriceBefDi = Function.ParseDouble(dt.Rows[num2]["PriceBefDi"]),
							WhsCode = Function.ToString(dt.Rows[num2]["WhsCode"]),
							UomCode = Function.ToString(dt.Rows[num2]["UomCode"]),
							UoMName = Function.ToString(dt.Rows[num2]["unitMsr"]),
							UseBaseUn = Function.ToString(dt.Rows[num2]["UseBaseUn"]),
							UomEntry = Function.ParseInt(dt.Rows[num2]["UomEntry"]),
							NumPerMsr = Function.ParseDouble(dt.Rows[num2]["NumPerMsr"]),
							ShipDate = Function.ParseDateTimes(dt.Rows[num2]["ShipDate"]),
							VatGroup = Function.ToString(dt.Rows[num2]["VatGroup"]),
							TaxPercentagePerRow = Function.ParseDouble(dt.Rows[num2]["VatPrcnt"]),
							Project = Function.ToString(dt.Rows[num2]["Project"])
						};
						if (string.IsNullOrEmpty(document_LinesImpl.Currency))
						{
							document_LinesImpl.Currency = GlobalData.MainCurrency;
						}
						num++;
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
						if (document_LinesImpl.ShipDate < dateTime)
						{
							dateTime = document_LinesImpl.ShipDate;
						}
						doc.Lines.Add(document_LinesImpl);
						dt.Rows.RemoveAt(num2);
						num2--;
					}
					i2--;
					doc.DocDueDate = dateTime;
					if (!(await doc.GetApprovalTemplate(new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseOder"), "3", this)))
					{
						return "Cannot get approval template";
					}
					lstDocument.Add(doc);
				}
				using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Commons");
				HttpResult httpResult = httpClientHelper.Post("AddListDocument", new Tuple<List<DocumentsImpl>, ApzObjectTypes>(lstDocument, ApzObjectTypes.oPurchaseOrders));
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					mess = mess + " " + httpResult.Message;
				}
			}
			else
			{
				DocumentsImpl doc = new DocumentsImpl();
				int num4 = -1;
				doc.ReqType = 12;
				doc.Requester = GlobalData.UserCodeLogin;
				doc.RequesterName = GlobalData.UserNameLogin;
				doc.DocStatus = "O";
				doc.DocDate = Function.ParseDateTimes(dt.Rows[0]["DocDate"]);
				doc.TaxDate = serverDate;
				doc.Series = Function.ParseInt(dt.Rows[0]["SeriesRequest"]);
				doc.OriginType = "R";
				doc.DocRate = 1.0;
				DateTime dateTime2 = Function.ParseDateTimes(dt.Rows[0]["ShipDate"]);
				doc.Lines = new List<Document_LinesImpl>();
				int num5 = 0;
				for (int num6 = 0; num6 < dt.Rows.Count; num6++)
				{
					string text2 = Function.ToString(dt.Rows[num6]["CardCode"]);
					if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num6]["WhsCode"])))
					{
						return "You must choose warehouse!";
					}
					if (num4 == -1)
					{
						num4 = Function.ParseInt(dt.Rows[num6]["BplID"]);
					}
					Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl
					{
						ItemCode = Function.ToString(dt.Rows[num6]["ItemCode"]),
						Dscription = Function.ToString(dt.Rows[num6]["Dscription"]),
						Rate = 1.0,
						Quantity = Function.ParseDouble(dt.Rows[num6]["Quantity"]),
						Price = Function.ParseDouble(dt.Rows[num6]["Price"]),
						VendorNum = text2,
						LineVendor = text2,
						Currency = Function.ToString(dt.Rows[num6]["Currency"]),
						PriceBefDi = Function.ParseDouble(dt.Rows[num6]["PriceBefDi"]),
						WhsCode = Function.ToString(dt.Rows[num6]["WhsCode"]),
						UomCode = Function.ToString(dt.Rows[num6]["UomCode"]),
						UoMName = Function.ToString(dt.Rows[num6]["unitMsr"]),
						UseBaseUn = Function.ToString(dt.Rows[num6]["UseBaseUn"]),
						UomEntry = Function.ParseInt(dt.Rows[num6]["UomEntry"]),
						NumPerMsr = Function.ParseDouble(dt.Rows[num6]["NumPerMsr"]),
						LineNum = num5,
						VatGroup = Function.ToString(dt.Rows[num6]["VatGroup"]),
						TaxPercentagePerRow = Function.ParseDouble(dt.Rows[num6]["VatPrcnt"]),
						Project = Function.ToString(dt.Rows[num6]["Project"]),
						UserFields = new List<UserFieldsImpl>()
					};
					if (string.IsNullOrEmpty(document_LinesImpl2.Currency))
					{
						document_LinesImpl2.Currency = GlobalData.MainCurrency;
					}
					if (udf.Count > 0)
					{
						for (int num7 = 0; num7 < udf.Count; num7++)
						{
							if (DocumentsHelper.Lines.Columns.Contains(udf[num7].ColumnName))
							{
								UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl
								{
									Name = udf[num7].ColumnName
								};
								if (udf[num7].DataType == typeof(DateTime))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Date;
								}
								else if (udf[num7].DataType == typeof(double) || udf[num7].DataType == typeof(decimal))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl2.SubType = BoFldSubTypes.st_Quantity;
								}
								else if (udf[num7].DataType == typeof(int) || udf[num7].DataType == typeof(short) || udf[num7].DataType == typeof(long))
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl2.Value = dt.Rows[num6][udf[num7]];
								document_LinesImpl2.UserFields.Add(userFieldsImpl2);
							}
						}
					}
					num5++;
					document_LinesImpl2.ShipDate = Function.ParseDateTimes(dt.Rows[num6]["ShipDate"]);
					document_LinesImpl2.PQTReqDate = Function.ParseDateTimes(dt.Rows[num6]["ShipDate"]);
					if (document_LinesImpl2.ShipDate < dateTime2)
					{
						dateTime2 = document_LinesImpl2.ShipDate;
					}
					doc.Lines.Add(document_LinesImpl2);
				}
				doc.DocDueDate = dateTime2;
				doc.ReqDate = dateTime2;
				doc.BPLId = num4;
				if (!(await doc.GetApprovalTemplate(new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseRequest"))))
				{
					return "Cannot get approval template";
				}
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PurchaseRequest");
				mess = (await http.PostAsync("SaveDoc", doc)).Message;
			}
			return mess;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task SaveRequest()
	{
		try
		{
			int[] selectedRows = gvDoc.GetSelectedRows();
			DataTable dt = grdDoc.DataSourceTable.Clone();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				dt.ImportRow(gvDoc.GetDataRow(selectedRows[i]));
			}
			DateTime dateTime = await HttpClientHelperFactory.GetHttpClient().Commons.GetServerDate();
			List<DataColumn> list = (from DataColumn t in dt.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			DateTime dateTime2 = Function.ParseDateTimes(dt.Rows[0]["ShipDate"]);
			int num = -1;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ReqType", typeof(int));
			dataTable.Columns.Add("Requester", typeof(string));
			dataTable.Columns.Add("ReqName", typeof(string));
			dataTable.Columns.Add("DocStatus", typeof(string));
			dataTable.Columns.Add("DocDate", typeof(DateTime));
			dataTable.Columns.Add("TaxDate", typeof(DateTime));
			dataTable.Columns.Add("Series", typeof(string));
			dataTable.Columns.Add("OriginType", typeof(string));
			dataTable.Columns.Add("DocRate", typeof(int));
			dataTable.Columns.Add("DocDueDate", typeof(DateTime));
			dataTable.Columns.Add("ReqDate", typeof(DateTime));
			dataTable.Columns.Add("BPLId", typeof(int));
			dataTable.Rows.Add(12, GlobalData.UserCodeLogin, GlobalData.UserNameLogin, "O", Function.ParseDateTimes(dt.Rows[0]["DocDate"]), dateTime, Function.ParseInt(dt.Rows[0]["SeriesRequest"]), "R", 1, Function.ParseDateTimes(dt.Rows[0]["ShipDate"]), Function.ParseDateTimes(dt.Rows[0]["ShipDate"]), num);
			DataTable dtLine = new DataTable();
			dtLine.Columns.Add("ItemCode", typeof(string));
			dtLine.Columns.Add("Dscription", typeof(string));
			dtLine.Columns.Add("Rate", typeof(int));
			dtLine.Columns.Add("Quantity", typeof(double));
			dtLine.Columns.Add("Price", typeof(double));
			dtLine.Columns.Add("VendorNum", typeof(string));
			dtLine.Columns.Add("LineVendor", typeof(string));
			dtLine.Columns.Add("Currency", typeof(string));
			dtLine.Columns.Add("PriceBefDi", typeof(double));
			dtLine.Columns.Add("WhsCode", typeof(string));
			dtLine.Columns.Add("UomCode", typeof(string));
			dtLine.Columns.Add("UoMName", typeof(string));
			dtLine.Columns.Add("UseBaseUn", typeof(string));
			dtLine.Columns.Add("UomEntry", typeof(int));
			dtLine.Columns.Add("NumPerMsr", typeof(double));
			dtLine.Columns.Add("LineNum", typeof(int));
			dtLine.Columns.Add("VatGroup", typeof(string));
			dtLine.Columns.Add("VatPrcnt", typeof(string));
			dtLine.Columns.Add("Project", typeof(string));
			dtLine.Columns.Add("ShipDate", typeof(DateTime));
			dtLine.Columns.Add("PQTReqDate", typeof(DateTime));
			int num2 = 0;
			for (int num3 = 0; num3 < dt.Rows.Count; num3++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dt.Rows[num3]["WhsCode"])))
				{
					WinCommons.Error(Enums.MessageCode.NoDocumentSelected, "");
					return;
				}
				if (num == -1)
				{
					num = Function.ParseInt(dt.Rows[num3]["BplID"]);
				}
				string text = Function.ToString(dt.Rows[num3]["CardCode"]);
				dtLine.Rows.Add(Function.ToString(dt.Rows[num3]["ItemCode"]), Function.ToString(dt.Rows[num3]["Dscription"]), 1, Function.ParseDouble(dt.Rows[num3]["Quantity"]), Function.ParseDouble(dt.Rows[num3]["Price"]), text, text, string.IsNullOrEmpty(Function.ToString(dt.Rows[num3]["Currency"])) ? GlobalData.MainCurrency : Function.ToString(dt.Rows[num3]["Currency"]), Function.ParseDouble(dt.Rows[num3]["PriceBefDi"]), Function.ToString(dt.Rows[num3]["WhsCode"]), Function.ToString(dt.Rows[num3]["UomCode"]), Function.ToString(dt.Rows[num3]["unitMsr"]), Function.ToString(dt.Rows[num3]["UseBaseUn"]), Function.ParseInt(dt.Rows[num3]["UomEntry"]), Function.ParseDouble(dt.Rows[num3]["NumPerMsr"]), num2, Function.ToString(dt.Rows[num3]["VatGroup"]), Function.ParseDouble(dt.Rows[num3]["VatPrcnt"]), Function.ToString(dt.Rows[num3]["Project"]), Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]), Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]));
				if (Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]) < dateTime2)
				{
					dateTime2 = Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]);
				}
				num2++;
			}
			if (list.Count > 0)
			{
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					dtLine.Columns.Add(list[num4].ColumnName, list[num4].DataType);
				}
				for (int num5 = 0; num5 < dtLine.Rows.Count; num5++)
				{
					int j;
					for (j = 0; j < dtLine.Columns.Count; j++)
					{
						if (list.AsEnumerable().Any((DataColumn c) => c.ColumnName == dtLine.Columns[j].ColumnName))
						{
							dtLine.Rows[num5][j] = dt.Rows[num5][dtLine.Columns[j].ColumnName];
						}
					}
				}
			}
			dataTable.Rows[0]["DocDueDate"] = dateTime2;
			dataTable.Rows[0]["ReqDate"] = dateTime2;
			List<ContextActionMappingImpl> list2 = new List<ContextActionMappingImpl>();
			for (int num6 = 0; num6 < dataTable.Columns.Count; num6++)
			{
				list2.Add(new ContextActionMappingImpl
				{
					TargetField = dataTable.Columns[num6].ColumnName,
					SourceField = dataTable.Columns[num6].ColumnName,
					DfltValue = dataTable.Columns[num6].DefaultValue
				});
			}
			ContextActionMappingImpl[] mappingDocument = list2.ToArray();
			List<ContextActionMappingImpl> list3 = new List<ContextActionMappingImpl>();
			for (int num7 = 0; num7 < dtLine.Columns.Count; num7++)
			{
				list3.Add(new ContextActionMappingImpl
				{
					TargetField = dtLine.Columns[num7].ColumnName,
					SourceField = dtLine.Columns[num7].ColumnName,
					DfltValue = dtLine.Columns[num7].DefaultValue
				});
			}
			ContextActionMappingImpl[] mappingLines = list3.ToArray();
			frm_PurchaseRequest frm_PurchaseRequest2 = new frm_PurchaseRequest(Function.ToString(txtInput.EditValue).Equals("S") ? ApzObjectTypes.oSchedule : ApzObjectTypes.oMrp, 0);
			frm_PurchaseRequest2.OpenFormViewMode = Enums.FormViewModeType.Add;
			frm_PurchaseRequest2.OpenFormDocument = dataTable;
			frm_PurchaseRequest2.OpenFormLines = dtLine;
			frm_PurchaseRequest2.MappingDocument = mappingDocument;
			frm_PurchaseRequest2.MappingLines = mappingLines;
			frm_PurchaseRequest2.MdiParent = MdiParent;
			frm_PurchaseRequest2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<string> SaveDataTable(Enums.MrpGenDocumentType type)
	{
		_ = 2;
		try
		{
			if (!DocumentsHelper.Document.IsNotNull() || !DocumentsHelper.Lines.IsNotNull())
			{
				return "Could not get structure";
			}
			grdDoc.DataSourceTable.AcceptChanges();
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
			DateTime serverDate = await HttpClientHelperFactory.GetHttpClient().Commons.GetServerDate();
			if (type == Enums.MrpGenDocumentType.Document)
			{
				for (int i2 = 0; i2 < dt.Rows.Count; i2++)
				{
					string text = Function.ToString(dt.Rows[i2]["CardCode"]);
					if (string.IsNullOrEmpty(text))
					{
						mess += " You must choose cardcode!";
						continue;
					}
					DataTable dataTable = DocumentsHelper.Document.Clone();
					DataRow dataRow = dataTable.NewRow();
					dataRow["DocEntry"] = 0;
					dataRow["DocNum"] = 0;
					dataRow["CardCode"] = text;
					dataRow["CardName"] = Function.ToString(dt.Rows[i2]["CardName"]);
					dataRow["BPLId"] = Function.ParseInt(dt.Rows[i2]["BplID"]);
					dataRow["DocStatus"] = "O";
					dataRow["DocDate"] = serverDate;
					dataRow["TaxDate"] = serverDate;
					dataRow["Series"] = Function.ParseInt(dt.Rows[i2]["Series"]);
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["DocCur"] = GlobalData.MainCurrency;
					dataRow["DocRate"] = 1;
					dataRow["Instance"] = 0;
					dataRow["OriginType"] = "R";
					dataRow["Segment"] = 0;
					dataRow["PIndicator"] = "Default";
					dataRow["DocSubType"] = "--";
					DateTime dateTime = Function.ParseDateTimes(dt.Rows[i2]["ShipDate"]);
					int num = 0;
					double num2 = 0.0;
					DataTable dataTable2 = DocumentsHelper.Lines.Clone();
					for (int num3 = i2; num3 < dt.Rows.Count; num3++)
					{
						if (Function.ToString(dt.Rows[num3]["CardCode"]).Equals(text))
						{
							DataRow dataRow2 = dataTable2.NewRow();
							dataRow2["DocEntry"] = 0;
							dataRow2["Rate"] = 1;
							dataRow2["LineNum"] = num;
							for (int num4 = 0; num4 < scurceColum.Count; num4++)
							{
								dataRow2[scurceColum[num4]] = Function.ToString(dt.Rows[num3][scurceColum[num4]]);
							}
							dataRow2["Price"] = Function.ParseDouble(dataRow2["PriceBefDi"]) * (1.0 - Function.ParseDouble(dataRow2["DiscPrcnt"]) / 100.0);
							dataRow2["LineTotal"] = Function.ParseDouble(dataRow2["Quantity"]) * Function.ParseDouble(dataRow2["Price"]);
							num2 += Function.ParseDouble(dataRow2["LineTotal"]);
							if (string.IsNullOrEmpty(Function.ToString(dataRow2["Currency"])))
							{
								dataRow2["Currency"] = GlobalData.MainCurrency;
							}
							if (Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]) < dateTime)
							{
								dateTime = Function.ParseDateTimes(dt.Rows[num3]["ShipDate"]);
							}
							dataTable2.Rows.Add(dataRow2);
							dt.Rows.RemoveAt(num3);
							num++;
							num3--;
						}
					}
					i2--;
					dataTable2.AcceptChanges();
					dataRow["DocTotal"] = num2;
					dataRow["DocDueDate"] = dateTime;
					dataTable.Rows.Add(dataRow);
					if (dataTable.Rows.Count == 0 || dataTable2.Rows.Count == 0)
					{
						mess = "Data " + text + " can not null!";
						continue;
					}
					DataSet dataSet = new DataSet();
					dataSet.Tables.Add(dataTable);
					dataSet.Tables.Add(dataTable2);
					using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PurchaseOder");
					HttpResult httpResult = await http.PostAsync("SaveDataSet", dataSet);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						mess = mess + " " + httpResult.Message;
					}
				}
			}
			else
			{
				DataTable dataTable3 = DocumentsHelper.Document.Clone();
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3["DocEntry"] = 0;
				dataRow3["DocEntry"] = 0;
				dataRow3["ReqType"] = 12;
				dataRow3["Requester"] = GlobalData.UserCodeLogin;
				dataRow3["ReqName"] = GlobalData.UserNameLogin;
				dataRow3["BPLId"] = Function.ParseInt(dt.Rows[0]["BplID"]);
				dataRow3["DocStatus"] = "O";
				dataRow3["DocDate"] = serverDate;
				dataRow3["TaxDate"] = serverDate;
				dataRow3["Series"] = Function.ParseInt(dt.Rows[0]["SeriesRequest"]);
				dataRow3["DocRate"] = 1;
				dataRow3["UserSign"] = GlobalData.UserID;
				dataRow3["DocCur"] = GlobalData.MainCurrency;
				DateTime dateTime2 = Function.ParseDateTimes(dt.Rows[0]["ShipDate"]);
				dataRow3["DocRate"] = 1;
				dataRow3["OriginType"] = "R";
				dataRow3["Instance"] = 0;
				dataRow3["Segment"] = 0;
				dataRow3["PIndicator"] = "Default";
				dataRow3["DocSubType"] = "--";
				DataTable dataTable4 = DocumentsHelper.Lines.Clone();
				int num5 = 0;
				double num6 = 0.0;
				for (int num7 = 0; num7 < dt.Rows.Count; num7++)
				{
					string value = Function.ToString(dt.Rows[num7]["CardCode"]);
					DataRow dataRow4 = dataTable4.NewRow();
					dataRow4["Rate"] = 1;
					dataRow4["LineVendor"] = value;
					dataRow4["LineNum"] = num5;
					for (int num8 = 0; num8 < scurceColum.Count; num8++)
					{
						dataRow4[scurceColum[num8]] = Function.ToString(dt.Rows[num7][scurceColum[num8]]);
					}
					dataRow4["Price"] = Function.ParseDouble(dataRow4["PriceBefDi"]) * (1.0 - Function.ParseDouble(dataRow4["DiscPrcnt"]) / 100.0);
					dataRow4["LineTotal"] = Function.ParseDouble(dataRow4["Quantity"]) * Function.ParseDouble(dataRow4["Price"]);
					num6 += Function.ParseDouble(dataRow4["LineTotal"]);
					if (string.IsNullOrEmpty(Function.ToString(dataRow4["Currency"])))
					{
						dataRow4["Currency"] = GlobalData.MainCurrency;
					}
					if (Function.ParseDateTimes(dt.Rows[num7]["ShipDate"]) < dateTime2)
					{
						dateTime2 = Function.ParseDateTimes(dt.Rows[num7]["ShipDate"]);
					}
					num5++;
					dataTable4.Rows.Add(dataRow4);
				}
				dataTable4.AcceptChanges();
				dataRow3["DocTotal"] = num6;
				dataRow3["DocDueDate"] = dateTime2;
				dataRow3["ReqDate"] = dateTime2;
				dataTable3.Rows.Add(dataRow3);
				if (dataTable3.Rows.Count == 0 || dataTable4.Rows.Count == 0)
				{
					return mess + " Data could not null!";
				}
				DataSet dataSet2 = new DataSet();
				dataSet2.Tables.Add(dataTable3);
				dataSet2.Tables.Add(dataTable4);
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PurchaseRequest");
				mess = (await http.PostAsync("SaveDataSet", dataSet2)).Message;
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
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "SlpCode", Function.ParseInt(frm_CFL2.ChooseItem["SlpCode"]));
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
		return await http.GetAsync("GetStructureGenPurchase");
	}
}
