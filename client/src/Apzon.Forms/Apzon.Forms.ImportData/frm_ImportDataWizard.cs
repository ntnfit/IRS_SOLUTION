using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraWizard;

namespace Apzon.Forms.ImportData;

public class frm_ImportDataWizard : XtraFormEx
{
	private List<ImportObjectImpls> _importObjects = new List<ImportObjectImpls>();

	private ImportObjectImpls _currentObject = new ImportObjectImpls();

	private Enums.ImportActionEnum _currentAction;

	private List<ImportDataTableObject> _lstImportDataTableObject = new List<ImportDataTableObject>();

	private Enums.ImportErrorHandlingEnum _errorHandling = Enums.ImportErrorHandlingEnum.IgnoreUpTo;

	private int _ignoreUpTo = 10;

	private SerializableAppearanceObject _serializableAppearanceObjectNew1 = new SerializableAppearanceObject();

	private IContainer components;

	private WizardControlEx wiz_ImportData;

	private DevExpress.XtraWizard.WizardPage wzp_SelectOperationType;

	private RadioGroupEx rdg_OperationType;

	private DevExpress.XtraWizard.WizardPage wzp_SelectObject;

	private DevExpress.XtraWizard.WizardPage wzp_SelectSource;

	private TreeList tl_Object;

	private LabelLine lbl_Object;

	private DevExpress.XtraWizard.WizardPage wzp_Mapping;

	private GridControlEx grd_MappingObject;

	private GridViewEx gv_MappingObject;

	private GridColumnEx gridColumn3;

	private XtraTabControlEx xTab_MapField;

	private XtraTabPageEx xTPage_MappingRule;

	private GridControlEx grd_MappingRule;

	private GridViewEx gv_MappingRule;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private XtraTabPageEx xTPage_SourceData;

	private GridControlEx grd_SourceData;

	private GridViewEx gv_SourceData;

	private Panel panel2;

	private Label label4;

	private Label label5;

	private Panel panel3;

	private Label label6;

	private Label label7;

	private GridControlEx grd_File;

	private GridViewEx gv_File;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private LookUpEdit luk_FileType;

	private LabelLine labelLine1;

	private Panel panel4;

	private Label label8;

	private Label label9;

	private Panel panel5;

	private Label label1;

	private Label label10;

	private TreeListColumnEx treeListColumn1;

	private RepositoryItemLookUpEdit rep_luk_TargetField;

	private GridColumnEx gridColumn4;

	private RepositoryItemButtonEdit rep_bte_FileObject;

	private RepositoryItemButtonEdit rep_bte_MappingObjectName;

	private DevExpress.XtraWizard.WizardPage wzp_DefineErrorHandling;

	private Panel panel6;

	private Label label12;

	private Label label13;

	private DevExpress.XtraWizard.WizardPage wzp_StartImport;

	private Panel panel1;

	private Label label2;

	private Label label3;

	private DevExpress.XtraWizard.WizardPage wzp_Finish;

	private Panel panel7;

	private Label label11;

	private Label label14;

	private CalcEditEx calc_IgnoreUpto;

	private RadioGroupEx rdg_ErrorHandling;

	private TextBoxEx txt_Summary;

	private ImageList imageList1;

	public frm_ImportDataWizard()
	{
		InitializeComponent();
	}

	private void frm_ImportDataWizard_Load(object sender, EventArgs e)
	{
		try
		{
			_importObjects = CreateImportObject();
			LoadTreeListObject();
			LoadFileType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gv_MappingObject_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int code = Function.ParseInt(gv_MappingObject.GetFocusedRowCellValue("Code"));
			ImportDataTableObject importDataTableObject = _lstImportDataTableObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == (Enums.ImportObjectEnum)code);
			if (importDataTableObject != null)
			{
				grd_MappingRule.DataSource = importDataTableObject.MappingData;
				grd_MappingRule.RefreshDataSource();
				gv_SourceData.Columns.Clear();
				grd_SourceData.DataSource = importDataTableObject.ImportData;
				grd_SourceData.RefreshDataSource();
				gv_SourceData.PopulateColumns();
				rep_luk_TargetField.DataSource = importDataTableObject.TableFields;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void wiz_ImportData_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		try
		{
			if (e.Page is DevExpress.XtraWizard.WizardPage)
			{
				if (e.Page.Name == "wzp_Finish")
				{
					e.PrevButton.Visible = false;
					e.CancelButton.Visible = false;
					e.FinishButton.Text = "Close";
				}
				else if (e.Page.Name == "wzp_StartImport")
				{
					e.NextButton.Text = "Import";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_File_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Path")
			{
				return;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			if (e.RepositoryItem != null && e.RepositoryItem is RepositoryItemButtonEdit)
			{
				repositoryItemButtonEdit = e.RepositoryItem as RepositoryItemButtonEdit;
			}
			else
			{
				repositoryItemButtonEdit.AutoHeight = false;
				repositoryItemButtonEdit.Buttons.Clear();
				repositoryItemButtonEdit.Buttons.AddRange(new EditorButton[2]
				{
					new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, imageList1.Images[0], new KeyShortcut(Keys.None), _serializableAppearanceObjectNew1, "", null, null, enableImageTransparency: true),
					new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, imageList1.Images[1], new KeyShortcut(Keys.None), _serializableAppearanceObjectNew1, "", null, null, enableImageTransparency: true)
				});
				repositoryItemButtonEdit.Name = "rep_bte_File" + e.RowHandle;
				repositoryItemButtonEdit.ButtonPressed += (object s1, ButtonPressedEventArgs e2) =>
				{
					ButtonEdit buttonEdit = s1 as ButtonEdit;
					if (e2.Button.Index == 1)
					{
						string filter = "All files (*.*)|*.*";
						switch (Function.ToString(luk_FileType.EditValue).ToLower())
						{
						case "txt":
							filter = "Text file (*.txt)|*.txt";
							break;
						case "csv":
							filter = "Csv file (*.csv)|*.csv";
							break;
						case "xlsx":
							filter = "Excels file (*.xlsx)|*.xlsx";
							break;
						}
						OpenFileDialog openFileDialog = new OpenFileDialog();
						openFileDialog.Filter = filter;
						openFileDialog.Multiselect = false;
						openFileDialog.ShowDialog(this);
						if (File.Exists(openFileDialog.FileName))
						{
							gv_File.SetRowCellValue(e.RowHandle, e.Column, openFileDialog.FileName);
							buttonEdit.Properties.Buttons[0].Visible = true;
						}
					}
					else
					{
						gv_File.SetRowCellValue(e.RowHandle, e.Column, "");
						buttonEdit.Properties.Buttons[0].Visible = false;
					}
				};
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(gv_File.GetRowCellValue(e.RowHandle, e.Column))))
			{
				repositoryItemButtonEdit.Buttons[0].Visible = false;
			}
			else
			{
				repositoryItemButtonEdit.Buttons[0].Visible = true;
			}
			e.RepositoryItem = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_File_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Path")
			{
				return;
			}
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			if (e.RepositoryItem != null && e.RepositoryItem is RepositoryItemButtonEdit)
			{
				repositoryItemButtonEdit = e.RepositoryItem as RepositoryItemButtonEdit;
			}
			else
			{
				repositoryItemButtonEdit.AutoHeight = false;
				repositoryItemButtonEdit.Buttons.Clear();
				repositoryItemButtonEdit.Buttons.AddRange(new EditorButton[2]
				{
					new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, imageList1.Images[0], new KeyShortcut(Keys.None), _serializableAppearanceObjectNew1, "", null, null, enableImageTransparency: true),
					new EditorButton(ButtonPredefines.Glyph, "", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, imageList1.Images[1], new KeyShortcut(Keys.None), _serializableAppearanceObjectNew1, "", null, null, enableImageTransparency: true)
				});
				repositoryItemButtonEdit.Name = "rep_bte_File" + e.RowHandle;
				repositoryItemButtonEdit.ButtonPressed += (object s1, ButtonPressedEventArgs e2) =>
				{
					try
					{
						ButtonEditEx buttonEditEx = s1 as ButtonEditEx;
						if (e2.Button.Index == 1)
						{
							string filter = "All files (*.*)|*.*";
							switch (Function.ToString(luk_FileType.EditValue).ToLower())
							{
							case "txt":
								filter = "Text file (*.txt)|*.txt";
								break;
							case "csv":
								filter = "Csv file (*.csv)|*.csv";
								break;
							case "xlsx":
								filter = "Excels file (*.xlsx)|*.xlsx";
								break;
							}
							OpenFileDialog openFileDialog = new OpenFileDialog();
							openFileDialog.Filter = filter;
							openFileDialog.Multiselect = false;
							openFileDialog.ShowDialog(this);
							if (File.Exists(openFileDialog.FileName))
							{
								gv_File.SetRowCellValue(e.RowHandle, e.Column, openFileDialog.FileName);
								buttonEditEx.Properties.Buttons[0].Visible = true;
							}
						}
						else
						{
							gv_File.SetRowCellValue(e.RowHandle, e.Column, "");
							buttonEditEx.Properties.Buttons[0].Visible = false;
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
					}
				};
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(gv_File.GetRowCellValue(e.RowHandle, e.Column))))
			{
				repositoryItemButtonEdit.Buttons[0].Visible = false;
			}
			else
			{
				repositoryItemButtonEdit.Buttons[0].Visible = true;
			}
			e.RepositoryItem = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_File_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_File.FocusedColumn.FieldName == "Name")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_MappingObject_ShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void wiz_ImportData_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		try
		{
			switch (e.Page.Name)
			{
			case "wzp_SelectOperationType":
				switch (rdg_OperationType.SelectedIndex)
				{
				case 0:
					_currentAction = Enums.ImportActionEnum.Add;
					break;
				case 1:
					_currentAction = Enums.ImportActionEnum.Update;
					break;
				case 2:
					_currentAction = Enums.ImportActionEnum.AddUpdate;
					break;
				}
				tl_Object.ExpandAll();
				break;
			case "wzp_SelectObject":
			{
				if (tl_Object.FocusedNode == null)
				{
					break;
				}
				int code = Function.ParseInt(tl_Object.FocusedNode.GetValue("Code"));
				ImportObjectImpls importObjectImpls = _importObjects.FirstOrDefault((ImportObjectImpls t) => t.Code == (Enums.ImportObjectEnum)code);
				if (importObjectImpls != null && importObjectImpls.IsObject)
				{
					if ((importObjectImpls.Code == Enums.ImportObjectEnum.ProductTrees || importObjectImpls.Code == Enums.ImportObjectEnum.ProductTrees_Lines) && _currentAction == Enums.ImportActionEnum.AddUpdate)
					{
						WinCommons.Error(Enums.MessageCode.AddOrUpdateIsNotValidActionForBOMObject, "");
						e.Handled = true;
						break;
					}
					_currentObject = new ImportObjectImpls(importObjectImpls.Code, importObjectImpls.Name, importObjectImpls.Parent, importObjectImpls.Category, importObjectImpls.TableName, importObjectImpls.IsTreeNode, importObjectImpls.IsObject);
					if (_currentObject.Code != Enums.ImportObjectEnum.None)
					{
						SetObjectsToFileGrid();
						break;
					}
					WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
					e.Handled = true;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
					e.Handled = true;
				}
				break;
			}
			case "wzp_SelectSource":
				if (grd_File.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					if (dataTable.AsEnumerable().Any((DataRow t) => File.Exists(Function.ToString(t["Path"]))))
					{
						if (!SetObjectsToMappingGrid())
						{
							e.Handled = true;
						}
						else
						{
							rdg_ErrorHandling.SelectedIndex = 2;
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.NoSourceFielSelected, "");
						e.Handled = true;
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoDataToImport, "");
					e.Handled = true;
				}
				_ignoreUpTo = 10;
				calc_IgnoreUpto.EditValue = 10;
				break;
			case "wzp_DefineErrorHandling":
			{
				string value = "";
				switch (rdg_ErrorHandling.SelectedIndex)
				{
				case 0:
					_errorHandling = Enums.ImportErrorHandlingEnum.CancelOnError;
					value = "Cancel Import and Perform Rollback When One or More Errors Occur";
					break;
				case 1:
					_errorHandling = Enums.ImportErrorHandlingEnum.IgnoreError;
					value = "Ignore All Errors and Process Valid Records";
					break;
				case 2:
					_errorHandling = Enums.ImportErrorHandlingEnum.IgnoreUpTo;
					_ignoreUpTo = Function.ParseInt(calc_IgnoreUpto.EditValue);
					value = "Ignore Up to " + _ignoreUpTo + " Errors and Process Valid Records";
					break;
				}
				StringBuilder stringBuilder = new StringBuilder("Import Process Type: ");
				switch (_currentAction)
				{
				case Enums.ImportActionEnum.Add:
					stringBuilder.Append("Add New Data");
					break;
				case Enums.ImportActionEnum.Update:
					stringBuilder.Append("Update Existing Data");
					break;
				case Enums.ImportActionEnum.AddUpdate:
					stringBuilder.Append("Add New Data and Update Existing Data");
					break;
				}
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("Business Objects: ");
				for (int num = 0; num < _lstImportDataTableObject.Count; num++)
				{
					stringBuilder.Append(_lstImportDataTableObject[num].ImportObject.Code.ToString() + ",");
				}
				if (stringBuilder[stringBuilder.Length - 1] == ',')
				{
					stringBuilder.Length--;
				}
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("Error Handling Type: ");
				stringBuilder.Append(value);
				txt_Summary.Text = stringBuilder.ToString();
				break;
			}
			case "wzp_StartImport":
			{
				frm_ImportDataProcess frm_ImportDataProcess2 = new frm_ImportDataProcess();
				frm_ImportDataProcess2.StartPosition = FormStartPosition.CenterScreen;
				frm_ImportDataProcess2.ImpAction = _currentAction;
				frm_ImportDataProcess2.ErrorHandling = _errorHandling;
				frm_ImportDataProcess2.IgnoreUpTo = _ignoreUpTo;
				frm_ImportDataProcess2.LstDtImportObject = _lstImportDataTableObject.Select((ImportDataTableObject r) => new ImportDataTableObject(r.ImportData.Copy(), r.MappingData.Copy(), r.ImportObject)
				{
					TableFields = r.TableFields.Copy()
				}).ToList();
				frm_ImportDataProcess2.ShowDialog();
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void wiz_ImportData_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		try
		{
			switch (e.Page.Name)
			{
			case "wzp_SelectObject":
				_currentObject = new ImportObjectImpls();
				break;
			case "wzp_SelectSource":
				grd_File.DataSource = null;
				grd_File.RefreshDataSource();
				tl_Object.ExpandAll();
				break;
			case "wzp_Mapping":
				grd_MappingObject.DataSource = null;
				grd_MappingObject.RefreshDataSource();
				grd_MappingRule.DataSource = null;
				grd_MappingRule.RefreshDataSource();
				grd_SourceData.DataSource = null;
				grd_SourceData.RefreshDataSource();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void wiz_ImportData_CancelClick(object sender, CancelEventArgs e)
	{
		Close();
	}

	private void wiz_ImportData_FinishClick(object sender, CancelEventArgs e)
	{
		Close();
	}

	private void gv_File_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (sender is GridView gridView && e.Column == gridView.FocusedColumn && e.RowHandle == gridView.FocusedRowHandle && e.Column.FieldName == "Name")
			{
				e.Appearance.BackColor = gridView.Appearance.SelectedRow.BackColor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_MappingObject_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (sender is GridView gridView && e.Column == gridView.FocusedColumn && e.RowHandle == gridView.FocusedRowHandle && e.Column.FieldName == "Name")
			{
				e.Appearance.BackColor = gridView.Appearance.SelectedRow.BackColor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<ImportObjectImpls> CreateImportObject()
	{
		try
		{
			return new List<ImportObjectImpls>
			{
				new ImportObjectImpls(Enums.ImportObjectEnum.BusinessPartnersNode, "Business Partners", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.BusinessPartners, "Business Partner Master Data", Enums.ImportObjectEnum.BusinessPartnersNode, Enums.ImportCategoryEnum.MasterData, "OCRD", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.BPAddresses, "BPAddresses", Enums.ImportObjectEnum.BusinessPartners, Enums.ImportCategoryEnum.MasterData, "CRD1", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.Contacts, "Contacts", Enums.ImportObjectEnum.BusinessPartners, Enums.ImportCategoryEnum.MasterData, "OCPR", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.BPPaymentDates, "BPPaymentDates", Enums.ImportObjectEnum.BusinessPartners, Enums.ImportCategoryEnum.MasterData, "CRD5", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.BPBankAccounts, "BPBankAccounts", Enums.ImportObjectEnum.BusinessPartners, Enums.ImportCategoryEnum.MasterData, "OCRB", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.InventoryNode, "Inventory", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.Items, "Item Master Data", Enums.ImportObjectEnum.InventoryNode, Enums.ImportCategoryEnum.MasterData, "OITM", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.Items_Prices, "Item Master Data", Enums.ImportObjectEnum.Items, Enums.ImportCategoryEnum.MasterData, "ITM1", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.ItemWarehouseInfo, "Item Master Data", Enums.ImportObjectEnum.Items, Enums.ImportCategoryEnum.MasterData, "OITW", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.ItemUnitOfMeasurement, "Item Master Data", Enums.ImportObjectEnum.Items, Enums.ImportCategoryEnum.MasterData, "ITM12", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.ProductionNode, "Production", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.ProductTrees, "Bill of Materials", Enums.ImportObjectEnum.ProductionNode, Enums.ImportCategoryEnum.MasterData, "OITT", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.ProductTrees_Lines, "ProductTrees_Lines", Enums.ImportObjectEnum.ProductTrees, Enums.ImportCategoryEnum.MasterData, "ITT1", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.AdvanceBillOfMaterials, "Advance Bill of Materials", Enums.ImportObjectEnum.ProductionNode, Enums.ImportCategoryEnum.MasterData, "APZ_OITT", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.AdvanceBillOfMaterialsLines, "Advance Bill of Materials Lines", Enums.ImportObjectEnum.AdvanceBillOfMaterials, Enums.ImportCategoryEnum.MasterData, "APZ_ITT1", isTreeNode: false, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.HumanResourcesNode, "Human Resources", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesInfo, "Employee Master Data", Enums.ImportObjectEnum.HumanResourcesNode, Enums.ImportCategoryEnum.MasterData, "OHEM", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesAbsenceInfo, "EmployeesAbsenceInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "HEM1", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesEducationInfo, "EmployeesEducationInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "HEM2", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesReviewsInfo, "EmployeesReviewsInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "HEM3", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo, "EmployeesPrevEmpoymentInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "HEM4", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesRolesInfo, "EmployeesRolesInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "HEM6", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesHealthInfo, "EmployeesHealthInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "APZ_OHEH", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesContractsInfo, "EmployeesContractsInfo", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "APZ_OHEC", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.EmployeesRelatives, "EmployeesRelatives", Enums.ImportObjectEnum.EmployeesInfo, Enums.ImportCategoryEnum.MasterData, "APZ_OHER", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.ProjectManagerNode, "Project Manager", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "APZ_OPRG", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.WBSTemplate, "WBSTemplate", Enums.ImportObjectEnum.ProjectManagerNode, Enums.ImportCategoryEnum.MasterData, "APZ_OWBS", isTreeNode: true, isObject: true),
				new ImportObjectImpls(Enums.ImportObjectEnum.WBSTemplateDetails, "WBSTemplateDetails", Enums.ImportObjectEnum.WBSTemplate, Enums.ImportCategoryEnum.MasterData, "APZ_WBS1", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.WBSTemplateTask, "WBSTemplateTask", Enums.ImportObjectEnum.WBSTemplate, Enums.ImportCategoryEnum.MasterData, "APZ_TASK", isTreeNode: false, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.Calendar, "Calendar", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, "", isTreeNode: true, isObject: false),
				new ImportObjectImpls(Enums.ImportObjectEnum.Task, "Task", Enums.ImportObjectEnum.Calendar, Enums.ImportCategoryEnum.MasterData, "APZ_OCLG", isTreeNode: true, isObject: true)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new List<ImportObjectImpls>();
		}
	}

	private void LoadTreeListObject()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Parent", typeof(int)));
			dataTable.Columns.Add(new DataColumn("TableName", typeof(string)));
			dataTable.AcceptChanges();
			for (int i = 0; i < _importObjects.Count; i++)
			{
				if (_importObjects[i].IsTreeNode)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = (int)_importObjects[i].Code;
					dataRow["Name"] = _importObjects[i].Name;
					dataRow["Parent"] = (int)_importObjects[i].Parent;
					dataRow["TableName"] = Function.ToString(_importObjects[i].TableName);
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			tl_Object.DataSource = dataTable;
			tl_Object.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadFileType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "txt";
			dataRow["Name"] = "txt (Tab delimited)";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "xlsx";
			dataRow2["Name"] = "Excels";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = "csv";
			dataRow3["Name"] = "csv (Comma delimited)";
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			luk_FileType.Properties.Columns.Clear();
			luk_FileType.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			luk_FileType.Properties.DisplayMember = "Name";
			luk_FileType.Properties.ValueMember = "Code";
			luk_FileType.Properties.DataSource = dataTable;
			luk_FileType.EditValue = "txt";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetObjectsToFileGrid()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("TableName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Path", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = (int)_currentObject.Code;
			dataRow["Name"] = _currentObject.Code.ToString();
			dataRow["TableName"] = _currentObject.TableName;
			dataTable.Rows.Add(dataRow);
			IEnumerable<ImportObjectImpls> source = _importObjects.Where((ImportObjectImpls t) => t.Parent == _currentObject.Code);
			if (source.Count() > 0)
			{
				List<ImportObjectImpls> list = source.ToList();
				for (int num = 0; num < list.Count; num++)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Code"] = (int)list[num].Code;
					dataRow2["Name"] = list[num].Code.ToString();
					dataRow2["TableName"] = list[num].TableName;
					dataTable.Rows.Add(dataRow2);
				}
			}
			dataTable.AcceptChanges();
			grd_File.DataSource = dataTable;
			grd_File.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool SetObjectsToMappingGrid()
	{
		try
		{
			DataTable dataSourceTable = grd_File.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
				dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
				_lstImportDataTableObject = new List<ImportDataTableObject>();
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					DataRow dataRow = dataSourceTable.Rows[i];
					if (File.Exists(Function.ToString(dataRow["Path"])))
					{
						int code = Function.ParseInt(dataRow["Code"]);
						string text = Function.ToString(dataRow["Path"]);
						string text2 = Function.ToString(dataRow["TableName"]);
						ImportObjectImpls importObject = new ImportObjectImpls((Enums.ImportObjectEnum)code, "", Enums.ImportObjectEnum.None, Enums.ImportCategoryEnum.None, text2, isTreeNode: false, isObject: false);
						DataTable tableStructure = WinFuntions.GetTableStructure(text2);
						if (tableStructure == null)
						{
							WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
							return false;
						}
						tableStructure.TableName = text2;
						DataTable dataTable2 = tableStructure.Clone();
						if (dataTable2 == null || dataTable2.Columns.Count == 0)
						{
							WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
							return false;
						}
						dataTable2.TableName = text2;
						Tuple<string, DataSet> dataTableFromFile = GetDataTableFromFile(text, Function.ToString(luk_FileType.EditValue), dataTable2);
						if (!(dataTableFromFile.Item1 == ""))
						{
							WinCommons.Error(Enums.MessageCode.None, dataTableFromFile.Item1);
							return false;
						}
						DataSet item = dataTableFromFile.Item2;
						if (item == null || item.Tables.Count != 2 || item.Tables[0] == null || item.Tables[0].Rows.Count <= 0 || item.Tables[1] == null || item.Tables[1].Rows.Count <= 0)
						{
							WinCommons.Error(Enums.MessageCode.FileInvalidFormat, "", text);
							return false;
						}
						ImportDataTableObject importDataTableObject = new ImportDataTableObject(item.Tables[0], item.Tables[1], importObject);
						importDataTableObject.TableFields = GetColumnsFromDatatable(tableStructure);
						_lstImportDataTableObject.Add(importDataTableObject);
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["Code"] = importDataTableObject.ImportObject.Code;
						dataRow2["Name"] = importDataTableObject.ImportObject.Code.ToString();
						dataTable.Rows.Add(dataRow2);
					}
				}
				if (_lstImportDataTableObject.Count > 0 && dataTable.Rows.Count > 0)
				{
					dataTable.AcceptChanges();
					grd_MappingObject.DataSource = dataTable;
					grd_MappingObject.RefreshDataSource();
					int code2 = Function.ParseInt(gv_MappingObject.GetFocusedRowCellValue("Code"));
					ImportDataTableObject importDataTableObject2 = _lstImportDataTableObject.FirstOrDefault((ImportDataTableObject t) => t.ImportObject.Code == (Enums.ImportObjectEnum)code2);
					if (importDataTableObject2 != null)
					{
						grd_MappingRule.DataSource = importDataTableObject2.MappingData;
						grd_MappingRule.RefreshDataSource();
						gv_SourceData.Columns.Clear();
						grd_SourceData.DataSource = importDataTableObject2.ImportData;
						grd_SourceData.RefreshDataSource();
						gv_SourceData.PopulateColumns();
						rep_luk_TargetField.DataSource = importDataTableObject2.TableFields;
					}
					return true;
				}
				WinCommons.Error(Enums.MessageCode.ThereIsNoObjectOrImportCodeGenerated, "");
				return false;
			}
			WinCommons.Error(Enums.MessageCode.NoDataToImport, "");
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private DataTable GetColumnsFromDatatable(DataTable dt)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Caption", typeof(string)));
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Name"] = dt.Columns[i].ColumnName;
				dataRow["Caption"] = dt.Columns[i].Caption;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private Tuple<string, DataSet> GetDataTableFromFile(string path, string fileType, DataTable dtStructure)
	{
		try
		{
			return fileType.ToLower() switch
			{
				"csv" => GetDataFromCsvFile(path, dtStructure), 
				"txt" => GetDataFromTextFile(path, dtStructure), 
				"xlsx" => GetDataFromExcelFile(path, dtStructure), 
				_ => new Tuple<string, DataSet>("File type '" + fileType + "' is not supported", new DataSet()), 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Tuple<string, DataSet>(ex.Message, new DataSet());
		}
	}

	private Tuple<string, DataSet> GetDataFromExcelFile(string path, DataTable dtStructure)
	{
		try
		{
			DataTable dataTableFromExcel = ExcelHelper.GetDataTableFromExcel(path);
			if (dataTableFromExcel != null && dataTableFromExcel.Rows.Count > 1)
			{
				dataTableFromExcel.TableName = "Data";
				DataTable dataTable = new DataTable();
				dataTable.TableName = "Mapping";
				dataTable.Columns.Add("Source", typeof(string));
				dataTable.Columns.Add("Target", typeof(string));
				dataTable.AcceptChanges();
				DataRow dataRow = dataTableFromExcel.Rows[0];
				for (int i = 0; i < dataTableFromExcel.Columns.Count; i++)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Source"] = dataTableFromExcel.Columns[i].ColumnName.Trim();
					if (dtStructure.Columns.Contains(Function.ToString(dataRow[i])))
					{
						dataRow2["Target"] = Function.ToString(dataRow[i]);
					}
					dataTable.Rows.Add(dataRow2);
				}
				dataTableFromExcel.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				dataSet.AcceptChanges();
				DataSet dataSet2 = dataTableFromExcel.DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove(dataTableFromExcel.TableName);
					dataSet2.AcceptChanges();
				}
				DataSet dataSet3 = dataTable.DataSet;
				if (dataSet3 != null)
				{
					dataSet3.Tables.Remove(dataTable.TableName);
					dataSet3.AcceptChanges();
				}
				dataSet.Tables.Add(dataTableFromExcel);
				dataSet.Tables.Add(dataTable);
				dataSet.AcceptChanges();
				return new Tuple<string, DataSet>("", dataSet);
			}
			return new Tuple<string, DataSet>("File lines number must greater than 2", new DataSet());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Tuple<string, DataSet>(ex.Message, new DataSet());
		}
	}

	private Tuple<string, DataSet> GetDataFromTextFile(string path, DataTable dtStructure, char delimiter = '\t')
	{
		try
		{
			string[] array = File.ReadAllLines(path);
			if (array.Length > 2)
			{
				DataTable dataTable = new DataTable();
				dataTable.TableName = "Data";
				DataTable dataTable2 = new DataTable();
				dataTable2.TableName = "Mapping";
				dataTable2.Columns.Add("Source", typeof(string));
				dataTable2.Columns.Add("Target", typeof(string));
				dataTable2.AcceptChanges();
				string[] array2 = array[0].Split(delimiter);
				string[] array3 = array[1].Split(delimiter);
				for (int i = 0; i < array2.Length; i++)
				{
					if (dtStructure.Columns.Contains(array3[i].Trim()))
					{
						DataRow dataRow = dataTable2.NewRow();
						dataRow["Source"] = array2[i]?.Trim();
						dataRow["Target"] = array3[i]?.Trim();
						dataTable2.Rows.Add(dataRow);
						dataTable.Columns.Add(array3[i], typeof(string));
						continue;
					}
					return new Tuple<string, DataSet>(GetObjectName(dtStructure.TableName) + " template should not contains column '" + array3[i] + "'", new DataSet());
				}
				for (int j = 2; j < array.Length; j++)
				{
					string[] array4 = array[j].Split(delimiter);
					DataRow dataRow2 = dataTable.NewRow();
					for (int k = 0; k < array4.Length; k++)
					{
						dataRow2[dataTable.Columns[k]] = Function.ToString(array4[k]);
					}
					dataTable.Rows.Add(dataRow2);
				}
				dataTable.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				dataSet.AcceptChanges();
				DataSet dataSet2 = dataTable.DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove(dataTable.TableName);
					dataSet2.AcceptChanges();
				}
				DataSet dataSet3 = dataTable2.DataSet;
				if (dataSet3 != null)
				{
					dataSet3.Tables.Remove(dataTable2.TableName);
					dataSet3.AcceptChanges();
				}
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(dataTable2);
				dataSet.AcceptChanges();
				return new Tuple<string, DataSet>("", dataSet);
			}
			WinCommons.Error(Enums.MessageCode.FileLineNumberMustGreaterThan2, "");
			return new Tuple<string, DataSet>("File lines number must greater than 2", new DataSet());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Tuple<string, DataSet>(ex.Message, new DataSet());
		}
	}

	private string GetObjectName(string tableName)
	{
		try
		{
			return tableName switch
			{
				"OCRD" => "Business Parner Master Data", 
				"CRD1" => "BP Address", 
				"OCPR" => "BP Contact Employee", 
				"CRD5" => "BP Payment Date", 
				"OCRB" => "BP Bank Account", 
				"OITM" => "Item Master Data", 
				"ITM1" => "Item Price", 
				"OITW" => "Item Warehouse", 
				"ITM12" => "Item Unit Of Measurement", 
				"OITT" => "Bill of Material", 
				"ITT1" => "Bill of Material Lines", 
				"OHEM" => "Employees Master Data", 
				"HEM2" => "Employees Education", 
				"HEM3" => "Employees Reviews", 
				"HEM4" => "Employees Previous Employment", 
				"HEM6" => "Employees Roles", 
				"APZ_OHEC" => "Employees Contracts", 
				"APZ_OHEH" => "Employees Health", 
				_ => "", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private Tuple<string, DataSet> GetDataFromCsvFile(string path, DataTable dtStructure)
	{
		return GetDataFromTextFile(path, dtStructure, ',');
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ImportData.frm_ImportDataWizard));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.wiz_ImportData = new Apzon.Controls.Controls.WizardControlEx();
		this.wzp_SelectOperationType = new DevExpress.XtraWizard.WizardPage();
		this.panel2 = new System.Windows.Forms.Panel();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.rdg_OperationType = new Apzon.Controls.Editor.RadioGroupEx();
		this.wzp_SelectObject = new DevExpress.XtraWizard.WizardPage();
		this.panel3 = new System.Windows.Forms.Panel();
		this.label6 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.tl_Object = new DevExpress.XtraTreeList.TreeList();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.lbl_Object = new Apzon.Controls.Controls.LabelLine();
		this.wzp_SelectSource = new DevExpress.XtraWizard.WizardPage();
		this.grd_File = new Apzon.Controls.Grid.GridControlEx();
		this.gv_File = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_FileObject = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_FileType = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.panel4 = new System.Windows.Forms.Panel();
		this.label8 = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.wzp_Mapping = new DevExpress.XtraWizard.WizardPage();
		this.panel5 = new System.Windows.Forms.Panel();
		this.label1 = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.grd_MappingObject = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MappingObject = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_MappingObjectName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xTab_MapField = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xTPage_MappingRule = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_MappingRule = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MappingRule = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_TargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xTPage_SourceData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_SourceData = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SourceData = new Apzon.Controls.Grid.GridViewEx();
		this.wzp_DefineErrorHandling = new DevExpress.XtraWizard.WizardPage();
		this.calc_IgnoreUpto = new Apzon.Controls.Editor.CalcEditEx();
		this.rdg_ErrorHandling = new Apzon.Controls.Editor.RadioGroupEx();
		this.panel6 = new System.Windows.Forms.Panel();
		this.label12 = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.wzp_StartImport = new DevExpress.XtraWizard.WizardPage();
		this.txt_Summary = new Apzon.Controls.Editor.TextBoxEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.wzp_Finish = new DevExpress.XtraWizard.WizardPage();
		this.panel7 = new System.Windows.Forms.Panel();
		this.label11 = new System.Windows.Forms.Label();
		this.label14 = new System.Windows.Forms.Label();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		((System.ComponentModel.ISupportInitialize)this.wiz_ImportData).BeginInit();
		this.wiz_ImportData.SuspendLayout();
		this.wzp_SelectOperationType.SuspendLayout();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).BeginInit();
		this.wzp_SelectObject.SuspendLayout();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tl_Object).BeginInit();
		this.wzp_SelectSource.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_File).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_File).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_FileObject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FileType.Properties).BeginInit();
		this.panel4.SuspendLayout();
		this.wzp_Mapping.SuspendLayout();
		this.panel5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MappingObject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingObject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_MappingObjectName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xTab_MapField).BeginInit();
		this.xTab_MapField.SuspendLayout();
		this.xTPage_MappingRule.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MappingRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).BeginInit();
		this.xTPage_SourceData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).BeginInit();
		this.wzp_DefineErrorHandling.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_IgnoreUpto.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_ErrorHandling.Properties).BeginInit();
		this.panel6.SuspendLayout();
		this.wzp_StartImport.SuspendLayout();
		this.panel1.SuspendLayout();
		this.wzp_Finish.SuspendLayout();
		this.panel7.SuspendLayout();
		base.SuspendLayout();
		this.wiz_ImportData.AllowPagePadding = false;
		this.wiz_ImportData.Controls.Add(this.wzp_SelectOperationType);
		this.wiz_ImportData.Controls.Add(this.wzp_SelectObject);
		this.wiz_ImportData.Controls.Add(this.wzp_SelectSource);
		this.wiz_ImportData.Controls.Add(this.wzp_Mapping);
		this.wiz_ImportData.Controls.Add(this.wzp_DefineErrorHandling);
		this.wiz_ImportData.Controls.Add(this.wzp_StartImport);
		this.wiz_ImportData.Controls.Add(this.wzp_Finish);
		this.wiz_ImportData.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wiz_ImportData.ImageWidth = 216;
		this.wiz_ImportData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_ImportData.MinimumSize = new System.Drawing.Size(117, 123);
		this.wiz_ImportData.Name = "wiz_ImportData";
		this.wiz_ImportData.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[7] { this.wzp_SelectOperationType, this.wzp_SelectObject, this.wzp_SelectSource, this.wzp_Mapping, this.wzp_DefineErrorHandling, this.wzp_StartImport, this.wzp_Finish });
		this.wiz_ImportData.Size = new System.Drawing.Size(917, 679);
		this.wiz_ImportData.TitleHeight = 0;
		this.wiz_ImportData.CancelClick += new System.ComponentModel.CancelEventHandler(this.wiz_ImportData_CancelClick);
		this.wiz_ImportData.FinishClick += new System.ComponentModel.CancelEventHandler(this.wiz_ImportData_FinishClick);
		this.wiz_ImportData.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_ImportData_NextClick);
		this.wiz_ImportData.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_ImportData_PrevClick);
		this.wiz_ImportData.CustomizeCommandButtons += new DevExpress.XtraWizard.WizardCustomizeCommandButtonsEventHandler(this.wiz_ImportData_CustomizeCommandButtons);
		this.wzp_SelectOperationType.Controls.Add(this.panel2);
		this.wzp_SelectOperationType.Controls.Add(this.rdg_OperationType);
		this.wzp_SelectOperationType.DescriptionText = "Select the type of operation you want to perform while importing data. To continue, choose the \"Next\" button. ";
		this.wzp_SelectOperationType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_SelectOperationType.Name = "wzp_SelectOperationType";
		this.wzp_SelectOperationType.Size = new System.Drawing.Size(917, 622);
		this.wzp_SelectOperationType.Text = "Step 2: Specify Operation Ty[e";
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.Color.White;
		this.panel2.Controls.Add(this.label4);
		this.panel2.Controls.Add(this.label5);
		this.panel2.Location = new System.Drawing.Point(3, 4);
		this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(899, 143);
		this.panel2.TabIndex = 3;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(27, 31);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(692, 17);
		this.label4.TabIndex = 2;
		this.label4.Text = "Select the type of operation you want to perform while importing data. To continue, choose the \"Next\" button. ";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(27, 7);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(240, 18);
		this.label5.TabIndex = 1;
		this.label5.Text = "Step 1: Specify Operation Type";
		this.rdg_OperationType.EditControl = null;
		this.rdg_OperationType.FieldName = null;
		this.rdg_OperationType.IsSearchControl = false;
		this.rdg_OperationType.Location = new System.Drawing.Point(267, 244);
		this.rdg_OperationType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.rdg_OperationType.Name = "rdg_OperationType";
		this.rdg_OperationType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_OperationType.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_OperationType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Add New Data"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Update Existing Data"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Add New Data and Update Existing Data")
		});
		this.rdg_OperationType.SettingInVisible = false;
		this.rdg_OperationType.SettingReadonly = false;
		this.rdg_OperationType.Size = new System.Drawing.Size(419, 118);
		this.rdg_OperationType.TabIndex = 1;
		this.wzp_SelectObject.Controls.Add(this.panel3);
		this.wzp_SelectObject.Controls.Add(this.tl_Object);
		this.wzp_SelectObject.Controls.Add(this.lbl_Object);
		this.wzp_SelectObject.DescriptionText = "Select a business object you want to import.  To continue, choose \"Next\" button.";
		this.wzp_SelectObject.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_SelectObject.Name = "wzp_SelectObject";
		this.wzp_SelectObject.Size = new System.Drawing.Size(917, 622);
		this.wzp_SelectObject.Text = "Step 3: Select a Business Object";
		this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel3.BackColor = System.Drawing.Color.White;
		this.panel3.Controls.Add(this.label6);
		this.panel3.Controls.Add(this.label7);
		this.panel3.Location = new System.Drawing.Point(3, 4);
		this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(899, 143);
		this.panel3.TabIndex = 4;
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(27, 31);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(508, 17);
		this.label6.TabIndex = 2;
		this.label6.Text = "Select a business object you want to import.  To continue, choose \"Next\" button.";
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.Location = new System.Drawing.Point(27, 7);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(248, 18);
		this.label7.TabIndex = 1;
		this.label7.Text = "Step 2: Select a Business Object";
		this.tl_Object.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tl_Object.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(239, 240, 242);
		this.tl_Object.Appearance.FocusedRow.Options.UseBackColor = true;
		this.tl_Object.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.treeListColumn1 });
		this.tl_Object.Cursor = System.Windows.Forms.Cursors.Default;
		this.tl_Object.KeyFieldName = "Code";
		this.tl_Object.Location = new System.Drawing.Point(217, 159);
		this.tl_Object.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tl_Object.MinWidth = 23;
		this.tl_Object.Name = "tl_Object";
		this.tl_Object.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.tl_Object.OptionsView.ShowColumns = false;
		this.tl_Object.OptionsView.ShowIndicator = false;
		this.tl_Object.ParentFieldName = "Parent";
		this.tl_Object.Size = new System.Drawing.Size(686, 438);
		this.tl_Object.TabIndex = 1;
		this.tl_Object.TreeLevelWidth = 21;
		this.treeListColumn1.Caption = "Name";
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "Name";
		this.treeListColumn1.MinWidth = 23;
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.OptionsColumn.AllowEdit = false;
		this.treeListColumn1.OptionsColumn.ReadOnly = true;
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.treeListColumn1.Width = 87;
		this.lbl_Object.EditControl = null;
		this.lbl_Object.EditValue = "Business Object";
		this.lbl_Object.FieldName = null;
		this.lbl_Object.HideWhenEmpty = false;
		this.lbl_Object.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Object.Location = new System.Drawing.Point(35, 161);
		this.lbl_Object.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lbl_Object.Name = "lbl_Object";
		this.lbl_Object.ReadOnly = false;
		this.lbl_Object.SettingInVisible = false;
		this.lbl_Object.SettingReadonly = false;
		this.lbl_Object.ShowUnderLine = true;
		this.lbl_Object.Size = new System.Drawing.Size(192, 25);
		this.lbl_Object.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Object.TabIndex = 0;
		this.lbl_Object.Utext = "Business Object";
		this.wzp_SelectSource.Controls.Add(this.grd_File);
		this.wzp_SelectSource.Controls.Add(this.luk_FileType);
		this.wzp_SelectSource.Controls.Add(this.labelLine1);
		this.wzp_SelectSource.Controls.Add(this.panel4);
		this.wzp_SelectSource.DescriptionText = "To define data sources for business objects, do the following:";
		this.wzp_SelectSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_SelectSource.Name = "wzp_SelectSource";
		this.wzp_SelectSource.Size = new System.Drawing.Size(917, 622);
		this.wzp_SelectSource.Text = "Step 4: Select Data Source";
		this.grd_File.AllowPasteExcel = false;
		this.grd_File.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_File.ClipboardData = null;
		this.grd_File.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_File.Location = new System.Drawing.Point(26, 199);
		this.grd_File.MainView = this.gv_File;
		this.grd_File.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_File.Name = "grd_File";
		this.grd_File.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_bte_FileObject });
		this.grd_File.Size = new System.Drawing.Size(877, 392);
		this.grd_File.TabIndex = 8;
		this.grd_File.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_File });
		this.gv_File.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_File.ColumnKey = null;
		this.gv_File.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.gv_File.DetailHeight = 431;
		this.gv_File.DisableFormSetting = false;
		this.gv_File.DisplayException = false;
		this.gv_File.EditColumn = null;
		this.gv_File.FieldCurrency = null;
		this.gv_File.FieldRate = null;
		this.gv_File.FormId = "oImportDataWz";
		this.gv_File.GridControl = this.grd_File;
		this.gv_File.IgnoreFilter = false;
		this.gv_File.IndexEditRow = -1;
		this.gv_File.IndicatorWidth = 41;
		this.gv_File.IsApzTable = false;
		this.gv_File.IsInsertNewRow = false;
		this.gv_File.IsLoadingData = false;
		this.gv_File.IsMappingContextAction = false;
		this.gv_File.ItemId = "OFILE";
		this.gv_File.LstColumnAllowBatch = null;
		this.gv_File.LstColumnNotAllowBatch = null;
		this.gv_File.LstColumnNotEditable = null;
		this.gv_File.LstColumNotSetting = null;
		this.gv_File.Name = "gv_File";
		this.gv_File.OptionsView.ColumnAutoWidth = false;
		this.gv_File.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_File.OptionsView.ShowColumnHeaders = false;
		this.gv_File.OptionsView.ShowGroupPanel = false;
		this.gv_File.OptionsView.ShowIndicator = false;
		this.gv_File.UseBatchMod = false;
		this.gv_File.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_File_RowCellStyle);
		this.gv_File.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_File_CustomRowCellEdit);
		this.gv_File.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_File_CustomRowCellEditForEditing);
		this.gv_File.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_File_ShowingEditor);
		this.gridColumn1.Caption = "Object Name";
		this.gridColumn1.ColumnEdit = this.rep_bte_FileObject;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.MinWidth = 23;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 222;
		this.rep_bte_FileObject.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_bte_FileObject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_FileObject.Name = "rep_bte_FileObject";
		this.rep_bte_FileObject.ReadOnly = true;
		this.gridColumn2.Caption = "File Path";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Path";
		this.gridColumn2.MinWidth = 23;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 350;
		this.luk_FileType.Location = new System.Drawing.Point(161, 169);
		this.luk_FileType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.luk_FileType.Name = "luk_FileType";
		this.luk_FileType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FileType.Properties.NullText = "";
		this.luk_FileType.Size = new System.Drawing.Size(198, 22);
		this.luk_FileType.TabIndex = 7;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Select File Type";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(28, 171);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(142, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Select File Type";
		this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel4.BackColor = System.Drawing.Color.White;
		this.panel4.Controls.Add(this.label8);
		this.panel4.Controls.Add(this.label9);
		this.panel4.Location = new System.Drawing.Point(3, 4);
		this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(899, 143);
		this.panel4.TabIndex = 5;
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(27, 31);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(467, 85);
		this.label8.TabIndex = 2;
		this.label8.Text = resources.GetString("label8.Text");
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label9.Location = new System.Drawing.Point(27, 7);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(207, 18);
		this.label9.TabIndex = 1;
		this.label9.Text = "Step 3: Select Data Source";
		this.wzp_Mapping.Controls.Add(this.panel5);
		this.wzp_Mapping.Controls.Add(this.grd_MappingObject);
		this.wzp_Mapping.Controls.Add(this.xTab_MapField);
		this.wzp_Mapping.DescriptionText = "Mapping source colums to target columns";
		this.wzp_Mapping.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_Mapping.Name = "wzp_Mapping";
		this.wzp_Mapping.Size = new System.Drawing.Size(917, 622);
		this.wzp_Mapping.Text = "Step 5: Map Object Fields";
		this.panel5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel5.BackColor = System.Drawing.Color.White;
		this.panel5.Controls.Add(this.label1);
		this.panel5.Controls.Add(this.label10);
		this.panel5.Location = new System.Drawing.Point(3, 4);
		this.panel5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel5.Name = "panel5";
		this.panel5.Size = new System.Drawing.Size(894, 143);
		this.panel5.TabIndex = 6;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(27, 31);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(687, 102);
		this.label1.TabIndex = 2;
		this.label1.Text = resources.GetString("label1.Text");
		this.label10.AutoSize = true;
		this.label10.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label10.Location = new System.Drawing.Point(27, 7);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(198, 18);
		this.label10.TabIndex = 1;
		this.label10.Text = "Step 4: Map Object Fields";
		this.grd_MappingObject.AllowPasteExcel = false;
		this.grd_MappingObject.ClipboardData = null;
		this.grd_MappingObject.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_MappingObject.Location = new System.Drawing.Point(8, 154);
		this.grd_MappingObject.MainView = this.gv_MappingObject;
		this.grd_MappingObject.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_MappingObject.Name = "grd_MappingObject";
		this.grd_MappingObject.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_bte_MappingObjectName });
		this.grd_MappingObject.Size = new System.Drawing.Size(233, 448);
		this.grd_MappingObject.TabIndex = 4;
		this.grd_MappingObject.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MappingObject });
		this.gv_MappingObject.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MappingObject.ColumnKey = null;
		this.gv_MappingObject.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn3, this.gridColumn4 });
		this.gv_MappingObject.DetailHeight = 431;
		this.gv_MappingObject.DisableFormSetting = false;
		this.gv_MappingObject.DisplayException = false;
		this.gv_MappingObject.EditColumn = null;
		this.gv_MappingObject.FieldCurrency = null;
		this.gv_MappingObject.FieldRate = null;
		this.gv_MappingObject.FormId = "oImportDataWz";
		this.gv_MappingObject.GridControl = this.grd_MappingObject;
		this.gv_MappingObject.IgnoreFilter = false;
		this.gv_MappingObject.IndexEditRow = -1;
		this.gv_MappingObject.IndicatorWidth = 41;
		this.gv_MappingObject.IsApzTable = false;
		this.gv_MappingObject.IsInsertNewRow = false;
		this.gv_MappingObject.IsLoadingData = false;
		this.gv_MappingObject.IsMappingContextAction = false;
		this.gv_MappingObject.ItemId = "OMAPOBJ";
		this.gv_MappingObject.LstColumnAllowBatch = null;
		this.gv_MappingObject.LstColumnNotAllowBatch = null;
		this.gv_MappingObject.LstColumnNotEditable = null;
		this.gv_MappingObject.LstColumNotSetting = null;
		this.gv_MappingObject.Name = "gv_MappingObject";
		this.gv_MappingObject.OptionsView.ColumnAutoWidth = false;
		this.gv_MappingObject.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_MappingObject.OptionsView.ShowColumnHeaders = false;
		this.gv_MappingObject.OptionsView.ShowGroupPanel = false;
		this.gv_MappingObject.OptionsView.ShowIndicator = false;
		this.gv_MappingObject.UseBatchMod = false;
		this.gv_MappingObject.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_MappingObject_RowCellStyle);
		this.gv_MappingObject.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_MappingObject_ShowingEditor);
		this.gv_MappingObject.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_MappingObject_FocusedRowObjectChanged);
		this.gridColumn3.Caption = "Object";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Code";
		this.gridColumn3.MinWidth = 23;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Width = 87;
		this.gridColumn4.Caption = "Name";
		this.gridColumn4.ColumnEdit = this.rep_bte_MappingObjectName;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Name";
		this.gridColumn4.MinWidth = 23;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 0;
		this.gridColumn4.Width = 198;
		this.rep_bte_MappingObjectName.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_bte_MappingObjectName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_MappingObjectName.Name = "rep_bte_MappingObjectName";
		this.rep_bte_MappingObjectName.ReadOnly = true;
		this.xTab_MapField.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xTab_MapField.IsSearchControl = false;
		this.xTab_MapField.Location = new System.Drawing.Point(269, 154);
		this.xTab_MapField.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.xTab_MapField.Name = "xTab_MapField";
		this.xTab_MapField.SelectedTabPage = this.xTPage_MappingRule;
		this.xTab_MapField.Size = new System.Drawing.Size(633, 452);
		this.xTab_MapField.TabIndex = 0;
		this.xTab_MapField.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xTPage_MappingRule, this.xTPage_SourceData });
		this.xTPage_MappingRule.Controls.Add(this.grd_MappingRule);
		this.xTPage_MappingRule.EditControl = null;
		this.xTPage_MappingRule.EditValue = "Mapping Rules";
		this.xTPage_MappingRule.FieldName = null;
		this.xTPage_MappingRule.IsSearchControl = false;
		this.xTPage_MappingRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.xTPage_MappingRule.Name = "xTPage_MappingRule";
		this.xTPage_MappingRule.ReadOnly = false;
		this.xTPage_MappingRule.SettingInVisible = false;
		this.xTPage_MappingRule.SettingReadonly = false;
		this.xTPage_MappingRule.Size = new System.Drawing.Size(631, 422);
		this.xTPage_MappingRule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xTPage_MappingRule.Text = "Mapping Rules";
		this.grd_MappingRule.AllowPasteExcel = false;
		this.grd_MappingRule.ClipboardData = null;
		this.grd_MappingRule.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MappingRule.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_MappingRule.Location = new System.Drawing.Point(0, 0);
		this.grd_MappingRule.MainView = this.gv_MappingRule;
		this.grd_MappingRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_MappingRule.Name = "grd_MappingRule";
		this.grd_MappingRule.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_luk_TargetField });
		this.grd_MappingRule.Size = new System.Drawing.Size(631, 422);
		this.grd_MappingRule.TabIndex = 5;
		this.grd_MappingRule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MappingRule });
		this.gv_MappingRule.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MappingRule.ColumnKey = null;
		this.gv_MappingRule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn5, this.gridColumn6 });
		this.gv_MappingRule.DetailHeight = 431;
		this.gv_MappingRule.DisableFormSetting = false;
		this.gv_MappingRule.DisplayException = false;
		this.gv_MappingRule.EditColumn = null;
		this.gv_MappingRule.FieldCurrency = null;
		this.gv_MappingRule.FieldRate = null;
		this.gv_MappingRule.FormId = "oImportDataWz";
		this.gv_MappingRule.GridControl = this.grd_MappingRule;
		this.gv_MappingRule.IgnoreFilter = false;
		this.gv_MappingRule.IndexEditRow = -1;
		this.gv_MappingRule.IndicatorWidth = 47;
		this.gv_MappingRule.IsApzTable = false;
		this.gv_MappingRule.IsInsertNewRow = false;
		this.gv_MappingRule.IsLoadingData = false;
		this.gv_MappingRule.IsMappingContextAction = false;
		this.gv_MappingRule.ItemId = "OMAPRULE";
		this.gv_MappingRule.LstColumnAllowBatch = null;
		this.gv_MappingRule.LstColumnNotAllowBatch = null;
		this.gv_MappingRule.LstColumnNotEditable = null;
		this.gv_MappingRule.LstColumNotSetting = null;
		this.gv_MappingRule.Name = "gv_MappingRule";
		this.gv_MappingRule.OptionsView.ColumnAutoWidth = false;
		this.gv_MappingRule.OptionsView.ShowGroupPanel = false;
		this.gv_MappingRule.UseBatchMod = false;
		this.gv_MappingRule.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CustomDrawRowIndicator);
		this.gridColumn5.Caption = "Source Fields";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Source";
		this.gridColumn5.MinWidth = 23;
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 0;
		this.gridColumn5.Width = 175;
		this.gridColumn6.Caption = "Target Fields";
		this.gridColumn6.ColumnEdit = this.rep_luk_TargetField;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Target";
		this.gridColumn6.MinWidth = 23;
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 1;
		this.gridColumn6.Width = 175;
		this.rep_luk_TargetField.AutoHeight = false;
		this.rep_luk_TargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_TargetField.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.rep_luk_TargetField.DisplayMember = "Caption";
		this.rep_luk_TargetField.Name = "rep_luk_TargetField";
		this.rep_luk_TargetField.NullText = "";
		this.rep_luk_TargetField.ValueMember = "Name";
		this.xTPage_SourceData.Controls.Add(this.grd_SourceData);
		this.xTPage_SourceData.EditControl = null;
		this.xTPage_SourceData.EditValue = "Source Data";
		this.xTPage_SourceData.FieldName = null;
		this.xTPage_SourceData.IsSearchControl = false;
		this.xTPage_SourceData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.xTPage_SourceData.Name = "xTPage_SourceData";
		this.xTPage_SourceData.ReadOnly = false;
		this.xTPage_SourceData.SettingInVisible = false;
		this.xTPage_SourceData.SettingReadonly = false;
		this.xTPage_SourceData.Size = new System.Drawing.Size(631, 422);
		this.xTPage_SourceData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xTPage_SourceData.Text = "Source Data";
		this.grd_SourceData.AllowPasteExcel = false;
		this.grd_SourceData.ClipboardData = null;
		this.grd_SourceData.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SourceData.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_SourceData.Location = new System.Drawing.Point(0, 0);
		this.grd_SourceData.MainView = this.gv_SourceData;
		this.grd_SourceData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_SourceData.Name = "grd_SourceData";
		this.grd_SourceData.Size = new System.Drawing.Size(631, 422);
		this.grd_SourceData.TabIndex = 6;
		this.grd_SourceData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SourceData });
		this.gv_SourceData.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SourceData.ColumnKey = null;
		this.gv_SourceData.DetailHeight = 431;
		this.gv_SourceData.DisableFormSetting = false;
		this.gv_SourceData.DisplayException = false;
		this.gv_SourceData.EditColumn = null;
		this.gv_SourceData.FieldCurrency = null;
		this.gv_SourceData.FieldRate = null;
		this.gv_SourceData.FormId = "oImportDataWz";
		this.gv_SourceData.GridControl = this.grd_SourceData;
		this.gv_SourceData.IgnoreFilter = false;
		this.gv_SourceData.IndexEditRow = -1;
		this.gv_SourceData.IndicatorWidth = 70;
		this.gv_SourceData.IsApzTable = false;
		this.gv_SourceData.IsInsertNewRow = false;
		this.gv_SourceData.IsLoadingData = false;
		this.gv_SourceData.IsMappingContextAction = false;
		this.gv_SourceData.ItemId = "OSOURCEDATA";
		this.gv_SourceData.LstColumnAllowBatch = null;
		this.gv_SourceData.LstColumnNotAllowBatch = null;
		this.gv_SourceData.LstColumnNotEditable = null;
		this.gv_SourceData.LstColumNotSetting = null;
		this.gv_SourceData.Name = "gv_SourceData";
		this.gv_SourceData.OptionsBehavior.ReadOnly = true;
		this.gv_SourceData.OptionsView.ColumnAutoWidth = false;
		this.gv_SourceData.OptionsView.ShowGroupPanel = false;
		this.gv_SourceData.UseBatchMod = false;
		this.gv_SourceData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CustomDrawRowIndicator);
		this.wzp_DefineErrorHandling.Controls.Add(this.calc_IgnoreUpto);
		this.wzp_DefineErrorHandling.Controls.Add(this.rdg_ErrorHandling);
		this.wzp_DefineErrorHandling.Controls.Add(this.panel6);
		this.wzp_DefineErrorHandling.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_DefineErrorHandling.Name = "wzp_DefineErrorHandling";
		this.wzp_DefineErrorHandling.Size = new System.Drawing.Size(917, 622);
		this.calc_IgnoreUpto.EditControl = null;
		this.calc_IgnoreUpto.FieldName = null;
		this.calc_IgnoreUpto.IsSearchControl = false;
		this.calc_IgnoreUpto.Location = new System.Drawing.Point(377, 331);
		this.calc_IgnoreUpto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.calc_IgnoreUpto.Name = "calc_IgnoreUpto";
		this.calc_IgnoreUpto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_IgnoreUpto.Properties.IsSearchControl = false;
		this.calc_IgnoreUpto.Properties.Mask.EditMask = "f0";
		this.calc_IgnoreUpto.SettingInVisible = false;
		this.calc_IgnoreUpto.SettingReadonly = false;
		this.calc_IgnoreUpto.Size = new System.Drawing.Size(45, 22);
		this.calc_IgnoreUpto.TabIndex = 9;
		this.rdg_ErrorHandling.EditControl = null;
		this.rdg_ErrorHandling.FieldName = null;
		this.rdg_ErrorHandling.IsSearchControl = false;
		this.rdg_ErrorHandling.Location = new System.Drawing.Point(271, 251);
		this.rdg_ErrorHandling.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.rdg_ErrorHandling.Name = "rdg_ErrorHandling";
		this.rdg_ErrorHandling.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_ErrorHandling.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_ErrorHandling.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Cancel Import and Perform Rollback When One or More Errors Occur"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Ignore All Errors and Process Valid Records"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Ignore Up to                  Errors and Process Valid Records")
		});
		this.rdg_ErrorHandling.SettingInVisible = false;
		this.rdg_ErrorHandling.SettingReadonly = false;
		this.rdg_ErrorHandling.Size = new System.Drawing.Size(419, 118);
		this.rdg_ErrorHandling.TabIndex = 8;
		this.panel6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel6.BackColor = System.Drawing.Color.White;
		this.panel6.Controls.Add(this.label12);
		this.panel6.Controls.Add(this.label13);
		this.panel6.Location = new System.Drawing.Point(3, 4);
		this.panel6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel6.Name = "panel6";
		this.panel6.Size = new System.Drawing.Size(899, 143);
		this.panel6.TabIndex = 7;
		this.label12.AutoSize = true;
		this.label12.Location = new System.Drawing.Point(27, 31);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(488, 34);
		this.label12.TabIndex = 2;
		this.label12.Text = "Define the option to handling ther errors that occur during the import process.\r\nTo continue, choose the \"Next\" button.";
		this.label13.AutoSize = true;
		this.label13.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label13.Location = new System.Drawing.Point(27, 7);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(227, 18);
		this.label13.TabIndex = 1;
		this.label13.Text = "Step 6: Define Error Handling";
		this.wzp_StartImport.Controls.Add(this.txt_Summary);
		this.wzp_StartImport.Controls.Add(this.panel1);
		this.wzp_StartImport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_StartImport.Name = "wzp_StartImport";
		this.wzp_StartImport.Size = new System.Drawing.Size(917, 622);
		this.txt_Summary.EditControl = null;
		this.txt_Summary.EditValue = "";
		this.txt_Summary.FieldName = null;
		this.txt_Summary.IsSearchControl = false;
		this.txt_Summary.Location = new System.Drawing.Point(55, 175);
		this.txt_Summary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Summary.Multiline = true;
		this.txt_Summary.Name = "txt_Summary";
		this.txt_Summary.SettingInVisible = false;
		this.txt_Summary.SettingReadonly = false;
		this.txt_Summary.Size = new System.Drawing.Size(805, 416);
		this.txt_Summary.TabIndex = 9;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.label2);
		this.panel1.Controls.Add(this.label3);
		this.panel1.Location = new System.Drawing.Point(3, 4);
		this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(899, 143);
		this.panel1.TabIndex = 8;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(27, 31);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(541, 34);
		this.label2.TabIndex = 2;
		this.label2.Text = "To start importing data into IRS, choose the \"Import\" button.\r\nNote that the duration of the data import depends on the number of records imported.";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.Location = new System.Drawing.Point(27, 7);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(220, 18);
		this.label3.TabIndex = 1;
		this.label3.Text = "Step 7: Start Importing Data";
		this.wzp_Finish.Controls.Add(this.panel7);
		this.wzp_Finish.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wzp_Finish.Name = "wzp_Finish";
		this.wzp_Finish.Size = new System.Drawing.Size(917, 622);
		this.panel7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel7.BackColor = System.Drawing.Color.White;
		this.panel7.Controls.Add(this.label11);
		this.panel7.Controls.Add(this.label14);
		this.panel7.Location = new System.Drawing.Point(7, 4);
		this.panel7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panel7.Name = "panel7";
		this.panel7.Size = new System.Drawing.Size(899, 143);
		this.panel7.TabIndex = 8;
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(27, 31);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(154, 17);
		this.label11.TabIndex = 2;
		this.label11.Text = "Import process finished.";
		this.label14.AutoSize = true;
		this.label14.Font = new System.Drawing.Font("Tahoma", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label14.Location = new System.Drawing.Point(27, 7);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(110, 18);
		this.label14.TabIndex = 1;
		this.label14.Text = "Step 8: Finish";
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "erase-icon.png");
		this.imageList1.Images.SetKeyName(1, "search_icon-black.png");
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(917, 679);
		base.Controls.Add(this.wiz_ImportData);
		base.FormId = "oImportDataWz";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_ImportDataWizard";
		this.Text = "Data Import Wizard";
		base.Load += new System.EventHandler(this.frm_ImportDataWizard_Load);
		((System.ComponentModel.ISupportInitialize)this.wiz_ImportData).EndInit();
		this.wiz_ImportData.ResumeLayout(false);
		this.wzp_SelectOperationType.ResumeLayout(false);
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).EndInit();
		this.wzp_SelectObject.ResumeLayout(false);
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.tl_Object).EndInit();
		this.wzp_SelectSource.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_File).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_File).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_FileObject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FileType.Properties).EndInit();
		this.panel4.ResumeLayout(false);
		this.panel4.PerformLayout();
		this.wzp_Mapping.ResumeLayout(false);
		this.panel5.ResumeLayout(false);
		this.panel5.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MappingObject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingObject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_MappingObjectName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xTab_MapField).EndInit();
		this.xTab_MapField.ResumeLayout(false);
		this.xTPage_MappingRule.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MappingRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).EndInit();
		this.xTPage_SourceData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).EndInit();
		this.wzp_DefineErrorHandling.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calc_IgnoreUpto.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_ErrorHandling.Properties).EndInit();
		this.panel6.ResumeLayout(false);
		this.panel6.PerformLayout();
		this.wzp_StartImport.ResumeLayout(false);
		this.wzp_StartImport.PerformLayout();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.wzp_Finish.ResumeLayout(false);
		this.panel7.ResumeLayout(false);
		this.panel7.PerformLayout();
		base.ResumeLayout(false);
	}
}
