using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Forms.UserControls.ContextAction;

public class UcContextActionHttpRequest : XtraUserControl
{
	public Action<string> VerbsChanged;

	public Action<string> AuthTypeChanged;

	public Action<string> UriChanged;

	public Action<string> DecryptPwdChanged;

	public Action<string> BodyTypeChanged;

	public Action<string> ResponseTypeChanged;

	public Action MappingChanged;

	private IContainer components;

	private LookUpEditEx look_Verbs;

	private TextEditEx txt_Uri;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx tab_Auth;

	private XtraTabPageEx tab_Header;

	private XtraTabPageEx tab_Body;

	private TreeEx tv_Body;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeCol_Father;

	private TreeListColumnEx treeListColumnEx3;

	private RepositoryItemButtonEdit rep_btnAddChild;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx treeListColumnEx5;

	private TreeListColumnEx treeListColumnEx6;

	private TreeListColumnEx treeListColumnEx7;

	private TreeListColumnEx treeListColumnEx8;

	private LookUpEditEx look_AuthType;

	private LabelLine labelLine3;

	private GridControlEx grd_Auth;

	private GridViewEx gv_Auth;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_SourceFieldAuth;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx23;

	private GridControlEx grd_Header;

	private GridViewEx gv_Header;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEdit rep_SourceFieldHeaders;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private XtraTabPageEx tab_Query;

	private GridControlEx grd_Query;

	private GridViewEx gv_Query;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemLookUpEdit rep_SourceFieldQuery;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemLookUpEdit rep_SourceFieldBody;

	private ContextMenuStrip ctx_Query;

	private ToolStripMenuItem ctm_QueryAdd;

	private ToolStripMenuItem ctm_QueryDelete;

	private ContextMenuStrip ctx_Auth;

	private ToolStripMenuItem ctm_AuthAdd;

	private ToolStripMenuItem ctm_AuthDelete;

	private ContextMenuStrip ctx_Header;

	private ToolStripMenuItem ctm_HeaderAdd;

	private ToolStripMenuItem ctm_HeaderDelete;

	private ContextMenuStrip ctx_Body;

	private ToolStripMenuItem ctm_BodyAdd;

	private ToolStripMenuItem ctm_BodyDelete;

	private RepositoryItemLookUpEdit rep_JsonType;

	private RepositoryItemLookUpEdit rep_DataType;

	private LabelLine labelLine1;

	private LookUpEditEx look_HttpBodyType;

	private LookUpEditEx look_ResponseType;

	private LabelLine labelLine2;

	public string FormId
	{
		get
		{
			return gv_Auth.FormId;
		}
		set
		{
			GridViewEx gridViewEx = gv_Auth;
			GridViewEx gridViewEx2 = gv_Header;
			GridViewEx gridViewEx3 = gv_Query;
			string text = (tv_Body.FormId = value);
			string text3 = (gridViewEx3.FormId = text);
			string formId = (gridViewEx2.FormId = text3);
			gridViewEx.FormId = formId;
		}
	}

	public string AuthItemId
	{
		get
		{
			return gv_Auth.ItemId;
		}
		set
		{
			gv_Auth.ItemId = value;
		}
	}

	public string HeaderItemId
	{
		get
		{
			return gv_Header.ItemId;
		}
		set
		{
			gv_Header.ItemId = value;
		}
	}

	public string QueryItemId
	{
		get
		{
			return gv_Query.ItemId;
		}
		set
		{
			gv_Query.ItemId = value;
		}
	}

	public string BodyItemId
	{
		get
		{
			return tv_Body.ItemId;
		}
		set
		{
			tv_Body.ItemId = value;
		}
	}

	public string ActionFormId { get; set; }

	public string ActionItemId { get; set; }

	public int ActionEntry { get; set; }

	public DataTable MappingStructure { get; set; }

	public DataTable SourceFieldsChanged
	{
		set
		{
			try
			{
				rep_SourceFieldAuth.SetDataSource(value, "Name", "Code", emptyRow: true);
				rep_SourceFieldBody.SetDataSource(value, "Name", "Code", emptyRow: true);
				rep_SourceFieldHeaders.SetDataSource(value, "Name", "Code", emptyRow: true);
				rep_SourceFieldQuery.SetDataSource(value, "Name", "Code", emptyRow: true);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public DataTable MappingQuery
	{
		get
		{
			return grd_Query.DataSourceTable;
		}
		set
		{
			grd_Query.DataSource = value;
			grd_Query.RefreshDataSource();
		}
	}

	public DataTable MappingHeder
	{
		get
		{
			return grd_Header.DataSourceTable;
		}
		set
		{
			grd_Header.DataSource = value;
			grd_Header.RefreshDataSource();
		}
	}

	public DataTable MappingAuth
	{
		get
		{
			return grd_Auth.DataSourceTable;
		}
		set
		{
			grd_Auth.DataSource = value;
			grd_Auth.RefreshDataSource();
		}
	}

	public DataTable MappingBody
	{
		get
		{
			return tv_Body.DataSourceTable;
		}
		set
		{
			tv_Body.DataSource = value;
			tv_Body.RefreshDataSource();
		}
	}

	public UcContextActionHttpRequest()
	{
		InitializeComponent();
		try
		{
			look_Verbs.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "GET" },
				{ "1", "POST" },
				{ "2", "PUT" },
				{ "3", "PATCH" },
				{ "4", "DELETE" }
			});
			look_Verbs.EditValue = "0";
			look_AuthType.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "No Auth" },
				{ "1", "Basic Auth" },
				{ "2", "OAuth 2.0" }
			});
			look_AuthType.EditValue = "1";
			rep_JsonType.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Object" },
				{ "A", "Array" },
				{ "F", "Field" },
				{ "E", "Encrypted" },
				{ "Z", "Ziped" },
				{ "EZ", "Encrypted and Ziped" }
			});
			look_ResponseType.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "Default" },
				{ "1", "Encrypted" },
				{ "2", "Ziped" },
				{ "3", "Ziped and Encrypted" }
			});
			look_HttpBodyType.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "application/json" },
				{ "1", "application/xml" },
				{ "2", "form-data" },
				{ "3", "encode parameter" }
			});
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"Code",
						typeof(string)
					},
					{
						"Name",
						typeof(string)
					}
				}
			};
			foreach (Enums.DisplayFormatType value2 in Enum.GetValues(typeof(Enums.DisplayFormatType)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)value2;
				string value = Function.ToString(value2);
				if (value2 == Enums.DisplayFormatType.None)
				{
					value = "String";
				}
				dataRow["Name"] = Function.SpaceBeforeCapital(value);
				dataTable.Rows.Add(dataRow);
			}
			rep_DataType.DataSource = dataTable;
			rep_DataType.DisplayMember = "Name";
			rep_DataType.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UcContextActionHttpRequest_Load(object sender, EventArgs e)
	{
	}

	private void look_Verbs_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (VerbsChanged != null)
			{
				VerbsChanged(Function.ToString(look_Verbs.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_AuthType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (AuthTypeChanged != null)
			{
				AuthTypeChanged(Function.ToString(look_AuthType.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Query_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TargetField")
			{
				NewMapping(ActionEntry, grd_Query, "Q");
			}
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Auth_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TargetField")
			{
				NewMapping(ActionEntry, grd_Auth, "A");
			}
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Header_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TargetField")
			{
				NewMapping(ActionEntry, grd_Header, "H");
			}
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tree_ResultConfig_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column != null)
			{
				if (e.Column.FieldName == "Name")
				{
					int fatherId = Function.ParseInt(e.Node.GetValue("FatherNum"));
					NewBody(ActionEntry, fatherId);
				}
				if (MappingChanged != null)
				{
					MappingChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void MappingTarget(int absEntry, DataTable dtMapping)
	{
		try
		{
			DataRow[] array = (from t in dtMapping.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == absEntry && Function.ToString(t["TargetItem"]) == "Q"
				select t).ToArray();
			grd_Query.DataSource = ((array.Length != 0) ? array.CopyToDataTable() : dtMapping.Clone());
			grd_Query.RefreshDataSource();
			DataRow[] array2 = (from t in dtMapping.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == absEntry && Function.ToString(t["TargetItem"]) == "H"
				select t).ToArray();
			grd_Header.DataSource = ((array2.Length != 0) ? array2.CopyToDataTable() : dtMapping.Clone());
			grd_Header.RefreshDataSource();
			DataRow[] array3 = (from t in dtMapping.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == absEntry && Function.ToString(t["TargetItem"]) == "A"
				select t).ToArray();
			grd_Auth.DataSource = ((array3.Length != 0) ? array3.CopyToDataTable() : dtMapping.Clone());
			grd_Auth.RefreshDataSource();
			NewMapping(absEntry, grd_Query, "Q");
			NewMapping(absEntry, grd_Header, "H");
			NewMapping(absEntry, grd_Auth, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewMapping(int absEntry, GridControlEx grd, string targetItem)
	{
		try
		{
			try
			{
				DataTable dataSourceTable = grd.DataSourceTable;
				if (dataSourceTable != null && dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["TargetField"]))) == null)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["AbsEntry"] = absEntry;
					dataRow["FormId"] = ActionFormId;
					dataRow["ItemId"] = ActionItemId;
					dataRow["TargetItem"] = targetItem;
					dataRow["MappingType"] = "9";
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
					if (MappingChanged != null)
					{
						MappingChanged();
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

	public void MappingTargetBody(int absEntry, DataTable httpConfig)
	{
		try
		{
			DataRow[] array = (from t in httpConfig.AsEnumerable()
				where Function.ParseInt(t["AbsEntry"]) == absEntry
				select t).ToArray();
			if (array.Length != 0)
			{
				tv_Body.DataSource = array.CopyToDataTable();
			}
			else
			{
				tv_Body.DataSource = httpConfig.Clone();
			}
			tv_Body.DataSourceTable.Columns["ConfigID"].AutoIncrement = true;
			tv_Body.DataSourceTable.Columns["ConfigID"].AutoIncrementSeed = (tv_Body.DataSourceTable.IsNotNull() ? (tv_Body.DataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["ConfigID"])) + 1) : 1);
			tv_Body.DataSourceTable.Columns["ConfigID"].AutoIncrementStep = 1L;
			tv_Body.RefreshDataSource();
			NewBody(absEntry);
			tv_Body.ExpandAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewBody(int absEntry, int fatherId = -1)
	{
		try
		{
			if (tv_Body.DataSource is DataTable dataTable && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"])) && Function.ParseInt(t["FatherNum"]) == fatherId) == null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = absEntry;
				dataRow["FormId"] = ActionFormId;
				dataRow["ItemId"] = ActionItemId;
				dataRow["FatherNum"] = fatherId;
				dataTable.Rows.Add(dataRow);
				tv_Body.DataSource = dataTable;
				tv_Body.RefreshDataSource();
				if (MappingChanged != null)
				{
					MappingChanged();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_btnAddChild_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_Body.FocusedNode.GetValue("ConfigID"));
			NewBody(ActionEntry, fatherId);
			tv_Body.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetFocusedValue(DataRow row)
	{
		try
		{
			look_Verbs.EditValue = Function.ToString(row["HttpVerb"]);
			txt_Uri.EditValue = Function.ToString(row["HttpUri"]);
			look_AuthType.EditValue = Function.ToString(row["HttpAuthType"]);
			look_HttpBodyType.EditValue = Function.ToString(row["HttpBodyType"]);
			look_ResponseType.EditValue = Function.ToString(row["HttpResponseType"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Uri_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (UriChanged != null)
			{
				UriChanged(Function.ToString(txt_Uri.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void look_HttpBodyType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (BodyTypeChanged != null)
			{
				BodyTypeChanged(Function.ToString(look_HttpBodyType.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_QueryAdd_Click(object sender, EventArgs e)
	{
		try
		{
			NewMapping(ActionEntry, grd_Query, "Q");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_QueryDelete_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Query.DeleteSelectedRows();
			grd_Query.DataSourceTable.AcceptChanges();
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_AuthAdd_Click(object sender, EventArgs e)
	{
		try
		{
			NewMapping(ActionEntry, grd_Auth, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_AuthDelete_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Auth.DeleteSelectedRows();
			grd_Auth.DataSourceTable.AcceptChanges();
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_HeaderAdd_Click(object sender, EventArgs e)
	{
		try
		{
			NewMapping(ActionEntry, grd_Header, "H");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_HeaderDelete_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Header.DeleteSelectedRows();
			grd_Header.DataSourceTable.AcceptChanges();
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_BodyAdd_Click(object sender, EventArgs e)
	{
		try
		{
			NewBody(ActionEntry, Function.ParseInt(tv_Body.FocusedNode.GetValue("ConfigID")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ctm_BodyDelete_Click(object sender, EventArgs e)
	{
		try
		{
			int configId = Function.ParseInt(tv_Body.FocusedNode.GetValue("ConfigID"));
			RemoveBody(configId);
			if (MappingChanged != null)
			{
				MappingChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RemoveBody(int configId)
	{
		try
		{
			DataTable dataSourceTable = tv_Body.DataSourceTable;
			DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ConfigID"]) == configId);
			if (dataRow != null)
			{
				dataSourceTable.Rows.Remove(dataRow);
				dataSourceTable.AcceptChanges();
			}
			DataRow[] array = dataSourceTable.Select("FatherNum=" + configId);
			for (int num = 0; num < array.Length; num++)
			{
				int configId2 = Function.ParseInt(array[num]["ConfigID"]);
				RemoveBody(configId2);
			}
			dataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void look_ResponseType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ResponseTypeChanged?.Invoke(Function.ToString(look_ResponseType.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		this.look_Verbs = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_Uri = new Apzon.Controls.Editor.TextEditEx();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Auth = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Auth = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Auth = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_AuthAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_AuthDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Auth = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldAuth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.look_AuthType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Query = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Query = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Query = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_QueryAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_QueryDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Query = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldQuery = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Header = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Header = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Header = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_HeaderAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_HeaderDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Header = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceFieldHeaders = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Body = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_Body = new Apzon.Controls.Tree.TreeEx();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeCol_Father = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_btnAddChild = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_JsonType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_DataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx6 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_SourceFieldBody = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctx_Body = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_BodyAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_BodyDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.look_HttpBodyType = new Apzon.Controls.Editor.LookUpEditEx();
		this.look_ResponseType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.look_Verbs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Uri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.tab_Auth.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Auth).BeginInit();
		this.ctx_Auth.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Auth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldAuth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_AuthType.Properties).BeginInit();
		this.tab_Query.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Query).BeginInit();
		this.ctx_Query.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Query).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldQuery).BeginInit();
		this.tab_Header.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Header).BeginInit();
		this.ctx_Header.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldHeaders).BeginInit();
		this.tab_Body.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Body).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldBody).BeginInit();
		this.ctx_Body.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_HttpBodyType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ResponseType.Properties).BeginInit();
		base.SuspendLayout();
		this.look_Verbs.FieldName = null;
		this.look_Verbs.IsSearchControl = false;
		this.look_Verbs.Location = new System.Drawing.Point(4, 12);
		this.look_Verbs.Name = "look_Verbs";
		this.look_Verbs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Verbs.Properties.NullText = "";
		this.look_Verbs.Size = new System.Drawing.Size(100, 20);
		this.look_Verbs.TabIndex = 1;
		this.look_Verbs.EditValueChanged += new System.EventHandler(this.look_Verbs_EditValueChanged);
		this.txt_Uri.FieldName = null;
		this.txt_Uri.IsSearchControl = false;
		this.txt_Uri.Location = new System.Drawing.Point(110, 12);
		this.txt_Uri.Name = "txt_Uri";
		this.txt_Uri.Properties.NullText = "http";
		this.txt_Uri.Size = new System.Drawing.Size(468, 20);
		this.txt_Uri.TabIndex = 4;
		this.txt_Uri.EditValueChanged += new System.EventHandler(this.txt_Uri_EditValueChanged);
		this.xtraTabControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(3, 79);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.tab_Auth;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(928, 421);
		this.xtraTabControlEx1.TabIndex = 5;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tab_Query, this.tab_Auth, this.tab_Header, this.tab_Body });
		this.tab_Auth.Controls.Add(this.grd_Auth);
		this.tab_Auth.Controls.Add(this.look_AuthType);
		this.tab_Auth.Controls.Add(this.labelLine3);
		this.tab_Auth.FieldName = "";
		this.tab_Auth.IsSearchControl = false;
		this.tab_Auth.Name = "tab_Auth";
		this.tab_Auth.Size = new System.Drawing.Size(926, 437);
		this.tab_Auth.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Auth.Text = "Authorization";
		this.grd_Auth.AllowPasteExcel = false;
		this.grd_Auth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Auth.ClipboardData = null;
		this.grd_Auth.ContextMenuStrip = this.ctx_Auth;
		this.grd_Auth.Location = new System.Drawing.Point(0, 41);
		this.grd_Auth.MainView = this.gv_Auth;
		this.grd_Auth.Name = "grd_Auth";
		this.grd_Auth.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_SourceFieldAuth });
		this.grd_Auth.Size = new System.Drawing.Size(709, 391);
		this.grd_Auth.TabIndex = 2;
		this.grd_Auth.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Auth });
		this.ctx_Auth.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_AuthAdd, this.ctm_AuthDelete });
		this.ctx_Auth.Name = "ctx_Auth";
		this.ctx_Auth.Size = new System.Drawing.Size(134, 48);
		this.ctm_AuthAdd.Name = "ctm_AuthAdd";
		this.ctm_AuthAdd.Size = new System.Drawing.Size(133, 22);
		this.ctm_AuthAdd.Text = "Add Row";
		this.ctm_AuthAdd.Click += new System.EventHandler(this.ctm_AuthAdd_Click);
		this.ctm_AuthDelete.Name = "ctm_AuthDelete";
		this.ctm_AuthDelete.Size = new System.Drawing.Size(133, 22);
		this.ctm_AuthDelete.Text = "Delete Row";
		this.ctm_AuthDelete.Click += new System.EventHandler(this.ctm_AuthDelete_Click);
		this.gv_Auth.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Auth.ColumnKey = null;
		this.gv_Auth.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx23 });
		this.gv_Auth.DisableFormSetting = false;
		this.gv_Auth.DisplayException = false;
		this.gv_Auth.EditColumn = null;
		this.gv_Auth.FieldCurrency = null;
		this.gv_Auth.FieldRate = null;
		this.gv_Auth.FormId = null;
		this.gv_Auth.GridControl = this.grd_Auth;
		this.gv_Auth.IgnoreFilter = false;
		this.gv_Auth.IndexEditRow = -1;
		this.gv_Auth.IndicatorWidth = 35;
		this.gv_Auth.IsApzTable = false;
		this.gv_Auth.IsInsertNewRow = false;
		this.gv_Auth.IsLoadingData = false;
		this.gv_Auth.ItemId = "OAUTH";
		this.gv_Auth.LstColumnAllowBatch = null;
		this.gv_Auth.LstColumnNotAllowBatch = null;
		this.gv_Auth.LstColumnNotEditable = null;
		this.gv_Auth.LstColumNotSetting = null;
		this.gv_Auth.Name = "gv_Auth";
		this.gv_Auth.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Auth.OptionsView.ColumnAutoWidth = false;
		this.gv_Auth.OptionsView.ShowGroupPanel = false;
		this.gv_Auth.UseBatchMod = false;
		this.gv_Auth.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Auth_CellValueChanged);
		this.gridColumnEx1.Caption = "AbsEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Source Field Name";
		this.gridColumnEx2.ColumnEdit = this.rep_SourceFieldAuth;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "SourceField";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 121;
		this.rep_SourceFieldAuth.AutoHeight = false;
		this.rep_SourceFieldAuth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldAuth.Name = "rep_SourceFieldAuth";
		this.rep_SourceFieldAuth.NullText = "";
		this.gridColumnEx3.Caption = "TargetItem";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "TargetItem";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "Target Field Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "TargetField";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 0;
		this.gridColumnEx4.Width = 114;
		this.gridColumnEx23.Caption = "Default Value";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "DfltValue";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 2;
		this.gridColumnEx23.Width = 175;
		this.look_AuthType.FieldName = null;
		this.look_AuthType.IsSearchControl = false;
		this.look_AuthType.Location = new System.Drawing.Point(95, 12);
		this.look_AuthType.Name = "look_AuthType";
		this.look_AuthType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_AuthType.Properties.NullText = "";
		this.look_AuthType.Size = new System.Drawing.Size(100, 20);
		this.look_AuthType.TabIndex = 1;
		this.look_AuthType.EditValueChanged += new System.EventHandler(this.look_AuthType_EditValueChanged);
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(17, 15);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Type";
		this.tab_Query.Controls.Add(this.grd_Query);
		this.tab_Query.FieldName = null;
		this.tab_Query.IsSearchControl = false;
		this.tab_Query.Name = "tab_Query";
		this.tab_Query.Size = new System.Drawing.Size(926, 437);
		this.tab_Query.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Query.Text = "Queries";
		this.grd_Query.AllowPasteExcel = false;
		this.grd_Query.ClipboardData = null;
		this.grd_Query.ContextMenuStrip = this.ctx_Query;
		this.grd_Query.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Query.Location = new System.Drawing.Point(0, 0);
		this.grd_Query.MainView = this.gv_Query;
		this.grd_Query.Name = "grd_Query";
		this.grd_Query.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_SourceFieldQuery });
		this.grd_Query.Size = new System.Drawing.Size(926, 437);
		this.grd_Query.TabIndex = 3;
		this.grd_Query.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Query });
		this.ctx_Query.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_QueryAdd, this.ctm_QueryDelete });
		this.ctx_Query.Name = "ctx_Query";
		this.ctx_Query.Size = new System.Drawing.Size(134, 48);
		this.ctm_QueryAdd.Name = "ctm_QueryAdd";
		this.ctm_QueryAdd.Size = new System.Drawing.Size(133, 22);
		this.ctm_QueryAdd.Text = "Add Row";
		this.ctm_QueryAdd.Click += new System.EventHandler(this.ctm_QueryAdd_Click);
		this.ctm_QueryDelete.Name = "ctm_QueryDelete";
		this.ctm_QueryDelete.Size = new System.Drawing.Size(133, 22);
		this.ctm_QueryDelete.Text = "Delete Row";
		this.ctm_QueryDelete.Click += new System.EventHandler(this.ctm_QueryDelete_Click);
		this.gv_Query.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Query.ColumnKey = null;
		this.gv_Query.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14 });
		this.gv_Query.DisableFormSetting = false;
		this.gv_Query.DisplayException = false;
		this.gv_Query.EditColumn = null;
		this.gv_Query.FieldCurrency = null;
		this.gv_Query.FieldRate = null;
		this.gv_Query.FormId = null;
		this.gv_Query.GridControl = this.grd_Query;
		this.gv_Query.IgnoreFilter = false;
		this.gv_Query.IndexEditRow = -1;
		this.gv_Query.IndicatorWidth = 35;
		this.gv_Query.IsApzTable = false;
		this.gv_Query.IsInsertNewRow = false;
		this.gv_Query.IsLoadingData = false;
		this.gv_Query.ItemId = "OQUERY";
		this.gv_Query.LstColumnAllowBatch = null;
		this.gv_Query.LstColumnNotAllowBatch = null;
		this.gv_Query.LstColumnNotEditable = null;
		this.gv_Query.LstColumNotSetting = null;
		this.gv_Query.Name = "gv_Query";
		this.gv_Query.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Query.OptionsView.ColumnAutoWidth = false;
		this.gv_Query.OptionsView.ShowGroupPanel = false;
		this.gv_Query.UseBatchMod = false;
		this.gv_Query.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Query_CellValueChanged);
		this.gridColumnEx10.Caption = "AbsEntry";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "AbsEntry";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "Source Field Name";
		this.gridColumnEx11.ColumnEdit = this.rep_SourceFieldQuery;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "SourceField";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 1;
		this.gridColumnEx11.Width = 121;
		this.rep_SourceFieldQuery.AutoHeight = false;
		this.rep_SourceFieldQuery.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldQuery.Name = "rep_SourceFieldQuery";
		this.rep_SourceFieldQuery.NullText = "";
		this.gridColumnEx12.Caption = "TargetItem";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "TargetItem";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx13.Caption = "Target Field Name";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "TargetField";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 0;
		this.gridColumnEx13.Width = 114;
		this.gridColumnEx14.Caption = "Default Value";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "DfltValue";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 2;
		this.gridColumnEx14.Width = 175;
		this.tab_Header.Controls.Add(this.grd_Header);
		this.tab_Header.FieldName = null;
		this.tab_Header.IsSearchControl = false;
		this.tab_Header.Name = "tab_Header";
		this.tab_Header.Size = new System.Drawing.Size(926, 396);
		this.tab_Header.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Header.Text = "Headers";
		this.grd_Header.AllowPasteExcel = false;
		this.grd_Header.ClipboardData = null;
		this.grd_Header.ContextMenuStrip = this.ctx_Header;
		this.grd_Header.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Header.Location = new System.Drawing.Point(0, 0);
		this.grd_Header.MainView = this.gv_Header;
		this.grd_Header.Name = "grd_Header";
		this.grd_Header.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_SourceFieldHeaders });
		this.grd_Header.Size = new System.Drawing.Size(926, 396);
		this.grd_Header.TabIndex = 3;
		this.grd_Header.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Header });
		this.ctx_Header.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_HeaderAdd, this.ctm_HeaderDelete });
		this.ctx_Header.Name = "ctx_Auth";
		this.ctx_Header.Size = new System.Drawing.Size(134, 48);
		this.ctm_HeaderAdd.Name = "ctm_HeaderAdd";
		this.ctm_HeaderAdd.Size = new System.Drawing.Size(133, 22);
		this.ctm_HeaderAdd.Text = "Add Row";
		this.ctm_HeaderAdd.Click += new System.EventHandler(this.ctm_HeaderAdd_Click);
		this.ctm_HeaderDelete.Name = "ctm_HeaderDelete";
		this.ctm_HeaderDelete.Size = new System.Drawing.Size(133, 22);
		this.ctm_HeaderDelete.Text = "Delete Row";
		this.ctm_HeaderDelete.Click += new System.EventHandler(this.ctm_HeaderDelete_Click);
		this.gv_Header.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Header.ColumnKey = null;
		this.gv_Header.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Header.DisableFormSetting = false;
		this.gv_Header.DisplayException = false;
		this.gv_Header.EditColumn = null;
		this.gv_Header.FieldCurrency = null;
		this.gv_Header.FieldRate = null;
		this.gv_Header.FormId = null;
		this.gv_Header.GridControl = this.grd_Header;
		this.gv_Header.IgnoreFilter = false;
		this.gv_Header.IndexEditRow = -1;
		this.gv_Header.IndicatorWidth = 35;
		this.gv_Header.IsApzTable = false;
		this.gv_Header.IsInsertNewRow = false;
		this.gv_Header.IsLoadingData = false;
		this.gv_Header.ItemId = "OHEADER";
		this.gv_Header.LstColumnAllowBatch = null;
		this.gv_Header.LstColumnNotAllowBatch = null;
		this.gv_Header.LstColumnNotEditable = null;
		this.gv_Header.LstColumNotSetting = null;
		this.gv_Header.Name = "gv_Header";
		this.gv_Header.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Header.OptionsView.ColumnAutoWidth = false;
		this.gv_Header.OptionsView.ShowGroupPanel = false;
		this.gv_Header.UseBatchMod = false;
		this.gv_Header.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Header_CellValueChanged);
		this.gridColumnEx5.Caption = "AbsEntry";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "AbsEntry";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx6.Caption = "Source Field Name";
		this.gridColumnEx6.ColumnEdit = this.rep_SourceFieldHeaders;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SourceField";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 121;
		this.rep_SourceFieldHeaders.AutoHeight = false;
		this.rep_SourceFieldHeaders.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldHeaders.Name = "rep_SourceFieldHeaders";
		this.rep_SourceFieldHeaders.NullText = "";
		this.gridColumnEx7.Caption = "TargetItem";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "TargetItem";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "Target Field Name";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "TargetField";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 0;
		this.gridColumnEx8.Width = 114;
		this.gridColumnEx9.Caption = "Default Value";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "DfltValue";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 175;
		this.tab_Body.Controls.Add(this.tv_Body);
		this.tab_Body.FieldName = null;
		this.tab_Body.IsSearchControl = false;
		this.tab_Body.Name = "tab_Body";
		this.tab_Body.Size = new System.Drawing.Size(926, 437);
		this.tab_Body.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Body.Text = "Body";
		this.tv_Body.AllowDrop = true;
		this.tv_Body.AllowPasteExcel = true;
		this.tv_Body.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Body.ColumnKey = null;
		this.tv_Body.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[8] { this.treeListColumnEx1, this.treeCol_Father, this.treeListColumnEx3, this.treeListColumnEx4, this.treeListColumnEx5, this.treeListColumnEx6, this.treeListColumnEx7, this.treeListColumnEx8 });
		this.tv_Body.ContextMenuStrip = this.ctx_Body;
		this.tv_Body.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Body.DisableFormSetting = false;
		this.tv_Body.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Body.EditColumn = null;
		this.tv_Body.FieldCurrency = null;
		this.tv_Body.FieldRate = null;
		treeListFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		treeListFormatRule.Rule = formatConditionRuleIconSet;
		this.tv_Body.FormatRules.Add(treeListFormatRule);
		this.tv_Body.FormId = null;
		this.tv_Body.IndexEditRow = 0;
		this.tv_Body.IndicatorWidth = 40;
		this.tv_Body.IsApzTable = false;
		this.tv_Body.IsLoadingData = false;
		this.tv_Body.ItemId = "OBODY";
		this.tv_Body.KeyFieldName = "ConfigID";
		this.tv_Body.Location = new System.Drawing.Point(0, 0);
		this.tv_Body.LstColumnNotEditable = null;
		this.tv_Body.LstColumNotSetting = null;
		this.tv_Body.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tv_Body.Name = "tv_Body";
		this.tv_Body.OptionsBehavior.AutoChangeParent = false;
		this.tv_Body.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tv_Body.OptionsBehavior.PopulateServiceColumns = true;
		this.tv_Body.OptionsBehavior.ShowToolTips = false;
		this.tv_Body.OptionsBehavior.SmartMouseHover = false;
		this.tv_Body.OptionsDragAndDrop.AcceptOuterNodes = true;
		this.tv_Body.OptionsDragAndDrop.CanCloneNodesOnDrop = true;
		this.tv_Body.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
		this.tv_Body.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.tv_Body.OptionsSelection.KeepSelectedOnClick = false;
		this.tv_Body.OptionsSelection.SelectNodesOnRightClick = true;
		this.tv_Body.OptionsView.AutoWidth = false;
		this.tv_Body.OptionsView.EnableAppearanceEvenRow = true;
		this.tv_Body.OptionsView.EnableAppearanceOddRow = true;
		this.tv_Body.ParentFieldName = "FatherNum";
		this.tv_Body.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_btnAddChild, this.rep_SourceFieldBody, this.rep_JsonType, this.rep_DataType });
		this.tv_Body.Size = new System.Drawing.Size(926, 437);
		this.tv_Body.TabIndex = 2;
		this.tv_Body.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tree_ResultConfig_CellValueChanged);
		this.treeListColumnEx1.Caption = "ID";
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "ConfigID";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeCol_Father.Caption = "Father Num";
		this.treeCol_Father.CurrencyDateField = null;
		this.treeCol_Father.CurrencyField = null;
		this.treeCol_Father.CurrencyFixed = null;
		this.treeCol_Father.CurrencyFixedRate = 0.0;
		this.treeCol_Father.CurrencyRateField = null;
		this.treeCol_Father.FieldName = "FatherNum";
		this.treeCol_Father.Name = "treeCol_Father";
		this.treeListColumnEx3.Caption = "Name";
		this.treeListColumnEx3.ColumnEdit = this.rep_btnAddChild;
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "Name";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 0;
		this.treeListColumnEx3.Width = 150;
		this.rep_btnAddChild.AutoHeight = false;
		this.rep_btnAddChild.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.rep_btnAddChild.Name = "rep_btnAddChild";
		this.rep_btnAddChild.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnAddChild_ButtonClick);
		this.treeListColumnEx4.Caption = "Type";
		this.treeListColumnEx4.ColumnEdit = this.rep_JsonType;
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = null;
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.FieldName = "JsonType";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 1;
		this.treeListColumnEx4.Width = 102;
		this.rep_JsonType.AutoHeight = false;
		this.rep_JsonType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_JsonType.Name = "rep_JsonType";
		this.rep_JsonType.NullText = "";
		this.treeListColumnEx5.Caption = "Data Type";
		this.treeListColumnEx5.ColumnEdit = this.rep_DataType;
		this.treeListColumnEx5.CurrencyDateField = null;
		this.treeListColumnEx5.CurrencyField = null;
		this.treeListColumnEx5.CurrencyFixed = null;
		this.treeListColumnEx5.CurrencyFixedRate = 0.0;
		this.treeListColumnEx5.CurrencyRateField = null;
		this.treeListColumnEx5.FieldName = "DataType";
		this.treeListColumnEx5.Name = "treeListColumnEx5";
		this.treeListColumnEx5.Visible = true;
		this.treeListColumnEx5.VisibleIndex = 2;
		this.treeListColumnEx5.Width = 121;
		this.rep_DataType.AutoHeight = false;
		this.rep_DataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DataType.Name = "rep_DataType";
		this.rep_DataType.NullText = "";
		this.treeListColumnEx6.Caption = "Value";
		this.treeListColumnEx6.ColumnEdit = this.rep_SourceFieldBody;
		this.treeListColumnEx6.CurrencyDateField = null;
		this.treeListColumnEx6.CurrencyField = null;
		this.treeListColumnEx6.CurrencyFixed = null;
		this.treeListColumnEx6.CurrencyFixedRate = 0.0;
		this.treeListColumnEx6.CurrencyRateField = null;
		this.treeListColumnEx6.FieldName = "Value";
		this.treeListColumnEx6.Name = "treeListColumnEx6";
		this.treeListColumnEx6.Visible = true;
		this.treeListColumnEx6.VisibleIndex = 3;
		this.treeListColumnEx6.Width = 185;
		this.rep_SourceFieldBody.AutoHeight = false;
		this.rep_SourceFieldBody.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceFieldBody.Name = "rep_SourceFieldBody";
		this.rep_SourceFieldBody.NullText = "";
		this.treeListColumnEx7.Caption = "Default Value";
		this.treeListColumnEx7.CurrencyDateField = null;
		this.treeListColumnEx7.CurrencyField = null;
		this.treeListColumnEx7.CurrencyFixed = null;
		this.treeListColumnEx7.CurrencyFixedRate = 0.0;
		this.treeListColumnEx7.CurrencyRateField = null;
		this.treeListColumnEx7.FieldName = "DfltValue";
		this.treeListColumnEx7.Name = "treeListColumnEx7";
		this.treeListColumnEx7.Visible = true;
		this.treeListColumnEx7.VisibleIndex = 4;
		this.treeListColumnEx7.Width = 172;
		this.treeListColumnEx8.Caption = "Description";
		this.treeListColumnEx8.CurrencyDateField = null;
		this.treeListColumnEx8.CurrencyField = null;
		this.treeListColumnEx8.CurrencyFixed = null;
		this.treeListColumnEx8.CurrencyFixedRate = 0.0;
		this.treeListColumnEx8.CurrencyRateField = null;
		this.treeListColumnEx8.FieldName = "Dscription";
		this.treeListColumnEx8.Name = "treeListColumnEx8";
		this.treeListColumnEx8.Visible = true;
		this.treeListColumnEx8.VisibleIndex = 5;
		this.treeListColumnEx8.Width = 250;
		this.ctx_Body.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_BodyAdd, this.ctm_BodyDelete });
		this.ctx_Body.Name = "ctx_Auth";
		this.ctx_Body.Size = new System.Drawing.Size(134, 48);
		this.ctm_BodyAdd.Name = "ctm_BodyAdd";
		this.ctm_BodyAdd.Size = new System.Drawing.Size(133, 22);
		this.ctm_BodyAdd.Text = "Add Row";
		this.ctm_BodyAdd.Click += new System.EventHandler(this.ctm_BodyAdd_Click);
		this.ctm_BodyDelete.Name = "ctm_BodyDelete";
		this.ctm_BodyDelete.Size = new System.Drawing.Size(133, 22);
		this.ctm_BodyDelete.Text = "Delete Row";
		this.ctm_BodyDelete.Click += new System.EventHandler(this.ctm_BodyDelete_Click);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(593, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Body Type";
		this.look_HttpBodyType.FieldName = null;
		this.look_HttpBodyType.IsSearchControl = false;
		this.look_HttpBodyType.Location = new System.Drawing.Point(672, 12);
		this.look_HttpBodyType.Name = "look_HttpBodyType";
		this.look_HttpBodyType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_HttpBodyType.Properties.NullText = "";
		this.look_HttpBodyType.Size = new System.Drawing.Size(125, 20);
		this.look_HttpBodyType.TabIndex = 7;
		this.look_HttpBodyType.EditValueChanged += new System.EventHandler(this.look_HttpBodyType_EditValueChanged);
		this.look_ResponseType.FieldName = null;
		this.look_ResponseType.IsSearchControl = false;
		this.look_ResponseType.Location = new System.Drawing.Point(110, 35);
		this.look_ResponseType.Name = "look_ResponseType";
		this.look_ResponseType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ResponseType.Properties.NullText = "";
		this.look_ResponseType.Size = new System.Drawing.Size(125, 20);
		this.look_ResponseType.TabIndex = 9;
		this.look_ResponseType.EditValueChanged += new System.EventHandler(this.look_ResponseType_EditValueChanged);
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 38);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(109, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 8;
		this.labelLine2.Utext = "Reponse Type";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.look_ResponseType);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.look_HttpBodyType);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.xtraTabControlEx1);
		base.Controls.Add(this.txt_Uri);
		base.Controls.Add(this.look_Verbs);
		base.Name = "UcContextActionHttpRequest";
		base.Size = new System.Drawing.Size(934, 503);
		base.Load += new System.EventHandler(this.UcContextActionHttpRequest_Load);
		((System.ComponentModel.ISupportInitialize)this.look_Verbs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Uri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.tab_Auth.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Auth).EndInit();
		this.ctx_Auth.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Auth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldAuth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_AuthType.Properties).EndInit();
		this.tab_Query.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Query).EndInit();
		this.ctx_Query.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Query).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldQuery).EndInit();
		this.tab_Header.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Header).EndInit();
		this.ctx_Header.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldHeaders).EndInit();
		this.tab_Body.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Body).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceFieldBody).EndInit();
		this.ctx_Body.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_HttpBodyType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ResponseType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
