using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using DevExpress.Office;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Design;
using DevExpress.XtraSpreadsheet.UI;

namespace Apzon.Forms.Reports;

public class frm_Speadsheet : XtraFormEx
{
	private IWorkbook workbook;

	private DataTable data;

	private DataTable structure;

	private string title;

	public string pathFile;

	private IContainer components;

	private SpreadsheetControl spreadsheetControl1;

	private RibbonControl ribbonControl1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem2;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem3;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem4;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem5;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem6;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem7;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem8;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem9;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem10;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem11;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem12;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem13;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem14;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem15;

	private BarButtonGroup barButtonGroup1;

	private ChangeFontNameItem changeFontNameItem1;

	private RepositoryItemFontEdit repositoryItemFontEdit1;

	private ChangeFontSizeItem changeFontSizeItem1;

	private RepositoryItemSpreadsheetFontSizeEdit repositoryItemSpreadsheetFontSizeEdit1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem16;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem17;

	private BarButtonGroup barButtonGroup2;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem1;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem2;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem3;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem4;

	private BarButtonGroup barButtonGroup3;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem18;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem19;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem20;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem21;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem22;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem23;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem24;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem25;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem26;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem27;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem28;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem29;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem30;

	private ChangeBorderLineColorItem changeBorderLineColorItem1;

	private ChangeBorderLineStyleItem changeBorderLineStyleItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown1;

	private BarButtonGroup barButtonGroup4;

	private ChangeCellFillColorItem changeCellFillColorItem1;

	private ChangeFontColorItem changeFontColorItem1;

	private BarButtonGroup barButtonGroup5;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem5;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem6;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem7;

	private BarButtonGroup barButtonGroup6;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem8;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem9;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem10;

	private BarButtonGroup barButtonGroup7;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem31;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem32;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem11;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem2;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem12;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem33;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem34;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem35;

	private BarButtonGroup barButtonGroup8;

	private ChangeNumberFormatItem changeNumberFormatItem1;

	private RepositoryItemPopupGalleryEdit repositoryItemPopupGalleryEdit1;

	private BarButtonGroup barButtonGroup9;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem3;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem36;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem37;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem38;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem39;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem40;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem41;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem42;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem43;

	private BarButtonGroup barButtonGroup10;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem44;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem45;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem4;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem5;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem46;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem47;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem48;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem49;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem50;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem51;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem52;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem6;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem53;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem54;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem55;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem56;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem57;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem58;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown2;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem2;

	private CommandBarGalleryDropDown commandBarGalleryDropDown3;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem3;

	private CommandBarGalleryDropDown commandBarGalleryDropDown4;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem59;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem7;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem60;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem61;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem62;

	private GalleryFormatAsTableItem galleryFormatAsTableItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown5;

	private GalleryChangeStyleItem galleryChangeStyleItem1;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem8;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem63;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem64;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem65;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem9;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem66;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem67;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem68;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem10;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem69;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem70;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem71;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem72;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem73;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem11;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem74;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem75;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem76;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem77;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem78;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem79;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem80;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem81;

	private ChangeSheetTabColorItem changeSheetTabColorItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem82;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem13;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem83;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem12;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem84;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem85;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem86;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem87;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem88;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem13;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem89;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem90;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem91;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem92;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem14;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem93;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem94;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem95;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem96;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem97;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem98;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem15;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem99;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem100;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem14;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem101;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem102;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem16;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem103;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem104;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem105;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem106;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem107;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem108;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem109;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem110;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem111;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem112;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem4;

	private CommandBarGalleryDropDown commandBarGalleryDropDown6;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem5;

	private CommandBarGalleryDropDown commandBarGalleryDropDown7;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem6;

	private CommandBarGalleryDropDown commandBarGalleryDropDown8;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem7;

	private CommandBarGalleryDropDown commandBarGalleryDropDown9;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem8;

	private CommandBarGalleryDropDown commandBarGalleryDropDown10;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem9;

	private CommandBarGalleryDropDown commandBarGalleryDropDown11;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem10;

	private CommandBarGalleryDropDown commandBarGalleryDropDown12;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem113;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem114;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem17;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem15;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem16;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem17;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem115;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem18;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem18;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem19;

	private PageSetupPaperKindItem pageSetupPaperKindItem1;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem19;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem116;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem117;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem118;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem119;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem20;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem21;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem22;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem23;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem20;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem120;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem121;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem21;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem122;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem123;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem22;

	private FunctionsFinancialItem functionsFinancialItem1;

	private FunctionsLogicalItem functionsLogicalItem1;

	private FunctionsTextItem functionsTextItem1;

	private FunctionsDateAndTimeItem functionsDateAndTimeItem1;

	private FunctionsLookupAndReferenceItem functionsLookupAndReferenceItem1;

	private FunctionsMathAndTrigonometryItem functionsMathAndTrigonometryItem1;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem23;

	private FunctionsStatisticalItem functionsStatisticalItem1;

	private FunctionsEngineeringItem functionsEngineeringItem1;

	private FunctionsInformationItem functionsInformationItem1;

	private FunctionsCompatibilityItem functionsCompatibilityItem1;

	private FunctionsWebItem functionsWebItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem129;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem130;

	private DefinedNameListItem definedNameListItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem131;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem24;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem24;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem25;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem26;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem132;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem133;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem25;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem134;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem135;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem136;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem26;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem137;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem138;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem27;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem139;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem140;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem141;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem142;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem143;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem144;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem145;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem146;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem147;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem148;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem149;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem150;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem151;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem152;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem153;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem154;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem155;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem28;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem156;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem157;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem158;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem159;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem160;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem161;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem162;

	private GalleryChartLayoutItem galleryChartLayoutItem1;

	private GalleryChartStyleItem galleryChartStyleItem1;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem29;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem11;

	private CommandBarGalleryDropDown commandBarGalleryDropDown13;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem12;

	private CommandBarGalleryDropDown commandBarGalleryDropDown14;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem30;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem13;

	private CommandBarGalleryDropDown commandBarGalleryDropDown15;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem14;

	private CommandBarGalleryDropDown commandBarGalleryDropDown16;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem15;

	private CommandBarGalleryDropDown commandBarGalleryDropDown17;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem31;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem16;

	private CommandBarGalleryDropDown commandBarGalleryDropDown18;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem17;

	private CommandBarGalleryDropDown commandBarGalleryDropDown19;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem18;

	private CommandBarGalleryDropDown commandBarGalleryDropDown20;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem19;

	private CommandBarGalleryDropDown commandBarGalleryDropDown21;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem20;

	private CommandBarGalleryDropDown commandBarGalleryDropDown22;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem21;

	private CommandBarGalleryDropDown commandBarGalleryDropDown23;

	private SpreadsheetCommandBarButtonGalleryDropDownItem spreadsheetCommandBarButtonGalleryDropDownItem22;

	private CommandBarGalleryDropDown commandBarGalleryDropDown24;

	private RenameTableItemCaption renameTableItemCaption1;

	private RenameTableItem renameTableItem1;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem27;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem28;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem29;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem30;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem31;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem32;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem33;

	private GalleryTableStylesItem galleryTableStylesItem1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem163;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem164;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem165;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem166;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem167;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem168;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem169;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem32;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem170;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem171;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem172;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem33;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem173;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem174;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem34;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem175;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem176;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem177;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem178;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem35;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem179;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem180;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem181;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem182;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem34;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem35;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem36;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem36;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem183;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem184;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem185;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem37;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem186;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem187;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem188;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem189;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem38;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem190;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem191;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem192;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem193;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem194;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem39;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem195;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem196;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem37;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem38;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem39;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem40;

	private GalleryPivotStylesItem galleryPivotStylesItem1;

	private ChartToolsRibbonPageCategory chartToolsRibbonPageCategory1;

	private ChartsDesignRibbonPage chartsDesignRibbonPage1;

	private ChartsDesignTypeRibbonPageGroup chartsDesignTypeRibbonPageGroup1;

	private ChartsDesignDataRibbonPageGroup chartsDesignDataRibbonPageGroup1;

	private ChartsDesignLayoutsRibbonPageGroup chartsDesignLayoutsRibbonPageGroup1;

	private ChartsDesignStylesRibbonPageGroup chartsDesignStylesRibbonPageGroup1;

	private ChartsLayoutRibbonPage chartsLayoutRibbonPage1;

	private ChartsLayoutAxesRibbonPageGroup chartsLayoutAxesRibbonPageGroup1;

	private ChartsLayoutLabelsRibbonPageGroup chartsLayoutLabelsRibbonPageGroup1;

	private ChartsLayoutAnalysisRibbonPageGroup chartsLayoutAnalysisRibbonPageGroup1;

	private ChartsFormatRibbonPage chartsFormatRibbonPage1;

	private ChartsFormatArrangeRibbonPageGroup chartsFormatArrangeRibbonPageGroup1;

	private TableToolsRibbonPageCategory tableToolsRibbonPageCategory1;

	private TableToolsDesignRibbonPage tableToolsDesignRibbonPage1;

	private TablePropertiesRibbonPageGroup tablePropertiesRibbonPageGroup1;

	private TableToolsRibbonPageGroup tableToolsRibbonPageGroup1;

	private TableStyleOptionsRibbonPageGroup tableStyleOptionsRibbonPageGroup1;

	private TableStylesRibbonPageGroup tableStylesRibbonPageGroup1;

	private DrawingToolsRibbonPageCategory drawingToolsRibbonPageCategory1;

	private DrawingFormatRibbonPage drawingFormatRibbonPage1;

	private DrawingFormatArrangeRibbonPageGroup drawingFormatArrangeRibbonPageGroup1;

	private PictureToolsRibbonPageCategory pictureToolsRibbonPageCategory1;

	private PictureFormatRibbonPage pictureFormatRibbonPage1;

	private PictureFormatArrangeRibbonPageGroup pictureFormatArrangeRibbonPageGroup1;

	private PivotTableToolsRibbonPageCategory pivotTableToolsRibbonPageCategory1;

	private PivotTableAnalyzeRibbonPage pivotTableAnalyzeRibbonPage1;

	private PivotTableAnalyzePivotTableRibbonPageGroup pivotTableAnalyzePivotTableRibbonPageGroup1;

	private PivotTableAnalyzeActiveFieldRibbonPageGroup pivotTableAnalyzeActiveFieldRibbonPageGroup1;

	private PivotTableAnalyzeGroupRibbonPageGroup pivotTableAnalyzeGroupRibbonPageGroup1;

	private PivotTableAnalyzeDataRibbonPageGroup pivotTableAnalyzeDataRibbonPageGroup1;

	private PivotTableAnalyzeActionsRibbonPageGroup pivotTableAnalyzeActionsRibbonPageGroup1;

	private PivotTableAnalyzeCalculationsRibbonPageGroup pivotTableAnalyzeCalculationsRibbonPageGroup1;

	private PivotTableAnalyzeShowRibbonPageGroup pivotTableAnalyzeShowRibbonPageGroup1;

	private PivotTableDesignRibbonPage pivotTableDesignRibbonPage1;

	private PivotTableDesignLayoutRibbonPageGroup pivotTableDesignLayoutRibbonPageGroup1;

	private PivotTableDesignPivotTableStyleOptionsRibbonPageGroup pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1;

	private PivotTableDesignPivotTableStylesRibbonPageGroup pivotTableDesignPivotTableStylesRibbonPageGroup1;

	private FileRibbonPage fileRibbonPage1;

	private CommonRibbonPageGroup commonRibbonPageGroup1;

	private InfoRibbonPageGroup infoRibbonPageGroup1;

	private HomeRibbonPage homeRibbonPage1;

	private ClipboardRibbonPageGroup clipboardRibbonPageGroup1;

	private FontRibbonPageGroup fontRibbonPageGroup1;

	private AlignmentRibbonPageGroup alignmentRibbonPageGroup1;

	private NumberRibbonPageGroup numberRibbonPageGroup1;

	private StylesRibbonPageGroup stylesRibbonPageGroup1;

	private CellsRibbonPageGroup cellsRibbonPageGroup1;

	private EditingRibbonPageGroup editingRibbonPageGroup1;

	private InsertRibbonPage insertRibbonPage1;

	private TablesRibbonPageGroup tablesRibbonPageGroup1;

	private IllustrationsRibbonPageGroup illustrationsRibbonPageGroup1;

	private ChartsRibbonPageGroup chartsRibbonPageGroup1;

	private LinksRibbonPageGroup linksRibbonPageGroup1;

	private SymbolsRibbonPageGroup symbolsRibbonPageGroup1;

	private PageLayoutRibbonPage pageLayoutRibbonPage1;

	private PageSetupRibbonPageGroup pageSetupRibbonPageGroup1;

	private PageSetupShowRibbonPageGroup pageSetupShowRibbonPageGroup1;

	private PageSetupPrintRibbonPageGroup pageSetupPrintRibbonPageGroup1;

	private ArrangeRibbonPageGroup arrangeRibbonPageGroup1;

	private FormulasRibbonPage formulasRibbonPage1;

	private FunctionLibraryRibbonPageGroup functionLibraryRibbonPageGroup1;

	private FormulaDefinedNamesRibbonPageGroup formulaDefinedNamesRibbonPageGroup1;

	private FormulaAuditingRibbonPageGroup formulaAuditingRibbonPageGroup1;

	private FormulaCalculationRibbonPageGroup formulaCalculationRibbonPageGroup1;

	private DataRibbonPage dataRibbonPage1;

	private SortAndFilterRibbonPageGroup sortAndFilterRibbonPageGroup1;

	private DataToolsRibbonPageGroup dataToolsRibbonPageGroup1;

	private OutlineRibbonPageGroup outlineRibbonPageGroup1;

	private ReviewRibbonPage reviewRibbonPage1;

	private CommentsRibbonPageGroup commentsRibbonPageGroup1;

	private ChangesRibbonPageGroup changesRibbonPageGroup1;

	private ViewRibbonPage viewRibbonPage1;

	private ShowRibbonPageGroup showRibbonPageGroup1;

	private ZoomRibbonPageGroup zoomRibbonPageGroup1;

	private WindowRibbonPageGroup windowRibbonPageGroup1;

	private SpreadsheetFormulaBarControl spreadsheetFormulaBarControl1;

	private SpreadsheetNameBoxControl spreadsheetNameBoxControl1;

	private SplitContainerControl splitContainerControl1;

	private SplitterControl splitterControl1;

	private SpreadsheetBarController spreadsheetBarController1;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem124;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem125;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem126;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem127;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem128;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem197;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem40;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem198;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem199;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem41;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem200;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem201;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem202;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem41;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem42;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem43;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem42;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem44;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem45;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem203;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem204;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem205;

	private SpreadsheetCommandBarSubItem spreadsheetCommandBarSubItem43;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem206;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem207;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem208;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem209;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem210;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem211;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem212;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem213;

	private SpreadsheetCommandBarCheckItem spreadsheetCommandBarCheckItem46;

	private SpreadsheetCommandBarButtonItem spreadsheetCommandBarButtonItem214;

	private MailMergeRibbonPage mailMergeRibbonPage1;

	private MailMergeDataRibbonPageGroup mailMergeDataRibbonPageGroup1;

	private MailMergeModeRibbonPageGroup mailMergeModeRibbonPageGroup1;

	private MailMergeExtendedRibbonPageGroup mailMergeExtendedRibbonPageGroup1;

	private MailMergeGroupingRibbonPageGroup mailMergeGroupingRibbonPageGroup1;

	private MailMergeFilteringRibbonPageGroup mailMergeFilteringRibbonPageGroup1;

	private MailMergeBindingRibbonPageGroup mailMergeBindingRibbonPageGroup1;

	private SpreadsheetDockManager spreadsheetDockManager1;

	private DockPanel panelContainer1;

	private FieldListDockPanel fieldListDockPanel1;

	private ControlContainer fieldListDockPanel1_Container;

	private MailMergeParametersDockPanel mailMergeParametersDockPanel1;

	private ControlContainer mailMergeParametersDockPanel1_Container;

	public frm_Speadsheet()
	{
		InitializeComponent();
	}

	public frm_Speadsheet(DataTable dt, DataTable dtStructure = null, string caption = "")
	{
		InitializeComponent();
		data = dt.Copy();
		structure = dtStructure.Copy();
		title = caption;
	}

	private void frm_Speadsheet_Load(object sender, EventArgs e)
	{
		try
		{
			workbook = spreadsheetControl1.Document;
			Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];
			int num = 0;
			int num2 = 0;
			worksheet.Cells[0, 0].Value = title;
			workbook.Unit = DocumentUnit.Point;
			worksheet.Rows[0].Height = 40.0;
			SpreadsheetFont font = worksheet.Cells[0, 0].Font;
			font.Size = 20.0;
			font.Bold = true;
			int columnOffset = 1;
			int num3 = 2;
			int columnOffset2 = 2;
			int num4 = 2;
			int num5 = 0;
			if (structure.IsNotNull())
			{
				DataRow[] array = structure.Select("Fixed= 'H'");
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string columnName = Function.ToString(array[i]["ColumnName"]);
						worksheet.Cells[num3, columnOffset].Value = Function.ToString(array[i]["Caption"]);
						worksheet.Cells[num4, columnOffset2].Value = Function.ToString(data.Rows[0][columnName]);
						if (num5 % 2 == 0 && num5 > 0)
						{
							columnOffset = 1;
							num3++;
							columnOffset2 = 2;
							num4++;
						}
						else if (num5 % 2 > 0 && num5 > 0)
						{
							columnOffset = 4;
							columnOffset2 = 5;
						}
						num5++;
					}
				}
			}
			num = ((num3 > 2) ? (num3 + 2) : num3);
			if (structure.IsNotNull())
			{
				int num6 = 0;
				foreach (DataRow row in structure.Rows)
				{
					string text = Function.ToString(row["ColumnName"]);
					string columnName2 = Function.ToString(row["Caption"]);
					if (!Function.ParseBool(row["Visible"]) && data.IsExistsColumn(text))
					{
						data.Columns.Remove(text);
					}
					else if (data.IsExistsColumn(text))
					{
						data.Columns[text].SetOrdinal(num6);
						num6++;
						data.Columns[text].ColumnName = columnName2;
					}
				}
			}
			worksheet.Import(data, addHeader: true, num, num2);
			for (int j = num2; j < data.Columns.Count + num2; j++)
			{
				Cell cell = worksheet.Cells[num, j];
				cell.RowHeight = 30.0;
				cell.FillColor = Color.LightBlue;
				cell.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
				cell.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
				SpreadsheetFont font2 = cell.Font;
				font2.Size = 12.0;
				font2.Bold = true;
			}
			CellRange cellRange = worksheet.Range.FromLTRB(num2, num, data.Columns.Count - 1, data.Rows.Count + num);
			cellRange.AutoFitColumns();
			cellRange.Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
			CellRange cellRange2 = worksheet.Range.FromLTRB(num2, 0, data.Columns.Count - 1, 0);
			cellRange2.Merge();
			cellRange2.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
			cellRange2.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
			workbook.History.Clear();
			workbook.MailMergeDataSource = data;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spreadsheetControl1_DocumentLoaded(object sender, EventArgs e)
	{
		try
		{
			if (data.IsNotNull())
			{
				spreadsheetControl1.Document.MailMergeDataSource = data;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void spreadsheetControl1_EmptyDocumentCreated(object sender, EventArgs e)
	{
		try
		{
			if (data.IsNotNull())
			{
				spreadsheetControl1.Document.MailMergeDataSource = data;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem107 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem108 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem109 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem110 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem111 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem112 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem113 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem114 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem115 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem116 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem117 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem118 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem119 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem120 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem121 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem122 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem123 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem124 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem125 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem126 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem127 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem128 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem129 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem130 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem131 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem132 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem133 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem134 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem135 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem136 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem137 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem138 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem139 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem140 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem141 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem142 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem143 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem144 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem145 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem146 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem147 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem148 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem149 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem150 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem151 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem152 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem153 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem154 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem155 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem156 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem157 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem158 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem159 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem160 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem161 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem162 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem163 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem164 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem165 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem166 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem167 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem168 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem169 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem170 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem171 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem172 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem173 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem174 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem175 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem176 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem177 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem178 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem179 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem180 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem181 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem182 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup spreadsheetCommandGalleryItemGroup40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItemGroup();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem183 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem184 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem185 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem spreadsheetCommandGalleryItem186 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandGalleryItem();
		DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation = new DevExpress.XtraBars.Ribbon.ReduceOperation();
		this.stylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.StylesRibbonPageGroup();
		this.spreadsheetCommandBarSubItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarSubItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem47 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem48 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem49 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem50 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem51 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem52 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem53 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem54 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem55 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem56 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem57 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem58 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown2 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.spreadsheetCommandBarButtonItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeFontNameItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontNameItem();
		this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
		this.changeFontSizeItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontSizeItem();
		this.repositoryItemSpreadsheetFontSizeEdit1 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetFontSizeEdit();
		this.spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
		this.spreadsheetCommandBarButtonItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.barButtonGroup2 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarCheckItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.barButtonGroup3 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarSubItem1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.changeBorderLineColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineColorItem();
		this.changeBorderLineStyleItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeBorderLineStyleItem();
		this.commandBarGalleryDropDown1 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.barButtonGroup4 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeCellFillColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeCellFillColorItem();
		this.changeFontColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeFontColorItem();
		this.barButtonGroup5 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarCheckItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.barButtonGroup6 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarCheckItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.barButtonGroup7 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarButtonItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.barButtonGroup8 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeNumberFormatItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeNumberFormatItem();
		this.repositoryItemPopupGalleryEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupGalleryEdit();
		this.barButtonGroup9 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarSubItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.barButtonGroup10 = new DevExpress.XtraBars.BarButtonGroup();
		this.spreadsheetCommandBarButtonItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem2 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown3 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem3 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown4 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonItem59 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem60 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem61 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem62 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.galleryFormatAsTableItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryFormatAsTableItem();
		this.commandBarGalleryDropDown5 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.galleryChangeStyleItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChangeStyleItem();
		this.spreadsheetCommandBarSubItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem63 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem64 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem65 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem66 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem67 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem68 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem69 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem70 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem71 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem72 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem73 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem74 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem75 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem76 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem77 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem78 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem79 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem80 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem81 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.changeSheetTabColorItem1 = new DevExpress.XtraSpreadsheet.UI.ChangeSheetTabColorItem();
		this.spreadsheetCommandBarButtonItem82 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem83 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem84 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem85 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem86 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem87 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem88 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem89 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem90 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem91 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem92 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem93 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem94 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem95 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem96 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem97 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem98 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem99 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem100 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem101 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem102 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem103 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem104 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem105 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem106 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem107 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem108 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem109 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem110 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem111 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem112 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem4 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown6 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem5 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown7 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem6 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown8 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem7 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown9 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem8 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown10 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem9 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown11 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem10 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown12 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonItem113 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem114 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem115 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.pageSetupPaperKindItem1 = new DevExpress.XtraSpreadsheet.UI.PageSetupPaperKindItem();
		this.spreadsheetCommandBarSubItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem116 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem117 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem118 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem119 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem120 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem121 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem122 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem123 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.functionsFinancialItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsFinancialItem();
		this.functionsLogicalItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsLogicalItem();
		this.functionsTextItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsTextItem();
		this.functionsDateAndTimeItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsDateAndTimeItem();
		this.functionsLookupAndReferenceItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsLookupAndReferenceItem();
		this.functionsMathAndTrigonometryItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsMathAndTrigonometryItem();
		this.spreadsheetCommandBarSubItem23 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.functionsStatisticalItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsStatisticalItem();
		this.functionsEngineeringItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsEngineeringItem();
		this.functionsInformationItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsInformationItem();
		this.functionsCompatibilityItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsCompatibilityItem();
		this.functionsWebItem1 = new DevExpress.XtraSpreadsheet.UI.FunctionsWebItem();
		this.spreadsheetCommandBarButtonItem129 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem130 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.definedNameListItem1 = new DevExpress.XtraSpreadsheet.UI.DefinedNameListItem();
		this.spreadsheetCommandBarButtonItem131 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem24 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem132 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem133 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem25 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem134 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem135 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem136 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem26 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem137 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem138 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem139 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem140 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem141 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem142 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem143 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem144 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem145 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem146 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem147 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem148 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem149 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem150 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem151 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem152 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem153 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem154 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem155 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem156 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem157 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem158 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem159 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem160 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem161 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem162 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.galleryChartLayoutItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChartLayoutItem();
		this.galleryChartStyleItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryChartStyleItem();
		this.spreadsheetCommandBarSubItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem11 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown13 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem12 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown14 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarSubItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem13 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown15 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem14 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown16 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem15 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown17 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarSubItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonGalleryDropDownItem16 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown18 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem17 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown19 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem18 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown20 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem19 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown21 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem20 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown22 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem21 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown23 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.spreadsheetCommandBarButtonGalleryDropDownItem22 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonGalleryDropDownItem();
		this.commandBarGalleryDropDown24 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.renameTableItemCaption1 = new DevExpress.XtraSpreadsheet.UI.RenameTableItemCaption();
		this.renameTableItem1 = new DevExpress.XtraSpreadsheet.UI.RenameTableItem();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.spreadsheetCommandBarCheckItem27 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem28 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem29 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem30 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem31 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.galleryTableStylesItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryTableStylesItem();
		this.spreadsheetCommandBarButtonItem163 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem164 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem165 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem166 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem167 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem168 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem169 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem32 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem170 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem171 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem172 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem33 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem173 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem174 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem175 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem176 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem177 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem178 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem179 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem180 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem181 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem182 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem34 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem35 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem36 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem183 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem184 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem185 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem186 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem187 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem188 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem189 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem190 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem191 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem192 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem193 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem194 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem195 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem196 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem37 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem38 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem39 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.galleryPivotStylesItem1 = new DevExpress.XtraSpreadsheet.UI.GalleryPivotStylesItem();
		this.spreadsheetCommandBarButtonItem197 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem40 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem198 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem199 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem200 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem201 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem202 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem41 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarSubItem42 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarCheckItem44 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarCheckItem45 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem203 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem204 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem205 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarSubItem43 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarSubItem();
		this.spreadsheetCommandBarButtonItem206 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem207 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem208 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem209 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem210 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem211 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem212 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem213 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarCheckItem46 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarCheckItem();
		this.spreadsheetCommandBarButtonItem214 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.chartToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.ChartToolsRibbonPageCategory();
		this.chartsDesignRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignRibbonPage();
		this.chartsDesignTypeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignTypeRibbonPageGroup();
		this.chartsDesignDataRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignDataRibbonPageGroup();
		this.chartsDesignLayoutsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignLayoutsRibbonPageGroup();
		this.chartsDesignStylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsDesignStylesRibbonPageGroup();
		this.chartsLayoutRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutRibbonPage();
		this.chartsLayoutAxesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutAxesRibbonPageGroup();
		this.chartsLayoutLabelsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutLabelsRibbonPageGroup();
		this.chartsLayoutAnalysisRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsLayoutAnalysisRibbonPageGroup();
		this.chartsFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ChartsFormatRibbonPage();
		this.chartsFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsFormatArrangeRibbonPageGroup();
		this.tableToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.TableToolsRibbonPageCategory();
		this.tableToolsDesignRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.TableToolsDesignRibbonPage();
		this.tablePropertiesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TablePropertiesRibbonPageGroup();
		this.tableToolsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableToolsRibbonPageGroup();
		this.tableStyleOptionsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableStyleOptionsRibbonPageGroup();
		this.tableStylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TableStylesRibbonPageGroup();
		this.drawingToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.DrawingToolsRibbonPageCategory();
		this.drawingFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.DrawingFormatRibbonPage();
		this.drawingFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.DrawingFormatArrangeRibbonPageGroup();
		this.pictureToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.PictureToolsRibbonPageCategory();
		this.pictureFormatRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PictureFormatRibbonPage();
		this.pictureFormatArrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PictureFormatArrangeRibbonPageGroup();
		this.pivotTableToolsRibbonPageCategory1 = new DevExpress.XtraSpreadsheet.UI.PivotTableToolsRibbonPageCategory();
		this.pivotTableAnalyzeRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeRibbonPage();
		this.pivotTableAnalyzePivotTableRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzePivotTableRibbonPageGroup();
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeActiveFieldRibbonPageGroup();
		this.pivotTableAnalyzeGroupRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeGroupRibbonPageGroup();
		this.pivotTableAnalyzeDataRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeDataRibbonPageGroup();
		this.pivotTableAnalyzeActionsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeActionsRibbonPageGroup();
		this.pivotTableAnalyzeCalculationsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeCalculationsRibbonPageGroup();
		this.pivotTableAnalyzeShowRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableAnalyzeShowRibbonPageGroup();
		this.pivotTableDesignRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PivotTableDesignRibbonPage();
		this.pivotTableDesignLayoutRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableDesignLayoutRibbonPageGroup();
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableDesignPivotTableStyleOptionsRibbonPageGroup();
		this.pivotTableDesignPivotTableStylesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PivotTableDesignPivotTableStylesRibbonPageGroup();
		this.fileRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.FileRibbonPage();
		this.commonRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CommonRibbonPageGroup();
		this.infoRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.InfoRibbonPageGroup();
		this.homeRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.HomeRibbonPage();
		this.clipboardRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ClipboardRibbonPageGroup();
		this.fontRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FontRibbonPageGroup();
		this.alignmentRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.AlignmentRibbonPageGroup();
		this.numberRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.NumberRibbonPageGroup();
		this.cellsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CellsRibbonPageGroup();
		this.editingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.EditingRibbonPageGroup();
		this.insertRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.InsertRibbonPage();
		this.tablesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.TablesRibbonPageGroup();
		this.illustrationsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.IllustrationsRibbonPageGroup();
		this.chartsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChartsRibbonPageGroup();
		this.linksRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.LinksRibbonPageGroup();
		this.symbolsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.SymbolsRibbonPageGroup();
		this.pageLayoutRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.PageLayoutRibbonPage();
		this.pageSetupRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupRibbonPageGroup();
		this.pageSetupShowRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupShowRibbonPageGroup();
		this.pageSetupPrintRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.PageSetupPrintRibbonPageGroup();
		this.arrangeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ArrangeRibbonPageGroup();
		this.formulasRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.FormulasRibbonPage();
		this.functionLibraryRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FunctionLibraryRibbonPageGroup();
		this.formulaDefinedNamesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaDefinedNamesRibbonPageGroup();
		this.formulaAuditingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaAuditingRibbonPageGroup();
		this.formulaCalculationRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.FormulaCalculationRibbonPageGroup();
		this.dataRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.DataRibbonPage();
		this.sortAndFilterRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.SortAndFilterRibbonPageGroup();
		this.dataToolsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.DataToolsRibbonPageGroup();
		this.outlineRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.OutlineRibbonPageGroup();
		this.reviewRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ReviewRibbonPage();
		this.commentsRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.CommentsRibbonPageGroup();
		this.changesRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ChangesRibbonPageGroup();
		this.viewRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.ViewRibbonPage();
		this.showRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ShowRibbonPageGroup();
		this.zoomRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.ZoomRibbonPageGroup();
		this.windowRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.WindowRibbonPageGroup();
		this.mailMergeRibbonPage1 = new DevExpress.XtraSpreadsheet.UI.MailMergeRibbonPage();
		this.mailMergeDataRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeDataRibbonPageGroup();
		this.mailMergeModeRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeModeRibbonPageGroup();
		this.mailMergeExtendedRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeExtendedRibbonPageGroup();
		this.mailMergeGroupingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeGroupingRibbonPageGroup();
		this.mailMergeFilteringRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeFilteringRibbonPageGroup();
		this.mailMergeBindingRibbonPageGroup1 = new DevExpress.XtraSpreadsheet.UI.MailMergeBindingRibbonPageGroup();
		this.spreadsheetFormulaBarControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetFormulaBarControl();
		this.spreadsheetNameBoxControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetNameBoxControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
		this.spreadsheetBarController1 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController(this.components);
		this.spreadsheetCommandBarButtonItem124 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem125 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem126 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem127 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetCommandBarButtonItem128 = new DevExpress.XtraSpreadsheet.UI.SpreadsheetCommandBarButtonItem();
		this.spreadsheetDockManager1 = new DevExpress.XtraSpreadsheet.SpreadsheetDockManager(this.components);
		this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
		this.fieldListDockPanel1 = new DevExpress.XtraSpreadsheet.FieldListDockPanel();
		this.fieldListDockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.mailMergeParametersDockPanel1 = new DevExpress.XtraSpreadsheet.MailMergeParametersDockPanel();
		this.mailMergeParametersDockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetFontSizeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPopupGalleryEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetNameBoxControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetBarController1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetDockManager1).BeginInit();
		this.panelContainer1.SuspendLayout();
		this.fieldListDockPanel1.SuspendLayout();
		this.mailMergeParametersDockPanel1.SuspendLayout();
		base.SuspendLayout();
		this.stylesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.stylesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem4);
		this.stylesRibbonPageGroup1.ItemLinks.Add(this.galleryFormatAsTableItem1);
		this.stylesRibbonPageGroup1.ItemLinks.Add(this.galleryChangeStyleItem1);
		this.stylesRibbonPageGroup1.Name = "stylesRibbonPageGroup1";
		this.spreadsheetCommandBarSubItem4.CommandName = "ConditionalFormattingCommandGroup";
		this.spreadsheetCommandBarSubItem4.Id = 78;
		this.spreadsheetCommandBarSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem59),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem62)
		});
		this.spreadsheetCommandBarSubItem4.Name = "spreadsheetCommandBarSubItem4";
		this.spreadsheetCommandBarSubItem5.CommandName = "ConditionalFormattingHighlightCellsRuleCommandGroup";
		this.spreadsheetCommandBarSubItem5.Id = 86;
		this.spreadsheetCommandBarSubItem5.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem46),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem47),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem48),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem49),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem50),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem51),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem52)
		});
		this.spreadsheetCommandBarSubItem5.Name = "spreadsheetCommandBarSubItem5";
		this.spreadsheetCommandBarButtonItem46.CommandName = "ConditionalFormattingGreaterThanRuleCommand";
		this.spreadsheetCommandBarButtonItem46.Id = 79;
		this.spreadsheetCommandBarButtonItem46.Name = "spreadsheetCommandBarButtonItem46";
		this.spreadsheetCommandBarButtonItem46.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem47.CommandName = "ConditionalFormattingLessThanRuleCommand";
		this.spreadsheetCommandBarButtonItem47.Id = 80;
		this.spreadsheetCommandBarButtonItem47.Name = "spreadsheetCommandBarButtonItem47";
		this.spreadsheetCommandBarButtonItem47.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem48.CommandName = "ConditionalFormattingBetweenRuleCommand";
		this.spreadsheetCommandBarButtonItem48.Id = 81;
		this.spreadsheetCommandBarButtonItem48.Name = "spreadsheetCommandBarButtonItem48";
		this.spreadsheetCommandBarButtonItem48.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem49.CommandName = "ConditionalFormattingEqualToRuleCommand";
		this.spreadsheetCommandBarButtonItem49.Id = 82;
		this.spreadsheetCommandBarButtonItem49.Name = "spreadsheetCommandBarButtonItem49";
		this.spreadsheetCommandBarButtonItem49.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem50.CommandName = "ConditionalFormattingTextContainsRuleCommand";
		this.spreadsheetCommandBarButtonItem50.Id = 83;
		this.spreadsheetCommandBarButtonItem50.Name = "spreadsheetCommandBarButtonItem50";
		this.spreadsheetCommandBarButtonItem50.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem51.CommandName = "ConditionalFormattingDateOccurringRuleCommand";
		this.spreadsheetCommandBarButtonItem51.Id = 84;
		this.spreadsheetCommandBarButtonItem51.Name = "spreadsheetCommandBarButtonItem51";
		this.spreadsheetCommandBarButtonItem51.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem52.CommandName = "ConditionalFormattingDuplicateValuesRuleCommand";
		this.spreadsheetCommandBarButtonItem52.Id = 85;
		this.spreadsheetCommandBarButtonItem52.Name = "spreadsheetCommandBarButtonItem52";
		this.spreadsheetCommandBarButtonItem52.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem6.CommandName = "ConditionalFormattingTopBottomRuleCommandGroup";
		this.spreadsheetCommandBarSubItem6.Id = 93;
		this.spreadsheetCommandBarSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem53),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem54),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem55),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem56),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem57),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem58)
		});
		this.spreadsheetCommandBarSubItem6.Name = "spreadsheetCommandBarSubItem6";
		this.spreadsheetCommandBarButtonItem53.CommandName = "ConditionalFormattingTop10RuleCommand";
		this.spreadsheetCommandBarButtonItem53.Id = 87;
		this.spreadsheetCommandBarButtonItem53.Name = "spreadsheetCommandBarButtonItem53";
		this.spreadsheetCommandBarButtonItem53.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem54.CommandName = "ConditionalFormattingTop10PercentRuleCommand";
		this.spreadsheetCommandBarButtonItem54.Id = 88;
		this.spreadsheetCommandBarButtonItem54.Name = "spreadsheetCommandBarButtonItem54";
		this.spreadsheetCommandBarButtonItem54.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem55.CommandName = "ConditionalFormattingBottom10RuleCommand";
		this.spreadsheetCommandBarButtonItem55.Id = 89;
		this.spreadsheetCommandBarButtonItem55.Name = "spreadsheetCommandBarButtonItem55";
		this.spreadsheetCommandBarButtonItem55.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem56.CommandName = "ConditionalFormattingBottom10PercentRuleCommand";
		this.spreadsheetCommandBarButtonItem56.Id = 90;
		this.spreadsheetCommandBarButtonItem56.Name = "spreadsheetCommandBarButtonItem56";
		this.spreadsheetCommandBarButtonItem56.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem57.CommandName = "ConditionalFormattingAboveAverageRuleCommand";
		this.spreadsheetCommandBarButtonItem57.Id = 91;
		this.spreadsheetCommandBarButtonItem57.Name = "spreadsheetCommandBarButtonItem57";
		this.spreadsheetCommandBarButtonItem57.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem58.CommandName = "ConditionalFormattingBelowAverageRuleCommand";
		this.spreadsheetCommandBarButtonItem58.Id = 92;
		this.spreadsheetCommandBarButtonItem58.Name = "spreadsheetCommandBarButtonItem58";
		this.spreadsheetCommandBarButtonItem58.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.CommandName = "ConditionalFormattingDataBarsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.DropDownControl = this.commandBarGalleryDropDown2;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.Id = 94;
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.Name = "spreadsheetCommandBarButtonGalleryDropDownItem1";
		this.spreadsheetCommandBarButtonGalleryDropDownItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown2.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup.CommandName = "ConditionalFormattingDataBarsGradientFillCommandGroup";
		spreadsheetCommandGalleryItem.CommandName = "ConditionalFormattingDataBarGradientBlue";
		spreadsheetCommandGalleryItem2.CommandName = "ConditionalFormattingDataBarGradientGreen";
		spreadsheetCommandGalleryItem3.CommandName = "ConditionalFormattingDataBarGradientRed";
		spreadsheetCommandGalleryItem4.CommandName = "ConditionalFormattingDataBarGradientOrange";
		spreadsheetCommandGalleryItem5.CommandName = "ConditionalFormattingDataBarGradientLightBlue";
		spreadsheetCommandGalleryItem6.CommandName = "ConditionalFormattingDataBarGradientPurple";
		spreadsheetCommandGalleryItemGroup.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem, spreadsheetCommandGalleryItem2, spreadsheetCommandGalleryItem3, spreadsheetCommandGalleryItem4, spreadsheetCommandGalleryItem5, spreadsheetCommandGalleryItem6 });
		spreadsheetCommandGalleryItemGroup2.CommandName = "ConditionalFormattingDataBarsSolidFillCommandGroup";
		spreadsheetCommandGalleryItem7.CommandName = "ConditionalFormattingDataBarSolidBlue";
		spreadsheetCommandGalleryItem8.CommandName = "ConditionalFormattingDataBarSolidGreen";
		spreadsheetCommandGalleryItem9.CommandName = "ConditionalFormattingDataBarSolidRed";
		spreadsheetCommandGalleryItem10.CommandName = "ConditionalFormattingDataBarSolidOrange";
		spreadsheetCommandGalleryItem11.CommandName = "ConditionalFormattingDataBarSolidLightBlue";
		spreadsheetCommandGalleryItem12.CommandName = "ConditionalFormattingDataBarSolidPurple";
		spreadsheetCommandGalleryItemGroup2.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem7, spreadsheetCommandGalleryItem8, spreadsheetCommandGalleryItem9, spreadsheetCommandGalleryItem10, spreadsheetCommandGalleryItem11, spreadsheetCommandGalleryItem12 });
		this.commandBarGalleryDropDown2.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup, spreadsheetCommandGalleryItemGroup2 });
		this.commandBarGalleryDropDown2.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown2.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown2.Name = "commandBarGalleryDropDown2";
		this.commandBarGalleryDropDown2.Ribbon = this.ribbonControl1;
		this.ribbonControl1.ExpandCollapseItem.Id = 0;
		this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[361]
		{
			this.ribbonControl1.ExpandCollapseItem,
			this.ribbonControl1.SearchEditItem,
			this.spreadsheetCommandBarButtonItem1,
			this.spreadsheetCommandBarButtonItem2,
			this.spreadsheetCommandBarButtonItem3,
			this.spreadsheetCommandBarButtonItem4,
			this.spreadsheetCommandBarButtonItem5,
			this.spreadsheetCommandBarButtonItem6,
			this.spreadsheetCommandBarButtonItem7,
			this.spreadsheetCommandBarButtonItem8,
			this.spreadsheetCommandBarButtonItem9,
			this.spreadsheetCommandBarButtonItem10,
			this.spreadsheetCommandBarButtonItem11,
			this.spreadsheetCommandBarButtonItem12,
			this.spreadsheetCommandBarButtonItem13,
			this.spreadsheetCommandBarButtonItem14,
			this.spreadsheetCommandBarButtonItem15,
			this.barButtonGroup1,
			this.changeFontNameItem1,
			this.changeFontSizeItem1,
			this.spreadsheetCommandBarButtonItem16,
			this.spreadsheetCommandBarButtonItem17,
			this.barButtonGroup2,
			this.spreadsheetCommandBarCheckItem1,
			this.spreadsheetCommandBarCheckItem2,
			this.spreadsheetCommandBarCheckItem3,
			this.spreadsheetCommandBarCheckItem4,
			this.barButtonGroup3,
			this.spreadsheetCommandBarSubItem1,
			this.spreadsheetCommandBarButtonItem18,
			this.spreadsheetCommandBarButtonItem19,
			this.spreadsheetCommandBarButtonItem20,
			this.spreadsheetCommandBarButtonItem21,
			this.spreadsheetCommandBarButtonItem22,
			this.spreadsheetCommandBarButtonItem23,
			this.spreadsheetCommandBarButtonItem24,
			this.spreadsheetCommandBarButtonItem25,
			this.spreadsheetCommandBarButtonItem26,
			this.spreadsheetCommandBarButtonItem27,
			this.spreadsheetCommandBarButtonItem28,
			this.spreadsheetCommandBarButtonItem29,
			this.spreadsheetCommandBarButtonItem30,
			this.changeBorderLineColorItem1,
			this.changeBorderLineStyleItem1,
			this.barButtonGroup4,
			this.changeCellFillColorItem1,
			this.changeFontColorItem1,
			this.barButtonGroup5,
			this.spreadsheetCommandBarCheckItem5,
			this.spreadsheetCommandBarCheckItem6,
			this.spreadsheetCommandBarCheckItem7,
			this.barButtonGroup6,
			this.spreadsheetCommandBarCheckItem8,
			this.spreadsheetCommandBarCheckItem9,
			this.spreadsheetCommandBarCheckItem10,
			this.barButtonGroup7,
			this.spreadsheetCommandBarButtonItem31,
			this.spreadsheetCommandBarButtonItem32,
			this.spreadsheetCommandBarCheckItem11,
			this.spreadsheetCommandBarSubItem2,
			this.spreadsheetCommandBarCheckItem12,
			this.spreadsheetCommandBarButtonItem33,
			this.spreadsheetCommandBarButtonItem34,
			this.spreadsheetCommandBarButtonItem35,
			this.barButtonGroup8,
			this.changeNumberFormatItem1,
			this.barButtonGroup9,
			this.spreadsheetCommandBarSubItem3,
			this.spreadsheetCommandBarButtonItem36,
			this.spreadsheetCommandBarButtonItem37,
			this.spreadsheetCommandBarButtonItem38,
			this.spreadsheetCommandBarButtonItem39,
			this.spreadsheetCommandBarButtonItem40,
			this.spreadsheetCommandBarButtonItem41,
			this.spreadsheetCommandBarButtonItem42,
			this.spreadsheetCommandBarButtonItem43,
			this.barButtonGroup10,
			this.spreadsheetCommandBarButtonItem44,
			this.spreadsheetCommandBarButtonItem45,
			this.spreadsheetCommandBarSubItem4,
			this.spreadsheetCommandBarButtonItem46,
			this.spreadsheetCommandBarButtonItem47,
			this.spreadsheetCommandBarButtonItem48,
			this.spreadsheetCommandBarButtonItem49,
			this.spreadsheetCommandBarButtonItem50,
			this.spreadsheetCommandBarButtonItem51,
			this.spreadsheetCommandBarButtonItem52,
			this.spreadsheetCommandBarSubItem5,
			this.spreadsheetCommandBarButtonItem53,
			this.spreadsheetCommandBarButtonItem54,
			this.spreadsheetCommandBarButtonItem55,
			this.spreadsheetCommandBarButtonItem56,
			this.spreadsheetCommandBarButtonItem57,
			this.spreadsheetCommandBarButtonItem58,
			this.spreadsheetCommandBarSubItem6,
			this.spreadsheetCommandBarButtonGalleryDropDownItem1,
			this.spreadsheetCommandBarButtonGalleryDropDownItem2,
			this.spreadsheetCommandBarButtonGalleryDropDownItem3,
			this.spreadsheetCommandBarButtonItem59,
			this.spreadsheetCommandBarButtonItem60,
			this.spreadsheetCommandBarButtonItem61,
			this.spreadsheetCommandBarSubItem7,
			this.spreadsheetCommandBarButtonItem62,
			this.galleryFormatAsTableItem1,
			this.galleryChangeStyleItem1,
			this.spreadsheetCommandBarSubItem8,
			this.spreadsheetCommandBarButtonItem63,
			this.spreadsheetCommandBarButtonItem64,
			this.spreadsheetCommandBarButtonItem65,
			this.spreadsheetCommandBarSubItem9,
			this.spreadsheetCommandBarButtonItem66,
			this.spreadsheetCommandBarButtonItem67,
			this.spreadsheetCommandBarButtonItem68,
			this.spreadsheetCommandBarSubItem10,
			this.spreadsheetCommandBarButtonItem69,
			this.spreadsheetCommandBarButtonItem70,
			this.spreadsheetCommandBarButtonItem71,
			this.spreadsheetCommandBarButtonItem72,
			this.spreadsheetCommandBarButtonItem73,
			this.spreadsheetCommandBarButtonItem74,
			this.spreadsheetCommandBarButtonItem75,
			this.spreadsheetCommandBarButtonItem76,
			this.spreadsheetCommandBarButtonItem77,
			this.spreadsheetCommandBarButtonItem78,
			this.spreadsheetCommandBarButtonItem79,
			this.spreadsheetCommandBarSubItem11,
			this.spreadsheetCommandBarButtonItem80,
			this.spreadsheetCommandBarButtonItem81,
			this.changeSheetTabColorItem1,
			this.spreadsheetCommandBarButtonItem82,
			this.spreadsheetCommandBarCheckItem13,
			this.spreadsheetCommandBarButtonItem83,
			this.spreadsheetCommandBarSubItem12,
			this.spreadsheetCommandBarButtonItem84,
			this.spreadsheetCommandBarButtonItem85,
			this.spreadsheetCommandBarButtonItem86,
			this.spreadsheetCommandBarButtonItem87,
			this.spreadsheetCommandBarButtonItem88,
			this.spreadsheetCommandBarSubItem13,
			this.spreadsheetCommandBarButtonItem89,
			this.spreadsheetCommandBarButtonItem90,
			this.spreadsheetCommandBarButtonItem91,
			this.spreadsheetCommandBarButtonItem92,
			this.spreadsheetCommandBarSubItem14,
			this.spreadsheetCommandBarButtonItem93,
			this.spreadsheetCommandBarButtonItem94,
			this.spreadsheetCommandBarButtonItem95,
			this.spreadsheetCommandBarButtonItem96,
			this.spreadsheetCommandBarButtonItem97,
			this.spreadsheetCommandBarButtonItem98,
			this.spreadsheetCommandBarSubItem15,
			this.spreadsheetCommandBarButtonItem99,
			this.spreadsheetCommandBarButtonItem100,
			this.spreadsheetCommandBarCheckItem14,
			this.spreadsheetCommandBarButtonItem101,
			this.spreadsheetCommandBarButtonItem102,
			this.spreadsheetCommandBarSubItem16,
			this.spreadsheetCommandBarButtonItem103,
			this.spreadsheetCommandBarButtonItem104,
			this.spreadsheetCommandBarButtonItem105,
			this.spreadsheetCommandBarButtonItem106,
			this.spreadsheetCommandBarButtonItem107,
			this.spreadsheetCommandBarButtonItem108,
			this.spreadsheetCommandBarButtonItem109,
			this.spreadsheetCommandBarButtonItem110,
			this.spreadsheetCommandBarButtonItem111,
			this.spreadsheetCommandBarButtonItem112,
			this.spreadsheetCommandBarButtonGalleryDropDownItem4,
			this.spreadsheetCommandBarButtonGalleryDropDownItem5,
			this.spreadsheetCommandBarButtonGalleryDropDownItem6,
			this.spreadsheetCommandBarButtonGalleryDropDownItem7,
			this.spreadsheetCommandBarButtonGalleryDropDownItem8,
			this.spreadsheetCommandBarButtonGalleryDropDownItem9,
			this.spreadsheetCommandBarButtonGalleryDropDownItem10,
			this.spreadsheetCommandBarButtonItem113,
			this.spreadsheetCommandBarButtonItem114,
			this.spreadsheetCommandBarSubItem17,
			this.spreadsheetCommandBarCheckItem15,
			this.spreadsheetCommandBarCheckItem16,
			this.spreadsheetCommandBarCheckItem17,
			this.spreadsheetCommandBarButtonItem115,
			this.spreadsheetCommandBarSubItem18,
			this.spreadsheetCommandBarCheckItem18,
			this.spreadsheetCommandBarCheckItem19,
			this.pageSetupPaperKindItem1,
			this.spreadsheetCommandBarSubItem19,
			this.spreadsheetCommandBarButtonItem116,
			this.spreadsheetCommandBarButtonItem117,
			this.spreadsheetCommandBarButtonItem118,
			this.spreadsheetCommandBarButtonItem119,
			this.spreadsheetCommandBarCheckItem20,
			this.spreadsheetCommandBarCheckItem21,
			this.spreadsheetCommandBarCheckItem22,
			this.spreadsheetCommandBarCheckItem23,
			this.spreadsheetCommandBarSubItem20,
			this.spreadsheetCommandBarButtonItem120,
			this.spreadsheetCommandBarButtonItem121,
			this.spreadsheetCommandBarSubItem21,
			this.spreadsheetCommandBarButtonItem122,
			this.spreadsheetCommandBarButtonItem123,
			this.spreadsheetCommandBarSubItem22,
			this.functionsFinancialItem1,
			this.functionsLogicalItem1,
			this.functionsTextItem1,
			this.functionsDateAndTimeItem1,
			this.functionsLookupAndReferenceItem1,
			this.functionsMathAndTrigonometryItem1,
			this.spreadsheetCommandBarSubItem23,
			this.functionsStatisticalItem1,
			this.functionsEngineeringItem1,
			this.functionsInformationItem1,
			this.functionsCompatibilityItem1,
			this.functionsWebItem1,
			this.spreadsheetCommandBarButtonItem129,
			this.spreadsheetCommandBarButtonItem130,
			this.definedNameListItem1,
			this.spreadsheetCommandBarButtonItem131,
			this.spreadsheetCommandBarCheckItem24,
			this.spreadsheetCommandBarSubItem24,
			this.spreadsheetCommandBarCheckItem25,
			this.spreadsheetCommandBarCheckItem26,
			this.spreadsheetCommandBarButtonItem132,
			this.spreadsheetCommandBarButtonItem133,
			this.spreadsheetCommandBarSubItem25,
			this.spreadsheetCommandBarButtonItem134,
			this.spreadsheetCommandBarButtonItem135,
			this.spreadsheetCommandBarButtonItem136,
			this.spreadsheetCommandBarSubItem26,
			this.spreadsheetCommandBarButtonItem137,
			this.spreadsheetCommandBarButtonItem138,
			this.spreadsheetCommandBarSubItem27,
			this.spreadsheetCommandBarButtonItem139,
			this.spreadsheetCommandBarButtonItem140,
			this.spreadsheetCommandBarButtonItem141,
			this.spreadsheetCommandBarButtonItem142,
			this.spreadsheetCommandBarButtonItem143,
			this.spreadsheetCommandBarButtonItem144,
			this.spreadsheetCommandBarButtonItem145,
			this.spreadsheetCommandBarButtonItem146,
			this.spreadsheetCommandBarButtonItem147,
			this.spreadsheetCommandBarButtonItem148,
			this.spreadsheetCommandBarButtonItem149,
			this.spreadsheetCommandBarButtonItem150,
			this.spreadsheetCommandBarButtonItem151,
			this.spreadsheetCommandBarButtonItem152,
			this.spreadsheetCommandBarButtonItem153,
			this.spreadsheetCommandBarButtonItem154,
			this.spreadsheetCommandBarButtonItem155,
			this.spreadsheetCommandBarSubItem28,
			this.spreadsheetCommandBarButtonItem156,
			this.spreadsheetCommandBarButtonItem157,
			this.spreadsheetCommandBarButtonItem158,
			this.spreadsheetCommandBarButtonItem159,
			this.spreadsheetCommandBarButtonItem160,
			this.spreadsheetCommandBarButtonItem161,
			this.spreadsheetCommandBarButtonItem162,
			this.galleryChartLayoutItem1,
			this.galleryChartStyleItem1,
			this.spreadsheetCommandBarSubItem29,
			this.spreadsheetCommandBarButtonGalleryDropDownItem11,
			this.spreadsheetCommandBarButtonGalleryDropDownItem12,
			this.spreadsheetCommandBarSubItem30,
			this.spreadsheetCommandBarButtonGalleryDropDownItem13,
			this.spreadsheetCommandBarButtonGalleryDropDownItem14,
			this.spreadsheetCommandBarButtonGalleryDropDownItem15,
			this.spreadsheetCommandBarSubItem31,
			this.spreadsheetCommandBarButtonGalleryDropDownItem16,
			this.spreadsheetCommandBarButtonGalleryDropDownItem17,
			this.spreadsheetCommandBarButtonGalleryDropDownItem18,
			this.spreadsheetCommandBarButtonGalleryDropDownItem19,
			this.spreadsheetCommandBarButtonGalleryDropDownItem20,
			this.spreadsheetCommandBarButtonGalleryDropDownItem21,
			this.spreadsheetCommandBarButtonGalleryDropDownItem22,
			this.renameTableItemCaption1,
			this.renameTableItem1,
			this.spreadsheetCommandBarCheckItem27,
			this.spreadsheetCommandBarCheckItem28,
			this.spreadsheetCommandBarCheckItem29,
			this.spreadsheetCommandBarCheckItem30,
			this.spreadsheetCommandBarCheckItem31,
			this.spreadsheetCommandBarCheckItem32,
			this.spreadsheetCommandBarCheckItem33,
			this.galleryTableStylesItem1,
			this.spreadsheetCommandBarButtonItem163,
			this.spreadsheetCommandBarButtonItem164,
			this.spreadsheetCommandBarButtonItem165,
			this.spreadsheetCommandBarButtonItem166,
			this.spreadsheetCommandBarButtonItem167,
			this.spreadsheetCommandBarButtonItem168,
			this.spreadsheetCommandBarButtonItem169,
			this.spreadsheetCommandBarSubItem32,
			this.spreadsheetCommandBarButtonItem170,
			this.spreadsheetCommandBarButtonItem171,
			this.spreadsheetCommandBarButtonItem172,
			this.spreadsheetCommandBarSubItem33,
			this.spreadsheetCommandBarButtonItem173,
			this.spreadsheetCommandBarButtonItem174,
			this.spreadsheetCommandBarSubItem34,
			this.spreadsheetCommandBarButtonItem175,
			this.spreadsheetCommandBarButtonItem176,
			this.spreadsheetCommandBarButtonItem177,
			this.spreadsheetCommandBarButtonItem178,
			this.spreadsheetCommandBarSubItem35,
			this.spreadsheetCommandBarButtonItem179,
			this.spreadsheetCommandBarButtonItem180,
			this.spreadsheetCommandBarButtonItem181,
			this.spreadsheetCommandBarButtonItem182,
			this.spreadsheetCommandBarCheckItem34,
			this.spreadsheetCommandBarCheckItem35,
			this.spreadsheetCommandBarCheckItem36,
			this.spreadsheetCommandBarSubItem36,
			this.spreadsheetCommandBarButtonItem183,
			this.spreadsheetCommandBarButtonItem184,
			this.spreadsheetCommandBarButtonItem185,
			this.spreadsheetCommandBarSubItem37,
			this.spreadsheetCommandBarButtonItem186,
			this.spreadsheetCommandBarButtonItem187,
			this.spreadsheetCommandBarButtonItem188,
			this.spreadsheetCommandBarButtonItem189,
			this.spreadsheetCommandBarSubItem38,
			this.spreadsheetCommandBarButtonItem190,
			this.spreadsheetCommandBarButtonItem191,
			this.spreadsheetCommandBarButtonItem192,
			this.spreadsheetCommandBarButtonItem193,
			this.spreadsheetCommandBarButtonItem194,
			this.spreadsheetCommandBarSubItem39,
			this.spreadsheetCommandBarButtonItem195,
			this.spreadsheetCommandBarButtonItem196,
			this.spreadsheetCommandBarCheckItem37,
			this.spreadsheetCommandBarCheckItem38,
			this.spreadsheetCommandBarCheckItem39,
			this.spreadsheetCommandBarCheckItem40,
			this.galleryPivotStylesItem1,
			this.spreadsheetCommandBarButtonItem197,
			this.spreadsheetCommandBarSubItem40,
			this.spreadsheetCommandBarButtonItem198,
			this.spreadsheetCommandBarButtonItem199,
			this.spreadsheetCommandBarSubItem41,
			this.spreadsheetCommandBarButtonItem200,
			this.spreadsheetCommandBarButtonItem201,
			this.spreadsheetCommandBarButtonItem202,
			this.spreadsheetCommandBarCheckItem41,
			this.spreadsheetCommandBarCheckItem42,
			this.spreadsheetCommandBarCheckItem43,
			this.spreadsheetCommandBarSubItem42,
			this.spreadsheetCommandBarCheckItem44,
			this.spreadsheetCommandBarCheckItem45,
			this.spreadsheetCommandBarButtonItem203,
			this.spreadsheetCommandBarButtonItem204,
			this.spreadsheetCommandBarButtonItem205,
			this.spreadsheetCommandBarSubItem43,
			this.spreadsheetCommandBarButtonItem206,
			this.spreadsheetCommandBarButtonItem207,
			this.spreadsheetCommandBarButtonItem208,
			this.spreadsheetCommandBarButtonItem209,
			this.spreadsheetCommandBarButtonItem210,
			this.spreadsheetCommandBarButtonItem211,
			this.spreadsheetCommandBarButtonItem212,
			this.spreadsheetCommandBarButtonItem213,
			this.spreadsheetCommandBarCheckItem46,
			this.spreadsheetCommandBarButtonItem214
		});
		this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl1.MaxItemId = 374;
		this.ribbonControl1.Name = "ribbonControl1";
		this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[5] { this.chartToolsRibbonPageCategory1, this.tableToolsRibbonPageCategory1, this.drawingToolsRibbonPageCategory1, this.pictureToolsRibbonPageCategory1, this.pivotTableToolsRibbonPageCategory1 });
		this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[9] { this.fileRibbonPage1, this.homeRibbonPage1, this.insertRibbonPage1, this.pageLayoutRibbonPage1, this.formulasRibbonPage1, this.dataRibbonPage1, this.reviewRibbonPage1, this.viewRibbonPage1, this.mailMergeRibbonPage1 });
		this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemFontEdit1, this.repositoryItemSpreadsheetFontSizeEdit1, this.repositoryItemPopupGalleryEdit1, this.repositoryItemTextEdit1 });
		this.ribbonControl1.Size = new System.Drawing.Size(946, 150);
		this.spreadsheetCommandBarButtonItem1.CommandName = "FileNew";
		this.spreadsheetCommandBarButtonItem1.Id = 1;
		this.spreadsheetCommandBarButtonItem1.Name = "spreadsheetCommandBarButtonItem1";
		this.spreadsheetCommandBarButtonItem2.CommandName = "FileOpen";
		this.spreadsheetCommandBarButtonItem2.Id = 2;
		this.spreadsheetCommandBarButtonItem2.Name = "spreadsheetCommandBarButtonItem2";
		this.spreadsheetCommandBarButtonItem3.CommandName = "FileSave";
		this.spreadsheetCommandBarButtonItem3.Id = 3;
		this.spreadsheetCommandBarButtonItem3.Name = "spreadsheetCommandBarButtonItem3";
		this.spreadsheetCommandBarButtonItem4.CommandName = "FileSaveAs";
		this.spreadsheetCommandBarButtonItem4.Id = 4;
		this.spreadsheetCommandBarButtonItem4.Name = "spreadsheetCommandBarButtonItem4";
		this.spreadsheetCommandBarButtonItem5.CommandName = "FileQuickPrint";
		this.spreadsheetCommandBarButtonItem5.Id = 5;
		this.spreadsheetCommandBarButtonItem5.Name = "spreadsheetCommandBarButtonItem5";
		this.spreadsheetCommandBarButtonItem6.CommandName = "FilePrint";
		this.spreadsheetCommandBarButtonItem6.Id = 6;
		this.spreadsheetCommandBarButtonItem6.Name = "spreadsheetCommandBarButtonItem6";
		this.spreadsheetCommandBarButtonItem7.CommandName = "FilePrintPreview";
		this.spreadsheetCommandBarButtonItem7.Id = 7;
		this.spreadsheetCommandBarButtonItem7.Name = "spreadsheetCommandBarButtonItem7";
		this.spreadsheetCommandBarButtonItem8.CommandName = "FileUndo";
		this.spreadsheetCommandBarButtonItem8.Id = 8;
		this.spreadsheetCommandBarButtonItem8.Name = "spreadsheetCommandBarButtonItem8";
		this.spreadsheetCommandBarButtonItem9.CommandName = "FileRedo";
		this.spreadsheetCommandBarButtonItem9.Id = 9;
		this.spreadsheetCommandBarButtonItem9.Name = "spreadsheetCommandBarButtonItem9";
		this.spreadsheetCommandBarButtonItem10.CommandName = "FileEncrypt";
		this.spreadsheetCommandBarButtonItem10.Id = 10;
		this.spreadsheetCommandBarButtonItem10.Name = "spreadsheetCommandBarButtonItem10";
		this.spreadsheetCommandBarButtonItem11.CommandName = "FileShowDocumentProperties";
		this.spreadsheetCommandBarButtonItem11.Id = 11;
		this.spreadsheetCommandBarButtonItem11.Name = "spreadsheetCommandBarButtonItem11";
		this.spreadsheetCommandBarButtonItem12.CommandName = "PasteSelection";
		this.spreadsheetCommandBarButtonItem12.Id = 22;
		this.spreadsheetCommandBarButtonItem12.Name = "spreadsheetCommandBarButtonItem12";
		this.spreadsheetCommandBarButtonItem12.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem13.CommandName = "CutSelection";
		this.spreadsheetCommandBarButtonItem13.Id = 23;
		this.spreadsheetCommandBarButtonItem13.Name = "spreadsheetCommandBarButtonItem13";
		this.spreadsheetCommandBarButtonItem13.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem14.CommandName = "CopySelection";
		this.spreadsheetCommandBarButtonItem14.Id = 24;
		this.spreadsheetCommandBarButtonItem14.Name = "spreadsheetCommandBarButtonItem14";
		this.spreadsheetCommandBarButtonItem14.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem15.CommandName = "ShowPasteSpecialForm";
		this.spreadsheetCommandBarButtonItem15.Id = 25;
		this.spreadsheetCommandBarButtonItem15.Name = "spreadsheetCommandBarButtonItem15";
		this.spreadsheetCommandBarButtonItem15.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup1.Id = 12;
		this.barButtonGroup1.ItemLinks.Add(this.changeFontNameItem1);
		this.barButtonGroup1.ItemLinks.Add(this.changeFontSizeItem1);
		this.barButtonGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem16);
		this.barButtonGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem17);
		this.barButtonGroup1.Name = "barButtonGroup1";
		this.barButtonGroup1.Tag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		this.changeFontNameItem1.Edit = this.repositoryItemFontEdit1;
		this.changeFontNameItem1.Id = 26;
		this.changeFontNameItem1.Name = "changeFontNameItem1";
		this.repositoryItemFontEdit1.AutoHeight = false;
		this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
		this.changeFontSizeItem1.Edit = this.repositoryItemSpreadsheetFontSizeEdit1;
		this.changeFontSizeItem1.Id = 27;
		this.changeFontSizeItem1.Name = "changeFontSizeItem1";
		this.repositoryItemSpreadsheetFontSizeEdit1.AutoHeight = false;
		this.repositoryItemSpreadsheetFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSpreadsheetFontSizeEdit1.Control = this.spreadsheetControl1;
		this.repositoryItemSpreadsheetFontSizeEdit1.Name = "repositoryItemSpreadsheetFontSizeEdit1";
		this.spreadsheetControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.spreadsheetControl1.Location = new System.Drawing.Point(0, 184);
		this.spreadsheetControl1.MenuManager = this.ribbonControl1;
		this.spreadsheetControl1.Name = "spreadsheetControl1";
		this.spreadsheetControl1.Options.Import.Csv.Encoding = null;
		this.spreadsheetControl1.Options.Import.Txt.Encoding = null;
		this.spreadsheetControl1.Size = new System.Drawing.Size(746, 454);
		this.spreadsheetControl1.TabIndex = 0;
		this.spreadsheetControl1.Text = "spreadsheetControl1";
		this.spreadsheetControl1.DocumentLoaded += new System.EventHandler(this.spreadsheetControl1_DocumentLoaded);
		this.spreadsheetControl1.EmptyDocumentCreated += new System.EventHandler(this.spreadsheetControl1_EmptyDocumentCreated);
		this.spreadsheetCommandBarButtonItem16.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		this.spreadsheetCommandBarButtonItem16.CommandName = "FormatIncreaseFontSize";
		this.spreadsheetCommandBarButtonItem16.Id = 28;
		this.spreadsheetCommandBarButtonItem16.Name = "spreadsheetCommandBarButtonItem16";
		this.spreadsheetCommandBarButtonItem16.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem17.ButtonGroupTag = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		this.spreadsheetCommandBarButtonItem17.CommandName = "FormatDecreaseFontSize";
		this.spreadsheetCommandBarButtonItem17.Id = 29;
		this.spreadsheetCommandBarButtonItem17.Name = "spreadsheetCommandBarButtonItem17";
		this.spreadsheetCommandBarButtonItem17.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup2.Id = 13;
		this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem1);
		this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem2);
		this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem3);
		this.barButtonGroup2.ItemLinks.Add(this.spreadsheetCommandBarCheckItem4);
		this.barButtonGroup2.Name = "barButtonGroup2";
		this.barButtonGroup2.Tag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem1.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem1.CommandName = "FormatFontBold";
		this.spreadsheetCommandBarCheckItem1.Id = 30;
		this.spreadsheetCommandBarCheckItem1.Name = "spreadsheetCommandBarCheckItem1";
		this.spreadsheetCommandBarCheckItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem2.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem2.CommandName = "FormatFontItalic";
		this.spreadsheetCommandBarCheckItem2.Id = 31;
		this.spreadsheetCommandBarCheckItem2.Name = "spreadsheetCommandBarCheckItem2";
		this.spreadsheetCommandBarCheckItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem3.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem3.CommandName = "FormatFontUnderline";
		this.spreadsheetCommandBarCheckItem3.Id = 32;
		this.spreadsheetCommandBarCheckItem3.Name = "spreadsheetCommandBarCheckItem3";
		this.spreadsheetCommandBarCheckItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem4.ButtonGroupTag = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		this.spreadsheetCommandBarCheckItem4.CommandName = "FormatFontStrikeout";
		this.spreadsheetCommandBarCheckItem4.Id = 33;
		this.spreadsheetCommandBarCheckItem4.Name = "spreadsheetCommandBarCheckItem4";
		this.spreadsheetCommandBarCheckItem4.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup3.Id = 14;
		this.barButtonGroup3.ItemLinks.Add(this.spreadsheetCommandBarSubItem1);
		this.barButtonGroup3.Name = "barButtonGroup3";
		this.barButtonGroup3.Tag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
		this.spreadsheetCommandBarSubItem1.ButtonGroupTag = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
		this.spreadsheetCommandBarSubItem1.CommandName = "FormatBordersCommandGroup";
		this.spreadsheetCommandBarSubItem1.Id = 34;
		this.spreadsheetCommandBarSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[15]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem19),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem20),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem21),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem22),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem23),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem24),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem25),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem26),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem27),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem28),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem29),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem30),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeBorderLineStyleItem1)
		});
		this.spreadsheetCommandBarSubItem1.Name = "spreadsheetCommandBarSubItem1";
		this.spreadsheetCommandBarSubItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem18.CommandName = "FormatBottomBorder";
		this.spreadsheetCommandBarButtonItem18.Id = 35;
		this.spreadsheetCommandBarButtonItem18.Name = "spreadsheetCommandBarButtonItem18";
		this.spreadsheetCommandBarButtonItem19.CommandName = "FormatTopBorder";
		this.spreadsheetCommandBarButtonItem19.Id = 36;
		this.spreadsheetCommandBarButtonItem19.Name = "spreadsheetCommandBarButtonItem19";
		this.spreadsheetCommandBarButtonItem20.CommandName = "FormatLeftBorder";
		this.spreadsheetCommandBarButtonItem20.Id = 37;
		this.spreadsheetCommandBarButtonItem20.Name = "spreadsheetCommandBarButtonItem20";
		this.spreadsheetCommandBarButtonItem21.CommandName = "FormatRightBorder";
		this.spreadsheetCommandBarButtonItem21.Id = 38;
		this.spreadsheetCommandBarButtonItem21.Name = "spreadsheetCommandBarButtonItem21";
		this.spreadsheetCommandBarButtonItem22.CommandName = "FormatNoBorders";
		this.spreadsheetCommandBarButtonItem22.Id = 39;
		this.spreadsheetCommandBarButtonItem22.Name = "spreadsheetCommandBarButtonItem22";
		this.spreadsheetCommandBarButtonItem23.CommandName = "FormatAllBorders";
		this.spreadsheetCommandBarButtonItem23.Id = 40;
		this.spreadsheetCommandBarButtonItem23.Name = "spreadsheetCommandBarButtonItem23";
		this.spreadsheetCommandBarButtonItem24.CommandName = "FormatOutsideBorders";
		this.spreadsheetCommandBarButtonItem24.Id = 41;
		this.spreadsheetCommandBarButtonItem24.Name = "spreadsheetCommandBarButtonItem24";
		this.spreadsheetCommandBarButtonItem25.CommandName = "FormatThickBorder";
		this.spreadsheetCommandBarButtonItem25.Id = 42;
		this.spreadsheetCommandBarButtonItem25.Name = "spreadsheetCommandBarButtonItem25";
		this.spreadsheetCommandBarButtonItem26.CommandName = "FormatBottomDoubleBorder";
		this.spreadsheetCommandBarButtonItem26.Id = 43;
		this.spreadsheetCommandBarButtonItem26.Name = "spreadsheetCommandBarButtonItem26";
		this.spreadsheetCommandBarButtonItem27.CommandName = "FormatBottomThickBorder";
		this.spreadsheetCommandBarButtonItem27.Id = 44;
		this.spreadsheetCommandBarButtonItem27.Name = "spreadsheetCommandBarButtonItem27";
		this.spreadsheetCommandBarButtonItem28.CommandName = "FormatTopAndBottomBorder";
		this.spreadsheetCommandBarButtonItem28.Id = 45;
		this.spreadsheetCommandBarButtonItem28.Name = "spreadsheetCommandBarButtonItem28";
		this.spreadsheetCommandBarButtonItem29.CommandName = "FormatTopAndThickBottomBorder";
		this.spreadsheetCommandBarButtonItem29.Id = 46;
		this.spreadsheetCommandBarButtonItem29.Name = "spreadsheetCommandBarButtonItem29";
		this.spreadsheetCommandBarButtonItem30.CommandName = "FormatTopAndDoubleBottomBorder";
		this.spreadsheetCommandBarButtonItem30.Id = 47;
		this.spreadsheetCommandBarButtonItem30.Name = "spreadsheetCommandBarButtonItem30";
		this.changeBorderLineColorItem1.ActAsDropDown = true;
		this.changeBorderLineColorItem1.Id = 48;
		this.changeBorderLineColorItem1.Name = "changeBorderLineColorItem1";
		this.changeBorderLineStyleItem1.DropDownControl = this.commandBarGalleryDropDown1;
		this.changeBorderLineStyleItem1.Id = 49;
		this.changeBorderLineStyleItem1.Name = "changeBorderLineStyleItem1";
		this.commandBarGalleryDropDown1.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown1.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown1.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.commandBarGalleryDropDown1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown1.Gallery.ItemSize = new System.Drawing.Size(136, 26);
		this.commandBarGalleryDropDown1.Gallery.RowCount = 14;
		this.commandBarGalleryDropDown1.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown1.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown1.Name = "commandBarGalleryDropDown1";
		this.commandBarGalleryDropDown1.Ribbon = this.ribbonControl1;
		this.barButtonGroup4.Id = 15;
		this.barButtonGroup4.ItemLinks.Add(this.changeCellFillColorItem1);
		this.barButtonGroup4.ItemLinks.Add(this.changeFontColorItem1);
		this.barButtonGroup4.Name = "barButtonGroup4";
		this.barButtonGroup4.Tag = "{C2275623-04A3-41E8-8D6A-EB5C7F8541D1}";
		this.changeCellFillColorItem1.Id = 50;
		this.changeCellFillColorItem1.Name = "changeCellFillColorItem1";
		this.changeFontColorItem1.Id = 51;
		this.changeFontColorItem1.Name = "changeFontColorItem1";
		this.barButtonGroup5.Id = 16;
		this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem5);
		this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem6);
		this.barButtonGroup5.ItemLinks.Add(this.spreadsheetCommandBarCheckItem7);
		this.barButtonGroup5.Name = "barButtonGroup5";
		this.barButtonGroup5.Tag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem5.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem5.CommandName = "FormatAlignmentTop";
		this.spreadsheetCommandBarCheckItem5.Id = 52;
		this.spreadsheetCommandBarCheckItem5.Name = "spreadsheetCommandBarCheckItem5";
		this.spreadsheetCommandBarCheckItem5.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem6.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem6.CommandName = "FormatAlignmentMiddle";
		this.spreadsheetCommandBarCheckItem6.Id = 53;
		this.spreadsheetCommandBarCheckItem6.Name = "spreadsheetCommandBarCheckItem6";
		this.spreadsheetCommandBarCheckItem6.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem7.ButtonGroupTag = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		this.spreadsheetCommandBarCheckItem7.CommandName = "FormatAlignmentBottom";
		this.spreadsheetCommandBarCheckItem7.Id = 54;
		this.spreadsheetCommandBarCheckItem7.Name = "spreadsheetCommandBarCheckItem7";
		this.spreadsheetCommandBarCheckItem7.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup6.Id = 17;
		this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem8);
		this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem9);
		this.barButtonGroup6.ItemLinks.Add(this.spreadsheetCommandBarCheckItem10);
		this.barButtonGroup6.Name = "barButtonGroup6";
		this.barButtonGroup6.Tag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.spreadsheetCommandBarCheckItem8.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.spreadsheetCommandBarCheckItem8.CommandName = "FormatAlignmentLeft";
		this.spreadsheetCommandBarCheckItem8.Id = 55;
		this.spreadsheetCommandBarCheckItem8.Name = "spreadsheetCommandBarCheckItem8";
		this.spreadsheetCommandBarCheckItem8.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem9.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.spreadsheetCommandBarCheckItem9.CommandName = "FormatAlignmentCenter";
		this.spreadsheetCommandBarCheckItem9.Id = 56;
		this.spreadsheetCommandBarCheckItem9.Name = "spreadsheetCommandBarCheckItem9";
		this.spreadsheetCommandBarCheckItem9.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem10.ButtonGroupTag = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		this.spreadsheetCommandBarCheckItem10.CommandName = "FormatAlignmentRight";
		this.spreadsheetCommandBarCheckItem10.Id = 57;
		this.spreadsheetCommandBarCheckItem10.Name = "spreadsheetCommandBarCheckItem10";
		this.spreadsheetCommandBarCheckItem10.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup7.Id = 18;
		this.barButtonGroup7.ItemLinks.Add(this.spreadsheetCommandBarButtonItem31);
		this.barButtonGroup7.ItemLinks.Add(this.spreadsheetCommandBarButtonItem32);
		this.barButtonGroup7.Name = "barButtonGroup7";
		this.barButtonGroup7.Tag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		this.spreadsheetCommandBarButtonItem31.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		this.spreadsheetCommandBarButtonItem31.CommandName = "FormatDecreaseIndent";
		this.spreadsheetCommandBarButtonItem31.Id = 58;
		this.spreadsheetCommandBarButtonItem31.Name = "spreadsheetCommandBarButtonItem31";
		this.spreadsheetCommandBarButtonItem31.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem32.ButtonGroupTag = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		this.spreadsheetCommandBarButtonItem32.CommandName = "FormatIncreaseIndent";
		this.spreadsheetCommandBarButtonItem32.Id = 59;
		this.spreadsheetCommandBarButtonItem32.Name = "spreadsheetCommandBarButtonItem32";
		this.spreadsheetCommandBarButtonItem32.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarCheckItem11.CommandName = "FormatWrapText";
		this.spreadsheetCommandBarCheckItem11.Id = 60;
		this.spreadsheetCommandBarCheckItem11.Name = "spreadsheetCommandBarCheckItem11";
		this.spreadsheetCommandBarCheckItem11.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarSubItem2.CommandName = "EditingMergeCellsCommandGroup";
		this.spreadsheetCommandBarSubItem2.Id = 61;
		this.spreadsheetCommandBarSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem33),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem34),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem35)
		});
		this.spreadsheetCommandBarSubItem2.Name = "spreadsheetCommandBarSubItem2";
		this.spreadsheetCommandBarSubItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem12.CommandName = "EditingMergeAndCenterCells";
		this.spreadsheetCommandBarCheckItem12.Id = 62;
		this.spreadsheetCommandBarCheckItem12.Name = "spreadsheetCommandBarCheckItem12";
		this.spreadsheetCommandBarButtonItem33.CommandName = "EditingMergeCellsAcross";
		this.spreadsheetCommandBarButtonItem33.Id = 63;
		this.spreadsheetCommandBarButtonItem33.Name = "spreadsheetCommandBarButtonItem33";
		this.spreadsheetCommandBarButtonItem34.CommandName = "EditingMergeCells";
		this.spreadsheetCommandBarButtonItem34.Id = 64;
		this.spreadsheetCommandBarButtonItem34.Name = "spreadsheetCommandBarButtonItem34";
		this.spreadsheetCommandBarButtonItem35.CommandName = "EditingUnmergeCells";
		this.spreadsheetCommandBarButtonItem35.Id = 65;
		this.spreadsheetCommandBarButtonItem35.Name = "spreadsheetCommandBarButtonItem35";
		this.barButtonGroup8.Id = 19;
		this.barButtonGroup8.ItemLinks.Add(this.changeNumberFormatItem1);
		this.barButtonGroup8.Name = "barButtonGroup8";
		this.barButtonGroup8.Tag = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
		this.changeNumberFormatItem1.Edit = this.repositoryItemPopupGalleryEdit1;
		this.changeNumberFormatItem1.Id = 66;
		this.changeNumberFormatItem1.Name = "changeNumberFormatItem1";
		this.repositoryItemPopupGalleryEdit1.AutoHeight = false;
		this.repositoryItemPopupGalleryEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemPopupGalleryEdit1.Gallery.AllowFilter = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.AutoFitColumns = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.ColumnCount = 1;
		this.repositoryItemPopupGalleryEdit1.Gallery.FixedImageSize = false;
		spreadsheetCommandGalleryItem13.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem13.CaptionAsValue = true;
		spreadsheetCommandGalleryItem13.Checked = true;
		spreadsheetCommandGalleryItem13.CommandName = "FormatNumberGeneral";
		spreadsheetCommandGalleryItem13.IsEmptyHint = true;
		spreadsheetCommandGalleryItem14.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem14.CaptionAsValue = true;
		spreadsheetCommandGalleryItem14.CommandName = "FormatNumberDecimal";
		spreadsheetCommandGalleryItem14.IsEmptyHint = true;
		spreadsheetCommandGalleryItem15.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem15.CaptionAsValue = true;
		spreadsheetCommandGalleryItem15.CommandName = "FormatNumberAccountingCurrency";
		spreadsheetCommandGalleryItem15.IsEmptyHint = true;
		spreadsheetCommandGalleryItem16.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem16.CaptionAsValue = true;
		spreadsheetCommandGalleryItem16.CommandName = "FormatNumberAccountingRegular";
		spreadsheetCommandGalleryItem16.IsEmptyHint = true;
		spreadsheetCommandGalleryItem17.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem17.CaptionAsValue = true;
		spreadsheetCommandGalleryItem17.CommandName = "FormatNumberShortDate";
		spreadsheetCommandGalleryItem17.IsEmptyHint = true;
		spreadsheetCommandGalleryItem18.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem18.CaptionAsValue = true;
		spreadsheetCommandGalleryItem18.CommandName = "FormatNumberLongDate";
		spreadsheetCommandGalleryItem18.IsEmptyHint = true;
		spreadsheetCommandGalleryItem19.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem19.CaptionAsValue = true;
		spreadsheetCommandGalleryItem19.CommandName = "FormatNumberTime";
		spreadsheetCommandGalleryItem19.IsEmptyHint = true;
		spreadsheetCommandGalleryItem20.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem20.CaptionAsValue = true;
		spreadsheetCommandGalleryItem20.CommandName = "FormatNumberPercentage";
		spreadsheetCommandGalleryItem20.IsEmptyHint = true;
		spreadsheetCommandGalleryItem21.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem21.CaptionAsValue = true;
		spreadsheetCommandGalleryItem21.CommandName = "FormatNumberFraction";
		spreadsheetCommandGalleryItem21.IsEmptyHint = true;
		spreadsheetCommandGalleryItem22.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem22.CaptionAsValue = true;
		spreadsheetCommandGalleryItem22.CommandName = "FormatNumberScientific";
		spreadsheetCommandGalleryItem22.IsEmptyHint = true;
		spreadsheetCommandGalleryItem23.AlwaysUpdateDescription = true;
		spreadsheetCommandGalleryItem23.CaptionAsValue = true;
		spreadsheetCommandGalleryItem23.CommandName = "FormatNumberText";
		spreadsheetCommandGalleryItem23.IsEmptyHint = true;
		galleryItemGroup.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[11]
		{
			spreadsheetCommandGalleryItem13, spreadsheetCommandGalleryItem14, spreadsheetCommandGalleryItem15, spreadsheetCommandGalleryItem16, spreadsheetCommandGalleryItem17, spreadsheetCommandGalleryItem18, spreadsheetCommandGalleryItem19, spreadsheetCommandGalleryItem20, spreadsheetCommandGalleryItem21, spreadsheetCommandGalleryItem22,
			spreadsheetCommandGalleryItem23
		});
		this.repositoryItemPopupGalleryEdit1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup });
		this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.repositoryItemPopupGalleryEdit1.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.repositoryItemPopupGalleryEdit1.Gallery.RowCount = 11;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowGroupCaption = false;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowItemText = true;
		this.repositoryItemPopupGalleryEdit1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
		this.repositoryItemPopupGalleryEdit1.Gallery.StretchItems = true;
		this.repositoryItemPopupGalleryEdit1.Name = "repositoryItemPopupGalleryEdit1";
		this.repositoryItemPopupGalleryEdit1.ShowButtons = false;
		this.repositoryItemPopupGalleryEdit1.ShowPopupCloseButton = false;
		this.repositoryItemPopupGalleryEdit1.ShowSizeGrip = false;
		this.barButtonGroup9.Id = 20;
		this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarSubItem3);
		this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarButtonItem42);
		this.barButtonGroup9.ItemLinks.Add(this.spreadsheetCommandBarButtonItem43);
		this.barButtonGroup9.Name = "barButtonGroup9";
		this.barButtonGroup9.Tag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarSubItem3.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarSubItem3.CommandName = "FormatNumberAccountingCommandGroup";
		this.spreadsheetCommandBarSubItem3.Id = 67;
		this.spreadsheetCommandBarSubItem3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem36),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem37),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem38),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem39),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem40),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem41)
		});
		this.spreadsheetCommandBarSubItem3.Name = "spreadsheetCommandBarSubItem3";
		this.spreadsheetCommandBarSubItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem36.CommandName = "FormatNumberAccountingDefault";
		this.spreadsheetCommandBarButtonItem36.Id = 68;
		this.spreadsheetCommandBarButtonItem36.Name = "spreadsheetCommandBarButtonItem36";
		this.spreadsheetCommandBarButtonItem37.CommandName = "FormatNumberAccountingUS";
		this.spreadsheetCommandBarButtonItem37.Id = 69;
		this.spreadsheetCommandBarButtonItem37.Name = "spreadsheetCommandBarButtonItem37";
		this.spreadsheetCommandBarButtonItem38.CommandName = "FormatNumberAccountingUK";
		this.spreadsheetCommandBarButtonItem38.Id = 70;
		this.spreadsheetCommandBarButtonItem38.Name = "spreadsheetCommandBarButtonItem38";
		this.spreadsheetCommandBarButtonItem39.CommandName = "FormatNumberAccountingEuro";
		this.spreadsheetCommandBarButtonItem39.Id = 71;
		this.spreadsheetCommandBarButtonItem39.Name = "spreadsheetCommandBarButtonItem39";
		this.spreadsheetCommandBarButtonItem40.CommandName = "FormatNumberAccountingPRC";
		this.spreadsheetCommandBarButtonItem40.Id = 72;
		this.spreadsheetCommandBarButtonItem40.Name = "spreadsheetCommandBarButtonItem40";
		this.spreadsheetCommandBarButtonItem41.CommandName = "FormatNumberAccountingSwiss";
		this.spreadsheetCommandBarButtonItem41.Id = 73;
		this.spreadsheetCommandBarButtonItem41.Name = "spreadsheetCommandBarButtonItem41";
		this.spreadsheetCommandBarButtonItem42.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarButtonItem42.CommandName = "FormatNumberPercent";
		this.spreadsheetCommandBarButtonItem42.Id = 74;
		this.spreadsheetCommandBarButtonItem42.Name = "spreadsheetCommandBarButtonItem42";
		this.spreadsheetCommandBarButtonItem42.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem43.ButtonGroupTag = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		this.spreadsheetCommandBarButtonItem43.CommandName = "FormatNumberAccounting";
		this.spreadsheetCommandBarButtonItem43.Id = 75;
		this.spreadsheetCommandBarButtonItem43.Name = "spreadsheetCommandBarButtonItem43";
		this.spreadsheetCommandBarButtonItem43.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.barButtonGroup10.Id = 21;
		this.barButtonGroup10.ItemLinks.Add(this.spreadsheetCommandBarButtonItem44);
		this.barButtonGroup10.ItemLinks.Add(this.spreadsheetCommandBarButtonItem45);
		this.barButtonGroup10.Name = "barButtonGroup10";
		this.barButtonGroup10.Tag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
		this.spreadsheetCommandBarButtonItem44.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
		this.spreadsheetCommandBarButtonItem44.CommandName = "FormatNumberIncreaseDecimal";
		this.spreadsheetCommandBarButtonItem44.Id = 76;
		this.spreadsheetCommandBarButtonItem44.Name = "spreadsheetCommandBarButtonItem44";
		this.spreadsheetCommandBarButtonItem44.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonItem45.ButtonGroupTag = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
		this.spreadsheetCommandBarButtonItem45.CommandName = "FormatNumberDecreaseDecimal";
		this.spreadsheetCommandBarButtonItem45.Id = 77;
		this.spreadsheetCommandBarButtonItem45.Name = "spreadsheetCommandBarButtonItem45";
		this.spreadsheetCommandBarButtonItem45.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText;
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.DropDownControl = this.commandBarGalleryDropDown3;
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.Id = 95;
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.Name = "spreadsheetCommandBarButtonGalleryDropDownItem2";
		this.spreadsheetCommandBarButtonGalleryDropDownItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown3.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup3.CommandName = "ConditionalFormattingColorScalesCommandGroup";
		spreadsheetCommandGalleryItem24.CommandName = "ConditionalFormattingColorScaleGreenYellowRed";
		spreadsheetCommandGalleryItem25.CommandName = "ConditionalFormattingColorScaleRedYellowGreen";
		spreadsheetCommandGalleryItem26.CommandName = "ConditionalFormattingColorScaleGreenWhiteRed";
		spreadsheetCommandGalleryItem27.CommandName = "ConditionalFormattingColorScaleRedWhiteGreen";
		spreadsheetCommandGalleryItem28.CommandName = "ConditionalFormattingColorScaleBlueWhiteRed";
		spreadsheetCommandGalleryItem29.CommandName = "ConditionalFormattingColorScaleRedWhiteBlue";
		spreadsheetCommandGalleryItem30.CommandName = "ConditionalFormattingColorScaleWhiteRed";
		spreadsheetCommandGalleryItem31.CommandName = "ConditionalFormattingColorScaleRedWhite";
		spreadsheetCommandGalleryItem32.CommandName = "ConditionalFormattingColorScaleGreenWhite";
		spreadsheetCommandGalleryItem33.CommandName = "ConditionalFormattingColorScaleWhiteGreen";
		spreadsheetCommandGalleryItem34.CommandName = "ConditionalFormattingColorScaleGreenYellow";
		spreadsheetCommandGalleryItem35.CommandName = "ConditionalFormattingColorScaleYellowGreen";
		spreadsheetCommandGalleryItemGroup3.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[12]
		{
			spreadsheetCommandGalleryItem24, spreadsheetCommandGalleryItem25, spreadsheetCommandGalleryItem26, spreadsheetCommandGalleryItem27, spreadsheetCommandGalleryItem28, spreadsheetCommandGalleryItem29, spreadsheetCommandGalleryItem30, spreadsheetCommandGalleryItem31, spreadsheetCommandGalleryItem32, spreadsheetCommandGalleryItem33,
			spreadsheetCommandGalleryItem34, spreadsheetCommandGalleryItem35
		});
		this.commandBarGalleryDropDown3.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup3 });
		this.commandBarGalleryDropDown3.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown3.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown3.Name = "commandBarGalleryDropDown3";
		this.commandBarGalleryDropDown3.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.CommandName = "ConditionalFormattingIconSetsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.DropDownControl = this.commandBarGalleryDropDown4;
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.Id = 96;
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.Name = "spreadsheetCommandBarButtonGalleryDropDownItem3";
		this.spreadsheetCommandBarButtonGalleryDropDownItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarGalleryDropDown4.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup4.CommandName = "ConditionalFormattingIconSetsDirectionalCommandGroup";
		spreadsheetCommandGalleryItem36.CommandName = "ConditionalFormattingIconSetArrows3Colored";
		spreadsheetCommandGalleryItem37.CommandName = "ConditionalFormattingIconSetArrows3Grayed";
		spreadsheetCommandGalleryItem38.CommandName = "ConditionalFormattingIconSetArrows4Colored";
		spreadsheetCommandGalleryItem39.CommandName = "ConditionalFormattingIconSetArrows4Grayed";
		spreadsheetCommandGalleryItem40.CommandName = "ConditionalFormattingIconSetArrows5Colored";
		spreadsheetCommandGalleryItem41.CommandName = "ConditionalFormattingIconSetArrows5Grayed";
		spreadsheetCommandGalleryItem42.CommandName = "ConditionalFormattingIconSetTriangles3";
		spreadsheetCommandGalleryItemGroup4.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem36, spreadsheetCommandGalleryItem37, spreadsheetCommandGalleryItem38, spreadsheetCommandGalleryItem39, spreadsheetCommandGalleryItem40, spreadsheetCommandGalleryItem41, spreadsheetCommandGalleryItem42 });
		spreadsheetCommandGalleryItemGroup5.CommandName = "ConditionalFormattingIconSetsShapesCommandGroup";
		spreadsheetCommandGalleryItem43.CommandName = "ConditionalFormattingIconSetTrafficLights3";
		spreadsheetCommandGalleryItem44.CommandName = "ConditionalFormattingIconSetTrafficLights3Rimmed";
		spreadsheetCommandGalleryItem45.CommandName = "ConditionalFormattingIconSetTrafficLights4";
		spreadsheetCommandGalleryItem46.CommandName = "ConditionalFormattingIconSetSigns3";
		spreadsheetCommandGalleryItem47.CommandName = "ConditionalFormattingIconSetRedToBlack";
		spreadsheetCommandGalleryItemGroup5.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem43, spreadsheetCommandGalleryItem44, spreadsheetCommandGalleryItem45, spreadsheetCommandGalleryItem46, spreadsheetCommandGalleryItem47 });
		spreadsheetCommandGalleryItemGroup6.CommandName = "ConditionalFormattingIconSetsIndicatorsCommandGroup";
		spreadsheetCommandGalleryItem48.CommandName = "ConditionalFormattingIconSetSymbols3Circled";
		spreadsheetCommandGalleryItem49.CommandName = "ConditionalFormattingIconSetSymbols3";
		spreadsheetCommandGalleryItem50.CommandName = "ConditionalFormattingIconSetFlags3";
		spreadsheetCommandGalleryItemGroup6.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem48, spreadsheetCommandGalleryItem49, spreadsheetCommandGalleryItem50 });
		spreadsheetCommandGalleryItemGroup7.CommandName = "ConditionalFormattingIconSetsRatingsCommandGroup";
		spreadsheetCommandGalleryItem51.CommandName = "ConditionalFormattingIconSetStars3";
		spreadsheetCommandGalleryItem52.CommandName = "ConditionalFormattingIconSetRatings4";
		spreadsheetCommandGalleryItem53.CommandName = "ConditionalFormattingIconSetRatings5";
		spreadsheetCommandGalleryItem54.CommandName = "ConditionalFormattingIconSetQuarters5";
		spreadsheetCommandGalleryItem55.CommandName = "ConditionalFormattingIconSetBoxes5";
		spreadsheetCommandGalleryItemGroup7.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem51, spreadsheetCommandGalleryItem52, spreadsheetCommandGalleryItem53, spreadsheetCommandGalleryItem54, spreadsheetCommandGalleryItem55 });
		this.commandBarGalleryDropDown4.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[4] { spreadsheetCommandGalleryItemGroup4, spreadsheetCommandGalleryItemGroup5, spreadsheetCommandGalleryItemGroup6, spreadsheetCommandGalleryItemGroup7 });
		this.commandBarGalleryDropDown4.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown4.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown4.Name = "commandBarGalleryDropDown4";
		this.commandBarGalleryDropDown4.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonItem59.CommandName = "NewConditionalFormattingRule";
		this.spreadsheetCommandBarButtonItem59.Id = 97;
		this.spreadsheetCommandBarButtonItem59.Name = "spreadsheetCommandBarButtonItem59";
		this.spreadsheetCommandBarButtonItem60.CommandName = "ConditionalFormattingRemoveFromSheet";
		this.spreadsheetCommandBarButtonItem60.Id = 98;
		this.spreadsheetCommandBarButtonItem60.Name = "spreadsheetCommandBarButtonItem60";
		this.spreadsheetCommandBarButtonItem61.CommandName = "ConditionalFormattingRemove";
		this.spreadsheetCommandBarButtonItem61.Id = 99;
		this.spreadsheetCommandBarButtonItem61.Name = "spreadsheetCommandBarButtonItem61";
		this.spreadsheetCommandBarSubItem7.CommandName = "ConditionalFormattingRemoveCommandGroup";
		this.spreadsheetCommandBarSubItem7.Id = 100;
		this.spreadsheetCommandBarSubItem7.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem60),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem61)
		});
		this.spreadsheetCommandBarSubItem7.Name = "spreadsheetCommandBarSubItem7";
		this.spreadsheetCommandBarButtonItem62.CommandName = "ConditionalFormattingRulesManager";
		this.spreadsheetCommandBarButtonItem62.Id = 101;
		this.spreadsheetCommandBarButtonItem62.Name = "spreadsheetCommandBarButtonItem62";
		this.galleryFormatAsTableItem1.DropDownControl = this.commandBarGalleryDropDown5;
		this.galleryFormatAsTableItem1.Id = 102;
		this.galleryFormatAsTableItem1.Name = "galleryFormatAsTableItem1";
		this.commandBarGalleryDropDown5.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown5.Gallery.ColumnCount = 7;
		this.commandBarGalleryDropDown5.Gallery.DrawImageBackground = false;
		this.commandBarGalleryDropDown5.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.commandBarGalleryDropDown5.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.commandBarGalleryDropDown5.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown5.Name = "commandBarGalleryDropDown5";
		this.commandBarGalleryDropDown5.Ribbon = this.ribbonControl1;
		this.galleryChangeStyleItem1.Gallery.DrawImageBackground = false;
		this.galleryChangeStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChangeStyleItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.galleryChangeStyleItem1.Gallery.ItemSize = new System.Drawing.Size(106, 28);
		this.galleryChangeStyleItem1.Gallery.RowCount = 9;
		this.galleryChangeStyleItem1.Gallery.ShowItemText = true;
		this.galleryChangeStyleItem1.Id = 103;
		this.galleryChangeStyleItem1.Name = "galleryChangeStyleItem1";
		this.spreadsheetCommandBarSubItem8.CommandName = "InsertCellsCommandGroup";
		this.spreadsheetCommandBarSubItem8.Id = 104;
		this.spreadsheetCommandBarSubItem8.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem63),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem64),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem65)
		});
		this.spreadsheetCommandBarSubItem8.Name = "spreadsheetCommandBarSubItem8";
		this.spreadsheetCommandBarButtonItem63.CommandName = "InsertSheetRows";
		this.spreadsheetCommandBarButtonItem63.Id = 105;
		this.spreadsheetCommandBarButtonItem63.Name = "spreadsheetCommandBarButtonItem63";
		this.spreadsheetCommandBarButtonItem64.CommandName = "InsertSheetColumns";
		this.spreadsheetCommandBarButtonItem64.Id = 106;
		this.spreadsheetCommandBarButtonItem64.Name = "spreadsheetCommandBarButtonItem64";
		this.spreadsheetCommandBarButtonItem65.CommandName = "InsertSheet";
		this.spreadsheetCommandBarButtonItem65.Id = 107;
		this.spreadsheetCommandBarButtonItem65.Name = "spreadsheetCommandBarButtonItem65";
		this.spreadsheetCommandBarSubItem9.CommandName = "RemoveCellsCommandGroup";
		this.spreadsheetCommandBarSubItem9.Id = 108;
		this.spreadsheetCommandBarSubItem9.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem66),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem67),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem68)
		});
		this.spreadsheetCommandBarSubItem9.Name = "spreadsheetCommandBarSubItem9";
		this.spreadsheetCommandBarButtonItem66.CommandName = "RemoveSheetRows";
		this.spreadsheetCommandBarButtonItem66.Id = 109;
		this.spreadsheetCommandBarButtonItem66.Name = "spreadsheetCommandBarButtonItem66";
		this.spreadsheetCommandBarButtonItem67.CommandName = "RemoveSheetColumns";
		this.spreadsheetCommandBarButtonItem67.Id = 110;
		this.spreadsheetCommandBarButtonItem67.Name = "spreadsheetCommandBarButtonItem67";
		this.spreadsheetCommandBarButtonItem68.CommandName = "RemoveSheet";
		this.spreadsheetCommandBarButtonItem68.Id = 111;
		this.spreadsheetCommandBarButtonItem68.Name = "spreadsheetCommandBarButtonItem68";
		this.spreadsheetCommandBarSubItem10.CommandName = "FormatCommandGroup";
		this.spreadsheetCommandBarSubItem10.Id = 112;
		this.spreadsheetCommandBarSubItem10.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[12]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem69),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem70),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem71),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem72),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem73),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarSubItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem80),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem81),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeSheetTabColorItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem82),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem83)
		});
		this.spreadsheetCommandBarSubItem10.Name = "spreadsheetCommandBarSubItem10";
		this.spreadsheetCommandBarButtonItem69.CommandName = "FormatRowHeight";
		this.spreadsheetCommandBarButtonItem69.Id = 113;
		this.spreadsheetCommandBarButtonItem69.Name = "spreadsheetCommandBarButtonItem69";
		this.spreadsheetCommandBarButtonItem70.CommandName = "FormatAutoFitRowHeight";
		this.spreadsheetCommandBarButtonItem70.Id = 114;
		this.spreadsheetCommandBarButtonItem70.Name = "spreadsheetCommandBarButtonItem70";
		this.spreadsheetCommandBarButtonItem71.CommandName = "FormatColumnWidth";
		this.spreadsheetCommandBarButtonItem71.Id = 115;
		this.spreadsheetCommandBarButtonItem71.Name = "spreadsheetCommandBarButtonItem71";
		this.spreadsheetCommandBarButtonItem72.CommandName = "FormatAutoFitColumnWidth";
		this.spreadsheetCommandBarButtonItem72.Id = 116;
		this.spreadsheetCommandBarButtonItem72.Name = "spreadsheetCommandBarButtonItem72";
		this.spreadsheetCommandBarButtonItem73.CommandName = "FormatDefaultColumnWidth";
		this.spreadsheetCommandBarButtonItem73.Id = 117;
		this.spreadsheetCommandBarButtonItem73.Name = "spreadsheetCommandBarButtonItem73";
		this.spreadsheetCommandBarSubItem11.CommandName = "HideAndUnhideCommandGroup";
		this.spreadsheetCommandBarSubItem11.Id = 124;
		this.spreadsheetCommandBarSubItem11.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem74),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem75),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem76),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem77),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem78),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem79)
		});
		this.spreadsheetCommandBarSubItem11.Name = "spreadsheetCommandBarSubItem11";
		this.spreadsheetCommandBarButtonItem74.CommandName = "HideRows";
		this.spreadsheetCommandBarButtonItem74.Id = 118;
		this.spreadsheetCommandBarButtonItem74.Name = "spreadsheetCommandBarButtonItem74";
		this.spreadsheetCommandBarButtonItem75.CommandName = "HideColumns";
		this.spreadsheetCommandBarButtonItem75.Id = 119;
		this.spreadsheetCommandBarButtonItem75.Name = "spreadsheetCommandBarButtonItem75";
		this.spreadsheetCommandBarButtonItem76.CommandName = "HideSheet";
		this.spreadsheetCommandBarButtonItem76.Id = 120;
		this.spreadsheetCommandBarButtonItem76.Name = "spreadsheetCommandBarButtonItem76";
		this.spreadsheetCommandBarButtonItem77.CommandName = "UnhideRows";
		this.spreadsheetCommandBarButtonItem77.Id = 121;
		this.spreadsheetCommandBarButtonItem77.Name = "spreadsheetCommandBarButtonItem77";
		this.spreadsheetCommandBarButtonItem78.CommandName = "UnhideColumns";
		this.spreadsheetCommandBarButtonItem78.Id = 122;
		this.spreadsheetCommandBarButtonItem78.Name = "spreadsheetCommandBarButtonItem78";
		this.spreadsheetCommandBarButtonItem79.CommandName = "UnhideSheet";
		this.spreadsheetCommandBarButtonItem79.Id = 123;
		this.spreadsheetCommandBarButtonItem79.Name = "spreadsheetCommandBarButtonItem79";
		this.spreadsheetCommandBarButtonItem80.CommandName = "RenameSheet";
		this.spreadsheetCommandBarButtonItem80.Id = 125;
		this.spreadsheetCommandBarButtonItem80.Name = "spreadsheetCommandBarButtonItem80";
		this.spreadsheetCommandBarButtonItem81.CommandName = "MoveOrCopySheet";
		this.spreadsheetCommandBarButtonItem81.Id = 126;
		this.spreadsheetCommandBarButtonItem81.Name = "spreadsheetCommandBarButtonItem81";
		this.changeSheetTabColorItem1.ActAsDropDown = true;
		this.changeSheetTabColorItem1.Id = 127;
		this.changeSheetTabColorItem1.Name = "changeSheetTabColorItem1";
		this.spreadsheetCommandBarButtonItem82.CommandName = "ReviewProtectSheet";
		this.spreadsheetCommandBarButtonItem82.Id = 128;
		this.spreadsheetCommandBarButtonItem82.Name = "spreadsheetCommandBarButtonItem82";
		this.spreadsheetCommandBarCheckItem13.CommandName = "FormatCellLocked";
		this.spreadsheetCommandBarCheckItem13.Id = 129;
		this.spreadsheetCommandBarCheckItem13.Name = "spreadsheetCommandBarCheckItem13";
		this.spreadsheetCommandBarButtonItem83.CommandName = "FormatCellsContextMenuItem";
		this.spreadsheetCommandBarButtonItem83.Id = 130;
		this.spreadsheetCommandBarButtonItem83.Name = "spreadsheetCommandBarButtonItem83";
		this.spreadsheetCommandBarSubItem12.CommandName = "EditingAutoSumCommandGroup";
		this.spreadsheetCommandBarSubItem12.Id = 131;
		this.spreadsheetCommandBarSubItem12.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem84),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem85),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem86),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem87),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem88)
		});
		this.spreadsheetCommandBarSubItem12.Name = "spreadsheetCommandBarSubItem12";
		this.spreadsheetCommandBarSubItem12.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem84.CommandName = "FunctionsInsertSum";
		this.spreadsheetCommandBarButtonItem84.Id = 132;
		this.spreadsheetCommandBarButtonItem84.Name = "spreadsheetCommandBarButtonItem84";
		this.spreadsheetCommandBarButtonItem85.CommandName = "FunctionsInsertAverage";
		this.spreadsheetCommandBarButtonItem85.Id = 133;
		this.spreadsheetCommandBarButtonItem85.Name = "spreadsheetCommandBarButtonItem85";
		this.spreadsheetCommandBarButtonItem86.CommandName = "FunctionsInsertCountNumbers";
		this.spreadsheetCommandBarButtonItem86.Id = 134;
		this.spreadsheetCommandBarButtonItem86.Name = "spreadsheetCommandBarButtonItem86";
		this.spreadsheetCommandBarButtonItem87.CommandName = "FunctionsInsertMax";
		this.spreadsheetCommandBarButtonItem87.Id = 135;
		this.spreadsheetCommandBarButtonItem87.Name = "spreadsheetCommandBarButtonItem87";
		this.spreadsheetCommandBarButtonItem88.CommandName = "FunctionsInsertMin";
		this.spreadsheetCommandBarButtonItem88.Id = 136;
		this.spreadsheetCommandBarButtonItem88.Name = "spreadsheetCommandBarButtonItem88";
		this.spreadsheetCommandBarSubItem13.CommandName = "EditingFillCommandGroup";
		this.spreadsheetCommandBarSubItem13.Id = 137;
		this.spreadsheetCommandBarSubItem13.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem89),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem90),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem91),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem92)
		});
		this.spreadsheetCommandBarSubItem13.Name = "spreadsheetCommandBarSubItem13";
		this.spreadsheetCommandBarSubItem13.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem89.CommandName = "EditingFillDown";
		this.spreadsheetCommandBarButtonItem89.Id = 138;
		this.spreadsheetCommandBarButtonItem89.Name = "spreadsheetCommandBarButtonItem89";
		this.spreadsheetCommandBarButtonItem90.CommandName = "EditingFillRight";
		this.spreadsheetCommandBarButtonItem90.Id = 139;
		this.spreadsheetCommandBarButtonItem90.Name = "spreadsheetCommandBarButtonItem90";
		this.spreadsheetCommandBarButtonItem91.CommandName = "EditingFillUp";
		this.spreadsheetCommandBarButtonItem91.Id = 140;
		this.spreadsheetCommandBarButtonItem91.Name = "spreadsheetCommandBarButtonItem91";
		this.spreadsheetCommandBarButtonItem92.CommandName = "EditingFillLeft";
		this.spreadsheetCommandBarButtonItem92.Id = 141;
		this.spreadsheetCommandBarButtonItem92.Name = "spreadsheetCommandBarButtonItem92";
		this.spreadsheetCommandBarSubItem14.CommandName = "FormatClearCommandGroup";
		this.spreadsheetCommandBarSubItem14.Id = 142;
		this.spreadsheetCommandBarSubItem14.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem93),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem94),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem95),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem96),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem97),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem98)
		});
		this.spreadsheetCommandBarSubItem14.Name = "spreadsheetCommandBarSubItem14";
		this.spreadsheetCommandBarSubItem14.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem93.CommandName = "FormatClearAll";
		this.spreadsheetCommandBarButtonItem93.Id = 143;
		this.spreadsheetCommandBarButtonItem93.Name = "spreadsheetCommandBarButtonItem93";
		this.spreadsheetCommandBarButtonItem94.CommandName = "FormatClearFormats";
		this.spreadsheetCommandBarButtonItem94.Id = 144;
		this.spreadsheetCommandBarButtonItem94.Name = "spreadsheetCommandBarButtonItem94";
		this.spreadsheetCommandBarButtonItem95.CommandName = "FormatClearContents";
		this.spreadsheetCommandBarButtonItem95.Id = 145;
		this.spreadsheetCommandBarButtonItem95.Name = "spreadsheetCommandBarButtonItem95";
		this.spreadsheetCommandBarButtonItem96.CommandName = "FormatClearComments";
		this.spreadsheetCommandBarButtonItem96.Id = 146;
		this.spreadsheetCommandBarButtonItem96.Name = "spreadsheetCommandBarButtonItem96";
		this.spreadsheetCommandBarButtonItem97.CommandName = "FormatClearHyperlinks";
		this.spreadsheetCommandBarButtonItem97.Id = 147;
		this.spreadsheetCommandBarButtonItem97.Name = "spreadsheetCommandBarButtonItem97";
		this.spreadsheetCommandBarButtonItem98.CommandName = "FormatRemoveHyperlinks";
		this.spreadsheetCommandBarButtonItem98.Id = 148;
		this.spreadsheetCommandBarButtonItem98.Name = "spreadsheetCommandBarButtonItem98";
		this.spreadsheetCommandBarSubItem15.CommandName = "EditingSortAndFilterCommandGroup";
		this.spreadsheetCommandBarSubItem15.Id = 149;
		this.spreadsheetCommandBarSubItem15.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem99),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem100),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem101),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem102)
		});
		this.spreadsheetCommandBarSubItem15.Name = "spreadsheetCommandBarSubItem15";
		this.spreadsheetCommandBarButtonItem99.CommandName = "DataSortAscending";
		this.spreadsheetCommandBarButtonItem99.Id = 150;
		this.spreadsheetCommandBarButtonItem99.Name = "spreadsheetCommandBarButtonItem99";
		this.spreadsheetCommandBarButtonItem100.CommandName = "DataSortDescending";
		this.spreadsheetCommandBarButtonItem100.Id = 151;
		this.spreadsheetCommandBarButtonItem100.Name = "spreadsheetCommandBarButtonItem100";
		this.spreadsheetCommandBarCheckItem14.CommandName = "DataFilterToggle";
		this.spreadsheetCommandBarCheckItem14.Id = 152;
		this.spreadsheetCommandBarCheckItem14.Name = "spreadsheetCommandBarCheckItem14";
		this.spreadsheetCommandBarButtonItem101.CommandName = "DataFilterClear";
		this.spreadsheetCommandBarButtonItem101.Id = 153;
		this.spreadsheetCommandBarButtonItem101.Name = "spreadsheetCommandBarButtonItem101";
		this.spreadsheetCommandBarButtonItem102.CommandName = "DataFilterReApply";
		this.spreadsheetCommandBarButtonItem102.Id = 154;
		this.spreadsheetCommandBarButtonItem102.Name = "spreadsheetCommandBarButtonItem102";
		this.spreadsheetCommandBarSubItem16.CommandName = "EditingFindAndSelectCommandGroup";
		this.spreadsheetCommandBarSubItem16.Id = 155;
		this.spreadsheetCommandBarSubItem16.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem103),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem104),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem105),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem106),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem107),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem108),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem109)
		});
		this.spreadsheetCommandBarSubItem16.Name = "spreadsheetCommandBarSubItem16";
		this.spreadsheetCommandBarButtonItem103.CommandName = "EditingFind";
		this.spreadsheetCommandBarButtonItem103.Id = 156;
		this.spreadsheetCommandBarButtonItem103.Name = "spreadsheetCommandBarButtonItem103";
		this.spreadsheetCommandBarButtonItem104.CommandName = "EditingReplace";
		this.spreadsheetCommandBarButtonItem104.Id = 157;
		this.spreadsheetCommandBarButtonItem104.Name = "spreadsheetCommandBarButtonItem104";
		this.spreadsheetCommandBarButtonItem105.CommandName = "EditingSelectFormulas";
		this.spreadsheetCommandBarButtonItem105.Id = 158;
		this.spreadsheetCommandBarButtonItem105.Name = "spreadsheetCommandBarButtonItem105";
		this.spreadsheetCommandBarButtonItem106.CommandName = "EditingSelectComments";
		this.spreadsheetCommandBarButtonItem106.Id = 159;
		this.spreadsheetCommandBarButtonItem106.Name = "spreadsheetCommandBarButtonItem106";
		this.spreadsheetCommandBarButtonItem107.CommandName = "EditingSelectConditionalFormatting";
		this.spreadsheetCommandBarButtonItem107.Id = 160;
		this.spreadsheetCommandBarButtonItem107.Name = "spreadsheetCommandBarButtonItem107";
		this.spreadsheetCommandBarButtonItem108.CommandName = "EditingSelectConstants";
		this.spreadsheetCommandBarButtonItem108.Id = 161;
		this.spreadsheetCommandBarButtonItem108.Name = "spreadsheetCommandBarButtonItem108";
		this.spreadsheetCommandBarButtonItem109.CommandName = "EditingSelectDataValidation";
		this.spreadsheetCommandBarButtonItem109.Id = 162;
		this.spreadsheetCommandBarButtonItem109.Name = "spreadsheetCommandBarButtonItem109";
		this.spreadsheetCommandBarButtonItem110.CommandName = "InsertPivotTable";
		this.spreadsheetCommandBarButtonItem110.Id = 163;
		this.spreadsheetCommandBarButtonItem110.Name = "spreadsheetCommandBarButtonItem110";
		this.spreadsheetCommandBarButtonItem111.CommandName = "InsertTable";
		this.spreadsheetCommandBarButtonItem111.Id = 164;
		this.spreadsheetCommandBarButtonItem111.Name = "spreadsheetCommandBarButtonItem111";
		this.spreadsheetCommandBarButtonItem112.CommandName = "InsertPicture";
		this.spreadsheetCommandBarButtonItem112.Id = 165;
		this.spreadsheetCommandBarButtonItem112.Name = "spreadsheetCommandBarButtonItem112";
		this.spreadsheetCommandBarButtonGalleryDropDownItem4.CommandName = "InsertChartColumnCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem4.DropDownControl = this.commandBarGalleryDropDown6;
		this.spreadsheetCommandBarButtonGalleryDropDownItem4.Id = 166;
		this.spreadsheetCommandBarButtonGalleryDropDownItem4.Name = "spreadsheetCommandBarButtonGalleryDropDownItem4";
		this.commandBarGalleryDropDown6.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup8.CommandName = "InsertChartColumn2DCommandGroup";
		spreadsheetCommandGalleryItem56.CommandName = "InsertChartColumnClustered2D";
		spreadsheetCommandGalleryItem57.CommandName = "InsertChartColumnStacked2D";
		spreadsheetCommandGalleryItem58.CommandName = "InsertChartColumnPercentStacked2D";
		spreadsheetCommandGalleryItemGroup8.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem56, spreadsheetCommandGalleryItem57, spreadsheetCommandGalleryItem58 });
		spreadsheetCommandGalleryItemGroup9.CommandName = "InsertChartColumn3DCommandGroup";
		spreadsheetCommandGalleryItem59.CommandName = "InsertChartColumnClustered3D";
		spreadsheetCommandGalleryItem60.CommandName = "InsertChartColumnStacked3D";
		spreadsheetCommandGalleryItem61.CommandName = "InsertChartColumnPercentStacked3D";
		spreadsheetCommandGalleryItem62.CommandName = "InsertChartColumn3D";
		spreadsheetCommandGalleryItemGroup9.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem59, spreadsheetCommandGalleryItem60, spreadsheetCommandGalleryItem61, spreadsheetCommandGalleryItem62 });
		spreadsheetCommandGalleryItemGroup10.CommandName = "InsertChartCylinderCommandGroup";
		spreadsheetCommandGalleryItem63.CommandName = "InsertChartCylinderClustered";
		spreadsheetCommandGalleryItem64.CommandName = "InsertChartCylinderStacked";
		spreadsheetCommandGalleryItem65.CommandName = "InsertChartCylinderPercentStacked";
		spreadsheetCommandGalleryItem66.CommandName = "InsertChartCylinder";
		spreadsheetCommandGalleryItemGroup10.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem63, spreadsheetCommandGalleryItem64, spreadsheetCommandGalleryItem65, spreadsheetCommandGalleryItem66 });
		spreadsheetCommandGalleryItemGroup11.CommandName = "InsertChartConeCommandGroup";
		spreadsheetCommandGalleryItem67.CommandName = "InsertChartConeClustered";
		spreadsheetCommandGalleryItem68.CommandName = "InsertChartConeStacked";
		spreadsheetCommandGalleryItem69.CommandName = "InsertChartConePercentStacked";
		spreadsheetCommandGalleryItem70.CommandName = "InsertChartCone";
		spreadsheetCommandGalleryItemGroup11.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem67, spreadsheetCommandGalleryItem68, spreadsheetCommandGalleryItem69, spreadsheetCommandGalleryItem70 });
		spreadsheetCommandGalleryItemGroup12.CommandName = "InsertChartPyramidCommandGroup";
		spreadsheetCommandGalleryItem71.CommandName = "InsertChartPyramidClustered";
		spreadsheetCommandGalleryItem72.CommandName = "InsertChartPyramidStacked";
		spreadsheetCommandGalleryItem73.CommandName = "InsertChartPyramidPercentStacked";
		spreadsheetCommandGalleryItem74.CommandName = "InsertChartPyramid";
		spreadsheetCommandGalleryItemGroup12.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem71, spreadsheetCommandGalleryItem72, spreadsheetCommandGalleryItem73, spreadsheetCommandGalleryItem74 });
		this.commandBarGalleryDropDown6.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[5] { spreadsheetCommandGalleryItemGroup8, spreadsheetCommandGalleryItemGroup9, spreadsheetCommandGalleryItemGroup10, spreadsheetCommandGalleryItemGroup11, spreadsheetCommandGalleryItemGroup12 });
		this.commandBarGalleryDropDown6.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown6.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown6.Name = "commandBarGalleryDropDown6";
		this.commandBarGalleryDropDown6.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem5.CommandName = "InsertChartLineCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem5.DropDownControl = this.commandBarGalleryDropDown7;
		this.spreadsheetCommandBarButtonGalleryDropDownItem5.Id = 167;
		this.spreadsheetCommandBarButtonGalleryDropDownItem5.Name = "spreadsheetCommandBarButtonGalleryDropDownItem5";
		this.commandBarGalleryDropDown7.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup13.CommandName = "InsertChartLine2DCommandGroup";
		spreadsheetCommandGalleryItem75.CommandName = "InsertChartLine";
		spreadsheetCommandGalleryItem76.CommandName = "InsertChartStackedLine";
		spreadsheetCommandGalleryItem77.CommandName = "InsertChartPercentStackedLine";
		spreadsheetCommandGalleryItem78.CommandName = "InsertChartLineWithMarkers";
		spreadsheetCommandGalleryItem79.CommandName = "InsertChartStackedLineWithMarkers";
		spreadsheetCommandGalleryItem80.CommandName = "InsertChartPercentStackedLineWithMarkers";
		spreadsheetCommandGalleryItemGroup13.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { spreadsheetCommandGalleryItem75, spreadsheetCommandGalleryItem76, spreadsheetCommandGalleryItem77, spreadsheetCommandGalleryItem78, spreadsheetCommandGalleryItem79, spreadsheetCommandGalleryItem80 });
		spreadsheetCommandGalleryItemGroup14.CommandName = "InsertChartLine3DCommandGroup";
		spreadsheetCommandGalleryItem81.CommandName = "InsertChartLine3D";
		spreadsheetCommandGalleryItemGroup14.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[1] { spreadsheetCommandGalleryItem81 });
		this.commandBarGalleryDropDown7.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup13, spreadsheetCommandGalleryItemGroup14 });
		this.commandBarGalleryDropDown7.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown7.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown7.Name = "commandBarGalleryDropDown7";
		this.commandBarGalleryDropDown7.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem6.CommandName = "InsertChartPieCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem6.DropDownControl = this.commandBarGalleryDropDown8;
		this.spreadsheetCommandBarButtonGalleryDropDownItem6.Id = 168;
		this.spreadsheetCommandBarButtonGalleryDropDownItem6.Name = "spreadsheetCommandBarButtonGalleryDropDownItem6";
		this.commandBarGalleryDropDown8.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup15.CommandName = "InsertChartPie2DCommandGroup";
		spreadsheetCommandGalleryItem82.CommandName = "InsertChartPie2D";
		spreadsheetCommandGalleryItem83.CommandName = "InsertChartPieExploded2D";
		spreadsheetCommandGalleryItemGroup15.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem82, spreadsheetCommandGalleryItem83 });
		spreadsheetCommandGalleryItemGroup16.CommandName = "InsertChartPie3DCommandGroup";
		spreadsheetCommandGalleryItem84.CommandName = "InsertChartPie3D";
		spreadsheetCommandGalleryItem85.CommandName = "InsertChartPieExploded3D";
		spreadsheetCommandGalleryItemGroup16.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem84, spreadsheetCommandGalleryItem85 });
		spreadsheetCommandGalleryItemGroup17.CommandName = "InsertChartDoughnut2DCommandGroup";
		spreadsheetCommandGalleryItem86.CommandName = "InsertChartDoughnut2D";
		spreadsheetCommandGalleryItem87.CommandName = "InsertChartDoughnutExploded2D";
		spreadsheetCommandGalleryItemGroup17.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem86, spreadsheetCommandGalleryItem87 });
		this.commandBarGalleryDropDown8.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[3] { spreadsheetCommandGalleryItemGroup15, spreadsheetCommandGalleryItemGroup16, spreadsheetCommandGalleryItemGroup17 });
		this.commandBarGalleryDropDown8.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown8.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown8.Name = "commandBarGalleryDropDown8";
		this.commandBarGalleryDropDown8.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem7.CommandName = "InsertChartBarCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem7.DropDownControl = this.commandBarGalleryDropDown9;
		this.spreadsheetCommandBarButtonGalleryDropDownItem7.Id = 169;
		this.spreadsheetCommandBarButtonGalleryDropDownItem7.Name = "spreadsheetCommandBarButtonGalleryDropDownItem7";
		this.commandBarGalleryDropDown9.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup18.CommandName = "InsertChartBar2DCommandGroup";
		spreadsheetCommandGalleryItem88.CommandName = "InsertChartBarClustered2D";
		spreadsheetCommandGalleryItem89.CommandName = "InsertChartBarStacked2D";
		spreadsheetCommandGalleryItem90.CommandName = "InsertChartBarPercentStacked2D";
		spreadsheetCommandGalleryItemGroup18.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem88, spreadsheetCommandGalleryItem89, spreadsheetCommandGalleryItem90 });
		spreadsheetCommandGalleryItemGroup19.CommandName = "InsertChartBar3DCommandGroup";
		spreadsheetCommandGalleryItem91.CommandName = "InsertChartBarClustered3D";
		spreadsheetCommandGalleryItem92.CommandName = "InsertChartBarStacked3D";
		spreadsheetCommandGalleryItem93.CommandName = "InsertChartBarPercentStacked3D";
		spreadsheetCommandGalleryItemGroup19.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem91, spreadsheetCommandGalleryItem92, spreadsheetCommandGalleryItem93 });
		spreadsheetCommandGalleryItemGroup20.CommandName = "InsertChartHorizontalCylinderCommandGroup";
		spreadsheetCommandGalleryItem94.CommandName = "InsertChartHorizontalCylinderClustered";
		spreadsheetCommandGalleryItem95.CommandName = "InsertChartHorizontalCylinderStacked";
		spreadsheetCommandGalleryItem96.CommandName = "InsertChartHorizontalCylinderPercentStacked";
		spreadsheetCommandGalleryItemGroup20.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem94, spreadsheetCommandGalleryItem95, spreadsheetCommandGalleryItem96 });
		spreadsheetCommandGalleryItemGroup21.CommandName = "InsertChartHorizontalConeCommandGroup";
		spreadsheetCommandGalleryItem97.CommandName = "InsertChartHorizontalConeClustered";
		spreadsheetCommandGalleryItem98.CommandName = "InsertChartHorizontalConeStacked";
		spreadsheetCommandGalleryItem99.CommandName = "InsertChartHorizontalConePercentStacked";
		spreadsheetCommandGalleryItemGroup21.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem97, spreadsheetCommandGalleryItem98, spreadsheetCommandGalleryItem99 });
		spreadsheetCommandGalleryItemGroup22.CommandName = "InsertChartHorizontalPyramidCommandGroup";
		spreadsheetCommandGalleryItem100.CommandName = "InsertChartHorizontalPyramidClustered";
		spreadsheetCommandGalleryItem101.CommandName = "InsertChartHorizontalPyramidStacked";
		spreadsheetCommandGalleryItem102.CommandName = "InsertChartHorizontalPyramidPercentStacked";
		spreadsheetCommandGalleryItemGroup22.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem100, spreadsheetCommandGalleryItem101, spreadsheetCommandGalleryItem102 });
		this.commandBarGalleryDropDown9.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[5] { spreadsheetCommandGalleryItemGroup18, spreadsheetCommandGalleryItemGroup19, spreadsheetCommandGalleryItemGroup20, spreadsheetCommandGalleryItemGroup21, spreadsheetCommandGalleryItemGroup22 });
		this.commandBarGalleryDropDown9.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown9.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown9.Name = "commandBarGalleryDropDown9";
		this.commandBarGalleryDropDown9.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem8.CommandName = "InsertChartAreaCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem8.DropDownControl = this.commandBarGalleryDropDown10;
		this.spreadsheetCommandBarButtonGalleryDropDownItem8.Id = 170;
		this.spreadsheetCommandBarButtonGalleryDropDownItem8.Name = "spreadsheetCommandBarButtonGalleryDropDownItem8";
		this.commandBarGalleryDropDown10.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup23.CommandName = "InsertChartArea2DCommandGroup";
		spreadsheetCommandGalleryItem103.CommandName = "InsertChartArea";
		spreadsheetCommandGalleryItem104.CommandName = "InsertChartStackedArea";
		spreadsheetCommandGalleryItem105.CommandName = "InsertChartPercentStackedArea";
		spreadsheetCommandGalleryItemGroup23.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem103, spreadsheetCommandGalleryItem104, spreadsheetCommandGalleryItem105 });
		spreadsheetCommandGalleryItemGroup24.CommandName = "InsertChartArea3DCommandGroup";
		spreadsheetCommandGalleryItem106.CommandName = "InsertChartArea3D";
		spreadsheetCommandGalleryItem107.CommandName = "InsertChartStackedArea3D";
		spreadsheetCommandGalleryItem108.CommandName = "InsertChartPercentStackedArea3D";
		spreadsheetCommandGalleryItemGroup24.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem106, spreadsheetCommandGalleryItem107, spreadsheetCommandGalleryItem108 });
		this.commandBarGalleryDropDown10.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup23, spreadsheetCommandGalleryItemGroup24 });
		this.commandBarGalleryDropDown10.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown10.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown10.Name = "commandBarGalleryDropDown10";
		this.commandBarGalleryDropDown10.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem9.CommandName = "InsertChartScatterCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem9.DropDownControl = this.commandBarGalleryDropDown11;
		this.spreadsheetCommandBarButtonGalleryDropDownItem9.Id = 171;
		this.spreadsheetCommandBarButtonGalleryDropDownItem9.Name = "spreadsheetCommandBarButtonGalleryDropDownItem9";
		this.commandBarGalleryDropDown11.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup25.CommandName = "InsertChartScatterCommandGroup";
		spreadsheetCommandGalleryItem109.CommandName = "InsertChartScatterMarkers";
		spreadsheetCommandGalleryItem110.CommandName = "InsertChartScatterSmoothLinesAndMarkers";
		spreadsheetCommandGalleryItem111.CommandName = "InsertChartScatterSmoothLines";
		spreadsheetCommandGalleryItem112.CommandName = "InsertChartScatterLinesAndMarkers";
		spreadsheetCommandGalleryItem113.CommandName = "InsertChartScatterLines";
		spreadsheetCommandGalleryItemGroup25.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem109, spreadsheetCommandGalleryItem110, spreadsheetCommandGalleryItem111, spreadsheetCommandGalleryItem112, spreadsheetCommandGalleryItem113 });
		spreadsheetCommandGalleryItemGroup26.CommandName = "InsertChartBubbleCommandGroup";
		spreadsheetCommandGalleryItem114.CommandName = "InsertChartBubble";
		spreadsheetCommandGalleryItem115.CommandName = "InsertChartBubble3D";
		spreadsheetCommandGalleryItemGroup26.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem114, spreadsheetCommandGalleryItem115 });
		this.commandBarGalleryDropDown11.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup25, spreadsheetCommandGalleryItemGroup26 });
		this.commandBarGalleryDropDown11.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown11.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown11.Name = "commandBarGalleryDropDown11";
		this.commandBarGalleryDropDown11.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem10.CommandName = "InsertChartOtherCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem10.DropDownControl = this.commandBarGalleryDropDown12;
		this.spreadsheetCommandBarButtonGalleryDropDownItem10.Id = 172;
		this.spreadsheetCommandBarButtonGalleryDropDownItem10.Name = "spreadsheetCommandBarButtonGalleryDropDownItem10";
		this.commandBarGalleryDropDown12.Gallery.AllowFilter = false;
		spreadsheetCommandGalleryItemGroup27.CommandName = "InsertChartStockCommandGroup";
		spreadsheetCommandGalleryItem116.CommandName = "InsertChartStockHighLowClose";
		spreadsheetCommandGalleryItem117.CommandName = "InsertChartStockOpenHighLowClose";
		spreadsheetCommandGalleryItem118.CommandName = "InsertChartStockVolumeHighLowClose";
		spreadsheetCommandGalleryItem119.CommandName = "InsertChartStockVolumeOpenHighLowClose";
		spreadsheetCommandGalleryItemGroup27.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem116, spreadsheetCommandGalleryItem117, spreadsheetCommandGalleryItem118, spreadsheetCommandGalleryItem119 });
		spreadsheetCommandGalleryItemGroup28.CommandName = "InsertChartRadarCommandGroup";
		spreadsheetCommandGalleryItem120.CommandName = "InsertChartRadar";
		spreadsheetCommandGalleryItem121.CommandName = "InsertChartRadarWithMarkers";
		spreadsheetCommandGalleryItem122.CommandName = "InsertChartRadarFilled";
		spreadsheetCommandGalleryItemGroup28.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem120, spreadsheetCommandGalleryItem121, spreadsheetCommandGalleryItem122 });
		this.commandBarGalleryDropDown12.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { spreadsheetCommandGalleryItemGroup27, spreadsheetCommandGalleryItemGroup28 });
		this.commandBarGalleryDropDown12.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown12.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown12.Name = "commandBarGalleryDropDown12";
		this.commandBarGalleryDropDown12.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonItem113.CommandName = "InsertHyperlink";
		this.spreadsheetCommandBarButtonItem113.Id = 173;
		this.spreadsheetCommandBarButtonItem113.Name = "spreadsheetCommandBarButtonItem113";
		this.spreadsheetCommandBarButtonItem114.CommandName = "InsertSymbol";
		this.spreadsheetCommandBarButtonItem114.Id = 174;
		this.spreadsheetCommandBarButtonItem114.Name = "spreadsheetCommandBarButtonItem114";
		this.spreadsheetCommandBarSubItem17.CommandName = "PageSetupMarginsCommandGroup";
		this.spreadsheetCommandBarSubItem17.Id = 175;
		this.spreadsheetCommandBarSubItem17.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem115)
		});
		this.spreadsheetCommandBarSubItem17.Name = "spreadsheetCommandBarSubItem17";
		this.spreadsheetCommandBarCheckItem15.CaptionDependOnUnits = true;
		this.spreadsheetCommandBarCheckItem15.CommandName = "PageSetupMarginsNormal";
		this.spreadsheetCommandBarCheckItem15.Id = 176;
		this.spreadsheetCommandBarCheckItem15.Name = "spreadsheetCommandBarCheckItem15";
		this.spreadsheetCommandBarCheckItem16.CaptionDependOnUnits = true;
		this.spreadsheetCommandBarCheckItem16.CommandName = "PageSetupMarginsWide";
		this.spreadsheetCommandBarCheckItem16.Id = 177;
		this.spreadsheetCommandBarCheckItem16.Name = "spreadsheetCommandBarCheckItem16";
		this.spreadsheetCommandBarCheckItem17.CaptionDependOnUnits = true;
		this.spreadsheetCommandBarCheckItem17.CommandName = "PageSetupMarginsNarrow";
		this.spreadsheetCommandBarCheckItem17.Id = 178;
		this.spreadsheetCommandBarCheckItem17.Name = "spreadsheetCommandBarCheckItem17";
		this.spreadsheetCommandBarButtonItem115.CommandName = "PageSetupCustomMargins";
		this.spreadsheetCommandBarButtonItem115.Id = 179;
		this.spreadsheetCommandBarButtonItem115.Name = "spreadsheetCommandBarButtonItem115";
		this.spreadsheetCommandBarSubItem18.CommandName = "PageSetupOrientationCommandGroup";
		this.spreadsheetCommandBarSubItem18.Id = 180;
		this.spreadsheetCommandBarSubItem18.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem19)
		});
		this.spreadsheetCommandBarSubItem18.Name = "spreadsheetCommandBarSubItem18";
		this.spreadsheetCommandBarCheckItem18.CommandName = "PageSetupOrientationPortrait";
		this.spreadsheetCommandBarCheckItem18.Id = 181;
		this.spreadsheetCommandBarCheckItem18.Name = "spreadsheetCommandBarCheckItem18";
		this.spreadsheetCommandBarCheckItem19.CommandName = "PageSetupOrientationLandscape";
		this.spreadsheetCommandBarCheckItem19.Id = 182;
		this.spreadsheetCommandBarCheckItem19.Name = "spreadsheetCommandBarCheckItem19";
		this.pageSetupPaperKindItem1.Id = 183;
		this.pageSetupPaperKindItem1.Name = "pageSetupPaperKindItem1";
		this.spreadsheetCommandBarSubItem19.CommandName = "PageSetupPrintAreaCommandGroup";
		this.spreadsheetCommandBarSubItem19.Id = 184;
		this.spreadsheetCommandBarSubItem19.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem116),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem117),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem118)
		});
		this.spreadsheetCommandBarSubItem19.Name = "spreadsheetCommandBarSubItem19";
		this.spreadsheetCommandBarButtonItem116.CommandName = "PageSetupSetPrintArea";
		this.spreadsheetCommandBarButtonItem116.Id = 185;
		this.spreadsheetCommandBarButtonItem116.Name = "spreadsheetCommandBarButtonItem116";
		this.spreadsheetCommandBarButtonItem117.CommandName = "PageSetupClearPrintArea";
		this.spreadsheetCommandBarButtonItem117.Id = 186;
		this.spreadsheetCommandBarButtonItem117.Name = "spreadsheetCommandBarButtonItem117";
		this.spreadsheetCommandBarButtonItem118.CommandName = "PageSetupAddPrintArea";
		this.spreadsheetCommandBarButtonItem118.Id = 187;
		this.spreadsheetCommandBarButtonItem118.Name = "spreadsheetCommandBarButtonItem118";
		this.spreadsheetCommandBarButtonItem119.CommandName = "PageSetupPrintTitles";
		this.spreadsheetCommandBarButtonItem119.Id = 188;
		this.spreadsheetCommandBarButtonItem119.Name = "spreadsheetCommandBarButtonItem119";
		this.spreadsheetCommandBarCheckItem20.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem20.CommandName = "ViewShowGridlines";
		this.spreadsheetCommandBarCheckItem20.Id = 189;
		this.spreadsheetCommandBarCheckItem20.Name = "spreadsheetCommandBarCheckItem20";
		this.spreadsheetCommandBarCheckItem21.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem21.CommandName = "ViewShowHeadings";
		this.spreadsheetCommandBarCheckItem21.Id = 190;
		this.spreadsheetCommandBarCheckItem21.Name = "spreadsheetCommandBarCheckItem21";
		this.spreadsheetCommandBarCheckItem22.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem22.CommandName = "PageSetupPrintGridlines";
		this.spreadsheetCommandBarCheckItem22.Id = 191;
		this.spreadsheetCommandBarCheckItem22.Name = "spreadsheetCommandBarCheckItem22";
		this.spreadsheetCommandBarCheckItem23.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem23.CommandName = "PageSetupPrintHeadings";
		this.spreadsheetCommandBarCheckItem23.Id = 192;
		this.spreadsheetCommandBarCheckItem23.Name = "spreadsheetCommandBarCheckItem23";
		this.spreadsheetCommandBarSubItem20.CommandName = "ArrangeBringForwardCommandGroup";
		this.spreadsheetCommandBarSubItem20.Id = 193;
		this.spreadsheetCommandBarSubItem20.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem120),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem121)
		});
		this.spreadsheetCommandBarSubItem20.Name = "spreadsheetCommandBarSubItem20";
		this.spreadsheetCommandBarButtonItem120.CommandName = "ArrangeBringForward";
		this.spreadsheetCommandBarButtonItem120.Id = 194;
		this.spreadsheetCommandBarButtonItem120.Name = "spreadsheetCommandBarButtonItem120";
		this.spreadsheetCommandBarButtonItem121.CommandName = "ArrangeBringToFront";
		this.spreadsheetCommandBarButtonItem121.Id = 195;
		this.spreadsheetCommandBarButtonItem121.Name = "spreadsheetCommandBarButtonItem121";
		this.spreadsheetCommandBarSubItem21.CommandName = "ArrangeSendBackwardCommandGroup";
		this.spreadsheetCommandBarSubItem21.Id = 196;
		this.spreadsheetCommandBarSubItem21.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem122),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem123)
		});
		this.spreadsheetCommandBarSubItem21.Name = "spreadsheetCommandBarSubItem21";
		this.spreadsheetCommandBarButtonItem122.CommandName = "ArrangeSendBackward";
		this.spreadsheetCommandBarButtonItem122.Id = 197;
		this.spreadsheetCommandBarButtonItem122.Name = "spreadsheetCommandBarButtonItem122";
		this.spreadsheetCommandBarButtonItem123.CommandName = "ArrangeSendToBack";
		this.spreadsheetCommandBarButtonItem123.Id = 198;
		this.spreadsheetCommandBarButtonItem123.Name = "spreadsheetCommandBarButtonItem123";
		this.spreadsheetCommandBarSubItem22.CommandName = "FunctionsAutoSumCommandGroup";
		this.spreadsheetCommandBarSubItem22.Id = 199;
		this.spreadsheetCommandBarSubItem22.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem84),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem85),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem86),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem87),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem88)
		});
		this.spreadsheetCommandBarSubItem22.Name = "spreadsheetCommandBarSubItem22";
		this.functionsFinancialItem1.Id = 200;
		this.functionsFinancialItem1.Name = "functionsFinancialItem1";
		this.functionsLogicalItem1.Id = 201;
		this.functionsLogicalItem1.Name = "functionsLogicalItem1";
		this.functionsTextItem1.Id = 202;
		this.functionsTextItem1.Name = "functionsTextItem1";
		this.functionsDateAndTimeItem1.Id = 203;
		this.functionsDateAndTimeItem1.Name = "functionsDateAndTimeItem1";
		this.functionsLookupAndReferenceItem1.Id = 204;
		this.functionsLookupAndReferenceItem1.Name = "functionsLookupAndReferenceItem1";
		this.functionsMathAndTrigonometryItem1.Id = 205;
		this.functionsMathAndTrigonometryItem1.Name = "functionsMathAndTrigonometryItem1";
		this.spreadsheetCommandBarSubItem23.CommandName = "FunctionsMoreCommandGroup";
		this.spreadsheetCommandBarSubItem23.Id = 206;
		this.spreadsheetCommandBarSubItem23.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsStatisticalItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsEngineeringItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsInformationItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsCompatibilityItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.functionsWebItem1)
		});
		this.spreadsheetCommandBarSubItem23.Name = "spreadsheetCommandBarSubItem23";
		this.functionsStatisticalItem1.Id = 207;
		this.functionsStatisticalItem1.Name = "functionsStatisticalItem1";
		this.functionsEngineeringItem1.Id = 208;
		this.functionsEngineeringItem1.Name = "functionsEngineeringItem1";
		this.functionsInformationItem1.Id = 209;
		this.functionsInformationItem1.Name = "functionsInformationItem1";
		this.functionsCompatibilityItem1.Id = 210;
		this.functionsCompatibilityItem1.Name = "functionsCompatibilityItem1";
		this.functionsWebItem1.Id = 211;
		this.functionsWebItem1.Name = "functionsWebItem1";
		this.spreadsheetCommandBarButtonItem129.CommandName = "FormulasShowNameManager";
		this.spreadsheetCommandBarButtonItem129.Id = 212;
		this.spreadsheetCommandBarButtonItem129.Name = "spreadsheetCommandBarButtonItem129";
		this.spreadsheetCommandBarButtonItem130.CommandName = "FormulasDefineNameCommand";
		this.spreadsheetCommandBarButtonItem130.Id = 213;
		this.spreadsheetCommandBarButtonItem130.Name = "spreadsheetCommandBarButtonItem130";
		this.spreadsheetCommandBarButtonItem130.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.definedNameListItem1.Id = 214;
		this.definedNameListItem1.Name = "definedNameListItem1";
		this.definedNameListItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem131.CommandName = "FormulasCreateDefinedNamesFromSelection";
		this.spreadsheetCommandBarButtonItem131.Id = 215;
		this.spreadsheetCommandBarButtonItem131.Name = "spreadsheetCommandBarButtonItem131";
		this.spreadsheetCommandBarButtonItem131.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem24.CommandName = "ViewShowFormulas";
		this.spreadsheetCommandBarCheckItem24.Id = 216;
		this.spreadsheetCommandBarCheckItem24.Name = "spreadsheetCommandBarCheckItem24";
		this.spreadsheetCommandBarSubItem24.CommandName = "FormulasCalculationOptionsCommandGroup";
		this.spreadsheetCommandBarSubItem24.Id = 217;
		this.spreadsheetCommandBarSubItem24.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem25),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem26)
		});
		this.spreadsheetCommandBarSubItem24.Name = "spreadsheetCommandBarSubItem24";
		this.spreadsheetCommandBarCheckItem25.CommandName = "FormulasCalculationModeAutomatic";
		this.spreadsheetCommandBarCheckItem25.Id = 218;
		this.spreadsheetCommandBarCheckItem25.Name = "spreadsheetCommandBarCheckItem25";
		this.spreadsheetCommandBarCheckItem26.CommandName = "FormulasCalculationModeManual";
		this.spreadsheetCommandBarCheckItem26.Id = 219;
		this.spreadsheetCommandBarCheckItem26.Name = "spreadsheetCommandBarCheckItem26";
		this.spreadsheetCommandBarButtonItem132.CommandName = "FormulasCalculateNow";
		this.spreadsheetCommandBarButtonItem132.Id = 220;
		this.spreadsheetCommandBarButtonItem132.Name = "spreadsheetCommandBarButtonItem132";
		this.spreadsheetCommandBarButtonItem132.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem133.CommandName = "FormulasCalculateSheet";
		this.spreadsheetCommandBarButtonItem133.Id = 221;
		this.spreadsheetCommandBarButtonItem133.Name = "spreadsheetCommandBarButtonItem133";
		this.spreadsheetCommandBarButtonItem133.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarSubItem25.CommandName = "DataToolsDataValidationCommandGroup";
		this.spreadsheetCommandBarSubItem25.Id = 222;
		this.spreadsheetCommandBarSubItem25.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem134),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem135),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem136)
		});
		this.spreadsheetCommandBarSubItem25.Name = "spreadsheetCommandBarSubItem25";
		this.spreadsheetCommandBarButtonItem134.CommandName = "DataToolsDataValidation";
		this.spreadsheetCommandBarButtonItem134.Id = 223;
		this.spreadsheetCommandBarButtonItem134.Name = "spreadsheetCommandBarButtonItem134";
		this.spreadsheetCommandBarButtonItem135.CommandName = "DataToolsCircleInvalidData";
		this.spreadsheetCommandBarButtonItem135.Id = 224;
		this.spreadsheetCommandBarButtonItem135.Name = "spreadsheetCommandBarButtonItem135";
		this.spreadsheetCommandBarButtonItem136.CommandName = "DataToolsClearValidationCircles";
		this.spreadsheetCommandBarButtonItem136.Id = 225;
		this.spreadsheetCommandBarButtonItem136.Name = "spreadsheetCommandBarButtonItem136";
		this.spreadsheetCommandBarSubItem26.CommandName = "OutlineGroupCommandGroup";
		this.spreadsheetCommandBarSubItem26.Id = 226;
		this.spreadsheetCommandBarSubItem26.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem137),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem138)
		});
		this.spreadsheetCommandBarSubItem26.Name = "spreadsheetCommandBarSubItem26";
		this.spreadsheetCommandBarSubItem26.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem137.CommandName = "GroupOutline";
		this.spreadsheetCommandBarButtonItem137.Id = 227;
		this.spreadsheetCommandBarButtonItem137.Name = "spreadsheetCommandBarButtonItem137";
		this.spreadsheetCommandBarButtonItem138.CommandName = "AutoOutline";
		this.spreadsheetCommandBarButtonItem138.Id = 228;
		this.spreadsheetCommandBarButtonItem138.Name = "spreadsheetCommandBarButtonItem138";
		this.spreadsheetCommandBarSubItem27.CommandName = "OutlineUngroupCommandGroup";
		this.spreadsheetCommandBarSubItem27.Id = 229;
		this.spreadsheetCommandBarSubItem27.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem139),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem140)
		});
		this.spreadsheetCommandBarSubItem27.Name = "spreadsheetCommandBarSubItem27";
		this.spreadsheetCommandBarSubItem27.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem139.CommandName = "UngroupOutline";
		this.spreadsheetCommandBarButtonItem139.Id = 230;
		this.spreadsheetCommandBarButtonItem139.Name = "spreadsheetCommandBarButtonItem139";
		this.spreadsheetCommandBarButtonItem140.CommandName = "ClearOutline";
		this.spreadsheetCommandBarButtonItem140.Id = 231;
		this.spreadsheetCommandBarButtonItem140.Name = "spreadsheetCommandBarButtonItem140";
		this.spreadsheetCommandBarButtonItem141.CommandName = "Subtotal";
		this.spreadsheetCommandBarButtonItem141.Id = 232;
		this.spreadsheetCommandBarButtonItem141.Name = "spreadsheetCommandBarButtonItem141";
		this.spreadsheetCommandBarButtonItem141.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem142.CommandName = "ShowDetail";
		this.spreadsheetCommandBarButtonItem142.Id = 233;
		this.spreadsheetCommandBarButtonItem142.Name = "spreadsheetCommandBarButtonItem142";
		this.spreadsheetCommandBarButtonItem142.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem143.CommandName = "HideDetail";
		this.spreadsheetCommandBarButtonItem143.Id = 234;
		this.spreadsheetCommandBarButtonItem143.Name = "spreadsheetCommandBarButtonItem143";
		this.spreadsheetCommandBarButtonItem143.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem144.CommandName = "ReviewInsertComment";
		this.spreadsheetCommandBarButtonItem144.Id = 235;
		this.spreadsheetCommandBarButtonItem144.Name = "spreadsheetCommandBarButtonItem144";
		this.spreadsheetCommandBarButtonItem145.CommandName = "ReviewEditComment";
		this.spreadsheetCommandBarButtonItem145.Id = 236;
		this.spreadsheetCommandBarButtonItem145.Name = "spreadsheetCommandBarButtonItem145";
		this.spreadsheetCommandBarButtonItem146.CommandName = "ReviewDeleteComment";
		this.spreadsheetCommandBarButtonItem146.Id = 237;
		this.spreadsheetCommandBarButtonItem146.Name = "spreadsheetCommandBarButtonItem146";
		this.spreadsheetCommandBarButtonItem147.CommandName = "ReviewShowHideComment";
		this.spreadsheetCommandBarButtonItem147.Id = 238;
		this.spreadsheetCommandBarButtonItem147.Name = "spreadsheetCommandBarButtonItem147";
		this.spreadsheetCommandBarButtonItem147.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem148.CommandName = "ReviewUnprotectSheet";
		this.spreadsheetCommandBarButtonItem148.Id = 239;
		this.spreadsheetCommandBarButtonItem148.Name = "spreadsheetCommandBarButtonItem148";
		this.spreadsheetCommandBarButtonItem149.CommandName = "ReviewProtectWorkbook";
		this.spreadsheetCommandBarButtonItem149.Id = 240;
		this.spreadsheetCommandBarButtonItem149.Name = "spreadsheetCommandBarButtonItem149";
		this.spreadsheetCommandBarButtonItem150.CommandName = "ReviewUnprotectWorkbook";
		this.spreadsheetCommandBarButtonItem150.Id = 241;
		this.spreadsheetCommandBarButtonItem150.Name = "spreadsheetCommandBarButtonItem150";
		this.spreadsheetCommandBarButtonItem151.CommandName = "ReviewShowProtectedRangeManager";
		this.spreadsheetCommandBarButtonItem151.Id = 242;
		this.spreadsheetCommandBarButtonItem151.Name = "spreadsheetCommandBarButtonItem151";
		this.spreadsheetCommandBarButtonItem152.CommandName = "ViewZoom";
		this.spreadsheetCommandBarButtonItem152.Id = 243;
		this.spreadsheetCommandBarButtonItem152.Name = "spreadsheetCommandBarButtonItem152";
		this.spreadsheetCommandBarButtonItem153.CommandName = "ViewZoomOut";
		this.spreadsheetCommandBarButtonItem153.Id = 244;
		this.spreadsheetCommandBarButtonItem153.Name = "spreadsheetCommandBarButtonItem153";
		this.spreadsheetCommandBarButtonItem154.CommandName = "ViewZoomIn";
		this.spreadsheetCommandBarButtonItem154.Id = 245;
		this.spreadsheetCommandBarButtonItem154.Name = "spreadsheetCommandBarButtonItem154";
		this.spreadsheetCommandBarButtonItem155.CommandName = "ViewZoom100Percent";
		this.spreadsheetCommandBarButtonItem155.Id = 246;
		this.spreadsheetCommandBarButtonItem155.Name = "spreadsheetCommandBarButtonItem155";
		this.spreadsheetCommandBarSubItem28.CommandName = "ViewFreezePanesCommandGroup";
		this.spreadsheetCommandBarSubItem28.Id = 247;
		this.spreadsheetCommandBarSubItem28.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem156),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem157),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem158),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem159)
		});
		this.spreadsheetCommandBarSubItem28.Name = "spreadsheetCommandBarSubItem28";
		this.spreadsheetCommandBarButtonItem156.CommandName = "ViewFreezePanes";
		this.spreadsheetCommandBarButtonItem156.Id = 248;
		this.spreadsheetCommandBarButtonItem156.Name = "spreadsheetCommandBarButtonItem156";
		this.spreadsheetCommandBarButtonItem157.CommandName = "ViewUnfreezePanes";
		this.spreadsheetCommandBarButtonItem157.Id = 249;
		this.spreadsheetCommandBarButtonItem157.Name = "spreadsheetCommandBarButtonItem157";
		this.spreadsheetCommandBarButtonItem158.CommandName = "ViewFreezeTopRow";
		this.spreadsheetCommandBarButtonItem158.Id = 250;
		this.spreadsheetCommandBarButtonItem158.Name = "spreadsheetCommandBarButtonItem158";
		this.spreadsheetCommandBarButtonItem159.CommandName = "ViewFreezeFirstColumn";
		this.spreadsheetCommandBarButtonItem159.Id = 251;
		this.spreadsheetCommandBarButtonItem159.Name = "spreadsheetCommandBarButtonItem159";
		this.spreadsheetCommandBarButtonItem160.CommandName = "ChartChangeType";
		this.spreadsheetCommandBarButtonItem160.Id = 252;
		this.spreadsheetCommandBarButtonItem160.Name = "spreadsheetCommandBarButtonItem160";
		this.spreadsheetCommandBarButtonItem161.CommandName = "ChartSwitchRowColumn";
		this.spreadsheetCommandBarButtonItem161.Id = 253;
		this.spreadsheetCommandBarButtonItem161.Name = "spreadsheetCommandBarButtonItem161";
		this.spreadsheetCommandBarButtonItem162.CommandName = "ChartSelectData";
		this.spreadsheetCommandBarButtonItem162.Id = 254;
		this.spreadsheetCommandBarButtonItem162.Name = "spreadsheetCommandBarButtonItem162";
		this.galleryChartLayoutItem1.Gallery.ColumnCount = 6;
		this.galleryChartLayoutItem1.Gallery.DrawImageBackground = false;
		this.galleryChartLayoutItem1.Gallery.ImageSize = new System.Drawing.Size(48, 48);
		this.galleryChartLayoutItem1.Gallery.RowCount = 2;
		this.galleryChartLayoutItem1.Id = 255;
		this.galleryChartLayoutItem1.Name = "galleryChartLayoutItem1";
		this.galleryChartStyleItem1.Gallery.ColumnCount = 8;
		this.galleryChartStyleItem1.Gallery.DrawImageBackground = false;
		this.galleryChartStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChartStyleItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.galleryChartStyleItem1.Gallery.ItemSize = new System.Drawing.Size(93, 56);
		this.galleryChartStyleItem1.Gallery.MinimumColumnCount = 4;
		this.galleryChartStyleItem1.Gallery.RowCount = 6;
		this.galleryChartStyleItem1.Id = 256;
		this.galleryChartStyleItem1.Name = "galleryChartStyleItem1";
		this.spreadsheetCommandBarSubItem29.CommandName = "ChartAxesCommandGroup";
		this.spreadsheetCommandBarSubItem29.Id = 257;
		this.spreadsheetCommandBarSubItem29.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem11),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem12)
		});
		this.spreadsheetCommandBarSubItem29.Name = "spreadsheetCommandBarSubItem29";
		this.spreadsheetCommandBarButtonGalleryDropDownItem11.CommandName = "ChartPrimaryHorizontalAxisCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem11.DropDownControl = this.commandBarGalleryDropDown13;
		this.spreadsheetCommandBarButtonGalleryDropDownItem11.Id = 258;
		this.spreadsheetCommandBarButtonGalleryDropDownItem11.Name = "spreadsheetCommandBarButtonGalleryDropDownItem11";
		this.commandBarGalleryDropDown13.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown13.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup29.CommandName = "ChartPrimaryHorizontalAxisCommandGroup";
		spreadsheetCommandGalleryItem123.CommandName = "ChartHidePrimaryHorizontalAxis";
		spreadsheetCommandGalleryItem124.CommandName = "ChartPrimaryHorizontalAxisLeftToRight";
		spreadsheetCommandGalleryItem125.CommandName = "ChartPrimaryHorizontalAxisHideLabels";
		spreadsheetCommandGalleryItem126.CommandName = "ChartPrimaryHorizontalAxisRightToLeft";
		spreadsheetCommandGalleryItem127.CommandName = "ChartPrimaryHorizontalAxisDefault";
		spreadsheetCommandGalleryItem128.CommandName = "ChartPrimaryHorizontalAxisScaleThousands";
		spreadsheetCommandGalleryItem129.CommandName = "ChartPrimaryHorizontalAxisScaleMillions";
		spreadsheetCommandGalleryItem130.CommandName = "ChartPrimaryHorizontalAxisScaleBillions";
		spreadsheetCommandGalleryItem131.CommandName = "ChartPrimaryHorizontalAxisScaleLogarithm";
		spreadsheetCommandGalleryItemGroup29.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[9] { spreadsheetCommandGalleryItem123, spreadsheetCommandGalleryItem124, spreadsheetCommandGalleryItem125, spreadsheetCommandGalleryItem126, spreadsheetCommandGalleryItem127, spreadsheetCommandGalleryItem128, spreadsheetCommandGalleryItem129, spreadsheetCommandGalleryItem130, spreadsheetCommandGalleryItem131 });
		this.commandBarGalleryDropDown13.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup29 });
		this.commandBarGalleryDropDown13.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown13.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown13.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown13.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown13.Name = "commandBarGalleryDropDown13";
		this.commandBarGalleryDropDown13.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem12.CommandName = "ChartPrimaryVerticalAxisCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem12.DropDownControl = this.commandBarGalleryDropDown14;
		this.spreadsheetCommandBarButtonGalleryDropDownItem12.Id = 259;
		this.spreadsheetCommandBarButtonGalleryDropDownItem12.Name = "spreadsheetCommandBarButtonGalleryDropDownItem12";
		this.commandBarGalleryDropDown14.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown14.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup30.CommandName = "ChartPrimaryVerticalAxisCommandGroup";
		spreadsheetCommandGalleryItem132.CommandName = "ChartHidePrimaryVerticalAxis";
		spreadsheetCommandGalleryItem133.CommandName = "ChartPrimaryVerticalAxisLeftToRight";
		spreadsheetCommandGalleryItem134.CommandName = "ChartPrimaryVerticalAxisHideLabels";
		spreadsheetCommandGalleryItem135.CommandName = "ChartPrimaryVerticalAxisRightToLeft";
		spreadsheetCommandGalleryItem136.CommandName = "ChartPrimaryVerticalAxisDefault";
		spreadsheetCommandGalleryItem137.CommandName = "ChartPrimaryVerticalAxisScaleThousands";
		spreadsheetCommandGalleryItem138.CommandName = "ChartPrimaryVerticalAxisScaleMillions";
		spreadsheetCommandGalleryItem139.CommandName = "ChartPrimaryVerticalAxisScaleBillions";
		spreadsheetCommandGalleryItem140.CommandName = "ChartPrimaryVerticalAxisScaleLogarithm";
		spreadsheetCommandGalleryItemGroup30.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[9] { spreadsheetCommandGalleryItem132, spreadsheetCommandGalleryItem133, spreadsheetCommandGalleryItem134, spreadsheetCommandGalleryItem135, spreadsheetCommandGalleryItem136, spreadsheetCommandGalleryItem137, spreadsheetCommandGalleryItem138, spreadsheetCommandGalleryItem139, spreadsheetCommandGalleryItem140 });
		this.commandBarGalleryDropDown14.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup30 });
		this.commandBarGalleryDropDown14.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown14.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown14.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown14.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown14.Name = "commandBarGalleryDropDown14";
		this.commandBarGalleryDropDown14.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarSubItem30.CommandName = "ChartGridlinesCommandGroup";
		this.spreadsheetCommandBarSubItem30.Id = 260;
		this.spreadsheetCommandBarSubItem30.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem14)
		});
		this.spreadsheetCommandBarSubItem30.Name = "spreadsheetCommandBarSubItem30";
		this.spreadsheetCommandBarButtonGalleryDropDownItem13.CommandName = "ChartPrimaryHorizontalGridlinesCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem13.DropDownControl = this.commandBarGalleryDropDown15;
		this.spreadsheetCommandBarButtonGalleryDropDownItem13.Id = 261;
		this.spreadsheetCommandBarButtonGalleryDropDownItem13.Name = "spreadsheetCommandBarButtonGalleryDropDownItem13";
		this.commandBarGalleryDropDown15.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown15.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup31.CommandName = "ChartPrimaryHorizontalGridlinesCommandGroup";
		spreadsheetCommandGalleryItem141.CommandName = "ChartPrimaryHorizontalGridlinesNone";
		spreadsheetCommandGalleryItem142.CommandName = "ChartPrimaryHorizontalGridlinesMajor";
		spreadsheetCommandGalleryItem143.CommandName = "ChartPrimaryHorizontalGridlinesMinor";
		spreadsheetCommandGalleryItem144.CommandName = "ChartPrimaryHorizontalGridlinesMajorAndMinor";
		spreadsheetCommandGalleryItemGroup31.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem141, spreadsheetCommandGalleryItem142, spreadsheetCommandGalleryItem143, spreadsheetCommandGalleryItem144 });
		this.commandBarGalleryDropDown15.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup31 });
		this.commandBarGalleryDropDown15.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown15.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown15.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown15.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown15.Name = "commandBarGalleryDropDown15";
		this.commandBarGalleryDropDown15.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem14.CommandName = "ChartPrimaryVerticalGridlinesCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem14.DropDownControl = this.commandBarGalleryDropDown16;
		this.spreadsheetCommandBarButtonGalleryDropDownItem14.Id = 262;
		this.spreadsheetCommandBarButtonGalleryDropDownItem14.Name = "spreadsheetCommandBarButtonGalleryDropDownItem14";
		this.commandBarGalleryDropDown16.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown16.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup32.CommandName = "ChartPrimaryVerticalGridlinesCommandGroup";
		spreadsheetCommandGalleryItem145.CommandName = "ChartPrimaryVerticalGridlinesNone";
		spreadsheetCommandGalleryItem146.CommandName = "ChartPrimaryVerticalGridlinesMajor";
		spreadsheetCommandGalleryItem147.CommandName = "ChartPrimaryVerticalGridlinesMinor";
		spreadsheetCommandGalleryItem148.CommandName = "ChartPrimaryVerticalGridlinesMajorAndMinor";
		spreadsheetCommandGalleryItemGroup32.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem145, spreadsheetCommandGalleryItem146, spreadsheetCommandGalleryItem147, spreadsheetCommandGalleryItem148 });
		this.commandBarGalleryDropDown16.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup32 });
		this.commandBarGalleryDropDown16.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown16.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown16.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown16.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown16.Name = "commandBarGalleryDropDown16";
		this.commandBarGalleryDropDown16.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem15.CommandName = "ChartTitleCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem15.DropDownControl = this.commandBarGalleryDropDown17;
		this.spreadsheetCommandBarButtonGalleryDropDownItem15.Id = 263;
		this.spreadsheetCommandBarButtonGalleryDropDownItem15.Name = "spreadsheetCommandBarButtonGalleryDropDownItem15";
		this.commandBarGalleryDropDown17.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown17.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup33.CommandName = "ChartTitleCommandGroup";
		spreadsheetCommandGalleryItem149.CommandName = "ChartTitleNone";
		spreadsheetCommandGalleryItem150.CommandName = "ChartTitleCenteredOverlay";
		spreadsheetCommandGalleryItem151.CommandName = "ChartTitleAbove";
		spreadsheetCommandGalleryItemGroup33.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { spreadsheetCommandGalleryItem149, spreadsheetCommandGalleryItem150, spreadsheetCommandGalleryItem151 });
		this.commandBarGalleryDropDown17.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup33 });
		this.commandBarGalleryDropDown17.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown17.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown17.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown17.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown17.Name = "commandBarGalleryDropDown17";
		this.commandBarGalleryDropDown17.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarSubItem31.CommandName = "ChartAxisTitlesCommandGroup";
		this.spreadsheetCommandBarSubItem31.Id = 264;
		this.spreadsheetCommandBarSubItem31.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonGalleryDropDownItem17)
		});
		this.spreadsheetCommandBarSubItem31.Name = "spreadsheetCommandBarSubItem31";
		this.spreadsheetCommandBarButtonGalleryDropDownItem16.CommandName = "ChartPrimaryHorizontalAxisTitleCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem16.DropDownControl = this.commandBarGalleryDropDown18;
		this.spreadsheetCommandBarButtonGalleryDropDownItem16.Id = 265;
		this.spreadsheetCommandBarButtonGalleryDropDownItem16.Name = "spreadsheetCommandBarButtonGalleryDropDownItem16";
		this.commandBarGalleryDropDown18.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown18.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup34.CommandName = "ChartPrimaryHorizontalAxisTitleCommandGroup";
		spreadsheetCommandGalleryItem152.CommandName = "ChartPrimaryHorizontalAxisTitleNone";
		spreadsheetCommandGalleryItem153.CommandName = "ChartPrimaryHorizontalAxisTitleBelow";
		spreadsheetCommandGalleryItemGroup34.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem152, spreadsheetCommandGalleryItem153 });
		this.commandBarGalleryDropDown18.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup34 });
		this.commandBarGalleryDropDown18.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown18.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown18.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown18.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown18.Name = "commandBarGalleryDropDown18";
		this.commandBarGalleryDropDown18.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem17.CommandName = "ChartPrimaryVerticalAxisTitleCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem17.DropDownControl = this.commandBarGalleryDropDown19;
		this.spreadsheetCommandBarButtonGalleryDropDownItem17.Id = 266;
		this.spreadsheetCommandBarButtonGalleryDropDownItem17.Name = "spreadsheetCommandBarButtonGalleryDropDownItem17";
		this.commandBarGalleryDropDown19.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown19.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup35.CommandName = "ChartPrimaryVerticalAxisTitleCommandGroup";
		spreadsheetCommandGalleryItem154.CommandName = "ChartPrimaryVerticalAxisTitleNone";
		spreadsheetCommandGalleryItem155.CommandName = "ChartPrimaryVerticalAxisTitleRotated";
		spreadsheetCommandGalleryItem156.CommandName = "ChartPrimaryVerticalAxisTitleVertical";
		spreadsheetCommandGalleryItem157.CommandName = "ChartPrimaryVerticalAxisTitleHorizontal";
		spreadsheetCommandGalleryItemGroup35.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem154, spreadsheetCommandGalleryItem155, spreadsheetCommandGalleryItem156, spreadsheetCommandGalleryItem157 });
		this.commandBarGalleryDropDown19.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup35 });
		this.commandBarGalleryDropDown19.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown19.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown19.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown19.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown19.Name = "commandBarGalleryDropDown19";
		this.commandBarGalleryDropDown19.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem18.CommandName = "ChartLegendCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem18.DropDownControl = this.commandBarGalleryDropDown20;
		this.spreadsheetCommandBarButtonGalleryDropDownItem18.Id = 267;
		this.spreadsheetCommandBarButtonGalleryDropDownItem18.Name = "spreadsheetCommandBarButtonGalleryDropDownItem18";
		this.commandBarGalleryDropDown20.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown20.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup36.CommandName = "ChartLegendCommandGroup";
		spreadsheetCommandGalleryItem158.CommandName = "ChartLegendNone";
		spreadsheetCommandGalleryItem159.CommandName = "ChartLegendAtRight";
		spreadsheetCommandGalleryItem160.CommandName = "ChartLegendAtTop";
		spreadsheetCommandGalleryItem161.CommandName = "ChartLegendAtLeft";
		spreadsheetCommandGalleryItem162.CommandName = "ChartLegendAtBottom";
		spreadsheetCommandGalleryItem163.CommandName = "ChartLegendOverlayAtRight";
		spreadsheetCommandGalleryItem164.CommandName = "ChartLegendOverlayAtLeft";
		spreadsheetCommandGalleryItemGroup36.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { spreadsheetCommandGalleryItem158, spreadsheetCommandGalleryItem159, spreadsheetCommandGalleryItem160, spreadsheetCommandGalleryItem161, spreadsheetCommandGalleryItem162, spreadsheetCommandGalleryItem163, spreadsheetCommandGalleryItem164 });
		this.commandBarGalleryDropDown20.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup36 });
		this.commandBarGalleryDropDown20.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown20.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown20.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown20.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown20.Name = "commandBarGalleryDropDown20";
		this.commandBarGalleryDropDown20.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem19.CommandName = "ChartDataLabelsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem19.DropDownControl = this.commandBarGalleryDropDown21;
		this.spreadsheetCommandBarButtonGalleryDropDownItem19.Id = 268;
		this.spreadsheetCommandBarButtonGalleryDropDownItem19.Name = "spreadsheetCommandBarButtonGalleryDropDownItem19";
		this.commandBarGalleryDropDown21.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown21.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup37.CommandName = "ChartDataLabelsCommandGroup";
		spreadsheetCommandGalleryItem165.CommandName = "ChartDataLabelsNone";
		spreadsheetCommandGalleryItem166.CommandName = "ChartDataLabelsDefault";
		spreadsheetCommandGalleryItem167.CommandName = "ChartDataLabelsCenter";
		spreadsheetCommandGalleryItem168.CommandName = "ChartDataLabelsInsideEnd";
		spreadsheetCommandGalleryItem169.CommandName = "ChartDataLabelsInsideBase";
		spreadsheetCommandGalleryItem170.CommandName = "ChartDataLabelsOutsideEnd";
		spreadsheetCommandGalleryItem171.CommandName = "ChartDataLabelsBestFit";
		spreadsheetCommandGalleryItem172.CommandName = "ChartDataLabelsLeft";
		spreadsheetCommandGalleryItem173.CommandName = "ChartDataLabelsRight";
		spreadsheetCommandGalleryItem174.CommandName = "ChartDataLabelsAbove";
		spreadsheetCommandGalleryItem175.CommandName = "ChartDataLabelsBelow";
		spreadsheetCommandGalleryItemGroup37.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[11]
		{
			spreadsheetCommandGalleryItem165, spreadsheetCommandGalleryItem166, spreadsheetCommandGalleryItem167, spreadsheetCommandGalleryItem168, spreadsheetCommandGalleryItem169, spreadsheetCommandGalleryItem170, spreadsheetCommandGalleryItem171, spreadsheetCommandGalleryItem172, spreadsheetCommandGalleryItem173, spreadsheetCommandGalleryItem174,
			spreadsheetCommandGalleryItem175
		});
		this.commandBarGalleryDropDown21.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup37 });
		this.commandBarGalleryDropDown21.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown21.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown21.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown21.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown21.Name = "commandBarGalleryDropDown21";
		this.commandBarGalleryDropDown21.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem20.CommandName = "ChartLinesCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem20.DropDownControl = this.commandBarGalleryDropDown22;
		this.spreadsheetCommandBarButtonGalleryDropDownItem20.Id = 269;
		this.spreadsheetCommandBarButtonGalleryDropDownItem20.Name = "spreadsheetCommandBarButtonGalleryDropDownItem20";
		this.commandBarGalleryDropDown22.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown22.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup38.CommandName = "ChartLinesCommandGroup";
		spreadsheetCommandGalleryItem176.CommandName = "ChartLinesNone";
		spreadsheetCommandGalleryItem177.CommandName = "ChartShowDropLines";
		spreadsheetCommandGalleryItem178.CommandName = "ChartShowHighLowLines";
		spreadsheetCommandGalleryItem179.CommandName = "ChartShowDropLinesAndHighLowLines";
		spreadsheetCommandGalleryItem180.CommandName = "ChartShowSeriesLines";
		spreadsheetCommandGalleryItemGroup38.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { spreadsheetCommandGalleryItem176, spreadsheetCommandGalleryItem177, spreadsheetCommandGalleryItem178, spreadsheetCommandGalleryItem179, spreadsheetCommandGalleryItem180 });
		this.commandBarGalleryDropDown22.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup38 });
		this.commandBarGalleryDropDown22.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown22.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown22.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown22.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown22.Name = "commandBarGalleryDropDown22";
		this.commandBarGalleryDropDown22.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem21.CommandName = "ChartUpDownBarsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem21.DropDownControl = this.commandBarGalleryDropDown23;
		this.spreadsheetCommandBarButtonGalleryDropDownItem21.Id = 270;
		this.spreadsheetCommandBarButtonGalleryDropDownItem21.Name = "spreadsheetCommandBarButtonGalleryDropDownItem21";
		this.commandBarGalleryDropDown23.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown23.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup39.CommandName = "ChartUpDownBarsCommandGroup";
		spreadsheetCommandGalleryItem181.CommandName = "ChartHideUpDownBars";
		spreadsheetCommandGalleryItem182.CommandName = "ChartShowUpDownBars";
		spreadsheetCommandGalleryItemGroup39.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { spreadsheetCommandGalleryItem181, spreadsheetCommandGalleryItem182 });
		this.commandBarGalleryDropDown23.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup39 });
		this.commandBarGalleryDropDown23.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown23.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown23.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown23.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown23.Name = "commandBarGalleryDropDown23";
		this.commandBarGalleryDropDown23.Ribbon = this.ribbonControl1;
		this.spreadsheetCommandBarButtonGalleryDropDownItem22.CommandName = "ChartErrorBarsCommandGroup";
		this.spreadsheetCommandBarButtonGalleryDropDownItem22.DropDownControl = this.commandBarGalleryDropDown24;
		this.spreadsheetCommandBarButtonGalleryDropDownItem22.Id = 271;
		this.spreadsheetCommandBarButtonGalleryDropDownItem22.Name = "spreadsheetCommandBarButtonGalleryDropDownItem22";
		this.commandBarGalleryDropDown24.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown24.Gallery.AutoSize = DevExpress.XtraBars.Ribbon.GallerySizeMode.Both;
		spreadsheetCommandGalleryItemGroup40.CommandName = "ChartErrorBarsCommandGroup";
		spreadsheetCommandGalleryItem183.CommandName = "ChartErrorBarsNone";
		spreadsheetCommandGalleryItem184.CommandName = "ChartErrorBarsStandardError";
		spreadsheetCommandGalleryItem185.CommandName = "ChartErrorBarsPercentage";
		spreadsheetCommandGalleryItem186.CommandName = "ChartErrorBarsStandardDeviation";
		spreadsheetCommandGalleryItemGroup40.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { spreadsheetCommandGalleryItem183, spreadsheetCommandGalleryItem184, spreadsheetCommandGalleryItem185, spreadsheetCommandGalleryItem186 });
		this.commandBarGalleryDropDown24.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { spreadsheetCommandGalleryItemGroup40 });
		this.commandBarGalleryDropDown24.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown24.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown24.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
		this.commandBarGalleryDropDown24.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown24.Name = "commandBarGalleryDropDown24";
		this.commandBarGalleryDropDown24.Ribbon = this.ribbonControl1;
		this.renameTableItemCaption1.Id = 272;
		this.renameTableItemCaption1.Name = "renameTableItemCaption1";
		this.renameTableItem1.Edit = this.repositoryItemTextEdit1;
		this.renameTableItem1.Id = 273;
		this.renameTableItem1.Name = "renameTableItem1";
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.spreadsheetCommandBarCheckItem27.CommandName = "TableToolsConvertToRange";
		this.spreadsheetCommandBarCheckItem27.Id = 274;
		this.spreadsheetCommandBarCheckItem27.Name = "spreadsheetCommandBarCheckItem27";
		this.spreadsheetCommandBarCheckItem27.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem28.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem28.CommandName = "TableToolsToggleHeaderRow";
		this.spreadsheetCommandBarCheckItem28.Id = 275;
		this.spreadsheetCommandBarCheckItem28.Name = "spreadsheetCommandBarCheckItem28";
		this.spreadsheetCommandBarCheckItem29.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem29.CommandName = "TableToolsToggleTotalRow";
		this.spreadsheetCommandBarCheckItem29.Id = 276;
		this.spreadsheetCommandBarCheckItem29.Name = "spreadsheetCommandBarCheckItem29";
		this.spreadsheetCommandBarCheckItem30.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem30.CommandName = "TableToolsToggleBandedColumns";
		this.spreadsheetCommandBarCheckItem30.Id = 277;
		this.spreadsheetCommandBarCheckItem30.Name = "spreadsheetCommandBarCheckItem30";
		this.spreadsheetCommandBarCheckItem31.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem31.CommandName = "TableToolsToggleFirstColumn";
		this.spreadsheetCommandBarCheckItem31.Id = 278;
		this.spreadsheetCommandBarCheckItem31.Name = "spreadsheetCommandBarCheckItem31";
		this.spreadsheetCommandBarCheckItem32.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem32.CommandName = "TableToolsToggleLastColumn";
		this.spreadsheetCommandBarCheckItem32.Id = 279;
		this.spreadsheetCommandBarCheckItem32.Name = "spreadsheetCommandBarCheckItem32";
		this.spreadsheetCommandBarCheckItem33.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem33.CommandName = "TableToolsToggleBandedRows";
		this.spreadsheetCommandBarCheckItem33.Id = 280;
		this.spreadsheetCommandBarCheckItem33.Name = "spreadsheetCommandBarCheckItem33";
		this.galleryTableStylesItem1.Gallery.ColumnCount = 7;
		this.galleryTableStylesItem1.Gallery.DrawImageBackground = false;
		this.galleryTableStylesItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryTableStylesItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.galleryTableStylesItem1.Gallery.ItemSize = new System.Drawing.Size(73, 58);
		this.galleryTableStylesItem1.Gallery.RowCount = 10;
		this.galleryTableStylesItem1.Id = 281;
		this.galleryTableStylesItem1.Name = "galleryTableStylesItem1";
		this.spreadsheetCommandBarButtonItem163.CommandName = "OptionsPivotTable";
		this.spreadsheetCommandBarButtonItem163.Id = 282;
		this.spreadsheetCommandBarButtonItem163.Name = "spreadsheetCommandBarButtonItem163";
		this.spreadsheetCommandBarButtonItem164.CommandName = "SelectFieldTypePivotTable";
		this.spreadsheetCommandBarButtonItem164.Id = 283;
		this.spreadsheetCommandBarButtonItem164.Name = "spreadsheetCommandBarButtonItem164";
		this.spreadsheetCommandBarButtonItem165.CommandName = "PivotTableExpandField";
		this.spreadsheetCommandBarButtonItem165.Id = 284;
		this.spreadsheetCommandBarButtonItem165.Name = "spreadsheetCommandBarButtonItem165";
		this.spreadsheetCommandBarButtonItem165.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem166.CommandName = "PivotTableCollapseField";
		this.spreadsheetCommandBarButtonItem166.Id = 285;
		this.spreadsheetCommandBarButtonItem166.Name = "spreadsheetCommandBarButtonItem166";
		this.spreadsheetCommandBarButtonItem166.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem167.CommandName = "PivotTableGroupSelection";
		this.spreadsheetCommandBarButtonItem167.Id = 286;
		this.spreadsheetCommandBarButtonItem167.Name = "spreadsheetCommandBarButtonItem167";
		this.spreadsheetCommandBarButtonItem167.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem168.CommandName = "PivotTableUngroup";
		this.spreadsheetCommandBarButtonItem168.Id = 287;
		this.spreadsheetCommandBarButtonItem168.Name = "spreadsheetCommandBarButtonItem168";
		this.spreadsheetCommandBarButtonItem168.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem169.CommandName = "PivotTableGroupField";
		this.spreadsheetCommandBarButtonItem169.Id = 288;
		this.spreadsheetCommandBarButtonItem169.Name = "spreadsheetCommandBarButtonItem169";
		this.spreadsheetCommandBarButtonItem169.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarSubItem32.CommandName = "PivotTableDataRefreshGroup";
		this.spreadsheetCommandBarSubItem32.Id = 289;
		this.spreadsheetCommandBarSubItem32.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem170),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem171)
		});
		this.spreadsheetCommandBarSubItem32.Name = "spreadsheetCommandBarSubItem32";
		this.spreadsheetCommandBarButtonItem170.CommandName = "RefreshPivotTable";
		this.spreadsheetCommandBarButtonItem170.Id = 290;
		this.spreadsheetCommandBarButtonItem170.Name = "spreadsheetCommandBarButtonItem170";
		this.spreadsheetCommandBarButtonItem171.CommandName = "RefreshAllPivotTable";
		this.spreadsheetCommandBarButtonItem171.Id = 291;
		this.spreadsheetCommandBarButtonItem171.Name = "spreadsheetCommandBarButtonItem171";
		this.spreadsheetCommandBarButtonItem172.CommandName = "ChangeDataSourcePivotTable";
		this.spreadsheetCommandBarButtonItem172.Id = 292;
		this.spreadsheetCommandBarButtonItem172.Name = "spreadsheetCommandBarButtonItem172";
		this.spreadsheetCommandBarSubItem33.CommandName = "PivotTableActionsClearGroup";
		this.spreadsheetCommandBarSubItem33.Id = 293;
		this.spreadsheetCommandBarSubItem33.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem173),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem174)
		});
		this.spreadsheetCommandBarSubItem33.Name = "spreadsheetCommandBarSubItem33";
		this.spreadsheetCommandBarButtonItem173.CommandName = "ClearAllPivotTable";
		this.spreadsheetCommandBarButtonItem173.Id = 294;
		this.spreadsheetCommandBarButtonItem173.Name = "spreadsheetCommandBarButtonItem173";
		this.spreadsheetCommandBarButtonItem174.CommandName = "ClearFiltersPivotTable";
		this.spreadsheetCommandBarButtonItem174.Id = 295;
		this.spreadsheetCommandBarButtonItem174.Name = "spreadsheetCommandBarButtonItem174";
		this.spreadsheetCommandBarSubItem34.CommandName = "PivotTableActionsSelectGroup";
		this.spreadsheetCommandBarSubItem34.Id = 296;
		this.spreadsheetCommandBarSubItem34.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem175),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem176),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem177)
		});
		this.spreadsheetCommandBarSubItem34.Name = "spreadsheetCommandBarSubItem34";
		this.spreadsheetCommandBarButtonItem175.CommandName = "SelectValuesPivotTable";
		this.spreadsheetCommandBarButtonItem175.Id = 297;
		this.spreadsheetCommandBarButtonItem175.Name = "spreadsheetCommandBarButtonItem175";
		this.spreadsheetCommandBarButtonItem176.CommandName = "SelectLabelsPivotTable";
		this.spreadsheetCommandBarButtonItem176.Id = 298;
		this.spreadsheetCommandBarButtonItem176.Name = "spreadsheetCommandBarButtonItem176";
		this.spreadsheetCommandBarButtonItem177.CommandName = "SelectEntirePivotTable";
		this.spreadsheetCommandBarButtonItem177.Id = 299;
		this.spreadsheetCommandBarButtonItem177.Name = "spreadsheetCommandBarButtonItem177";
		this.spreadsheetCommandBarButtonItem178.CommandName = "MovePivotTable";
		this.spreadsheetCommandBarButtonItem178.Id = 300;
		this.spreadsheetCommandBarButtonItem178.Name = "spreadsheetCommandBarButtonItem178";
		this.spreadsheetCommandBarSubItem35.CommandName = "PivotTableCalculationFieldsItemsSetsGroup";
		this.spreadsheetCommandBarSubItem35.Id = 301;
		this.spreadsheetCommandBarSubItem35.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem179),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem180),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem181),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem182)
		});
		this.spreadsheetCommandBarSubItem35.Name = "spreadsheetCommandBarSubItem35";
		this.spreadsheetCommandBarButtonItem179.CommandName = "PivotTableCalculatedField";
		this.spreadsheetCommandBarButtonItem179.Id = 302;
		this.spreadsheetCommandBarButtonItem179.Name = "spreadsheetCommandBarButtonItem179";
		this.spreadsheetCommandBarButtonItem180.CommandName = "PivotTableCalculatedItem";
		this.spreadsheetCommandBarButtonItem180.Id = 303;
		this.spreadsheetCommandBarButtonItem180.Name = "spreadsheetCommandBarButtonItem180";
		this.spreadsheetCommandBarButtonItem181.CommandName = "PivotTableCalculatedItemSolveOrder";
		this.spreadsheetCommandBarButtonItem181.Id = 304;
		this.spreadsheetCommandBarButtonItem181.Name = "spreadsheetCommandBarButtonItem181";
		this.spreadsheetCommandBarButtonItem182.CommandName = "PivotTableListFormulas";
		this.spreadsheetCommandBarButtonItem182.Id = 305;
		this.spreadsheetCommandBarButtonItem182.Name = "spreadsheetCommandBarButtonItem182";
		this.spreadsheetCommandBarCheckItem34.CommandName = "FieldListPanelPivotTable";
		this.spreadsheetCommandBarCheckItem34.Id = 306;
		this.spreadsheetCommandBarCheckItem34.Name = "spreadsheetCommandBarCheckItem34";
		this.spreadsheetCommandBarCheckItem35.CommandName = "ShowPivotTableExpandCollapseButtons";
		this.spreadsheetCommandBarCheckItem35.Id = 307;
		this.spreadsheetCommandBarCheckItem35.Name = "spreadsheetCommandBarCheckItem35";
		this.spreadsheetCommandBarCheckItem36.CommandName = "ShowPivotTableFieldHeaders";
		this.spreadsheetCommandBarCheckItem36.Id = 308;
		this.spreadsheetCommandBarCheckItem36.Name = "spreadsheetCommandBarCheckItem36";
		this.spreadsheetCommandBarSubItem36.CommandName = "PivotTableLayoutSubtotalsGroup";
		this.spreadsheetCommandBarSubItem36.Id = 309;
		this.spreadsheetCommandBarSubItem36.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem183),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem184),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem185)
		});
		this.spreadsheetCommandBarSubItem36.Name = "spreadsheetCommandBarSubItem36";
		this.spreadsheetCommandBarButtonItem183.CommandName = "PivotTableDoNotShowSubtotals";
		this.spreadsheetCommandBarButtonItem183.Id = 310;
		this.spreadsheetCommandBarButtonItem183.Name = "spreadsheetCommandBarButtonItem183";
		this.spreadsheetCommandBarButtonItem183.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem184.CommandName = "PivotTableShowAllSubtotalsAtBottom";
		this.spreadsheetCommandBarButtonItem184.Id = 311;
		this.spreadsheetCommandBarButtonItem184.Name = "spreadsheetCommandBarButtonItem184";
		this.spreadsheetCommandBarButtonItem184.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem185.CommandName = "PivotTableShowAllSubtotalsAtTop";
		this.spreadsheetCommandBarButtonItem185.Id = 312;
		this.spreadsheetCommandBarButtonItem185.Name = "spreadsheetCommandBarButtonItem185";
		this.spreadsheetCommandBarButtonItem185.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem37.CommandName = "PivotTableLayoutGrandTotalsGroup";
		this.spreadsheetCommandBarSubItem37.Id = 313;
		this.spreadsheetCommandBarSubItem37.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem186),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem187),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem188),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem189)
		});
		this.spreadsheetCommandBarSubItem37.Name = "spreadsheetCommandBarSubItem37";
		this.spreadsheetCommandBarButtonItem186.CommandName = "PivotTableGrandTotalsOffRowsColumns";
		this.spreadsheetCommandBarButtonItem186.Id = 314;
		this.spreadsheetCommandBarButtonItem186.Name = "spreadsheetCommandBarButtonItem186";
		this.spreadsheetCommandBarButtonItem186.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem187.CommandName = "PivotTableGrandTotalsOnRowsColumns";
		this.spreadsheetCommandBarButtonItem187.Id = 315;
		this.spreadsheetCommandBarButtonItem187.Name = "spreadsheetCommandBarButtonItem187";
		this.spreadsheetCommandBarButtonItem187.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem188.CommandName = "PivotTableGrandTotalsOnRowsOnly";
		this.spreadsheetCommandBarButtonItem188.Id = 316;
		this.spreadsheetCommandBarButtonItem188.Name = "spreadsheetCommandBarButtonItem188";
		this.spreadsheetCommandBarButtonItem188.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem189.CommandName = "PivotTableGrandTotalsOnColumnsOnly";
		this.spreadsheetCommandBarButtonItem189.Id = 317;
		this.spreadsheetCommandBarButtonItem189.Name = "spreadsheetCommandBarButtonItem189";
		this.spreadsheetCommandBarButtonItem189.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem38.CommandName = "PivotTableLayoutReportLayoutGroup";
		this.spreadsheetCommandBarSubItem38.Id = 318;
		this.spreadsheetCommandBarSubItem38.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem190),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem191),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem192),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem193),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem194)
		});
		this.spreadsheetCommandBarSubItem38.Name = "spreadsheetCommandBarSubItem38";
		this.spreadsheetCommandBarButtonItem190.CommandName = "PivotTableShowCompactForm";
		this.spreadsheetCommandBarButtonItem190.Id = 319;
		this.spreadsheetCommandBarButtonItem190.Name = "spreadsheetCommandBarButtonItem190";
		this.spreadsheetCommandBarButtonItem190.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem191.CommandName = "PivotTableShowOutlineForm";
		this.spreadsheetCommandBarButtonItem191.Id = 320;
		this.spreadsheetCommandBarButtonItem191.Name = "spreadsheetCommandBarButtonItem191";
		this.spreadsheetCommandBarButtonItem191.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem192.CommandName = "PivotTableShowTabularForm";
		this.spreadsheetCommandBarButtonItem192.Id = 321;
		this.spreadsheetCommandBarButtonItem192.Name = "spreadsheetCommandBarButtonItem192";
		this.spreadsheetCommandBarButtonItem192.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem193.CommandName = "PivotTableRepeatAllItemLabels";
		this.spreadsheetCommandBarButtonItem193.Id = 322;
		this.spreadsheetCommandBarButtonItem193.Name = "spreadsheetCommandBarButtonItem193";
		this.spreadsheetCommandBarButtonItem193.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem194.CommandName = "PivotTableDoNotRepeatItemLabels";
		this.spreadsheetCommandBarButtonItem194.Id = 323;
		this.spreadsheetCommandBarButtonItem194.Name = "spreadsheetCommandBarButtonItem194";
		this.spreadsheetCommandBarButtonItem194.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem39.CommandName = "PivotTableLayoutBlankRowsGroup";
		this.spreadsheetCommandBarSubItem39.Id = 324;
		this.spreadsheetCommandBarSubItem39.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem195),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem196)
		});
		this.spreadsheetCommandBarSubItem39.Name = "spreadsheetCommandBarSubItem39";
		this.spreadsheetCommandBarButtonItem195.CommandName = "PivotTableInsertBlankLineEachItem";
		this.spreadsheetCommandBarButtonItem195.Id = 325;
		this.spreadsheetCommandBarButtonItem195.Name = "spreadsheetCommandBarButtonItem195";
		this.spreadsheetCommandBarButtonItem195.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem196.CommandName = "PivotTableRemoveBlankLineEachItem";
		this.spreadsheetCommandBarButtonItem196.Id = 326;
		this.spreadsheetCommandBarButtonItem196.Name = "spreadsheetCommandBarButtonItem196";
		this.spreadsheetCommandBarButtonItem196.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarCheckItem37.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem37.CommandName = "PivotTableToggleRowHeaders";
		this.spreadsheetCommandBarCheckItem37.Id = 327;
		this.spreadsheetCommandBarCheckItem37.Name = "spreadsheetCommandBarCheckItem37";
		this.spreadsheetCommandBarCheckItem38.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem38.CommandName = "PivotTableToggleColumnHeaders";
		this.spreadsheetCommandBarCheckItem38.Id = 328;
		this.spreadsheetCommandBarCheckItem38.Name = "spreadsheetCommandBarCheckItem38";
		this.spreadsheetCommandBarCheckItem39.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem39.CommandName = "PivotTableToggleBandedRows";
		this.spreadsheetCommandBarCheckItem39.Id = 329;
		this.spreadsheetCommandBarCheckItem39.Name = "spreadsheetCommandBarCheckItem39";
		this.spreadsheetCommandBarCheckItem40.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.spreadsheetCommandBarCheckItem40.CommandName = "PivotTableToggleBandedColumns";
		this.spreadsheetCommandBarCheckItem40.Id = 330;
		this.spreadsheetCommandBarCheckItem40.Name = "spreadsheetCommandBarCheckItem40";
		this.galleryPivotStylesItem1.Gallery.ColumnCount = 7;
		this.galleryPivotStylesItem1.Gallery.DrawImageBackground = false;
		this.galleryPivotStylesItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryPivotStylesItem1.Gallery.ItemAutoSizeMode = DevExpress.XtraBars.Ribbon.Gallery.GalleryItemAutoSizeMode.None;
		this.galleryPivotStylesItem1.Gallery.ItemSize = new System.Drawing.Size(73, 61);
		this.galleryPivotStylesItem1.Gallery.RowCount = 10;
		this.galleryPivotStylesItem1.Id = 331;
		this.galleryPivotStylesItem1.Name = "galleryPivotStylesItem1";
		this.spreadsheetCommandBarButtonItem197.CommandName = "MailMergeAddDataSource";
		this.spreadsheetCommandBarButtonItem197.Id = 335;
		this.spreadsheetCommandBarButtonItem197.Name = "spreadsheetCommandBarButtonItem197";
		this.spreadsheetCommandBarButtonItem197.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem40.CommandName = "MailMergeManageRelationsCommandGroup";
		this.spreadsheetCommandBarSubItem40.Id = 336;
		this.spreadsheetCommandBarSubItem40.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem198),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem199)
		});
		this.spreadsheetCommandBarSubItem40.Name = "spreadsheetCommandBarSubItem40";
		this.spreadsheetCommandBarSubItem40.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem198.CommandName = "MailMergeManageQueriesCommand";
		this.spreadsheetCommandBarButtonItem198.Id = 337;
		this.spreadsheetCommandBarButtonItem198.Name = "spreadsheetCommandBarButtonItem198";
		this.spreadsheetCommandBarButtonItem199.CommandName = "MailMergeManageRelationsCommand";
		this.spreadsheetCommandBarButtonItem199.Id = 338;
		this.spreadsheetCommandBarButtonItem199.Name = "spreadsheetCommandBarButtonItem199";
		this.spreadsheetCommandBarSubItem41.CommandName = "MailMergeManageDataSourceCommandGroup";
		this.spreadsheetCommandBarSubItem41.Id = 339;
		this.spreadsheetCommandBarSubItem41.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem200),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem201),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem202)
		});
		this.spreadsheetCommandBarSubItem41.Name = "spreadsheetCommandBarSubItem41";
		this.spreadsheetCommandBarSubItem41.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem200.CommandName = "MailMergeSelectDataSource";
		this.spreadsheetCommandBarButtonItem200.Id = 340;
		this.spreadsheetCommandBarButtonItem200.Name = "spreadsheetCommandBarButtonItem200";
		this.spreadsheetCommandBarButtonItem201.CommandName = "MailMergeSelectDataMember";
		this.spreadsheetCommandBarButtonItem201.Id = 341;
		this.spreadsheetCommandBarButtonItem201.Name = "spreadsheetCommandBarButtonItem201";
		this.spreadsheetCommandBarButtonItem202.CommandName = "MailMergeManageDataSourcesCommand";
		this.spreadsheetCommandBarButtonItem202.Id = 342;
		this.spreadsheetCommandBarButtonItem202.Name = "spreadsheetCommandBarButtonItem202";
		this.spreadsheetCommandBarCheckItem41.CommandName = "MailMergeDocumentsMode";
		this.spreadsheetCommandBarCheckItem41.Id = 343;
		this.spreadsheetCommandBarCheckItem41.Name = "spreadsheetCommandBarCheckItem41";
		this.spreadsheetCommandBarCheckItem41.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem42.CommandName = "MailMergeOneDocumentMode";
		this.spreadsheetCommandBarCheckItem42.Id = 344;
		this.spreadsheetCommandBarCheckItem42.Name = "spreadsheetCommandBarCheckItem42";
		this.spreadsheetCommandBarCheckItem42.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarCheckItem43.CommandName = "MailMergeOneSheetMode";
		this.spreadsheetCommandBarCheckItem43.Id = 345;
		this.spreadsheetCommandBarCheckItem43.Name = "spreadsheetCommandBarCheckItem43";
		this.spreadsheetCommandBarCheckItem43.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarSubItem42.CommandName = "MailMergeOrientationCommandGroup";
		this.spreadsheetCommandBarSubItem42.Id = 346;
		this.spreadsheetCommandBarSubItem42.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem44),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarCheckItem45)
		});
		this.spreadsheetCommandBarSubItem42.Name = "spreadsheetCommandBarSubItem42";
		this.spreadsheetCommandBarSubItem42.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarCheckItem44.CommandName = "MailMergeHorizontalMode";
		this.spreadsheetCommandBarCheckItem44.Id = 347;
		this.spreadsheetCommandBarCheckItem44.Name = "spreadsheetCommandBarCheckItem44";
		this.spreadsheetCommandBarCheckItem45.CommandName = "MailMergeVerticalMode";
		this.spreadsheetCommandBarCheckItem45.Id = 348;
		this.spreadsheetCommandBarCheckItem45.Name = "spreadsheetCommandBarCheckItem45";
		this.spreadsheetCommandBarButtonItem203.CommandName = "MailMergeSetHeaderRange";
		this.spreadsheetCommandBarButtonItem203.Id = 349;
		this.spreadsheetCommandBarButtonItem203.Name = "spreadsheetCommandBarButtonItem203";
		this.spreadsheetCommandBarButtonItem203.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem204.CommandName = "MailMergeSetFooterRange";
		this.spreadsheetCommandBarButtonItem204.Id = 350;
		this.spreadsheetCommandBarButtonItem204.Name = "spreadsheetCommandBarButtonItem204";
		this.spreadsheetCommandBarButtonItem204.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem205.CommandName = "MailMergeSetDetailRange";
		this.spreadsheetCommandBarButtonItem205.Id = 351;
		this.spreadsheetCommandBarButtonItem205.Name = "spreadsheetCommandBarButtonItem205";
		this.spreadsheetCommandBarButtonItem205.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarSubItem43.CommandName = "EditingMailMergeMasterDetailCommandGroup";
		this.spreadsheetCommandBarSubItem43.Id = 352;
		this.spreadsheetCommandBarSubItem43.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem206),
			new DevExpress.XtraBars.LinkPersistInfo(this.spreadsheetCommandBarButtonItem207)
		});
		this.spreadsheetCommandBarSubItem43.Name = "spreadsheetCommandBarSubItem43";
		this.spreadsheetCommandBarSubItem43.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem206.CommandName = "MailMergeSetDetailLevel";
		this.spreadsheetCommandBarButtonItem206.Id = 353;
		this.spreadsheetCommandBarButtonItem206.Name = "spreadsheetCommandBarButtonItem206";
		this.spreadsheetCommandBarButtonItem207.CommandName = "MailMergeSetDetailDataMember";
		this.spreadsheetCommandBarButtonItem207.Id = 354;
		this.spreadsheetCommandBarButtonItem207.Name = "spreadsheetCommandBarButtonItem207";
		this.spreadsheetCommandBarButtonItem208.CommandName = "MailMergeResetRange";
		this.spreadsheetCommandBarButtonItem208.Id = 355;
		this.spreadsheetCommandBarButtonItem208.Name = "spreadsheetCommandBarButtonItem208";
		this.spreadsheetCommandBarButtonItem208.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem209.CommandName = "MailMergeSetGroup";
		this.spreadsheetCommandBarButtonItem209.Id = 356;
		this.spreadsheetCommandBarButtonItem209.Name = "spreadsheetCommandBarButtonItem209";
		this.spreadsheetCommandBarButtonItem209.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem210.CommandName = "MailMergeSetGroupHeader";
		this.spreadsheetCommandBarButtonItem210.Id = 357;
		this.spreadsheetCommandBarButtonItem210.Name = "spreadsheetCommandBarButtonItem210";
		this.spreadsheetCommandBarButtonItem210.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem211.CommandName = "MailMergeSetGroupFooter";
		this.spreadsheetCommandBarButtonItem211.Id = 358;
		this.spreadsheetCommandBarButtonItem211.Name = "spreadsheetCommandBarButtonItem211";
		this.spreadsheetCommandBarButtonItem211.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
		this.spreadsheetCommandBarButtonItem212.CommandName = "MailMergeSetFilter";
		this.spreadsheetCommandBarButtonItem212.Id = 359;
		this.spreadsheetCommandBarButtonItem212.Name = "spreadsheetCommandBarButtonItem212";
		this.spreadsheetCommandBarButtonItem212.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem213.CommandName = "MailMergeResetFilter";
		this.spreadsheetCommandBarButtonItem213.Id = 360;
		this.spreadsheetCommandBarButtonItem213.Name = "spreadsheetCommandBarButtonItem213";
		this.spreadsheetCommandBarButtonItem213.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarCheckItem46.CommandName = "MailMergeShowRanges";
		this.spreadsheetCommandBarCheckItem46.Id = 361;
		this.spreadsheetCommandBarCheckItem46.Name = "spreadsheetCommandBarCheckItem46";
		this.spreadsheetCommandBarCheckItem46.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.spreadsheetCommandBarButtonItem214.CommandName = "MailMergePreview";
		this.spreadsheetCommandBarButtonItem214.Id = 362;
		this.spreadsheetCommandBarButtonItem214.Name = "spreadsheetCommandBarButtonItem214";
		this.spreadsheetCommandBarButtonItem214.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.chartToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
		this.chartToolsRibbonPageCategory1.Name = "chartToolsRibbonPageCategory1";
		this.chartToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[3] { this.chartsDesignRibbonPage1, this.chartsLayoutRibbonPage1, this.chartsFormatRibbonPage1 });
		this.chartToolsRibbonPageCategory1.Visible = false;
		this.chartsDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.chartsDesignTypeRibbonPageGroup1, this.chartsDesignDataRibbonPageGroup1, this.chartsDesignLayoutsRibbonPageGroup1, this.chartsDesignStylesRibbonPageGroup1 });
		this.chartsDesignRibbonPage1.Name = "chartsDesignRibbonPage1";
		this.chartsDesignRibbonPage1.Visible = false;
		this.chartsDesignTypeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsDesignTypeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem160);
		this.chartsDesignTypeRibbonPageGroup1.Name = "chartsDesignTypeRibbonPageGroup1";
		this.chartsDesignDataRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsDesignDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem161);
		this.chartsDesignDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem162);
		this.chartsDesignDataRibbonPageGroup1.Name = "chartsDesignDataRibbonPageGroup1";
		this.chartsDesignLayoutsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsDesignLayoutsRibbonPageGroup1.ItemLinks.Add(this.galleryChartLayoutItem1);
		this.chartsDesignLayoutsRibbonPageGroup1.Name = "chartsDesignLayoutsRibbonPageGroup1";
		this.chartsDesignStylesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsDesignStylesRibbonPageGroup1.ItemLinks.Add(this.galleryChartStyleItem1);
		this.chartsDesignStylesRibbonPageGroup1.Name = "chartsDesignStylesRibbonPageGroup1";
		this.chartsLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.chartsLayoutAxesRibbonPageGroup1, this.chartsLayoutLabelsRibbonPageGroup1, this.chartsLayoutAnalysisRibbonPageGroup1 });
		this.chartsLayoutRibbonPage1.Name = "chartsLayoutRibbonPage1";
		this.chartsLayoutRibbonPage1.Visible = false;
		this.chartsLayoutAxesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsLayoutAxesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem29);
		this.chartsLayoutAxesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem30);
		this.chartsLayoutAxesRibbonPageGroup1.Name = "chartsLayoutAxesRibbonPageGroup1";
		this.chartsLayoutLabelsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem15);
		this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem31);
		this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem18);
		this.chartsLayoutLabelsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem19);
		this.chartsLayoutLabelsRibbonPageGroup1.Name = "chartsLayoutLabelsRibbonPageGroup1";
		this.chartsLayoutAnalysisRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem20);
		this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem21);
		this.chartsLayoutAnalysisRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem22);
		this.chartsLayoutAnalysisRibbonPageGroup1.Name = "chartsLayoutAnalysisRibbonPageGroup1";
		this.chartsFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.chartsFormatArrangeRibbonPageGroup1 });
		this.chartsFormatRibbonPage1.Name = "chartsFormatRibbonPage1";
		this.chartsFormatRibbonPage1.Visible = false;
		this.chartsFormatArrangeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
		this.chartsFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
		this.chartsFormatArrangeRibbonPageGroup1.Name = "chartsFormatArrangeRibbonPageGroup1";
		this.tableToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
		this.tableToolsRibbonPageCategory1.Name = "tableToolsRibbonPageCategory1";
		this.tableToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.tableToolsDesignRibbonPage1 });
		this.tableToolsRibbonPageCategory1.Visible = false;
		this.tableToolsDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.tablePropertiesRibbonPageGroup1, this.tableToolsRibbonPageGroup1, this.tableStyleOptionsRibbonPageGroup1, this.tableStylesRibbonPageGroup1 });
		this.tableToolsDesignRibbonPage1.Name = "tableToolsDesignRibbonPage1";
		this.tableToolsDesignRibbonPage1.Visible = false;
		this.tablePropertiesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.tablePropertiesRibbonPageGroup1.ItemLinks.Add(this.renameTableItemCaption1);
		this.tablePropertiesRibbonPageGroup1.ItemLinks.Add(this.renameTableItem1);
		this.tablePropertiesRibbonPageGroup1.Name = "tablePropertiesRibbonPageGroup1";
		this.tableToolsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.tableToolsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem27);
		this.tableToolsRibbonPageGroup1.Name = "tableToolsRibbonPageGroup1";
		this.tableStyleOptionsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem28);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem29);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem30);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem31);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem32);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem33);
		this.tableStyleOptionsRibbonPageGroup1.Name = "tableStyleOptionsRibbonPageGroup1";
		this.tableStylesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.tableStylesRibbonPageGroup1.ItemLinks.Add(this.galleryTableStylesItem1);
		this.tableStylesRibbonPageGroup1.Name = "tableStylesRibbonPageGroup1";
		this.drawingToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
		this.drawingToolsRibbonPageCategory1.Name = "drawingToolsRibbonPageCategory1";
		this.drawingToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.drawingFormatRibbonPage1 });
		this.drawingToolsRibbonPageCategory1.Visible = false;
		this.drawingFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.drawingFormatArrangeRibbonPageGroup1 });
		this.drawingFormatRibbonPage1.Name = "drawingFormatRibbonPage1";
		this.drawingFormatRibbonPage1.Visible = false;
		this.drawingFormatArrangeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.drawingFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
		this.drawingFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
		this.drawingFormatArrangeRibbonPageGroup1.Name = "drawingFormatArrangeRibbonPageGroup1";
		this.pictureToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
		this.pictureToolsRibbonPageCategory1.Name = "pictureToolsRibbonPageCategory1";
		this.pictureToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.pictureFormatRibbonPage1 });
		this.pictureToolsRibbonPageCategory1.Visible = false;
		this.pictureFormatRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.pictureFormatArrangeRibbonPageGroup1 });
		this.pictureFormatRibbonPage1.Name = "pictureFormatRibbonPage1";
		this.pictureFormatRibbonPage1.Visible = false;
		this.pictureFormatArrangeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pictureFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
		this.pictureFormatArrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
		this.pictureFormatArrangeRibbonPageGroup1.Name = "pictureFormatArrangeRibbonPageGroup1";
		this.pivotTableToolsRibbonPageCategory1.Control = this.spreadsheetControl1;
		this.pivotTableToolsRibbonPageCategory1.Name = "pivotTableToolsRibbonPageCategory1";
		this.pivotTableToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[2] { this.pivotTableAnalyzeRibbonPage1, this.pivotTableDesignRibbonPage1 });
		this.pivotTableToolsRibbonPageCategory1.Visible = false;
		this.pivotTableAnalyzeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[7] { this.pivotTableAnalyzePivotTableRibbonPageGroup1, this.pivotTableAnalyzeActiveFieldRibbonPageGroup1, this.pivotTableAnalyzeGroupRibbonPageGroup1, this.pivotTableAnalyzeDataRibbonPageGroup1, this.pivotTableAnalyzeActionsRibbonPageGroup1, this.pivotTableAnalyzeCalculationsRibbonPageGroup1, this.pivotTableAnalyzeShowRibbonPageGroup1 });
		this.pivotTableAnalyzeRibbonPage1.Name = "pivotTableAnalyzeRibbonPage1";
		this.pivotTableAnalyzeRibbonPage1.Visible = false;
		this.pivotTableAnalyzePivotTableRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzePivotTableRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem163);
		this.pivotTableAnalyzePivotTableRibbonPageGroup1.Name = "pivotTableAnalyzePivotTableRibbonPageGroup1";
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem164);
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem165);
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem166);
		this.pivotTableAnalyzeActiveFieldRibbonPageGroup1.Name = "pivotTableAnalyzeActiveFieldRibbonPageGroup1";
		this.pivotTableAnalyzeGroupRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeGroupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem167);
		this.pivotTableAnalyzeGroupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem168);
		this.pivotTableAnalyzeGroupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem169);
		this.pivotTableAnalyzeGroupRibbonPageGroup1.Name = "pivotTableAnalyzeGroupRibbonPageGroup1";
		this.pivotTableAnalyzeDataRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem32);
		this.pivotTableAnalyzeDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem172);
		this.pivotTableAnalyzeDataRibbonPageGroup1.Name = "pivotTableAnalyzeDataRibbonPageGroup1";
		this.pivotTableAnalyzeActionsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeActionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem33);
		this.pivotTableAnalyzeActionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem34);
		this.pivotTableAnalyzeActionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem178);
		this.pivotTableAnalyzeActionsRibbonPageGroup1.Name = "pivotTableAnalyzeActionsRibbonPageGroup1";
		this.pivotTableAnalyzeCalculationsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeCalculationsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem35);
		this.pivotTableAnalyzeCalculationsRibbonPageGroup1.Name = "pivotTableAnalyzeCalculationsRibbonPageGroup1";
		this.pivotTableAnalyzeShowRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableAnalyzeShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem34);
		this.pivotTableAnalyzeShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem35);
		this.pivotTableAnalyzeShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem36);
		this.pivotTableAnalyzeShowRibbonPageGroup1.Name = "pivotTableAnalyzeShowRibbonPageGroup1";
		this.pivotTableDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.pivotTableDesignLayoutRibbonPageGroup1, this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1, this.pivotTableDesignPivotTableStylesRibbonPageGroup1 });
		this.pivotTableDesignRibbonPage1.Name = "pivotTableDesignRibbonPage1";
		this.pivotTableDesignRibbonPage1.Visible = false;
		this.pivotTableDesignLayoutRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableDesignLayoutRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem36);
		this.pivotTableDesignLayoutRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem37);
		this.pivotTableDesignLayoutRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem38);
		this.pivotTableDesignLayoutRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem39);
		this.pivotTableDesignLayoutRibbonPageGroup1.Name = "pivotTableDesignLayoutRibbonPageGroup1";
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem37);
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem38);
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem39);
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem40);
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.ItemsLayout = DevExpress.XtraBars.Ribbon.RibbonPageGroupItemsLayout.TwoRows;
		this.pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1.Name = "pivotTableDesignPivotTableStyleOptionsRibbonPageGroup1";
		this.pivotTableDesignPivotTableStylesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pivotTableDesignPivotTableStylesRibbonPageGroup1.ItemLinks.Add(this.galleryPivotStylesItem1);
		this.pivotTableDesignPivotTableStylesRibbonPageGroup1.Name = "pivotTableDesignPivotTableStylesRibbonPageGroup1";
		this.fileRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.commonRibbonPageGroup1, this.infoRibbonPageGroup1 });
		this.fileRibbonPage1.Name = "fileRibbonPage1";
		this.commonRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem1);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem2);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem3);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem4);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem5);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem6);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem7);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem8);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem9);
		this.commonRibbonPageGroup1.Name = "commonRibbonPageGroup1";
		this.infoRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.infoRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem10);
		this.infoRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem11);
		this.infoRibbonPageGroup1.Name = "infoRibbonPageGroup1";
		this.homeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[7] { this.clipboardRibbonPageGroup1, this.fontRibbonPageGroup1, this.alignmentRibbonPageGroup1, this.numberRibbonPageGroup1, this.stylesRibbonPageGroup1, this.cellsRibbonPageGroup1, this.editingRibbonPageGroup1 });
		this.homeRibbonPage1.Name = "homeRibbonPage1";
		reduceOperation.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.UntilAvailable;
		reduceOperation.GroupName = null;
		reduceOperation.ItemLinkIndex = 2;
		reduceOperation.ItemLinksCount = 0;
		reduceOperation.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
		this.homeRibbonPage1.ReduceOperations.Add(reduceOperation);
		this.clipboardRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem12);
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem13);
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem14);
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem15);
		this.clipboardRibbonPageGroup1.Name = "clipboardRibbonPageGroup1";
		this.fontRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup1);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup2);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup3);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup4);
		this.fontRibbonPageGroup1.Name = "fontRibbonPageGroup1";
		this.alignmentRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup5);
		this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup6);
		this.alignmentRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup7);
		this.alignmentRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem11);
		this.alignmentRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem2);
		this.alignmentRibbonPageGroup1.Name = "alignmentRibbonPageGroup1";
		this.numberRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup8);
		this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup9);
		this.numberRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup10);
		this.numberRibbonPageGroup1.Name = "numberRibbonPageGroup1";
		this.cellsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem8);
		this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem9);
		this.cellsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem10);
		this.cellsRibbonPageGroup1.Name = "cellsRibbonPageGroup1";
		this.editingRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem12);
		this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem13);
		this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem14);
		this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem15);
		this.editingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem16);
		this.editingRibbonPageGroup1.Name = "editingRibbonPageGroup1";
		this.insertRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[5] { this.tablesRibbonPageGroup1, this.illustrationsRibbonPageGroup1, this.chartsRibbonPageGroup1, this.linksRibbonPageGroup1, this.symbolsRibbonPageGroup1 });
		this.insertRibbonPage1.Name = "insertRibbonPage1";
		this.tablesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.tablesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem110);
		this.tablesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem111);
		this.tablesRibbonPageGroup1.Name = "tablesRibbonPageGroup1";
		this.illustrationsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.illustrationsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem112);
		this.illustrationsRibbonPageGroup1.Name = "illustrationsRibbonPageGroup1";
		this.chartsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem4);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem5);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem6);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem7);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem8);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem9);
		this.chartsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem10);
		this.chartsRibbonPageGroup1.Name = "chartsRibbonPageGroup1";
		this.linksRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.linksRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem113);
		this.linksRibbonPageGroup1.Name = "linksRibbonPageGroup1";
		this.symbolsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.symbolsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem114);
		this.symbolsRibbonPageGroup1.Name = "symbolsRibbonPageGroup1";
		this.pageLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.pageSetupRibbonPageGroup1, this.pageSetupShowRibbonPageGroup1, this.pageSetupPrintRibbonPageGroup1, this.arrangeRibbonPageGroup1 });
		this.pageLayoutRibbonPage1.Name = "pageLayoutRibbonPage1";
		this.pageSetupRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem17);
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem18);
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.pageSetupPaperKindItem1);
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem19);
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem119);
		this.pageSetupRibbonPageGroup1.Name = "pageSetupRibbonPageGroup1";
		this.pageSetupShowRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.pageSetupShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem20);
		this.pageSetupShowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem21);
		this.pageSetupShowRibbonPageGroup1.Name = "pageSetupShowRibbonPageGroup1";
		this.pageSetupPrintRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.pageSetupPrintRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem22);
		this.pageSetupPrintRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem23);
		this.pageSetupPrintRibbonPageGroup1.Name = "pageSetupPrintRibbonPageGroup1";
		this.arrangeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.arrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem20);
		this.arrangeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem21);
		this.arrangeRibbonPageGroup1.Name = "arrangeRibbonPageGroup1";
		this.formulasRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.functionLibraryRibbonPageGroup1, this.formulaDefinedNamesRibbonPageGroup1, this.formulaAuditingRibbonPageGroup1, this.formulaCalculationRibbonPageGroup1 });
		this.formulasRibbonPage1.Name = "formulasRibbonPage1";
		this.functionLibraryRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem22);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsFinancialItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsLogicalItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsTextItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsDateAndTimeItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsLookupAndReferenceItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.functionsMathAndTrigonometryItem1);
		this.functionLibraryRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem23);
		this.functionLibraryRibbonPageGroup1.Name = "functionLibraryRibbonPageGroup1";
		this.formulaDefinedNamesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem129);
		this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem130);
		this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.definedNameListItem1);
		this.formulaDefinedNamesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem131);
		this.formulaDefinedNamesRibbonPageGroup1.Name = "formulaDefinedNamesRibbonPageGroup1";
		this.formulaAuditingRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.formulaAuditingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem24);
		this.formulaAuditingRibbonPageGroup1.Name = "formulaAuditingRibbonPageGroup1";
		this.formulaCalculationRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem24);
		this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem132);
		this.formulaCalculationRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem133);
		this.formulaCalculationRibbonPageGroup1.Name = "formulaCalculationRibbonPageGroup1";
		this.dataRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.sortAndFilterRibbonPageGroup1, this.dataToolsRibbonPageGroup1, this.outlineRibbonPageGroup1 });
		this.dataRibbonPage1.Name = "dataRibbonPage1";
		this.sortAndFilterRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem99);
		this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem100);
		this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem14);
		this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem101);
		this.sortAndFilterRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem102);
		this.sortAndFilterRibbonPageGroup1.Name = "sortAndFilterRibbonPageGroup1";
		this.dataToolsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.dataToolsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem25);
		this.dataToolsRibbonPageGroup1.Name = "dataToolsRibbonPageGroup1";
		this.outlineRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.True;
		this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem26);
		this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem27);
		this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem141);
		this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem142);
		this.outlineRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem143);
		this.outlineRibbonPageGroup1.Name = "outlineRibbonPageGroup1";
		this.reviewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.commentsRibbonPageGroup1, this.changesRibbonPageGroup1 });
		this.reviewRibbonPage1.Name = "reviewRibbonPage1";
		this.commentsRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem144);
		this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem145);
		this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem146);
		this.commentsRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem147);
		this.commentsRibbonPageGroup1.Name = "commentsRibbonPageGroup1";
		this.changesRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem82);
		this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem148);
		this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem149);
		this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem150);
		this.changesRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem151);
		this.changesRibbonPageGroup1.Name = "changesRibbonPageGroup1";
		this.viewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.showRibbonPageGroup1, this.zoomRibbonPageGroup1, this.windowRibbonPageGroup1 });
		this.viewRibbonPage1.Name = "viewRibbonPage1";
		this.showRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.showRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem20);
		this.showRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem21);
		this.showRibbonPageGroup1.Name = "showRibbonPageGroup1";
		this.zoomRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem152);
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem153);
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem154);
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem155);
		this.zoomRibbonPageGroup1.Name = "zoomRibbonPageGroup1";
		this.windowRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.windowRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem28);
		this.windowRibbonPageGroup1.Name = "windowRibbonPageGroup1";
		this.mailMergeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[6] { this.mailMergeDataRibbonPageGroup1, this.mailMergeModeRibbonPageGroup1, this.mailMergeExtendedRibbonPageGroup1, this.mailMergeGroupingRibbonPageGroup1, this.mailMergeFilteringRibbonPageGroup1, this.mailMergeBindingRibbonPageGroup1 });
		this.mailMergeRibbonPage1.Name = "mailMergeRibbonPage1";
		this.mailMergeDataRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem197);
		this.mailMergeDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem40);
		this.mailMergeDataRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem41);
		this.mailMergeDataRibbonPageGroup1.Name = "mailMergeDataRibbonPageGroup1";
		this.mailMergeModeRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeModeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem41);
		this.mailMergeModeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem42);
		this.mailMergeModeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem43);
		this.mailMergeModeRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem42);
		this.mailMergeModeRibbonPageGroup1.Name = "mailMergeModeRibbonPageGroup1";
		this.mailMergeExtendedRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeExtendedRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem203);
		this.mailMergeExtendedRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem204);
		this.mailMergeExtendedRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem205);
		this.mailMergeExtendedRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarSubItem43);
		this.mailMergeExtendedRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem208);
		this.mailMergeExtendedRibbonPageGroup1.Name = "mailMergeExtendedRibbonPageGroup1";
		this.mailMergeGroupingRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeGroupingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem209);
		this.mailMergeGroupingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem210);
		this.mailMergeGroupingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem211);
		this.mailMergeGroupingRibbonPageGroup1.Name = "mailMergeGroupingRibbonPageGroup1";
		this.mailMergeFilteringRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeFilteringRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem212);
		this.mailMergeFilteringRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem213);
		this.mailMergeFilteringRibbonPageGroup1.Name = "mailMergeFilteringRibbonPageGroup1";
		this.mailMergeBindingRibbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.mailMergeBindingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarCheckItem46);
		this.mailMergeBindingRibbonPageGroup1.ItemLinks.Add(this.spreadsheetCommandBarButtonItem214);
		this.mailMergeBindingRibbonPageGroup1.Name = "mailMergeBindingRibbonPageGroup1";
		this.spreadsheetFormulaBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.spreadsheetFormulaBarControl1.Location = new System.Drawing.Point(0, 0);
		this.spreadsheetFormulaBarControl1.MinimumSize = new System.Drawing.Size(0, 20);
		this.spreadsheetFormulaBarControl1.Name = "spreadsheetFormulaBarControl1";
		this.spreadsheetFormulaBarControl1.Size = new System.Drawing.Size(591, 24);
		this.spreadsheetFormulaBarControl1.SpreadsheetControl = this.spreadsheetControl1;
		this.spreadsheetFormulaBarControl1.TabIndex = 0;
		this.spreadsheetNameBoxControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.spreadsheetNameBoxControl1.EditValue = "A1";
		this.spreadsheetNameBoxControl1.Location = new System.Drawing.Point(0, 0);
		this.spreadsheetNameBoxControl1.MinimumSize = new System.Drawing.Size(0, 20);
		this.spreadsheetNameBoxControl1.Name = "spreadsheetNameBoxControl1";
		this.spreadsheetNameBoxControl1.Properties.AutoHeight = false;
		this.spreadsheetNameBoxControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)
		});
		this.spreadsheetNameBoxControl1.Size = new System.Drawing.Size(145, 24);
		this.spreadsheetNameBoxControl1.SpreadsheetControl = this.spreadsheetControl1;
		this.spreadsheetNameBoxControl1.TabIndex = 0;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 150);
		this.splitContainerControl1.MinimumSize = new System.Drawing.Size(0, 24);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.spreadsheetNameBoxControl1);
		this.splitContainerControl1.Panel2.Controls.Add(this.spreadsheetFormulaBarControl1);
		this.splitContainerControl1.Size = new System.Drawing.Size(746, 24);
		this.splitContainerControl1.SplitterPosition = 145;
		this.splitContainerControl1.TabIndex = 2;
		this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.splitterControl1.Location = new System.Drawing.Point(0, 174);
		this.splitterControl1.MinSize = 20;
		this.splitterControl1.Name = "splitterControl1";
		this.splitterControl1.Size = new System.Drawing.Size(746, 10);
		this.splitterControl1.TabIndex = 1;
		this.splitterControl1.TabStop = false;
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem7);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem15);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontNameItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontSizeItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem16);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem17);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem18);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem19);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem20);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem21);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem22);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem23);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem24);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem25);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem26);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem27);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem28);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem29);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem30);
		this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeBorderLineStyleItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeCellFillColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.changeFontColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem7);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem31);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem32);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem33);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem34);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem35);
		this.spreadsheetBarController1.BarItems.Add(this.changeNumberFormatItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem36);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem37);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem38);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem39);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem40);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem41);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem42);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem43);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem44);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem45);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem46);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem47);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem48);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem49);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem50);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem51);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem52);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem53);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem54);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem55);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem56);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem57);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem58);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem2);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem3);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem59);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem7);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem60);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem61);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem62);
		this.spreadsheetBarController1.BarItems.Add(this.galleryFormatAsTableItem1);
		this.spreadsheetBarController1.BarItems.Add(this.galleryChangeStyleItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem63);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem64);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem65);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem66);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem67);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem68);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem69);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem70);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem71);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem72);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem73);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem74);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem75);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem76);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem77);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem78);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem79);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem80);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem81);
		this.spreadsheetBarController1.BarItems.Add(this.changeSheetTabColorItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem82);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem83);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem84);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem85);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem86);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem87);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem88);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem89);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem90);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem91);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem92);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem93);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem94);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem95);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem96);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem97);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem98);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem15);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem99);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem100);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem101);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem102);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem16);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem103);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem104);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem105);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem106);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem107);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem108);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem109);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem110);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem111);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem112);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem4);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem5);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem6);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem7);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem8);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem9);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem10);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem113);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem114);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem17);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem15);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem16);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem17);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem115);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem18);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem18);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem19);
		this.spreadsheetBarController1.BarItems.Add(this.pageSetupPaperKindItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem19);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem116);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem117);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem118);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem119);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem20);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem21);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem22);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem23);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem20);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem120);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem121);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem21);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem122);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem123);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem22);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem124);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem125);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem126);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem127);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem128);
		this.spreadsheetBarController1.BarItems.Add(this.functionsFinancialItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsLogicalItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsTextItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsDateAndTimeItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsLookupAndReferenceItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsMathAndTrigonometryItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem23);
		this.spreadsheetBarController1.BarItems.Add(this.functionsStatisticalItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsEngineeringItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsInformationItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsCompatibilityItem1);
		this.spreadsheetBarController1.BarItems.Add(this.functionsWebItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem129);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem130);
		this.spreadsheetBarController1.BarItems.Add(this.definedNameListItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem131);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem24);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem24);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem25);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem26);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem132);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem133);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem25);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem134);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem135);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem136);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem26);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem137);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem138);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem27);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem139);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem140);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem141);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem142);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem143);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem144);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem145);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem146);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem147);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem148);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem149);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem150);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem151);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem152);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem153);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem154);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem155);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem28);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem156);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem157);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem158);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem159);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem160);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem161);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem162);
		this.spreadsheetBarController1.BarItems.Add(this.galleryChartLayoutItem1);
		this.spreadsheetBarController1.BarItems.Add(this.galleryChartStyleItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem29);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem11);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem12);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem30);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem13);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem14);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem15);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem31);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem16);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem17);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem18);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem19);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem20);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem21);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonGalleryDropDownItem22);
		this.spreadsheetBarController1.BarItems.Add(this.renameTableItemCaption1);
		this.spreadsheetBarController1.BarItems.Add(this.renameTableItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem27);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem28);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem29);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem30);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem31);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem32);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem33);
		this.spreadsheetBarController1.BarItems.Add(this.galleryTableStylesItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem163);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem164);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem165);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem166);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem167);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem168);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem169);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem32);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem170);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem171);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem172);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem33);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem173);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem174);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem34);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem175);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem176);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem177);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem178);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem35);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem179);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem180);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem181);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem182);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem34);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem35);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem36);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem36);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem183);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem184);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem185);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem37);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem186);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem187);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem188);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem189);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem38);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem190);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem191);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem192);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem193);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem194);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem39);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem195);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem196);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem37);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem38);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem39);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem40);
		this.spreadsheetBarController1.BarItems.Add(this.galleryPivotStylesItem1);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem197);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem40);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem198);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem199);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem41);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem200);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem201);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem202);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem41);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem42);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem43);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem42);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem44);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem45);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem203);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem204);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem205);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarSubItem43);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem206);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem207);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem208);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem209);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem210);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem211);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem212);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem213);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarCheckItem46);
		this.spreadsheetBarController1.BarItems.Add(this.spreadsheetCommandBarButtonItem214);
		this.spreadsheetBarController1.Control = this.spreadsheetControl1;
		this.spreadsheetCommandBarButtonItem124.CommandName = "FunctionsInsertSum";
		this.spreadsheetCommandBarButtonItem124.Name = "spreadsheetCommandBarButtonItem124";
		this.spreadsheetCommandBarButtonItem125.CommandName = "FunctionsInsertAverage";
		this.spreadsheetCommandBarButtonItem125.Name = "spreadsheetCommandBarButtonItem125";
		this.spreadsheetCommandBarButtonItem126.CommandName = "FunctionsInsertCountNumbers";
		this.spreadsheetCommandBarButtonItem126.Name = "spreadsheetCommandBarButtonItem126";
		this.spreadsheetCommandBarButtonItem127.CommandName = "FunctionsInsertMax";
		this.spreadsheetCommandBarButtonItem127.Name = "spreadsheetCommandBarButtonItem127";
		this.spreadsheetCommandBarButtonItem128.CommandName = "FunctionsInsertMin";
		this.spreadsheetCommandBarButtonItem128.Name = "spreadsheetCommandBarButtonItem128";
		this.spreadsheetDockManager1.Form = this;
		this.spreadsheetDockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[1] { this.panelContainer1 });
		this.spreadsheetDockManager1.SpreadsheetControl = this.spreadsheetControl1;
		this.spreadsheetDockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.panelContainer1.Controls.Add(this.fieldListDockPanel1);
		this.panelContainer1.Controls.Add(this.mailMergeParametersDockPanel1);
		this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
		this.panelContainer1.ID = new System.Guid("f431677b-54a9-4ffa-b9cb-75a72202eecc");
		this.panelContainer1.Location = new System.Drawing.Point(746, 150);
		this.panelContainer1.Name = "panelContainer1";
		this.panelContainer1.OriginalSize = new System.Drawing.Size(200, 200);
		this.panelContainer1.Size = new System.Drawing.Size(200, 488);
		this.panelContainer1.Text = "panelContainer1";
		this.fieldListDockPanel1.Controls.Add(this.fieldListDockPanel1_Container);
		this.fieldListDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.fieldListDockPanel1.ID = new System.Guid("75db9379-85ae-4ec7-9eb1-7ad33d9de073");
		this.fieldListDockPanel1.Location = new System.Drawing.Point(0, 0);
		this.fieldListDockPanel1.Name = "fieldListDockPanel1";
		this.fieldListDockPanel1.OriginalSize = new System.Drawing.Size(200, 249);
		this.fieldListDockPanel1.Size = new System.Drawing.Size(200, 244);
		this.fieldListDockPanel1.SpreadsheetControl = this.spreadsheetControl1;
		this.fieldListDockPanel1_Container.Location = new System.Drawing.Point(4, 26);
		this.fieldListDockPanel1_Container.Name = "fieldListDockPanel1_Container";
		this.fieldListDockPanel1_Container.Size = new System.Drawing.Size(193, 214);
		this.fieldListDockPanel1_Container.TabIndex = 0;
		this.mailMergeParametersDockPanel1.Controls.Add(this.mailMergeParametersDockPanel1_Container);
		this.mailMergeParametersDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.mailMergeParametersDockPanel1.ID = new System.Guid("f96120f1-3d7a-4265-8f4b-d61d9e2d8f73");
		this.mailMergeParametersDockPanel1.Location = new System.Drawing.Point(0, 244);
		this.mailMergeParametersDockPanel1.Name = "mailMergeParametersDockPanel1";
		this.mailMergeParametersDockPanel1.OriginalSize = new System.Drawing.Size(200, 248);
		this.mailMergeParametersDockPanel1.Size = new System.Drawing.Size(200, 244);
		this.mailMergeParametersDockPanel1.SpreadsheetControl = this.spreadsheetControl1;
		this.mailMergeParametersDockPanel1_Container.Location = new System.Drawing.Point(4, 26);
		this.mailMergeParametersDockPanel1_Container.Name = "mailMergeParametersDockPanel1_Container";
		this.mailMergeParametersDockPanel1_Container.Size = new System.Drawing.Size(193, 215);
		this.mailMergeParametersDockPanel1_Container.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(946, 638);
		base.Controls.Add(this.spreadsheetControl1);
		base.Controls.Add(this.splitterControl1);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelContainer1);
		base.Controls.Add(this.ribbonControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Speadsheet";
		base.ShowInTaskbar = false;
		this.Text = "Speadsheet";
		base.Load += new System.EventHandler(this.frm_Speadsheet_Load);
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpreadsheetFontSizeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPopupGalleryEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetNameBoxControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.spreadsheetBarController1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spreadsheetDockManager1).EndInit();
		this.panelContainer1.ResumeLayout(false);
		this.fieldListDockPanel1.ResumeLayout(false);
		this.mailMergeParametersDockPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
