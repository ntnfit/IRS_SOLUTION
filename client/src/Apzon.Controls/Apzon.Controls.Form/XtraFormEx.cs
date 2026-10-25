using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Controls.Form;

public class XtraFormEx : XtraForm
{
	private readonly string[] AdminFormSetupSystem = new string[4] { "frm_Authorization", "frm_Users", "frm_LicenseManager", "frm_SetupAdmin" };

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public static bool ExitApplication;

	public List<IControlEx> LstControlSource = new List<IControlEx>();

	public List<IControlEx> LstControlTittleSource = new List<IControlEx>();

	public List<SimpleButtonEx> LstButtonControl = new List<SimpleButtonEx>();

	protected bool _onlyChangeRemark;

	public Enums.FormAuthorities FormAuth;

	public XtraScrollableControl ScrollEx = new XtraScrollableControl();

	protected List<Token> SyntaxBeforeOpen;

	protected List<Token> SyntaxAfterOpen;

	protected List<Token> SyntaxControlValueChanged;

	[Browsable(false)]
	public List<Token> SyntaxOnAddMode;

	[Browsable(false)]
	public List<Token> SyntaxBeforeClick;

	[Browsable(false)]
	public List<Token> SyntaxAfterClick;

	protected Dictionary<Enums.EventId, List<Token>> SyntaxEventControl = new Dictionary<Enums.EventId, List<Token>>();

	public List<ViewEditModel> ListGrid;

	protected bool CancelOpen;

	public bool IsFormClosing;

	public bool IsCanClose = true;

	public bool IsSetSuperTip;

	private bool _isScrollable;

	public Size ScrollMinSize;

	public static bool MouseOnComponent;

	public static bool ViewSystemInformation;

	private GridColumn prevColumn;

	public static EnableDirectionalControl EnableDirectionalControl;

	public static ShowProgressBar ShowProgressBar;

	public static HideProgressBar HideProgressBar;

	public static SetValueProgressBar SetValueProgressBar;

	public bool IsLoadForm;

	private object _dataSource;

	private DataTable _dataSourceTitle;

	protected Enums.FormViewModeType _formMode = Enums.FormViewModeType.View;

	private static Icon _icon;

	private readonly Dictionary<string, string> _formNameCheckAuth = new Dictionary<string, string> { { "frm_UserDefinedProcedureCreate", "frm_UserDefinedProcedure" } };

	private bool _setFontContextMenu;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	protected DataSet DsContextAction;

	public static Enums.DirectionalControl[] GridToolbarButtons = new Enums.DirectionalControl[8]
	{
		Enums.DirectionalControl.ExportExcel,
		Enums.DirectionalControl.ExportPdf,
		Enums.DirectionalControl.ExportWord,
		Enums.DirectionalControl.ExportClipboard,
		Enums.DirectionalControl.AutoFilterRow,
		Enums.DirectionalControl.ShowFooter,
		Enums.DirectionalControl.BestFitColumn,
		Enums.DirectionalControl.Expand
	};

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Dictionary<string, DataSet> ListContextAction = new Dictionary<string, DataSet>();

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Dictionary<string, List<Token>> ListSyntaxContextAction = new Dictionary<string, List<Token>>();

	private TreeListColumn _prevTreeCol;

	private readonly Dictionary<string, bool> _lstMethod = new Dictionary<string, bool>();

	public List<Control> UserDefinedControl { get; set; }

	private bool _isMovingControl { get; set; }

	public bool DisableContextAction { get; set; }

	public bool DisableLoadScript { get; set; }

	public virtual bool DisableAuthorization { get; set; }

	public string FormId { get; set; }

	public ApzObjectTypes FormObjectType { get; set; }

	public IHttpClient HttpClient { get; set; }

	public bool IsScrollable
	{
		get
		{
			return _isScrollable;
		}
		set
		{
			_isScrollable = value;
			if (value)
			{
				SetMinScrollSize(Size);
				List<Control> list = Controls.Cast<Control>().ToList();
				ScrollEx.Dock = DockStyle.Fill;
				Controls.Clear();
				Controls.Add(ScrollEx);
				ScrollEx.Controls.Clear();
				ScrollEx.Controls.AddRange(list.ToArray());
				ScrollEx.FireScrollEventOnMouseWheel = true;
				new ScrollHelper(ScrollEx).EnableScrollOnMouseWheel();
				ScrollEx.Visible = false;
				ScrollEx.Visible = true;
				Refresh();
			}
		}
	}

	public virtual Enums.FormViewModeType FormMode
	{
		get
		{
			return _formMode;
		}
		set
		{
			SetFormMode(value);
		}
	}

	public bool IsSetDataSouceControl { get; set; }

	public string FirstItemId { get; set; }

	public GridViewEx DefaultGrid { get; set; }

	public TreeEx DefaultTree { get; set; }

	public PivotGridControlEx DefaultPivot { get; set; }

	protected static Point DialogFormLocation { get; set; }

	protected static Size DialogFormSize { get; set; }

	private Size DialogFormNormalSize { get; set; }

	private Point DialogFormNormalLocation { get; set; }

	public object DataSource
	{
		get
		{
			return _dataSource;
		}
		set
		{
			IsSetDataSouceControl = true;
			_dataSource = value;
			if (value == null)
			{
				for (int i = 0; i < LstControlSource.Count; i++)
				{
					LstControlSource[i].EditValue = null;
				}
				IsSetDataSouceControl = false;
				return;
			}
			if (value is DataTable)
			{
				DataTable dataTable = value as DataTable;
				if (!dataTable.IsNotNull())
				{
					IsSetDataSouceControl = false;
					return;
				}
				for (int j = 0; j < LstControlSource.Count; j++)
				{
					if (dataTable.Columns.Contains(LstControlSource[j].FieldName))
					{
						object editValue = dataTable.Rows[0][LstControlSource[j].FieldName];
						LstControlSource[j].EditValue = editValue;
					}
				}
				IsSetDataSouceControl = false;
				return;
			}
			if (value is DataRow)
			{
				DataRow dataRow = value as DataRow;
				for (int k = 0; k < LstControlSource.Count; k++)
				{
					IControlEx controlEx = LstControlSource[k];
					if (dataRow.Table.Columns.Contains(controlEx.FieldName))
					{
						object editValue = dataRow[controlEx.FieldName];
						controlEx.EditValue = editValue;
					}
				}
				IsSetDataSouceControl = false;
				return;
			}
			for (int l = 0; l < LstControlSource.Count; l++)
			{
				IControlEx controlEx2 = LstControlSource[l];
				if (!(value.GetType().GetProperty(controlEx2.FieldName) == null))
				{
					object editValue = value.GetType().GetProperty(controlEx2.FieldName).GetValue(value);
					controlEx2.EditValue = editValue;
				}
			}
			IsSetDataSouceControl = false;
		}
	}

	public DataTable DataSourceTittle
	{
		get
		{
			return _dataSourceTitle;
		}
		set
		{
			_dataSourceTitle = value;
			if (value == null || !_dataSourceTitle.IsNotNull() || !_dataSourceTitle.Columns.Contains("ColumnId") || !_dataSourceTitle.Columns.Contains("ColumnName"))
			{
				return;
			}
			for (int i = 0; i < LstControlTittleSource.Count; i++)
			{
				IControlEx control = LstControlTittleSource[i];
				DataRow dataRow = _dataSourceTitle.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(control.FieldName));
				if (dataRow == null)
				{
					continue;
				}
				bool flag = Function.ParseBool(dataRow["Visible"]);
				bool flag2 = !Function.ParseBool(dataRow["Editable"]);
				control.Text = Function.ToString(dataRow["ColumnName"]);
				if (control.Visible)
				{
					control.Visible = flag;
				}
				control.SettingInVisible = !flag;
				control.ReadOnly = flag2;
				control.SettingReadonly = flag2;
				if (control.EditControl != null)
				{
					if (control.EditControl.Visible)
					{
						control.EditControl.Visible = flag;
					}
					control.EditControl.SettingInVisible = !flag;
					control.EditControl.ReadOnly = flag2;
					control.EditControl.SettingReadonly = flag2;
				}
			}
			DataRow dataRow2 = _dataSourceTitle.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(GetType().Name));
			if (dataRow2 != null)
			{
				Text = Function.ToString(dataRow2["ColumnName"]);
			}
		}
	}

	public event LoadAsyncEventHandler OnLoadAsync;

	[Browsable(true)]
	public event ControlValueChanged OnControlValueChanged;

	[Browsable(true)]
	public event ControlValueChanging OnControlValueChanging;

	public void SetMinScrollSize(Size size)
	{
		ScrollMinSize = size;
		ScrollEx.AutoScrollMinSize = new Size(ScrollMinSize.Width - 20, ScrollMinSize.Height - 50);
	}

	protected virtual void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
	}

	public XtraFormEx()
	{
		CheckAuthorization();
		if (_icon == null)
		{
			_icon = (Icon)new ComponentResourceManager(typeof(XtraFormEx)).GetObject("XtraFormEx.IconOptions.Icon");
		}
		DoubleBuffered = true;
	}

	protected virtual void CheckAuthorization()
	{
		if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DisableAuthorization)
		{
			return;
		}
		HttpClient = HttpClientHelperFactory.GetHttpClient(FormObjectType);
		string formName = GetType().Name;
		if (_formNameCheckAuth.ContainsKey(formName))
		{
			formName = _formNameCheckAuth[formName];
		}
		if (formName.Equals("Main"))
		{
			return;
		}
		if (GlobalData.UserName != null && GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser) && AdminFormSetupSystem.Contains(formName))
		{
			FormAuth = Enums.FormAuthorities.Full;
			return;
		}
		if (GlobalData.ListAccessForm == null)
		{
			throw new Exception(Resources.Msg_FormAccessDenied);
		}
		Shown += XtraFormEx_Shown;
		AuthsImpl authsImpl = GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl t) => t.menucode == formName);
		if (authsImpl == null)
		{
			FormAuth = Enums.FormAuthorities.Full;
			return;
		}
		if (!authsImpl.authoriza.Equals("N"))
		{
			if (authsImpl.authoriza.Equals("F"))
			{
				FormAuth = Enums.FormAuthorities.Full;
			}
			else
			{
				FormAuth = Enums.FormAuthorities.Read;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void ClearMappingFields()
	{
		try
		{
			if (ListGrid == null)
			{
				ListGrid = ControlFunction.GetListEditGriview(this);
			}
			foreach (ViewEditModel item in ListGrid)
			{
				if (item.Grid is GridViewEx gridViewEx)
				{
					gridViewEx.ClearListMappingField();
				}
				else if (item.Grid is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.ClearListMappingField();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual void XtraFormEx_Shown(object sender, EventArgs e)
	{
		try
		{
			if (!DisableContextAction)
			{
				Task.Run(() =>
				{
					ClearMappingFields();
				});
			}
			ContextMenuStrip contextMenuStrip = ContextMenuStrip;
			if (contextMenuStrip == null)
			{
				contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
			}
			if (!_setFontContextMenu)
			{
				contextMenuStrip.Font = new Font(contextMenuStrip.Font.FontFamily, contextMenuStrip.Font.Size + 0.5f, contextMenuStrip.Font.Unit);
				_setFontContextMenu = true;
			}
			if (!DisableContextAction)
			{
				if (contextMenuStrip.Items.Count != 0)
				{
					contextMenuStrip.Items.Add(new ToolStripSeparator());
				}
				if (string.IsNullOrEmpty(FirstItemId))
				{
					FirstItemId = "B$" + GetFirstItemId();
				}
				GetDataContextAction();
				if (FormAuth == Enums.FormAuthorities.Full && GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl r) => r.menucode == "barSub_ContextAction" && r.authoriza != "N") != null)
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem
					{
						Name = "CTM_ADD_ITEM",
						Text = (DsContextAction.IsNotNull() ? "Update Buttons" : "Add Buttons")
					};
					toolStripMenuItem.Click += CtxAction_Click;
					contextMenuStrip.Items.Add(toolStripMenuItem);
					toolStripMenuItem = new ToolStripMenuItem
					{
						Name = "CTM_Move_Control",
						Text = "Moving user-defined Controls"
					};
					toolStripMenuItem.Click += Moving_Click;
					contextMenuStrip.Items.Add(toolStripMenuItem);
				}
				ReloadContextAction(reload: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		AfterOpen(e);
	}

	protected virtual void GetDataContextAction()
	{
		DsContextAction = HttpClient.ContextAction.GetData(FormId, FirstItemId);
	}

	protected void Moving_Click(object sender, EventArgs e)
	{
		try
		{
			if (UserDefinedControl == null || !(sender is ToolStripMenuItem toolStripMenuItem))
			{
				return;
			}
			if (toolStripMenuItem.Text.StartsWith("Mov"))
			{
				for (int i = 0; i < UserDefinedControl.Count; i++)
				{
					UserDefinedControl[i].Draggable(Enable: true);
					if (UserDefinedControl[i] is SimpleButtonEx)
					{
						(UserDefinedControl[i] as SimpleButtonEx).ButtonStyle = BorderStyles.Flat;
					}
					else if (UserDefinedControl[i] is DropDownButtonEx)
					{
						(UserDefinedControl[i] as DropDownButtonEx).ButtonStyle = BorderStyles.Flat;
					}
				}
				toolStripMenuItem.Text = "Freeze user-defined Controls";
				_isMovingControl = true;
				return;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FormId", typeof(string));
			dataTable.Columns.Add("ItemId", typeof(string));
			dataTable.Columns.Add("AbsEntry", typeof(int));
			dataTable.Columns.Add("LocationX", typeof(int));
			dataTable.Columns.Add("LocationY", typeof(int));
			dataTable.Columns.Add("LocationLeft", typeof(int));
			dataTable.Columns.Add("LocationTop", typeof(int));
			dataTable.Columns.Add("LocationRight", typeof(int));
			dataTable.Columns.Add("LocationBottom", typeof(int));
			dataTable.Columns.Add("Anchor", typeof(string));
			if (string.IsNullOrEmpty(FirstItemId))
			{
				FirstItemId = "B$" + GetFirstItemId();
			}
			for (int j = 0; j < UserDefinedControl.Count; j++)
			{
				Control control = UserDefinedControl[j];
				DataRow dataRow = dataTable.NewRow();
				dataRow["FormId"] = FormId;
				dataRow["ItemId"] = FirstItemId;
				dataRow["AbsEntry"] = Function.ParseInt(control.Name.Replace("cta_btn_", ""));
				dataRow["LocationX"] = control.Location.X;
				dataRow["LocationY"] = Height - control.Location.Y;
				dataRow["LocationLeft"] = control.Left;
				dataRow["LocationTop"] = control.Top;
				dataRow["LocationRight"] = control.Right;
				dataRow["LocationBottom"] = control.Bottom;
				dataRow["Anchor"] = ((control.Location.Y > Height / 2) ? "1001" : "1100");
				dataTable.Rows.Add(dataRow);
				control.Draggable(Enable: false);
				if (control is SimpleButtonEx)
				{
					(control as SimpleButtonEx).ButtonStyle = BorderStyles.Default;
				}
				else if (control is DropDownButtonEx)
				{
					(control as DropDownButtonEx).ButtonStyle = BorderStyles.Default;
				}
			}
			ContextActionSavePoint(dataTable);
			toolStripMenuItem.Text = "Moving user-defined Controls";
			_isMovingControl = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	protected virtual void ContextActionSavePoint(DataTable dt)
	{
		HttpClient.ContextAction.SavePoint(dt);
	}

	protected virtual void CtxAction_Click(object sender, EventArgs e)
	{
		try
		{
			DataSet dataSet = DataSet();
			if (dataSet.Tables.Count == 1 && (DefaultGrid != null || DefaultPivot != null || DefaultTree != null))
			{
				Type type = Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms");
				if (type != null)
				{
					((XtraFormEx)Activator.CreateInstance(type, FormId, FirstItemId, dataSet.Tables[0], this)).ShowDialog(this);
					ReloadContextAction();
				}
				return;
			}
			DataTable dataTable = new DataTable();
			foreach (DataTable table in dataSet.Tables)
			{
				foreach (DataColumn column in table.Columns)
				{
					dataTable.Columns.Add($"{table.TableName}.{column.ColumnName}", column.DataType);
				}
			}
			Type type2 = Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms");
			if (type2 != null)
			{
				((XtraFormEx)Activator.CreateInstance(type2, FormId, FirstItemId, dataTable, this)).ShowDialog(this);
				ReloadContextAction();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public DataSet DataSet()
	{
		DataSet dataSet = new DataSet();
		try
		{
			if (DataSource is DataTable dataTable && dataTable.Columns.Count > 0)
			{
				if (dataTable.IsNotNull())
				{
					DataTable dataTable2 = new DataRow[1] { dataTable.Rows[0] }.CopyToDataTable();
					dataTable2.TableName = FormId;
					dataSet.Tables.Add(dataTable2);
				}
				else
				{
					dataTable.TableName = FormId;
					dataSet.Tables.Add(dataTable);
				}
			}
			FirstItemId = "";
			if (ListGrid == null)
			{
				ListGrid = ControlFunction.GetListEditGriview(this);
			}
			if (ListGrid.Count > 0)
			{
				for (int i = 0; i < ListGrid.Count; i++)
				{
					if (string.IsNullOrEmpty(ListGrid[i].ItemId))
					{
						continue;
					}
					if (string.IsNullOrEmpty(FirstItemId))
					{
						FirstItemId = "B$" + ListGrid[i].ItemId;
					}
					DataTable dataSource = ListGrid[i].DataSource;
					if (dataSource.Columns.Count <= 0)
					{
						continue;
					}
					dataSource.TableName = ListGrid[i].ItemId;
					dataSet.Tables.Add(dataSource.Copy());
					if (DefaultGrid == null && DefaultPivot == null && DefaultTree == null)
					{
						if (ListGrid[i].Grid is GridViewEx)
						{
							DefaultGrid = ListGrid[i].Grid as GridViewEx;
						}
						else if (ListGrid[i].Grid is TreeEx)
						{
							DefaultTree = ListGrid[i].Grid as TreeEx;
						}
						else if (ListGrid[i].Grid is PivotGridControlEx)
						{
							DefaultPivot = ListGrid[i].Grid as PivotGridControlEx;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(FirstItemId))
			{
				FirstItemId = "B$";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return dataSet;
	}

	public virtual string GetFirstItemId()
	{
		try
		{
			if (ListGrid == null)
			{
				ListGrid = ControlFunction.GetListEditGriview(this);
			}
			if (ListGrid.Count > 0)
			{
				for (int i = 0; i < ListGrid.Count; i++)
				{
					if (!string.IsNullOrEmpty(ListGrid[i].ItemId))
					{
						return ListGrid[i].ItemId;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	protected virtual void ReloadContextAction(bool reload = true)
	{
		try
		{
			if (reload)
			{
				if (string.IsNullOrEmpty(FirstItemId))
				{
					FirstItemId = "B$" + GetFirstItemId();
				}
				GetContextActionData();
			}
			if (!DsContextAction.IsNotNull())
			{
				return;
			}
			DataTable dataTable = DsContextAction.Tables[0];
			UserDefinedControl = new List<Control>();
			foreach (DataRow item in from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["FatherId"]) <= 0 && Function.ToString(t["Invisible"]) != "Y"
				select t)
			{
				LoadActionControl(item, dataTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	protected virtual void GetContextActionData()
	{
		if (!DisableContextAction)
		{
			DsContextAction = HttpClient.ContextAction.GetData(FormId, FirstItemId);
		}
	}

	protected void LoadActionControl(DataRow row, DataTable dtDoc)
	{
		int absEntry = Function.ParseInt(row["AbsEntry"]);
		bool flag = Function.ParseInt(row["EditorType"]) == 3;
		if (flag)
		{
			if (AcceptButton is SimpleButtonEx simpleButtonEx)
			{
				simpleButtonEx.Enabled = FormAuth == Enums.FormAuthorities.Full;
				string text = Function.ToString(row["Icon"]);
				if (!string.IsNullOrEmpty(text))
				{
					simpleButtonEx.Image = ImageResourceCache.Default.GetImageById(text, ImageSize.Size16x16, ImageType.Office2013);
				}
				simpleButtonEx.Text = Function.ToString(row["Caption"]);
				simpleButtonEx.OnClickAsync += async (object sender, EventArgs events) =>
				{
					await HandleEvent(absEntry);
				};
			}
			return;
		}
		DataRow[] dr = (from t in dtDoc.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) == absEntry && Function.ToString(t["Invisible"]) != "Y"
			select t).ToArray();
		Enums.ContextActionActionTypeEx contextActionActionTypeEx = (Enums.ContextActionActionTypeEx)Function.ParseInt(row["ActionTypeEx"]);
		if (dr.Length != 0 && contextActionActionTypeEx != Enums.ContextActionActionTypeEx.WorkFlowProcessGroup)
		{
			DropDownButtonEx dropDownButtonEx = new DropDownButtonEx();
			dropDownButtonEx.Enabled = FormAuth == Enums.FormAuthorities.Full;
			string text2 = Function.ToString(row["Icon"]);
			if (!string.IsNullOrEmpty(text2))
			{
				dropDownButtonEx.Image = ImageResourceCache.Default.GetImageById(text2, ImageSize.Size16x16, ImageType.Office2013);
			}
			dropDownButtonEx.Name = "cta_btn_" + absEntry;
			dropDownButtonEx.Text = Function.ToString(row["Caption"]);
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			for (int num = 0; num < dr.Length; num++)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = Function.ToString(dr[num]["Caption"]);
				dXPopupMenu.Items.Add(dXMenuItem);
				if (Function.ParseInt(dr[num]["ActionTypeEx"]) != 0)
				{
					int i1 = num;
					dXMenuItem.Click += async (object s, EventArgs events) =>
					{
						await HandleEvent(Function.ParseInt(dr[i1]["AbsEntry"]));
					};
				}
				string text3 = Function.ToString(dr[num]["Icon"]);
				if (!string.IsNullOrEmpty(text3))
				{
					dXMenuItem.Image = ImageResourceCache.Default.GetImageById(text3, ImageSize.Size16x16, ImageType.Office2013);
				}
			}
			dropDownButtonEx.DropDownControl = dXPopupMenu;
			dropDownButtonEx.Appearance.BorderColor = Color.Black;
			dropDownButtonEx.Location = new Point(Function.ParseInt(row["LocationX"]), Function.ParseInt(row["LocationY"]));
			Size size = dropDownButtonEx.CalcBestSize();
			dropDownButtonEx.Width = size.Width + 10;
			dropDownButtonEx.Height = size.Height;
			dropDownButtonEx.MinimumSize = new Size(75, 23);
			dropDownButtonEx.Padding = new Padding(0, 0, 0, 0);
			string text4 = Function.ToString(row["Anchor"]);
			if (!(text4 == "1001"))
			{
				if (text4 == "1100")
				{
					dropDownButtonEx.Left = Function.ParseInt(row["LocationLeft"]);
					dropDownButtonEx.Top = Function.ParseInt(row["LocationTop"]);
					dropDownButtonEx.Anchor = AnchorStyles.Top | AnchorStyles.Left;
				}
			}
			else
			{
				dropDownButtonEx.Location = new Point(Function.ParseInt(row["LocationX"]), Height - Function.ParseInt(row["LocationY"]));
				dropDownButtonEx.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			}
			if (IsScrollable)
			{
				Controls[0].Controls.Add(dropDownButtonEx);
			}
			else
			{
				Controls.Add(dropDownButtonEx);
			}
			UserDefinedControl.Add(dropDownButtonEx);
			dropDownButtonEx.BringToFront();
			if (contextActionActionTypeEx != Enums.ContextActionActionTypeEx.None)
			{
				dropDownButtonEx.Click += async (object s, EventArgs events) =>
				{
					await HandleEvent(absEntry);
				};
			}
			if (flag)
			{
				AcceptButton = dropDownButtonEx;
			}
			return;
		}
		SimpleButtonEx simpleButtonEx2 = new SimpleButtonEx();
		simpleButtonEx2.Enabled = FormAuth == Enums.FormAuthorities.Full;
		string text5 = Function.ToString(row["Icon"]);
		if (!string.IsNullOrEmpty(text5))
		{
			simpleButtonEx2.Image = ImageResourceCache.Default.GetImageById(text5, ImageSize.Size16x16, ImageType.Office2013);
		}
		simpleButtonEx2.Name = "cta_btn_" + absEntry;
		simpleButtonEx2.Text = Function.ToString(row["Caption"]);
		simpleButtonEx2.Appearance.BorderColor = Color.Black;
		if (IsScrollable)
		{
			Controls[0].Controls.Add(simpleButtonEx2);
		}
		else
		{
			Controls.Add(simpleButtonEx2);
		}
		UserDefinedControl.Add(simpleButtonEx2);
		simpleButtonEx2.BringToFront();
		simpleButtonEx2.Location = new Point(Function.ParseInt(row["LocationX"]), Function.ParseInt(row["LocationY"]));
		Size size2 = simpleButtonEx2.CalcBestSize();
		simpleButtonEx2.Width = size2.Width + 10;
		simpleButtonEx2.Height = size2.Height;
		simpleButtonEx2.MinimumSize = new Size(75, 23);
		simpleButtonEx2.Padding = new Padding(0, 0, 0, 0);
		string text6 = Function.ToString(row["Anchor"]);
		if (!(text6 == "1001"))
		{
			if (text6 == "1100")
			{
				simpleButtonEx2.Left = Function.ParseInt(row["LocationLeft"]);
				simpleButtonEx2.Top = Function.ParseInt(row["LocationTop"]);
				simpleButtonEx2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			}
		}
		else
		{
			simpleButtonEx2.Location = new Point(Function.ParseInt(row["LocationX"]), Height - Function.ParseInt(row["LocationY"]));
			simpleButtonEx2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		}
		if (contextActionActionTypeEx != Enums.ContextActionActionTypeEx.None)
		{
			simpleButtonEx2.OnClickAsync += async (object sender, EventArgs events) =>
			{
				await HandleEvent(absEntry);
			};
		}
		if (flag)
		{
			AcceptButton = simpleButtonEx2;
		}
	}

	protected virtual async Task HandleEvent(int absEntry)
	{
		_ = 1;
		try
		{
			if (_isMovingControl)
			{
				return;
			}
			Type type = Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms");
			if (type != null)
			{
				DataSet dataSet = DataSet();
				if (dataSet.Tables.Count == 1)
				{
					if (DefaultGrid != null)
					{
						Activator.CreateInstance(type, absEntry, FormId, FirstItemId, DefaultGrid.GridControl.DataSource as DataTable, DefaultGrid);
					}
					else if (DefaultTree != null)
					{
						Activator.CreateInstance(type, absEntry, FormId, FirstItemId, DefaultTree.DataSourceTable, DefaultTree);
					}
					else if (DefaultPivot != null)
					{
						Activator.CreateInstance(type, absEntry, FormId, FirstItemId, DefaultPivot.DataSource as DataTable, DefaultPivot);
					}
					else
					{
						object obj = Activator.CreateInstance(type, absEntry, FormId, FirstItemId, dataSet, this);
						MethodInfo method = type.GetMethod("Handle");
						if (method != null)
						{
							WinCommons._OpenProcessing();
							AfterAction(await (Task<HttpResult>)method.Invoke(obj, null));
						}
					}
				}
				else
				{
					object obj2 = Activator.CreateInstance(type, absEntry, FormId, FirstItemId, dataSet, this);
					MethodInfo method2 = type.GetMethod("Handle");
					if (method2 != null)
					{
						WinCommons._OpenProcessing();
						AfterAction(await (Task<HttpResult>)method2.Invoke(obj2, null));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public virtual void AfterAction(HttpResult ret, bool resetData = false)
	{
	}

	[DllImport("user32.dll")]
	private static extern bool EnableWindow(IntPtr hWnd, int bEnable);

	[DllImport("user32.dll")]
	private static extern bool ReleaseCapture();

	[DllImport("user32.dll")]
	public static extern long LockWindowUpdate(IntPtr hWndLock);

	public DialogResult ShowDialog(System.Windows.Forms.Form frmParent)
	{
		ShowInTaskbar = false;
		MinimizeBox = false;
		Owner = frmParent;
		StartPosition = FormStartPosition.CenterScreen;
		EnableWindow(frmParent.Handle, 0);
		ReleaseCapture();
		SizeChanged += DialogForm_SizeChanged;
		frmParent.Activated += Parent_Actived;
		Activated += Modal_Actived;
		EnableAllDirectionalControl(isEnable: true);
		Enums.DirectionalControl[] gridToolbarButtons = GridToolbarButtons;
		foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
		{
			EnableDirectionalControl(diectioControl, isEnable: false);
		}
		Show();
		DialogFormNormalSize = Size;
		DialogFormNormalLocation = Location;
		while (!Disposing && !IsDisposed && DialogResult == DialogResult.None)
		{
			Application.DoEvents();
			Thread.Sleep(100);
		}
		if (frmParent.IsDisposed || frmParent.Disposing)
		{
			return DialogResult;
		}
		frmParent.Activated -= Parent_Actived;
		EnableWindow(frmParent.Handle, 1);
		frmParent.Focus();
		if (frmParent is XtraFormEx xtraFormEx)
		{
			xtraFormEx.EnableAllDirectionalControl(isEnable: true);
			if (EnableDirectionalControl != null)
			{
				EditorContainer activeGridControl = xtraFormEx.ActiveControl.GetActiveGridControl();
				bool flag = activeGridControl is GridControlEx || activeGridControl is TreeEx || activeGridControl is PivotGridControlEx;
				gridToolbarButtons = GridToolbarButtons;
				foreach (Enums.DirectionalControl diectioControl2 in gridToolbarButtons)
				{
					EnableDirectionalControl(diectioControl2, flag);
				}
				if (flag)
				{
					activeGridControl.Focus();
				}
			}
		}
		Close();
		return DialogResult;
	}

	private void DialogForm_SizeChanged(object sender, EventArgs e)
	{
		if (WindowState == FormWindowState.Maximized)
		{
			WindowState = FormWindowState.Normal;
			if (Size.Width == DialogFormSize.Width && Size.Height == DialogFormSize.Height)
			{
				Location = DialogFormNormalLocation;
				Size = DialogFormNormalSize;
			}
			else
			{
				Location = DialogFormLocation;
				Size = DialogFormSize;
			}
		}
	}

	private void Modal_Actived(object sender, EventArgs e)
	{
		try
		{
			if (Owner == null || Owner.IsMdiContainer)
			{
				return;
			}
			FormCollection openForms = Application.OpenForms;
			if (openForms.Count == 0)
			{
				return;
			}
			System.Windows.Forms.Form form = openForms["Main"];
			if (form != null)
			{
				System.Windows.Forms.Form owner = Owner;
				while (owner != null && owner.MdiParent == null)
				{
					owner = owner.Owner;
				}
				if (owner != null && owner.MdiParent != null && owner != form.ActiveMdiChild)
				{
					EnableWindow(Owner.Handle, 1);
					Owner.Activate();
					EnableWindow(Owner.Handle, 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Parent_Actived(object sender, EventArgs e)
	{
		Activate();
	}

	public new void Show()
	{
		try
		{
			BeforeOpen(out var cancelEvent);
			if (!cancelEvent)
			{
				base.Show();
			}
			else
			{
				ForceClose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
	}

	private void ForceClose()
	{
		BeginInvoke(new MethodInvoker(Close));
	}

	public virtual void FORMSETTING()
	{
		try
		{
			EditorContainer activeGridControl = ActiveControl.GetActiveGridControl();
			if (activeGridControl != null)
			{
				if (activeGridControl is GridControlEx)
				{
					BaseView mainView = (activeGridControl as GridControlEx).MainView;
					if (mainView is GridViewEx)
					{
						(mainView as GridViewEx).ShowFormSetting(this);
					}
					else
					{
						(mainView as BandedGridViewEx)?.ShowFormSetting(this);
					}
				}
				else
				{
					(activeGridControl as TreeEx)?.ShowFormSetting(this);
				}
			}
			else
			{
				ShowSettingForm();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
	}

	public virtual async Task ReloadFormSetting(Control ctl, Enums.PivotFormSetting pivotType = Enums.PivotFormSetting.None)
	{
		try
		{
			foreach (Control control in ctl.Controls)
			{
				if (control is GridControlEx gridControlEx)
				{
					if (gridControlEx.DefaultView is GridViewEx gridViewEx)
					{
						if (!gridViewEx.DisableFormSetting && !gridViewEx.NotLoadFormsettingAtLoad && gridViewEx.FirstLoadFormSetting == null)
						{
							gridViewEx.FirstLoadFormSetting = gridViewEx.ReloadFormSetting(pivotType);
						}
					}
					else
					{
						(gridControlEx.DefaultView as BandedGridViewEx)?.ReloadFormSetting();
					}
				}
				else if (!(control is TreeEx treeEx))
				{
					await ReloadFormSetting(control, pivotType);
				}
				else if (!treeEx.DisableFormSetting && !treeEx.NotLoadFormsettingAtLoad && treeEx.FirstLoadFormSetting == null)
				{
					treeEx.FirstLoadFormSetting = treeEx.ReloadFormSetting();
				}
			}
			CustomFormSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
	}

	public virtual void CustomFormSetting()
	{
	}

	public virtual async Task ReloadFormatRule(Control ctl)
	{
		try
		{
			List<Task> list = new List<Task>();
			foreach (Control control in ctl.Controls)
			{
				if (control is GridControlEx && (control as GridControlEx).DefaultView is GridViewEx gridViewEx)
				{
					list.Add(ExtensionMethods.ReloadFormatRuleGrid(gridViewEx, gridViewEx.FormId));
				}
				if (control is TreeEx treeEx)
				{
					list.Add(ExtensionMethods.ReloadFormatRuleTree(treeEx, treeEx.FormId));
				}
				if (control is PivotGridControlEx pivotGridControlEx)
				{
					list.Add(ExtensionMethods.ReloadFormatRulePivot(pivotGridControlEx, pivotGridControlEx.FormId));
				}
				else
				{
					list.Add(ReloadFormatRule(control));
				}
			}
			await Task.WhenAll(list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			XtraMessageBox.Show(ex.Message);
		}
	}

	protected override async void OnLoad(EventArgs e)
	{
		if (GetType().Name == "Main")
		{
			base.OnLoad(e);
			return;
		}
		IsLoadForm = true;
		if (!CancelOpen)
		{
			if (IconOptions.Icon == null)
			{
				IconOptions.Icon = _icon;
			}
			IconOptions.ShowIcon = false;
			if (IsSetSuperTip)
			{
				SetToolTipController(this);
			}
			SetControlFormReadOnly();
			if (OnLoadAsync != null)
			{
				await OnLoadAsync(this, e);
			}
			if (ViewSystemInformation)
			{
				AddViewComponentInfoEvents(this);
			}
			base.OnLoad(e);
		}
		ReloadSettingForm();
		await ReloadFormSetting(this);
		ReloadFormatRule(this);
		IsLoadForm = false;
	}

	public void SetControlFormReadOnly()
	{
		if (FormAuth == Enums.FormAuthorities.NoAuthorities)
		{
			throw new Exception("User does not have permission to access this function.");
		}
		if (FormAuth == Enums.FormAuthorities.Read)
		{
			if (ContextMenuStrip != null)
			{
				ContextMenuStrip.Enabled = false;
			}
			Controls.EnableDisableControl(isEnable: false, readAuth: true);
			NoAuthSetControlSearch();
		}
	}

	private void SetToolTipController(Control ctr)
	{
		try
		{
			ToolTipController toolTipController = new ToolTipController();
			toolTipController.GetActiveObjectInfo += ttc_GetActiveObjectInfo;
			foreach (Control control in ctr.Controls)
			{
				if (control is BaseEdit baseEdit)
				{
					baseEdit.ToolTipController = toolTipController;
				}
				else if (control is BaseControl)
				{
					((BaseControl)control).ToolTipController = toolTipController;
				}
				else
				{
					SetToolTipController(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ttc_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			Control selectedControl = e.SelectedControl;
			string text = "";
			string text2 = "";
			if (e.SelectedControl is BaseEdit)
			{
				text2 = ((BaseEdit)selectedControl).ToolTip;
				int num = ((BaseEdit)selectedControl).Width;
				int num2 = ((BaseEdit)selectedControl).CalcBestSize().Width;
				if (selectedControl is TextEdit textEdit && textEdit.Properties.UseSystemPasswordChar)
				{
					return;
				}
				if ((!(selectedControl is MemoEdit) && num2 > num) || (selectedControl is MemoEdit && ((MemoEdit)selectedControl).Height * 2 < ((MemoEdit)selectedControl).CalcAutoHeight()))
				{
					text = e.SelectedControl.Text;
				}
				else if (string.IsNullOrEmpty(text2))
				{
					return;
				}
			}
			else if (e.SelectedControl is BaseControl)
			{
				if (selectedControl is TextBox { UseSystemPasswordChar: not false })
				{
					return;
				}
				text = e.SelectedControl.Text;
				text2 = ((BaseControl)selectedControl).ToolTip;
			}
			e.Info = new ToolTipControlInfo($"{1} + {2}", text, text2)
			{
				IconType = (string.IsNullOrEmpty(text2) ? ToolTipIconType.None : ToolTipIconType.Information)
			};
			if (sender is ToolTipController toolTipController)
			{
				toolTipController.KeepWhileHovered = true;
				int num3 = Function.ParseInt(text.Length) * 50;
				toolTipController.AutoPopDelay = ((num3 <= 5000) ? 5000 : num3);
				int num4 = Function.ParseInt(text.Length) * 2;
				toolTipController.InitialDelay = ((num4 <= 500) ? 500 : num4);
				if (num4 <= 500)
				{
					toolTipController.InitialDelay = 500;
				}
				else if (num4 >= 5000)
				{
					toolTipController.InitialDelay = 5000;
				}
				else
				{
					toolTipController.InitialDelay = num4;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void NoAuthSetControlSearch()
	{
	}

	protected virtual void AfterOpen(EventArgs e)
	{
		try
		{
			if (DisableLoadScript || string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(FormId))
			{
				return;
			}
			if (SyntaxAfterOpen == null)
			{
				string text = FormId;
				if (FormId.Equals("oUserDefinedReports"))
				{
					try
					{
						text += ((dynamic)this).ItemId;
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
				string formulaText = HttpClient.Formula.GetFormulaText(FormId, Enums.EventId.OnAfterOpen, text);
				SyntaxAfterOpen = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxAfterOpen.Count != 0)
			{
				CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(this, SyntaxAfterOpen, out var _);
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	protected virtual void BeforeOpen(out bool cancelEvent)
	{
		cancelEvent = false;
	}

	public void AddViewComponentInfoEvents(Control control)
	{
		try
		{
			if (control.Controls != null)
			{
				foreach (Control control2 in control.Controls)
				{
					AddViewComponentInfoEvents(control2);
				}
			}
			if (control is BaseEdit || control is GridControlEx || control is TreeEx || control is TextBox)
			{
				control.MouseEnter -= MouseEnterComponent;
				control.MouseLeave -= MouseLeaveComponent;
				if (ViewSystemInformation)
				{
					control.MouseEnter += MouseEnterComponent;
					control.MouseLeave += MouseLeaveComponent;
				}
			}
			if (control is GridControlEx)
			{
				(control as GridControlEx).MainView.MouseMove -= gridView_MouseMove;
				if (ViewSystemInformation)
				{
					(control as GridControlEx).MainView.MouseMove += gridView_MouseMove;
				}
			}
			else if (control is TreeEx treeEx)
			{
				treeEx.MouseMove -= tree_MouseMove;
				if (ViewSystemInformation)
				{
					treeEx.MouseMove += tree_MouseMove;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MouseEnterComponent(object sender, EventArgs e)
	{
		try
		{
			MouseOnComponent = true;
			if (!(sender is GridControlEx) && !(sender is TreeEx) && !(sender is XtraFormEx) && !(sender is TextBoxMaskBox))
			{
				string text = Function.ToString((sender as Control).Tag);
				string text2 = ((!string.IsNullOrEmpty(FormId)) ? ("FormId= " + FormId) : "");
				string text3 = ((!string.IsNullOrEmpty(text)) ? (text ?? "") : "");
				string text4 = "";
				string text5 = ((!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3)) ? ", " : "");
				string text6 = (((!string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text2)) && !string.IsNullOrEmpty(text4)) ? ", " : "");
				string text7 = "[" + text2 + text5 + text3 + text6 + text4 + "]";
				if (!string.IsNullOrEmpty(text7))
				{
					ViewComponentInfo(text7);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ViewComponentInfo(string componentInfo)
	{
		try
		{
			if (!ViewSystemInformation)
			{
				return;
			}
			if (MouseOnComponent)
			{
				if (!MainMessage.MsgEnable || (MainMessage.MsgEnable && (MainMessage.MsgColor == Enums.MessageType.Info || MainMessage.MsgColor == Enums.MessageType.Transparent)))
				{
					WinCommons.InfoForm(Enums.MessageCode.None, componentInfo);
				}
			}
			else if (MainMessage.MsgEnable && MainMessage.MsgColor == Enums.MessageType.Info)
			{
				WinCommons.Transparent(Enums.MessageCode.None, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MouseLeaveComponent(object sender, EventArgs e)
	{
		try
		{
			MouseOnComponent = false;
			string componentInfo = Function.ToString((sender as Control).Tag) + " " + (sender as Control).Name;
			ViewComponentInfo(componentInfo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (!(sender is GridViewEx gridViewEx))
			{
				return;
			}
			GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(e.Location);
			if (gridHitInfo.Column != prevColumn)
			{
				prevColumn = gridHitInfo.Column;
				string text = ((!string.IsNullOrEmpty(gridViewEx.FormId)) ? ("FormId= " + gridViewEx.FormId) : "");
				if (string.IsNullOrEmpty(text))
				{
					text = ((!string.IsNullOrEmpty(FormId)) ? ("FormId= " + FormId) : "");
				}
				string text2 = ((!string.IsNullOrEmpty(gridViewEx.ItemId)) ? ("ItemId= " + gridViewEx.ItemId) : "");
				string text3 = "";
				if (gridHitInfo.Column != null)
				{
					text3 = gridHitInfo.Column.FieldName ?? "";
				}
				string text4 = ((!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2)) ? ", " : "");
				string text5 = (((!string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text)) && !string.IsNullOrEmpty(text3)) ? ", " : "");
				string componentInfo = "[" + text + text4 + text2 + text5 + text3 + "]";
				ViewComponentInfo(componentInfo);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tree_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (!(sender is TreeEx treeEx))
			{
				return;
			}
			TreeListHitInfo treeListHitInfo = treeEx.CalcHitInfo(e.Location);
			if (treeListHitInfo.Column != _prevTreeCol)
			{
				_prevTreeCol = treeListHitInfo.Column;
				string text = ((!string.IsNullOrEmpty(treeEx.FormId)) ? ("FormId= " + treeEx.FormId) : "");
				if (string.IsNullOrEmpty(text))
				{
					text = ((!string.IsNullOrEmpty(FormId)) ? ("FormId= " + FormId) : "");
				}
				string text2 = ((!string.IsNullOrEmpty(treeEx.ItemId)) ? ("ItemId= " + treeEx.ItemId) : "");
				string text3 = "";
				if (treeListHitInfo.Column != null)
				{
					text3 = treeListHitInfo.Column.FieldName ?? "";
				}
				string text4 = ((!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2)) ? ", " : "");
				string text5 = (((!string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text)) && !string.IsNullOrEmpty(text3)) ? ", " : "");
				string componentInfo = "[" + text + text4 + text2 + text5 + text3 + "]";
				if (!MouseOnComponent)
				{
					MouseOnComponent = true;
				}
				ViewComponentInfo(componentInfo);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (ExitApplication)
		{
			e.Cancel = false;
			return;
		}
		if (!IsCanClose && !IsFormClosing)
		{
			e.Cancel = true;
			return;
		}
		IsFormClosing = false;
		base.OnFormClosing(e);
	}

	public void EnableAllDirectionalControl(bool isEnable)
	{
		if (EnableDirectionalControl != null)
		{
			EnableDirectionalControl(Enums.DirectionalControl.PreviewLayout, isEnable && CheckExistsMethodsInForm("PREVIEWLAYOUTS"));
			EnableDirectionalControl(Enums.DirectionalControl.PrintLayout, isEnable && CheckExistsMethodsInForm("PRINTLAYOUT"));
			EnableDirectionalControl(Enums.DirectionalControl.Print, isEnable && CheckExistsMethodsInForm("PRINT"));
			EnableDirectionalControl(Enums.DirectionalControl.Find, isEnable && CheckExistsMethodsInForm("MODE_FIND"));
			EnableDirectionalControl(Enums.DirectionalControl.ButtonRefreshData, isEnable && CheckExistsMethodsInForm("REFRESH_DATA"));
			EnableDirectionalControl(Enums.DirectionalControl.NewDoc, ((FormAuth == Enums.FormAuthorities.Full) & isEnable) && CheckExistsMethodsInForm("REFRESH"));
			EnableDirectionalControl(Enums.DirectionalControl.ButtonModeFirst, isEnable && CheckExistsMethodsInForm("MOVE_FIRST"));
			EnableDirectionalControl(Enums.DirectionalControl.ButtonModePrev, isEnable && CheckExistsMethodsInForm("MOVE_PREVIOUS"));
			EnableDirectionalControl(Enums.DirectionalControl.ButtonModeNext, isEnable && CheckExistsMethodsInForm("MOVE_NEXT"));
			EnableDirectionalControl(Enums.DirectionalControl.ButtonLast, isEnable && CheckExistsMethodsInForm("MOVE_LAST"));
			EnableDirectionalControl(Enums.DirectionalControl.ChangeLog, isEnable && CheckExistsMethodsInForm("CHANGELOG"));
			EnableDirectionalControl(Enums.DirectionalControl.Procedure, isEnable && !DisableLoadScript);
			EnableDirectionalControl(Enums.DirectionalControl.WidgetEdit, isEnable && Name == "frm_Widget");
			EnableDirectionalControl(Enums.DirectionalControl.WidgetTable, isEnable && Name == "frm_Widget");
			EnableDirectionalControl(Enums.DirectionalControl.WidgetFlow, isEnable && Name == "frm_Widget");
			EnableDirectionalControl(Enums.DirectionalControl.WidgetStack, isEnable && Name == "frm_Widget");
			EnableDirectionalControl(Enums.DirectionalControl.WidgetFree, isEnable && Name == "frm_Widget");
			EnableDirectionalControl(Enums.DirectionalControl.WidgetSave, isEnable && Name == "frm_Widget");
		}
	}

	public void ShowProgress(int maximum, int minimum = 0, int step = 1, bool percentview = true)
	{
		if (ShowProgressBar != null)
		{
			ShowProgressBar(maximum, minimum, step, percentview);
		}
	}

	public void HideProgress(int sleep = 0)
	{
		if (HideProgressBar != null)
		{
			HideProgressBar(sleep);
		}
	}

	public void SetValueProgress(int value)
	{
		if (SetValueProgressBar != null)
		{
			SetValueProgressBar(value);
		}
	}

	public bool CheckExistsMethodsInForm(string methoName)
	{
		try
		{
			if (_lstMethod.ContainsKey(methoName))
			{
				return _lstMethod[methoName];
			}
			MethodInfo method = GetType().GetMethod(methoName);
			_lstMethod.Add(methoName, method != null);
			return method != null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public virtual void RaiseControlValueChanged(ControlChangedEventArgs e)
	{
		try
		{
			if (OnControlValueChanged != null)
			{
				OnControlValueChanged(this, e);
			}
			HandleComplierControlValueChanged(e.FieldName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual void HandleComplierControlValueChanged(string controlFieldName)
	{
		if (DisableLoadScript || string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(FormId))
		{
			return;
		}
		if (SyntaxControlValueChanged == null)
		{
			string text = FormId;
			if (FormId.Equals("oUserDefinedReports"))
			{
				try
				{
					text += ((dynamic)this).ItemId;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
			string formulaText = HttpClient.Formula.GetFormulaText(FormId, Enums.EventId.OnControlValueChanged, text);
			SyntaxControlValueChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
		}
		if (SyntaxControlValueChanged.Count != 0)
		{
			CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, -1, controlFieldName).UserdefinedFomular(this, SyntaxControlValueChanged, out var _);
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
			}
		}
	}

	public virtual void RaiseControlValueChanging(ControlChangingEventArgs e)
	{
		try
		{
			if (OnControlValueChanging != null)
			{
				OnControlValueChanging(this, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public virtual void HandleControlEvent(Enums.EventId eventId, string controlFieldName)
	{
		try
		{
			if (DisableLoadScript || string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(FormId))
			{
				return;
			}
			if (!SyntaxEventControl.ContainsKey(eventId))
			{
				string text = FormId;
				if (FormId.Equals("oUserDefinedReports"))
				{
					try
					{
						text += ((dynamic)this).ItemId;
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
				string formulaText = HttpClient.Formula.GetFormulaText(FormId, eventId, text);
				SyntaxEventControl[eventId] = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxEventControl[eventId].Count != 0)
			{
				CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, -1, controlFieldName).UserdefinedFomular(this, SyntaxEventControl[eventId], out var _);
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
	}

	public object GetDataFormSourceControl(string fieldName)
	{
		return LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName))?.EditValue;
	}

	public string GetDisplayTextControl(string fieldName)
	{
		return LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName))?.Text;
	}

	public void SetDataSourceControl(string fieldName, object value)
	{
		IControlEx controlEx = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (controlEx == null)
		{
			DataSource.ChangeSourceObjectByKey(fieldName, value);
		}
		else
		{
			controlEx.EditValue = value;
		}
	}

	public virtual async Task ReloadSettingForm()
	{
		try
		{
			if (string.IsNullOrEmpty(FormId) || FormId.Equals("oUserDefinedReports"))
			{
				return;
			}
			DataTable dataTable;
			if (GlobalData.FormSettings.ContainsKey(FormId) && GlobalData.FormSettings[FormId].Tables.Contains(FormId))
			{
				dataTable = GlobalData.FormSettings[FormId].Tables[FormId];
			}
			else
			{
				dataTable = await HttpClient.FormSettingClient.GetFormSetting(FormId, FormId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
				if (dataTable.Columns.Count > 0)
				{
					lock (GlobalData.FormSettings)
					{
						if (!GlobalData.FormSettings.ContainsKey(FormId))
						{
							GlobalData.FormSettings.Add(FormId, new DataSet());
						}
						dataTable.TableName = FormId;
						GlobalData.FormSettings[FormId].Tables.Add(dataTable);
					}
				}
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataSourceTittle = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void ShowSettingForm(bool showdialog = false)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FormId", typeof(string));
			dataTable.Columns.Add("ItemId", typeof(string));
			dataTable.Columns.Add("ColumnId", typeof(string));
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("UserSign", typeof(string));
			dataTable.Columns.Add("Width", typeof(int));
			dataTable.Columns.Add("Visible", typeof(bool));
			dataTable.Columns.Add("VisIndex", typeof(int));
			dataTable.Columns.Add("Editable", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("Fixed", typeof(string));
			dataTable.Columns.Add("AllowChSet", typeof(bool));
			dataTable.Columns.Add("VisSetting", typeof(bool));
			dataTable.Columns.Add("IsSystem", typeof(string));
			dataTable.Columns.Add("LngCode", typeof(string));
			dataTable.Columns.Add("NeverEditable", typeof(bool));
			dataTable.Columns.Add("GroupIndex", typeof(int));
			dataTable.Columns.Add("CurrField", typeof(string));
			if (DataSourceTittle.IsNotNull() && DataSourceTittle.Columns.Contains("ColumnId") && DataSourceTittle.Columns.Contains("ColumnName"))
			{
				for (int i = 0; i < DataSourceTittle.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["FormId"] = FormId;
					dataRow["ItemId"] = FormId;
					dataRow["ColumnId"] = DataSourceTittle.Rows[i]["ColumnId"];
					dataRow["ColumnName"] = DataSourceTittle.Rows[i]["ColumnName"];
					dataRow["UserSign"] = GlobalData.UserName;
					dataRow["Width"] = 10;
					dataRow["Visible"] = DataSourceTittle.Rows[i]["Visible"];
					dataRow["VisIndex"] = 1;
					dataRow["Editable"] = DataSourceTittle.Rows[i]["Editable"];
					dataRow["IsReadOnly"] = DataSourceTittle.Rows[i]["IsReadOnly"];
					dataRow["DataType"] = "string";
					dataRow["Fixed"] = "N";
					dataRow["AllowChSet"] = true;
					dataRow["VisSetting"] = true;
					dataRow["NeverEditable"] = false;
					dataRow["IsSystem"] = "Y";
					dataRow["LngCode"] = "-";
					dataRow["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow);
				}
			}
			if (LstControlTittleSource != null && LstControlTittleSource.Any())
			{
				for (int j = 0; j < LstControlTittleSource.Count; j++)
				{
					IControlEx con = LstControlTittleSource[j];
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(Function.ToString(con.FieldName))) == null)
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["FormId"] = FormId;
						dataRow2["ItemId"] = FormId;
						dataRow2["ColumnId"] = con.FieldName;
						dataRow2["ColumnName"] = con.Text;
						dataRow2["UserSign"] = GlobalData.UserName;
						dataRow2["Width"] = 10;
						dataRow2["Visible"] = true;
						dataRow2["VisIndex"] = 1;
						dataRow2["Editable"] = !con.ReadOnly;
						dataRow2["IsReadOnly"] = con.ReadOnly;
						dataRow2["DataType"] = "string";
						dataRow2["Fixed"] = "N";
						dataRow2["AllowChSet"] = true;
						dataRow2["VisSetting"] = true;
						dataRow2["NeverEditable"] = false;
						dataRow2["IsSystem"] = "Y";
						dataRow2["LngCode"] = "-";
						dataRow2["GroupIndex"] = -1;
						dataTable.Rows.Add(dataRow2);
					}
				}
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(GetType().Name)) == null)
			{
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3["FormId"] = FormId;
				dataRow3["ItemId"] = FormId;
				dataRow3["ColumnId"] = GetType().Name;
				dataRow3["ColumnName"] = Text;
				dataRow3["UserSign"] = GlobalData.UserName;
				dataRow3["Width"] = 10;
				dataRow3["Visible"] = true;
				dataRow3["VisIndex"] = 1;
				dataRow3["Editable"] = false;
				dataRow3["IsReadOnly"] = true;
				dataRow3["DataType"] = "string";
				dataRow3["Fixed"] = "N";
				dataRow3["AllowChSet"] = false;
				dataRow3["VisSetting"] = true;
				dataRow3["NeverEditable"] = false;
				dataRow3["IsSystem"] = "Y";
				dataRow3["LngCode"] = "-";
				dataRow3["GroupIndex"] = -1;
				dataTable.Rows.Add(dataRow3);
			}
			if (dataTable.IsNotNull())
			{
				frm_FormSetting frm_FormSetting2 = new frm_FormSetting
				{
					DtSource = dataTable,
					BaseForm = this,
					FormSettingId = FormId
				};
				frm_FormSetting2.IsSettingForm = true;
				if (showdialog)
				{
					frm_FormSetting2.ShowInTaskbar = true;
					frm_FormSetting2.ShowDialog();
				}
				else if (MdiParent != null)
				{
					frm_FormSetting2.MdiParent = MdiParent;
					frm_FormSetting2.Show();
				}
				else
				{
					frm_FormSetting2.ShowDialog();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual bool SetVisibleControl(string fieldName, bool isVisible)
	{
		IControlEx control = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (control == null)
		{
			return isVisible;
		}
		BeginInvoke((MethodInvoker)(() =>
		{
			control.Visible = isVisible;
		}));
		return isVisible;
	}

	public virtual void SetFocusControl(string fieldName)
	{
		IControlEx control = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (control != null)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				control.Focus();
			}));
		}
	}

	public virtual void SetReadOnlyControl(string fieldName, bool isReadOnly)
	{
		IControlEx controlEx = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (controlEx != null)
		{
			controlEx.ReadOnly = isReadOnly;
		}
	}

	public virtual void SetEnableControl(string fieldName, bool isEnable)
	{
		IControlEx controlEx = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (controlEx != null)
		{
			controlEx.Enabled = isEnable;
		}
	}

	public virtual void SetColorControl(string fieldName, Color colorControl)
	{
		IControlEx controlEx = LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(fieldName));
		if (controlEx != null)
		{
			controlEx.BackColor = colorControl;
		}
	}

	public virtual void CHANGELOG()
	{
		try
		{
			CustomChangeLogParam(out var changelog);
			if (changelog.Object != Enums.ChangeLog.clNone)
			{
				Type type = Type.GetType("Apzon.Forms.Commons.frm_ChangeLogList, Apzon.Forms");
				if (type != null)
				{
					dynamic val = Activator.CreateInstance(type);
					val.GetChangeLogParams = changelog;
					val.LogFormId = FormId;
					val.MdiParent = MdiParent;
					val.Text += " - " + Text + (string.IsNullOrWhiteSpace(changelog.PrimaryKey) ? "" : (" (" + changelog.PrimaryKey + ")"));
					val.Show();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ChangeLogNotSupportObject, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public virtual void CustomChangeLogParam(out GetChangeLogParams changelog)
	{
		changelog = new GetChangeLogParams();
		Enums.ChangeLog changeLog = Enums.ChangeLog.clNone;
		List<string> list = new List<string>();
		try
		{
			GridControlEx gridControlEx = ActiveControl as GridControlEx;
			if (gridControlEx?.FocusedView is GridViewEx gridViewEx)
			{
				changeLog = gridViewEx.ChangeLogType;
				bool isApzTable = gridViewEx.IsApzTable;
				if (CommonList.ChangeLogObjMapping.TryGetValue(changeLog, out var value))
				{
					if (CommonList.ListLogInstance.TryGetValue(value, out var value2))
					{
						LogInstance logInstance = value2.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
						if (logInstance != null)
						{
							string[] array = logInstance.Key.Split(',');
							foreach (string key in array)
							{
								if (!string.IsNullOrWhiteSpace(key))
								{
									GridColumn gridColumn = gridViewEx.Columns.FirstOrDefault((GridColumn t) => t.FieldName == key);
									GridColumn gridColumn2 = gridViewEx.Columns.FirstOrDefault((GridColumn t) => t.FieldName.ToUpper() == key.ToUpper());
									if (gridColumn != null)
									{
										list.Add(Function.ToString(gridViewEx.GetFocusedRowCellValue(gridColumn.FieldName)));
									}
									else if (gridColumn2 != null)
									{
										list.Add(Function.ToString(gridViewEx.GetFocusedRowCellValue(gridColumn2.FieldName)));
									}
								}
							}
						}
					}
					string primaryKey = string.Join(",", list);
					changelog.PrimaryKey = primaryKey;
					changelog.IsApzTable = isApzTable;
				}
			}
			if (gridControlEx?.FocusedView is BandedGridViewEx bandedGridViewEx)
			{
				changeLog = bandedGridViewEx.ChangeLogType;
				bool isApzTable = bandedGridViewEx.IsApzTable;
				if (CommonList.ChangeLogObjMapping.TryGetValue(changeLog, out var value3))
				{
					if (CommonList.ListLogInstance.TryGetValue(value3, out var value4))
					{
						LogInstance logInstance2 = value4.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
						if (logInstance2 != null)
						{
							string[] array = logInstance2.Key.Split(',');
							foreach (string key2 in array)
							{
								if (!string.IsNullOrWhiteSpace(key2))
								{
									GridColumn gridColumn3 = bandedGridViewEx.Columns.FirstOrDefault((GridColumn t) => t.FieldName == key2);
									GridColumn gridColumn4 = bandedGridViewEx.Columns.FirstOrDefault((GridColumn t) => t.FieldName.ToUpper() == key2.ToUpper());
									if (gridColumn3 != null)
									{
										list.Add(Function.ToString(bandedGridViewEx.GetFocusedRowCellValue(gridColumn3.FieldName)));
									}
									else if (gridColumn4 != null)
									{
										list.Add(Function.ToString(bandedGridViewEx.GetFocusedRowCellValue(gridColumn4.FieldName)));
									}
								}
							}
						}
					}
					string primaryKey2 = string.Join(",", list);
					changelog.PrimaryKey = primaryKey2;
					changelog.IsApzTable = isApzTable;
				}
			}
			if (ActiveControl is TreeEx treeEx)
			{
				changeLog = treeEx.ChangeLogType;
				bool isApzTable = treeEx.IsApzTable;
				if (CommonList.ChangeLogObjMapping.TryGetValue(changeLog, out var value5) && CommonList.ListLogInstance.TryGetValue(value5, out var value6))
				{
					LogInstance logInstance3 = value6.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
					if (logInstance3 != null)
					{
						string[] array = logInstance3.Key.Split(',');
						foreach (string key3 in array)
						{
							if (!string.IsNullOrWhiteSpace(key3))
							{
								TreeListColumn treeListColumn = treeEx.Columns.FirstOrDefault((TreeListColumn t) => t.FieldName == key3);
								TreeListColumn treeListColumn2 = treeEx.Columns.FirstOrDefault((TreeListColumn t) => t.FieldName.ToUpper() == key3.ToUpper());
								if (treeListColumn != null)
								{
									list.Add(Function.ToString(treeEx.GetFocusedRowCellValue(treeEx.Columns[treeListColumn.FieldName])));
								}
								else if (treeListColumn2 != null)
								{
									list.Add(Function.ToString(treeEx.GetFocusedRowCellValue(treeEx.Columns[treeListColumn2.FieldName])));
								}
							}
						}
						string primaryKey3 = string.Join(",", list);
						changelog.PrimaryKey = primaryKey3;
						changelog.IsApzTable = isApzTable;
					}
				}
			}
			changelog.Object = changeLog;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public virtual XtraFormEx ShowForm(string formType, params object[] parmObjects)
	{
		return this;
	}

	public virtual XtraFormEx ShowFormDialog(string formType, params object[] parmObjects)
	{
		return this;
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.ClientSize = new System.Drawing.Size(284, 261);
		base.Name = "XtraFormEx";
		base.ResumeLayout(false);
	}
}
