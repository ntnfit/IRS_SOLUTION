using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Data;

namespace Apzon.Forms.FormTemplate;

public class UcTemplate : XtraUserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly DataTable _validValueStruct = new DataTable();

	public int ObjectId;

	public DataSet WizardData = new DataSet();

	private Color _mandoryColor = Color.Bisque;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	public XtraTabControl StackGroup1;

	public XtraTabControl StackGroup2;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControl layoutControl2;

	private LayoutControlGroup layoutControlGroup2;

	private SplitContainerControl splitContainerControl3;

	private LayoutControl layoutControl4;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControl layoutControl3;

	private LayoutControlGroup Root;

	public List<ucAttachment> Attachments { get; set; }

	public int TemplateId { get; set; }

	public string FormId { get; set; }

	public int SpliterPosition { get; set; }

	public bool HasLeft { get; set; }

	public bool HasRight { get; set; }

	public UcTemplate()
	{
		InitializeComponent();
	}

	private void AddControlToLayoutItem(DataRow item, LayoutControlItem layoutItem)
	{
		try
		{
			int num = layoutControl1.Width - 50;
			string text = Function.ToString(item["DataType"]);
			Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(item["Editor"]);
			string text2 = Function.ToString(item["FieldName"]);
			string text3 = Function.ToString(item["Caption"]);
			bool flag = Function.ToString(item["PrimaryKey"]) == "Y";
			bool flag2 = Function.ToString(item["Mandatory"]) == "Y";
			char? c = Function.ParseChar(item["PasswordChar"]);
			string text4 = Function.ToString(item["SubType"]);
			if (text == "A" && text4 == "T")
			{
				TimeEditEx timeEditEx = new TimeEditEx();
				if (flag2)
				{
					timeEditEx.Properties.Appearance.BackColor = _mandoryColor;
				}
				timeEditEx.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
				timeEditEx.StyleController = layoutControl1;
				layoutItem.Control = timeEditEx;
				timeEditEx.StyleController = layoutControl1;
				layoutItem.Control = timeEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize;
				timeEditEx.Name = text2;
				timeEditEx.FieldName = text2;
				return;
			}
			switch (text)
			{
			case "A":
				switch (formTemplateEditorType)
				{
				case Enums.FormTemplateEditorType.ChooseFromList:
				{
					ButtonEditCFL cfl = new ButtonEditCFL();
					if (flag2)
					{
						cfl.Validating += Editor_Validating;
						cfl.Properties.Appearance.BackColor = _mandoryColor;
					}
					cfl.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
					cfl.Properties.TextEditStyle = ((!(Function.ToString(item["Editable"]) == "Y")) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
					cfl.StyleController = layoutControl1;
					layoutItem.Control = cfl;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize8 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize8;
					Enums.TypeCFL typeCFL = (Enums.TypeCFL)Function.ParseInt(item["CFLType"]);
					cfl.Properties.TypeCFL = typeCFL;
					if (typeCFL == Enums.TypeCFL.UserDefinedData)
					{
						RepositoryItemButtonEditCFL properties = cfl.Properties;
						properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL f) =>
						{
							try
							{
								string text5 = Function.ToString(item["CFLQuery"]);
								if (!string.IsNullOrEmpty(text5))
								{
									Regex regex = new Regex("@@((\\S+)\\s?|(\\S+);?|(\\S+),?|(\\S+)\\n?|(\\S+)\\r\\n?|(\\S+)\\r?|(\\S+)\\Z)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
									text5 = text5.Replace("[@", "##");
									MatchCollection matchCollection = regex.Matches(text5);
									DataTable dataTable = (FindForm() as XtraFormEx)?.DataSource as DataTable;
									if (dataTable.IsNotNull())
									{
										foreach (Match item2 in matchCollection)
										{
											string text6 = item2.Value.Trim().Replace(",", "").Replace(";", "");
											text6 = text6.Split(';')[0];
											if (dataTable.Columns.Contains(text6.Substring(2)))
											{
												object arg = dataTable.Rows[0][text6.Substring(2)];
												text5 = text5.Replace(text6, $"'{arg}'");
												text5 = text5.Replace("##", "[@");
											}
										}
									}
								}
								DataTable executeSqlResult = _httpClient.Commons.GetExecuteSqlResult(text5, new List<QueryParamater>());
								DataSet dataSet = new DataSet
								{
									Tables = { executeSqlResult.Copy() }
								};
								if (f != null)
								{
									f.DataSource = dataSet.Copy();
								}
								if (executeSqlResult.IsNotNull())
								{
									cfl.Properties.DataSource = executeSqlResult;
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", ex2);
								WinCommons.ErrorLineNumber(ex2);
							}
						}));
					}
					cfl.Properties.DisplayMember = Function.ToString(item["DisplayMember"]);
					cfl.Properties.ValueMember = Function.ToString(item["ValueMember"]);
					ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(item["DrillDownType"]);
					if (apzObjectTypes != ApzObjectTypes.oNone)
					{
						cfl.Properties.TypeDrillDown = apzObjectTypes;
					}
					if (Function.ToString(item["MultiSelect"]) == "Y")
					{
						cfl.Properties.MultiSelectMode = true;
						RepositoryItemButtonEditCFL properties2 = cfl.Properties;
						properties2.HandleChooseItemMulti = (Action<List<DataRow>>)Delegate.Combine(properties2.HandleChooseItemMulti, (Action<List<DataRow>>)((List<DataRow> data) =>
						{
						}));
					}
					if (flag)
					{
						cfl.EditValue = ObjectId;
						cfl.EditValueChanged += (object s, EventArgs ev) =>
						{
							try
							{
								ObjectId = Function.ParseInt(cfl.EditValue);
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", ex2);
								WinCommons.ErrorLineNumber(ex2);
							}
						};
					}
					cfl.FieldName = text2;
					cfl.Name = text2;
					if (c.HasValue && c != ' ')
					{
						cfl.Properties.PasswordChar = c.Value;
					}
					break;
				}
				case Enums.FormTemplateEditorType.ValidValue:
				{
					LookUpEditEx lookUpEditEx = new LookUpEditEx();
					if (flag2)
					{
						lookUpEditEx.Validating += Editor_Validating;
						lookUpEditEx.Properties.Appearance.BackColor = _mandoryColor;
					}
					lookUpEditEx.Properties.NullText = string.Empty;
					lookUpEditEx.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
					lookUpEditEx.Properties.TextEditStyle = ((!(Function.ToString(item["Editable"]) == "Y")) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
					lookUpEditEx.StyleController = layoutControl1;
					layoutItem.Control = lookUpEditEx;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize7 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize7;
					DataTable validSource = GetValidSource(item, out var valueMember, out var displayMember);
					lookUpEditEx.SetDataSource(validSource, displayMember, valueMember, emptyRow: true);
					if (flag)
					{
						lookUpEditEx.EditValue = ObjectId;
					}
					lookUpEditEx.Name = text2;
					lookUpEditEx.FieldName = text2;
					if (c.HasValue && c != ' ')
					{
						lookUpEditEx.Properties.PasswordChar = c.Value;
					}
					break;
				}
				case Enums.FormTemplateEditorType.CheckEdit:
				{
					CheckEditEx checkEditEx = new CheckEditEx
					{
						Text = text3,
						Name = text2,
						FieldName = text2,
						ReadOnly = (Function.ToString(item["IsReadOnly"]) == "Y")
					};
					checkEditEx.Properties.ValueChecked = "Y";
					checkEditEx.Properties.ValueUnchecked = "N";
					layoutItem.Control = checkEditEx;
					break;
				}
				case Enums.FormTemplateEditorType.RadioGroup:
				{
					RadioGroupEx radioGroupEx = new RadioGroupEx
					{
						FieldName = text2
					};
					DataRow[] array = (from t in WizardData.Tables["FWZ3"].AsEnumerable()
						where Function.ParseInt(t["LineId"]) == Function.ParseInt(item["LineId"])
						select t).ToArray();
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						radioGroupEx.Properties.Items.Add(new RadioGroupItem(dataRow["Code"], Function.ToString(dataRow["Name"])));
					}
					int num3 = 30;
					if (Function.ToString(item["RGVertical"]) == "Y")
					{
						radioGroupEx.Properties.Columns = 0;
						num3 = array.Length * 30;
					}
					layoutItem.Control = radioGroupEx;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize9 = (layoutItem.MaxSize = new Size(num, num3));
					layoutItem.MinSize = minSize9;
					break;
				}
				default:
				{
					TextEditEx txt = new TextEditEx();
					if (flag2)
					{
						txt.Validating += Editor_Validating;
						txt.Properties.Appearance.BackColor = _mandoryColor;
					}
					txt.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
					txt.Properties.Enabled = Function.ToString(item["Editable"]) == "Y";
					txt.StyleController = layoutControl1;
					layoutItem.Control = txt;
					layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size minSize6 = (layoutItem.MaxSize = new Size(num, 20));
					layoutItem.MinSize = minSize6;
					txt.FieldName = text2;
					txt.Name = text2;
					if (flag)
					{
						txt.EditValue = ObjectId;
						txt.EditValueChanged += (object s, EventArgs ev) =>
						{
							try
							{
								ObjectId = Function.ParseInt(txt.EditValue);
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", ex2);
								WinCommons.ErrorLineNumber(ex2);
							}
						};
					}
					if (c.HasValue && c != ' ')
					{
						txt.Properties.PasswordChar = c.Value;
					}
					break;
				}
				}
				break;
			case "D":
			{
				DateEditEx dateEditEx = new DateEditEx();
				if (flag2)
				{
					dateEditEx.Validating += Editor_Validating;
					dateEditEx.Properties.Appearance.BackColor = _mandoryColor;
				}
				dateEditEx.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
				dateEditEx.Properties.TextEditStyle = ((!(Function.ToString(item["Editable"]) == "Y")) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
				dateEditEx.Properties.DisplayFormat.FormatType = FormatType.DateTime;
				dateEditEx.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				dateEditEx.StyleController = layoutControl1;
				layoutItem.Control = dateEditEx;
				dateEditEx.StyleController = layoutControl1;
				layoutItem.Control = dateEditEx;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize5 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize5;
				dateEditEx.Name = text2;
				dateEditEx.FieldName = text2;
				break;
			}
			case "F":
			case "B":
			{
				CalcEditEx calcEdit2 = new CalcEditEx();
				if (flag2)
				{
					calcEdit2.Validating += Editor_Validating;
					calcEdit2.Properties.Appearance.BackColor = _mandoryColor;
				}
				calcEdit2.Properties.DisplayFormatType = Enums.DisplayFormatType.Amount;
				calcEdit2.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
				calcEdit2.Properties.TextEditStyle = ((!(Function.ToString(item["Editable"]) == "Y")) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
				calcEdit2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEdit2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				calcEdit2.StyleController = layoutControl1;
				layoutItem.Control = calcEdit2;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize4 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize4;
				if (flag)
				{
					calcEdit2.EditValue = ObjectId;
					calcEdit2.EditValueChanged += (object s, EventArgs ev) =>
					{
						try
						{
							ObjectId = Function.ParseInt(calcEdit2.EditValue);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.ErrorLineNumber(ex2);
						}
					};
				}
				calcEdit2.FieldName = text2;
				calcEdit2.Name = text2;
				break;
			}
			case "M":
			{
				MemoEditEx memoExEdit = new MemoEditEx();
				if (flag2)
				{
					memoExEdit.Validating += Editor_Validating;
					memoExEdit.Properties.Appearance.BackColor = _mandoryColor;
				}
				memoExEdit.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
				memoExEdit.Enabled = Function.ToString(item["Editable"]) == "Y";
				memoExEdit.StyleController = layoutControl1;
				layoutItem.Control = memoExEdit;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize3 = (layoutItem.MaxSize = new Size(num, 100));
				layoutItem.MinSize = minSize3;
				if (flag)
				{
					memoExEdit.EditValue = ObjectId;
					memoExEdit.EditValueChanged += (object s, EventArgs ev) =>
					{
						try
						{
							ObjectId = Function.ParseInt(memoExEdit.EditValue);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.ErrorLineNumber(ex2);
						}
					};
				}
				memoExEdit.FieldName = text2;
				memoExEdit.Name = text2;
				if (c.HasValue && c != ' ')
				{
					memoExEdit.Properties.PasswordChar = c.Value;
				}
				break;
			}
			case "N":
			{
				CalcEditEx calcEdit = new CalcEditEx();
				if (flag2)
				{
					calcEdit.Validating += Editor_Validating;
					calcEdit.Properties.Appearance.BackColor = _mandoryColor;
				}
				calcEdit.Properties.DisplayFormatType = Enums.DisplayFormatType.Integer;
				calcEdit.Properties.ReadOnly = Function.ToString(item["IsReadOnly"]) == "Y";
				calcEdit.Properties.TextEditStyle = ((!(Function.ToString(item["Editable"]) == "Y")) ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard);
				calcEdit.StyleController = layoutControl1;
				calcEdit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEdit.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				layoutItem.Control = calcEdit;
				layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
				Size minSize2 = (layoutItem.MaxSize = new Size(num, 20));
				layoutItem.MinSize = minSize2;
				if (flag)
				{
					calcEdit.EditValue = ObjectId;
					calcEdit.EditValueChanged += (object s, EventArgs ev) =>
					{
						try
						{
							ObjectId = Function.ParseInt(calcEdit.EditValue);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", ex2);
							WinCommons.ErrorLineNumber(ex2);
						}
					};
				}
				calcEdit.FieldName = text2;
				calcEdit.Name = text2;
				if (c.HasValue && c != ' ')
				{
					calcEdit.Properties.PasswordChar = c.Value;
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

	private void Editor_Validating(object sender, CancelEventArgs e)
	{
	}

	public async Task LoadLayoutControl(bool newRow = false)
	{
		_ = 1;
		try
		{
			if (!WizardData.Tables.Contains("FWZ1"))
			{
				return;
			}
			Attachments = new List<ucAttachment>();
			DataTable wizardContainer = WizardData.Tables["FWZ1"];
			EnumerableRowCollection<DataRow> enumerableRowCollection = from t in wizardContainer.AsEnumerable()
				where Function.ToString(t["ContainerType"]) == "F"
				select t;
			string text = Function.ToString(WizardData.Tables["OFWZ"].Rows[0]["Stack0"]);
			string text2 = Function.ToString(WizardData.Tables["OFWZ"].Rows[0]["Stack1"]);
			string text3 = Function.ToString(WizardData.Tables["OFWZ"].Rows[0]["Stack2"]);
			SetStackVisibility(text == "Y", text2 == "Y", text3 == "Y", stack3: true);
			DataRow[] array2;
			if (enumerableRowCollection.Any() && WizardData.Tables.Contains("FWZ2"))
			{
				foreach (DataRow dataRow in enumerableRowCollection)
				{
					DataRow[] array = (from t in WizardData.Tables["FWZ2"].AsEnumerable()
						where Function.ParseInt(t["ContainerId"]) == Function.ParseInt(dataRow["ContainerId"])
						select t).ToArray();
					if (array.Length != 0)
					{
						foreach (DataRow item in array.OrderBy((DataRow t) => Function.ParseInt(t["VisIndex"])))
						{
							bool flag = Function.ToString(item["Visible"]) != "N";
							bool flag2 = Function.ToString(item["Mandatory"]) == "Y";
							LayoutControlItem layoutControlItem;
							if (Function.ParseInt(dataRow["StackGroup"]) != 0)
							{
								layoutControlItem = ((Function.ToString(item["IsLeft"]) == "N") ? layoutControl4.AddItem() : layoutControl3.AddItem());
							}
							else
							{
								layoutControlItem = ((Function.ToString(item["IsLeft"]) == "N") ? layoutControl2.AddItem() : layoutControl1.AddItem());
							}
							string value = Function.ToString(item["Caption"]) + (flag2 ? "(*)" : "");
							layoutControlItem.TextToControlDistance = 10;
							layoutControlItem.TextAlignMode = TextAlignModeItem.CustomSize;
							layoutControlItem.TextSize = ((Function.ToString(item["DataType"]) != "M") ? new Size(130, 25) : new Size(130, 100));
							layoutControlItem.AppearanceItemCaption.Font = new Font(layoutControlItem.AppearanceItemCaption.Font.FontFamily, layoutControlItem.AppearanceItemCaption.Font.Size, FontStyle.Regular);
							AddControlToLayoutItem(item, layoutControlItem);
							if (string.IsNullOrWhiteSpace(value))
							{
								layoutControlItem.TextVisible = false;
							}
							else
							{
								layoutControlItem.Text = value;
							}
							layoutControlItem.Visibility = ((!flag) ? LayoutVisibility.Never : LayoutVisibility.Always);
							layoutControlItem.Name = "layout_" + Function.ToString(item["FieldName"]);
						}
					}
					int num = (from t in array.AsEnumerable()
						where Function.ToString(t["IsLeft"]) != "Y" && Function.ToString(t["Visible"]) == "Y"
						select t).ToArray().Length;
					int num2 = (from t in array.AsEnumerable()
						where new string[2] { "Y", "" }.Contains(Function.ToString(t["IsLeft"])) && Function.ToString(t["Visible"]) == "Y"
						select t).ToArray().Length;
					DataTable source = WizardData.Tables["FWZ3"];
					array2 = array;
					foreach (DataRow row in array2)
					{
						if (Function.ToString(row["Visible"]) == "Y" && Function.ParseInt(row["Editor"]) == 5 && Function.ToString(row["RGVertical"]) == "Y")
						{
							int num4 = source.AsEnumerable().Count((DataRow r) => Function.ParseInt(r["LineId"]) == Function.ParseInt(row["LineId"]));
							if (Function.ToString(row["IsLeft"]) == "Y")
							{
								num2 += num4;
							}
							else
							{
								num += num4;
							}
						}
					}
					int num5 = ((num2 > num) ? num2 : num);
					num = (from t in array.AsEnumerable()
						where Function.ToString(t["IsLeft"]) != "Y" && Function.ToString(t["DataType"]) == "M" && Function.ToString(t["Visible"]) == "Y"
						select t).ToArray().Length;
					num2 = (from t in array.AsEnumerable()
						where new string[2] { "Y", "" }.Contains(Function.ToString(t["IsLeft"])) && Function.ToString(t["DataType"]) == "M" && Function.ToString(t["Visible"]) == "Y"
						select t).ToArray().Length;
					((Function.ParseInt(dataRow["StackGroup"]) == 0) ? splitContainerControl1 : splitContainerControl3).SplitterPosition = (num5 + 1) * 27 + ((num2 > num) ? num2 : num) * 80 + 20;
					if (!HasLeft)
					{
						HasLeft = num2 > 0;
					}
					if (!HasRight)
					{
						HasRight = num > 0;
					}
				}
				SpliterPosition = (splitContainerControl3.Visible ? splitContainerControl3.SplitterPosition : splitContainerControl1.SplitterPosition);
			}
			DataRow[] detailContainer = (from t in wizardContainer.AsEnumerable()
				where Function.ToString(t["ContainerType"]) == "T"
				select t).ToArray();
			for (int i = 0; i < detailContainer.Length; i++)
			{
				if (Function.ParseInt(detailContainer[i]["Type"]) == 1)
				{
					int num6 = Function.ParseInt(detailContainer[i]["StackGroup"]);
					int containerId = Function.ParseInt(detailContainer[i]["ContainerId"]);
					string text4 = Function.ToString(detailContainer[i]["ItemId"]);
					GridControlEx grd = new GridControlEx();
					grd.AllowPasteExcel = true;
					string viewCaption = Function.ToString(detailContainer[i]["Caption"]);
					grd.Dock = DockStyle.Fill;
					grd.Size = new Size(966, 385);
					GridViewEx gv = new GridViewEx();
					grd.ViewCollection.AddRange(new BaseView[1] { gv });
					gv.GridControl = grd;
					grd.MainView = gv;
					gv.FormId = FormId;
					gv.ItemId = (string.IsNullOrEmpty(text4) ? ("W" + containerId) : text4);
					gv.ViewCaption = viewCaption;
					gv.Name = gv.ItemId;
					gv.Tag = containerId;
					DataRow[] array3 = (from t in WizardData.Tables["FWZ2"].AsEnumerable()
						where Function.ParseInt(t["ContainerId"]) == containerId
						select t).ToArray();
					gv.BeginUpdate();
					if (Function.ToString(detailContainer[i]["AddCtxMenu"]) == "Y")
					{
						ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
						grd.ContextMenuStrip = contextMenuStrip;
						ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
						toolStripMenuItem.Text = "Add Row";
						toolStripMenuItem.Click += (object s, EventArgs e) =>
						{
							try
							{
								DataTable dataSourceTable = grd.DataSourceTable;
								dataSourceTable.Rows.Add(dataSourceTable.NewRow());
								dataSourceTable.AcceptChanges();
							}
							catch (Exception ex2)
							{
								WinCommons.ErrorLineNumber(ex2);
								Logging.Write("ERROR", ex2);
							}
						};
						contextMenuStrip.Items.Add(toolStripMenuItem);
						ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
						toolStripMenuItem2.Text = "Delete Row";
						toolStripMenuItem2.Click += (object s, EventArgs e) =>
						{
							try
							{
								gv.DeleteSelectedRows();
								grd.DataSourceTable.AcceptChanges();
							}
							catch (Exception ex2)
							{
								WinCommons.ErrorLineNumber(ex2);
								Logging.Write("ERROR", ex2);
							}
						};
						contextMenuStrip.Items.Add(toolStripMenuItem2);
					}
					if (Function.ToString(detailContainer[i]["ShowCheckCol"]) == "Y")
					{
						gv.OptionsSelection.MultiSelect = true;
						gv.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
					}
					array2 = array3;
					foreach (DataRow dataRow3 in array2)
					{
						string text5 = Function.ToString(dataRow3["PrimaryKey"]);
						GridColumnEx gridColumnEx = new GridColumnEx();
						gridColumnEx.FieldName = Function.ToString(dataRow3["FieldName"]);
						gridColumnEx.Caption = Function.ToString(dataRow3["Caption"]);
						gridColumnEx.ShowUnboundExpressionMenu = true;
						gridColumnEx.Visible = Function.ToString(dataRow3["Visible"]) != "N";
						gridColumnEx.Width = 100;
						gridColumnEx.PrimaryKey = text5 == "Y";
						gridColumnEx.Currency = Enums.CurrencyColumn.None;
						gridColumnEx.OptionsColumn.ReadOnly = Function.ToString(dataRow3["IsReadOnly"]) == "Y";
						gridColumnEx.OptionsColumn.AllowEdit = Function.ToString(dataRow3["Editable"]) == "Y";
						GridColumnEx gridColumnEx2 = gridColumnEx;
						DevExpress.Data.SummaryItemType summaryItemType = Function.ToString(dataRow3["SumType"]) switch
						{
							"S" => DevExpress.Data.SummaryItemType.Sum, 
							"C" => DevExpress.Data.SummaryItemType.Count, 
							"M" => DevExpress.Data.SummaryItemType.Min, 
							"X" => DevExpress.Data.SummaryItemType.Max, 
							"A" => DevExpress.Data.SummaryItemType.Average, 
							_ => DevExpress.Data.SummaryItemType.None, 
						};
						if (summaryItemType != DevExpress.Data.SummaryItemType.None)
						{
							gv.OptionsView.ShowFooter = true;
							gridColumnEx2.Summary.Add(new GridColumnSummaryItem
							{
								SummaryType = summaryItemType,
								DisplayFormat = Function.ToString(dataRow3["DisplayFormat"])
							});
						}
						string text6 = Function.ToString(dataRow3["DataType"]);
						Enums.FormTemplateEditorType formTemplateEditorType = (Enums.FormTemplateEditorType)Function.ParseInt(dataRow3["Editor"]);
						Enums.TypeCFL typeCFL = (Enums.TypeCFL)Function.ParseInt(dataRow3["CFLType"]);
						string text7 = Function.ToString(dataRow3["SubType"]);
						char? c = Function.ParseChar(dataRow3["PasswordChar"]);
						switch (text6)
						{
						case "A":
							switch (formTemplateEditorType)
							{
							case Enums.FormTemplateEditorType.ChooseFromList:
							{
								RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL(Function.ToString(dataRow3["ValueMember"]), Function.ToString(dataRow3["DisplayMember"]));
								repositoryItemButtonEditCFL.TypeCFL = typeCFL;
								if (typeCFL == Enums.TypeCFL.UserDefinedData)
								{
									string sql = Function.ToString(dataRow3["CFLQuery"]);
									DataTable executeSqlResult = _httpClient.Commons.GetExecuteSqlResult(sql, new List<QueryParamater>());
									DataSet dsSource = new DataSet();
									dsSource.Tables.Add(executeSqlResult.Copy());
									repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL f) =>
									{
										try
										{
											if (f != null)
											{
												f.DataSource = dsSource.Copy();
											}
										}
										catch (Exception ex2)
										{
											Logging.Write("ERROR", ex2);
											WinCommons.ErrorLineNumber(ex2);
										}
									}));
									if (executeSqlResult.IsNotNull())
									{
										repositoryItemButtonEditCFL.DataSource = executeSqlResult;
									}
								}
								ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(dataRow3["DrillDownType"]);
								if (apzObjectTypes != ApzObjectTypes.oNone)
								{
									repositoryItemButtonEditCFL.TypeDrillDown = apzObjectTypes;
								}
								if (c.HasValue && c != ' ')
								{
									repositoryItemButtonEditCFL.PasswordChar = c.Value;
								}
								gridColumnEx2.ColumnEdit = repositoryItemButtonEditCFL;
								break;
							}
							case Enums.FormTemplateEditorType.ValidValue:
							{
								RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
								{
									NullText = ""
								};
								DataTable validSource = GetValidSource(dataRow3, out var valueMember, out var displayMember);
								repositoryItemLookUpEdit.SetDataSource(validSource, displayMember, valueMember, emptyRow: true);
								gridColumnEx2.ColumnEdit = repositoryItemLookUpEdit;
								if (c.HasValue && c != ' ')
								{
									repositoryItemLookUpEdit.PasswordChar = c.Value;
								}
								break;
							}
							case Enums.FormTemplateEditorType.CheckEdit:
							{
								RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
								repositoryItemCheckEdit.ValueChecked = "Y";
								repositoryItemCheckEdit.ValueUnchecked = "N";
								repositoryItemCheckEdit.ValueGrayed = "N";
								repositoryItemCheckEdit.ReadOnly = Function.ToString(dataRow3["IsReadOnly"]) == "Y";
								gridColumnEx2.ColumnEdit = repositoryItemCheckEdit;
								break;
							}
							}
							break;
						case "B":
							switch (text7)
							{
							case "S":
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Amount;
								if (c.HasValue && c != ' ')
								{
									RepositoryItemCalcEditEx repositoryItemCalcEditEx2 = (RepositoryItemCalcEditEx)(gridColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
									{
										NullText = ""
									});
									repositoryItemCalcEditEx2.PasswordChar = c.Value;
								}
								break;
							case "P":
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Price;
								if (c.HasValue && c != ' ')
								{
									RepositoryItemCalcEditEx repositoryItemCalcEditEx4 = (RepositoryItemCalcEditEx)(gridColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
									{
										NullText = ""
									});
									repositoryItemCalcEditEx4.PasswordChar = c.Value;
								}
								break;
							case "Q":
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Qty;
								if (c.HasValue && c != ' ')
								{
									RepositoryItemCalcEditEx repositoryItemCalcEditEx3 = (RepositoryItemCalcEditEx)(gridColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
									{
										NullText = ""
									});
									repositoryItemCalcEditEx3.PasswordChar = c.Value;
								}
								break;
							case "%":
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Percent;
								if (c.HasValue && c != ' ')
								{
									RepositoryItemCalcEditEx repositoryItemCalcEditEx = (RepositoryItemCalcEditEx)(gridColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
									{
										NullText = ""
									});
									repositoryItemCalcEditEx.PasswordChar = c.Value;
								}
								break;
							}
							break;
						case "M":
						{
							gridColumnEx2.UnboundType = DevExpress.Data.UnboundColumnType.String;
							RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
							EditorButton editorButton = new EditorButton();
							editorButton.Kind = ButtonPredefines.Ellipsis;
							editorButton.ToolTip = "Browse";
							editorButton.ImageLocation = ImageLocation.MiddleCenter;
							EditorButton editorButton2 = new EditorButton();
							editorButton2.Kind = ButtonPredefines.Search;
							editorButton2.ToolTip = "Display";
							editorButton2.ImageLocation = ImageLocation.MiddleCenter;
							repositoryItemButtonEdit.Buttons.Clear();
							repositoryItemButtonEdit.Buttons.Add(editorButton);
							repositoryItemButtonEdit.Buttons.Add(editorButton2);
							repositoryItemButtonEdit.ButtonPressed += rp_btnEdit_ButtonPressed;
							if (c.HasValue && c != ' ')
							{
								repositoryItemButtonEdit.PasswordChar = c.Value;
							}
							gridColumnEx2.ColumnEdit = repositoryItemButtonEdit;
							break;
						}
						case "D":
							if (text7 == "T")
							{
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Time;
							}
							else
							{
								gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.DateTime;
							}
							break;
						}
						gv.Columns.Add(gridColumnEx2);
					}
					gv.CellValueChangedAsync += async (DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs ev) =>
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
					switch (num6)
					{
					case 1:
						StackGroup1.TabPages.Add(xtraTabPageEx);
						break;
					case 2:
						StackGroup2.TabPages.Add(xtraTabPageEx);
						break;
					}
					xtraTabPageEx.ResumeLayout(performLayout: false);
					DataTable dataTable = await GetData(detailContainer[i]);
					if (newRow)
					{
						dataTable.Rows.Add(dataTable.NewRow());
					}
					grd.DataSource = dataTable;
					grd.RefreshDataSource();
					continue;
				}
				int num7 = Function.ParseInt(detailContainer[i]["StackGroup"]);
				int containerId2 = Function.ParseInt(detailContainer[i]["ContainerId"]);
				string text8 = Function.ToString(detailContainer[i]["ItemId"]);
				TreeEx tree = new TreeEx();
				tree.KeyFieldName = string.Empty;
				tree.AllowPasteExcel = true;
				string caption = Function.ToString(detailContainer[i]["Caption"]);
				tree.Dock = DockStyle.Fill;
				tree.Size = new Size(966, 385);
				tree.FormId = FormId;
				tree.ItemId = (string.IsNullOrEmpty(text8) ? ("W" + containerId2) : text8);
				tree.Caption = caption;
				tree.Name = text8;
				tree.Tag = containerId2;
				DataRow[] array4 = (from t in WizardData.Tables["FWZ2"].AsEnumerable()
					where Function.ParseInt(t["ContainerId"]) == containerId2
					select t).ToArray();
				tree.BeginUpdate();
				if (Function.ToString(detailContainer[i]["AddCtxMenu"]) == "Y")
				{
					ContextMenuStrip contextMenuStrip2 = new ContextMenuStrip();
					tree.ContextMenuStrip = contextMenuStrip2;
					ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
					toolStripMenuItem3.Text = "Add Row";
					toolStripMenuItem3.Click += (object s, EventArgs e) =>
					{
						try
						{
							DataTable dataSourceTable = tree.DataSourceTable;
							dataSourceTable.Rows.Add(dataSourceTable.NewRow());
							dataSourceTable.AcceptChanges();
						}
						catch (Exception ex2)
						{
							WinCommons.ErrorLineNumber(ex2);
							Logging.Write("ERROR", ex2);
						}
					};
					contextMenuStrip2.Items.Add(toolStripMenuItem3);
					ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
					toolStripMenuItem4.Text = "Delete Row";
					toolStripMenuItem4.Click += (object s, EventArgs e) =>
					{
						try
						{
							tree.DeleteSelectedNodes();
							tree.DataSourceTable.AcceptChanges();
						}
						catch (Exception ex2)
						{
							WinCommons.ErrorLineNumber(ex2);
							Logging.Write("ERROR", ex2);
						}
					};
					contextMenuStrip2.Items.Add(toolStripMenuItem4);
				}
				if (Function.ToString(detailContainer[i]["ShowCheckCol"]) == "Y")
				{
					tree.OptionsSelection.MultiSelect = true;
					tree.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
				}
				array2 = array4;
				foreach (DataRow dataRow4 in array2)
				{
					Function.ToString(dataRow4["PrimaryKey"]);
					TreeListColumnEx treeListColumnEx = new TreeListColumnEx();
					treeListColumnEx.FieldName = Function.ToString(dataRow4["FieldName"]);
					treeListColumnEx.Caption = Function.ToString(dataRow4["Caption"]);
					treeListColumnEx.ShowUnboundExpressionMenu = true;
					treeListColumnEx.Visible = Function.ToString(dataRow4["Visible"]) != "N";
					treeListColumnEx.Width = 100;
					treeListColumnEx.Currency = Enums.CurrencyColumn.None;
					treeListColumnEx.OptionsColumn.ReadOnly = Function.ToString(dataRow4["IsReadOnly"]) == "Y";
					treeListColumnEx.OptionsColumn.AllowEdit = Function.ToString(dataRow4["Editable"]) == "Y";
					TreeListColumnEx treeListColumnEx2 = treeListColumnEx;
					DevExpress.XtraTreeList.SummaryItemType summaryItemType2 = Function.ToString(dataRow4["SumType"]) switch
					{
						"S" => DevExpress.XtraTreeList.SummaryItemType.Sum, 
						"C" => DevExpress.XtraTreeList.SummaryItemType.Count, 
						"M" => DevExpress.XtraTreeList.SummaryItemType.Min, 
						"X" => DevExpress.XtraTreeList.SummaryItemType.Max, 
						"A" => DevExpress.XtraTreeList.SummaryItemType.Average, 
						_ => DevExpress.XtraTreeList.SummaryItemType.None, 
					};
					if (summaryItemType2 != DevExpress.XtraTreeList.SummaryItemType.None)
					{
						tree.OptionsView.ShowSummaryFooter = true;
						treeListColumnEx2.AllNodesSummary = true;
						treeListColumnEx2.SummaryFooterStrFormat = "Total: {0:c0}";
						treeListColumnEx2.SummaryFooter = summaryItemType2;
					}
					string text9 = Function.ToString(dataRow4["DataType"]);
					Enums.FormTemplateEditorType formTemplateEditorType2 = (Enums.FormTemplateEditorType)Function.ParseInt(dataRow4["Editor"]);
					Enums.TypeCFL typeCFL2 = (Enums.TypeCFL)Function.ParseInt(dataRow4["CFLType"]);
					string text10 = Function.ToString(dataRow4["SubType"]);
					char? c2 = Function.ParseChar(dataRow4["PasswordChar"]);
					switch (text9)
					{
					case "A":
						switch (formTemplateEditorType2)
						{
						case Enums.FormTemplateEditorType.ChooseFromList:
						{
							RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = new RepositoryItemButtonEditCFL(Function.ToString(dataRow4["ValueMember"]), Function.ToString(dataRow4["DisplayMember"]));
							repositoryItemButtonEditCFL2.TypeCFL = typeCFL2;
							if (typeCFL2 == Enums.TypeCFL.UserDefinedData)
							{
								string sql2 = Function.ToString(dataRow4["CFLQuery"]);
								DataTable executeSqlResult2 = _httpClient.Commons.GetExecuteSqlResult(sql2, new List<QueryParamater>());
								DataSet dsSource2 = new DataSet();
								dsSource2.Tables.Add(executeSqlResult2.Copy());
								repositoryItemButtonEditCFL2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL f) =>
								{
									try
									{
										if (f != null)
										{
											f.DataSource = dsSource2.Copy();
										}
									}
									catch (Exception ex2)
									{
										Logging.Write("ERROR", ex2);
										WinCommons.ErrorLineNumber(ex2);
									}
								}));
								if (executeSqlResult2.IsNotNull())
								{
									repositoryItemButtonEditCFL2.DataSource = executeSqlResult2;
								}
							}
							ApzObjectTypes apzObjectTypes2 = (ApzObjectTypes)Function.ParseInt(dataRow4["DrillDownType"]);
							if (apzObjectTypes2 != ApzObjectTypes.oNone)
							{
								repositoryItemButtonEditCFL2.TypeDrillDown = apzObjectTypes2;
							}
							if (c2.HasValue && c2 != ' ')
							{
								repositoryItemButtonEditCFL2.PasswordChar = c2.Value;
							}
							treeListColumnEx2.ColumnEdit = repositoryItemButtonEditCFL2;
							break;
						}
						case Enums.FormTemplateEditorType.ValidValue:
						{
							RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
							{
								NullText = ""
							};
							DataTable validSource2 = GetValidSource(dataRow4, out var valueMember2, out var displayMember2);
							repositoryItemLookUpEdit2.SetDataSource(validSource2, displayMember2, valueMember2, emptyRow: true);
							treeListColumnEx2.ColumnEdit = repositoryItemLookUpEdit2;
							if (c2.HasValue && c2 != ' ')
							{
								repositoryItemLookUpEdit2.PasswordChar = c2.Value;
							}
							break;
						}
						case Enums.FormTemplateEditorType.CheckEdit:
						{
							RepositoryItemCheckEdit repositoryItemCheckEdit2 = new RepositoryItemCheckEdit();
							repositoryItemCheckEdit2.ValueChecked = "Y";
							repositoryItemCheckEdit2.ValueUnchecked = "N";
							repositoryItemCheckEdit2.ValueGrayed = "N";
							repositoryItemCheckEdit2.ReadOnly = Function.ToString(dataRow4["IsReadOnly"]) == "Y";
							treeListColumnEx2.ColumnEdit = repositoryItemCheckEdit2;
							break;
						}
						}
						break;
					case "B":
						switch (text10)
						{
						case "S":
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Amount;
							if (c2.HasValue && c2 != ' ')
							{
								RepositoryItemCalcEditEx repositoryItemCalcEditEx6 = (RepositoryItemCalcEditEx)(treeListColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
								{
									NullText = ""
								});
								repositoryItemCalcEditEx6.PasswordChar = c2.Value;
							}
							break;
						case "P":
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Price;
							if (c2.HasValue && c2 != ' ')
							{
								RepositoryItemCalcEditEx repositoryItemCalcEditEx8 = (RepositoryItemCalcEditEx)(treeListColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
								{
									NullText = ""
								});
								repositoryItemCalcEditEx8.PasswordChar = c2.Value;
							}
							break;
						case "Q":
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Qty;
							if (c2.HasValue && c2 != ' ')
							{
								RepositoryItemCalcEditEx repositoryItemCalcEditEx7 = (RepositoryItemCalcEditEx)(treeListColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
								{
									NullText = ""
								});
								repositoryItemCalcEditEx7.PasswordChar = c2.Value;
							}
							break;
						case "%":
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Percent;
							if (c2.HasValue && c2 != ' ')
							{
								RepositoryItemCalcEditEx repositoryItemCalcEditEx5 = (RepositoryItemCalcEditEx)(treeListColumnEx2.ColumnEdit = new RepositoryItemCalcEditEx
								{
									NullText = ""
								});
								repositoryItemCalcEditEx5.PasswordChar = c2.Value;
							}
							break;
						}
						break;
					case "M":
					{
						treeListColumnEx2.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
						RepositoryItemButtonEdit repositoryItemButtonEdit2 = new RepositoryItemButtonEdit();
						EditorButton editorButton3 = new EditorButton();
						editorButton3.Kind = ButtonPredefines.Ellipsis;
						editorButton3.ToolTip = "Browse";
						editorButton3.ImageLocation = ImageLocation.MiddleCenter;
						EditorButton editorButton4 = new EditorButton();
						editorButton4.Kind = ButtonPredefines.Search;
						editorButton4.ToolTip = "Display";
						editorButton4.ImageLocation = ImageLocation.MiddleCenter;
						repositoryItemButtonEdit2.Buttons.Clear();
						repositoryItemButtonEdit2.Buttons.Add(editorButton3);
						repositoryItemButtonEdit2.Buttons.Add(editorButton4);
						repositoryItemButtonEdit2.ButtonPressed += rp_btnEdit_ButtonPressed;
						if (c2.HasValue && c2 != ' ')
						{
							repositoryItemButtonEdit2.PasswordChar = c2.Value;
						}
						treeListColumnEx2.ColumnEdit = repositoryItemButtonEdit2;
						break;
					}
					case "D":
						if (text10 == "T")
						{
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Time;
						}
						else
						{
							treeListColumnEx2.DisplayFormatType = Enums.DisplayFormatType.DateTime;
						}
						break;
					}
					tree.Columns.Add(treeListColumnEx2);
					if (Function.ToString(dataRow4["IsParentFieldName"]) == "Y")
					{
						tree.ParentFieldName = Function.ToString(dataRow4["FieldName"]);
					}
					if (Function.ToString(dataRow4["PrimaryKey"]) == "Y")
					{
						tree.KeyFieldName = Function.ToString(dataRow4["FieldName"]);
					}
				}
				tree.CellValueChanged += Tree_CellValueChanged;
				tree.EndUpdate();
				XtraTabPageEx xtraTabPageEx2 = new XtraTabPageEx();
				xtraTabPageEx2.Size = new Size(966, 385);
				xtraTabPageEx2.SuspendLayout();
				xtraTabPageEx2.Controls.Add(tree);
				xtraTabPageEx2.SourceType = Enums.DataSourceType.TittleValue;
				xtraTabPageEx2.Text = caption;
				switch (num7)
				{
				case 1:
					StackGroup1.TabPages.Add(xtraTabPageEx2);
					break;
				case 2:
					StackGroup2.TabPages.Add(xtraTabPageEx2);
					break;
				}
				xtraTabPageEx2.ResumeLayout(performLayout: false);
				DataTable dataTable2 = await GetData(detailContainer[i]);
				if (newRow)
				{
					dataTable2.Rows.Add(dataTable2.NewRow());
				}
				tree.DataSource = dataTable2;
				tree.RefreshDataSource();
			}
			DataRow[] array5 = (from t in wizardContainer.AsEnumerable()
				where Function.ToString(t["ContainerType"]) == "A"
				select t).ToArray();
			if (array5.Length == 0)
			{
				return;
			}
			array2 = array5;
			foreach (DataRow obj in array2)
			{
				int num8 = Function.ParseInt(obj["StackGroup"]);
				new GridControlEx().AllowPasteExcel = true;
				string text11 = Function.ToString(obj["Caption"]);
				XtraTabPageEx xtraTabPageEx3 = new XtraTabPageEx();
				xtraTabPageEx3.Size = new Size(966, 385);
				xtraTabPageEx3.SuspendLayout();
				ucAttachment ucAttachment2 = new ucAttachment();
				Attachments.Add(ucAttachment2);
				ucAttachment2.Dock = DockStyle.Fill;
				xtraTabPageEx3.Controls.Add(ucAttachment2);
				xtraTabPageEx3.SourceType = Enums.DataSourceType.TittleValue;
				xtraTabPageEx3.Text = text11;
				switch (num8)
				{
				case 1:
					StackGroup1.TabPages.Add(xtraTabPageEx3);
					break;
				case 2:
					StackGroup2.TabPages.Add(xtraTabPageEx3);
					break;
				}
				xtraTabPageEx3.ResumeLayout(performLayout: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Tree_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			TreeListColumnEx col = e.Column as TreeListColumnEx;
			TreeEx treeEx = sender as TreeEx;
			if (col != null && treeEx.KeyFieldName == col.FieldName && treeEx.DataSource is DataTable dataTable && dataTable.Columns.Count > 0 && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t[col.FieldName]))) == null)
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

	public static string PathOpenFile()
	{
		string result = string.Empty;
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				result = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	public static async void rp_btnEdit_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		_ = 2;
		try
		{
			ButtonEdit bt = (ButtonEdit)sender;
			if (e.Button.Index == 0)
			{
				WinCommons._OpenProcessing();
				string pathFile = PathOpenFile();
				if (!string.IsNullOrEmpty(pathFile))
				{
					if (Regex.IsMatch(pathFile, "^(?:[a-zA-Z]\\:(\\\\|\\/)|file\\:\\/\\/|\\\\\\\\|\\.(\\/|\\\\))([^\\\\\\/\\:\\*\\?\\<\\>\\\"\\|]+(\\\\|\\/){0,1})+$"))
					{
						string value = await WinFuntions.CopyFileNetwork(pathFile, string.Concat(str1: FileIO.getFileName(pathFile), str0: await LoadConfigFileSharingUpload()));
						if (!string.IsNullOrEmpty(value))
						{
							bt.Text = value;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.CanNotSaveFile, "");
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.UrlNotValid, "");
					}
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				if (e.Button.Index != 1)
				{
					return;
				}
				WinCommons._OpenProcessing();
				string text = bt.Text;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = await WinFuntions.OpenFileNetwork(text);
					if (!string.IsNullOrEmpty(text2))
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
	}

	public static async Task<string> LoadConfigFileSharingUpload()
	{
		string attachPath = "";
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GeneralSetting");
			DataTable dataTable = await hp.GetAsync("GetFileSharingUploadPath");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				attachPath = Function.ToString(dataTable.Rows[0]["AttachPath"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return attachPath;
	}

	private DataTable GetValidSource(DataRow row, out string valueMember, out string displayMember)
	{
		try
		{
			string text = Function.ToString(row["ValidType"]);
			if (text == "T")
			{
				string sql = Function.ToString(row["ValidQuery"]);
				DataTable executeSqlResult = _httpClient.Commons.GetExecuteSqlResult(sql, new List<QueryParamater>());
				string text2 = Function.ToString(row["ValueMember"]);
				string text3 = Function.ToString(row["DisplayMember"]);
				// Original binary checks text2 (ValueMember) for both members; kept as-is.
				valueMember = string.IsNullOrEmpty(text2)
					? ((executeSqlResult.Columns.Count > 0) ? executeSqlResult.Columns[0].ColumnName : "Code")
					: text2;
				displayMember = string.IsNullOrEmpty(text2)
					? ((executeSqlResult.Columns.Count > 0) ? executeSqlResult.Columns[0].ColumnName : "Name")
					: text3;
				return executeSqlResult;
			}
			if (text == "F")
			{
				DataRow[] array = (from t in WizardData.Tables["FWZ3"].AsEnumerable()
					where Function.ParseInt(t["LineId"]) == Function.ParseInt(row["LineId"])
					select t).ToArray();
				if (array.Length != 0)
				{
					string text4 = Function.ToString(row["ValueMember"]);
					string text5 = Function.ToString(row["DisplayMember"]);
					valueMember = (string.IsNullOrEmpty(text4) ? "Code" : text4);
					displayMember = (string.IsNullOrEmpty(text4) ? "Name" : text5);
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add(valueMember, typeof(string));
					dataTable.Columns.Add(displayMember, typeof(string));
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2[valueMember] = Function.ToString(dataRow["Code"]);
						dataRow2[displayMember] = Function.ToString(dataRow["Name"]);
						dataTable.Rows.Add(dataRow2);
					}
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		valueMember = "Code";
		displayMember = "Name";
		return _validValueStruct.Clone();
	}

	public void SetStackVisibility(bool stack0, bool stack1, bool stack2, bool stack3)
	{
		try
		{
			if (stack0)
			{
				if (stack1 | stack2)
				{
					splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
					if (!stack1)
					{
						splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel2;
					}
					else if (!stack2)
					{
						splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel1;
					}
					else if (stack1 & stack2)
					{
						splitContainerControl2.PanelVisibility = SplitPanelVisibility.Both;
					}
				}
				else
				{
					splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
				}
			}
			else if (stack1 | stack2)
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
				if (!stack1)
				{
					splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel2;
				}
				else if (!stack2)
				{
					splitContainerControl2.PanelVisibility = SplitPanelVisibility.Panel1;
				}
				else if (stack1 & stack2)
				{
					splitContainerControl2.PanelVisibility = SplitPanelVisibility.Both;
				}
			}
			else
			{
				splitContainerControl1.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ClearControl()
	{
		try
		{
			layoutControl1.Clear(clearHiddenItems: true, disposeControls: true);
			layoutControl2.Clear(clearHiddenItems: true, disposeControls: true);
			layoutControl3.Clear(clearHiddenItems: true, disposeControls: true);
			layoutControl4.Clear(clearHiddenItems: true, disposeControls: true);
			StackGroup1.TabPages.Clear();
			StackGroup2.TabPages.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void HandleQuery(Dictionary<string, string> query)
	{
		try
		{
			if (query == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> item in query)
			{
				bool flag = false;
				foreach (object item2 in layoutControl1.Items)
				{
					if (!(item2 is LayoutControlItem))
					{
						continue;
					}
					Control control = (item2 as LayoutControlItem).Control;
					if (control != null && control.Name == item.Key)
					{
						if (control is BaseEdit)
						{
							((BaseEdit)control).EditValue = item.Value;
						}
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				foreach (object item3 in layoutControl2.Items)
				{
					if (!(item3 is LayoutControlItem))
					{
						continue;
					}
					Control control2 = (item3 as LayoutControlItem).Control;
					if (control2 != null && control2.Name == item.Key)
					{
						if (control2 is BaseEdit)
						{
							((BaseEdit)control2).EditValue = item.Value;
						}
						flag = true;
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

	public async Task<DataTable> GetData(DataRow header)
	{
		try
		{
			string qtxt = Function.ToString(header["DataString"]);
			if (string.IsNullOrEmpty(qtxt))
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from t in WizardData.Tables["FWZ2"].AsEnumerable()
					where Function.ParseInt(t["ContainerId"]) == Function.ParseInt(header["ContainerId"])
					select t;
				DataTable dataTable = new DataTable();
				foreach (DataRow item in enumerableRowCollection)
				{
					string text = Function.ToString(item["FieldName"]);
					string text2 = Function.ToString(item["DataType"]);
					string text3 = Function.ToString(item["SubType"]);
					string text4 = Function.ToString(item["DefaultValue"]);
					Type typeFromHandle;
					object obj;
					switch (text2)
					{
					default:
						typeFromHandle = typeof(string);
						obj = text4;
						break;
					case "N":
						typeFromHandle = typeof(long);
						obj = Function.ParseLong(item["DefaultValue"]);
						break;
					case "D":
						if (text3 == "T")
						{
							typeFromHandle = typeof(int);
							obj = Function.ParseInt(item["DefaultValue"]);
						}
						else
						{
							typeFromHandle = typeof(DateTime);
							obj = Function.ParseDateTime(item["DefaultValue"]);
						}
						break;
					case "B":
						typeFromHandle = typeof(double);
						obj = Function.ParseDouble(item["DefaultValue"]);
						break;
					}
					DataColumn dataColumn = new DataColumn();
					dataColumn.ColumnName = text;
					dataColumn.DataType = typeFromHandle;
					dataColumn.Caption = Function.ToString(item["Caption"]) ?? text;
					if (!string.IsNullOrEmpty(text4))
					{
						if (typeFromHandle == typeof(DateTime) && obj != null)
						{
							dataColumn.DefaultValue = obj;
						}
						else
						{
							dataColumn.DefaultValue = obj;
						}
					}
					dataTable.Columns.Add(dataColumn);
				}
				return dataTable;
			}
			DataTable dataTable2 = await _httpClient.Cfl.GetQueryParams(qtxt);
			List<QueryParamater> list = new List<QueryParamater>();
			foreach (DataRow row in dataTable2.Rows)
			{
				QueryParamater queryParamater = new QueryParamater
				{
					ParaName = Function.ToString(row["Para"])
				};
				queryParamater.Value = ObjectId;
				list.Add(queryParamater);
			}
			return _httpClient.Commons.GetExecuteSqlResult(qtxt, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void Gv_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
	}

	private async Task Gv_CellValueChangedAsync(GridViewEx gv, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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

	private void UcTemplate_Load(object sender, EventArgs e)
	{
		try
		{
			_validValueStruct.Columns.Add("Code", typeof(string));
			_validValueStruct.Columns.Add("Name", typeof(string));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void HandleSpliterPosition()
	{
		try
		{
			splitContainerControl2.SplitterPosition += 1;
			foreach (XtraTabPage tabPage in StackGroup1.TabPages)
			{
				Size size = tabPage.Size;
				if (tabPage.Controls[0] is GridControlEx gridControlEx)
				{
					gridControlEx.Size = size;
				}
				else if (tabPage.Controls[0] is ucAttachment)
				{
					((ucAttachment)tabPage.Controls[0]).Size = size;
				}
			}
			foreach (XtraTabPage tabPage2 in StackGroup2.TabPages)
			{
				Size size2 = tabPage2.Size;
				if (tabPage2.Controls[0] is GridControlEx gridControlEx2)
				{
					gridControlEx2.Size = size2;
				}
				else if (tabPage2.Controls[0] is ucAttachment)
				{
					((ucAttachment)tabPage2.Controls[0]).Size = size2;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void GetDataHeader()
	{
		throw new NotImplementedException();
	}

	public void SetVisibleControl(string fieldName, bool isVisible)
	{
		try
		{
			try
			{
				bool flag = false;
				foreach (object item in layoutControl1.Items)
				{
					if (item is LayoutItem)
					{
						LayoutItem layoutItem = item as LayoutItem;
						if (layoutItem.Name == "layout_" + fieldName)
						{
							layoutItem.Visibility = ((!isVisible) ? LayoutVisibility.Never : LayoutVisibility.Always);
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					return;
				}
				foreach (object item2 in layoutControl2.Items)
				{
					if (item2 is LayoutItem)
					{
						LayoutItem layoutItem2 = item2 as LayoutItem;
						if (layoutItem2.Name == "layout_" + fieldName)
						{
							layoutItem2.Visibility = ((!isVisible) ? LayoutVisibility.Never : LayoutVisibility.Always);
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
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
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
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.StackGroup1 = new DevExpress.XtraTab.XtraTabControl();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.StackGroup2 = new DevExpress.XtraTab.XtraTabControl();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.StackGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.StackGroup2).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl2);
		this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(958, 651);
		this.splitContainerControl1.SplitterPosition = 180;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.layoutControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl2.Location = new System.Drawing.Point(510, 8);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.Root = this.layoutControlGroup2;
		this.layoutControl2.Size = new System.Drawing.Size(433, 165);
		this.layoutControl2.TabIndex = 3;
		this.layoutControl2.Text = "layoutControl2";
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Name = "layoutControlGroup1";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(433, 165);
		this.layoutControlGroup2.TextVisible = false;
		this.layoutControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl1.Location = new System.Drawing.Point(14, 8);
		this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(433, 165);
		this.layoutControl1.TabIndex = 2;
		this.layoutControl1.Text = "layoutControl1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(433, 165);
		this.layoutControlGroup1.TextVisible = false;
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.StackGroup1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.splitContainerControl3);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(958, 461);
		this.splitContainerControl2.SplitterPosition = 181;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.StackGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.StackGroup1.Location = new System.Drawing.Point(0, 0);
		this.StackGroup1.Name = "StackGroup1";
		this.StackGroup1.Size = new System.Drawing.Size(958, 181);
		this.StackGroup1.TabIndex = 2;
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Horizontal = false;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.layoutControl4);
		this.splitContainerControl3.Panel1.Controls.Add(this.layoutControl3);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.Controls.Add(this.StackGroup2);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(958, 270);
		this.splitContainerControl3.SplitterPosition = 52;
		this.splitContainerControl3.TabIndex = 3;
		this.layoutControl4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl4.Location = new System.Drawing.Point(510, 12);
		this.layoutControl4.Name = "layoutControl4";
		this.layoutControl4.Root = this.layoutControlGroup3;
		this.layoutControl4.Size = new System.Drawing.Size(433, 40);
		this.layoutControl4.TabIndex = 1;
		this.layoutControl4.Text = "layoutControl4";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(433, 40);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.layoutControl3.Location = new System.Drawing.Point(14, 12);
		this.layoutControl3.Name = "layoutControl3";
		this.layoutControl3.Root = this.Root;
		this.layoutControl3.Size = new System.Drawing.Size(433, 40);
		this.layoutControl3.TabIndex = 0;
		this.layoutControl3.Text = "layoutControl3";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(433, 40);
		this.Root.TextVisible = false;
		this.StackGroup2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.StackGroup2.Location = new System.Drawing.Point(0, 0);
		this.StackGroup2.Name = "StackGroup2";
		this.StackGroup2.Size = new System.Drawing.Size(958, 208);
		this.StackGroup2.TabIndex = 2;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.splitContainerControl1);
		base.Name = "UcTemplate";
		base.Size = new System.Drawing.Size(958, 651);
		base.Load += new System.EventHandler(this.UcTemplate_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.StackGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.StackGroup2).EndInit();
		base.ResumeLayout(false);
	}
}
