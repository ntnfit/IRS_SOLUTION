using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.LandedCosts;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Payments;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Dashboard;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Interfaces.FormInventory;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Production;
using Apzon.Forms.Purchase;
using Apzon.Forms.Reports;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Apzon.Kit.ContextAction;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.ClientDoc;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions.Shared;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTreeList.Nodes;
using Newtonsoft.Json;
using SAPbobsCOM;

namespace Apzon.Forms.Commons;

public class ContextActionHandle
{
	private readonly int _absEntry;

	private readonly string _formId;

	private GridViewEx _gridsource;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly string _itemId;

	private PivotGridControlEx _pivotGrid;

	private TreeEx _treeView;

	private readonly DataTable _ui;

	private readonly System.Data.DataSet _dataset;

	private readonly XtraFormEx _form;

	private readonly List<AttachmentFile_Impl> _files;

	private readonly string _actionType = "";

	private async Task ProcessMappingApprovalDecision(DataTable source, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range)
	{
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='V' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			for (int k = 0; k < source.Rows.Count; k++)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("WddCode", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Password", typeof(string)));
				dataTable.Columns.Add(new DataColumn("status", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Remarks", typeof(string)));
				DataRow dataRow = dataTable.NewRow();
				dataRow["Password"] = GlobalData.OldPassword;
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow[mapHeaderField[num].TargetField] = source.Rows[k][mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && dataRow[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				string text = await httpClient.WorkingTable.SetStatus(dataTable);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw ex;
		}
	}

	private void ProcessMappingBlanket(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, Enums.ContextActionMethod method = Enums.ContextActionMethod.AddNew, string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataRow[] array = dtMapping.Select("TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='R'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='D'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			List<ContextActionMappingImpl> list = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToList();
			if (method == Enums.ContextActionMethod.Update || method == Enums.ContextActionMethod.AddOrUpdate)
			{
				list.Add(new ContextActionMappingImpl
				{
					SourceField = objectId,
					TargetField = "AbsEntry",
					DfltValue = 0
				});
			}
			ContextActionMappingImpl[] array2 = list.ToArray();
			List<ContextActionMappingImpl> list2 = (from t in source3.AsEnumerable()
				where new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
				select new ContextActionMappingImpl(t)).ToList();
			list2.AddRange((from t in source3.AsEnumerable()
				where !new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
				select new ContextActionMappingImpl(t)).ToArray());
			ContextActionMappingImpl[] array3 = list2.ToArray();
			ContextActionMappingImpl[] mappingDetails = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] array4 = array2.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
			DataTable dataTable = source.Clone();
			if (array4.Length != 0)
			{
				ContextActionMappingImpl[] groupcol = array2.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in source.AsEnumerable()
					group r by new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
				{
					DataRow dataRow = dataTable.NewRow();
					List<Common.ParameterValue> list3 = new List<Common.ParameterValue>();
					object[] values = item2.Key.Values;
					for (int num = 0; num < values.Length; num++)
					{
						if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
						{
							list3.Add(item);
						}
					}
					ContextActionMappingImpl[] array5 = groupcol;
					foreach (ContextActionMappingImpl col in array5)
					{
						if (!string.IsNullOrEmpty(col.SourceField))
						{
							Common.ParameterValue parameterValue = list3.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
							if (parameterValue != null)
							{
								dataRow[col.SourceField] = parameterValue.Value;
							}
						}
					}
					array5 = array4;
					foreach (ContextActionMappingImpl col2 in array5)
					{
						dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			if (dataTable.IsNotNull())
			{
				source = dataTable;
			}
			List<string> list4 = new List<string>();
			List<string> list5 = new List<string>();
			int num2 = 0;
			for (int num3 = 0; num3 < array2.Length; num3++)
			{
				if (!string.IsNullOrEmpty(array2[num3].SourceField))
				{
					list4.Add("ISNULL(" + array2[num3].SourceField + ",'')='{" + num2 + "}'");
					num2++;
				}
			}
			num2 = 0;
			for (int num4 = 0; num4 < array3.Length; num4++)
			{
				if (!string.IsNullOrEmpty(array3[num4].SourceField))
				{
					list5.Add("ISNULL(" + array3[num4].SourceField + ",'')='{" + num2 + "}'");
					num2++;
				}
			}
			dataTable = source.DefaultView.ToTable(distinct: true, (from t in array2
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = BlanketTableHelper.Blanket.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			if (showdoc == "N")
			{
				baseForm.ShowProgress(dataTable.Rows.Count);
			}
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list6 = new List<object>();
				for (int num5 = 0; num5 < array2.Length; num5++)
				{
					if (!string.IsNullOrEmpty(array2[num5].SourceField))
					{
						list6.Add((row[array2[num5].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[array2[num5].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array6 = source.Select(string.Format(string.Join(" AND ", list4), list6.ToArray()));
				if (array6.Length != 0)
				{
					DataTable lines = array6.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in array3
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					if (showdoc != "N")
					{
						DataTable doc = new DataRow[1] { row }.CopyToDataTable();
						frm_SalesBlanketAgreement frm_SalesBlanketAgreement2 = new frm_SalesBlanketAgreement();
						frm_SalesBlanketAgreement2.SetMapping(doc, lines, array2, array3, source, mappingDetails);
						frm_SalesBlanketAgreement2.MdiParent = baseForm.MdiParent;
						frm_SalesBlanketAgreement2.Show();
					}
				}
				if (list4.Count == 0)
				{
					break;
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ProcessMappingBusinesPartner(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='2' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='2' AND TargetItem='A'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='2' AND TargetItem='C'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] array2 = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] array3 = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] array4 = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			if (method == "U")
			{
				List<ContextActionMappingImpl> list = array2.ToList();
				list.Add(new ContextActionMappingImpl
				{
					SourceField = objectId,
					TargetField = "OldCardCode"
				});
				array2 = list.ToArray();
			}
			List<string> list2 = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < array2.Length; num2++)
			{
				if (!string.IsNullOrEmpty(array2[num2].SourceField))
				{
					list2.Add("ISNULL(" + array2[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			if (BussinessPartnerTableHelper.BusinessPartner != null && !BussinessPartnerTableHelper.BusinessPartner.ExistsColumn("OldCardCode"))
			{
				BussinessPartnerTableHelper.BusinessPartner.Columns.Add("OldCardCode", typeof(string));
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in array2
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = BussinessPartnerTableHelper.BusinessPartner.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int num3 = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list3 = new List<object>();
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					if (!string.IsNullOrEmpty(array2[num4].SourceField))
					{
						list3.Add((row[array2[num4].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[array2[num4].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array5 = source.Select(string.Format(string.Join(" AND ", list2), list3.ToArray()));
				if (array5.Length != 0)
				{
					DataTable dataTable2 = array5.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in array3
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					DataTable dataTable3 = array5.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in array4
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					baseForm.SetValueProgress(num3);
					num3++;
					DataTable dataTable4 = BussinessPartnerTableHelper.BusinessPartner.Clone();
					DataRow dataRow2 = dataTable4.NewRow();
					for (int num5 = 0; num5 < array2.Length; num5++)
					{
						if (!string.IsNullOrEmpty(array2[num5].SourceField))
						{
							dataRow2[array2[num5].TargetField] = row[array2[num5].SourceField];
						}
						if (array2[num5].DfltValue != null && dataRow2[array2[num5].TargetField] == DBNull.Value)
						{
							dataRow2[array2[num5].TargetField] = array2[num5].DfltValue;
						}
					}
					dataTable4.Rows.Add(dataRow2);
					DataTable dataTable5 = BussinessPartnerTableHelper.BPAddresses.Clone();
					for (int num6 = 0; num6 < dataTable2.Rows.Count; num6++)
					{
						DataRow dataRow3 = dataTable5.NewRow();
						for (int num7 = 0; num7 < array3.Length; num7++)
						{
							if (!string.IsNullOrEmpty(array3[num7].TargetField) && !string.IsNullOrEmpty(array3[num7].SourceField))
							{
								dataRow3[array3[num7].TargetField] = dataTable2.Rows[num6][array3[num7].SourceField];
							}
							if (array3[num7].DfltValue != null && dataRow3[array3[num7].TargetField] == DBNull.Value)
							{
								dataRow3[array3[num7].TargetField] = array3[num7].DfltValue;
							}
						}
						dataTable5.Rows.Add(dataRow3);
					}
					DataTable dataTable6 = BussinessPartnerTableHelper.ContactEmployees.Clone();
					for (int num8 = 0; num8 < dataTable3.Rows.Count; num8++)
					{
						DataRow dataRow4 = dataTable6.NewRow();
						for (int num9 = 0; num9 < array4.Length; num9++)
						{
							if (!string.IsNullOrEmpty(array4[num9].TargetField) && !string.IsNullOrEmpty(array4[num9].SourceField))
							{
								dataRow4[array4[num9].TargetField] = dataTable3.Rows[num8][array4[num9].SourceField];
							}
							if (array4[num9].DfltValue != null && dataRow4[array4[num9].TargetField] == DBNull.Value)
							{
								dataRow4[array4[num9].TargetField] = array4[num9].DfltValue;
							}
						}
						dataTable6.Rows.Add(dataRow4);
					}
					BPImpls bPImpls = dataRow2.ToBusinessPartner();
					if (bPImpls == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataBusinessPartnerGeneral, "");
						return;
					}
					bPImpls.Addresses = dataTable5.ToBpAddresses();
					bPImpls.ListContactPerson = EntitiesHelper<ContactPersonImpl>.ToEntities(dataTable6);
					string text = HttpClientHelperFactory.GetHttpClient().BusinessMaster.SaveIgnoreEmpty(bPImpls);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
				if (list2.Count == 0)
				{
					break;
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, DataTable ui, GridViewEx gv)
	{
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_ui = ui;
			_gridsource = gv;
			Handle();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public ContextActionHandle(string alias, XtraFormEx form, GridViewEx gv)
	{
		try
		{
			_actionType = alias;
			if (alias == "REFRESH_DATA")
			{
				try
				{
					((dynamic)form).REFRESH_DATA();
					return;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
					WinCommons.Info(Enums.MessageCode.None, ex.Message);
					return;
				}
			}
			DataTable headerByAlias = _httpClient.ContextAction.GetHeaderByAlias(alias, form.FormId, (gv != null) ? gv.ItemId : "");
			if (headerByAlias.Rows.Count != 1)
			{
				throw new Exception("Method is not defined or more than one - [Unique alias] " + alias);
			}
			_absEntry = Function.ParseInt(headerByAlias.Rows[0]["AbsEntry"]);
			_formId = Function.ToString(headerByAlias.Rows[0]["FormId"]);
			_itemId = Function.ToString(headerByAlias.Rows[0]["ItemId"]);
			_dataset = form.DataSet();
			_form = form;
			if (gv == null)
			{
				List<EditorContainer> lst = new List<EditorContainer>();
				GetGridControl(_form, ref lst);
				foreach (GridControlEx item in lst)
				{
					if (item.MainView is GridViewEx { ItemId: var itemId } gridViewEx && itemId == (_itemId.StartsWith("B$") ? _itemId.Substring(2, _itemId.Length - 2) : _itemId))
					{
						gv = gridViewEx;
						break;
					}
				}
			}
			_gridsource = gv;
			_ui = gv.GridControl.DataSource as DataTable;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private static void GetGridControl(Control control, ref List<EditorContainer> lst)
	{
		if (control == null)
		{
			return;
		}
		if (control is GridControlEx)
		{
			lst.Add(control as GridControlEx);
			return;
		}
		foreach (Control control2 in control.Controls)
		{
			GetGridControl(control2, ref lst);
		}
	}

	public ContextActionHandle(string alias, GridViewEx gv)
	{
		try
		{
			_form = gv.GridControl?.FindForm() as XtraFormEx;
			DataTable headerByAlias = _httpClient.ContextAction.GetHeaderByAlias(alias, _form.FormId, (gv != null) ? gv.ItemId : "");
			if (!headerByAlias.IsNotNull())
			{
				throw new Exception("Method is not defined - [Unique alias] " + alias);
			}
			_absEntry = Function.ParseInt(headerByAlias.Rows[0]["AbsEntry"]);
			_formId = Function.ToString(headerByAlias.Rows[0]["FormId"]);
			_itemId = Function.ToString(headerByAlias.Rows[0]["ItemId"]);
			_gridsource = gv;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, DataTable ui, TreeEx tv)
	{
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_ui = ui;
			_treeView = tv;
			Handle();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, DataTable ui)
	{
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_ui = ui;
			Handle();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, DataTable ui, PivotGridControlEx pgc)
	{
		WinCommons._OpenProcessing();
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_ui = ui;
			_pivotGrid = pgc;
			Handle();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, System.Data.DataSet ds, XtraFormEx form, List<AttachmentFile_Impl> lstFile = null)
	{
		WinCommons._OpenProcessing();
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_dataset = ds;
			_form = form;
			_files = lstFile;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public ContextActionHandle(int absEntry, string formId, string itemId, System.Data.DataSet ds, XtraFormEx form)
	{
		WinCommons._OpenProcessing();
		try
		{
			_absEntry = absEntry;
			_formId = formId;
			_itemId = itemId;
			_dataset = ds;
			_form = form;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<HttpResult> Handle()
	{
		HttpResult httpResult = new HttpResult();
		if (_actionType.Equals("REFRESH_DATA"))
		{
			return httpResult;
		}
		try
		{
			XtraFormEx baseForm = new XtraFormEx();
			if (_gridsource != null)
			{
				baseForm = _gridsource.GridControl.FindForm() as XtraFormEx;
			}
			else if (_pivotGrid != null)
			{
				baseForm = _pivotGrid.FindForm() as XtraFormEx;
			}
			else if (_treeView != null)
			{
				baseForm = _treeView.FindForm() as XtraFormEx;
			}
			else if (_form != null)
			{
				baseForm = _form;
			}
			System.Data.DataSet ds;
			if (baseForm.ListContextAction.ContainsKey(_itemId + " " + _absEntry))
			{
				ds = baseForm.ListContextAction[_itemId + " " + _absEntry].Copy();
			}
			else
			{
				ds = _httpClient.ContextAction.GetDataByEntry(_absEntry, _formId, _itemId);
				baseForm.ListContextAction.Add(_itemId + " " + _absEntry, ds.Copy());
			}
			if (ds.IsNotNull())
			{
				DataTable dtDoc = ds.Tables[0];
				Enums.ContextActionActionTypeEx actionType = (Enums.ContextActionActionTypeEx)Function.ParseInt(dtDoc.Rows[0]["ActionTypeEx"]);
				Enums.ContextActionSourceType sourceType = (Enums.ContextActionSourceType)(Function.ParseChar(dtDoc.Rows[0]["SourceType"]) ?? 'N');
				Enums.ContextActionDataRange range = (Enums.ContextActionDataRange)(Function.ParseChar(dtDoc.Rows[0]["DataRange"]) ?? 'A');
				Enums.ContextActionMethod method = (Enums.ContextActionMethod)(Function.ParseChar(dtDoc.Rows[0]["Method"]) ?? 'A');
				Enums.ContextActionDataFrom datafrom = (Enums.ContextActionDataFrom)(Function.ParseChar(dtDoc.Rows[0]["DataFrom"]) ?? 'T');
				string dataString = Function.ToString(dtDoc.Rows[0]["DataString"]);
				string objectId = Function.ToString(dtDoc.Rows[0]["ObjectId"]);
				string filterString = Function.ToString(dtDoc.Rows[0]["FilterString"]);
				DataTable source = await GetMappingSource(ds, sourceType, range, datafrom, dataString);
				if (!string.IsNullOrEmpty(filterString))
				{
					DataRow[] array = source.Select(filterString);
					if (array.Length != 0)
					{
						source = array.CopyToDataTable();
					}
					else
					{
						source = source.Clone();
					}
				}
				string showdoc = Function.ToString(dtDoc.Rows[0]["ShowDocForm"]);
				string ignoreError = Function.ToString(dtDoc.Rows[0]["IgnError"]);
				string text = Function.ToString(Function.ToString(dtDoc.Rows[0]["MsgBefore"]));
				if (actionType != Enums.ContextActionActionTypeEx.None && !string.IsNullOrWhiteSpace(text) && XtraMessageBox.Show(baseForm, text, "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					return new HttpResult(Enums.MessageCode.Error, "EventCanceled");
				}
				switch (actionType)
				{
				case Enums.ContextActionActionTypeEx.MaketingDocument:
				{
					Enums.ContextActionDocumentType objtype = (Enums.ContextActionDocumentType)Function.ParseInt(dtDoc.Rows[0]["ObjType"]);
					DataTable dtMap3 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap3 = ds.Tables[2];
					}
					IContextActionHandle actionHandle = ContextActionHandleFactory.GetActionHandle();
					Enums.ContextActionMethod contextActionMethod = method;
					if ((uint)(contextActionMethod - 67) > 1u && contextActionMethod != Enums.ContextActionMethod.Close)
					{
						if (dtMap3.IsNotNull())
						{
							if (showdoc != "N")
							{
								return await ProcessMappingDocument(source, showdoc, ignoreError, baseForm, dtMap3, objtype, method, objectId);
							}
							return Task.Run(async () => await ProcessMappingDocument(source, showdoc, ignoreError, baseForm, dtMap3, objtype, method, objectId)).Result;
						}
						WinCommons.Error(Enums.MessageCode.DataMappingIsMissing, "");
						return new HttpResult(Enums.MessageCode.Error, "Data mapping is missing");
					}
					ApzObjectTypes objectTypesFromContext = actionHandle.GetObjectTypesFromContext(objtype);
					IHttpClient httpclient2 = HttpClientHelperFactory.GetHttpClient(objectTypesFromContext);
					string[] array4 = Function.ToString(GetValue(objectId, sourceType, range, "0")).Split(',');
					List<string> list2 = new List<string>();
					WinCommons._OpenProcessing();
					int num4 = 1;
					string[] array3 = array4;
					foreach (string obj2 in array3)
					{
						int docEntry = Function.ParseInt(obj2);
						if (docEntry == 0)
						{
							WinCommons.Error(Enums.MessageCode.DocEntryIsMissing, "");
							WinCommons._CloseProcessing();
							return new HttpResult(Enums.MessageCode.Error, "DocEntry is missing");
						}
						HttpResult httpResult2 = new HttpResult();
						switch (method)
						{
						case Enums.ContextActionMethod.Close:
							if (new ApzObjectTypes[1] { ApzObjectTypes.oStockTransfer }.Contains(objectTypesFromContext))
							{
								HttpClientHelper<string> helper4 = new HttpClientHelper<string>(Configuration.ApiServer, "InventoryTranfer");
								try
								{
									httpResult2 = new HttpResult(Enums.MessageCode.None, Task.Run(async () => await helper4.PostAsync("CloseDoc", docEntry)).Result);
								}
								finally
								{
									if (helper4 != null)
									{
										((IDisposable)helper4).Dispose();
									}
								}
							}
							else
							{
								httpResult2 = Task.Run(async () => await httpclient2.Documents.Close(docEntry)).Result;
							}
							break;
						case Enums.ContextActionMethod.Cancel:
							if (new ApzObjectTypes[1] { ApzObjectTypes.oStockTransfer }.Contains(objectTypesFromContext))
							{
								HttpClientHelper<string> helper3 = new HttpClientHelper<string>(Configuration.ApiServer, "InventoryTranfer");
								try
								{
									httpResult2 = new HttpResult(Enums.MessageCode.None, Task.Run(async () => await helper3.PostAsync("CancelDoc", docEntry)).Result);
								}
								finally
								{
									if (helper3 != null)
									{
										((IDisposable)helper3).Dispose();
									}
								}
							}
							else
							{
								httpResult2 = Task.Run(async () => await httpclient2.Documents.Cancel(docEntry)).Result;
							}
							break;
						case Enums.ContextActionMethod.SaveDraftToDocument:
						{
							if (new ApzObjectTypes[2]
							{
								ApzObjectTypes.oInventoryTransferRequest,
								ApzObjectTypes.oStockTransfer
							}.Contains(objectTypesFromContext))
							{
								HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "InventoryTranfer");
								try
								{
									httpResult2 = Task.Run(async () => await helper.PostAsync("SaveDraftToDocument", docEntry)).Result;
								}
								finally
								{
									if (helper != null)
									{
										((IDisposable)helper).Dispose();
									}
								}
								break;
							}
							HttpClientHelper<HttpResult> helper2 = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingProcess");
							try
							{
								httpResult2 = Task.Run(async () => await helper2.PostAsync("SaveDraftToDocument", new DocumentsImpl
								{
									draftKey = docEntry
								})).Result;
							}
							finally
							{
								if (helper2 != null)
								{
									((IDisposable)helper2).Dispose();
								}
							}
							break;
						}
						}
						if (!string.IsNullOrEmpty(httpResult2.Message))
						{
							if (ignoreError == "N")
							{
								WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
								return httpResult2;
							}
							list2.Add($"(*{num4++}*) {httpResult2.Message}");
						}
					}
					WinCommons._CloseProcessing();
					if (list2.Any())
					{
						WinCommons.Error(Enums.MessageCode.None, string.Join(Environment.NewLine, list2));
						return new HttpResult(Enums.MessageCode.Error, string.Join(Environment.NewLine, list2));
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
					return new HttpResult();
				}
				case Enums.ContextActionActionTypeEx.QueryReport:
				{
					Enums.ContextActionReportFrom contextActionReportFrom2 = (Enums.ContextActionReportFrom)(Function.ParseChar(dtDoc.Rows[0]["RptFrom"]) ?? 'Q');
					DataTable dataTable6 = ds.Tables[1];
					bool flag = Function.ToString(dtDoc.Rows[0]["JoinValue"]) == "Y";
					foreach (DataRow row in source.Rows)
					{
						switch (contextActionReportFrom2)
						{
						case Enums.ContextActionReportFrom.DefinedQuery:
						case Enums.ContextActionReportFrom.QueryText:
						{
							List<QueryParamater> list10 = new List<QueryParamater>();
							if (dataTable6.IsNotNull())
							{
								foreach (DataRow item4 in from t in dataTable6.AsEnumerable()
									where Function.ToString(t["ParamType"]) == "R"
									select t)
								{
									QueryParamater queryParamater2 = new QueryParamater();
									queryParamater2.ParaName = Function.ToString(item4["ParamName"]);
									string text7 = Function.ToString(item4["SourceField"]);
									if (!string.IsNullOrEmpty(text7))
									{
										if (flag)
										{
											queryParamater2.Value = GetValue(text7, sourceType, range, Function.ToString(item4["DefaultValue"]));
										}
										else
										{
											object value3 = ((row[text7] == DBNull.Value) ? Function.ToString(item4["DefaultValue"]) : row[text7]);
											queryParamater2.Value = value3;
										}
									}
									else if (!string.IsNullOrEmpty(Function.ToString(item4["DefaultValue"])))
									{
										queryParamater2.Value = Function.ToString(item4["DefaultValue"]);
									}
									list10.Add(queryParamater2);
								}
							}
							switch (contextActionReportFrom2)
							{
							case Enums.ContextActionReportFrom.DefinedQuery:
							{
								int queryId = Function.ParseInt(dtDoc.Rows[0]["RptString"]);
								frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
								frm_QueryPreview2.QueryParamaters = list10;
								frm_QueryPreview2.QueryId = queryId;
								ShowForm(frm_QueryPreview2, baseForm);
								break;
							}
							case Enums.ContextActionReportFrom.QueryText:
							{
								frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
								frm_QueryPreview2.QueryParamaters = list10;
								frm_QueryPreview2.Sql = Function.ToString(dtDoc.Rows[0]["RptString"]);
								string text8 = Function.ToString(dtDoc.Rows[0]["FormId"]) + Function.ToString(dtDoc.Rows[0]["ItemId"]) + Function.ToString(dtDoc.Rows[0]["AbsEntry"]);
								frm_QueryPreview2.ActionId = Function.ToString(text8.GetHashCode());
								ShowForm(frm_QueryPreview2, baseForm);
								break;
							}
							}
							break;
						}
						case Enums.ContextActionReportFrom.UserDefinedReport:
						{
							string text5 = Function.ToString(dtDoc.Rows[0]["RptString"]);
							List<Common.ParameterValue> list9 = new List<Common.ParameterValue>();
							if (dataTable6.IsNotNull())
							{
								foreach (DataRow item5 in from t in dataTable6.AsEnumerable()
									where Function.ToString(t["ParamType"]) == "R"
									select t)
								{
									Common.ParameterValue parameterValue2 = new Common.ParameterValue();
									parameterValue2.Name = Function.ToString(item5["ParamName"]);
									string text6 = Function.ToString(item5["SourceField"]);
									if (!string.IsNullOrEmpty(text6))
									{
										if (flag)
										{
											parameterValue2.Value = GetValue(text6, sourceType, range, Function.ToString(item5["DefaultValue"]));
										}
										else
										{
											object value2 = ((row[text6] == DBNull.Value) ? Function.ToString(item5["DefaultValue"]) : row[text6]);
											parameterValue2.Value = value2;
										}
									}
									else if (!string.IsNullOrEmpty(Function.ToString(item5["DefaultValue"])))
									{
										parameterValue2.Value = Function.ToString(item5["DefaultValue"]);
									}
									list9.Add(parameterValue2);
								}
							}
							frm_ReportForm frm_ReportForm2 = new frm_ReportForm(text5, list9);
							frm_ReportForm2.Name = "frm_" + text5;
							frm_ReportForm2.isExecuteWhenLoad = true;
							ShowForm(frm_ReportForm2, baseForm);
							break;
						}
						}
						if (flag)
						{
							break;
						}
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.DoQuery:
				{
					Enums.ContextActionReportFrom contextActionReportFrom = (Enums.ContextActionReportFrom)(Function.ParseChar(dtDoc.Rows[0]["QueryFrom"]) ?? 'Q');
					DataTable dataTable5 = ds.Tables[1];
					if (contextActionReportFrom != Enums.ContextActionReportFrom.DefinedQuery && contextActionReportFrom != Enums.ContextActionReportFrom.QueryText)
					{
						break;
					}
					if (!string.IsNullOrWhiteSpace(Function.ToString(dtDoc.Rows[0]["TempTblNameDoQuery"])))
					{
						ContextActionDoQuery contextActionDoQuery = new ContextActionDoQuery();
						if (!source.IsNotNull())
						{
							return new HttpResult(Enums.MessageCode.Error, "No Data");
						}
						if (string.IsNullOrWhiteSpace(Function.ToString(dtDoc.Rows[0]["QueryString"])))
						{
							return new HttpResult(Enums.MessageCode.Error, "Could not get query.");
						}
						contextActionDoQuery.TempTableData = source;
						contextActionDoQuery.TempTableName = Function.ToString(dtDoc.Rows[0]["TempTblNameDoQuery"]);
						contextActionDoQuery.QueryText = Function.ToString(dtDoc.Rows[0]["QueryString"]);
						HttpResult result5 = Task.Run(async () => await _httpClient.Commons.DoQueryWithTempTable(Function.ToString(dtDoc.Rows[0]["QueryString"]), Function.ToString(dtDoc.Rows[0]["TempTblNameDoQuery"]), source)).Result;
						if (!string.IsNullOrEmpty(result5.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, result5.Message);
							return result5;
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						return result5;
					}
					if (!dataTable5.IsNotNull())
					{
						break;
					}
					List<ContextActionDoQuery> doquery = new List<ContextActionDoQuery>();
					EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable5.AsEnumerable()
						where Function.ToString(t["ParamType"]) == "Q"
						select t;
					foreach (DataRow row2 in source.DefaultView.ToTable(distinct: true, enumerableRowCollection.Select((DataRow t) => Function.ToString(t["SourceField"])).Distinct().ToArray()).Rows)
					{
						ContextActionDoQuery contextActionDoQuery2 = new ContextActionDoQuery();
						contextActionDoQuery2.QueryText = Function.ToString(dtDoc.Rows[0]["QueryString"]);
						List<QueryParamater> list7 = new List<QueryParamater>();
						foreach (DataRow item6 in enumerableRowCollection)
						{
							QueryParamater queryParamater = new QueryParamater();
							queryParamater.ParaName = Function.ToString(item6["ParamName"]);
							string text3 = Function.ToString(item6["SourceField"]);
							if (!string.IsNullOrEmpty(text3))
							{
								object value = ((row2[text3] == DBNull.Value) ? Function.ToString(item6["DefaultValue"]) : row2[text3]);
								queryParamater.Value = value;
							}
							else if (!string.IsNullOrEmpty(Function.ToString(item6["DefaultValue"])))
							{
								queryParamater.Value = Function.ToString(item6["DefaultValue"]);
							}
							list7.Add(queryParamater);
						}
						contextActionDoQuery2.Paramaters = list7;
						doquery.Add(contextActionDoQuery2);
					}
					if (doquery.Count > 0)
					{
						HttpResult result6 = Task.Run(async () => await _httpClient.Commons.DoQuery(doquery)).Result;
						if (!string.IsNullOrEmpty(result6.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, result6.Message);
							return result6;
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						return result6;
					}
					return new HttpResult(Enums.MessageCode.Error, "Could not get query info.");
				}
				case Enums.ContextActionActionTypeEx.UserDefinedObject:
				{
					if (!source.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.DatasourceIsMissing, "");
						return new HttpResult(Enums.MessageCode.Error, "Data source is missing");
					}
					DataTable dataTable2 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dataTable2 = ds.Tables[2];
					}
					if (dataTable2.IsNotNull())
					{
						ContextActionMappingImpl[] map = (from t in dataTable2.AsEnumerable()
							select new ContextActionMappingImpl(t)).ToArray();
						string udoCode = Function.ToString(dtDoc.Rows[0]["UdoCode"]);
						ContextActionMappingImpl[] mapHeaderField = map.Where((ContextActionMappingImpl t) => t.TargetItem == udoCode).ToArray();
						DataTable dataTable3 = source.DefaultView.ToTable(distinct: false, (from t in mapHeaderField
							where !string.IsNullOrEmpty(t.SourceField)
							select t.SourceField).Distinct().ToArray());
						ContextActionMappingImpl[] array5 = mapHeaderField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
						DataTable dataTable4 = dataTable3.Clone();
						if (array5.Length != 0)
						{
							ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
							foreach (IGrouping<NTuple<object>, DataRow> item7 in from r in dataTable3.AsEnumerable()
								group r by new NTuple<object>(from m in groupcol
									where !string.IsNullOrEmpty(m.SourceField)
									select new
									{
										Name = m.SourceField,
										Value = r[m.SourceField]
									}))
							{
								DataRow dataRow = dataTable4.NewRow();
								List<Common.ParameterValue> list3 = new List<Common.ParameterValue>();
								object[] values = item7.Key.Values;
								for (int num2 = 0; num2 < values.Length; num2++)
								{
									if (values[num2].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
									{
										list3.Add(item);
									}
								}
								ContextActionMappingImpl[] array6 = groupcol;
								foreach (ContextActionMappingImpl col in array6)
								{
									if (!string.IsNullOrEmpty(col.SourceField))
									{
										Common.ParameterValue parameterValue = list3.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
										if (parameterValue != null)
										{
											dataRow[col.SourceField] = parameterValue.Value;
										}
									}
								}
								array6 = array5;
								foreach (ContextActionMappingImpl col2 in array6)
								{
									dataRow[col2.SourceField] = item7.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
								}
								dataTable4.Rows.Add(dataRow);
							}
							dataTable4.AcceptChanges();
						}
						else
						{
							dataTable4 = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
								where !string.IsNullOrEmpty(t.SourceField)
								select t.SourceField).Distinct().ToArray());
						}
						if (showdoc != "Y")
						{
							System.Data.DataSet result2 = Task.Run(async () => await GetUdoStruct(udoCode, mapHeaderField, map, source)).Result;
							if (!dataTable4.IsNotNull())
							{
								break;
							}
							foreach (DataRow row3 in dataTable4.Rows)
							{
								System.Data.DataSet dsRequest = GetUdo(result2, udoCode, mapHeaderField, row3, map, source);
								HttpResult result3 = Task.Run(async () => await _httpClient.Udo.Save(dsRequest)).Result;
								if (!string.IsNullOrEmpty(result3.Message))
								{
									WinCommons.Error(Enums.MessageCode.None, result3.Message);
									return result3;
								}
								WinCommons.Success(Enums.MessageCode.Success, "");
								if (mapHeaderField.Length == 0)
								{
									break;
								}
							}
							return new HttpResult();
						}
						List<string> list4 = new List<string>();
						int num5 = 0;
						for (int num6 = 0; num6 < mapHeaderField.Length; num6++)
						{
							if (!string.IsNullOrEmpty(mapHeaderField[num6].SourceField))
							{
								list4.Add("ISNULL(" + mapHeaderField[num6].SourceField + ",'')='{" + num5 + "}'");
								num5++;
							}
						}
						if (dataTable4.IsNotNull())
						{
							foreach (DataRow item3 in dataTable4.Rows)
							{
								List<object> list5 = new List<object>();
								for (int num7 = 0; num7 < mapHeaderField.Length; num7++)
								{
									if (!string.IsNullOrEmpty(mapHeaderField[num7].SourceField))
									{
										list5.Add((item3[mapHeaderField[num7].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(item3[mapHeaderField[num7].SourceField]).Replace("'", "''"));
									}
								}
								DataTable lines = new DataTable();
								ContextActionMappingImpl[] groupcol2 = mapHeaderField.Where((ContextActionMappingImpl r) => r.SumType == "").Distinct().ToArray();
								DataRow[] array7 = (from r in source.AsEnumerable()
									where new NTuple<object>(from m in groupcol2
										where !string.IsNullOrEmpty(m.SourceField)
										select new
										{
											Name = m.SourceField,
											Value = item3[m.SourceField]
										}).Equals(new NTuple<object>(from m in groupcol2
										where !string.IsNullOrEmpty(m.SourceField)
										select new
										{
											Name = m.SourceField,
											Value = r[m.SourceField]
										}))
									select (r)).ToArray();
								if (array7.Length != 0)
								{
									lines = array7.CopyToDataTable();
								}
								int docEntry2 = 0;
								if (mapHeaderField.Any((ContextActionMappingImpl m) => m.TargetField == "DocEntry"))
								{
									string sourceField = mapHeaderField.FirstOrDefault((ContextActionMappingImpl m) => m.TargetField == "DocEntry").SourceField;
									if (!string.IsNullOrEmpty(sourceField))
									{
										docEntry2 = Function.ParseInt(item3[sourceField]);
									}
								}
								string code = "";
								if (mapHeaderField.Any((ContextActionMappingImpl m) => m.TargetField == "Code"))
								{
									string sourceField2 = mapHeaderField.FirstOrDefault((ContextActionMappingImpl m) => m.TargetField == "Code").SourceField;
									if (!string.IsNullOrEmpty(sourceField2))
									{
										code = Function.ToString(item3[sourceField2]);
									}
								}
								frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry2, code);
								frm_UDO2.Name = "UDO_" + udoCode;
								frm_UDO2.SetMapping(new DataRow[1] { item3 }.CopyToDataTable(), lines, map);
								if (baseForm != null)
								{
									frm_UDO2.MdiParent = baseForm.MdiParent;
								}
								frm_UDO2.Show();
								if (mapHeaderField.Length == 0)
								{
									break;
								}
							}
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.HeaderDataMappingIsMissing, "");
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.DataMappingIsMissing, "");
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.Dashboard:
				{
					WinCommons._OpenProcessing();
					int dashboardId = Function.ParseInt(dtDoc.Rows[0]["DashboardId"]);
					if (dashboardId > 0)
					{
						frm_DashboardViewer frm_DashboardViewer2 = new frm_DashboardViewer(await baseForm.LoadDashboardFromSource(dashboardId, source), dashboardId)
						{
							Text = Function.ToString(dtDoc.Rows[0]["Caption"])
						};
						if (baseForm != null)
						{
							frm_DashboardViewer2.MdiParent = baseForm.MdiParent;
						}
						frm_DashboardViewer2.Show();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.ProductionOrder:
				{
					DataTable dtMapping6 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping6 = ds.Tables[2];
					}
					Enums.ContextActionMethod contextActionMethod = method;
					if (contextActionMethod == Enums.ContextActionMethod.Cancel || contextActionMethod == Enums.ContextActionMethod.Close || contextActionMethod == Enums.ContextActionMethod.Release)
					{
						string[] array3 = Function.ToString(GetValue(objectId, sourceType, range, "0")).Split(',');
						foreach (string obj4 in array3)
						{
							if (Function.ParseInt(obj4) == 0)
							{
								WinCommons.Error(Enums.MessageCode.DocEntryIsMissing, "");
								WinCommons._CloseProcessing();
								return new HttpResult(Enums.MessageCode.Error, "DocEntry is missing");
							}
							ProductionOder_Impl_OWOR docProduction = new ProductionOder_Impl_OWOR();
							docProduction.DocEntry = Function.ParseInt(obj4);
							switch (method)
							{
							case Enums.ContextActionMethod.Release:
								docProduction.Status = "R";
								break;
							case Enums.ContextActionMethod.Close:
								docProduction.Status = "L";
								break;
							case Enums.ContextActionMethod.Cancel:
								docProduction.Status = "C";
								break;
							}
							WinCommons._OpenProcessing();
							HttpResult result7 = Task.Run(async () => await _httpClient.ProductionOrders.UpdateStatus(new List<ProductionOder_Impl_OWOR> { docProduction })).Result;
							WinCommons._CloseProcessing();
							if (!string.IsNullOrEmpty(result7.Message))
							{
								WinCommons.Error(Enums.MessageCode.None, result7.Message);
							}
							else
							{
								WinCommons.Success(Enums.MessageCode.Success, "");
							}
						}
					}
					else
					{
						await ProcessMappingProduction(source, showdoc, baseForm, dtMapping6, objectId, sourceType, range, method);
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.BusinessBusinessPartner:
				{
					DataTable dtMapping12 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping12 = ds.Tables[2];
					}
					ProcessMappingBusinesPartner(source, showdoc, baseForm, dtMapping12, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.Items:
				{
					DataTable dtMapping7 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping7 = ds.Tables[2];
					}
					await ProcessMappingItem(source, showdoc, baseForm, dtMapping7, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.BillofMaterials:
				{
					DataTable dtMapping11 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping11 = ds.Tables[2];
					}
					ProcessMappingProductTree(source, showdoc, baseForm, dtMapping11, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.JournalEntry:
					if (method == Enums.ContextActionMethod.AddNew)
					{
						DataTable dtMap4 = new DataTable();
						if (ds.Tables.Count >= 3)
						{
							dtMap4 = ds.Tables[2];
						}
						return Task.Run(async () => await ProcessMappingJournalEntry(source, showdoc, baseForm, dtMap4, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]))).Result;
					}
					if (method == Enums.ContextActionMethod.Cancel)
					{
						IHttpClient httpclient3 = HttpClientHelperFactory.GetHttpClient();
						string[] array8 = Function.ToString(GetValue(objectId, sourceType, range, "0")).Split(',');
						List<string> list6 = new List<string>();
						WinCommons._OpenProcessing();
						int num8 = 1;
						string[] array3 = array8;
						foreach (string obj3 in array3)
						{
							int transId = Function.ParseInt(obj3);
							if (transId == 0)
							{
								WinCommons.Error(Enums.MessageCode.DocEntryIsMissing, "");
								WinCommons._CloseProcessing();
								return new HttpResult(Enums.MessageCode.Error, "DocEntry is missing");
							}
							HttpResult result4 = Task.Run(async () => await httpclient3.JournalEntry.CancelJE(transId)).Result;
							if (!string.IsNullOrEmpty(result4.Message))
							{
								if (ignoreError == "N")
								{
									WinCommons.Error(Enums.MessageCode.None, result4.Message);
									return result4;
								}
								list6.Add($"(*{num8++}*) {result4.Message}");
							}
						}
						WinCommons._CloseProcessing();
						if (list6.Any())
						{
							WinCommons.Error(Enums.MessageCode.None, string.Join(Environment.NewLine, list6));
							return new HttpResult(Enums.MessageCode.Error, string.Join(Environment.NewLine, list6));
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						return new HttpResult();
					}
					throw new Exception("Invalid method, only Add new and Cancel are accepted.");
				case Enums.ContextActionActionTypeEx.JournalVouchers:
				{
					DataTable dtMap6 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap6 = ds.Tables[2];
					}
					return Task.Run(async () => await ProcessMappingJournalVoucher(source, showdoc, baseForm, dtMap6, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]))).Result;
				}
				case Enums.ContextActionActionTypeEx.EMail:
				{
					string text4 = Function.ToString(dtDoc.Rows[0]["EmailGroupBy"]);
					string[] emailgroupbyFields = Function.ToString(dtDoc.Rows[0]["EmailGroupByFields"]).Split(';');
					string addressFrom = Function.ToString(dtDoc.Rows[0]["AddressFrom"]);
					string addressTo = Function.ToString(dtDoc.Rows[0]["AddressTo"]);
					string addressCc = Function.ToString(dtDoc.Rows[0]["AddressCc"]);
					string addressBcc = Function.ToString(dtDoc.Rows[0]["AddressBcc"]);
					string subject = Function.ToString(dtDoc.Rows[0]["Subject"]);
					string bodytext = Function.ToString(dtDoc.Rows[0]["BodyText"]);
					string msgSuccess = Function.ToString(dtDoc.Rows[0]["MsgSuccess"]);
					string msgSuccessType = Function.ToString(dtDoc.Rows[0]["MsgType"]);
					if (!(text4 == "R"))
					{
						if (!(text4 == "F"))
						{
							await ProcessEmail(ds, addressTo, addressFrom, addressCc, addressBcc, subject, bodytext, msgSuccess, msgSuccessType, new DataRow[1] { source.Rows[0] }.CopyToDataTable());
							break;
						}
						var enumerable = from r in source.AsEnumerable()
							group r by new NTuple<object>(from text9 in emailgroupbyFields
								where !string.IsNullOrEmpty(text9)
								select new
								{
									Name = text9,
									Value = r[text9]
								}) into r
							select new
							{
								Items = r.CopyToDataTable()
							};
						foreach (var item8 in enumerable)
						{
							await ProcessEmail(ds, addressTo, addressFrom, addressCc, addressBcc, subject, bodytext, msgSuccess, msgSuccessType, item8.Items);
						}
						break;
					}
					foreach (DataRow row4 in source.Rows)
					{
						await ProcessEmail(ds, addressTo, addressFrom, addressCc, addressBcc, subject, bodytext, msgSuccess, msgSuccessType, new DataRow[1] { row4 }.CopyToDataTable());
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.Forecast:
				{
					DataTable dataTable = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dataTable = ds.Tables[2];
					}
					if (!dataTable.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.DataMappingIsMissing, "");
						return new HttpResult(Enums.MessageCode.Error, "Data mapping is missing");
					}
					await ProcessMappingForecast(source, showdoc, baseForm, dataTable);
					break;
				}
				case Enums.ContextActionActionTypeEx.WorkFlowProcessGroup:
					return Task.Run(async () => await ProcessWorkflow(_httpClient, _absEntry, _formId, _itemId, source)).Result;
				case Enums.ContextActionActionTypeEx.PrintLayout:
					return await ProcessPrintLayout(source, ds, baseForm);
				case Enums.ContextActionActionTypeEx.ApprovalDecision:
				{
					DataTable dtMapping13 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping13 = ds.Tables[2];
					}
					await ProcessMappingApprovalDecision(source, dtMapping13, sourceType, range);
					break;
				}
				case Enums.ContextActionActionTypeEx.Task:
				{
					DataTable dtMapping10 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping10 = ds.Tables[2];
					}
					await ProcessMappingTask(source, showdoc, baseForm, dtMapping10, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.Note:
				{
					DataTable dtMapping8 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping8 = ds.Tables[2];
					}
					await ProcessMappingNote(source, showdoc, baseForm, dtMapping8, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.Event:
				{
					DataTable dtMapping5 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping5 = ds.Tables[2];
					}
					await ProcessMappingEvent(source, showdoc, baseForm, dtMapping5, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.ProjectManagement:
				{
					DataTable dtMapping4 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping4 = ds.Tables[2];
					}
					await ProcessMappingProjectManagement(source, showdoc, baseForm, dtMapping4, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.Opportunity:
				{
					DataTable dtMapping3 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping3 = ds.Tables[2];
					}
					await ProcessMappingOpportunity(source, showdoc, baseForm, dtMapping3, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.TrackingIssue:
				{
					DataTable dtMapping = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping = ds.Tables[2];
					}
					await ProcessMappingTrackingIssue(source, showdoc, baseForm, dtMapping, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.HttpRequest:
				{
					DataTable dtMap7 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap7 = ds.Tables[2];
					}
					return Task.Run(async () => await ProcessMappingHttpRequest(source, baseForm, dtMap7, ds)).Result;
				}
				case Enums.ContextActionActionTypeEx.Script:
				{
					List<Token> list8;
					if (baseForm.ListSyntaxContextAction.ContainsKey(_itemId + " " + _absEntry))
					{
						list8 = baseForm.ListSyntaxContextAction[_itemId + " " + _absEntry];
					}
					else
					{
						list8 = ControlFunction.GetToken(Function.ToString(dtDoc.Rows[0]["DIYScript"]));
						baseForm.ListSyntaxContextAction.Add(_itemId + " " + _absEntry, list8);
					}
					CtdMessage ctdMessage = new Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(baseForm, _gridsource, list8, out var _);
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.Blanket:
				{
					DataTable dtMapping9 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping9 = ds.Tables[2];
					}
					ProcessMappingBlanket(source, showdoc, baseForm, dtMapping9, sourceType, range, method, Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.InternalReconciliations:
				{
					DataTable dtMap5 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap5 = ds.Tables[2];
					}
					return Task.Run(async () => await ProcessMappingInternalReconciliations(source, showdoc, baseForm, dtMap5, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]))).Result;
				}
				case Enums.ContextActionActionTypeEx.Payments:
				{
					string text2 = Function.ToString(dtDoc.Rows[0]["ObjType"]);
					ContextActionHandleFactory.GetActionHandle();
					if (method == Enums.ContextActionMethod.Cancel)
					{
						string[] array2 = Function.ToString(GetValue(objectId, sourceType, range, "0")).Split(',');
						List<string> list = new List<string>();
						WinCommons._OpenProcessing();
						int num = 1;
						string[] array3 = array2;
						foreach (string obj in array3)
						{
							int num3 = Function.ParseInt(obj);
							if (num3 == 0)
							{
								WinCommons.Error(Enums.MessageCode.DocEntryIsMissing, "");
								WinCommons._CloseProcessing();
								return new HttpResult(Enums.MessageCode.Error, "DocEntry is missing");
							}
							IHttpClient httpclient = HttpClientHelperFactory.GetHttpClient();
							new HttpResult();
							ProcessJsonImpl data = new ProcessJsonImpl();
							data.ObjKey = num3;
							data.Action = "C";
							if (!(text2 == "24"))
							{
								if (text2 == "46")
								{
									data.ObjType = "46";
								}
								else
								{
									new HttpResult(Enums.MessageCode.DataMappingIsMissing, "The objtype has not been mapped");
								}
							}
							else
							{
								data.ObjType = "24";
							}
							HttpResult result = Task.Run(async () => await httpclient.Commons.ProcessSAPJson(data)).Result;
							if (!string.IsNullOrEmpty(result.Message))
							{
								if (ignoreError == "N")
								{
									WinCommons.Error(Enums.MessageCode.None, result.Message);
									return result;
								}
								list.Add($"(*{num++}*) {result.Message}");
							}
						}
						WinCommons._CloseProcessing();
						if (list.Any())
						{
							WinCommons.Error(Enums.MessageCode.None, string.Join(Environment.NewLine, list));
							return new HttpResult(Enums.MessageCode.Error, string.Join(Environment.NewLine, list));
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						return new HttpResult();
					}
					DataTable dtMap2 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap2 = ds.Tables[2];
					}
					return Task.Run(async () => await ProcessMappingPayment(source, showdoc, baseForm, dtMap2, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]), Function.ToString(dtDoc.Rows[0]["ObjType"]))).Result;
				}
				case Enums.ContextActionActionTypeEx.PurchaseLandedCost:
				{
					DataTable dtMapping2 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMapping2 = ds.Tables[2];
					}
					await ProcessMappingLandedCosts(source, showdoc, baseForm, dtMapping2, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]));
					break;
				}
				case Enums.ContextActionActionTypeEx.Warehouses:
				{
					DataTable dtMap = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dtMap = ds.Tables[2];
					}
					return Task.Run(async () => await ProcessMappingWarehouse(source, showdoc, baseForm, dtMap, sourceType, range, Function.ToString(dtDoc.Rows[0]["Method"]), Function.ToString(dtDoc.Rows[0]["ObjectId"]))).Result;
				}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex.InnerException ?? ex);
			Logging.Write("ERROR", _absEntry + " - " + ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, (ex.InnerException ?? ex).Message);
		}
		WinCommons._CloseProcessing();
		return httpResult;
	}

	private async Task<System.Data.DataSet> GetUdoStruct(string udoCode, ContextActionMappingImpl[] mapHeaderField, ContextActionMappingImpl[] map, DataTable source)
	{
		System.Data.DataSet dsUdo = new System.Data.DataSet();
		DataTable dataTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
		{
			Name = "OUDO",
			Code = udoCode
		});
		string tableName = Function.ToString(dataTable.Rows[0]["TableName"]);
		dsUdo.Tables.Add(dataTable.Copy());
		DataTable dataTable2 = new DataTable(tableName);
		string text = Function.ToString(dataTable.Rows[0]["TYPE"]);
		if (!(text == "1"))
		{
			if (text == "3")
			{
				dataTable2.Columns.Add("DocEntry", typeof(int));
			}
		}
		else
		{
			dataTable2.Columns.Add("Code", typeof(string));
			dataTable2.Columns.Add("Name", typeof(string));
			dataTable2.Columns.Add("DocEntry", typeof(int));
		}
		foreach (ContextActionMappingImpl contextActionMappingImpl in mapHeaderField)
		{
			string targetField = contextActionMappingImpl.TargetField;
			if (dataTable2.ExistsColumn(targetField))
			{
				continue;
			}
			Type type = typeof(string);
			if (source.Columns.Contains(contextActionMappingImpl.SourceField))
			{
				type = source.Columns[contextActionMappingImpl.SourceField].DataType;
				if (type == typeof(DateTime))
				{
					dataTable2.Columns.Add(targetField, typeof(DateTime));
					continue;
				}
			}
			dataTable2.Columns.Add(targetField, type);
		}
		dsUdo.Tables.Add(dataTable2);
		var array = (from t in map
			where t.TargetItem != udoCode
			group t by t.TargetItem into t
			select new
			{
				Key = t.Key,
				Items = t
			}).ToArray();
		foreach (var anon in array)
		{
			DataTable dataTable3 = new DataTable(anon.Key);
			if (!(text == "1"))
			{
				if (text == "3")
				{
					dataTable3.Columns.Add("DocEntry", typeof(int));
				}
			}
			else
			{
				dataTable3.Columns.Add("Code", typeof(string));
			}
			dataTable3.Columns.Add("LineId", typeof(int));
			foreach (ContextActionMappingImpl item in anon.Items)
			{
				string targetField2 = item.TargetField;
				if (dataTable3.ExistsColumn(targetField2))
				{
					continue;
				}
				Type type2 = typeof(string);
				if (source.Columns.Contains(item.SourceField))
				{
					type2 = source.Columns[item.SourceField].DataType;
					if (type2 == typeof(DateTime))
					{
						dataTable3.Columns.Add(targetField2, typeof(DateTime));
						continue;
					}
				}
				dataTable3.Columns.Add(targetField2, type2);
			}
			dsUdo.Tables.Add(dataTable3);
		}
		return dsUdo;
	}

	private System.Data.DataSet GetUdo(System.Data.DataSet dsUdo, string udoCode, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] map, DataTable source)
	{
		System.Data.DataSet dataSet = dsUdo.Clone();
		var array = (from t in map
			where t.TargetItem != udoCode
			group t by t.TargetItem into t
			select new
			{
				Key = t.Key,
				Items = t
			}).ToArray();
		DataRow dataRow = dataSet.Tables[0].NewRow();
		dataRow.ItemArray = dsUdo.Tables[0].Rows[0].ItemArray;
		dataSet.Tables[0].Rows.Add(dataRow);
		string name = Function.ToString(dsUdo.Tables[0].Rows[0]["TableName"]);
		DataTable dataTable = dataSet.Tables[name];
		DataRow dataRow2 = dataTable.NewRow();
		ContextActionMappingImpl[] array2 = mapHeaderField;
		foreach (ContextActionMappingImpl contextActionMappingImpl in array2)
		{
			if (!string.IsNullOrEmpty(contextActionMappingImpl.SourceField))
			{
				object obj = item[contextActionMappingImpl.SourceField];
				if (item.Table.Columns[contextActionMappingImpl.SourceField].DataType == typeof(DateTime))
				{
					DateTime? dateTime = Function.ParseDateTime(obj);
					if (dateTime.HasValue)
					{
						dataRow2[contextActionMappingImpl.TargetField] = dateTime;
					}
					else
					{
						dataRow2[contextActionMappingImpl.TargetField] = DBNull.Value;
					}
				}
				else
				{
					dataRow2[contextActionMappingImpl.TargetField] = obj;
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(contextActionMappingImpl.DfltValue)) && DBNull.Value == dataRow2[contextActionMappingImpl.TargetField])
			{
				dataRow2[contextActionMappingImpl.TargetField] = contextActionMappingImpl.DfltValue;
			}
		}
		dataTable.Rows.Add(dataRow2);
		ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
		DataRow[] array3 = (from r in source.AsEnumerable()
			where new NTuple<object>(from m in groupcol
				where !string.IsNullOrEmpty(m.SourceField)
				select new
				{
					Name = m.SourceField,
					Value = item[m.SourceField]
				}).Equals(new NTuple<object>(from m in groupcol
				where !string.IsNullOrEmpty(m.SourceField)
				select new
				{
					Name = m.SourceField,
					Value = r[m.SourceField]
				}))
			select (r)).ToArray();
		if (array3.Length != 0)
		{
			var array4 = array;
			foreach (var anon in array4)
			{
				ContextActionMappingImpl[] array5 = anon.Items.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
				DataTable dataTable2 = source.Clone();
				if (array5.Length != 0)
				{
					ContextActionMappingImpl[] groupcolLine = anon.Items.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
					foreach (IGrouping<NTuple<object>, DataRow> item3 in from r in array3
						group r by new NTuple<object>(from mapItem in groupcolLine
							where !string.IsNullOrEmpty(mapItem.SourceField)
							select new
							{
								Name = mapItem.SourceField,
								Value = r[mapItem.SourceField]
							}))
					{
						DataRow dataRow3 = dataTable2.NewRow();
						List<Common.ParameterValue> list = new List<Common.ParameterValue>();
						object[] values = item3.Key.Values;
						for (int num2 = 0; num2 < values.Length; num2++)
						{
							if (values[num2].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item2)
							{
								list.Add(item2);
							}
						}
						array2 = groupcolLine;
						foreach (ContextActionMappingImpl col in array2)
						{
							if (!string.IsNullOrEmpty(col.SourceField))
							{
								Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
								if (parameterValue != null)
								{
									dataRow3[col.SourceField] = parameterValue.Value;
								}
							}
						}
						array2 = array5;
						foreach (ContextActionMappingImpl col2 in array2)
						{
							dataRow3[col2.SourceField] = item3.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
						}
						dataTable2.Rows.Add(dataRow3);
					}
				}
				else
				{
					dataTable2 = array3.CopyToDataTable();
				}
				DataTable dataTable3 = dataSet.Tables[anon.Key];
				if (!dataTable2.IsNotNull())
				{
					continue;
				}
				int num3 = 1;
				foreach (DataRow row in dataTable2.Rows)
				{
					DataRow dataRow5 = dataTable3.NewRow();
					foreach (ContextActionMappingImpl item4 in anon.Items)
					{
						if (!string.IsNullOrEmpty(item4.SourceField))
						{
							if (dataTable3.Columns[item4.TargetField].DataType == typeof(DateTime))
							{
								DateTime? dateTime2 = Function.ParseDateTime(row[item4.SourceField]);
								if (dateTime2.HasValue)
								{
									dataRow5[item4.TargetField] = dateTime2;
								}
								else
								{
									dataRow5[item4.TargetField] = DBNull.Value;
								}
							}
							else
							{
								dataRow5[item4.TargetField] = row[item4.SourceField];
							}
						}
						if (!string.IsNullOrEmpty(Function.ToString(item4.DfltValue)) && DBNull.Value == dataRow5[item4.TargetField])
						{
							dataRow5[item4.TargetField] = item4.DfltValue;
						}
					}
					dataRow5["LineId"] = num3;
					dataTable3.Rows.Add(dataRow5);
					num3++;
				}
			}
		}
		return dataSet;
	}

	private async Task ProcessMappingForecast(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping)
	{
		if (!source.IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
			return;
		}
		DataTable source2 = new DataTable();
		DataTable source3 = new DataTable();
		DataRow[] array = dtMapping.Select("MappingType='F' AND TargetItem='H'");
		if (array.Length != 0)
		{
			source2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='F' AND TargetItem='I'");
		if (array.Length != 0)
		{
			source3 = array.CopyToDataTable();
		}
		ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		List<ContextActionMappingImpl> list = (from t in source3.AsEnumerable()
			where new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToList();
		list.AddRange((from t in source3.AsEnumerable()
			where !new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToArray());
		ContextActionMappingImpl[] mapItemField = list.ToArray();
		source.Clone();
		List<string> lstselect = new List<string>();
		int num = 0;
		for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
		{
			if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
			{
				lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
				num++;
			}
		}
		DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable.IsNotNull())
		{
			dataTable = ForecastTableHelper.Header.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		if (showdoc == "N")
		{
			baseForm.ShowProgress(dataTable.Rows.Count);
		}
		int k = 1;
		foreach (DataRow row in dataTable.Rows)
		{
			List<object> list2 = new List<object>();
			for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
				{
					list2.Add((row[mapHeaderField[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num3].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", lstselect), list2.ToArray()));
			if (array2.Length != 0)
			{
				DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				if (showdoc != "N")
				{
					DataTable openFormDocument = new DataRow[1] { row }.CopyToDataTable();
					frm_AdvanceForecast frm_AdvanceForecast2 = new frm_AdvanceForecast();
					frm_AdvanceForecast2.OpenFormViewMode = Enums.FormViewModeType.Add;
					frm_AdvanceForecast2.OpenFormDocument = openFormDocument;
					frm_AdvanceForecast2.OpenFormLines = dataTable2;
					frm_AdvanceForecast2.MappingDocument = mapHeaderField;
					frm_AdvanceForecast2.MappingLines = mapItemField;
					frm_AdvanceForecast2.MdiParent = baseForm.MdiParent;
					frm_AdvanceForecast2.Show();
				}
				else
				{
					baseForm.SetValueProgress(k);
					k++;
					DataTable dataTable3 = ForecastTableHelper.Header.Clone();
					DataRow dataRow2 = dataTable3.NewRow();
					dataRow2["OriginType"] = "S";
					for (int num4 = 0; num4 < mapHeaderField.Length; num4++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num4].SourceField))
						{
							dataRow2[mapHeaderField[num4].TargetField] = row[mapHeaderField[num4].SourceField];
						}
						if (mapHeaderField[num4].DfltValue != null && DBNull.Value == dataRow2[mapHeaderField[num4].TargetField])
						{
							dataRow2[mapHeaderField[num4].TargetField] = mapHeaderField[num4].DfltValue;
						}
					}
					if (string.IsNullOrEmpty(Function.ToString(dataRow2["FormView"])))
					{
						dataRow2["FormView"] = "D";
					}
					dataTable3.Rows.Add(dataRow2);
					DataTable dataTable4 = ForecastTableHelper.Items.Clone();
					for (int num5 = 0; num5 < dataTable2.Rows.Count; num5++)
					{
						DataRow dataRow3 = dataTable4.NewRow();
						for (int num6 = 0; num6 < mapItemField.Length; num6++)
						{
							if (!string.IsNullOrEmpty(mapItemField[num6].TargetField) && !string.IsNullOrEmpty(mapItemField[num6].SourceField))
							{
								dataRow3[mapItemField[num6].TargetField] = dataTable2.Rows[num5][mapItemField[num6].SourceField];
							}
							if (mapItemField[num6].DfltValue != null && DBNull.Value == dataRow3[mapItemField[num6].TargetField])
							{
								dataRow3[mapItemField[num6].TargetField] = mapItemField[num6].DfltValue;
							}
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataRow3["ItemCode"])))
						{
							dataTable4.Rows.Add(dataRow3);
						}
					}
					string text = Function.ToString(dataRow2["FormView"]);
					DataTable dataTable5 = Pivot(dataTable4, text);
					if (!Function.ParseDateTime(dataTable3.Rows[0]["StartDate"]).HasValue)
					{
						dataTable3.Rows[0]["StartDate"] = dataTable5.AsEnumerable().Min((DataRow t) => Function.ParseDateTime(t["Date"]));
					}
					if (!Function.ParseDateTime(dataTable3.Rows[0]["EndDate"]).HasValue)
					{
						dataTable3.Rows[0]["EndDate"] = dataTable5.AsEnumerable().Max((DataRow t) => Function.ParseDateTime(t["Date"]));
					}
					DateTime? dateTime2 = Function.ParseDateTime(dataTable3.Rows[0]["StartDate"]);
					DateTime? dateTime3 = Function.ParseDateTime(dataTable3.Rows[0]["EndDate"]);
					if (!dateTime2.HasValue)
					{
						WinCommons.Error(Enums.MessageCode.StartDateIsMissing, "");
						return;
					}
					if (!dateTime3.HasValue)
					{
						WinCommons.Error(Enums.MessageCode.EndDateIsMissing, "");
						return;
					}
					DateTime value = dateTime2.Value;
					DateTime value2 = dateTime3.Value;
					if (!(text == "W"))
					{
						if (text == "M")
						{
							dataTable3.Rows[0]["StartDate"] = new DateTime(value.Year, value.Month, 1);
							dataTable3.Rows[0]["EndDate"] = new DateTime(value2.Year, value2.Month, DateTime.DaysInMonth(value2.Year, value2.Month));
						}
					}
					else
					{
						DateTime dateTime4 = value.AddDays((double)(1 - value.DayOfWeek));
						dataTable3.Rows[0]["StartDate"] = dateTime4;
						dataTable3.Rows[0]["EndDate"] = value2.AddDays(6.0);
					}
					System.Data.DataSet dataSet = new System.Data.DataSet();
					dataSet.Tables.AddRange(new DataTable[2]
					{
						dataTable3.Copy(),
						dataTable5.Copy()
					});
					HttpResult httpResult = await _httpClient.AdvanceForecastClient.Save(dataSet);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
			}
			if (lstselect.Count == 0)
			{
				break;
			}
		}
		baseForm.HideProgress(200);
	}

	private DataTable Pivot(DataTable dtLines, string formView)
	{
		DataTable dataTable = dtLines.Copy();
		DataTable dataTable2 = dataTable.Clone();
		List<string> list = new List<string>();
		int num = 0;
		foreach (DataColumn item in from DataColumn t in dataTable.Columns
			where t.ColumnName != "Date" && t.ColumnName != "Quantity"
			select t)
		{
			list.Add("ISNULL(" + item.ColumnName + ",'')='{" + num + "}'");
			num++;
		}
		foreach (DataRow row in dataTable.Rows)
		{
			List<object> list2 = new List<object>();
			foreach (DataColumn item2 in from DataColumn t in dataTable.Columns
				where t.ColumnName != "Date" && t.ColumnName != "Quantity"
				select t)
			{
				list2.Add(Function.ToString(row[item2]));
			}
			DataRow[] array = dataTable.Select(string.Format(string.Join(" AND ", list), list2.ToArray()));
			if (array.Length == 0)
			{
				continue;
			}
			DataRow dataRow2 = dataTable2.Select(string.Format(string.Join(" AND ", list), list2.ToArray())).FirstOrDefault();
			if (dataRow2 == null)
			{
				dataRow2 = dataTable2.NewRow();
				dataRow2.ItemArray = array[0].ItemArray;
				dataTable2.Rows.Add(dataRow2);
			}
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				if (array[num2]["Date"] == null)
				{
					continue;
				}
				DateTime? dateTime = Function.ParseDateTime(array[num2]["Date"]);
				if (dateTime.HasValue)
				{
					string zColumnName = GetZColumnName(Convert.ToDateTime(dateTime), formView);
					if (!dataTable2.Columns.Contains(zColumnName))
					{
						dataTable2.Columns.Add(zColumnName, typeof(double));
					}
					double num3 = Function.ParseDouble(row["Quantity"]);
					dataRow2[zColumnName] = Function.ParseDouble(dataRow2[zColumnName]) + num3;
				}
			}
		}
		return dataTable2;
	}

	private string GetZColumnName(DateTime date, string formView)
	{
		if (!(formView == "W"))
		{
			if (!(formView == "M"))
			{
				return "Q" + Convert.ToDateTime(date).ToString("yyyy.MM.dd");
			}
			DateTime value = new DateTime(date.Year, date.Month, 1);
			return "Q" + Convert.ToDateTime(value).ToString("yyyy.MM.dd");
		}
		DateTime value2 = date.AddDays((double)(1 - date.DayOfWeek));
		return "Q" + Convert.ToDateTime(value2).ToString("yyyy.MM.dd");
	}

	private async Task ProcessEmail(System.Data.DataSet ds, string addressTo, string addressFrom, string addressCc, string addressBcc, string subject, string bodytext, string msgSuccess, string msgSuccessType, DataTable source)
	{
		ReportDocument cryRpt = new ReportDocument();
		try
		{
			addressTo = GetTextFromSource(addressTo, source, ";");
			addressFrom = GetTextFromSource(addressFrom, source, ";");
			addressCc = GetTextFromSource(addressCc, source, ";");
			addressBcc = GetTextFromSource(addressBcc, source, ";");
			subject = GetTextFromSource(subject, source, " ");
			bodytext = GetTextFromSource(bodytext, source, "", replaceHtml: true);
			msgSuccess = GetTextFromSource(msgSuccess, source, "");
			Dictionary<string, Stream> listAttach = new Dictionary<string, Stream>();
			if (ds.Tables.Count > 3)
			{
				DataTable dataTable = ds.Tables[3];
				if (dataTable.IsNotNull())
				{
					string pathFile = string.Concat(Application.StartupPath + "\\Action\\" + GlobalData.DatabaseName + "\\", Path.GetFileName(Function.ToString(dataTable.Rows[0]["FilePath"])));
					string pdfFileName = GetTextFromSource(Function.ToString(dataTable.Rows[0]["FileAlias"]), source, "") + ".pdf";
					List<Common.ParameterValue> list = new List<Common.ParameterValue>();
					if (ds.Tables.Count > 4 && ds.Tables[4].IsNotNull())
					{
						foreach (DataRow row in ds.Tables[4].Rows)
						{
							string text = Function.ToString(row["SourceField"]);
							object value = ((string.IsNullOrEmpty(text) || string.IsNullOrEmpty(Function.ToString(source.Rows[0][text]))) ? row["DfltValue"] : source.Rows[0][text]);
							list.Add(new Common.ParameterValue
							{
								Name = Function.ToString(row["ParamName"]),
								Value = value
							});
						}
					}
					cryRpt = await WinFuntions.ReportToPrinter(pathFile, list);
					Stream value2 = cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
					listAttach[pdfFileName] = value2;
				}
			}
			string text2 = await new SmtpMailClient().SendEmail(addressTo.Split(';').ToList(), subject, bodytext, addressFrom, addressCc.Split(';').ToList(), addressBcc.Split(';').ToList(), listAttach);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
			else if (!string.IsNullOrEmpty(msgSuccess) && !string.IsNullOrEmpty(msgSuccessType) && msgSuccessType != "-")
			{
				if (!(msgSuccessType == "S"))
				{
					if (msgSuccessType == "M")
					{
						XtraMessageBox.Show(msgSuccess);
					}
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.None, msgSuccess);
				}
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			if (cryRpt != null)
			{
				cryRpt.Close();
				cryRpt.Dispose();
			}
		}
	}

	private string GetTextFromSource(string s, DataTable source, string joinstring, bool replaceHtml = false)
	{
		try
		{
			string text = s;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != '$' || i + 1 >= text.Length || text[i + 1] != '[')
				{
					continue;
				}
				string item = "";
				text = text.Substring(i + 2, text.Length - i - 2);
				for (int j = 0; j < text.Length && text[j] != ']'; j++)
				{
					item += text[j];
				}
				if (source.Columns.Contains(item))
				{
					string text2 = string.Join(joinstring, (from t in source.AsEnumerable()
						select Function.ToString(t[item])).Distinct().ToArray());
					if (replaceHtml)
					{
						text2 = text2.Replace(Environment.NewLine, "<br/>");
					}
					s = s.Replace("$[" + item + "]", text2);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.SourceDataDoesNotContainColumn, "", item);
				}
				i = item.Length;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return s;
	}

	protected DataTable ConvertPivotDrillDownDataSourceToDataTable(PivotDrillDownDataSource source)
	{
		DataTable dataTable = new DataTable();
		if (source == null)
		{
			return dataTable;
		}
		foreach (PropertyDescriptor itemProperty in ((ITypedList)source).GetItemProperties((PropertyDescriptor[])null))
		{
			dataTable.Columns.Add(itemProperty.Name, itemProperty.PropertyType);
		}
		for (int i = 0; i < source.RowCount; i++)
		{
			List<object> list = new List<object>();
			foreach (DataColumn column in dataTable.Columns)
			{
				list.Add(source.GetValue(i, column.ColumnName));
			}
			dataTable.Rows.Add(list.ToArray());
		}
		return dataTable;
	}

	private async Task<DataTable> GetMappingSource(System.Data.DataSet ds, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, Enums.ContextActionDataFrom datafrom, string dataString)
	{
		DataTable source = new DataTable();
		switch (sourceType)
		{
		case Enums.ContextActionSourceType.UI:
			switch (range)
			{
			case Enums.ContextActionDataRange.FocusedRow:
				if (_gridsource == null && _pivotGrid == null && _treeView == null && _form != null)
				{
					_gridsource = _form.GetActiveGridView();
					_treeView = _form.GetActiveTreeView();
					_pivotGrid = _form.GetActivePivot();
				}
				if (_gridsource != null)
				{
					DataRow focusedDataRow = _gridsource.GetFocusedDataRow();
					source = new DataRow[1] { focusedDataRow }.CopyToDataTable();
				}
				else if (_pivotGrid != null)
				{
					Point focusedCell = _pivotGrid.Cells.FocusedCell;
					PivotCellEventArgs cellInfo = _pivotGrid.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
					PivotDrillDownDataSource source2 = _pivotGrid.CreateDrillDownDataSource(cellInfo.ColumnIndex, cellInfo.RowIndex);
					source = ConvertPivotDrillDownDataSourceToDataTable(source2);
				}
				else if (_treeView != null)
				{
					DataRow focusedDataRow2 = _treeView.GetFocusedDataRow();
					source = new DataRow[1] { focusedDataRow2 }.CopyToDataTable();
				}
				break;
			case Enums.ContextActionDataRange.SelectedRows:
				if (_gridsource == null && _pivotGrid == null && _treeView == null && _form != null)
				{
					_gridsource = _form.GetActiveGridView();
					_treeView = _form.GetActiveTreeView();
					_pivotGrid = _form.GetActivePivot();
				}
				if (_gridsource != null)
				{
					List<DataRow> listDataSourceSelectedRow = _gridsource.GetListDataSourceSelectedRow();
					if (listDataSourceSelectedRow.Count > 0)
					{
						source = listDataSourceSelectedRow.CopyToDataTable();
					}
				}
				else if (_pivotGrid != null)
				{
					DataTable dataTable8 = new DataTable();
					List<PivotCellEventArgs> list = _pivotGrid.Cells.MultiSelection.SelectedCells.Select((Point pt) => _pivotGrid.Cells.GetCellInfo(pt.X, pt.Y)).ToList();
					List<PivotCellEventArgs> list2 = new List<PivotCellEventArgs>();
					foreach (PivotCellEventArgs ci in list)
					{
						if (ci.ColumnValueType == PivotGridValueType.GrandTotal || ci.ColumnValueType == PivotGridValueType.Total || ci.ColumnValueType == PivotGridValueType.CustomTotal || ci.RowValueType == PivotGridValueType.CustomTotal || ci.RowValueType == PivotGridValueType.GrandTotal || ci.RowValueType == PivotGridValueType.Total || list2.Any((PivotCellEventArgs e) => e.ColumnFieldIndex == ci.ColumnFieldIndex && e.RowFieldIndex == ci.RowFieldIndex))
						{
							continue;
						}
						list2.Add(ci);
						PivotDrillDownDataSource pivotDrillDownDataSource = ci.CreateDrillDownDataSource();
						if (dataTable8.Columns.Count == 0)
						{
							ITypedList typedList = pivotDrillDownDataSource;
							if (typedList == null)
							{
								return dataTable8;
							}
							foreach (PropertyDescriptor itemProperty in typedList.GetItemProperties(null))
							{
								dataTable8.Columns.Add(itemProperty.Name, itemProperty.PropertyType);
							}
						}
						for (int num4 = 0; num4 < pivotDrillDownDataSource.RowCount; num4++)
						{
							List<object> list3 = new List<object>();
							foreach (DataColumn column in dataTable8.Columns)
							{
								list3.Add(pivotDrillDownDataSource.GetValue(num4, column.ColumnName));
							}
							dataTable8.Rows.Add(list3.ToArray());
						}
					}
					source = dataTable8;
				}
				else if (_treeView != null)
				{
					List<DataRow> listDataSourceSelectedRow2 = _treeView.GetListDataSourceSelectedRow();
					if (listDataSourceSelectedRow2.Count > 0)
					{
						source = listDataSourceSelectedRow2.CopyToDataTable();
					}
				}
				break;
			case Enums.ContextActionDataRange.All:
				if (_itemId.StartsWith("B$"))
				{
					if (_form == null || ds.Tables.Count <= 2)
					{
						break;
					}
					DataTable dataTable3 = ds.Tables[2];
					DataTable dataTable4 = ds.Tables[1];
					switch ((Enums.ContextActionActionTypeEx)Function.ParseInt(ds.Tables[0].Rows[0]["ActionTypeEx"]))
					{
					case Enums.ContextActionActionTypeEx.QueryReport:
					case Enums.ContextActionActionTypeEx.DoQuery:
						if (!dataTable4.IsNotNull())
						{
							break;
						}
						dataTable3.Rows.Clear();
						dataTable3.AcceptChanges();
						foreach (DataRow row in dataTable4.Rows)
						{
							DataRow dataRow4 = dataTable3.NewRow();
							dataRow4["SourceField"] = row["SourceField"];
							dataRow4["DfltValue"] = row["DefaultValue"];
							dataRow4["TargetItem"] = row["ParamType"];
							dataRow4["TargetField"] = row["SourceField"];
							dataTable3.Rows.Add(dataRow4);
						}
						break;
					case Enums.ContextActionActionTypeEx.PrintLayout:
					{
						DataTable dataTable6 = ds.Tables[4];
						if (!dataTable6.IsNotNull())
						{
							break;
						}
						dataTable3.Rows.Clear();
						dataTable3.AcceptChanges();
						foreach (DataRow row2 in dataTable6.Rows)
						{
							DataRow dataRow6 = dataTable3.NewRow();
							dataRow6["SourceField"] = row2["SourceField"];
							dataRow6["DfltValue"] = row2["DfltValue"];
							dataRow6["TargetItem"] = row2["ParamType"];
							dataRow6["TargetField"] = row2["SourceField"];
							dataTable3.Rows.Add(dataRow6);
						}
						break;
					}
					case Enums.ContextActionActionTypeEx.HttpRequest:
					{
						DataTable dataTable5 = ds.Tables[5];
						if (dataTable5.IsNotNull())
						{
							foreach (DataRow row3 in dataTable5.Rows)
							{
								DataRow dataRow2 = dataTable3.NewRow();
								dataRow2["SourceField"] = row3["Value"];
								dataRow2["DfltValue"] = row3["DfltValue"];
								dataRow2["TargetItem"] = row3["ItemId"];
								dataRow2["TargetField"] = row3["Name"];
								dataTable3.Rows.Add(dataRow2);
							}
						}
						dataTable3.AcceptChanges();
						break;
					}
					}
					if (!dataTable3.IsNotNull())
					{
						break;
					}
					DataTable dataTable7 = new DataTable();
					foreach (DataRow row4 in dataTable3.Rows)
					{
						string text2 = Function.ToString(row4["SourceField"]);
						string text3;
						Type type;
						if (string.IsNullOrEmpty(text2))
						{
							if (string.IsNullOrEmpty(Function.ToString(row4["DfltValue"])))
							{
								continue;
							}
							text3 = (string)(row4["SourceField"] = string.Format("{0}.{1}", Function.ToString(row4["TargetItem"]), Function.ToString(row4["TargetField"])));
							type = typeof(object);
						}
						else
						{
							string[] array = text2.Split('.');
							string name = array[0];
							string name2 = array[1];
							type = ((_dataset.Tables[name].Columns[name2] == null) ? typeof(object) : _dataset.Tables[name].Columns[name2].DataType);
							text3 = text2;
						}
						if (!dataTable7.Columns.Contains(text3))
						{
							dataTable7.Columns.Add(text3, type);
						}
					}
					DataTable table = _dataset.Tables[0];
					for (int j = 0; j < table.Rows.Count; j++)
					{
						DataRow dataRow8 = dataTable7.NewRow();
						foreach (DataRow item in from t in dataTable3.AsEnumerable()
							where Function.ToString(t["SourceField"]).Split('.')[0] == table.TableName
							select t)
						{
							string text5 = Function.ToString(item["SourceField"]).Split('.')[1];
							dataRow8[$"{table.TableName}.{text5}"] = table.Rows[j][text5];
						}
						dataTable7.Rows.Add(dataRow8);
						dataTable7.AcceptChanges();
					}
					int num = 1;
					for (int count = _dataset.Tables.Count; num < count; num++)
					{
						DataTable childTable = _dataset.Tables[num];
						int count2 = dataTable7.Rows.Count;
						for (int num2 = 0; num2 < count2; num2++)
						{
							for (int num3 = 0; num3 < childTable.Rows.Count; num3++)
							{
								DataRow dataRow9 = dataTable7.NewRow();
								if (num3 == 0)
								{
									dataRow9 = dataTable7.Rows[num2];
								}
								else
								{
									dataRow9.ItemArray = dataTable7.Rows[num2].ItemArray;
								}
								bool flag = false;
								foreach (DataRow item2 in from t in dataTable3.AsEnumerable()
									where Function.ToString(t["SourceField"]).Split('.')[0] == childTable.TableName
									select t)
								{
									string text6 = Function.ToString(item2["SourceField"]).Split('.')[1];
									dataRow9[$"{childTable.TableName}.{text6}"] = childTable.Rows[num3][text6];
									flag = true;
								}
								if (!flag)
								{
									continue;
								}
								foreach (DataRow item3 in from t in dataTable3.AsEnumerable()
									where !string.IsNullOrEmpty(Function.ToString(t["DfltValue"]))
									select t)
								{
									string columnName = (string.IsNullOrEmpty(Function.ToString(item3["SourceField"])) ? string.Format("{0}.{1}", Function.ToString(item3["TargetItem"]), Function.ToString(item3["TargetField"])) : Function.ToString(item3["SourceField"]));
									if (dataRow9[columnName] == DBNull.Value)
									{
										dataRow9[columnName] = item3["DfltValue"];
									}
								}
								if (num3 > 0)
								{
									dataTable7.Rows.Add(dataRow9);
								}
								dataTable7.AcceptChanges();
							}
						}
					}
					source = dataTable7.Copy();
				}
				else
				{
					source = _ui.Copy();
				}
				break;
			}
			break;
		case Enums.ContextActionSourceType.Database:
		{
			string dataParamType = Function.ToString(ds.Tables[0].Rows[0]["DataParamType"]);
			DataTable dtTemporary = new DataTable();
			List<QueryParamater> lstPr = new List<QueryParamater>();
			DataTable dataTable9 = ds.Tables[1];
			if (dataTable9.IsNotNull())
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable9.AsEnumerable()
					where Function.ToString(t["ParamType"]) == "S"
					select t;
				if (dataParamType == "T")
				{
					string text7 = Function.ToString(ds.Tables[0].Rows[0]["TempTblName"]);
					if (string.IsNullOrEmpty(text7))
					{
						text7 = "tbltemp";
					}
					dtTemporary.TableName = text7;
					foreach (DataRow item4 in enumerableRowCollection)
					{
						DataColumn dataColumn2 = new DataColumn();
						string text8 = (dataColumn2.ColumnName = Function.ToString(item4["SourceField"]));
						if (!string.IsNullOrEmpty(text8))
						{
							string text10 = Function.ToString(item4["DefaultValue"]);
							if (!string.IsNullOrEmpty(text10))
							{
								dataColumn2.DefaultValue = text10;
							}
							if (_ui != null && _ui.ExistsColumn(text8))
							{
								dataColumn2.DataType = _ui.Columns[text8].DataType;
							}
							else
							{
								dataColumn2.DataType = typeof(string);
							}
							if (!dtTemporary.ExistsColumn(text8))
							{
								dtTemporary.Columns.Add(dataColumn2);
							}
						}
					}
					SetTemporaryTableValue(dtTemporary, range);
				}
				else
				{
					foreach (DataRow item5 in enumerableRowCollection)
					{
						QueryParamater queryParamater = new QueryParamater
						{
							ParaName = Function.ToString(item5["ParamName"])
						};
						string text11 = Function.ToString(item5["SourceField"]);
						if (!string.IsNullOrEmpty(text11))
						{
							queryParamater.Value = GetValue(text11, sourceType, range, Function.ToString(item5["DefaultValue"]));
						}
						else if (!string.IsNullOrEmpty(Function.ToString(item5["DefaultValue"])))
						{
							queryParamater.Value = item5["DefaultValue"];
						}
						lstPr.Add(queryParamater);
					}
				}
			}
			string text12 = "";
			switch (datafrom)
			{
			case Enums.ContextActionDataFrom.DefinedQuery:
			{
				int queryId = Function.ParseInt(dataString);
				text12 = await _httpClient.Queries.GetQueryText(queryId);
				break;
			}
			case Enums.ContextActionDataFrom.QueryText:
				text12 = dataString;
				break;
			}
			if (string.IsNullOrEmpty(text12))
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetSourceDataDataSourceStringIsEmpty, "");
			}
			text12 = WinFuntions.QueryReplaceUserAndBranch(text12);
			ds = ((!(dataParamType == "T")) ? _httpClient.Commons.ExecuteSql(text12, lstPr) : _httpClient.Commons.ExecuteTemporyTable(text12, dtTemporary, dtTemporary.TableName));
			if (ds != null && ds.Tables.Count > 1)
			{
				DataTable dataTable10 = ds.Tables["Message"];
				if (dataTable10.IsNotNull())
				{
					string text13 = Function.ToString(dataTable10.Rows[0]["MsgType"]);
					string msg = Function.ToString(dataTable10.Rows[0]["Message"]);
					if (text13 == "5xx")
					{
						WinCommons.Error(Enums.MessageCode.None, msg);
					}
				}
				source = ds.Tables[1];
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetSourceData, "");
			}
			break;
		}
		case Enums.ContextActionSourceType.ImportFromExcel:
		{
			DataTable dataTable = new DataTable();
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = $"Excel Files|*.xls;*.xlsx;*.xlsm";
			openFileDialog.FilterIndex = 1;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;
				System.Data.DataSet dataSet = WinFuntions.ReadExcelToDataTable(fileName, Path.GetExtension(fileName).ToLower());
				if (dataSet.IsNotNull())
				{
					dataTable = dataSet.Tables[0];
				}
			}
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = new DataTable();
				if (ds.Tables.Count >= 3)
				{
					dataTable2 = ds.Tables[2];
				}
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					string text = Function.ToString(dataTable2.Rows[i]["SourceField"]);
					if (!string.IsNullOrEmpty(text) && !dataTable.IsExistsColumn(text))
					{
						throw new Exception("Invalid file: columns do not match!");
					}
				}
				source = dataTable.Copy();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetSourceData, "");
			}
			break;
		}
		}
		return source;
	}

	private void SetTemporaryTableValue(DataTable dtTemporary, Enums.ContextActionDataRange range)
	{
		try
		{
			switch (range)
			{
			case Enums.ContextActionDataRange.FocusedRow:
				if (_gridsource != null)
				{
					if (_gridsource.GetFocusedDataRow() == null)
					{
						break;
					}
					DataRow dataRow5 = dtTemporary.NewRow();
					foreach (DataColumn column in dtTemporary.Columns)
					{
						dataRow5[column.ColumnName] = _gridsource.GetFocusedRowCellValue(column.ColumnName);
					}
					dtTemporary.Rows.Add(dataRow5);
				}
				else if (_pivotGrid != null)
				{
					Point focusedCell2 = _pivotGrid.Cells.FocusedCell;
					PivotCellEventArgs cellInfo2 = _pivotGrid.Cells.GetCellInfo(focusedCell2.X, focusedCell2.Y);
					PivotDrillDownDataSource pivotDrillDownDataSource2 = _pivotGrid.CreateDrillDownDataSource(cellInfo2.ColumnIndex, cellInfo2.RowIndex);
					for (int k = 0; k < pivotDrillDownDataSource2.RowCount; k++)
					{
						DataRow dataRow6 = dtTemporary.NewRow();
						foreach (DataColumn column2 in dtTemporary.Columns)
						{
							dataRow6[column2.ColumnName] = pivotDrillDownDataSource2[k][column2.ColumnName];
						}
						dtTemporary.Rows.Add(dataRow6);
					}
				}
				else
				{
					if (_treeView == null)
					{
						break;
					}
					DataRow focusedDataRow = _treeView.GetFocusedDataRow();
					DataRow dataRow7 = dtTemporary.NewRow();
					foreach (DataColumn column3 in dtTemporary.Columns)
					{
						dataRow7[column3.ColumnName] = focusedDataRow[column3.ColumnName];
					}
					dtTemporary.Rows.Add(dataRow7);
				}
				break;
			case Enums.ContextActionDataRange.SelectedRows:
				if (_gridsource != null)
				{
					foreach (DataRow item in _gridsource.GetListDataSourceSelectedRow())
					{
						DataRow dataRow2 = dtTemporary.NewRow();
						foreach (DataColumn column4 in dtTemporary.Columns)
						{
							dataRow2[column4.ColumnName] = item[column4.ColumnName];
						}
						dtTemporary.Rows.Add(dataRow2);
					}
				}
				else if (_pivotGrid != null)
				{
					Point focusedCell = _pivotGrid.Cells.FocusedCell;
					PivotCellEventArgs cellInfo = _pivotGrid.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
					PivotDrillDownDataSource pivotDrillDownDataSource = _pivotGrid.CreateDrillDownDataSource(cellInfo.ColumnIndex, cellInfo.RowIndex);
					for (int j = 0; j < pivotDrillDownDataSource.RowCount; j++)
					{
						DataRow dataRow3 = dtTemporary.NewRow();
						foreach (DataColumn column5 in dtTemporary.Columns)
						{
							dataRow3[column5.ColumnName] = pivotDrillDownDataSource[j][column5.ColumnName];
						}
						dtTemporary.Rows.Add(dataRow3);
					}
				}
				else
				{
					if (_treeView == null)
					{
						break;
					}
					foreach (TreeListNode item2 in _treeView.GetCheckedNodeCollection())
					{
						DataRow dataRow4 = dtTemporary.NewRow();
						foreach (DataColumn column6 in dtTemporary.Columns)
						{
							dataRow4[column6.ColumnName] = item2.GetValue(column6.ColumnName);
						}
						dtTemporary.Rows.Add(dataRow4);
					}
				}
				break;
			case Enums.ContextActionDataRange.All:
			{
				DataTable dataTable = new DataTable();
				if (_gridsource != null)
				{
					dataTable = _gridsource.GridControl.DatasourceTable();
				}
				else if (_pivotGrid != null)
				{
					dataTable = _pivotGrid.DataSourceTable();
				}
				else if (_treeView != null)
				{
					dataTable = _treeView.DataSourceTable();
				}
				if (dataTable == null)
				{
					break;
				}
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dtTemporary.NewRow();
					foreach (DataColumn column7 in dtTemporary.Columns)
					{
						dataRow[column7.ColumnName] = dataTable.Rows[i][column7.ColumnName];
					}
					dtTemporary.Rows.Add(dataRow);
				}
				break;
			}
			}
			if (_gridsource == null && _pivotGrid == null && _treeView == null && _formId != ApzObjectTypes.oDashboardDetail.ToString())
			{
				DataTable dataTable2 = _dataset.Tables[0];
				for (int l = 0; l < dataTable2.Rows.Count; l++)
				{
					DataRow dataRow8 = dtTemporary.NewRow();
					foreach (DataColumn column8 in dataTable2.Columns)
					{
						if (dtTemporary.Columns.Contains(dataTable2.TableName + "." + column8.ColumnName))
						{
							dataRow8[dataTable2.TableName + "." + column8.ColumnName] = dataTable2.Rows[l][column8];
						}
					}
					dtTemporary.Rows.Add(dataRow8);
				}
				int m = 1;
				for (int count = _dataset.Tables.Count; m < count; m++)
				{
					DataTable dataTable3 = _dataset.Tables[m];
					int count2 = dtTemporary.Rows.Count;
					for (int n = 0; n < count2; n++)
					{
						for (int num = 0; num < dataTable3.Rows.Count; num++)
						{
							DataRow dataRow9 = dtTemporary.NewRow();
							if (num == 0)
							{
								dataRow9 = dtTemporary.Rows[n];
							}
							else
							{
								dataRow9.ItemArray = dtTemporary.Rows[n].ItemArray;
							}
							bool flag = false;
							foreach (DataColumn column9 in dataTable3.Columns)
							{
								if (dtTemporary.Columns.Contains(dataTable3.TableName + "." + column9.ColumnName))
								{
									dataRow9[dataTable3.TableName + "." + column9.ColumnName] = dataTable3.Rows[num][column9];
									flag = true;
								}
							}
							if ((num > 0) & flag)
							{
								dtTemporary.Rows.Add(dataRow9);
							}
						}
					}
				}
			}
			dtTemporary.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	private async Task ProcessMappingProduction(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, string objectId, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, Enums.ContextActionMethod method)
	{
		if (!source.IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
			return;
		}
		DataTable source2 = new DataTable();
		DataTable source3 = new DataTable();
		DataTable source4 = new DataTable();
		DataRow[] array = dtMapping.Select("MappingType='W' AND TargetItem='H'");
		if (array.Length != 0)
		{
			source2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='W' AND TargetItem='I'");
		if (array.Length != 0)
		{
			source3 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='W' AND TargetItem='B'");
		if (array.Length != 0)
		{
			source4 = array.CopyToDataTable();
		}
		List<ContextActionMappingImpl> list = (from t in source2.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToList();
		if (method == Enums.ContextActionMethod.Update || method == Enums.ContextActionMethod.AddOrUpdate)
		{
			list.Add(new ContextActionMappingImpl
			{
				SourceField = objectId,
				TargetField = "DocEntry",
				DfltValue = 0
			});
		}
		ContextActionMappingImpl[] mapHeaderField = list.ToArray();
		List<ContextActionMappingImpl> list2 = (from t in source3.AsEnumerable()
			where new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToList();
		list2.AddRange((from t in source3.AsEnumerable()
			where !new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToArray());
		ContextActionMappingImpl[] mapItemField = list2.ToArray();
		ContextActionMappingImpl[] mapBatchField = (from t in source4.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		ContextActionMappingImpl[] array2 = mapHeaderField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable = source.Clone();
		if (array2.Length != 0)
		{
			ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in source.AsEnumerable()
				group r by new NTuple<object>(from map in groupcol
					where !string.IsNullOrEmpty(map.SourceField)
					select new
					{
						Name = map.SourceField,
						Value = r[map.SourceField]
					}))
			{
				DataRow dataRow = dataTable.NewRow();
				List<Common.ParameterValue> list3 = new List<Common.ParameterValue>();
				object[] values = item2.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
					{
						list3.Add(item);
					}
				}
				ContextActionMappingImpl[] array3 = groupcol;
				foreach (ContextActionMappingImpl col in array3)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list3.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array3 = array2;
				foreach (ContextActionMappingImpl col2 in array3)
				{
					dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable.Rows.Add(dataRow);
			}
		}
		if (dataTable.IsNotNull())
		{
			source = dataTable;
		}
		List<string> lstselect = new List<string>();
		List<string> lstselectLine = new List<string>();
		int num2 = 0;
		for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
		{
			if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
			{
				lstselect.Add("ISNULL(" + mapHeaderField[num3].SourceField + ",'')='{" + num2 + "}'");
				num2++;
			}
		}
		num2 = 0;
		for (int num4 = 0; num4 < mapItemField.Length; num4++)
		{
			if (!string.IsNullOrEmpty(mapItemField[num4].SourceField))
			{
				lstselectLine.Add("ISNULL(" + mapItemField[num4].SourceField + ",'')='{" + num2 + "}'");
				num2++;
			}
		}
		dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable.IsNotNull())
		{
			dataTable = ProductionOrderTableHelper.ProductionHeader.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		if (showdoc == "N")
		{
			baseForm.ShowProgress(dataTable.Rows.Count);
		}
		int k = 1;
		foreach (DataRow row in dataTable.Rows)
		{
			List<object> list4 = new List<object>();
			for (int num5 = 0; num5 < mapHeaderField.Length; num5++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num5].SourceField))
				{
					list4.Add((row[mapHeaderField[num5].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num5].SourceField]).Replace("'", "''"));
				}
			}
			DataRow[] array4 = source.Select(string.Format(string.Join(" AND ", lstselect), list4.ToArray()));
			if (array4.Length != 0)
			{
				DataTable dataTable2 = array4.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				if (showdoc != "N")
				{
					DataTable doc = new DataRow[1] { row }.CopyToDataTable();
					frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder();
					frm_ProductionOrder2.SetMapping(doc, dataTable2, mapHeaderField, mapItemField, source, mapBatchField);
					frm_ProductionOrder2.MdiParent = baseForm.MdiParent;
					frm_ProductionOrder2.Show();
				}
				else
				{
					baseForm.SetValueProgress(k);
					k++;
					DataTable dataTable3 = ProductionOrderTableHelper.ProductionHeader.Clone();
					DataRow dataRow3 = dataTable3.NewRow();
					for (int num6 = 0; num6 < mapHeaderField.Length; num6++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num6].SourceField))
						{
							dataRow3[mapHeaderField[num6].TargetField] = row[mapHeaderField[num6].SourceField];
						}
					}
					dataTable3.Rows.Add(dataRow3);
					DataTable dataTable4 = ProductionOrderTableHelper.ProductionLines.Clone();
					for (int num7 = 0; num7 < dataTable2.Rows.Count; num7++)
					{
						DataRow dataRow4 = dataTable4.NewRow();
						dataRow4["LineNum"] = num7;
						for (int num8 = 0; num8 < mapItemField.Length; num8++)
						{
							if (!string.IsNullOrEmpty(mapItemField[num8].TargetField) && !string.IsNullOrEmpty(mapItemField[num8].SourceField))
							{
								dataRow4[mapItemField[num8].TargetField] = dataTable2.Rows[num7][mapItemField[num8].SourceField];
							}
						}
						if (!string.IsNullOrEmpty(Function.ToString(dataRow4["ItemCode"])))
						{
							dataTable4.Rows.Add(dataRow4);
						}
					}
					ProductionOder_Impl_OWOR productionOder_Impl_OWOR = dataRow3.ToProduction();
					if (productionOder_Impl_OWOR == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataProductionHeader, "");
						return;
					}
					productionOder_Impl_OWOR.Line_WOR1 = dataTable4.ToProductionLine();
					if (mapBatchField.Length != 0)
					{
						for (int num9 = 0; num9 < dataTable2.Rows.Count; num9++)
						{
							List<object> list5 = new List<object>();
							for (int num10 = 0; num10 < mapItemField.Length; num10++)
							{
								if (!string.IsNullOrEmpty(mapItemField[num10].SourceField))
								{
									list5.Add(Function.ToString(dataTable2.Rows[num9][mapItemField[num10].SourceField]));
								}
							}
							DataRow[] array5 = source.Select(string.Format(string.Join(" AND ", lstselectLine), list5.ToArray()));
							if (array5.Length == 0)
							{
								continue;
							}
							DataTable dataTable5 = DocumentsHelper.BatchSerialNumbersStruct.Clone();
							for (int num11 = 0; num11 < array5.Length; num11++)
							{
								DataRow dataRow5 = dataTable5.NewRow();
								for (int num12 = 0; num12 < mapBatchField.Length; num12++)
								{
									if (!string.IsNullOrEmpty(mapBatchField[num12].TargetField) && !string.IsNullOrEmpty(mapBatchField[num12].SourceField))
									{
										dataRow5[mapBatchField[num12].TargetField] = array5[num11][mapBatchField[num12].SourceField];
									}
								}
								dataTable5.Rows.Add(dataRow5);
							}
						}
					}
					HttpResult httpResult = await HttpClientHelperFactory.GetHttpClient().ProductionOrders.SaveDoc(productionOder_Impl_OWOR);
					if (httpResult.MessageCode != Enums.MessageCode.Success)
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
			}
			if (lstselect.Count == 0)
			{
				break;
			}
		}
		baseForm.HideProgress(200);
	}

	private List<DocumentsImpl> GetMappingDocument(DataTable source, DataTable dtMapping, Enums.ContextActionDocumentType objtype, Enums.ContextActionMethod method, string objectId)
	{
		GetMappingDocument(ref source, dtMapping, out var mapHeaderField, out var mapItemField, out var mapBatchField);
		if (method == Enums.ContextActionMethod.Update)
		{
			if (string.IsNullOrEmpty(objectId))
			{
				throw new Exception("DocEntry is missing (mapping object id)");
			}
			ContextActionMappingImpl contextActionMappingImpl = mapHeaderField.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "DocEntry");
			if (contextActionMappingImpl != null)
			{
				contextActionMappingImpl.SourceField = objectId;
			}
			else
			{
				ContextActionMappingImpl contextActionMappingImpl2 = new ContextActionMappingImpl();
				contextActionMappingImpl2.TargetField = "DocEntry";
				contextActionMappingImpl2.SourceField = objectId;
				contextActionMappingImpl2.DfltValue = 0;
				List<ContextActionMappingImpl> list = mapHeaderField.ToList();
				list.Add(contextActionMappingImpl2);
				mapHeaderField = list.ToArray();
			}
		}
		List<DocumentsImpl> list2 = new List<DocumentsImpl>();
		DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
			where !string.IsNullOrEmpty(t.SourceField)
			select t.SourceField).Distinct().ToArray());
		if (!dataTable.IsNotNull())
		{
			dataTable = DocumentsHelper.Document.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		foreach (DataRow row in dataTable.Rows)
		{
			DataTable mappingLines = GetMappingLines(source, mapHeaderField, row, mapItemField);
			if (mappingLines.IsNotNull())
			{
				DocumentsImpl document = GetDocument(source, objtype, mapHeaderField, row, mapItemField, mappingLines, mapBatchField, method);
				list2.Add(document);
			}
		}
		return list2;
	}

	private async Task<HttpResult> ProcessMappingDocument(DataTable source, string showdoc, string ignoreError, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionDocumentType objtype, Enums.ContextActionMethod method, string objectId)
	{
		try
		{
			ConcurrentBag<string> lstMess = new ConcurrentBag<string>();
			ConcurrentBag<string> lstEntry = new ConcurrentBag<string>();
			IContextActionHandle _contextActionHandle = ContextActionHandleFactory.GetActionHandle();
			GetMappingDocument(ref source, dtMapping, out var mapHeaderField, out var mapItemField, out var mapBatchField);
			List<string> list = new List<string>();
			int num = 0;
			if (method == Enums.ContextActionMethod.Update)
			{
				if (string.IsNullOrEmpty(objectId))
				{
					return new HttpResult(Enums.MessageCode.Error, "DocEntry is missing (mapping object id)");
				}
				ContextActionMappingImpl contextActionMappingImpl = mapHeaderField.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "DocEntry");
				if (contextActionMappingImpl != null)
				{
					contextActionMappingImpl.SourceField = objectId;
				}
				else
				{
					ContextActionMappingImpl contextActionMappingImpl2 = new ContextActionMappingImpl();
					contextActionMappingImpl2.TargetField = "DocEntry";
					contextActionMappingImpl2.SourceField = objectId;
					contextActionMappingImpl2.DfltValue = 0;
					List<ContextActionMappingImpl> list2 = mapHeaderField.ToList();
					list2.Add(contextActionMappingImpl2);
					mapHeaderField = list2.ToArray();
				}
			}
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					list.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dt = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dt.IsNotNull())
			{
				dt = DocumentsHelper.Document.Clone();
				dt.Rows.Add(dt.NewRow());
			}
			if (showdoc == "N")
			{
				WinCommons._OpenProcessing();
				baseForm.IsCanClose = false;
				int k = 1;
				try
				{
					if (ignoreError.Equals("N"))
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							DataRow item = dt.Rows[i];
							DataTable mappingLines = GetMappingLines(source, mapHeaderField, item, mapItemField);
							if (!mappingLines.IsNotNull())
							{
								continue;
							}
							DocumentsImpl doc = GetDocument(source, objtype, mapHeaderField, item, mapItemField, mappingLines, mapBatchField, method);
							HttpResult ret = new HttpResult();
							if (new Enums.ContextActionDocumentType[2]
							{
								Enums.ContextActionDocumentType.oInventoryTransferRequest,
								Enums.ContextActionDocumentType.oInventoryTransfer
							}.Contains(objtype))
							{
								switch (objtype)
								{
								case Enums.ContextActionDocumentType.oInventoryTransferRequest:
								{
									IHttpClient httpclient = HttpClientHelperFactory.GetHttpClient();
									ret = ((method == Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient.InventoryTransferRequest.SaveAsDraft(doc)) : (await httpclient.InventoryTransferRequest.SaveDoc(doc)));
									if (!string.IsNullOrEmpty(ret.Message))
									{
										WinCommons.Error(Enums.MessageCode.None, ret.Message);
									}
									break;
								}
								case Enums.ContextActionDocumentType.oInventoryTransfer:
								{
									IHttpClient httpclient = HttpClientHelperFactory.GetHttpClient();
									ret = ((method == Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient.StockTransfer.SaveAsDraft(doc)) : (await httpclient.StockTransfer.SaveDoc(doc)));
									if (!string.IsNullOrEmpty(ret.Message))
									{
										WinCommons.Error(Enums.MessageCode.None, ret.Message);
									}
									break;
								}
								}
							}
							else
							{
								ApzObjectTypes objectTypesFromContext = _contextActionHandle.GetObjectTypesFromContext(objtype);
								IHttpClient httpclient = HttpClientHelperFactory.GetHttpClient(objectTypesFromContext);
								if (_files != null)
								{
									doc.AttachmentFiles = _files;
								}
								if (GlobalData.GeneralSetting.useDocWrf == "Y")
								{
									using HttpClientHelper<DataTable> dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, httpclient.Documents.GetFormController());
									if (!(await doc.GetApprovalTemplate(dtClientHelper, (doc.DocEntry > 0) ? 2.ToString() : 3.ToString(), _form)))
									{
										WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
										ret.Message = "Cannot get approval template";
									}
								}
								ret = ((method != Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient.Documents.SaveDoc(doc)) : (await httpclient.Documents.SaveAsDraft(doc)));
							}
							if (!string.IsNullOrEmpty(ret.Message))
							{
								lstMess.Add($"(*{k}*) {ret.Message}");
								break;
							}
							lstEntry.Add(ret.Content.ToString());
							k++;
						}
					}
					else
					{
						List<Task> list3 = new List<Task>();
						for (int num3 = 0; num3 < dt.Rows.Count; num3++)
						{
							DataRow item2 = dt.Rows[num3];
							list3.Add(Task.Run(async () =>
							{
								_ = 6;
								try
								{
									DataTable mappingLines3 = GetMappingLines(source, mapHeaderField, item2, mapItemField);
									if (mappingLines3.IsNotNull())
									{
										DocumentsImpl doc2 = GetDocument(source, objtype, mapHeaderField, item2, mapItemField, mappingLines3, mapBatchField, method);
										HttpResult ret2 = new HttpResult();
										if (new Enums.ContextActionDocumentType[2]
										{
											Enums.ContextActionDocumentType.oInventoryTransferRequest,
											Enums.ContextActionDocumentType.oInventoryTransfer
										}.Contains(objtype))
										{
											switch (objtype)
											{
											case Enums.ContextActionDocumentType.oInventoryTransferRequest:
											{
												IHttpClient httpclient2 = HttpClientHelperFactory.GetHttpClient();
												ret2 = ((method == Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient2.InventoryTransferRequest.SaveAsDraft(doc2)) : (await httpclient2.InventoryTransferRequest.SaveDoc(doc2)));
												if (!string.IsNullOrEmpty(ret2.Message))
												{
													WinCommons.Error(Enums.MessageCode.None, ret2.Message);
												}
												break;
											}
											case Enums.ContextActionDocumentType.oInventoryTransfer:
											{
												IHttpClient httpclient2 = HttpClientHelperFactory.GetHttpClient();
												ret2 = ((method == Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient2.StockTransfer.SaveAsDraft(doc2)) : (await httpclient2.StockTransfer.SaveDoc(doc2)));
												if (!string.IsNullOrEmpty(ret2.Message))
												{
													WinCommons.Error(Enums.MessageCode.None, ret2.Message);
												}
												break;
											}
											}
										}
										else
										{
											ApzObjectTypes objectTypesFromContext2 = _contextActionHandle.GetObjectTypesFromContext(objtype);
											IHttpClient httpclient2 = HttpClientHelperFactory.GetHttpClient(objectTypesFromContext2);
											if (_files != null)
											{
												doc2.AttachmentFiles = _files;
											}
											if (GlobalData.GeneralSetting.useDocWrf == "Y")
											{
												using HttpClientHelper<DataTable> dtClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, httpclient2.Documents.GetFormController());
												if (!(await doc2.GetApprovalTemplate(dtClientHelper2, (doc2.DocEntry > 0) ? 2.ToString() : 3.ToString(), _form)))
												{
													WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
													ret2.Message = "Cannot get approval template";
												}
											}
											ret2 = ((method != Enums.ContextActionMethod.SaveAsDraft) ? (await httpclient2.Documents.SaveDoc(doc2)) : (await httpclient2.Documents.SaveAsDraft(doc2)));
										}
										if (!string.IsNullOrEmpty(ret2.Message))
										{
											lstMess.Add($"(*{k}*) {ret2.Message}");
										}
										else
										{
											lstEntry.Add(ret2.Content.ToString());
										}
										int num5 = k;
										k = num5 + 1;
									}
								}
								catch (Exception ex3)
								{
									lstMess.Add($"(*{k}*) Exception occured.");
									int num5 = k;
									k = num5 + 1;
									Logging.Write("ERROR", ex3);
								}
							}));
						}
						await Task.WhenAll(list3);
					}
				}
				catch (Exception ex)
				{
					lstMess.Add($"(*{k}*) Exception occured.");
					Logging.Write("ERROR", ex);
				}
				finally
				{
					baseForm.IsCanClose = true;
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				foreach (DataRow row in dt.Rows)
				{
					DataTable mappingLines2 = GetMappingLines(source, mapHeaderField, row, mapItemField);
					if (!mappingLines2.IsNotNull())
					{
						continue;
					}
					DataTable dataTable = new DataRow[1] { row }.CopyToDataTable();
					if (new Enums.ContextActionDocumentType[2]
					{
						Enums.ContextActionDocumentType.oGoodsReceipt,
						Enums.ContextActionDocumentType.oGoodsIssue
					}.Contains(objtype))
					{
						int num4 = -1;
						ContextActionMappingImpl mapBaseType = mapItemField.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == "BaseType");
						if (mapBaseType != null)
						{
							if (string.IsNullOrEmpty(mapBaseType.SourceField) && Function.ParseInt(mapBaseType.DfltValue) == 202)
							{
								num4 = 202;
							}
							else if (!string.IsNullOrEmpty(mapBaseType.SourceField))
							{
								if (mappingLines2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t[mapBaseType.SourceField]) == 202) != null)
								{
									num4 = 202;
								}
								else if (Function.ParseInt(mapBaseType.DfltValue) == 202 && !mappingLines2.AsEnumerable().Any((DataRow t) => t[mapBaseType.SourceField] != DBNull.Value))
								{
									num4 = 202;
								}
							}
						}
						switch (objtype)
						{
						case Enums.ContextActionDocumentType.oGoodsReceipt:
						{
							VirFormReceiptIssue virFormReceiptIssue2 = ((num4 != 202) ? new frm_GoodsReceipt() : new frm_ReceiptFromProduction());
							virFormReceiptIssue2.SetMapping(dataTable, mappingLines2, mapHeaderField, mapItemField, source, mapBatchField);
							virFormReceiptIssue2.MdiParent = baseForm.MdiParent;
							virFormReceiptIssue2.Show();
							break;
						}
						case Enums.ContextActionDocumentType.oGoodsIssue:
						{
							VirFormReceiptIssue virFormReceiptIssue = ((num4 != 202) ? new frm_GoodsIssue() : new frm_IssueForProduction());
							virFormReceiptIssue.SetMapping(dataTable, mappingLines2, mapHeaderField, mapItemField, source, mapBatchField);
							virFormReceiptIssue.MdiParent = baseForm.MdiParent;
							virFormReceiptIssue.Show();
							break;
						}
						}
						continue;
					}
					if (new Enums.ContextActionDocumentType[2]
					{
						Enums.ContextActionDocumentType.oInventoryTransferRequest,
						Enums.ContextActionDocumentType.oInventoryTransfer
					}.Contains(objtype))
					{
						switch (objtype)
						{
						case Enums.ContextActionDocumentType.oInventoryTransferRequest:
						{
							frm_InventoryTranferRequest frm_InventoryTranferRequest2 = new frm_InventoryTranferRequest();
							frm_InventoryTranferRequest2.SetMapping(dataTable, mappingLines2, mapHeaderField, mapItemField, source, mapBatchField);
							frm_InventoryTranferRequest2.MdiParent = baseForm.MdiParent;
							frm_InventoryTranferRequest2.Show();
							break;
						}
						case Enums.ContextActionDocumentType.oInventoryTransfer:
						{
							frm_InventoryTranfer frm_InventoryTranfer2 = new frm_InventoryTranfer();
							frm_InventoryTranfer2.SetMapping(dataTable, mappingLines2, mapHeaderField, mapItemField, source, mapBatchField);
							frm_InventoryTranfer2.MdiParent = baseForm.MdiParent;
							frm_InventoryTranfer2.Show();
							break;
						}
						}
						continue;
					}
					VirFormDocument virFormDocument = new VirFormDocument();
					switch (objtype)
					{
					case Enums.ContextActionDocumentType.oQuotations:
						virFormDocument = new frm_SalesQuotationForBatch();
						break;
					case Enums.ContextActionDocumentType.oOrders:
						virFormDocument = new frm_SalesOrder();
						break;
					case Enums.ContextActionDocumentType.oDeliveryNotes:
						virFormDocument = new frm_Delivery();
						break;
					case Enums.ContextActionDocumentType.oReturnRequest:
						virFormDocument = new frm_ReturnRequest();
						break;
					case Enums.ContextActionDocumentType.oReturns:
						virFormDocument = new frm_Return();
						break;
					case Enums.ContextActionDocumentType.oInvoices:
						virFormDocument = new frm_ARInvoices();
						break;
					case Enums.ContextActionDocumentType.oReserveInvoices:
						virFormDocument = new frm_AR_ReserveInvoices();
						break;
					case Enums.ContextActionDocumentType.oCreditNotes:
						virFormDocument = new frm_ARCreditMemos();
						break;
					case Enums.ContextActionDocumentType.oPurchaseRequest:
						virFormDocument = new frm_PurchaseRequest();
						break;
					case Enums.ContextActionDocumentType.oPurchaseQuotations:
						virFormDocument = new frm_PurchaseQuotation();
						break;
					case Enums.ContextActionDocumentType.oPurchaseOrders:
						virFormDocument = new frm_PurchaseOder();
						break;
					case Enums.ContextActionDocumentType.oGoodsReceiptPO:
						virFormDocument = new frm_GoodReceiptPO();
						break;
					case Enums.ContextActionDocumentType.oGoodsReturns:
						virFormDocument = new frm_GoodsReturn();
						break;
					case Enums.ContextActionDocumentType.oAPInvoices:
						virFormDocument = new frm_APInvoices();
						break;
					case Enums.ContextActionDocumentType.oAPCreditNotes:
						virFormDocument = new frm_APCreditMemos();
						break;
					case Enums.ContextActionDocumentType.oAPReserveInvoice:
						virFormDocument = new frm_APReserveInvoices();
						break;
					case Enums.ContextActionDocumentType.oDownPaymentRequest:
					{
						ContextActionMappingImpl contextActionMappingImpl4 = mapHeaderField.FirstOrDefault((ContextActionMappingImpl m) => m.TargetField == "Posted");
						Enums.DownPaymentType downPaymentType2 = Enums.DownPaymentType.dptRequest;
						if (contextActionMappingImpl4 != null)
						{
							if (!string.IsNullOrEmpty(contextActionMappingImpl4.SourceField))
							{
								if (dataTable.Columns.Contains(contextActionMappingImpl4.SourceField))
								{
									downPaymentType2 = (Enums.DownPaymentType)Function.ParseInt(dataTable.Rows[0][contextActionMappingImpl4.SourceField]);
								}
							}
							else
							{
								downPaymentType2 = (Enums.DownPaymentType)Function.ParseInt(contextActionMappingImpl4.DfltValue);
							}
						}
						virFormDocument = ((downPaymentType2 == Enums.DownPaymentType.dptInvoice) ? ((VirFormSales)new frm_DownPaymentInvoice()) : ((VirFormSales)new frm_DownPaymentRequest()));
						break;
					}
					case Enums.ContextActionDocumentType.oPurchaseDownPaymentRequest:
					{
						ContextActionMappingImpl contextActionMappingImpl3 = mapHeaderField.FirstOrDefault((ContextActionMappingImpl m) => m.TargetField == "Posted");
						Enums.DownPaymentType downPaymentType = Enums.DownPaymentType.dptRequest;
						if (contextActionMappingImpl3 != null)
						{
							if (!string.IsNullOrEmpty(contextActionMappingImpl3.SourceField))
							{
								if (dataTable.Columns.Contains(contextActionMappingImpl3.SourceField))
								{
									downPaymentType = (Enums.DownPaymentType)Function.ParseInt(dataTable.Rows[0][contextActionMappingImpl3.SourceField]);
								}
							}
							else
							{
								downPaymentType = (Enums.DownPaymentType)Function.ParseInt(contextActionMappingImpl3.DfltValue);
							}
						}
						virFormDocument = ((downPaymentType == Enums.DownPaymentType.dptInvoice) ? ((VirFormPurchase)new frm_PurchaseDownPaymentInvoice()) : ((VirFormPurchase)new frm_PurchaseDownPaymentRequest()));
						break;
					}
					}
					virFormDocument.OpenFormViewMode = Enums.FormViewModeType.Add;
					virFormDocument.OpenFormDocument = dataTable;
					virFormDocument.OpenFormLines = mappingLines2;
					if (mapBatchField.Length != 0)
					{
						virFormDocument.OpenFormBatchSerial = source;
					}
					virFormDocument.MappingDocument = mapHeaderField;
					virFormDocument.MappingLines = mapItemField;
					virFormDocument.MappingBatchSerial = mapBatchField;
					virFormDocument.MdiParent = baseForm.MdiParent;
					virFormDocument.Show();
				}
			}
			if (lstMess.Any())
			{
				WinCommons.Error(Enums.MessageCode.None, string.Join(Environment.NewLine, lstMess));
				return new HttpResult(Enums.MessageCode.Error, string.Join(Environment.NewLine, lstMess), string.Join(",", lstEntry));
			}
			if (showdoc == "N")
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			return new HttpResult(Enums.MessageCode.None, "", string.Join(",", lstEntry));
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			baseForm.IsCanClose = true;
			throw;
		}
	}

	private static DataTable GetMappingLines(DataTable source, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] mapItemField)
	{
		ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
		DataRow[] array = (from r in source.AsEnumerable()
			where new NTuple<object>(from map in groupcol
				where !string.IsNullOrEmpty(map.SourceField)
				select new
				{
					Name = map.SourceField,
					Value = item[map.SourceField]
				}).Equals(new NTuple<object>(from map in groupcol
				where !string.IsNullOrEmpty(map.SourceField)
				select new
				{
					Name = map.SourceField,
					Value = r[map.SourceField]
				}))
			select (r)).ToArray();
		if (array.Length != 0)
		{
			return array.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
		}
		return new DataTable();
	}

	private void GetMappingDocument(ref DataTable source, DataTable dtMapping, out ContextActionMappingImpl[] mapHeaderField, out ContextActionMappingImpl[] mapItemField, out ContextActionMappingImpl[] mapBatchField)
	{
		if (!source.IsNotNull())
		{
			throw new Exception("Datasource is Empty");
		}
		DataTable source2 = new DataTable();
		DataTable source3 = new DataTable();
		DataTable source4 = new DataTable();
		DataRow[] array = dtMapping.Select("MappingType='D' AND TargetItem='H'");
		if (array.Length != 0)
		{
			source2 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='D' AND TargetItem='I'");
		if (array.Length != 0)
		{
			source3 = array.CopyToDataTable();
		}
		array = dtMapping.Select("MappingType='D' AND TargetItem='B'");
		if (array.Length != 0)
		{
			source4 = array.CopyToDataTable();
		}
		mapHeaderField = (from t in source2.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		List<ContextActionMappingImpl> list = (from t in source3.AsEnumerable()
			where new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToList();
		list.AddRange((from t in source3.AsEnumerable()
			where !new string[1] { "ItemCode" }.Contains(Function.ToString(t["TargetField"]))
			select new ContextActionMappingImpl(t)).ToArray());
		mapItemField = list.ToArray();
		mapBatchField = (from t in source4.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		ContextActionMappingImpl[] source5 = (from t in dtMapping.AsEnumerable()
			select new ContextActionMappingImpl(t)).ToArray();
		ContextActionMappingImpl[] array2 = source5.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable = source.Clone();
		if (array2.Length != 0)
		{
			ContextActionMappingImpl[] groupcol = source5.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in source.AsEnumerable()
				group r by new NTuple<object>(from map in groupcol
					where !string.IsNullOrEmpty(map.SourceField)
					select new
					{
						Name = map.SourceField,
						Value = r[map.SourceField]
					}))
			{
				DataRow dataRow = dataTable.NewRow();
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				object[] values = item2.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
					{
						list2.Add(item);
					}
				}
				ContextActionMappingImpl[] array3 = groupcol;
				foreach (ContextActionMappingImpl col in array3)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list2.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array3 = array2;
				foreach (ContextActionMappingImpl col2 in array3)
				{
					dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable.Rows.Add(dataRow);
			}
		}
		else
		{
			dataTable = source.DefaultView.ToTable(distinct: true, (from t in source5
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
		}
		if (!dataTable.IsNotNull())
		{
			dataTable = DocumentsHelper.Document.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
		}
		dataTable.AcceptChanges();
		if (dataTable.IsNotNull())
		{
			source = dataTable;
		}
	}

	private DocumentsImpl GetDocument(DataTable source, Enums.ContextActionDocumentType objtype, ContextActionMappingImpl[] mapHeaderField, DataRow item, ContextActionMappingImpl[] mapItemField, DataTable line, ContextActionMappingImpl[] mapBatchField, Enums.ContextActionMethod method)
	{
		IContextActionHandle actionHandle = ContextActionHandleFactory.GetActionHandle();
		DataTable dataTable = DocumentsHelper.Document.Clone();
		DocumentRow documentRow = (DocumentRow)dataTable.NewRow();
		for (int i = 0; i < mapHeaderField.Length; i++)
		{
			if (!string.IsNullOrEmpty(mapHeaderField[i].SourceField))
			{
				documentRow[mapHeaderField[i].TargetField] = item[mapHeaderField[i].SourceField];
			}
			else if (!string.IsNullOrEmpty(Function.ToString(mapHeaderField[i].DfltValue)))
			{
				documentRow[mapHeaderField[i].TargetField] = SetDefaultMapping(mapHeaderField, i, dataTable);
			}
		}
		ContextActionMappingImpl[] array = mapItemField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
		DataTable dataTable2 = line.Clone();
		if (array.Length != 0)
		{
			ContextActionMappingImpl[] groupcolLine = mapItemField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
			foreach (IGrouping<NTuple<object>, DataRow> item3 in from r in line.AsEnumerable()
				group r by new NTuple<object>(from map in groupcolLine
					where !string.IsNullOrEmpty(map.SourceField)
					select new
					{
						Name = map.SourceField,
						Value = r[map.SourceField]
					}))
			{
				DataRow dataRow = dataTable2.NewRow();
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				object[] values = item3.Key.Values;
				for (int num = 0; num < values.Length; num++)
				{
					if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item2)
					{
						list.Add(item2);
					}
				}
				ContextActionMappingImpl[] array2 = groupcolLine;
				foreach (ContextActionMappingImpl col in array2)
				{
					if (!string.IsNullOrEmpty(col.SourceField))
					{
						Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
						if (parameterValue != null)
						{
							dataRow[col.SourceField] = parameterValue.Value;
						}
					}
				}
				array2 = array;
				foreach (ContextActionMappingImpl col2 in array2)
				{
					dataRow[col2.SourceField] = item3.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
				}
				dataTable2.Rows.Add(dataRow);
			}
		}
		if (dataTable2.IsNotNull())
		{
			line = dataTable2;
		}
		DocumentsImpl documentsImpl = documentRow.ToDocument();
		documentsImpl.ObjType = (int)objtype;
		if (documentsImpl != null)
		{
			documentsImpl.Lines = new List<Document_LinesImpl>();
			documentsImpl.SpecialLines = new List<Document_SpecialLinesImpl>();
			for (int i2 = 0; i2 < line.Rows.Count; i2++)
			{
				DataRow dataRow2 = DocumentsHelper.Lines.Clone().NewRow();
				if (method == Enums.ContextActionMethod.AddNew || method == Enums.ContextActionMethod.SaveAsDraft)
				{
					dataRow2["LineNum"] = (documentsImpl.Lines.Any() ? (documentsImpl.Lines.Max((Document_LinesImpl t) => t.LineNum) + 1) : 0);
				}
				for (int num2 = 0; num2 < mapItemField.Length; num2++)
				{
					if (!string.IsNullOrEmpty(mapItemField[num2].TargetField) && !string.IsNullOrEmpty(mapItemField[num2].SourceField) && line.Rows[i2][mapItemField[num2].SourceField] != DBNull.Value)
					{
						dataRow2[mapItemField[num2].TargetField] = line.Rows[i2][mapItemField[num2].SourceField];
					}
					if ((mapItemField[num2].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapItemField[num2].DfltValue)) && dataRow2[mapItemField[num2].TargetField] == DBNull.Value) || string.IsNullOrEmpty(mapItemField[num2].SourceField))
					{
						dataRow2[mapItemField[num2].TargetField] = SetDefaultMapping(mapItemField, num2, DocumentsHelper.Lines);
					}
				}
				if (Function.ToString(dataRow2["LineType"]).Equals("T") || Function.ToString(dataRow2["LineType"]).Equals("S"))
				{
					Document_SpecialLinesImpl document_SpecialLinesImpl = new Document_SpecialLinesImpl();
					document_SpecialLinesImpl.AfterLineNumber = (documentsImpl.Lines.Any() ? documentsImpl.Lines[documentsImpl.Lines.Count - 1].LineNum : (-1));
					string text = Function.ToString(dataRow2["LineType"]);
					if (text == "T")
					{
						document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Text;
						document_SpecialLinesImpl.LineText = Function.ToString(dataRow2["ItemCode"]);
					}
					else if (text == "S")
					{
						document_SpecialLinesImpl.LineType = BoDocSpecialLineType.dslt_Subtotal;
						document_SpecialLinesImpl.Subtotal = Function.ParseDouble(dataRow2["LineTotal"]);
						document_SpecialLinesImpl.SubtotalFC = Function.ParseDouble(dataRow2["TotalFrgn"]);
					}
					documentsImpl.SpecialLines.Add(document_SpecialLinesImpl);
				}
				else
				{
					if ((string.IsNullOrEmpty(Function.ToString(dataRow2["ItemCode"])) && documentsImpl.DocType != "S") || (string.IsNullOrEmpty(Function.ToString(dataRow2["Dscription"])) && documentsImpl.DocType == "S"))
					{
						continue;
					}
					List<Document_LinesImpl> list2 = new DataRow[1] { dataRow2 }.CopyToDataTable().ToDocumentLine();
					if (list2.Count != 1)
					{
						continue;
					}
					Document_LinesImpl document_LinesImpl = list2[0];
					if (mapBatchField.Length != 0)
					{
						ContextActionMappingImpl[] grpBatch = mapItemField.Where((ContextActionMappingImpl x) => Function.ToString(x.SumType) == "").ToArray();
						DataRow[] array3 = (from r in source.AsEnumerable()
							where new NTuple<object>(from map in grpBatch
								where !string.IsNullOrEmpty(map.SourceField)
								select new
								{
									Name = map.SourceField,
									Value = line.Rows[i2][map.SourceField]
								}).Equals(new NTuple<object>(from map in grpBatch
								where !string.IsNullOrEmpty(map.SourceField)
								select new
								{
									Name = map.SourceField,
									Value = r[map.SourceField]
								}))
							select (r)).ToArray();
						if (array3.Length != 0)
						{
							DataTable dataTable3 = DocumentsHelper.BatchSerialNumbersStruct.Clone();
							if (!dataTable3.Columns.Contains("ToBinAbs"))
							{
								dataTable3.Columns.Add("ToBinAbs", typeof(int));
							}
							for (int num3 = 0; num3 < array3.Length; num3++)
							{
								DataRow dataRow3 = dataTable3.NewRow();
								dataTable3.Rows.Add(dataRow3);
								for (int num4 = 0; num4 < mapBatchField.Length; num4++)
								{
									if (string.IsNullOrEmpty(mapBatchField[num4].TargetField))
									{
										continue;
									}
									if (!string.IsNullOrEmpty(mapBatchField[num4].SourceField))
									{
										if (dataTable3.Columns[mapBatchField[num4].TargetField].DataType == typeof(DateTime))
										{
											if (Function.ParseDateTime(array3[num3][mapBatchField[num4].SourceField]).HasValue)
											{
												dataRow3[mapBatchField[num4].TargetField] = Function.ParseDateTime(array3[num3][mapBatchField[num4].SourceField]);
											}
											else
											{
												dataRow3[mapBatchField[num4].TargetField] = DBNull.Value;
											}
										}
										else
										{
											dataRow3[mapBatchField[num4].TargetField] = array3[num3][mapBatchField[num4].SourceField];
										}
									}
									if (mapBatchField[num4].DfltValue != null && DBNull.Value == dataRow3[mapBatchField[num4].TargetField])
									{
										if (dataTable3.Columns[mapBatchField[num4].TargetField].DataType == typeof(DateTime))
										{
											if (Function.ParseDateTime(mapBatchField[num4].DfltValue).HasValue)
											{
												dataRow3[mapBatchField[num4].TargetField] = Function.ParseDateTime(mapBatchField[num4].DfltValue);
											}
										}
										else
										{
											dataRow3[mapBatchField[num4].TargetField] = mapBatchField[num4].DfltValue;
										}
									}
									dataRow3.AcceptChanges();
								}
								dataRow3["BaseLineNumber"] = document_LinesImpl.LineNum;
								dataRow3["TranType"] = 1;
							}
							ExtenseionMethods.LoadDataBatch(document_LinesImpl, dataTable3, actionHandle.GetObjectTypesFromContext(objtype));
						}
					}
					documentsImpl.Lines.Add(document_LinesImpl);
				}
			}
			return documentsImpl;
		}
		throw new Exception("Could not get data - document header");
	}

	private object GetValue(string field, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string defaultValue)
	{
		object obj = null;
		if (_gridsource != null && !string.IsNullOrEmpty(field))
		{
			obj = GetValueGridView(field, sourceType, range);
		}
		else if (_pivotGrid != null && !string.IsNullOrEmpty(field))
		{
			obj = GetValuePivot(field, sourceType, range);
		}
		else if (_treeView != null && !string.IsNullOrEmpty(field))
		{
			obj = GetValueTree(field, sourceType, range);
		}
		else if (_formId == ApzObjectTypes.oDashboardDetail.ToString())
		{
			if (field != null)
			{
				obj = _ui.Rows[0][field];
			}
		}
		else if (!string.IsNullOrEmpty(field) && _form != null && _dataset.IsNotNull())
		{
			string[] array = field.Split('.');
			if (array.Length > 1)
			{
				string name = array[0];
				if (_dataset.Tables.Contains(name))
				{
					DataTable dataTable = _dataset.Tables[name];
					string fieldName = array[1];
					if (dataTable.Columns.Contains(fieldName))
					{
						obj = string.Join(",", (from t in dataTable.AsEnumerable()
							select Function.ToString(t[fieldName])).ToArray());
					}
				}
			}
			else if (_dataset.Tables.Count > 0)
			{
				DataTable dataTable2 = _dataset.Tables[0];
				if (dataTable2.Columns.Contains(field))
				{
					obj = string.Join(",", (from t in dataTable2.AsEnumerable()
						select Function.ToString(t[field])).ToArray());
				}
			}
		}
		if (!string.IsNullOrEmpty(Function.ToString(obj)))
		{
			return obj;
		}
		return defaultValue;
	}

	private object GetValuePivot(string field, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range)
	{
		List<string> list = new List<string>();
		Point focusedCell = _pivotGrid.Cells.FocusedCell;
		PivotCellEventArgs cellInfo = _pivotGrid.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
		PivotDrillDownDataSource pivotDrillDownDataSource = _pivotGrid.CreateDrillDownDataSource(cellInfo.ColumnIndex, cellInfo.RowIndex);
		switch (sourceType)
		{
		case Enums.ContextActionSourceType.UI:
			switch (range)
			{
			case Enums.ContextActionDataRange.FocusedRow:
				return pivotDrillDownDataSource[0][field];
			case Enums.ContextActionDataRange.SelectedRows:
				foreach (PivotCellEventArgs item in _pivotGrid.Cells.MultiSelection.SelectedCells.Select((Point pt) => _pivotGrid.Cells.GetCellInfo(pt.X, pt.Y)).ToList())
				{
					PivotDrillDownDataSource pivotDrillDownDataSource2 = item.CreateDrillDownDataSource();
					for (int num = 0; num < pivotDrillDownDataSource2.RowCount; num++)
					{
						string text2 = Function.ToString(pivotDrillDownDataSource2[num][field]);
						if (string.IsNullOrEmpty(text2))
						{
							list.Add(text2);
						}
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			case Enums.ContextActionDataRange.All:
			{
				for (int i = 0; i < _ui.Rows.Count; i++)
				{
					string text = Function.ToString(_ui.Rows[i][field]);
					if (string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			}
			break;
		case Enums.ContextActionSourceType.Database:
			return pivotDrillDownDataSource[0][field];
		}
		return "";
	}

	private object GetValueTree(string field, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range)
	{
		List<string> list = new List<string>();
		switch (sourceType)
		{
		case Enums.ContextActionSourceType.UI:
			switch (range)
			{
			case Enums.ContextActionDataRange.FocusedRow:
				return _treeView.FocusedNode.GetValue(field);
			case Enums.ContextActionDataRange.SelectedRows:
				foreach (TreeListNode item in _treeView.GetCheckedNodeCollection())
				{
					string text3 = Function.ToString(item.GetValue(field));
					if (!string.IsNullOrEmpty(text3))
					{
						list.Add(text3);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			case Enums.ContextActionDataRange.All:
			{
				for (int i = 0; i < _ui.Rows.Count; i++)
				{
					string text2 = Function.ToString(_ui.Rows[i][field]);
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			}
			break;
		case Enums.ContextActionSourceType.Database:
			foreach (TreeListNode item2 in _treeView.GetCheckedNodeCollection())
			{
				string text = Function.ToString(item2.GetValue(field));
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			return string.Join(",", list.Distinct().ToArray());
		}
		return "";
	}

	private object GetValueGridView(string field, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range)
	{
		List<string> list = new List<string>();
		switch (sourceType)
		{
		case Enums.ContextActionSourceType.UI:
			switch (range)
			{
			case Enums.ContextActionDataRange.FocusedRow:
				return _gridsource.GetFocusedRowCellValue(field);
			case Enums.ContextActionDataRange.SelectedRows:
			{
				int[] array = _gridsource.GetSelectedRows();
				foreach (int rowHandle2 in array)
				{
					string text3 = Function.ToString(_gridsource.GetRowCellValue(rowHandle2, field));
					if (!string.IsNullOrEmpty(text3))
					{
						list.Add(text3);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			case Enums.ContextActionDataRange.All:
			{
				for (int num2 = 0; num2 < _ui.Rows.Count; num2++)
				{
					string text2 = Function.ToString(_ui.Rows[num2][field]);
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			}
			break;
		case Enums.ContextActionSourceType.Database:
			switch (range)
			{
			case Enums.ContextActionDataRange.SelectedRows:
			{
				int[] selectedRows = _gridsource.GetSelectedRows();
				if (selectedRows.Length != 0)
				{
					int[] array = selectedRows;
					foreach (int rowHandle in array)
					{
						string text = Function.ToString(_gridsource.GetRowCellValue(rowHandle, field));
						if (!string.IsNullOrEmpty(text))
						{
							list.Add(text);
						}
					}
				}
				else
				{
					DataRow focusedDataRow3 = _gridsource.GetFocusedDataRow();
					if (focusedDataRow3 != null)
					{
						return Function.ToString(focusedDataRow3[field]);
					}
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			case Enums.ContextActionDataRange.FocusedRow:
			{
				DataRow focusedDataRow2 = _gridsource.GetFocusedDataRow();
				if (focusedDataRow2 != null)
				{
					return Function.ToString(focusedDataRow2[field]);
				}
				break;
			}
			case Enums.ContextActionDataRange.All:
			{
				DataTable dataTable = _gridsource.GridControl.DatasourceTable();
				if (dataTable.Rows.Count > 0)
				{
					return string.Join(",", from r in dataTable.AsEnumerable()
						select Function.ToString(r[field]));
				}
				DataRow focusedDataRow = _gridsource.GetFocusedDataRow();
				if (focusedDataRow != null)
				{
					return Function.ToString(focusedDataRow[field]);
				}
				return string.Join(",", list.Distinct().ToArray());
			}
			}
			break;
		}
		return "";
	}

	private void ShowForm(XtraFormEx frm, XtraFormEx baseForm)
	{
		if (baseForm != null && baseForm.MdiParent != null)
		{
			frm.MdiParent = baseForm.MdiParent;
			frm.Show();
		}
		else
		{
			frm.ShowDialog(baseForm);
		}
	}

	private object SetDefaultMapping(ContextActionMappingImpl[] mapHeaderField, int i, DataTable dtDoc)
	{
		if (dtDoc.Columns[mapHeaderField[i].TargetField].DataType == typeof(DateTime) || dtDoc.Columns[mapHeaderField[i].TargetField].DataType == typeof(DateTime?))
		{
			if (Function.ToString(mapHeaderField[i].DfltValue).ToUpper() == "DATETIME.TODAY")
			{
				return DateTime.Today;
			}
			return mapHeaderField[i].DfltValue;
		}
		return mapHeaderField[i].DfltValue;
	}

	private async Task ProcessMappingEvent(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 1;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='A' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dt = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dt.IsNotNull())
			{
				dt = ActivitiesTableHelper.Acivity.Clone();
				dt.Rows.Add(dt.NewRow());
			}
			if (showdoc == "N")
			{
				baseForm.ShowProgress(dt.Rows.Count);
			}
			int k = 1;
			foreach (DataRow row in dt.Rows)
			{
				k++;
				if (showdoc != "N")
				{
					DataTable dataTable = dt.Clone();
					dataTable.ImportRow(row);
					dataTable.AcceptChanges();
					frm_EventManagement frm_EventManagement2 = new frm_EventManagement(Enums.FormViewModeType.Add, mapHeaderField, dataTable);
					frm_EventManagement2.MdiParent = baseForm.MdiParent;
					frm_EventManagement2.Show();
					continue;
				}
				baseForm.SetValueProgress(k);
				DataTable dataTable2 = ActivitiesTableHelper.Acivity.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow2[mapHeaderField[num].TargetField] = row[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && dataRow2[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow2[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["Closed"])))
				{
					dataRow2["Closed"] = "N";
				}
				if (Function.ParseInt(dataRow2["AssignedBy"]) == 0)
				{
					dataRow2["AssignedBy"] = GlobalData.UserID;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["GroupUser"])) && string.IsNullOrEmpty(Function.ToString(dataRow2["GroupEmployee"])))
				{
					dataRow2["GroupUser"] = GlobalData.UserID.ToString();
				}
				dataRow2["Action"] = "T";
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Details"])))
				{
					frm_TaskManagement.SetAutoValueForTask(dataRow2);
					dataTable2.Rows.Add(dataRow2);
					System.Data.DataSet dataSet = new System.Data.DataSet();
					dataSet.Tables.Add(dataTable2.Copy());
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
					HttpResult httpResult = ((method == "U") ? (await httpClient.SubActivities.AddEvent(dataSet)) : (await httpClient.SubActivities.AddEvent(dataSet)));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> ProcessMappingHttpRequest(DataTable source, XtraFormEx baseForm, DataTable dtMapping, System.Data.DataSet dsAction)
	{
		_ = 8;
		try
		{
			DataTable dataTable = dsAction.Tables[0];
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Thông số cấu hình không đúng - Header");
				return new HttpResult(Enums.MessageCode.Error, "Thông số cấu hình không đúng - Header");
			}
			string text = Function.ToString(dataTable.Rows[0]["HttpVerb"]);
			string uri = Function.ToString(dataTable.Rows[0]["HttpUri"]);
			string text2 = Function.ToString(dataTable.Rows[0]["HttpAuthType"]);
			Function.ToString(dataTable.Rows[0]["HttpDecryptPwd"]);
			string text3 = Function.ToString(dataTable.Rows[0]["HttpBodyType"]);
			string responseType = Function.ToString(dataTable.Rows[0]["HttpResponseType"]);
			string logRequest = Function.ToString(dataTable.Rows[0]["LogRequest"]);
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
			HttpClient httpClient = new HttpClient
			{
				Timeout = new TimeSpan(0, 30, 0)
			};
			if (string.IsNullOrEmpty(uri))
			{
				WinCommons.Error(Enums.MessageCode.UriIsMissing, "");
			}
			if (!source.IsNotNull())
			{
				if (source == null || source.Columns.Count == 0)
				{
					source = new DataTable();
					source.Columns.Add("Id", typeof(int));
				}
				source.Rows.Add(source.NewRow());
			}
			if (text2 == "1")
			{
				string text4 = string.Empty;
				string arg = string.Empty;
				DataTable source2 = new DataTable();
				DataRow[] array = dtMapping.Select("TargetItem='A'");
				if (array.Length != 0)
				{
					source2 = array.CopyToDataTable();
				}
				ContextActionMappingImpl[] array2 = (from t in source2.AsEnumerable()
					select new ContextActionMappingImpl(t)).ToArray();
				DataRow dataRow = source.Rows[0];
				for (int num = 0; num < array2.Length; num++)
				{
					if (array2[num].TargetField == "UserName")
					{
						if (!string.IsNullOrEmpty(array2[num].SourceField))
						{
							text4 = Function.ToString(dataRow[array2[num].SourceField]);
						}
						if (array2[num].DfltValue != null && string.IsNullOrEmpty(text4))
						{
							text4 = Function.ToString(array2[num].DfltValue);
						}
					}
					if (array2[num].TargetField == "Password")
					{
						if (!string.IsNullOrEmpty(array2[num].SourceField))
						{
							arg = Function.ToString(dataRow[array2[num].SourceField]);
						}
						if (array2[num].DfltValue != null && string.IsNullOrEmpty(text4))
						{
							arg = Function.ToString(array2[num].DfltValue);
						}
					}
				}
				byte[] bytes = Encoding.UTF8.GetBytes($"{text4}:{arg}");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
			}
			DataTable source3 = new DataTable();
			DataRow[] array3 = dtMapping.Select("TargetItem='H'");
			if (array3.Length != 0)
			{
				source3 = array3.CopyToDataTable();
			}
			ContextActionMappingImpl[] array4 = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			for (int num2 = 0; num2 < array4.Length; num2++)
			{
				object obj = null;
				if (!string.IsNullOrEmpty(array4[num2].SourceField))
				{
					obj = source.Rows[0][array4[num2].SourceField];
				}
				if (array4[num2].DfltValue != null && (obj == DBNull.Value || obj == null))
				{
					obj = array4[num2].DfltValue;
				}
				httpClient.DefaultRequestHeaders.Add(array4[num2].TargetField, Function.ToString(obj));
			}
			string jsonObject = "";
			if (dsAction.Tables[5].IsNotNull())
			{
				int father = (from t in dsAction.Tables[5].AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["JsonType"]))
					select t).Min((DataRow t) => Function.ParseInt(t["FatherNum"]));
				jsonObject = Function.ConvertTableToJson(dsAction.Tables[5], source, father, isArrayObject: false, GlobalData.ApiToken);
				if (logRequest == "Y")
				{
					Logging.Write("TRACE", "json converter:" + jsonObject);
				}
			}
			DataTable source4 = new DataTable();
			DataRow[] array5 = dtMapping.Select("TargetItem='Q'");
			if (array5.Length != 0)
			{
				source4 = array5.CopyToDataTable();
			}
			ContextActionMappingImpl[] array6 = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			if (array6.Length != 0)
			{
				List<string> list = new List<string>();
				for (int num3 = 0; num3 < array6.Length; num3++)
				{
					object obj2 = null;
					if (!string.IsNullOrEmpty(array6[num3].SourceField))
					{
						obj2 = source.Rows[0][array6[num3].SourceField];
					}
					if (array6[num3].DfltValue != null && (obj2 == DBNull.Value || obj2 == null))
					{
						obj2 = array6[num3].DfltValue;
					}
					list.Add(array6[num3].TargetField + "=" + HttpUtility.UrlEncode(Function.ToString(obj2)));
				}
				if (list.Count > 0)
				{
					uri = uri + "?" + string.Join("&", list.ToArray());
				}
			}
			StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
			string httpverb = "get";
			HttpResponseMessage response;
			switch (text)
			{
			case "1":
				switch (text3)
				{
				default:
					response = await httpClient.PostAsync(uri, content);
					httpverb = "post";
					break;
				case "1":
					response = await httpClient.PostAsync(uri, content);
					httpverb = "post";
					break;
				case "2":
					response = await httpClient.PostAsync(uri, content);
					httpverb = "post";
					break;
				case "3":
				{
					string requestUri = uri + HttpUtility.UrlEncode(jsonObject);
					response = await httpClient.PostAsync(requestUri, new StringContent(""));
					httpverb = "post";
					break;
				}
				}
				break;
			case "2":
				response = await httpClient.PutAsync(uri, content);
				httpverb = "put";
				break;
			case "3":
			{
				HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
				{
					Content = content
				};
				response = await httpClient.SendAsync(request);
				break;
			}
			case "4":
				response = await httpClient.DeleteAsync(uri);
				httpverb = "delete";
				break;
			default:
				response = await httpClient.GetAsync(uri);
				break;
			}
			using (response)
			{
				string text5 = await response.Content.ReadAsStringAsync();
				if (logRequest == "Y")
				{
					Logging.Write("WATCH", httpverb + " - " + uri + " - content:" + jsonObject + " - respone:" + text5);
				}
				if (!response.IsSuccessStatusCode)
				{
					return new HttpResult(Enums.MessageCode.Error, $"{(int)response.StatusCode} - {response.StatusCode} - {text5}");
				}
				DataTable dataTable2 = JsonConvert.DeserializeObject<DataTable>(text5, new JsonConverter[1]
				{
					new ApzonJsonClientJsonToDataTableApzConverter<DataTable>()
				});
				if (responseType == "3")
				{
					string text6 = SecurityData.Decrypt_Aes(Function.ToString(dataTable2.Rows[0][0]), GlobalData.ApiToken, out byte[] result);
					if (!string.IsNullOrEmpty(text6))
					{
						return new HttpResult(Enums.MessageCode.Error, text6);
					}
					string text7 = Convertor.Unzip(result, out string unzippedData);
					if (!string.IsNullOrEmpty(text7))
					{
						return new HttpResult(Enums.MessageCode.Error, text7);
					}
					DataTable dataTable3 = JsonConvert.DeserializeObject<DataTable>(unzippedData, new JsonConverter[1]
					{
						new ApzonJsonClientJsonToDataTableApzConverter<DataTable>()
					});
					if (Function.ParseInt(dataTable3.Rows[0]["Status"]) != 0)
					{
						return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable3.Rows[0]["Object"]));
					}
					string text8 = Function.ToString(dataTable3.Rows[0]["Object"]);
					if (logRequest == "Y")
					{
						Logging.Write("TRACE", "Return Object Json String:" + text8);
					}
					DataTable obj3 = JsonConvert.DeserializeObject<DataTable>(text8, new JsonConverter[1]
					{
						new ApzJsonClientObjectToDataTable()
					});
					return new HttpResult(Enums.MessageCode.Success, "", obj3);
				}
				return new HttpResult(Enums.MessageCode.Success, "", JsonConvert.DeserializeObject<DataTable>(text5, new JsonConverter[1]
				{
					new ApzonJsonClientJsonToDataTableApzConverter<DataTable>()
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", dsAction.Tables[0].Rows[0]["AbsEntry"]?.ToString() + " - " + ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<HttpResult> ProcessMappingInternalReconciliations(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return new HttpResult(Enums.MessageCode.DatasourceIsMissing, "No data to process");
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='R' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='R' AND TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapItemField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> lstselect = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				if (InternalReconciliationTableHelper.Header == null)
				{
					System.Data.DataSet structure = _httpClient.InternalReconciliationsClient.GetStructure();
					if (structure == null || structure.Tables.Count != 2)
					{
						WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
						return new HttpResult(Enums.MessageCode.CannotLoadStructure, "Cannot load structure");
					}
					InternalReconciliationTableHelper.Header = structure.Tables[0];
				}
				dataTable = InternalReconciliationTableHelper.Header.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			string message = "";
			WinCommons._OpenProcessing();
			int k = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list = new List<object>();
				for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
					{
						list.Add((row[mapHeaderField[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num3].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", lstselect), list.ToArray()));
				if (array2.Length != 0)
				{
					DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					k++;
					InternalReconciliationImpl doc = new InternalReconciliationImpl();
					for (int num4 = 0; num4 < mapHeaderField.Length; num4++)
					{
						PropertyInfo property = doc.GetType().GetProperty(mapHeaderField[num4].TargetField, BindingFlags.Instance | BindingFlags.Public);
						if (null != property && property.CanWrite)
						{
							if (!string.IsNullOrEmpty(mapHeaderField[num4].SourceField))
							{
								property.SetValue(doc, row[mapHeaderField[num4].SourceField], null);
							}
							if (mapHeaderField[num4].DfltValue != null && (string.IsNullOrEmpty(mapHeaderField[num4].SourceField) || row[mapHeaderField[num4].SourceField] == DBNull.Value))
							{
								property.SetValue(doc, mapHeaderField[num4].DfltValue, null);
							}
						}
					}
					List<InternalReconciliationLineImpl> list2 = new List<InternalReconciliationLineImpl>();
					if (dataTable2.Rows.Count < 100)
					{
						for (int num5 = 0; num5 < dataTable2.Rows.Count; num5++)
						{
							InternalReconciliationLineImpl internalReconciliationLineImpl = new InternalReconciliationLineImpl();
							internalReconciliationLineImpl.LineSeq = num5;
							for (int num6 = 0; num6 < mapItemField.Length; num6++)
							{
								PropertyInfo property2 = internalReconciliationLineImpl.GetType().GetProperty(mapItemField[num6].TargetField, BindingFlags.Instance | BindingFlags.Public);
								if (null != property2 && property2.CanWrite)
								{
									if (!string.IsNullOrEmpty(mapItemField[num6].SourceField))
									{
										property2.SetValue(internalReconciliationLineImpl, dataTable2.Rows[num5][mapItemField[num6].SourceField], null);
									}
									if (mapItemField[num6].DfltValue != null && (string.IsNullOrEmpty(mapItemField[num6].SourceField) || dataTable2.Rows[num5][mapItemField[num6].SourceField] == DBNull.Value))
									{
										property2.SetValue(internalReconciliationLineImpl, mapItemField[num6].DfltValue, null);
									}
								}
							}
							list2.Add(internalReconciliationLineImpl);
						}
					}
					else
					{
						ConcurrentBag<InternalReconciliationLineImpl> lstReconLine = new ConcurrentBag<InternalReconciliationLineImpl>();
						if (!Parallel.ForEach(dataTable2.AsEnumerable(), (DataRow row) =>
						{
							InternalReconciliationLineImpl internalReconciliationLineImpl2 = new InternalReconciliationLineImpl();
							for (int i = 0; i < mapItemField.Length; i++)
							{
								PropertyInfo property3 = internalReconciliationLineImpl2.GetType().GetProperty(mapItemField[i].TargetField, BindingFlags.Instance | BindingFlags.Public);
								if (null != property3 && property3.CanWrite)
								{
									if (!string.IsNullOrEmpty(mapItemField[i].SourceField))
									{
										property3.SetValue(internalReconciliationLineImpl2, row[mapItemField[i].SourceField], null);
									}
									if (mapItemField[i].DfltValue != null && (string.IsNullOrEmpty(mapItemField[i].SourceField) || row[mapItemField[i].SourceField] == DBNull.Value))
									{
										property3.SetValue(internalReconciliationLineImpl2, mapItemField[i].DfltValue, null);
									}
								}
							}
							lstReconLine.Add(internalReconciliationLineImpl2);
						}).IsCompleted)
						{
							throw new Exception("Could not complete action-parallel mapping json");
						}
						list2.AddRange(lstReconLine);
						try
						{
							GC.Collect();
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", ex);
						}
					}
					if (doc != null)
					{
						doc.InternalReconciliationRows = list2;
					}
					HttpResult httpResult = await Task.Run(() => _httpClient.InternalReconciliationsClient.Add(doc));
					if (httpResult.MessageCode != Enums.MessageCode.Success)
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						message = message + httpResult.Message + Environment.NewLine;
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
				if (lstselect.Count == 0)
				{
					break;
				}
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task ProcessMappingItem(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='4' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='4' AND TargetItem='P'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='4' AND TargetItem='W'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapPriceField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapWhsField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> lstselect = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = ItemsTableHelper.Items.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list = new List<object>();
				for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
					{
						list.Add((row[mapHeaderField[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num3].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", lstselect), list.ToArray()));
				if (array2.Length != 0)
				{
					DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapPriceField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					DataTable dataTable3 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapWhsField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					baseForm.SetValueProgress(k);
					k++;
					DataTable dataTable4 = ItemsTableHelper.Items.Clone();
					DataRow dataRow2 = dataTable4.NewRow();
					for (int num4 = 0; num4 < mapHeaderField.Length; num4++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num4].SourceField))
						{
							dataRow2[mapHeaderField[num4].TargetField] = row[mapHeaderField[num4].SourceField];
						}
						if (mapHeaderField[num4].DfltValue != null && dataRow2[mapHeaderField[num4].TargetField] == DBNull.Value)
						{
							dataRow2[mapHeaderField[num4].TargetField] = mapHeaderField[num4].DfltValue;
						}
					}
					dataTable4.Rows.Add(dataRow2);
					DataTable dataTable5 = ItemsTableHelper.ItemPrices.Clone();
					for (int num5 = 0; num5 < dataTable2.Rows.Count; num5++)
					{
						DataRow dataRow3 = dataTable5.NewRow();
						for (int num6 = 0; num6 < mapPriceField.Length; num6++)
						{
							if (!string.IsNullOrEmpty(mapPriceField[num6].TargetField) && !string.IsNullOrEmpty(mapPriceField[num6].SourceField))
							{
								dataRow3[mapPriceField[num6].TargetField] = dataTable2.Rows[num5][mapPriceField[num6].SourceField];
							}
							if (mapPriceField[num6].DfltValue != null && dataRow3[mapPriceField[num6].TargetField] == DBNull.Value)
							{
								dataRow3[mapPriceField[num6].TargetField] = mapPriceField[num6].DfltValue;
							}
						}
						dataTable5.Rows.Add(dataRow3);
					}
					DataTable dataTable6 = ItemsTableHelper.ItemWarehouses.Clone();
					for (int num7 = 0; num7 < dataTable3.Rows.Count; num7++)
					{
						DataRow dataRow4 = dataTable6.NewRow();
						for (int num8 = 0; num8 < mapWhsField.Length; num8++)
						{
							if (!string.IsNullOrEmpty(mapWhsField[num8].TargetField) && !string.IsNullOrEmpty(mapWhsField[num8].SourceField))
							{
								dataRow4[mapWhsField[num8].TargetField] = dataTable2.Rows[num7][mapWhsField[num8].SourceField];
							}
							if (mapWhsField[num8].DfltValue != null && dataRow4[mapWhsField[num8].TargetField] == DBNull.Value)
							{
								dataRow4[mapWhsField[num8].TargetField] = mapWhsField[num8].DfltValue;
							}
						}
						dataTable6.Rows.Add(dataRow4);
					}
					ItemsImpl doc = dataRow2.ToItems();
					if (doc == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataItemGeneral, "");
						return;
					}
					if (doc.validFor != "Y" && doc.frozenFor != "Y")
					{
						doc.validFor = "Y";
					}
					doc.ListITM1 = dataTable5.ToItemPrices();
					doc.OitwLines = dataTable6.ToItemWarehouses();
					doc.APZ_SAVE_ACTION = ((method == "U") ? "U" : "A");
					string text = await HttpClientHelperFactory.GetHttpClient().Items.SaveIgnoreEmpty(doc);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						Logging.Write("TRACE", text + " Item No." + doc.ItemCode);
					}
				}
				if (lstselect.Count == 0)
				{
					break;
				}
			}
			WinCommons.Success(Enums.MessageCode.GenerateItemSuccess, "");
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> ProcessMappingJournalEntry(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			List<JournalEntriesImpl> mappingJournalEntry = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.JournalEntry, JournalEntryTableHelper.JournalEntry, JournalEntryTableHelper.JournalEntryRows).GetMappingJournalEntry(source, dtMapping);
			if (mappingJournalEntry == null)
			{
				Logging.Write("ERROR", "lst null - get mapping journal entry");
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataJornalEntryList, "");
				return new HttpResult(Enums.MessageCode.CouldNotGetDataJornalEntryList, "lst null - get mapping journal entry");
			}
			string message = "";
			WinCommons._OpenProcessing();
			int k = 1;
			foreach (JournalEntriesImpl item in mappingJournalEntry)
			{
				k++;
				HttpResult httpResult = await HttpClientHelperFactory.GetHttpClient().JournalEntry.SaveDoc(item);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					message = message + "\t" + httpResult.Message;
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task ProcessMappingJournalEntryMultiTask(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 2;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='J' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='J' AND TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapItemField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> lstselect = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			Tuple<LoginImpl, int> model = new Tuple<LoginImpl, int>(new LoginImpl
			{
				Doimain = Function.Encrypt(RSA.PublicKey, GlobalData.Domain),
				UserName = Function.Encrypt(RSA.PublicKey, GlobalData.UserName),
				Password = Function.Encrypt(RSA.PublicKey, EncodePassword.DecryptString(GlobalData.OldPassword, RSA.PasswordPhrase)),
				DbName = Function.Encrypt(RSA.PublicKey, GlobalData.DatabaseName),
				ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)),
				BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3))
			}, 51);
			List<string> pool = await HttpClientHelperFactory.GetHttpClient().LoginClient.GetSessionPool(model);
			for (int pageIndex = 0; pageIndex < 50; pageIndex++)
			{
				Stopwatch watchPage = new Stopwatch();
				watchPage.Start();
				int num3 = pageIndex * 20000 + 1;
				int num4 = (pageIndex + 1) * 20000;
				Logging.Write("WATCH", $"Start Add JE Page {num3} - {num4}");
				source = (await _httpClient.Commons.ExecuteQuery($"USP_POST_MAN_JE {num3}, {num4}, 200")).Data;
				DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				if (!dataTable.IsNotNull())
				{
					dataTable = JournalEntryTableHelper.JournalEntry.Clone();
					dataTable.Rows.Add(dataTable.NewRow());
				}
				baseForm.ShowProgress(dataTable.Rows.Count);
				int k = 1;
				int num5 = 0;
				List<JournalEntriesImpl> list = new List<JournalEntriesImpl>();
				foreach (DataRow row in dataTable.Rows)
				{
					num5++;
					Stopwatch watch = new Stopwatch();
					watch.Start();
					List<object> list2 = new List<object>();
					for (int num6 = 0; num6 < mapHeaderField.Length; num6++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num6].SourceField))
						{
							list2.Add((row[mapHeaderField[num6].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num6].SourceField]).Replace("'", "''"));
						}
					}
					DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", lstselect), list2.ToArray()));
					if (array2.Length != 0)
					{
						DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
							where !string.IsNullOrEmpty(t.SourceField)
							select t.SourceField).Distinct().ToArray());
						baseForm.SetValueProgress(k);
						k++;
						DataTable dataTable3 = JournalEntryTableHelper.JournalEntry.Clone();
						DataRow dataRow2 = dataTable3.NewRow();
						for (int num7 = 0; num7 < mapHeaderField.Length; num7++)
						{
							if (!string.IsNullOrEmpty(mapHeaderField[num7].SourceField))
							{
								dataRow2[mapHeaderField[num7].TargetField] = row[mapHeaderField[num7].SourceField];
							}
							if (Function.ToString(mapHeaderField[num7].DfltValue) != "" && dataRow2[mapHeaderField[num7].TargetField] == DBNull.Value)
							{
								dataRow2[mapHeaderField[num7].TargetField] = mapHeaderField[num7].DfltValue;
							}
						}
						dataTable3.Rows.Add(dataRow2);
						DataTable dataTable4 = JournalEntryTableHelper.JournalEntryRows.Clone();
						for (int num8 = 0; num8 < dataTable2.Rows.Count; num8++)
						{
							DataRow dataRow3 = dataTable4.NewRow();
							dataRow3["Line_ID"] = num8;
							for (int num9 = 0; num9 < mapItemField.Length; num9++)
							{
								if (!string.IsNullOrEmpty(mapItemField[num9].TargetField) && !string.IsNullOrEmpty(mapItemField[num9].SourceField))
								{
									dataRow3[mapItemField[num9].TargetField] = dataTable2.Rows[num8][mapItemField[num9].SourceField];
								}
								if (Function.ToString(mapItemField[num9].DfltValue) != "" && dataRow3[mapItemField[num9].TargetField] == DBNull.Value)
								{
									dataRow3[mapItemField[num9].TargetField] = mapItemField[num9].DfltValue;
								}
							}
							dataTable4.Rows.Add(dataRow3);
						}
						JournalEntriesImpl journalEntriesImpl = EntitiesHelper<JournalEntriesImpl>.ToEntity(dataRow2);
						if (journalEntriesImpl == null)
						{
							WinCommons.Error(Enums.MessageCode.CouldNotGetJornalEntryGeneral, "");
							return;
						}
						journalEntriesImpl.Lines = EntitiesHelper<JournalEntries_LinesImpl>.ToEntities(dataTable4);
						IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
						if (num5 <= 50)
						{
							list.Add(journalEntriesImpl);
						}
						else
						{
							list.Add(journalEntriesImpl);
							List<Task<HttpResult>> list3 = new List<Task<HttpResult>>();
							for (int num10 = 0; num10 < list.Count; num10++)
							{
								if (pool.Count > num10)
								{
									list3.Add(httpClient.JournalEntry.SaveDoc(journalEntriesImpl, pool[num10]));
								}
								else
								{
									list3.Add(httpClient.JournalEntry.SaveDoc(journalEntriesImpl));
								}
							}
							await Task.WhenAll(list3.ToArray());
							list = new List<JournalEntriesImpl>();
							watch.Stop();
							Logging.Write("TRACE", "save: " + k + "  time:" + watch.Elapsed.TotalMilliseconds);
							watch.Start();
							num5 = 0;
						}
					}
					if (lstselect.Count == 0)
					{
						break;
					}
				}
				WinCommons.Success(Enums.MessageCode.GenerateJournalEntryCopleted, "");
				baseForm.HideProgress(200);
				watchPage.Stop();
				Logging.Write("TRACE", "End  time:" + watchPage.Elapsed.TotalMinutes);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> ProcessMappingJournalVoucher(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return new HttpResult(Enums.MessageCode.DatasourceIsNull, "Data source is null");
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			new DataTable();
			DataRow[] array = dtMapping.Select("TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapItemField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> lstselect = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = JournalEntryTableHelper.JournalVoucher.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			WinCommons._OpenProcessing();
			int k = 1;
			string message = "";
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list = new List<object>();
				for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
					{
						list.Add((row[mapHeaderField[num3].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[mapHeaderField[num3].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array2 = source.Select(string.Format(string.Join(" AND ", lstselect), list.ToArray()));
				if (array2.Length != 0)
				{
					DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					k++;
					DataTable dataTable3 = JournalEntryTableHelper.JournalVoucher.Clone();
					DataRow dataRow2 = dataTable3.NewRow();
					for (int num4 = 0; num4 < mapHeaderField.Length; num4++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num4].SourceField))
						{
							dataRow2[mapHeaderField[num4].TargetField] = row[mapHeaderField[num4].SourceField];
						}
						if (mapHeaderField[num4].DfltValue != null && dataRow2[mapHeaderField[num4].TargetField] == DBNull.Value)
						{
							dataRow2[mapHeaderField[num4].TargetField] = mapHeaderField[num4].DfltValue;
						}
					}
					dataTable3.Rows.Add(dataRow2);
					DataTable dataTable4 = JournalEntryTableHelper.JournalVoucherRows.Clone();
					if (mapItemField.Length != 0)
					{
						for (int num5 = 0; num5 < dataTable2.Rows.Count; num5++)
						{
							DataRow dataRow3 = dataTable4.NewRow();
							for (int num6 = 0; num6 < mapItemField.Length; num6++)
							{
								if (!string.IsNullOrEmpty(mapItemField[num6].TargetField) && !string.IsNullOrEmpty(mapItemField[num6].SourceField))
								{
									dataRow3[mapItemField[num6].TargetField] = dataTable2.Rows[num5][mapItemField[num6].SourceField];
								}
								if (mapItemField[num6].DfltValue != null && dataRow3[mapItemField[num6].TargetField] == DBNull.Value)
								{
									dataRow3[mapItemField[num6].TargetField] = mapItemField[num6].DfltValue;
								}
							}
							dataTable4.Rows.Add(dataRow3);
						}
					}
					JournalEntriesImpl journalEntriesImpl = EntitiesHelper<JournalEntriesImpl>.ToEntity(dataRow2);
					if (journalEntriesImpl == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataBillOfMaterialGeneral, "");
						return new HttpResult(Enums.MessageCode.DatasourceIsNull, "Could not get data");
					}
					journalEntriesImpl.Lines = EntitiesHelper<JournalEntries_LinesImpl>.ToEntities(dataTable4);
					HttpClientHelperFactory.GetHttpClient();
					HttpResult httpResult;
					using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JournalEntries"))
					{
						httpResult = await http.PostAsync("CreateJournalvoucher", journalEntriesImpl);
					}
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						message = message + "\t" + httpResult.Message;
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						Logging.Write("ERROR", httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, Function.ParseInt(httpResult.Content), "");
					}
				}
				if (lstselect.Count == 0)
				{
					break;
				}
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	public async Task ProcessMappingLandedCosts(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataTable source5 = new DataTable();
			DataRow[] array = dtMapping.Select("TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='C'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='L'");
			if (array.Length != 0)
			{
				source5 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapItemField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapCostField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			(from t in source5.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> lstselect = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < mapHeaderField.Length; num2++)
			{
				if (!string.IsNullOrEmpty(mapHeaderField[num2].SourceField))
				{
					lstselect.Add("ISNULL(" + mapHeaderField[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = CtxTableHelper.PurchaseLandedCosts.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow item in dataTable.Rows)
			{
				ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array2 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				if (array2.Length != 0)
				{
					DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapItemField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					baseForm.SetValueProgress(k);
					k++;
					DataTable dataTable3 = CtxTableHelper.PurchaseLandedCosts.Clone();
					DataRow dataRow = dataTable3.NewRow();
					for (int num3 = 0; num3 < mapHeaderField.Length; num3++)
					{
						if (!string.IsNullOrEmpty(mapHeaderField[num3].SourceField))
						{
							dataRow[mapHeaderField[num3].TargetField] = item[mapHeaderField[num3].SourceField];
						}
						if (mapHeaderField[num3].DfltValue != null && dataRow[mapHeaderField[num3].TargetField] == DBNull.Value)
						{
							try
							{
								dataRow[mapHeaderField[num3].TargetField] = mapHeaderField[num3].DfltValue;
							}
							catch (Exception ex)
							{
								WinCommons.Warning(Enums.MessageCode.Warning, ex.Message);
							}
						}
					}
					dataTable3.Rows.Add(dataRow);
					DataTable dataTable4 = CtxTableHelper.PurchaseLandedCosts_Rows.Clone();
					if (mapItemField.Length != 0)
					{
						for (int num4 = 0; num4 < dataTable2.Rows.Count; num4++)
						{
							DataRow dataRow2 = dataTable4.NewRow();
							for (int num5 = 0; num5 < mapItemField.Length; num5++)
							{
								if (!string.IsNullOrEmpty(mapItemField[num5].TargetField) && !string.IsNullOrEmpty(mapItemField[num5].SourceField))
								{
									dataRow2[mapItemField[num5].TargetField] = dataTable2.Rows[num4][mapItemField[num5].SourceField];
								}
								if (mapItemField[num5].DfltValue != null && dataRow2[mapItemField[num5].TargetField] == DBNull.Value)
								{
									try
									{
										dataRow2[mapItemField[num5].TargetField] = mapItemField[num5].DfltValue;
									}
									catch (Exception ex2)
									{
										WinCommons.Warning(Enums.MessageCode.Warning, ex2.Message);
									}
								}
							}
							dataTable4.Rows.Add(dataRow2);
						}
					}
					DataTable dataTable5 = CtxTableHelper.PurchaseLandedCosts_Costs.Clone();
					DataTable dataTable6 = array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapCostField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					if (mapCostField.Length != 0)
					{
						for (int num6 = 0; num6 < dataTable6.Rows.Count; num6++)
						{
							DataRow dataRow3 = dataTable5.NewRow();
							for (int num7 = 0; num7 < mapCostField.Length; num7++)
							{
								if (!string.IsNullOrEmpty(mapCostField[num7].TargetField) && !string.IsNullOrEmpty(mapCostField[num7].SourceField))
								{
									dataRow3[mapCostField[num7].TargetField] = dataTable6.Rows[num6][mapCostField[num7].SourceField];
								}
								if (mapCostField[num7].DfltValue != null && dataRow3[mapCostField[num7].TargetField] == DBNull.Value)
								{
									try
									{
										dataRow3[mapCostField[num7].TargetField] = mapCostField[num7].DfltValue;
									}
									catch (Exception ex3)
									{
										WinCommons.Warning(Enums.MessageCode.Warning, ex3.Message);
									}
								}
							}
							dataTable5.Rows.Add(dataRow3);
						}
					}
					LandedCostImpl landedCostImpl = EntitiesHelper<LandedCostImpl>.ToEntity(dataRow);
					if (landedCostImpl == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataLandCost, "");
						return;
					}
					landedCostImpl.ItemLines = EntitiesHelper<LandedCost_ItemLineImpl>.ToEntities(dataTable4);
					landedCostImpl.CostLines = EntitiesHelper<LandedCost_CostLineImpl>.ToEntities(dataTable5);
					HttpResult httpResult = await HttpClientHelperFactory.GetHttpClient().LandedCosts.Save(landedCostImpl);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						Logging.Write("ERROR", httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, Function.ParseInt(httpResult.Content), "");
					}
				}
				if (lstselect.Count == 0)
				{
					break;
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex4)
		{
			Logging.Write("ERROR", ex4);
			WinCommons.ErrorLineNumber(ex4);
		}
	}

	private async Task ProcessMappingNote(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 1;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='A' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dt = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dt.IsNotNull())
			{
				dt = ActivitiesTableHelper.Acivity.Clone();
				dt.Rows.Add(dt.NewRow());
			}
			if (showdoc == "N")
			{
				baseForm.ShowProgress(dt.Rows.Count);
			}
			int k = 1;
			foreach (DataRow row in dt.Rows)
			{
				k++;
				if (showdoc != "N")
				{
					DataTable dataTable = dt.Clone();
					dataTable.ImportRow(row);
					dataTable.AcceptChanges();
					frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement(Enums.FormViewModeType.Add, mapHeaderField, dataTable);
					frm_NoteManagement2.MdiParent = baseForm.MdiParent;
					frm_NoteManagement2.Show();
					continue;
				}
				baseForm.SetValueProgress(k);
				DataTable dataTable2 = ActivitiesTableHelper.Acivity.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow2[mapHeaderField[num].TargetField] = row[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && dataRow2[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow2[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["Closed"])))
				{
					dataRow2["Closed"] = "N";
				}
				if (Function.ParseInt(dataRow2["AssignedBy"]) == 0)
				{
					dataRow2["AssignedBy"] = GlobalData.UserID;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["GroupUser"])) && string.IsNullOrEmpty(Function.ToString(dataRow2["GroupEmployee"])))
				{
					dataRow2["GroupUser"] = GlobalData.UserID.ToString();
				}
				dataRow2["Action"] = "T";
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Details"])))
				{
					frm_TaskManagement.SetAutoValueForTask(dataRow2);
					dataTable2.Rows.Add(dataRow2);
					System.Data.DataSet dataSet = new System.Data.DataSet();
					dataSet.Tables.Add(dataTable2.Copy());
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
					HttpResult httpResult = ((method == "U") ? (await httpClient.SubActivities.SaveNote(dataSet)) : (await httpClient.SubActivities.SaveNote(dataSet)));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ProcessMappingOpportunity(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='6' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable source3 = new DataTable();
			DataRow[] array2 = dtMapping.Select("MappingType='6' AND TargetItem='S'");
			if (array2.Length != 0)
			{
				source3 = array2.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapStageField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable source4 = new DataTable();
			DataRow[] array3 = dtMapping.Select("MappingType='6' AND TargetItem='I'");
			if (array3.Length != 0)
			{
				source4 = array3.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapItemField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = OpportunityTableHelper.Opportunity.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow item in dataTable.Rows)
			{
				baseForm.SetValueProgress(k);
				k++;
				DataTable dataTable2 = OpportunityTableHelper.Opportunity.Clone();
				DataRow dataRow = dataTable2.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow[mapHeaderField[num].TargetField] = item[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && dataRow[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array4 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				DataTable dataTable3 = new DataTable();
				if (array4.Length != 0)
				{
					dataTable3 = array4.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapStageField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
				}
				DataTable dataTable4 = OpportunityTableHelper.OpportunityStages.Clone();
				for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
				{
					DataRow dataRow2 = dataTable4.NewRow();
					dataRow2["Line"] = num2;
					for (int num3 = 0; num3 < mapStageField.Length; num3++)
					{
						if (!string.IsNullOrEmpty(mapStageField[num3].TargetField) && !string.IsNullOrEmpty(mapStageField[num3].SourceField) && dataTable3.Rows[num2][mapStageField[num3].SourceField] != DBNull.Value)
						{
							dataRow2[mapStageField[num3].TargetField] = dataTable3.Rows[num2][mapStageField[num3].SourceField];
						}
						if (mapStageField[num3].DfltValue != null && dataRow2[mapStageField[num3].TargetField] == DBNull.Value)
						{
							dataRow2[mapStageField[num3].TargetField] = mapStageField[num3].DfltValue;
						}
					}
					dataTable4.Rows.Add(dataRow2);
				}
				ContextActionMappingImpl[] groupcolBudget = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array5 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcolBudget
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				DataTable dataTable5 = new DataTable();
				if (array5.Length != 0)
				{
					dataTable5 = array5.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in mapItemField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
				}
				DataTable dataTable6 = OpportunityTableHelper.OpportunityItems.Clone();
				for (int num4 = 0; num4 < dataTable5.Rows.Count; num4++)
				{
					DataRow dataRow3 = dataTable6.NewRow();
					for (int num5 = 0; num5 < mapItemField.Length; num5++)
					{
						if (!string.IsNullOrEmpty(mapItemField[num5].TargetField) && !string.IsNullOrEmpty(mapItemField[num5].SourceField) && dataTable5.Rows[num4][mapItemField[num5].SourceField] != DBNull.Value)
						{
							dataRow3[mapItemField[num5].TargetField] = dataTable5.Rows[num4][mapItemField[num5].SourceField];
						}
						if (mapItemField[num5].DfltValue != null && dataRow3[mapItemField[num5].TargetField] == DBNull.Value)
						{
							dataRow3[mapItemField[num5].TargetField] = mapItemField[num5].DfltValue;
						}
					}
					dataRow3["Line"] = num4;
					dataTable6.Rows.Add(dataRow3);
				}
				if (showdoc != "N")
				{
					dataTable2.Rows.Add(dataRow);
					frm_SalesOpportunity frm_SalesOpportunity2 = new frm_SalesOpportunity();
					frm_SalesOpportunity2.MdiParent = baseForm.MdiParent;
					frm_SalesOpportunity2.Show();
					continue;
				}
				dataTable2.Rows.Add(dataRow);
				dataTable2.TableName = "OOPR";
				System.Data.DataSet dataSet = new System.Data.DataSet();
				dataSet.Tables.Add(dataTable2.Copy());
				dataTable4.TableName = "OPR1";
				dataSet.Tables.Add(dataTable4.Copy());
				DataTable dataTable7 = OpportunityTableHelper.Competitors.Clone();
				dataTable7.TableName = "OPR3";
				dataSet.Tables.Add(dataTable7.Copy());
				DataTable dataTable8 = OpportunityTableHelper.Partner.Clone();
				dataTable8.TableName = "OPR2";
				dataSet.Tables.Add(dataTable8.Copy());
				DataTable dataTable9 = OpportunityTableHelper.Reason.Clone();
				dataTable9.TableName = "OPR5";
				dataSet.Tables.Add(dataTable9.Copy());
				dataTable6.TableName = "OPR6";
				dataSet.Tables.Add(dataTable6.Copy());
				DataTable dataTable10 = OpportunityTableHelper.LinkActivity.Clone();
				dataTable10.TableName = "Activity";
				dataSet.Tables.Add(dataTable10.Copy());
				DataTable dataTable11 = OpportunityTableHelper.Attachment.Clone();
				dataTable11.TableName = "ATC1";
				dataSet.Tables.Add(dataTable11.Copy());
				DataTable dataTable12 = OpportunityTableHelper.Interest.Clone();
				dataTable12.TableName = "OPR4";
				dataSet.Tables.Add(dataTable12.Copy());
				string text = await _httpClient.OpportunityClient.SaveDoc(new Tuple<string, System.Data.DataSet>(method, dataSet));
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> ProcessMappingPayment(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "", string objtype = "")
	{
		_ = 3;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return new HttpResult(Enums.MessageCode.DatasourceIsNull, "Datasource is null");
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataTable source5 = new DataTable();
			DataRow[] array = dtMapping.Select("TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='A'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='C'");
			if (array.Length != 0)
			{
				source5 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapItemField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapAccountField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapCheckField = (from t in source5.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = IncommingPaymentTableHelper.Payment.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			WinCommons._OpenProcessing();
			string message = "";
			int k = 1;
			foreach (DataRow item in dataTable.Rows)
			{
				ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array2 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				if (array2.Length == 0)
				{
					continue;
				}
				DataTable dataTable2 = array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapItemField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				k++;
				DataTable dataTable3 = IncommingPaymentTableHelper.Payment.Clone();
				DataRow dataRow = dataTable3.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow[mapHeaderField[num].TargetField] = item[mapHeaderField[num].SourceField];
					}
					if (!string.IsNullOrEmpty(Function.ToString(mapHeaderField[num].DfltValue)) && dataRow[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				dataTable3.Rows.Add(dataRow);
				DataTable dataTable4 = IncommingPaymentTableHelper.PaymentInvoices.Clone();
				if (mapItemField.Length != 0)
				{
					for (int num2 = 0; num2 < dataTable2.Rows.Count; num2++)
					{
						DataRow dataRow2 = dataTable4.NewRow();
						for (int num3 = 0; num3 < mapItemField.Length; num3++)
						{
							if (!string.IsNullOrEmpty(mapItemField[num3].TargetField) && !string.IsNullOrEmpty(mapItemField[num3].SourceField))
							{
								dataRow2[mapItemField[num3].TargetField] = dataTable2.Rows[num2][mapItemField[num3].SourceField];
							}
							if (!string.IsNullOrEmpty(Function.ToString(mapItemField[num3].DfltValue)) && dataRow2[mapItemField[num3].TargetField] == DBNull.Value)
							{
								dataRow2[mapItemField[num3].TargetField] = mapItemField[num3].DfltValue;
							}
						}
						dataTable4.Rows.Add(dataRow2);
					}
				}
				DataTable dataTable5 = array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapAccountField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable6 = IncommingPaymentTableHelper.PaymentAccounts.Clone();
				if (mapAccountField.Length != 0)
				{
					for (int num4 = 0; num4 < dataTable5.Rows.Count; num4++)
					{
						DataRow dataRow3 = dataTable6.NewRow();
						for (int num5 = 0; num5 < mapAccountField.Length; num5++)
						{
							if (!string.IsNullOrEmpty(mapAccountField[num5].TargetField) && !string.IsNullOrEmpty(mapAccountField[num5].SourceField))
							{
								dataRow3[mapAccountField[num5].TargetField] = dataTable5.Rows[num4][mapAccountField[num5].SourceField];
							}
							if (!string.IsNullOrEmpty(Function.ToString(mapAccountField[num5].DfltValue)) && dataRow3[mapAccountField[num5].TargetField] == DBNull.Value)
							{
								dataRow3[mapAccountField[num5].TargetField] = mapAccountField[num5].DfltValue;
							}
						}
						dataTable6.Rows.Add(dataRow3);
					}
				}
				DataTable dataTable7 = array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapCheckField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				DataTable dataTable8 = IncommingPaymentTableHelper.PaymentChecks.Clone();
				if (mapCheckField.Length != 0)
				{
					for (int num6 = 0; num6 < dataTable7.Rows.Count; num6++)
					{
						DataRow dataRow4 = dataTable8.NewRow();
						for (int num7 = 0; num7 < mapCheckField.Length; num7++)
						{
							if (!string.IsNullOrEmpty(mapCheckField[num7].TargetField) && !string.IsNullOrEmpty(mapCheckField[num7].SourceField))
							{
								dataRow4[mapCheckField[num7].TargetField] = dataTable7.Rows[num6][mapCheckField[num7].SourceField];
							}
							if (!string.IsNullOrEmpty(Function.ToString(mapCheckField[num7].DfltValue)) && dataRow4[mapCheckField[num7].TargetField] == DBNull.Value)
							{
								dataRow4[mapCheckField[num7].TargetField] = mapCheckField[num7].DfltValue;
							}
						}
						dataTable8.Rows.Add(dataRow4);
					}
				}
				IncomingPaymentsImpl incomingPaymentsImpl = EntitiesHelper<IncomingPaymentsImpl>.ToEntity(dataRow);
				if (incomingPaymentsImpl != null)
				{
					incomingPaymentsImpl.Invoices = EntitiesHelper<Payments_InvoicesImpl>.ToEntities(dataTable4);
					incomingPaymentsImpl._listPurpose = EntitiesHelper<Payments_AccountsImpl>.ToEntities(dataTable6);
					incomingPaymentsImpl._listCheck = EntitiesHelper<Payments_ChecksImpl>.ToEntities(dataTable8);
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
					HttpResult httpResult = new HttpResult();
					switch (objtype)
					{
					case "24":
						httpResult = await httpClient.IncommingPayments.Save(incomingPaymentsImpl);
						break;
					case "46":
					{
						using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "OutgoingPayment"))
						{
							httpResult = await http.PostAsync("SaveDoc", incomingPaymentsImpl);
						}
						break;
					}
					case "2400":
					{
						using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "IncomingPayment"))
						{
							httpResult = await http.PostAsync("SaveAsDraft", incomingPaymentsImpl);
						}
						break;
					}
					case "4600":
					{
						using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "OutgoingPayment"))
						{
							httpResult = await http.PostAsync("SaveAsDraft", incomingPaymentsImpl);
						}
						break;
					}
					default:
						httpResult = new HttpResult(Enums.MessageCode.DataMappingIsMissing, "The objtype has not been mapped");
						break;
					}
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						message = message + "\t" + httpResult.Message;
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, Function.ParseInt(httpResult.Content), "");
					}
					continue;
				}
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataPayment, "");
				return new HttpResult(Enums.MessageCode.CouldNotGetDataPayment, "Could not get data payment.");
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	public async Task<HttpResult> ProcessPrintLayout(DataTable source, System.Data.DataSet ds, XtraFormEx baseForm)
	{
		try
		{
			ReportDocument reportDocument = new ReportDocument();
			string filename = string.Concat(Application.StartupPath + "\\Action\\" + GlobalData.DatabaseName + "\\", Path.GetFileName(Function.ToString(ds.Tables[0].Rows[0]["FilePath"])));
			string text = Function.ToString(ds.Tables[0].Rows[0]["RptUseSource"]);
			string text2 = Function.ToString(ds.Tables[0].Rows[0]["PrintAction"]);
			reportDocument.Load(filename);
			try
			{
				ISCDReportClientDocument reportClientDocument = reportDocument.ReportClientDocument;
				if (text == "I")
				{
					reportDocument.Refresh();
					reportDocument.SetDataSource(source);
					reportDocument.Refresh();
					if (text2 == "P")
					{
						reportDocument.PrintToPrinter(1, collated: true, 1, reportDocument.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
						foreach (CrystalDecisions.CrystalReports.Engine.Table table2 in reportDocument.Database.Tables)
						{
							table2.Dispose();
						}
						reportDocument.Database.Dispose();
						reportDocument.Close();
						reportDocument.Dispose();
						GC.Collect();
					}
					else
					{
						frm_ReportViewer frm_ReportViewer2 = new frm_ReportViewer();
						if (ds.Tables.Count > 1)
						{
							frm_ReportViewer2.IsDataSet = true;
						}
						reportDocument.Refresh();
						frm_ReportViewer2._rp = reportDocument;
						if (baseForm != null)
						{
							frm_ReportViewer2.MdiParent = baseForm.MdiParent;
							frm_ReportViewer2.MinimizeBox = true;
							frm_ReportViewer2.Show();
						}
						else
						{
							frm_ReportViewer2.ShowDialog();
						}
					}
				}
				else
				{
					string text3 = "";
					foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table table3 in reportClientDocument.DatabaseController.Database.Tables)
					{
						if (table3 is CommandTable)
						{
							text3 = ((CommandTable)table3).CommandText;
							break;
						}
					}
					DataTable dataTable = new DataTable();
					DataTable dataTable2 = new DataTable();
					if (ds.Tables.Count > 4)
					{
						dataTable2 = ds.Tables[4];
					}
					List<string> list = new List<string>();
					if (dataTable2.IsNotNull())
					{
						list = (from t in dataTable2.AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(t["SourceField"]))
							select Function.ToString(t["SourceField"])).ToList();
					}
					DataTable dataTable3 = ((list.Count <= 0) ? source.Copy() : source.DefaultView.ToTable("SOURCE", distinct: true, list.ToArray()));
					for (int num = 0; num < dataTable3.Rows.Count; num++)
					{
						string text4 = text3;
						if (dataTable2.IsNotNull())
						{
							foreach (object parameterField in reportDocument.DataDefinition.ParameterFields)
							{
								if (!(parameterField is ParameterFieldDefinition))
								{
									continue;
								}
								ParameterFieldDefinition parameterFieldDefinition = (ParameterFieldDefinition)parameterField;
								string prname = parameterFieldDefinition.Name;
								if (parameterFieldDefinition.Name.ToLower().Equals("schema@"))
								{
									text4 = text4.Replace(parameterFieldDefinition.FormulaName, GlobalData.DatabaseName);
									continue;
								}
								if (parameterFieldDefinition.Name.ToLower().Equals("printbyusername@"))
								{
									text4 = text4.Replace(parameterFieldDefinition.FormulaName, GlobalData.UserNameLogin);
									continue;
								}
								if (parameterFieldDefinition.Name.ToLower().Equals("usercode@"))
								{
									text4 = text4.Replace(parameterFieldDefinition.FormulaName, GlobalData.UserName);
									continue;
								}
								DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ParamName"]) == prname);
								if (dataRow == null)
								{
									continue;
								}
								string text5 = "";
								if (!string.IsNullOrEmpty(Function.ToString(dataRow["SourceField"])))
								{
									if (parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateParameter || parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateTimeParameter)
									{
										DateTime? dateTime = Function.ParseDateTime(dataTable3.Rows[num][Function.ToString(dataRow["SourceField"])]);
										if (dateTime.HasValue)
										{
											text5 = dateTime?.ToString((parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateParameter) ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss");
										}
									}
									else
									{
										text5 = Function.ToString(dataTable3.Rows[num][Function.ToString(dataRow["SourceField"])]);
									}
								}
								if (!string.IsNullOrEmpty(text5))
								{
									text4 = text4.Replace("'" + parameterFieldDefinition.FormulaName + "'", "'" + text5 + "'");
									if (parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateParameter || parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateTimeParameter)
									{
										text5 = "'" + text5 + "'";
									}
									text4 = text4.Replace(parameterFieldDefinition.FormulaName, text5);
									continue;
								}
								string text6 = Function.ToString(dataRow["DfltValue"]);
								text4 = text4.Replace("'" + parameterFieldDefinition.FormulaName + "'", "'" + text6 + "'");
								if (text6 != "" && (parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateParameter || parameterFieldDefinition.ParameterValueKind == ParameterValueKind.DateTimeParameter))
								{
									text6 = "'" + text6 + "'";
								}
								text4 = text4.Replace(parameterFieldDefinition.FormulaName, (text6 != "") ? text6 : "null");
							}
						}
						System.Data.DataSet dataSet = HttpClientHelperFactory.GetHttpClient().Commons.ExecuteSql(text4);
						if (dataSet == null || dataSet.Tables.Count <= 1)
						{
							continue;
						}
						DataTable dataTable4 = dataSet.Tables["Message"];
						if (dataTable4.IsNotNull())
						{
							string text7 = Function.ToString(dataTable4.Rows[0]["MsgType"]);
							string text8 = Function.ToString(dataTable4.Rows[0]["Message"]);
							if (!(text7 == "200") && text7 == "5xx")
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text8, text4);
								WinCommons.Error(Enums.MessageCode.None, text8);
							}
						}
						if (dataTable.IsNotNull())
						{
							dataTable.Merge(dataSet.Tables[1].Copy());
						}
						else
						{
							dataTable = dataSet.Tables[1].Copy();
						}
					}
					reportDocument.SetDataSource(dataTable);
					if (text2 == "P")
					{
						reportDocument.Refresh();
						reportDocument.PrintToPrinter(1, collated: true, 1, reportDocument.FormatEngine.GetLastPageNumber(new ReportPageRequestContext()));
						foreach (CrystalDecisions.CrystalReports.Engine.Table table4 in reportDocument.Database.Tables)
						{
							table4.Dispose();
						}
						reportDocument.Database.Dispose();
						reportDocument.Close();
						reportDocument.Dispose();
						GC.Collect();
					}
					else
					{
						frm_ReportViewer frm_ReportViewer3 = new frm_ReportViewer();
						if (ds.Tables.Count > 1)
						{
							frm_ReportViewer3.IsDataSet = true;
						}
						reportDocument.Refresh();
						frm_ReportViewer3._rp = reportDocument;
						if (baseForm != null)
						{
							frm_ReportViewer3.MdiParent = baseForm.MdiParent;
							frm_ReportViewer3.MinimizeBox = true;
							frm_ReportViewer3.Show();
						}
						else
						{
							frm_ReportViewer3.ShowDialog();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
			reportDocument.Refresh();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		return new HttpResult();
	}

	private void ProcessMappingProductTree(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='T' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("MappingType='T' AND TargetItem='I'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] array2 = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] array3 = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			List<string> list = new List<string>();
			int num = 0;
			for (int num2 = 0; num2 < array2.Length; num2++)
			{
				if (!string.IsNullOrEmpty(array2[num2].SourceField))
				{
					list.Add("ISNULL(" + array2[num2].SourceField + ",'')='{" + num + "}'");
					num++;
				}
			}
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in array2
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = ProductTreeTableHelper.ProductTreeHeader.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int num3 = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				List<object> list2 = new List<object>();
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					if (!string.IsNullOrEmpty(array2[num4].SourceField))
					{
						list2.Add((row[array2[num4].SourceField] is DateTime dateTime) ? dateTime.ToString("yyyy-MM-dd") : Function.ToString(row[array2[num4].SourceField]).Replace("'", "''"));
					}
				}
				DataRow[] array4 = source.Select(string.Format(string.Join(" AND ", list), list2.ToArray()));
				if (array4.Length != 0)
				{
					DataTable dataTable2 = array4.CopyToDataTable().DefaultView.ToTable(distinct: false, (from t in array3
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					baseForm.SetValueProgress(num3);
					num3++;
					DataTable dataTable3 = ProductTreeTableHelper.ProductTreeHeader.Clone();
					DataRow dataRow2 = dataTable3.NewRow();
					for (int num5 = 0; num5 < array2.Length; num5++)
					{
						if (!string.IsNullOrEmpty(array2[num5].SourceField))
						{
							dataRow2[array2[num5].TargetField] = row[array2[num5].SourceField];
						}
						if (array2[num5].DfltValue != null && dataRow2[array2[num5].TargetField] == DBNull.Value)
						{
							dataRow2[array2[num5].TargetField] = array2[num5].DfltValue;
						}
					}
					dataTable3.Rows.Add(dataRow2);
					DataTable dataTable4 = ProductTreeTableHelper.ProductTreeLines.Clone();
					for (int num6 = 0; num6 < dataTable2.Rows.Count; num6++)
					{
						DataRow dataRow3 = dataTable4.NewRow();
						dataRow3["Father"] = dataRow2["Code"];
						for (int num7 = 0; num7 < array3.Length; num7++)
						{
							if (!string.IsNullOrEmpty(array3[num7].TargetField) && !string.IsNullOrEmpty(array3[num7].SourceField))
							{
								dataRow3[array3[num7].TargetField] = dataTable2.Rows[num6][array3[num7].SourceField];
							}
							if (array3[num7].DfltValue != null && dataRow3[array3[num7].TargetField] == DBNull.Value)
							{
								dataRow3[array3[num7].TargetField] = array3[num7].DfltValue;
							}
						}
						dataTable4.Rows.Add(dataRow3);
					}
					BillOfMasterials_Impl_OITT billOfMasterials_Impl_OITT = EntitiesHelper<BillOfMasterials_Impl_OITT>.ToEntity(dataRow2);
					if (billOfMasterials_Impl_OITT == null)
					{
						WinCommons.Error(Enums.MessageCode.CouldNotGetDataBillOfMaterialGeneral, "");
						return;
					}
					billOfMasterials_Impl_OITT.Line_IIT1 = EntitiesHelper<BillOfMasterials_LineImpl_ITT1>.ToEntities(dataTable4);
					string text = HttpClientHelperFactory.GetHttpClient().BillOfMasterials.SaveDoc(billOfMasterials_Impl_OITT);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						Logging.Write("ERROR", text);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
				if (list.Count == 0)
				{
					break;
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ProcessMappingProjectManagement(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 1;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='5' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable source3 = new DataTable();
			DataRow[] array2 = dtMapping.Select("MappingType='5' AND TargetItem='W'");
			if (array2.Length != 0)
			{
				source3 = array2.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapWbsField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable source4 = new DataTable();
			DataRow[] array3 = dtMapping.Select("MappingType='5' AND TargetItem='B'");
			if (array3.Length != 0)
			{
				source4 = array3.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapItemField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = ProjectManagementTableHelper.Project.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow item in dataTable.Rows)
			{
				baseForm.SetValueProgress(k);
				k++;
				DataTable dataTable2 = ProjectManagementTableHelper.Project.Clone();
				DataRow dataRow = dataTable2.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow[mapHeaderField[num].TargetField] = item[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapHeaderField[num].DfltValue)) && dataRow[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				ContextActionMappingImpl[] groupcol = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array4 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).Distinct().ToArray();
				DataTable dataTable3 = new DataTable();
				if (array4.Length != 0)
				{
					dataTable3 = array4.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapWbsField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
				}
				DataTable dataTable4 = ProjectManagementTableHelper.ProjectWorkBreakdownStructure.Clone();
				for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
				{
					DataRow dataRow2 = dataTable4.NewRow();
					dataRow2["LineNum"] = num2 + 1;
					for (int num3 = 0; num3 < mapWbsField.Length; num3++)
					{
						if (!string.IsNullOrEmpty(mapWbsField[num3].TargetField) && !string.IsNullOrEmpty(mapWbsField[num3].SourceField) && dataTable3.Rows[num2][mapWbsField[num3].SourceField] != DBNull.Value)
						{
							dataRow2[mapWbsField[num3].TargetField] = dataTable3.Rows[num2][mapWbsField[num3].SourceField];
						}
						if (mapWbsField[num3].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapWbsField[num3].DfltValue)) && dataRow2[mapWbsField[num3].TargetField] == DBNull.Value)
						{
							dataRow2[mapWbsField[num3].TargetField] = mapWbsField[num3].DfltValue;
						}
					}
					dataTable4.Rows.Add(dataRow2);
				}
				ContextActionMappingImpl[] groupcolBudget = mapHeaderField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array5 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcolBudget
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcolBudget
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).Distinct().ToArray();
				DataTable dataTable5 = new DataTable();
				if (array5.Length != 0)
				{
					dataTable5 = array5.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapItemField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
				}
				DataTable dataTable6 = ProjectManagementTableHelper.ProjectBudget.Clone();
				for (int num4 = 0; num4 < dataTable5.Rows.Count; num4++)
				{
					DataRow dataRow3 = dataTable6.NewRow();
					dataRow3["LineNum"] = num4 + 1;
					for (int num5 = 0; num5 < mapItemField.Length; num5++)
					{
						if (!string.IsNullOrEmpty(mapItemField[num5].TargetField) && !string.IsNullOrEmpty(mapItemField[num5].SourceField) && dataTable5.Rows[num4][mapItemField[num5].SourceField] != DBNull.Value)
						{
							dataRow3[mapItemField[num5].TargetField] = dataTable5.Rows[num4][mapItemField[num5].SourceField];
						}
						if (mapItemField[num5].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapItemField[num5].DfltValue)) && dataRow3[mapItemField[num5].TargetField] == DBNull.Value)
						{
							dataRow3[mapItemField[num5].TargetField] = mapItemField[num5].DfltValue;
						}
					}
					dataTable6.Rows.Add(dataRow3);
				}
				if (showdoc != "N")
				{
					dataTable2.Rows.Add(dataRow);
					frm_ProjectManager frm_ProjectManager2 = new frm_ProjectManager();
					frm_ProjectManager2.MdiParent = baseForm.MdiParent;
					frm_ProjectManager2.Show();
					continue;
				}
				dataTable2.Rows.Add(dataRow);
				dataTable2.TableName = "APZ_OPRG";
				System.Data.DataSet dataSet = new System.Data.DataSet();
				dataSet.Tables.Add(dataTable2.Copy());
				dataTable4.TableName = "APZ_PRG1";
				dataSet.Tables.Add(dataTable4.Copy());
				DataTable dataTable7 = ProjectManagementTableHelper.ProjectDataLines.Tables[5].Clone();
				dataTable7.TableName = "APZ_PRG2";
				dataSet.Tables.Add(dataTable7.Copy());
				DataTable dataTable8 = ProjectManagementTableHelper.ProjectDataLines.Tables[1].Clone();
				dataTable8.TableName = "APZ_PRG3";
				var array6 = (from t in dataTable4.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["Resource"]))
					select new
					{
						LineId = Function.ParseInt(t["LineNum"]),
						Resources = Function.ToString(t["Resource"])
					}).Distinct().ToArray();
				foreach (var anon in array6)
				{
					string[] array7 = anon.Resources.Split(';');
					if (array7.Length == 0)
					{
						continue;
					}
					string[] array8 = array7;
					foreach (string obj in array8)
					{
						if (Function.ParseInt(obj) > 0)
						{
							DataRow dataRow4 = dataTable8.NewRow();
							dataRow4["LineNum"] = anon.LineId;
							dataRow4["Resource"] = Function.ParseInt(obj);
							dataRow4["Role"] = 0;
							dataTable8.Rows.Add(dataRow4);
						}
					}
				}
				dataSet.Tables.Add(dataTable8.Copy());
				dataTable6.TableName = "APZ_PRG4";
				dataSet.Tables.Add(dataTable6.Copy());
				DataTable dataTable9 = ProjectManagementTableHelper.ProjectDataLines.Tables[3].Clone();
				dataTable9.TableName = "APZ_PRG5";
				dataSet.Tables.Add(dataTable9.Copy());
				DataTable dataTable10 = ProjectManagementTableHelper.ProjectDataLines.Tables[4].Clone();
				dataTable10.TableName = "SUB";
				dataSet.Tables.Add(dataTable10.Copy());
				DataTable dataTable11 = ProjectManagementTableHelper.ProjectDataLines.Tables[7].Clone();
				dataTable11.TableName = "APZ_PRG6";
				dataSet.Tables.Add(dataTable11.Copy());
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
				HttpResult httpResult = new HttpResult();
				if (!(method == "U"))
				{
					if (method == "A")
					{
						httpResult = await httpClient.ProjectManager.Add(dataSet);
					}
				}
				else
				{
					httpResult = await httpClient.ProjectManager.Update(dataSet);
				}
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ProcessMappingTask(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 1;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='A' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = ActivitiesTableHelper.Acivity.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				baseForm.SetValueProgress(k);
				k++;
				DataTable dataTable2 = ActivitiesTableHelper.Acivity.Clone();
				DataRow dataRow2 = dataTable2.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						dataRow2[mapHeaderField[num].TargetField] = row[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && dataRow2[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						dataRow2[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["Closed"])))
				{
					dataRow2["Closed"] = "N";
				}
				if (Function.ParseInt(dataRow2["AssignedBy"]) == 0)
				{
					dataRow2["AssignedBy"] = GlobalData.UserID;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow2["GroupUser"])) && string.IsNullOrEmpty(Function.ToString(dataRow2["GroupEmployee"])))
				{
					dataRow2["GroupUser"] = GlobalData.UserID.ToString();
				}
				dataRow2["Action"] = "T";
				if (showdoc != "N")
				{
					dataTable2.Rows.Add(dataRow2);
					frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
					frm_TaskManagement2.MdiParent = baseForm.MdiParent;
					frm_TaskManagement2.Show();
				}
				else if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Details"])))
				{
					frm_TaskManagement.SetAutoValueForTask(dataRow2);
					dataTable2.Rows.Add(dataRow2);
					System.Data.DataSet dataSet = new System.Data.DataSet();
					dataSet.Tables.Add(dataTable2.Copy());
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
					HttpResult httpResult = ((method == "U") ? (await httpClient.SubActivities.SaveTask(dataSet)) : (await httpClient.SubActivities.AddTask(dataSet)));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ProcessMappingTrackingIssue(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		_ = 2;
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return;
			}
			DataTable source2 = new DataTable();
			DataRow[] array = dtMapping.Select("MappingType='Y' AND TargetItem='H'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapHeaderField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = TrackingIssueTableHelper.TrackingIssues.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			baseForm.ShowProgress(dataTable.Rows.Count);
			int k = 1;
			foreach (DataRow row in dataTable.Rows)
			{
				baseForm.SetValueProgress(k);
				k++;
				if (showdoc != "N")
				{
					frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue();
					frm_TrackingIssue2.OpenFormViewMode = Enums.FormViewModeType.Add;
					frm_TrackingIssue2.MappingDocument = mapHeaderField;
					frm_TrackingIssue2.MappingSource = new DataRow[1] { row }.CopyToDataTable();
					frm_TrackingIssue2.MdiParent = baseForm.MdiParent;
					frm_TrackingIssue2.Show();
					continue;
				}
				DataTable dtDoc = TrackingIssueTableHelper.TrackingIssues.Clone();
				DataRow drDoc = dtDoc.NewRow();
				for (int num = 0; num < mapHeaderField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapHeaderField[num].SourceField))
					{
						drDoc[mapHeaderField[num].TargetField] = row[mapHeaderField[num].SourceField];
					}
					if (mapHeaderField[num].DfltValue != null && !string.IsNullOrEmpty(Function.ToString(mapHeaderField[num].DfltValue)) && drDoc[mapHeaderField[num].TargetField] == DBNull.Value)
					{
						drDoc[mapHeaderField[num].TargetField] = mapHeaderField[num].DfltValue;
					}
				}
				if (!Function.ParseDateTime(drDoc["IssueDate"]).HasValue)
				{
					drDoc["IssueDate"] = DateTime.Today;
				}
				if (string.IsNullOrEmpty(Function.ToString(drDoc["Status"])))
				{
					drDoc["Status"] = "1";
				}
				if (Function.ParseInt(drDoc["Reporter"]) <= 0)
				{
					DataTable dataTable2 = await _httpClient.TrackingIssue.GetEmployeeByUserId(GlobalData.UserID);
					if (dataTable2.IsNotNull())
					{
						drDoc["Reporter"] = Function.ParseInt(dataTable2.Rows[0]["empId"]);
					}
				}
				string text = method;
				if (!(text == "U"))
				{
					if (text == "A")
					{
						drDoc["UserSign"] = GlobalData.UserID;
					}
				}
				else
				{
					drDoc["UserSign2"] = GlobalData.UserID;
				}
				dtDoc.Rows.Add(drDoc);
				dtDoc.TableName = "dtTracking";
				DataTable dataTable3 = WinFuntions.GetTableStructure("ATC1").Clone();
				dataTable3.TableName = "AttFile";
				DataTable dataTable4 = TrackingIssueTableHelper.TrackingHistory.Clone();
				System.Data.DataSet dataSet = new System.Data.DataSet();
				dataSet.Tables.AddRange(new DataTable[3]
				{
					dtDoc.Copy(),
					dataTable3.Copy(),
					dataTable4.Copy()
				});
				WinCommons._OpenProcessing();
				HttpResult httpResult = new HttpResult();
				text = method;
				if (!(text == "U"))
				{
					if (text == "A")
					{
						httpResult = await _httpClient.TrackingIssue.Add(dataSet);
					}
				}
				else
				{
					httpResult = await _httpClient.TrackingIssue.Update(dataSet);
				}
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			baseForm.HideProgress(200);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<HttpResult> ProcessMappingWarehouse(DataTable source, string showdoc, XtraFormEx baseForm, DataTable dtMapping, Enums.ContextActionSourceType sourceType, Enums.ContextActionDataRange range, string method = "A", string objectId = "")
	{
		try
		{
			if (!source.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return new HttpResult(Enums.MessageCode.DatasourceIsNull, "Datasource is null");
			}
			DataTable source2 = new DataTable();
			DataTable source3 = new DataTable();
			DataTable source4 = new DataTable();
			DataRow[] array = dtMapping.Select("TargetItem='G'");
			if (array.Length != 0)
			{
				source2 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='A'");
			if (array.Length != 0)
			{
				source3 = array.CopyToDataTable();
			}
			array = dtMapping.Select("TargetItem='B'");
			if (array.Length != 0)
			{
				source4 = array.CopyToDataTable();
			}
			ContextActionMappingImpl[] mapWarehouseField = (from t in source2.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapAccountingField = (from t in source3.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			ContextActionMappingImpl[] mapBinLocationField = (from t in source4.AsEnumerable()
				select new ContextActionMappingImpl(t)).ToArray();
			DataTable dataTable = source.DefaultView.ToTable(distinct: true, (from t in mapWarehouseField
				where !string.IsNullOrEmpty(t.SourceField)
				select t.SourceField).Distinct().ToArray());
			if (!dataTable.IsNotNull())
			{
				dataTable = WarehousesTableHelper.Warehouse.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			WinCommons._OpenProcessing();
			string message = "";
			int k = 1;
			foreach (DataRow item in dataTable.Rows)
			{
				ContextActionMappingImpl[] groupcol = mapWarehouseField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
				DataRow[] array2 = (from r in source.AsEnumerable()
					where new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = item[map.SourceField]
						}).Equals(new NTuple<object>(from map in groupcol
						where !string.IsNullOrEmpty(map.SourceField)
						select new
						{
							Name = map.SourceField,
							Value = r[map.SourceField]
						}))
					select (r)).ToArray();
				if (array2.Length == 0)
				{
					continue;
				}
				array2.CopyToDataTable().DefaultView.ToTable(distinct: true, (from t in mapWarehouseField
					where !string.IsNullOrEmpty(t.SourceField)
					select t.SourceField).Distinct().ToArray());
				k++;
				DataTable dataTable2 = WarehousesTableHelper.Warehouse.Clone();
				DataRow dataRow = dataTable2.NewRow();
				for (int num = 0; num < mapWarehouseField.Length; num++)
				{
					if (!string.IsNullOrEmpty(mapWarehouseField[num].SourceField))
					{
						dataRow[mapWarehouseField[num].TargetField] = item[mapWarehouseField[num].SourceField];
					}
					if (mapWarehouseField[num].DfltValue != null && dataRow[mapWarehouseField[num].TargetField] == DBNull.Value)
					{
						try
						{
							dataRow[mapWarehouseField[num].TargetField] = mapWarehouseField[num].DfltValue;
						}
						catch (Exception ex)
						{
							WinCommons.Warning(Enums.MessageCode.Warning, ex.Message);
						}
					}
				}
				for (int num2 = 0; num2 < mapAccountingField.Length; num2++)
				{
					if (!string.IsNullOrEmpty(mapAccountingField[num2].SourceField))
					{
						dataRow[mapAccountingField[num2].TargetField] = item[mapAccountingField[num2].SourceField];
					}
					if (mapAccountingField[num2].DfltValue != null && dataRow[mapAccountingField[num2].TargetField] == DBNull.Value)
					{
						try
						{
							dataRow[mapAccountingField[num2].TargetField] = mapAccountingField[num2].DfltValue;
						}
						catch (Exception ex2)
						{
							WinCommons.Warning(Enums.MessageCode.Warning, ex2.Message);
						}
					}
				}
				for (int num3 = 0; num3 < mapBinLocationField.Length; num3++)
				{
					if (!string.IsNullOrEmpty(mapBinLocationField[num3].SourceField))
					{
						dataRow[mapBinLocationField[num3].TargetField] = item[mapBinLocationField[num3].SourceField];
					}
					if (mapBinLocationField[num3].DfltValue != null && dataRow[mapBinLocationField[num3].TargetField] == DBNull.Value)
					{
						try
						{
							dataRow[mapBinLocationField[num3].TargetField] = mapBinLocationField[num3].DfltValue;
						}
						catch (Exception ex3)
						{
							WinCommons.Warning(Enums.MessageCode.Warning, ex3.Message);
						}
					}
				}
				dataTable2.Rows.Add(dataRow);
				WarehouseImpl doc = EntitiesHelper<WarehouseImpl>.ToEntity(dataRow);
				string text = await HttpClientHelperFactory.GetHttpClient().Warehouse.Save(doc);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					message = message + "\t" + text;
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex4)
		{
			Logging.Write("ERROR", ex4);
			WinCommons.ErrorLineNumber(ex4);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex4.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	public async Task<HttpResult> ProcessWorkflow(IHttpClient httpClient, int absEntry, string formId, string itemId, DataTable workflowSource)
	{
		try
		{
			DataTable source = await httpClient.ContextAction.GetProcessGroup(absEntry, formId, itemId);
			List<IWorkflowProcess> processGroup = new List<IWorkflowProcess>();
			foreach (DataRow item2 in from t in source.AsEnumerable()
				where Function.ParseInt(t["FatherId"]) == absEntry
				select t)
			{
				int absEntry2 = Function.ParseInt(item2["AbsEntry"]);
				System.Data.DataSet ds = _httpClient.ContextAction.GetDataByEntry(absEntry2, _formId, _itemId);
				if (!ds.IsNotNull())
				{
					continue;
				}
				DataTable dtDoc = ds.Tables[0];
				Enums.ContextActionActionTypeEx actionType = (Enums.ContextActionActionTypeEx)Function.ParseInt(dtDoc.Rows[0]["ActionTypeEx"]);
				Enums.ContextActionSourceType contextActionSourceType = (Enums.ContextActionSourceType)(Function.ParseChar(dtDoc.Rows[0]["SourceType"]) ?? 'N');
				Enums.ContextActionDataRange range = (Enums.ContextActionDataRange)(Function.ParseChar(dtDoc.Rows[0]["DataRange"]) ?? 'A');
				Enums.ContextActionMethod method = (Enums.ContextActionMethod)(Function.ParseChar(dtDoc.Rows[0]["Method"]) ?? 'A');
				Enums.ContextActionDataFrom datafrom = (Enums.ContextActionDataFrom)(Function.ParseChar(dtDoc.Rows[0]["DataFrom"]) ?? 'T');
				string dataString = Function.ToString(dtDoc.Rows[0]["DataString"]);
				string objectId = Function.ToString(dtDoc.Rows[0]["ObjectId"]);
				string filterString = Function.ToString(dtDoc.Rows[0]["FilterString"]);
				DataTable source2;
				if (contextActionSourceType == Enums.ContextActionSourceType.None)
				{
					source2 = workflowSource.Copy();
				}
				else
				{
					try
					{
						source2 = await GetMappingSource(ds, contextActionSourceType, range, datafrom, dataString);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
						return new HttpResult(Enums.MessageCode.Error, ex.Message);
					}
				}
				if (!string.IsNullOrEmpty(filterString))
				{
					DataRow[] array = source2.Select(filterString);
					source2 = ((array.Length == 0) ? source2.Clone() : array.CopyToDataTable());
				}
				switch (actionType)
				{
				case Enums.ContextActionActionTypeEx.MaketingDocument:
				{
					Enums.ContextActionDocumentType objtype = (Enums.ContextActionDocumentType)Function.ParseInt(dtDoc.Rows[0]["ObjType"]);
					DataTable dataTable3 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dataTable3 = ds.Tables[2];
					}
					IContextActionHandle actionHandle2 = ContextActionHandleFactory.GetActionHandle();
					if (!dataTable3.IsNotNull())
					{
						break;
					}
					foreach (DocumentsImpl item3 in GetMappingDocument(source2, dataTable3, objtype, method, objectId))
					{
						WorkflowProcess<DocumentsImpl> workflowProcess3 = new WorkflowProcess<DocumentsImpl>();
						workflowProcess3.ActionType = actionType;
						workflowProcess3.ObjectType = actionHandle2.GetObjectTypesFromContext(objtype);
						workflowProcess3.Method = method;
						workflowProcess3.Content = item3;
						processGroup.Add(workflowProcess3);
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.UserDefinedObject:
				{
					if (!source2.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.DatasourceIsMissing, "");
						return new HttpResult(Enums.MessageCode.Error, "Data source is missing");
					}
					DataTable dataTable2 = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dataTable2 = ds.Tables[2];
					}
					if (!dataTable2.IsNotNull())
					{
						break;
					}
					ContextActionMappingImpl[] map = (from t in dataTable2.AsEnumerable()
						select new ContextActionMappingImpl(t)).ToArray();
					string udoCode = Function.ToString(dtDoc.Rows[0]["UdoCode"]);
					ContextActionMappingImpl[] mapHeaderField = map.Where((ContextActionMappingImpl t) => t.TargetItem == udoCode).ToArray();
					DataTable dtSource = source2.DefaultView.ToTable(distinct: true, (from t in mapHeaderField
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray());
					System.Data.DataSet dsUdo = await GetUdoStruct(udoCode, mapHeaderField, map, source2);
					if (!dtSource.IsNotNull())
					{
						break;
					}
					foreach (DataRow row in dtSource.Rows)
					{
						System.Data.DataSet udo = GetUdo(dsUdo, udoCode, mapHeaderField, row, map, source2);
						WorkflowProcess<System.Data.DataSet> workflowProcess2 = new WorkflowProcess<System.Data.DataSet>();
						workflowProcess2.ActionType = actionType;
						workflowProcess2.ObjectType = ApzObjectTypes.oUserDefinedObject;
						workflowProcess2.Method = method;
						workflowProcess2.Content = udo;
						processGroup.Add(workflowProcess2);
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.JournalEntry:
				{
					Function.ParseInt(dtDoc.Rows[0]["ObjType"]);
					DataTable dataTable = new DataTable();
					if (ds.Tables.Count >= 3)
					{
						dataTable = ds.Tables[2];
					}
					IContextActionHandle actionHandle = ContextActionHandleFactory.GetActionHandle(Enums.ContextActionActionTypeEx.JournalEntry, JournalEntryTableHelper.JournalEntry, JournalEntryTableHelper.JournalEntryRows);
					if (!dataTable.IsNotNull())
					{
						break;
					}
					foreach (JournalEntriesImpl item4 in actionHandle.GetMappingJournalEntry(source2, dataTable))
					{
						WorkflowProcess<JournalEntriesImpl> workflowProcess = new WorkflowProcess<JournalEntriesImpl>();
						workflowProcess.ActionType = actionType;
						workflowProcess.ObjectType = ApzObjectTypes.oJournalEntries;
						workflowProcess.Method = method;
						workflowProcess.Content = item4;
						processGroup.Add(workflowProcess);
					}
					break;
				}
				}
			}
			if (processGroup.Count > 0)
			{
				HttpResult httpResult = await _httpClient.ContextAction.HandleWorkflowProcess(processGroup);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					XtraMessageBox.Show(httpResult.Message, "System", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				return httpResult;
			}
			return new HttpResult(Enums.MessageCode.UnknownMessage);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			return new HttpResult(Enums.MessageCode.Error, ex2.Message);
		}
	}
}
