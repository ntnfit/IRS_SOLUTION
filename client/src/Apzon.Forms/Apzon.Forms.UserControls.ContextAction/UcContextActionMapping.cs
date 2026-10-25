using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;

namespace Apzon.Forms.UserControls.ContextAction;

public class UcContextActionMapping : XtraUserControl
{
	public Action<string> ObjectIdValueChanged;

	public Action CellValueChanged;

	public Action<string> ShowFormValueChanged;

	public Action<string> ObjectTypeValueChanged;

	private List<ContextActionMappingTab> _tabs;

	private IContainer components;

	private LookUpEditEx look_Entry;

	private LabelLine lb_Entry;

	private XtraTabControlEx tabControl_Mapping;

	private XtraTabPageEx xtraTabPage2;

	private CheckEditEx chk_ShowForm;

	private UcContextActionMappingGridView ucContextActionMappingGridView1;

	private LookUpEditEx look_ObjType;

	private LabelLine lb_ObjType;

	public string ActionFormId { get; set; }

	public string ActionItemId { get; set; }

	public bool ShowDocForm
	{
		get
		{
			return Function.ParseBool(chk_ShowForm.Checked);
		}
		set
		{
			chk_ShowForm.Checked = value;
		}
	}

	public bool ShowDocFormVisible
	{
		get
		{
			return chk_ShowForm.Visible;
		}
		set
		{
			chk_ShowForm.Visible = value;
		}
	}

	public bool ObjectTypeVisible
	{
		get
		{
			return lb_ObjType.Visible;
		}
		set
		{
			lb_ObjType.Visible = value;
			look_ObjType.Visible = value;
		}
	}

	public string ObjectType
	{
		get
		{
			return Function.ToString(look_ObjType.EditValue);
		}
		set
		{
			look_ObjType.EditValue = value;
		}
	}

	public string DocEntry
	{
		get
		{
			return Function.ToString(look_Entry.EditValue);
		}
		set
		{
			look_Entry.EditValue = value;
		}
	}

	public string ObjectIdCaption
	{
		get
		{
			return Function.ToString(lb_Entry.Utext);
		}
		set
		{
			lb_Entry.Utext = value;
		}
	}

	public bool DocEntryVisible
	{
		get
		{
			return lb_Entry.Visible;
		}
		set
		{
			lb_Entry.Visible = value;
			look_Entry.Visible = value;
		}
	}

	public bool TabMappingVisible
	{
		get
		{
			return tabControl_Mapping.Visible;
		}
		set
		{
			tabControl_Mapping.Visible = value;
		}
	}

	public List<ContextActionMappingTab> Tabs
	{
		get
		{
			return _tabs;
		}
		set
		{
			LoadTabs(value);
		}
	}

	private void LoadTabs(List<ContextActionMappingTab> value)
	{
		try
		{
			_tabs = value;
			if (value == null || value.Count <= 0)
			{
				return;
			}
			tabControl_Mapping.TabPages.Clear();
			foreach (ContextActionMappingTab page in value)
			{
				XtraTabPageEx xtraTabPageEx = new XtraTabPageEx();
				xtraTabPageEx.Size = new Size(850, 452);
				xtraTabPageEx.SuspendLayout();
				UcContextActionMappingGridView ucContextActionMappingGridView = new UcContextActionMappingGridView();
				ucContextActionMappingGridView.CellValueChanged = (Action)Delegate.Combine(ucContextActionMappingGridView.CellValueChanged, new Action(gv_CellValueChanged));
				ucContextActionMappingGridView.Location = new Point(0, 0);
				ucContextActionMappingGridView.Size = new Size(850, 452);
				ucContextActionMappingGridView.MappingType = page.MappingType;
				ucContextActionMappingGridView.TargetItem = page.TargetItem;
				ucContextActionMappingGridView.HasSumaryType = page.HasSumaryType;
				ucContextActionMappingGridView.Dock = DockStyle.Fill;
				xtraTabPageEx.Controls.Add(ucContextActionMappingGridView);
				page.GridView = ucContextActionMappingGridView;
				xtraTabPageEx.FieldName = page.Name;
				xtraTabPageEx.SourceType = Enums.DataSourceType.TittleValue;
				xtraTabPageEx.Text = page.Caption;
				tabControl_Mapping.TabPages.Add(xtraTabPageEx);
				xtraTabPageEx.ResumeLayout(performLayout: false);
				ComponentResourceManager componentResourceManager = null;
				bool flag = false;
				if (Type.GetType("Apzon.Commons.Properties." + page.ResourceName + ", Apzon.Commons") != null)
				{
					componentResourceManager = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties." + page.ResourceName + ", Apzon.Commons"));
					flag = true;
				}
				page.MappingSource = new DataTable();
				page.MappingSource.Columns.Add("Code", typeof(string));
				page.MappingSource.Columns.Add("Name", typeof(string));
				if (page.FixedRow != null)
				{
					foreach (ContextActionMappingFixedRow item in page.FixedRow)
					{
						string text = (flag ? componentResourceManager.GetString(item.ResourceKey, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) : null);
						DataRow dataRow = page.MappingSource.NewRow();
						dataRow["Code"] = item.Code;
						dataRow["Name"] = text ?? item.DlftCaption ?? item.Code;
						page.MappingSource.Rows.Add(dataRow);
					}
				}
				foreach (DataColumn item2 in from DataColumn t in page.Structure.Columns
					where !string.IsNullOrEmpty(t.ColumnName) && !page.IgnoreFields.Contains(t.ColumnName)
					select t)
				{
					string text2 = (flag ? componentResourceManager.GetString(item2.ColumnName, CultureInfo.CreateSpecificCulture(GlobalData.CURRENTLANGUAGE)) : null);
					DataRow dataRow2 = page.MappingSource.NewRow();
					dataRow2["Code"] = item2.ColumnName;
					dataRow2["Name"] = text2 ?? item2.Caption;
					page.MappingSource.Rows.Add(dataRow2);
				}
				ucContextActionMappingGridView.SetTargetField(page.MappingSource);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CellValueChanged()
	{
		try
		{
			CellValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void LoadObjectType(Dictionary<string, string> dic)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		foreach (KeyValuePair<string, string> item in dic)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = item.Key;
			dataRow["Name"] = item.Value;
			dataTable.Rows.Add(dataRow);
		}
		look_ObjType.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
	}

	public void MappingTarget(int absEntry, DataTable dtMapping)
	{
		try
		{
			foreach (ContextActionMappingTab tab in _tabs)
			{
				DataRow[] array = dtMapping.Select($"AbsEntry={absEntry} AND MappingType='{tab.MappingType}' AND TargetItem='{tab.TargetItem}'");
				DataTable ret = dtMapping.Clone();
				DataTable dataTable = dtMapping.Clone();
				if (array.Length != 0)
				{
					dataTable = array.CopyToDataTable();
				}
				DataRow[] array2 = (from t0 in tab.MappingSource.AsEnumerable()
					join t1 in dataTable.AsEnumerable() on Function.ToString(t0["Code"]) equals Function.ToString(t1["TargetField"]) into grp
					from item in grp.DefaultIfEmpty()
					select ret.LoadDataRow(new object[9]
					{
						absEntry,
						ActionFormId,
						ActionItemId,
						tab.MappingType,
						tab.TargetItem,
						Function.ToString(t0["Code"]),
						(item != null) ? Function.ToString(item["SourceField"]) : "",
						(item != null) ? Function.ToString(item["DfltValue"]) : null,
						(item != null) ? Function.ToString(item["SumType"]) : ""
					}, fAcceptChanges: false)).ToArray();
				tab.GridView.DataSource = ((array2.Length != 0) ? array2.CopyToDataTable() : dataTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ChangeSourceField(DataTable source)
	{
		try
		{
			foreach (ContextActionMappingTab tab in _tabs)
			{
				tab.GridView.ChangeSourceField(source);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		look_Entry.SetDataSource(source, "Name", "Code", emptyRow: true);
	}

	public UcContextActionMapping()
	{
		InitializeComponent();
	}

	private void UcContextActionMapping_Load(object sender, EventArgs e)
	{
	}

	private void look_Entry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ObjectIdValueChanged(Function.ToString(look_Entry.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_ShowForm_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ShowFormValueChanged(Function.ToString(chk_ShowForm.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void InsertDataMapping(DataTable dtMapping)
	{
		if (dtMapping == null || dtMapping.Columns.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.CouldNotGetDataMapping, "");
			return;
		}
		foreach (ContextActionMappingTab tab in _tabs)
		{
			DataTable dataSource = tab.GridView.DataSource;
			if (dataSource == null || dataSource.Rows.Count == 0)
			{
				return;
			}
			DataRow[] array = dtMapping.Select("AbsEntry=" + Function.ParseInt(dataSource.Rows[0]["AbsEntry"]) + " AND MappingType='" + tab.MappingType + "' AND TargetItem='" + tab.TargetItem + "'");
			for (int i = 0; i < array.Length; i++)
			{
				dtMapping.Rows.Remove(array[i]);
			}
			for (int j = 0; j < dataSource.Rows.Count; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataSource.Rows[j]["SourceField"])) || !string.IsNullOrEmpty(Function.ToString(dataSource.Rows[j]["DfltValue"])))
				{
					DataRow dataRow = dtMapping.NewRow();
					dataRow.ItemArray = dataSource.Rows[j].ItemArray;
					dtMapping.Rows.Add(dataRow);
				}
			}
		}
		dtMapping.AcceptChanges();
	}

	private void look_ObjType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ObjectTypeValueChanged(Function.ToString(look_ObjType.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.ContextAction.UcContextActionMapping));
		this.look_Entry = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_Entry = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_Mapping = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucContextActionMappingGridView1 = new Apzon.Forms.UserControls.ContextAction.UcContextActionMappingGridView();
		this.chk_ShowForm = new Apzon.Controls.Editor.CheckEditEx();
		this.look_ObjType = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_ObjType = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.look_Entry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Mapping).BeginInit();
		this.tabControl_Mapping.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_ShowForm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_ObjType.Properties).BeginInit();
		base.SuspendLayout();
		this.look_Entry.Location = new System.Drawing.Point(253, 3);
		this.look_Entry.Name = "look_Entry";
		this.look_Entry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Entry.Properties.NullText = "";
		this.look_Entry.Size = new System.Drawing.Size(141, 20);
		this.look_Entry.TabIndex = 11;
		this.look_Entry.EditValueChanged += new System.EventHandler(this.look_Entry_EditValueChanged);
		this.lb_Entry.FieldName = null;
		this.lb_Entry.HideWhenEmpty = false;
		this.lb_Entry.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Entry.Location = new System.Drawing.Point(122, 6);
		this.lb_Entry.Name = "lb_Entry";
		this.lb_Entry.ShowUnderLine = true;
		this.lb_Entry.Size = new System.Drawing.Size(132, 20);
		this.lb_Entry.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Entry.TabIndex = 10;
		this.lb_Entry.Utext = "Doc. Entry";
		this.tabControl_Mapping.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_Mapping.IsSearchControl = false;
		this.tabControl_Mapping.Location = new System.Drawing.Point(2, 28);
		this.tabControl_Mapping.Name = "tabControl_Mapping";
		this.tabControl_Mapping.SelectedTabPage = this.xtraTabPage2;
		this.tabControl_Mapping.Size = new System.Drawing.Size(856, 480);
		this.tabControl_Mapping.TabIndex = 9;
		this.tabControl_Mapping.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage2 });
		this.xtraTabPage2.Controls.Add(this.ucContextActionMappingGridView1);
		this.xtraTabPage2.FieldName = "xtraTabPage2";
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(854, 455);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Header";
		this.ucContextActionMappingGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucContextActionMappingGridView1.FormId = null;
		this.ucContextActionMappingGridView1.HasSumaryType = false;
		this.ucContextActionMappingGridView1.ItemId = "OHEADER";
		this.ucContextActionMappingGridView1.Location = new System.Drawing.Point(0, 0);
		this.ucContextActionMappingGridView1.MappingType = null;
		this.ucContextActionMappingGridView1.Name = "ucContextActionMappingGridView1";
		this.ucContextActionMappingGridView1.Size = new System.Drawing.Size(854, 455);
		this.ucContextActionMappingGridView1.TabIndex = 0;
		this.ucContextActionMappingGridView1.TargetItem = null;
		this.chk_ShowForm.EditValue = "Y";
		this.chk_ShowForm.FieldName = null;
		this.chk_ShowForm.IsSearchControl = false;
		this.chk_ShowForm.Location = new System.Drawing.Point(12, 3);
		this.chk_ShowForm.Name = "chk_ShowForm";
		this.chk_ShowForm.Properties.Caption = "Show Window";
		this.chk_ShowForm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.chk_ShowForm.Properties.ValueChecked = "Y";
		this.chk_ShowForm.Properties.ValueGrayed = "N";
		this.chk_ShowForm.Properties.ValueUnchecked = "N";
		this.chk_ShowForm.Size = new System.Drawing.Size(104, 20);
		this.chk_ShowForm.TabIndex = 8;
		this.chk_ShowForm.CheckedChanged += new System.EventHandler(this.chk_ShowForm_CheckedChanged);
		this.look_ObjType.Location = new System.Drawing.Point(531, 0);
		this.look_ObjType.Name = "look_ObjType";
		this.look_ObjType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_ObjType.Properties.NullText = "";
		this.look_ObjType.Size = new System.Drawing.Size(141, 20);
		this.look_ObjType.TabIndex = 13;
		this.look_ObjType.Visible = false;
		this.look_ObjType.EditValueChanged += new System.EventHandler(this.look_ObjType_EditValueChanged);
		this.lb_ObjType.FieldName = null;
		this.lb_ObjType.HideWhenEmpty = false;
		this.lb_ObjType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ObjType.Location = new System.Drawing.Point(400, 3);
		this.lb_ObjType.Name = "lb_ObjType";
		this.lb_ObjType.ShowUnderLine = true;
		this.lb_ObjType.Size = new System.Drawing.Size(132, 20);
		this.lb_ObjType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ObjType.TabIndex = 12;
		this.lb_ObjType.Utext = "Object type";
		this.lb_ObjType.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.look_ObjType);
		base.Controls.Add(this.lb_ObjType);
		base.Controls.Add(this.look_Entry);
		base.Controls.Add(this.lb_Entry);
		base.Controls.Add(this.tabControl_Mapping);
		base.Controls.Add(this.chk_ShowForm);
		base.Name = "UcContextActionMapping";
		base.Size = new System.Drawing.Size(858, 508);
		base.Load += new System.EventHandler(this.UcContextActionMapping_Load);
		((System.ComponentModel.ISupportInitialize)this.look_Entry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Mapping).EndInit();
		this.tabControl_Mapping.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_ShowForm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_ObjType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
