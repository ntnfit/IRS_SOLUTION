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
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Promotions;

public class frm_PromotionDefinition : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _benefitInitColumnCount;

	public Action ReloadAfterSave;

	private Dictionary<int, Color> dicPackColor = new Dictionary<int, Color>
	{
		{
			1,
			Color.AliceBlue
		},
		{
			2,
			Color.AntiqueWhite
		},
		{
			3,
			Color.Aquamarine
		},
		{
			4,
			Color.LightGreen
		},
		{
			5,
			Color.Orchid
		},
		{
			6,
			Color.Salmon
		},
		{
			7,
			Color.SteelBlue
		},
		{
			8,
			Color.Gold
		},
		{
			9,
			Color.RoyalBlue
		},
		{
			10,
			Color.Olive
		}
	};

	public bool CloseFormAfterSave;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	private SimpleButton btn_OK;

	private TextEditEx txt_Name;

	private LabelLine lb_Name;

	private TextEditEx txt_Code;

	private LabelLine lb_Code;

	private DateEditEx date_ToDate;

	private LabelLine lb_ToDate;

	private DateEditEx date_FromDate;

	private LabelLine lb_FromDate;

	private CheckEditEx chk_Status;

	private LabelLine lb_Status;

	private SimpleButton btn_Cancel;

	private LookUpEditEx look_StartType;

	private LabelLine labelLine1;

	private TextEditEx txt_BenefitDescription;

	private LabelLine labelLine3;

	private TextEditEx txt_ConditionDescription;

	private LabelLine labelLine2;

	private LookUpEditEx look_Group;

	private LabelLine labelLine6;

	private LookUpEditEx look_Type;

	private LabelLine labelLine5;

	private LookUpEditEx look_Catetory;

	private LabelLine labelLine4;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private CheckEditEx chk_AllowOverlap;

	private CheckEditEx chk_SplitGroup;

	private GridControlEx grd_Condition;

	private GridViewEx gv_Condition;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemButtonEditCFL rep_ConditionCode;

	private GridControlEx grd_Benefit;

	private GridViewEx gv_Benefit;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEditCFL rep_BenefitCode;

	private RepositoryItemTextEdit rep_GroupCode;

	private GridColumnEx gridColumnEx2;

	private DataTable BenefitData { get; set; }

	private DataTable PivotBenefitData { get; set; }

	private DataTable ConditionData { get; set; }

	public string FromCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButton simpleButton4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton4.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButton simpleButton3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton3.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButton simpleButton2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton2.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButton simpleButton = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButton.Enabled = enabled;
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

	public frm_PromotionDefinition()
	{
		InitializeComponent();
		try
		{
			GridViewEx gridViewEx = gv_Condition;
			GridViewEx gridViewEx2 = gv_Benefit;
			string text = (FormId = ApzObjectTypes.oPromotionDefinition.ToString());
			string formId = (gridViewEx2.FormId = text);
			gridViewEx.FormId = formId;
			gv_Condition.ItemId = "APZ_D_PMO1";
			gv_Benefit.ItemId = "APZ_D_PMO2";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
			{
				await AddNew();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await AddNew();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private async Task AddNew()
	{
		try
		{
			FromCode = "";
			DataTable obj = DataSource as DataTable;
			obj.Rows.Clear();
			obj.AcceptChanges();
			await LoadData();
			if (string.IsNullOrEmpty(FromCode))
			{
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_PromotionDefinition_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			rep_ConditionCode.TypeCFL = Enums.TypeCFL.PromotionCondition;
			rep_ConditionCode.ValueMember = "Code";
			rep_ConditionCode.DisplayMember = "Name";
			rep_ConditionCode.TypeDrillDown = ApzObjectTypes.oPromotionCondition;
			await rep_ConditionCode.RefreshDataSourceAsync();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_ConditionCode;
			repositoryItemButtonEditCFL.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.ItemSelected, new Action<DataRow, GridView>(ConditionCode_ItemSelected));
			rep_BenefitCode.TypeCFL = Enums.TypeCFL.PromotionBenefit;
			rep_BenefitCode.ValueMember = "Code";
			rep_BenefitCode.DisplayMember = "Name";
			rep_BenefitCode.TypeDrillDown = ApzObjectTypes.oPromotionBenefit;
			await rep_BenefitCode.RefreshDataSourceAsync();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_BenefitCode;
			repositoryItemButtonEditCFL2.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL2.ItemSelected, new Action<DataRow, GridView>(BenefitCode_ItemSelected));
			date_FromDate.EditValue = DateTime.Today;
			txt_Code.Focus();
			look_StartType.SetDataSource(new Dictionary<string, string>
			{
				{ "S", "Theo ngày hiệu lực" },
				{ "R", "Theo ngày đăng ký" }
			});
			await LoadCategory();
			await LoadData();
			if (string.IsNullOrEmpty(FromCode))
			{
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BenefitCode_ItemSelected(DataRow row, GridView arg2)
	{
		try
		{
			string value = Function.ToString(row["Code"]);
			gv_Benefit.SetFocusedRowCellValue(gv_Benefit.FocusedColumn, value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		DataTable dataTable = await _httpClient.Promotion.GetHeader(FromCode);
		if (dataTable.Columns.Count > 0)
		{
			if (!dataTable.IsNotNull())
			{
				dataTable.Rows.Add(dataTable.NewRow());
			}
			DataSource = dataTable;
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
		await LoadHeader();
		await LoadBenefit();
		await LoadCondition();
	}

	private async Task LoadCategory()
	{
		try
		{
			DataSet dataSet = await _httpClient.PromotionSetupClient.GetData();
			if (dataSet.Tables.Count > 2)
			{
				look_Catetory.SetDataSource(dataSet.Tables[0], "Name", "ID", emptyRow: true);
				look_Type.SetDataSource(dataSet.Tables[1], "Name", "ID", emptyRow: true);
				look_Group.SetDataSource(dataSet.Tables[2], "Name", "ID", emptyRow: true);
			}
			else
			{
				Logging.Write("ERROR", "could not get category dataa");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadHeader()
	{
		try
		{
			DataTable dataTable = await _httpClient.Promotion.GetHeader(FromCode);
			if (dataTable.Rows.Count == 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["FromDate"] = DateTime.Today;
				dataRow["ToDate"] = DateTime.Today;
				dataRow["Status"] = "Y";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			DataSource = dataTable;
			txt_Code.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCondition()
	{
		try
		{
			ConditionData = await _httpClient.Promotion.LoadCondition(FromCode);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("GroupCode", typeof(string));
			dataTable.Columns.Add("PackId", typeof(int));
			dataTable.Columns.Add("Id", typeof(int));
			int num = 0;
			if (ConditionData.IsNotNull())
			{
				IEnumerable<IGrouping<int, DataRow>> enumerable = from t in ConditionData.AsEnumerable()
					group t by Function.ParseInt(t["FatherId"]);
				num = enumerable.Select((IGrouping<int, DataRow> t) => t.Count()).Max();
				if (num > 0)
				{
					for (int num2 = 0; num2 < num; num2++)
					{
						dataTable.Columns.Add("ConditionCode" + (num2 + 1), typeof(string));
						gv_Condition.Columns.Add(new GridColumnEx
						{
							Caption = "Điều kiện " + (num2 + 1),
							FieldName = "ConditionCode" + (num2 + 1),
							ColumnEdit = rep_ConditionCode,
							Visible = true,
							Width = 150
						});
					}
				}
				foreach (IGrouping<int, DataRow> item in enumerable)
				{
					DataRow[] array = item.ToArray();
					int num3 = 0;
					DataRow dataRow = dataTable.NewRow();
					dataRow["GroupCode"] = Function.ToString(array[0]["GroupCode"]);
					dataRow["PackId"] = Function.ParseInt(array[0]["PackId"]);
					DataRow[] array2 = array;
					foreach (DataRow dataRow2 in array2)
					{
						dataRow["Id"] = Function.ParseInt(dataRow2["FatherId"]);
						dataRow["ConditionCode" + (num3 + 1)] = Function.ToString(dataRow2["ConditionCode"]);
						num3++;
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.Columns.Add("ConditionCode" + (num + 1), typeof(string));
			int num5 = (dataTable.IsNotNull() ? dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Id"])) : 0);
			if (dataTable.IsNotNull())
			{
				dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["PackId"]));
			}
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Id"] = num5 + 1;
			dataRow3["PackId"] = 1;
			dataTable.Rows.Add(dataRow3);
			gv_Condition.Columns.Add(new GridColumnEx
			{
				Caption = "Điều kiện " + (num + 1),
				FieldName = "ConditionCode" + (num + 1),
				ColumnEdit = rep_ConditionCode,
				Visible = true,
				Width = 150
			});
			grd_Condition.DataSource = dataTable;
			grd_Condition.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable InitNewBenefitTable()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Id", typeof(int));
		dataTable.Columns.Add("GroupId", typeof(int));
		dataTable.Columns.Add("PackId", typeof(int));
		dataTable.Columns.Add("GroupCode", typeof(string));
		_benefitInitColumnCount = dataTable.Columns.Count;
		return dataTable;
	}

	private async Task LoadBenefit()
	{
		try
		{
			BenefitData = await _httpClient.Promotion.LoadBenefit(FromCode);
			PivotBenefitData = InitNewBenefitTable();
			if (!BenefitData.IsNotNull())
			{
				return;
			}
			var enumerable = from t in BenefitData.AsEnumerable()
				group t by new
				{
					groupCode = Function.ToString(t["GroupCode"]),
					fatherId = Function.ParseInt(t["FatherId"]),
					packId = Function.ParseInt(t["PackId"])
				} into t
				select new
				{
					groupCode = t.Key.groupCode,
					fatherId = t.Key.fatherId,
					packId = t.Key.packId,
					items = t.ToList()
				};
			int num = enumerable.Select(t => t.items.Count).Max();
			if (num > 0)
			{
				for (int num2 = 0; num2 < num; num2++)
				{
					PivotBenefitData.Columns.Add("BenefitCode" + (num2 + 1), typeof(string));
					gv_Benefit.Columns.Add(new GridColumnEx
					{
						Caption = "Lợi ích " + (num2 + 1),
						FieldName = "BenefitCode" + (num2 + 1),
						ColumnEdit = rep_ConditionCode,
						Visible = true,
						Width = 150
					});
				}
			}
			foreach (var item in enumerable)
			{
				int num3 = 0;
				DataRow dataRow = PivotBenefitData.NewRow();
				dataRow["GroupCode"] = Function.ToString(item.groupCode);
				dataRow["PackId"] = Function.ParseInt(item.packId);
				foreach (DataRow item2 in item.items)
				{
					dataRow["BenefitCode" + (num3 + 1)] = Function.ToString(item2["BenefitCode"]);
					num3++;
				}
				PivotBenefitData.Rows.Add(dataRow);
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
		Enums.FormViewModeType currMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonOK || FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				FormMode = currMode;
				return;
			}
			DataSet dataset = GetDataset();
			HttpResult httpResult = await _httpClient.Promotion.Save(dataset);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				FormMode = currMode;
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			FormMode = Enums.FormViewModeType.View;
			if (ReloadAfterSave != null)
			{
				ReloadAfterSave();
			}
			if (CloseFormAfterSave)
			{
				DialogResult = DialogResult.Yes;
			}
			return;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currMode;
	}

	private DataSet GetDataset()
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = DataSource as DataTable;
		if (!dataTable.ExistsColumn("OldCode"))
		{
			dataTable.Columns.Add("OldCode", typeof(string));
		}
		if (dataTable.IsNotNull())
		{
			dataTable.Rows[0]["OldCode"] = FromCode;
			DataTable dataSourceTable = grd_Condition.DataSourceTable;
			int num = (from DataColumn t in dataSourceTable.Columns
				where t.ColumnName.StartsWith("ConditionCode")
				select t.ColumnName.Replace("ConditionCode", "")).ToList().Max((string t) => Function.ParseInt(t));
			DataTable dataTable2 = ConditionData.Clone();
			int num2 = 0;
			for (int num3 = 0; num3 < dataSourceTable.Rows.Count; num3++)
			{
				for (int num4 = 1; num4 <= num; num4++)
				{
					string value = Function.ToString(dataSourceTable.Rows[num3]["ConditionCode" + num4]);
					if (!string.IsNullOrEmpty(value))
					{
						DataRow dataRow = dataTable2.NewRow();
						dataRow["ConditionCode"] = value;
						dataRow["Id"] = num2;
						dataRow["FatherId"] = Function.ParseInt(dataSourceTable.Rows[num3]["Id"]);
						dataRow["GroupCode"] = Function.ToString(dataSourceTable.Rows[num3]["GroupCode"]);
						dataRow["PackId"] = Function.ParseInt(dataSourceTable.Rows[num3]["PackId"]);
						num2++;
						dataTable2.Rows.Add(dataRow);
					}
				}
			}
			InsertDataToTableBenefit(grd_Benefit.DataSourceTable);
			num = (from DataColumn t in PivotBenefitData.Columns
				where t.ColumnName.StartsWith("BenefitCode")
				select t.ColumnName.Replace("BenefitCode", "")).ToList().Max((string t) => Function.ParseInt(t));
			DataTable dataTable3 = BenefitData.Clone();
			num2 = 0;
			foreach (var item in from t in PivotBenefitData.AsEnumerable()
				group t by new
				{
					groupCode = Function.ToString(t["GroupCode"]),
					packId = Function.ParseInt(t["PackId"])
				} into t
				select new
				{
					groupCode = t.Key.groupCode,
					packId = t.Key.packId,
					items = t.ToList()
				})
			{
				foreach (DataRow item2 in item.items)
				{
					for (int num5 = 1; num5 <= num; num5++)
					{
						string value2 = Function.ToString(item2["BenefitCode" + num5]);
						if (!string.IsNullOrEmpty(value2))
						{
							DataRow dataRow2 = dataTable3.NewRow();
							dataRow2["BenefitCode"] = value2;
							dataRow2["Id"] = num2;
							dataRow2["GroupId"] = Function.ParseInt(item2["GroupId"]);
							dataRow2["PackId"] = item.packId;
							dataRow2["GroupCode"] = item.groupCode;
							num2++;
							dataTable3.Rows.Add(dataRow2);
						}
					}
				}
			}
			dataSet.Tables.AddRange(new DataTable[3]
			{
				dataTable.Copy(),
				dataTable2.Copy(),
				dataTable3.Copy()
			});
			return dataSet;
		}
		throw new Exception("Dữ liệu không hợp lệ - header");
	}

	private int GetGroupId(TreeListNode node)
	{
		try
		{
			if (node != null)
			{
				if (Function.ToString(node.GetValue("Type")) == "G")
				{
					return Function.ParseInt(node.GetValue("Id"));
				}
				return GetGroupId(node.ParentNode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void txt_Code_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeViewMode()
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txt_Name_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_FromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_ToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Status_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_StartType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_ConditionDescription_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_BenefitDescription_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_Catetory_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_Type_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_Group_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeViewMode();
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
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_PromotionDefinition_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Condition_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Condition.DataSourceTable;
			int num = (from DataColumn t in dataSourceTable.Columns
				where t.ColumnName.StartsWith("ConditionCode")
				select t.ColumnName.Replace("ConditionCode", "")).ToList().Max((string t) => Function.ParseInt(t));
			if (!string.IsNullOrEmpty(Function.ToString(gv_Condition.GetFocusedRowCellValue("ConditionCode" + num))))
			{
				dataSourceTable.Columns.Add("ConditionCode" + (num + 1), typeof(string));
				gv_Condition.Columns.Add(new GridColumnEx
				{
					Caption = "Điều kiện " + (num + 1),
					FieldName = "ConditionCode" + (num + 1),
					ColumnEdit = rep_ConditionCode,
					Visible = true,
					Width = 150
				});
			}
			bool flag = false;
			for (int num2 = 0; num2 < dataSourceTable.Rows.Count; num2++)
			{
				bool flag2 = true;
				for (int num3 = 1; num3 <= num; num3++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[num2]["ConditionCode" + num3])))
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				int num4 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Id"]));
				dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["PackId"]));
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["Id"] = num4 + 1;
				dataRow["PackId"] = 1;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
			if (e.Column.FieldName == "GroupCode")
			{
				foreach (DataRow row in grd_Benefit.DataSourceTable.Rows)
				{
					row["GroupCode"] = Function.ToString(e.Value);
				}
			}
			if (e.Column.FieldName == "PackId")
			{
				foreach (DataRow row2 in grd_Benefit.DataSourceTable.Rows)
				{
					row2["PackId"] = Function.ParseInt(e.Value);
				}
			}
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ConditionCode_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			string value = Function.ToString(row["Code"]);
			gv_Condition.SetFocusedRowCellValue(gv_Condition.FocusedColumn, value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Benefit_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			NewEmptyBenefit();
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewEmptyBenefit()
	{
		DataTable dataSourceTable = grd_Benefit.DataSourceTable;
		List<string> list = (from DataColumn t in dataSourceTable.Columns
			where t.ColumnName.StartsWith("BenefitCode")
			select t.ColumnName.Replace("BenefitCode", "")).ToList();
		int maxId = 0;
		if (list.Count > 0)
		{
			maxId = list.Max((string t) => Function.ParseInt(t));
		}
		string value = Function.ToString(gv_Benefit.GetFocusedRowCellValue("BenefitCode" + maxId));
		if (!gv_Benefit.Columns.Any((GridColumn t) => t.FieldName == "BenefitCode" + maxId) || !string.IsNullOrEmpty(value))
		{
			dataSourceTable.Columns.Add("BenefitCode" + (maxId + 1), typeof(string));
			gv_Benefit.Columns.Add(new GridColumnEx
			{
				Caption = "Lợi ích " + (maxId + 1),
				FieldName = "BenefitCode" + (maxId + 1),
				ColumnEdit = rep_BenefitCode,
				Visible = true,
				Width = 150
			});
		}
		bool flag = false;
		for (int num = 0; num < dataSourceTable.Rows.Count; num++)
		{
			bool flag2 = true;
			for (int num2 = 1; num2 <= maxId; num2++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[num]["BenefitCode" + num2])))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			int num3 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Id"]));
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Id"] = num3 + 1;
			dataRow["GroupId"] = Function.ParseInt(gv_Condition.GetFocusedRowCellValue("Id"));
			dataRow["PackId"] = Function.ParseInt(gv_Condition.GetFocusedRowCellValue("PackId"));
			dataRow["GroupCode"] = Function.ToString(gv_Condition.GetFocusedRowCellValue("GroupCode"));
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
		}
	}

	private void gv_Condition_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int groupId = Function.ParseInt(gv_Condition.GetFocusedRowCellValue("Id"));
			int packId = Function.ParseInt(gv_Condition.GetFocusedRowCellValue("PackId"));
			string groupCode = Function.ToString(gv_Condition.GetFocusedRowCellValue("GroupCode"));
			SetFocusLine(groupId, packId, groupCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetFocusLine(int groupId, int packId, string groupCode)
	{
		try
		{
			InsertDataToTableBenefit(grd_Benefit.DataSourceTable.Copy());
			grd_Benefit.DataSourceTable.Rows.Clear();
			grd_Benefit.DataSourceTable.AcceptChanges();
			if (PivotBenefitData == null || PivotBenefitData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable = PivotBenefitData.Clone();
			DataRow[] array = (from t in PivotBenefitData.AsEnumerable()
				where Function.ToString(t["GroupCode"]) == groupCode && Function.ParseInt(t["PackId"]) == packId
				select (t)).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = array[num].ItemArray;
				dataTable.Rows.Add(dataRow);
			}
			if (!dataTable.IsNotNull())
			{
				dataTable = InitNewBenefitTable();
				dataTable.Columns.Add("BenefitCode1", typeof(string));
				int count = (from t in gv_Benefit.Columns
					where t.FieldName.StartsWith("BenefitCode")
					select t.FieldName.Replace("BenefitCode", "")).ToList().Count;
				for (int num2 = 2; num2 <= count; num2++)
				{
					gv_Benefit.Columns.Remove(gv_Benefit.Columns["BenefitCode" + num2]);
				}
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["GroupId"] = groupId;
				dataRow2["GroupCode"] = groupCode;
				dataRow2["PackId"] = packId;
				dataTable.Rows.Add(dataRow2);
			}
			else
			{
				int num3 = 0;
				for (int num4 = 0; num4 < dataTable.Rows.Count; num4++)
				{
					for (int num5 = dataTable.Columns.Count - 1; num5 >= 4; num5--)
					{
						if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num4][num5])))
						{
							if (num3 < num5)
							{
								num3 = num5;
							}
							break;
						}
					}
				}
				if (num3 == 0)
				{
					num3 = dataTable.Columns.Count;
				}
				List<DataColumn> list = dataTable.Columns.Cast<DataColumn>().Skip(num3 + 1).Take(dataTable.Columns.Count - num3 - 1)
					.ToList();
				if (list.Count > 0)
				{
					for (int num6 = 0; num6 < list.Count; num6++)
					{
						dataTable.Columns.Remove(list[num6]);
					}
				}
			}
			gv_Benefit.Columns.Clear();
			for (int num7 = 0; num7 < dataTable.Columns.Count - _benefitInitColumnCount; num7++)
			{
				gv_Benefit.Columns.Add(new GridColumnEx
				{
					Caption = "Lợi ích " + (num7 + 1),
					FieldName = "BenefitCode" + (num7 + 1),
					ColumnEdit = rep_BenefitCode,
					Visible = true,
					Width = 150
				});
			}
			dataTable.AcceptChanges();
			grd_Benefit.DataSource = dataTable;
			grd_Benefit.RefreshDataSource();
			NewEmptyBenefit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InsertDataToTableBenefit(DataTable dt)
	{
		if (PivotBenefitData == null || PivotBenefitData.Columns.Count == 0 || dt == null || dt.Rows.Count == 0)
		{
			return;
		}
		DataRow[] array = (from t in PivotBenefitData.AsEnumerable()
			where Function.ToString(t["GroupCode"]) == Function.ToString(dt.Rows[0]["GroupCode"]) && Function.ParseInt(t["PackId"]) == Function.ParseInt(dt.Rows[0]["PackId"])
			select (t)).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			PivotBenefitData.Rows.Remove(array[num]);
		}
		if (dt.Columns.Count > PivotBenefitData.Columns.Count)
		{
			for (int num2 = PivotBenefitData.Columns.Count - _benefitInitColumnCount; num2 < dt.Columns.Count - _benefitInitColumnCount; num2++)
			{
				PivotBenefitData.Columns.Add("BenefitCode" + (num2 + 1));
			}
		}
		if (dt.Columns.Count < PivotBenefitData.Columns.Count)
		{
			for (int num3 = dt.Columns.Count - _benefitInitColumnCount; num3 < PivotBenefitData.Columns.Count - _benefitInitColumnCount; num3++)
			{
				dt.Columns.Add("BenefitCode" + (num3 + 1));
			}
		}
		for (int num4 = 0; num4 < dt.Rows.Count; num4++)
		{
			DataRow dataRow = PivotBenefitData.NewRow();
			dataRow.ItemArray = dt.Rows[num4].ItemArray;
			PivotBenefitData.Rows.Add(dataRow);
		}
		PivotBenefitData.AcceptChanges();
	}

	private void gv_Condition_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			if (!(gv_Condition.FocusedColumn.FieldName == "GroupCode") || !((sender as GridView).ActiveEditor is TextEdit textEdit))
			{
				return;
			}
			DataTable dataSourceTable = grd_Condition.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
				autoCompleteStringCollection.AddRange((from t in dataSourceTable.AsEnumerable()
					select Function.ToString(t["GroupCode"])).ToArray());
				textEdit.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				textEdit.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
				textEdit.MaskBox.AutoCompleteCustomSource = autoCompleteStringCollection;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void gv_Condition_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Condition.GetRowCellValue(e.RowHandle, "PackId"));
			if (num == 0)
			{
				return;
			}
			if (dicPackColor.ContainsKey(num))
			{
				e.Appearance.BackColor = dicPackColor[num];
				return;
			}
			num = GetPackId(num);
			if (dicPackColor.ContainsKey(num))
			{
				e.Appearance.BackColor = dicPackColor[num];
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private int GetPackId(int packId)
	{
		try
		{
			if (packId > 10)
			{
				packId -= 10;
				return GetPackId(packId);
			}
			return packId;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		return 0;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Promotions.frm_PromotionDefinition));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.chk_SplitGroup = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_AllowOverlap = new Apzon.Controls.Editor.CheckEditEx();
		this.look_Group = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.look_Type = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.look_Catetory = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BenefitDescription = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ConditionDescription = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.look_StartType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.chk_Status = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Code = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Condition = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Condition = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_GroupCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ConditionCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.grd_Benefit = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Benefit = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BenefitCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_SplitGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowOverlap.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Group.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Catetory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BenefitDescription.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ConditionDescription.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_StartType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Condition).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Condition).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GroupCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConditionCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BenefitCode).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_SplitGroup);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_AllowOverlap);
		this.splitContainerControl1.Panel1.Controls.Add(this.look_Group);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine6);
		this.splitContainerControl1.Panel1.Controls.Add(this.look_Type);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine5);
		this.splitContainerControl1.Panel1.Controls.Add(this.look_Catetory);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_BenefitDescription);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_ConditionDescription);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Controls.Add(this.look_StartType);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_Status);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_Status);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_ToDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_ToDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_FromDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_FromDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_Name);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_Name);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_Code);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_Code);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(912, 626);
		this.splitContainerControl1.SplitterPosition = 166;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.chk_SplitGroup.EditControl = null;
		this.chk_SplitGroup.EditValue = "N";
		this.chk_SplitGroup.FieldName = "SplitGroup";
		this.chk_SplitGroup.IsSearchControl = false;
		this.chk_SplitGroup.Location = new System.Drawing.Point(626, 140);
		this.chk_SplitGroup.Name = "chk_SplitGroup";
		this.chk_SplitGroup.Properties.Caption = "CT được chia thành gói nhỏ";
		this.chk_SplitGroup.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.chk_SplitGroup.Properties.ValueChecked = "Y";
		this.chk_SplitGroup.Properties.ValueGrayed = "N";
		this.chk_SplitGroup.Properties.ValueUnchecked = "N";
		this.chk_SplitGroup.SettingInVisible = false;
		this.chk_SplitGroup.SettingReadonly = false;
		this.chk_SplitGroup.Size = new System.Drawing.Size(241, 20);
		this.chk_SplitGroup.TabIndex = 23;
		this.chk_AllowOverlap.EditControl = null;
		this.chk_AllowOverlap.EditValue = "N";
		this.chk_AllowOverlap.FieldName = "AllowOverlap";
		this.chk_AllowOverlap.IsSearchControl = false;
		this.chk_AllowOverlap.Location = new System.Drawing.Point(396, 143);
		this.chk_AllowOverlap.Name = "chk_AllowOverlap";
		this.chk_AllowOverlap.Properties.Caption = "Cho phép áp dụng chồng với CTKM khác";
		this.chk_AllowOverlap.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.chk_AllowOverlap.Properties.ValueChecked = "Y";
		this.chk_AllowOverlap.Properties.ValueGrayed = "N";
		this.chk_AllowOverlap.Properties.ValueUnchecked = "N";
		this.chk_AllowOverlap.SettingInVisible = false;
		this.chk_AllowOverlap.SettingReadonly = false;
		this.chk_AllowOverlap.Size = new System.Drawing.Size(241, 20);
		this.chk_AllowOverlap.TabIndex = 22;
		this.look_Group.EditControl = null;
		this.look_Group.FieldName = "GroupID";
		this.look_Group.IsSearchControl = false;
		this.look_Group.Location = new System.Drawing.Point(529, 114);
		this.look_Group.Name = "look_Group";
		this.look_Group.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Group.Properties.DisplayMember = "Name";
		this.look_Group.Properties.NullText = "";
		this.look_Group.Properties.ValueMember = "Code";
		this.look_Group.SettingInVisible = false;
		this.look_Group.SettingReadonly = false;
		this.look_Group.Size = new System.Drawing.Size(250, 20);
		this.look_Group.TabIndex = 21;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Nhóm";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(399, 117);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(130, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 20;
		this.labelLine6.Utext = "Nhóm";
		this.look_Type.EditControl = null;
		this.look_Type.FieldName = "TypeID";
		this.look_Type.IsSearchControl = false;
		this.look_Type.Location = new System.Drawing.Point(529, 88);
		this.look_Type.Name = "look_Type";
		this.look_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Type.Properties.DisplayMember = "Name";
		this.look_Type.Properties.NullText = "";
		this.look_Type.Properties.ValueMember = "Code";
		this.look_Type.SettingInVisible = false;
		this.look_Type.SettingReadonly = false;
		this.look_Type.Size = new System.Drawing.Size(250, 20);
		this.look_Type.TabIndex = 19;
		this.look_Type.EditValueChanged += new System.EventHandler(this.look_Type_EditValueChanged);
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Kiểu";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(399, 91);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(130, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 18;
		this.labelLine5.Utext = "Kiểu";
		this.look_Catetory.EditControl = null;
		this.look_Catetory.FieldName = "CategoryID";
		this.look_Catetory.IsSearchControl = false;
		this.look_Catetory.Location = new System.Drawing.Point(529, 62);
		this.look_Catetory.Name = "look_Catetory";
		this.look_Catetory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Catetory.Properties.DisplayMember = "Name";
		this.look_Catetory.Properties.NullText = "";
		this.look_Catetory.Properties.ValueMember = "Code";
		this.look_Catetory.SettingInVisible = false;
		this.look_Catetory.SettingReadonly = false;
		this.look_Catetory.Size = new System.Drawing.Size(250, 20);
		this.look_Catetory.TabIndex = 17;
		this.look_Catetory.EditValueChanged += new System.EventHandler(this.look_Catetory_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Loại";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(399, 65);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(130, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 16;
		this.labelLine4.Utext = "Loại";
		this.txt_BenefitDescription.EditControl = null;
		this.txt_BenefitDescription.FieldName = "BenefitDesc";
		this.txt_BenefitDescription.IsSearchControl = false;
		this.txt_BenefitDescription.Location = new System.Drawing.Point(529, 35);
		this.txt_BenefitDescription.Name = "txt_BenefitDescription";
		this.txt_BenefitDescription.SettingInVisible = false;
		this.txt_BenefitDescription.SettingReadonly = false;
		this.txt_BenefitDescription.Size = new System.Drawing.Size(371, 20);
		this.txt_BenefitDescription.TabIndex = 15;
		this.txt_BenefitDescription.EditValueChanged += new System.EventHandler(this.txt_BenefitDescription_EditValueChanged);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Mô tả lợi ích";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(399, 38);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(130, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 14;
		this.labelLine3.Utext = "Mô tả lợi ích";
		this.txt_ConditionDescription.EditControl = null;
		this.txt_ConditionDescription.FieldName = "ConditionDesc";
		this.txt_ConditionDescription.IsSearchControl = false;
		this.txt_ConditionDescription.Location = new System.Drawing.Point(529, 9);
		this.txt_ConditionDescription.Name = "txt_ConditionDescription";
		this.txt_ConditionDescription.SettingInVisible = false;
		this.txt_ConditionDescription.SettingReadonly = false;
		this.txt_ConditionDescription.Size = new System.Drawing.Size(371, 20);
		this.txt_ConditionDescription.TabIndex = 13;
		this.txt_ConditionDescription.EditValueChanged += new System.EventHandler(this.txt_ConditionDescription_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Mô tả điều kiện";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(399, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(130, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 12;
		this.labelLine2.Utext = "Mô tả điều kiện";
		this.look_StartType.EditControl = null;
		this.look_StartType.FieldName = null;
		this.look_StartType.IsSearchControl = false;
		this.look_StartType.Location = new System.Drawing.Point(143, 114);
		this.look_StartType.Name = "look_StartType";
		this.look_StartType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_StartType.Properties.DisplayMember = "Name";
		this.look_StartType.Properties.NullText = "";
		this.look_StartType.Properties.ValueMember = "Code";
		this.look_StartType.SettingInVisible = false;
		this.look_StartType.SettingReadonly = false;
		this.look_StartType.Size = new System.Drawing.Size(250, 20);
		this.look_StartType.TabIndex = 11;
		this.look_StartType.EditValueChanged += new System.EventHandler(this.look_StartType_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Ngày bắt đầu tính CT";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 117);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(130, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 10;
		this.labelLine1.Utext = "Ngày bắt đầu tính CT";
		this.chk_Status.EditControl = null;
		this.chk_Status.EditValue = "Y";
		this.chk_Status.FieldName = "Status";
		this.chk_Status.IsSearchControl = false;
		this.chk_Status.Location = new System.Drawing.Point(143, 144);
		this.chk_Status.Name = "chk_Status";
		this.chk_Status.Properties.Caption = "Hoạt động";
		this.chk_Status.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.chk_Status.Properties.ValueChecked = "Y";
		this.chk_Status.Properties.ValueGrayed = "N";
		this.chk_Status.Properties.ValueUnchecked = "N";
		this.chk_Status.SettingInVisible = false;
		this.chk_Status.SettingReadonly = false;
		this.chk_Status.Size = new System.Drawing.Size(133, 20);
		this.chk_Status.TabIndex = 5;
		this.chk_Status.CheckedChanged += new System.EventHandler(this.chk_Status_CheckedChanged);
		this.lb_Status.EditControl = null;
		this.lb_Status.EditValue = "Trạng thái";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(13, 143);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(130, 20);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 8;
		this.lb_Status.Utext = "Trạng thái";
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = "ToDate";
		this.date_ToDate.Location = new System.Drawing.Point(143, 88);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(250, 20);
		this.date_ToDate.TabIndex = 4;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_ToDate_EditValueChanged);
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "Ngày hết hiệu lực";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(13, 91);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(130, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 6;
		this.lb_ToDate.Utext = "Ngày hết hiệu lực";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = "FromDate";
		this.date_FromDate.Location = new System.Drawing.Point(143, 62);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(250, 20);
		this.date_FromDate.TabIndex = 3;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "Ngày hiệu lực";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(13, 65);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(130, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 4;
		this.lb_FromDate.Utext = "Ngày hiệu lực";
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = "Name";
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(143, 35);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(250, 20);
		this.txt_Name.TabIndex = 2;
		this.txt_Name.EditValueChanged += new System.EventHandler(this.txt_Name_EditValueChanged);
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "Tên chương trình";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(13, 38);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(130, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 2;
		this.lb_Name.Utext = "Tên chương trình";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "Code";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(143, 9);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(250, 20);
		this.txt_Code.TabIndex = 1;
		this.txt_Code.EditValueChanged += new System.EventHandler(this.txt_Code_EditValueChanged);
		this.lb_Code.EditControl = null;
		this.lb_Code.EditValue = "Mã chương trình";
		this.lb_Code.FieldName = null;
		this.lb_Code.HideWhenEmpty = false;
		this.lb_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Code.Location = new System.Drawing.Point(13, 12);
		this.lb_Code.Name = "lb_Code";
		this.lb_Code.ReadOnly = false;
		this.lb_Code.SettingInVisible = false;
		this.lb_Code.SettingReadonly = false;
		this.lb_Code.ShowUnderLine = true;
		this.lb_Code.Size = new System.Drawing.Size(130, 20);
		this.lb_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Code.TabIndex = 0;
		this.lb_Code.Utext = "Mã chương trình";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Condition);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Benefit);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(912, 450);
		this.splitContainerControl2.SplitterPosition = 227;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_Condition.AllowPasteExcel = true;
		this.grd_Condition.ClipboardData = null;
		this.grd_Condition.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Condition.Location = new System.Drawing.Point(0, 0);
		this.grd_Condition.MainView = this.gv_Condition;
		this.grd_Condition.Name = "grd_Condition";
		this.grd_Condition.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ConditionCode, this.rep_GroupCode });
		this.grd_Condition.Size = new System.Drawing.Size(912, 227);
		this.grd_Condition.TabIndex = 0;
		this.grd_Condition.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Condition });
		this.gv_Condition.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Condition.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Condition.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Condition.ColumnKey = null;
		this.gv_Condition.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx1, this.gridColumnEx2 });
		this.gv_Condition.DisableFormSetting = false;
		this.gv_Condition.DisplayException = false;
		this.gv_Condition.EditColumn = null;
		this.gv_Condition.FieldCurrency = null;
		this.gv_Condition.FieldRate = null;
		this.gv_Condition.FormId = null;
		this.gv_Condition.GridControl = this.grd_Condition;
		this.gv_Condition.IgnoreFilter = false;
		this.gv_Condition.IndexEditRow = -1;
		this.gv_Condition.IndicatorWidth = 35;
		this.gv_Condition.IsApzTable = false;
		this.gv_Condition.IsInsertNewRow = false;
		this.gv_Condition.IsLoadingData = false;
		this.gv_Condition.IsMappingContextAction = false;
		this.gv_Condition.ItemId = null;
		this.gv_Condition.LstColumnAllowBatch = null;
		this.gv_Condition.LstColumnNotAllowBatch = null;
		this.gv_Condition.LstColumnNotEditable = null;
		this.gv_Condition.LstColumNotSetting = null;
		this.gv_Condition.Name = "gv_Condition";
		this.gv_Condition.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Condition.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Condition.OptionsPrint.AutoWidth = false;
		this.gv_Condition.OptionsPrint.UsePrintStyles = false;
		this.gv_Condition.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Condition.OptionsView.ColumnAutoWidth = false;
		this.gv_Condition.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Condition.OptionsView.ShowGroupedColumns = true;
		this.gv_Condition.OptionsView.ShowGroupPanel = false;
		this.gv_Condition.OptionsView.ShowViewCaption = true;
		this.gv_Condition.UseBatchMod = false;
		this.gv_Condition.ViewCaption = "Điều kiện";
		this.gv_Condition.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Condition_RowCellStyle);
		this.gv_Condition.ShownEditor += new System.EventHandler(this.gv_Condition_ShownEditor);
		this.gv_Condition.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Condition_FocusedRowObjectChanged);
		this.gv_Condition.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Condition_CellValueChanged);
		this.gridColumnEx1.Caption = "Tên gói CTKM";
		this.gridColumnEx1.ColumnEdit = this.rep_GroupCode;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "GroupCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 159;
		this.rep_GroupCode.AutoHeight = false;
		this.rep_GroupCode.Name = "rep_GroupCode";
		this.gridColumnEx2.Caption = "Mã nhóm điều kiện";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "PackId";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 120;
		this.rep_ConditionCode.AutoHeight = false;
		this.rep_ConditionCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_ConditionCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ConditionCode.ContainerControl = this;
		this.rep_ConditionCode.DisplayMember = "";
		this.rep_ConditionCode.DisplayMember2 = "";
		this.rep_ConditionCode.LoadDataSourceWhenEnter = false;
		this.rep_ConditionCode.Name = "rep_ConditionCode";
		this.rep_ConditionCode.SeparateCharacterForMulti = ';';
		this.rep_ConditionCode.SeparateString = ", ";
		this.rep_ConditionCode.UsingAutoCompleteBox = false;
		this.rep_ConditionCode.UsingDataSource = true;
		this.rep_ConditionCode.ValidateWhenNoDataSource = false;
		this.rep_ConditionCode.ValueMember = "";
		this.grd_Benefit.AllowPasteExcel = true;
		this.grd_Benefit.ClipboardData = null;
		this.grd_Benefit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Benefit.Location = new System.Drawing.Point(0, 0);
		this.grd_Benefit.MainView = this.gv_Benefit;
		this.grd_Benefit.Name = "grd_Benefit";
		this.grd_Benefit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_BenefitCode });
		this.grd_Benefit.Size = new System.Drawing.Size(912, 213);
		this.grd_Benefit.TabIndex = 1;
		this.grd_Benefit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Benefit });
		this.gv_Benefit.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Benefit.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Benefit.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Benefit.ColumnKey = null;
		this.gv_Benefit.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx4 });
		this.gv_Benefit.DisableFormSetting = false;
		this.gv_Benefit.DisplayException = false;
		this.gv_Benefit.EditColumn = null;
		this.gv_Benefit.FieldCurrency = null;
		this.gv_Benefit.FieldRate = null;
		this.gv_Benefit.FormId = null;
		this.gv_Benefit.GridControl = this.grd_Benefit;
		this.gv_Benefit.IgnoreFilter = false;
		this.gv_Benefit.IndexEditRow = -1;
		this.gv_Benefit.IndicatorWidth = 35;
		this.gv_Benefit.IsApzTable = false;
		this.gv_Benefit.IsInsertNewRow = false;
		this.gv_Benefit.IsLoadingData = false;
		this.gv_Benefit.IsMappingContextAction = false;
		this.gv_Benefit.ItemId = null;
		this.gv_Benefit.LstColumnAllowBatch = null;
		this.gv_Benefit.LstColumnNotAllowBatch = null;
		this.gv_Benefit.LstColumnNotEditable = null;
		this.gv_Benefit.LstColumNotSetting = null;
		this.gv_Benefit.Name = "gv_Benefit";
		this.gv_Benefit.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Benefit.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Benefit.OptionsPrint.AutoWidth = false;
		this.gv_Benefit.OptionsPrint.UsePrintStyles = false;
		this.gv_Benefit.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Benefit.OptionsView.ColumnAutoWidth = false;
		this.gv_Benefit.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Benefit.OptionsView.ShowGroupedColumns = true;
		this.gv_Benefit.OptionsView.ShowGroupPanel = false;
		this.gv_Benefit.OptionsView.ShowViewCaption = true;
		this.gv_Benefit.UseBatchMod = false;
		this.gv_Benefit.ViewCaption = "Lợi  ích";
		this.gv_Benefit.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Benefit_CellValueChanged);
		this.gridColumnEx4.Caption = "Lợi ích 1";
		this.gridColumnEx4.ColumnEdit = this.rep_BenefitCode;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "BenefitCode1";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 0;
		this.gridColumnEx4.Width = 164;
		this.rep_BenefitCode.AutoHeight = false;
		this.rep_BenefitCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BenefitCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BenefitCode.ContainerControl = this;
		this.rep_BenefitCode.DisplayMember = "";
		this.rep_BenefitCode.DisplayMember2 = "";
		this.rep_BenefitCode.LoadDataSourceWhenEnter = false;
		this.rep_BenefitCode.Name = "rep_BenefitCode";
		this.rep_BenefitCode.SeparateCharacterForMulti = ';';
		this.rep_BenefitCode.SeparateString = ", ";
		this.rep_BenefitCode.UsingAutoCompleteBox = false;
		this.rep_BenefitCode.UsingDataSource = true;
		this.rep_BenefitCode.ValidateWhenNoDataSource = false;
		this.rep_BenefitCode.ValueMember = "";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(10, 633);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "Add";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(88, 633);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(97, 26);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
		this.addToolStripMenuItem.Text = "Add";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(912, 665);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PromotionDefinition.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PromotionDefinition";
		base.ShowInTaskbar = false;
		this.Text = "Chương trình khuyến mại";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_PromotionDefinition_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PromotionDefinition_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_SplitGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowOverlap.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Group.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Catetory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BenefitDescription.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ConditionDescription.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_StartType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Condition).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Condition).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GroupCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConditionCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BenefitCode).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
