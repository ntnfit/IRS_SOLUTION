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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
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
using DevExpress.XtraTab;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_CheckPromotion : XtraFormEx
{
	private readonly IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	public RepositoryItem PromotiontypeEditor;

	public Action<List<DataRow>, DataTable> ChoosePromotion;

	private DataTable DataBenefit = new DataTable();

	private string guid = string.Empty;

	private IContainer components;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Preview;

	private GridViewEx gv_Preview;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridControlEx grd_Header;

	private GridViewEx gv_Header;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx9;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx10;

	private XtraTabControl xtraTabControl1;

	private XtraTabPage xtraTabPage2;

	private XtraTabPage xtraTabPage1;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_Benefit;

	private GridViewEx gv_Benefit;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gridColumnEx30;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private RepositoryItemLookUpEdit rep_AppliedType;

	public DataSet Input { get; set; }

	public int DocEntry { get; set; }

	public ApzObjectTypes ObjType { get; internal set; }

	public frm_CheckPromotion()
	{
		InitializeComponent();
	}

	private async Task frm_CheckPromotion_Load(object sender, EventArgs e)
	{
		try
		{
			rep_AppliedType.SetDataSource(new Dictionary<string, string>
			{
				{ "H", "Header" },
				{ "R", "Rows" }
			});
			string value = string.Empty;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CardCode", typeof(string));
			dataTable.Columns.Add("DocDate", typeof(DateTime));
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("Qty", typeof(double));
			dataTable.Columns.Add("VatPrcnt", typeof(double));
			dataTable.Columns.Add("LineTotal", typeof(string));
			dataTable.Columns.Add("ItemGroup", typeof(int));
			dataTable.Columns.Add("BPGroup", typeof(int));
			dataTable.Columns.Add("DocEntry", typeof(int));
			if (Input.IsNotNull())
			{
				DataTable dataTable2 = Input.Tables[0];
				if (dataTable2.IsNotNull())
				{
					value = Function.ToString(dataTable2.Rows[0]["CardCode"]);
					guid = Function.ToString(dataTable2.Rows[0]["U_GUID"]);
					if (string.IsNullOrEmpty(guid))
					{
						guid = Guid.NewGuid().ToString();
						dataTable2.Rows[0]["U_GUID"] = guid;
					}
					DataTable dataTable3 = Input.Tables[1];
					if (dataTable3.IsNotNull())
					{
						DataRow[] array = (from t in dataTable3.AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
							select t).ToArray();
						foreach (DataRow dataRow in array)
						{
							DataRow dataRow2 = dataTable.NewRow();
							dataRow2["CardCode"] = value;
							dataRow2["DocDate"] = Function.ParseDateTime(dataTable2.Rows[0]["DocDate"]) ?? DateTime.Today;
							dataRow2["ItemCode"] = Function.ToString(dataRow["ItemCode"]);
							dataRow2["Qty"] = Function.ParseDouble(dataRow["InvQty"]);
							dataRow2["LineTotal"] = Function.ParseDouble(dataRow["LineTotal"]);
							dataRow2["VatPrcnt"] = Function.ParseDouble(dataRow["VatPrcnt"]);
							dataRow2["DocEntry"] = DocEntry;
							dataTable.Rows.Add(dataRow2);
						}
					}
				}
			}
			if (dataTable.Rows.Count == 0)
			{
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3["CardCode"] = value;
				dataRow3["DocEntry"] = DocEntry;
				dataTable.Rows.Add(dataRow3);
			}
			DataTable dataTable4 = await httpClient.Promotion.CheckPromotion(dataTable);
			DataTable dataTable5 = dataTable4.Copy();
			dataTable5.Columns.Add("AppliedType", typeof(string));
			dataTable5.Columns.Add("LineNum", typeof(int));
			dataTable5.Columns.Add("GUID", typeof(string));
			DataBenefit = dataTable5.Clone();
			grd_Benefit.DataSource = DataBenefit;
			grd_Benefit.RefreshDataSource();
			grd_Data.DataSource = dataTable4;
			grd_Data.RefreshDataSource();
			gv_Data.ClearSelection();
			if (dataTable4.IsNotNull())
			{
				gv_Data.Columns["ProgramName"].GroupIndex = 0;
			}
			gv_Data.ExpandAllGroups();
			grd_Header.DataSource = Input.Tables[0].Copy();
			grd_Header.RefreshDataSource();
			grd_Preview.DataSource = Input.Tables[1].Copy();
			grd_Preview.RefreshDataSource();
			if (gv_Preview.Columns["U_PromotionType"] != null)
			{
				gv_Preview.Columns["U_PromotionType"].ColumnEdit = PromotiontypeEditor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grd_Benefit.DataSourceTable;
			if (dt.IsNotNull())
			{
				HttpResult httpResult = await httpClient.Promotion.SaveAppliedBenefit(dt);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
					return;
				}
				ChoosePromotion(gv_Data.GetListDataSourceSelectedRow(), dt);
				Close();
			}
			else
			{
				ChoosePromotion(gv_Data.GetListDataSourceSelectedRow(), dt);
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			ProcessPromotion(gv_Data.GetListDataSourceSelectedRow());
			gv_Benefit.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ProcessPromotion(List<DataRow> rows)
	{
		try
		{
			DataTable dataTable = Input.Tables[1].Copy();
			DataTable dataTable2 = Input.Tables[0].Copy();
			grd_Benefit.DataSource = DataBenefit.Copy();
			foreach (DataRow row in rows)
			{
				int num = Function.ParseInt(row["TypeID"]);
				switch (num)
				{
				case 1:
				case 2:
				case 7:
				{
					string itemCode = Function.ToString(row["ItemCode"]);
					for (int num8 = 0; num8 < dataTable.Rows.Count; num8++)
					{
						int lineNum = Function.ParseInt(dataTable.Rows[num8]["LineNum"]);
						if (grd_Benefit.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == lineNum && Function.ToString(t["ItemCode"]) == itemCode && new int[2] { 3, 4 }.Contains(Function.ParseInt(t["TypeID"]))) != null || !(Function.ToString(dataTable.Rows[num8]["ItemCode"]) == itemCode))
						{
							continue;
						}
						switch (num)
						{
						case 1:
						{
							double num14 = Function.ParseDouble(dataTable.Rows[num8]["DiscPrcnt"]);
							double num15 = Function.ParseDouble(row["ActualVal"]);
							double num16 = num14 + num15;
							dataTable.Rows[num8]["DiscPrcnt"] = num16;
							dataTable.Rows[num8]["LineTotal"] = (1.0 - num16 / 100.0) * Function.ParseDouble(dataTable.Rows[num8]["PriceBefDi"]) * Function.ParseDouble(dataTable.Rows[num8]["Quantity"]);
							break;
						}
						case 2:
						{
							double num17 = Function.ParseDouble(dataTable.Rows[num8]["LineTotal"]);
							double num18 = Function.ParseDouble(row["ActualVal"]);
							double num19 = num17 - num18;
							dataTable.Rows[num8]["LineTotal"] = num19;
							Function.ParseDouble(dataTable.Rows[num8]["DiscPrcnt"]);
							double num20 = Function.ParseDouble(dataTable.Rows[num8]["Quantity"]);
							double num21 = Function.ParseDouble(dataTable.Rows[num8]["PriceBefDi"]);
							if (num20 != 0.0)
							{
								if (num21.Equals(0.0))
								{
									dataTable.Rows[num8]["DiscPrcnt"] = 0;
									dataTable.Rows[num8]["PriceBefDi"] = num19 / num20;
								}
								else
								{
									dataTable.Rows[num8]["DiscPrcnt"] = (num20 * num21 - num19) * 100.0 / (num20 * num21);
								}
							}
							break;
						}
						case 7:
						{
							double num9 = Function.ParseDouble(dataTable.Rows[num8]["LineTotal"]);
							double num10 = Function.ParseDouble(row["ActualVal"]);
							double num11 = num9 - num10;
							dataTable.Rows[num8]["LineTotal"] = num11;
							Function.ParseDouble(dataTable.Rows[num8]["DiscPrcnt"]);
							double num12 = Function.ParseDouble(dataTable.Rows[num8]["Quantity"]);
							double num13 = Function.ParseDouble(dataTable.Rows[num8]["PriceBefDi"]);
							if (num12 != 0.0)
							{
								if (num13.Equals(0.0))
								{
									dataTable.Rows[num8]["DiscPrcnt"] = 0;
									dataTable.Rows[num8]["PriceBefDi"] = num11 / num12;
								}
								else
								{
									dataTable.Rows[num8]["DiscPrcnt"] = (num12 * num13 - num11) * 100.0 / (num12 * num13);
								}
							}
							break;
						}
						}
						SetDocTotal(dataTable2, dataTable);
						dataTable.Rows[num8]["U_PromotionType"] = num;
						dataTable.Rows[num8]["U_PromotionCode"] = Function.ToString(row["ProgramCode"]);
						dataTable.Rows[num8]["U_PromotionGroup"] = Function.ToString(row["GroupID"]);
						dataTable.Rows[num8]["U_PromotionPack"] = Function.ToString(row["PackNo"]);
						dataTable.Rows[num8]["U_PromotionLine"] = Function.ParseInt(row["ProfitLineID"]);
						dataTable.Rows[num8]["U_PromotionGroupProfit"] = Function.ParseInt(row["ProfitGroupID"]);
						DataTable dataSourceTable3 = grd_Benefit.DataSourceTable;
						DataRow dataRow5 = dataSourceTable3.NewRow();
						dataRow5.ItemArray = row.ItemArray;
						dataRow5["AppliedType"] = "R";
						dataRow5["LineNum"] = Function.ParseInt(dataTable.Rows[num8]["LineNum"]);
						dataRow5["GUID"] = guid;
						dataSourceTable3.Rows.Add(dataRow5);
						dataSourceTable3.AcceptChanges();
					}
					break;
				}
				case 3:
				case 4:
				{
					string text = Function.ToString(row["ItemCode"]);
					double num4 = Function.ParseDouble(row["ActualVal"]);
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["ItemCode"] = text;
					dataRow2["Quantity"] = num4;
					dataRow2["DiscPrcnt"] = 100;
					DataTable dataTable3 = httpClient.Commons.ItemMaster(text, GlobalData.DefaultBranch, Enums.ItemObjectType.NoItemType, Function.ToString(gv_Header.GetFocusedRowCellValue("CardCode")), Function.ParseDateTime(gv_Header.GetFocusedRowCellValue("DocDate")));
					if (dataTable3.IsNotNull())
					{
						string text2 = "N";
						DataRow dataRow3 = dataTable3.Rows[0];
						if (dataRow3.IsExistsColumn("IsGrossPrc"))
						{
							text2 = Function.ToString(dataRow3["IsGrossPrc"]);
						}
						double num5 = Function.ParseDouble(dataRow3["Price"]);
						if (text2 == "Y")
						{
							dataRow2["PriceBefDi"] = num5 / (1.0 + Function.ParseDouble(dataRow3["RateSa"]) / 100.0);
						}
						else
						{
							dataRow2["PriceBefDi"] = num5;
						}
						double num6 = Function.ParseDouble(dataRow2["PriceBefDi"]);
						dataRow2["Price"] = num6 - num6 * 100.0 / 100.0;
					}
					dataRow2["U_PromotionType"] = num;
					dataRow2["U_PromotionCode"] = Function.ToString(row["ProgramCode"]);
					dataRow2["U_PromotionGroup"] = Function.ToString(row["GroupID"]);
					dataRow2["U_PromotionPack"] = Function.ToString(row["PackNo"]);
					dataRow2["U_PromotionLine"] = Function.ParseInt(row["ProfitLineID"]);
					dataRow2["U_PromotionGroupProfit"] = Function.ParseInt(row["ProfitGroupID"]);
					int num7 = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
					dataRow2["LineNum"] = num7;
					dataTable.Rows.Add(dataRow2);
					SetDocTotal(dataTable2, dataTable);
					DataTable dataSourceTable2 = grd_Benefit.DataSourceTable;
					DataRow dataRow4 = dataSourceTable2.NewRow();
					dataRow4.ItemArray = row.ItemArray;
					dataRow4["AppliedType"] = "R";
					dataRow4["GUID"] = guid;
					dataRow4["LineNum"] = num7;
					dataSourceTable2.Rows.Add(dataRow4);
					dataSourceTable2.AcceptChanges();
					break;
				}
				case 5:
				{
					double num22 = Function.ParseDouble(dataTable2.Rows[0]["DiscPrcnt"]);
					double num23 = Function.ParseDouble(row["ActualVal"]);
					dataTable2.Rows[0]["DiscPrcnt"] = num22 + num23;
					double num24 = num22 + num23;
					double num25 = grd_Preview.DatasourceTable().AsEnumerable().Sum((DataRow x) => Function.ParseDouble(x["LineTotal"]));
					dataTable2.Rows[0]["Total"] = num25 * (1.0 - num24 / 100.0);
					DataTable dataSourceTable4 = grd_Benefit.DataSourceTable;
					DataRow dataRow6 = dataSourceTable4.NewRow();
					dataRow6.ItemArray = row.ItemArray;
					dataRow6["AppliedType"] = "H";
					dataRow6["GUID"] = guid;
					dataRow6["LineNum"] = -1;
					dataSourceTable4.Rows.Add(dataRow6);
					dataSourceTable4.AcceptChanges();
					break;
				}
				case 6:
				{
					double num2 = Function.ParseDouble(dataTable2.Rows[0]["DocTotal"]);
					double num3 = Function.ParseDouble(row["ActualVal"]);
					dataTable2.Rows[0]["Total"] = num2 - num3;
					grd_Preview.DatasourceTable().AsEnumerable().Sum((DataRow x) => Function.ParseDouble(x["LineTotal"]));
					dataTable2.Rows[0]["DiscPrcnt"] = num3 * 100.0 / num2;
					DataTable dataSourceTable = grd_Benefit.DataSourceTable;
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow.ItemArray = row.ItemArray;
					dataRow["AppliedType"] = "H";
					dataRow["GUID"] = guid;
					dataRow["LineNum"] = -1;
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
					break;
				}
				}
			}
			grd_Preview.DataSource = dataTable;
			grd_Preview.RefreshDataSource();
			grd_Header.DataSource = dataTable2;
			grd_Header.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDocTotal(DataTable sourceHeader, DataTable source)
	{
		try
		{
			double num = Function.ParseDouble(sourceHeader.Rows[0]["DiscPrcnt"]);
			double num2 = source.AsEnumerable().Sum((DataRow x) => Function.ParseDouble(x["LineTotal"]));
			sourceHeader.Rows[0]["TotalBeforeDis"] = num2 * (1.0 - num / 100.0);
			sourceHeader.Rows[0]["Total"] = num2 * (1.0 - num / 100.0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_CheckPromotion));
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx9 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx10 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Header = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Header = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Preview = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Preview = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
		this.grd_Benefit = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Benefit = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AppliedType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Preview).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Preview).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AppliedType).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemCalcEditEx8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx8.IsSearchControl = false;
		this.repositoryItemCalcEditEx8.Name = "repositoryItemCalcEditEx8";
		this.repositoryItemCalcEditEx9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx9.IsSearchControl = false;
		this.repositoryItemCalcEditEx9.Name = "repositoryItemCalcEditEx9";
		this.repositoryItemCalcEditEx10.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx10.IsSearchControl = false;
		this.repositoryItemCalcEditEx10.Name = "repositoryItemCalcEditEx10";
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Data.Location = new System.Drawing.Point(0, 0);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(954, 280);
		this.grd_Data.TabIndex = 0;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Data.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx14, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13 });
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "CheckPromotion";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "PMOR";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Data.OptionsBehavior.ReadOnly = true;
		this.gv_Data.OptionsPrint.AutoWidth = false;
		this.gv_Data.OptionsPrint.UsePrintStyles = false;
		this.gv_Data.OptionsSelection.MultiSelect = true;
		this.gv_Data.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Data.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Data.OptionsView.ShowGroupedColumns = true;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.gv_Data.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Data_SelectionChanged);
		this.gridColumnEx14.Caption = "Program Name";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "ProgramName";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 7;
		this.gridColumnEx14.Width = 100;
		this.gridColumnEx7.Caption = "Profit Code";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ProfitCode";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 8;
		this.gridColumnEx7.Width = 150;
		this.gridColumnEx8.Caption = "Note";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Note";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 1;
		this.gridColumnEx8.Width = 150;
		this.gridColumnEx9.Caption = "Type";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "TypeNote";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 150;
		this.gridColumnEx10.Caption = "Actual Value";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx10.FieldName = "ActualVal";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 3;
		this.gridColumnEx10.Width = 150;
		this.gridColumnEx11.Caption = "Item Code";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ItemCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 4;
		this.gridColumnEx11.Width = 150;
		this.gridColumnEx12.Caption = "ItemOnHandQty";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx12.FieldName = "Item On Hand Qty";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 5;
		this.gridColumnEx12.Width = 150;
		this.gridColumnEx13.Caption = "Unit Price";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx13.FieldName = "UnitPrice";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 6;
		this.gridColumnEx13.Width = 150;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 614);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 614);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 12);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Data);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(954, 596);
		this.splitContainerControl1.SplitterPosition = 280;
		this.splitContainerControl1.TabIndex = 3;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(954, 306);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.splitContainerControl2);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(952, 281);
		this.xtraTabPage1.Text = "Preview";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Header);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Preview);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(952, 281);
		this.splitContainerControl2.TabIndex = 0;
		this.grd_Header.AllowPasteExcel = true;
		this.grd_Header.ClipboardData = null;
		this.grd_Header.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Header.Location = new System.Drawing.Point(0, 0);
		this.grd_Header.MainView = this.gv_Header;
		this.grd_Header.Name = "grd_Header";
		this.grd_Header.Size = new System.Drawing.Size(952, 100);
		this.grd_Header.TabIndex = 0;
		this.grd_Header.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Header });
		this.gv_Header.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Header.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Header.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Header.ColumnKey = null;
		this.gv_Header.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25 });
		this.gv_Header.DisableFormSetting = false;
		this.gv_Header.DisplayException = false;
		this.gv_Header.EditColumn = null;
		this.gv_Header.FieldCurrency = null;
		this.gv_Header.FieldRate = null;
		this.gv_Header.FormId = "CheckPromotion";
		this.gv_Header.GridControl = this.grd_Header;
		this.gv_Header.IgnoreFilter = false;
		this.gv_Header.IndexEditRow = -1;
		this.gv_Header.IndicatorWidth = 35;
		this.gv_Header.IsApzTable = false;
		this.gv_Header.IsInsertNewRow = false;
		this.gv_Header.IsLoadingData = false;
		this.gv_Header.IsMappingContextAction = false;
		this.gv_Header.ItemId = "PMOH";
		this.gv_Header.LstColumnAllowBatch = null;
		this.gv_Header.LstColumnNotAllowBatch = null;
		this.gv_Header.LstColumnNotEditable = null;
		this.gv_Header.LstColumNotSetting = null;
		this.gv_Header.Name = "gv_Header";
		this.gv_Header.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Header.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Header.OptionsBehavior.ReadOnly = true;
		this.gv_Header.OptionsPrint.AutoWidth = false;
		this.gv_Header.OptionsPrint.UsePrintStyles = false;
		this.gv_Header.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Header.OptionsView.ColumnAutoWidth = false;
		this.gv_Header.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Header.OptionsView.ShowGroupedColumns = true;
		this.gv_Header.OptionsView.ShowGroupPanel = false;
		this.gv_Header.UseBatchMod = false;
		this.gridColumnEx20.Caption = "BP Code";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "CardCode";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = true;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 0;
		this.gridColumnEx20.Width = 150;
		this.gridColumnEx21.Caption = "BP Name";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "CardName";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 1;
		this.gridColumnEx21.Width = 150;
		this.gridColumnEx22.Caption = "Total Before Discount";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx22.FieldName = "TotalBeforeDis";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 2;
		this.gridColumnEx22.Width = 150;
		this.gridColumnEx23.Caption = "Discount %";
		this.gridColumnEx23.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx23.FieldName = "DiscPrcnt";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 3;
		this.gridColumnEx23.Width = 150;
		this.gridColumnEx24.Caption = "Tax Total";
		this.gridColumnEx24.ColumnEdit = this.repositoryItemCalcEditEx9;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx24.FieldName = "TaxTotal";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 4;
		this.gridColumnEx24.Width = 150;
		this.gridColumnEx25.Caption = "Total";
		this.gridColumnEx25.ColumnEdit = this.repositoryItemCalcEditEx10;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx25.FieldName = "Total";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 5;
		this.gridColumnEx25.Width = 150;
		this.grd_Preview.AllowPasteExcel = true;
		this.grd_Preview.ClipboardData = null;
		this.grd_Preview.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Preview.Location = new System.Drawing.Point(0, 0);
		this.grd_Preview.MainView = this.gv_Preview;
		this.grd_Preview.Name = "grd_Preview";
		this.grd_Preview.Size = new System.Drawing.Size(952, 171);
		this.grd_Preview.TabIndex = 0;
		this.grd_Preview.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Preview });
		this.gv_Preview.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Preview.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Preview.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Preview.ColumnKey = null;
		this.gv_Preview.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18,
			this.gridColumnEx19
		});
		this.gv_Preview.DisableFormSetting = false;
		this.gv_Preview.DisplayException = false;
		this.gv_Preview.EditColumn = null;
		this.gv_Preview.FieldCurrency = null;
		this.gv_Preview.FieldRate = null;
		this.gv_Preview.FormId = "CheckPromotion";
		this.gv_Preview.GridControl = this.grd_Preview;
		this.gv_Preview.IgnoreFilter = false;
		this.gv_Preview.IndexEditRow = -1;
		this.gv_Preview.IndicatorWidth = 35;
		this.gv_Preview.IsApzTable = false;
		this.gv_Preview.IsInsertNewRow = false;
		this.gv_Preview.IsLoadingData = false;
		this.gv_Preview.IsMappingContextAction = false;
		this.gv_Preview.ItemId = "PMOL";
		this.gv_Preview.LstColumnAllowBatch = null;
		this.gv_Preview.LstColumnNotAllowBatch = null;
		this.gv_Preview.LstColumnNotEditable = null;
		this.gv_Preview.LstColumNotSetting = null;
		this.gv_Preview.Name = "gv_Preview";
		this.gv_Preview.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Preview.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Preview.OptionsPrint.AutoWidth = false;
		this.gv_Preview.OptionsPrint.UsePrintStyles = false;
		this.gv_Preview.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Preview.OptionsView.ColumnAutoWidth = false;
		this.gv_Preview.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Preview.OptionsView.ShowGroupedColumns = true;
		this.gv_Preview.OptionsView.ShowGroupPanel = false;
		this.gv_Preview.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Item Code";
		this.gridColumnEx1.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ItemCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 150;
		this.gridColumnEx2.Caption = "Item Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Dscription";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 150;
		this.gridColumnEx3.Caption = "Unit Price";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx3.FieldName = "PriceBefDi";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 150;
		this.gridColumnEx4.Caption = "Quantity";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx4.FieldName = "Quantity";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 150;
		this.gridColumnEx5.Caption = "Discount";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx5.FieldName = "DiscPrcnt";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 150;
		this.gridColumnEx6.Caption = "Total";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx6.FieldName = "LineTotal";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.gridColumnEx6.Width = 150;
		this.gridColumnEx15.Caption = "Promotion Type";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "U_PromotionType";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 6;
		this.gridColumnEx15.Width = 150;
		this.gridColumnEx16.Caption = "PromotionGroup";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "U_PromotionGroup";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Width = 150;
		this.gridColumnEx17.Caption = "PromotionPack";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "U_PromotionPack";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Width = 150;
		this.gridColumnEx18.Caption = "PromotionLine";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "U_PromotionLine";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Width = 150;
		this.gridColumnEx19.Caption = "ProfitGroupID";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "U_ProfitGroupID";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.xtraTabPage2.Controls.Add(this.grd_Benefit);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(952, 281);
		this.xtraTabPage2.Text = "Benefit";
		this.grd_Benefit.AllowPasteExcel = false;
		this.grd_Benefit.ClipboardData = null;
		this.grd_Benefit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Benefit.Location = new System.Drawing.Point(0, 0);
		this.grd_Benefit.MainView = this.gv_Benefit;
		this.grd_Benefit.Name = "grd_Benefit";
		this.grd_Benefit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_AppliedType });
		this.grd_Benefit.Size = new System.Drawing.Size(952, 281);
		this.grd_Benefit.TabIndex = 1;
		this.grd_Benefit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Benefit });
		this.gv_Benefit.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Benefit.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Benefit.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Benefit.ColumnKey = null;
		this.gv_Benefit.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx26, this.gridColumnEx27, this.gridColumnEx28, this.gridColumnEx29, this.gridColumnEx30, this.gridColumnEx31, this.gridColumnEx32, this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35 });
		this.gv_Benefit.DisableFormSetting = false;
		this.gv_Benefit.DisplayException = false;
		this.gv_Benefit.EditColumn = null;
		this.gv_Benefit.FieldCurrency = null;
		this.gv_Benefit.FieldRate = null;
		this.gv_Benefit.FormId = "CheckPromotion";
		this.gv_Benefit.GridControl = this.grd_Benefit;
		this.gv_Benefit.GroupCount = 1;
		this.gv_Benefit.IgnoreFilter = false;
		this.gv_Benefit.IndexEditRow = -1;
		this.gv_Benefit.IndicatorWidth = 35;
		this.gv_Benefit.IsApzTable = false;
		this.gv_Benefit.IsInsertNewRow = false;
		this.gv_Benefit.IsLoadingData = false;
		this.gv_Benefit.IsMappingContextAction = false;
		this.gv_Benefit.ItemId = "PMOB";
		this.gv_Benefit.LstColumnAllowBatch = null;
		this.gv_Benefit.LstColumnNotAllowBatch = null;
		this.gv_Benefit.LstColumnNotEditable = null;
		this.gv_Benefit.LstColumNotSetting = null;
		this.gv_Benefit.Name = "gv_Benefit";
		this.gv_Benefit.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Benefit.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Benefit.OptionsBehavior.ReadOnly = true;
		this.gv_Benefit.OptionsPrint.AutoWidth = false;
		this.gv_Benefit.OptionsPrint.UsePrintStyles = false;
		this.gv_Benefit.OptionsSelection.MultiSelect = true;
		this.gv_Benefit.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Benefit.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Benefit.OptionsView.ColumnAutoWidth = false;
		this.gv_Benefit.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Benefit.OptionsView.ShowGroupedColumns = true;
		this.gv_Benefit.OptionsView.ShowGroupPanel = false;
		this.gv_Benefit.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx34, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Benefit.UseBatchMod = false;
		this.gridColumnEx26.Caption = "Program Name";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "ProgramName";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 7;
		this.gridColumnEx26.Width = 100;
		this.gridColumnEx27.Caption = "Profit Code";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "ProfitCode";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 10;
		this.gridColumnEx27.Width = 150;
		this.gridColumnEx28.Caption = "Note";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "Note";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 1;
		this.gridColumnEx28.Width = 150;
		this.gridColumnEx29.Caption = "Type";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "TypeNote";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 2;
		this.gridColumnEx29.Width = 150;
		this.gridColumnEx30.Caption = "Actual Value";
		this.gridColumnEx30.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx30.FieldName = "ActualVal";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 3;
		this.gridColumnEx30.Width = 150;
		this.gridColumnEx31.Caption = "Item Code";
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "ItemCode";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 4;
		this.gridColumnEx31.Width = 150;
		this.gridColumnEx32.Caption = "ItemOnHandQty";
		this.gridColumnEx32.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx32.FieldName = "Item On Hand Qty";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 5;
		this.gridColumnEx32.Width = 150;
		this.gridColumnEx33.Caption = "Unit Price";
		this.gridColumnEx33.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx33.FieldName = "UnitPrice";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 6;
		this.gridColumnEx33.Width = 150;
		this.gridColumnEx34.Caption = "Target type";
		this.gridColumnEx34.ColumnEdit = this.rep_AppliedType;
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "AppliedType";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 8;
		this.rep_AppliedType.AutoHeight = false;
		this.rep_AppliedType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AppliedType.Name = "rep_AppliedType";
		this.rep_AppliedType.NullText = "";
		this.gridColumnEx35.Caption = "LineNum";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "LineNum";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Visible = true;
		this.gridColumnEx35.VisibleIndex = 9;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(954, 649);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "CheckPromotion";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_CheckPromotion.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CheckPromotion";
		base.ShowInTaskbar = false;
		this.Text = "Check Promotion";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_CheckPromotion_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Preview).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Preview).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AppliedType).EndInit();
		base.ResumeLayout(false);
	}
}
