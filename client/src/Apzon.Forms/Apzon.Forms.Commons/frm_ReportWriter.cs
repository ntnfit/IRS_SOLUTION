using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using DevExpress.Skins;
using DevExpress.Snap;
using DevExpress.Snap.Design;
using DevExpress.Snap.Extensions;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.NoDocuments;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraCharts.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.UI;

namespace Apzon.Forms.Commons;

public class frm_ReportWriter : XtraFormEx
{
	private IContainer components;

	private SnapControl snapControl1;

	private SnapDockManager snapDockManager1;

	private DockPanel panelContainer1;

	private FieldListDockPanel fieldListDockPanel1;

	private ControlContainer fieldListDockPanel1_Container;

	private ReportExplorerDockPanel reportExplorerDockPanel1;

	private ControlContainer reportExplorerDockPanel1_Container;

	private SnapDocumentManager snapDocumentManager1;

	private NoDocumentsView noDocumentsView1;

	private RibbonControl ribbonControl1;

	private UndoItem undoItem1;

	private RedoItem redoItem1;

	private FileNewItem fileNewItem1;

	private FileOpenItem fileOpenItem1;

	private FileSaveItem fileSaveItem1;

	private FileSaveAsItem fileSaveAsItem1;

	private CommandBarItem commandBarItem1;

	private QuickPrintItem quickPrintItem1;

	private PrintItem printItem1;

	private PrintPreviewItem printPreviewItem1;

	private ShowDocumentPropertiesFormItem showDocumentPropertiesFormItem1;

	private CommandBarItem commandBarItem2;

	private PasteItem pasteItem1;

	private CutItem cutItem1;

	private CopyItem copyItem1;

	private PasteSpecialItem pasteSpecialItem1;

	private BarButtonGroup barButtonGroup1;

	private ChangeFontNameItem changeFontNameItem1;

	private RepositoryItemFontEdit repositoryItemFontEdit1;

	private ChangeFontSizeItem changeFontSizeItem1;

	private RepositoryItemRichEditFontSizeEdit repositoryItemRichEditFontSizeEdit1;

	private FontSizeIncreaseItem fontSizeIncreaseItem1;

	private FontSizeDecreaseItem fontSizeDecreaseItem1;

	private BarButtonGroup barButtonGroup2;

	private ToggleFontBoldItem toggleFontBoldItem1;

	private ToggleFontItalicItem toggleFontItalicItem1;

	private ToggleFontUnderlineItem toggleFontUnderlineItem1;

	private ToggleFontDoubleUnderlineItem toggleFontDoubleUnderlineItem1;

	private ToggleFontStrikeoutItem toggleFontStrikeoutItem1;

	private ToggleFontDoubleStrikeoutItem toggleFontDoubleStrikeoutItem1;

	private ToggleFontSuperscriptItem toggleFontSuperscriptItem1;

	private ToggleFontSubscriptItem toggleFontSubscriptItem1;

	private BarButtonGroup barButtonGroup3;

	private ChangeFontColorItem changeFontColorItem1;

	private ChangeFontHighlightColorItem changeFontHighlightColorItem1;

	private ChangeTextCaseItem changeTextCaseItem1;

	private MakeTextUpperCaseItem makeTextUpperCaseItem1;

	private MakeTextLowerCaseItem makeTextLowerCaseItem1;

	private CapitalizeEachWordCaseItem capitalizeEachWordCaseItem1;

	private ToggleTextCaseItem toggleTextCaseItem1;

	private ClearFormattingItem clearFormattingItem1;

	private BarButtonGroup barButtonGroup4;

	private ToggleBulletedListItem toggleBulletedListItem1;

	private ToggleNumberingListItem toggleNumberingListItem1;

	private ToggleMultiLevelListItem toggleMultiLevelListItem1;

	private BarButtonGroup barButtonGroup5;

	private DecreaseIndentItem decreaseIndentItem1;

	private IncreaseIndentItem increaseIndentItem1;

	private ToggleShowWhitespaceItem toggleShowWhitespaceItem1;

	private BarButtonGroup barButtonGroup6;

	private ToggleParagraphAlignmentLeftItem toggleParagraphAlignmentLeftItem1;

	private ToggleParagraphAlignmentCenterItem toggleParagraphAlignmentCenterItem1;

	private ToggleParagraphAlignmentRightItem toggleParagraphAlignmentRightItem1;

	private ToggleParagraphAlignmentJustifyItem toggleParagraphAlignmentJustifyItem1;

	private BarButtonGroup barButtonGroup7;

	private ChangeParagraphLineSpacingItem changeParagraphLineSpacingItem1;

	private SetSingleParagraphSpacingItem setSingleParagraphSpacingItem1;

	private SetSesquialteralParagraphSpacingItem setSesquialteralParagraphSpacingItem1;

	private SetDoubleParagraphSpacingItem setDoubleParagraphSpacingItem1;

	private ShowLineSpacingFormItem showLineSpacingFormItem1;

	private AddSpacingBeforeParagraphItem addSpacingBeforeParagraphItem1;

	private RemoveSpacingBeforeParagraphItem removeSpacingBeforeParagraphItem1;

	private AddSpacingAfterParagraphItem addSpacingAfterParagraphItem1;

	private RemoveSpacingAfterParagraphItem removeSpacingAfterParagraphItem1;

	private ChangeParagraphBackColorItem changeParagraphBackColorItem1;

	private GalleryChangeStyleItem galleryChangeStyleItem1;

	private FindItem findItem1;

	private ReplaceItem replaceItem1;

	private InsertPageBreakItem2 insertPageBreakItem21;

	private InsertTableItem insertTableItem1;

	private InsertPictureItem insertPictureItem1;

	private InsertFloatingPictureItem insertFloatingPictureItem1;

	private CommandBarItem commandBarItem3;

	private CommandBarItem commandBarItem4;

	private CommandBarItem commandBarItem5;

	private CommandBarItem commandBarItem6;

	private CommandBarItem commandBarItem7;

	private InsertBookmarkItem insertBookmarkItem1;

	private InsertHyperlinkItem insertHyperlinkItem1;

	private EditPageHeaderItem editPageHeaderItem1;

	private EditPageFooterItem editPageFooterItem1;

	private InsertPageNumberItem insertPageNumberItem1;

	private InsertPageCountItem insertPageCountItem1;

	private InsertTextBoxItem insertTextBoxItem1;

	private InsertSymbolItem insertSymbolItem1;

	private ChangeSectionPageMarginsItem changeSectionPageMarginsItem1;

	private SetNormalSectionPageMarginsItem setNormalSectionPageMarginsItem1;

	private SetNarrowSectionPageMarginsItem setNarrowSectionPageMarginsItem1;

	private SetModerateSectionPageMarginsItem setModerateSectionPageMarginsItem1;

	private SetWideSectionPageMarginsItem setWideSectionPageMarginsItem1;

	private ShowPageMarginsSetupFormItem showPageMarginsSetupFormItem1;

	private ChangeSectionPageOrientationItem changeSectionPageOrientationItem1;

	private SetPortraitPageOrientationItem setPortraitPageOrientationItem1;

	private SetLandscapePageOrientationItem setLandscapePageOrientationItem1;

	private ChangeSectionPaperKindItem changeSectionPaperKindItem1;

	private ChangeSectionColumnsItem changeSectionColumnsItem1;

	private SetSectionOneColumnItem setSectionOneColumnItem1;

	private SetSectionTwoColumnsItem setSectionTwoColumnsItem1;

	private SetSectionThreeColumnsItem setSectionThreeColumnsItem1;

	private ShowColumnsSetupFormItem showColumnsSetupFormItem1;

	private InsertBreakItem insertBreakItem1;

	private InsertPageBreakItem insertPageBreakItem1;

	private InsertColumnBreakItem insertColumnBreakItem1;

	private InsertSectionBreakNextPageItem insertSectionBreakNextPageItem1;

	private InsertSectionBreakEvenPageItem insertSectionBreakEvenPageItem1;

	private InsertSectionBreakOddPageItem insertSectionBreakOddPageItem1;

	private ChangeSectionLineNumberingItem changeSectionLineNumberingItem1;

	private SetSectionLineNumberingNoneItem setSectionLineNumberingNoneItem1;

	private SetSectionLineNumberingContinuousItem setSectionLineNumberingContinuousItem1;

	private SetSectionLineNumberingRestartNewPageItem setSectionLineNumberingRestartNewPageItem1;

	private SetSectionLineNumberingRestartNewSectionItem setSectionLineNumberingRestartNewSectionItem1;

	private ToggleParagraphSuppressLineNumbersItem toggleParagraphSuppressLineNumbersItem1;

	private ShowLineNumberingFormItem showLineNumberingFormItem1;

	private ChangePageColorItem changePageColorItem1;

	private SwitchToSimpleViewItem switchToSimpleViewItem1;

	private SwitchToDraftViewItem switchToDraftViewItem1;

	private SwitchToPrintLayoutViewItem switchToPrintLayoutViewItem1;

	private ToggleShowHorizontalRulerItem toggleShowHorizontalRulerItem1;

	private ToggleShowVerticalRulerItem toggleShowVerticalRulerItem1;

	private ZoomOutItem zoomOutItem1;

	private ZoomInItem zoomInItem1;

	private SnapBarToolbarsListItem snapBarToolbarsListItem1;

	private CommandBarCheckItem commandBarCheckItem1;

	private ShowAllFieldCodesItem showAllFieldCodesItem1;

	private ShowAllFieldResultsItem showAllFieldResultsItem1;

	private ToggleFieldHighlightingItem toggleFieldHighlightingItem1;

	private GoToPageHeaderItem goToPageHeaderItem1;

	private GoToPageFooterItem goToPageFooterItem1;

	private GoToNextHeaderFooterItem goToNextHeaderFooterItem1;

	private GoToPreviousHeaderFooterItem goToPreviousHeaderFooterItem1;

	private ToggleLinkToPreviousItem toggleLinkToPreviousItem1;

	private ToggleDifferentFirstPageItem toggleDifferentFirstPageItem1;

	private ToggleDifferentOddAndEvenPagesItem toggleDifferentOddAndEvenPagesItem1;

	private ClosePageHeaderFooterItem closePageHeaderFooterItem1;

	private ToggleFirstRowItem toggleFirstRowItem1;

	private ToggleLastRowItem toggleLastRowItem1;

	private ToggleBandedRowsItem toggleBandedRowsItem1;

	private ToggleFirstColumnItem toggleFirstColumnItem1;

	private ToggleLastColumnItem toggleLastColumnItem1;

	private ToggleBandedColumnsItem toggleBandedColumnsItem1;

	private GalleryChangeTableStyleItem galleryChangeTableStyleItem1;

	private GalleryChangeTableCellStyleItem galleryChangeTableCellStyleItem1;

	private ChangeTableBorderLineStyleItem changeTableBorderLineStyleItem1;

	private RepositoryItemBorderLineStyle repositoryItemBorderLineStyle1;

	private ChangeTableBorderLineWeightItem changeTableBorderLineWeightItem1;

	private RepositoryItemBorderLineWeight repositoryItemBorderLineWeight1;

	private ChangeTableBorderColorItem changeTableBorderColorItem1;

	private ChangeTableBordersItem changeTableBordersItem1;

	private ToggleTableCellsBottomBorderItem toggleTableCellsBottomBorderItem1;

	private ToggleTableCellsTopBorderItem toggleTableCellsTopBorderItem1;

	private ToggleTableCellsLeftBorderItem toggleTableCellsLeftBorderItem1;

	private ToggleTableCellsRightBorderItem toggleTableCellsRightBorderItem1;

	private ResetTableCellsAllBordersItem resetTableCellsAllBordersItem1;

	private ToggleTableCellsAllBordersItem toggleTableCellsAllBordersItem1;

	private ToggleTableCellsOutsideBorderItem toggleTableCellsOutsideBorderItem1;

	private ToggleTableCellsInsideBorderItem toggleTableCellsInsideBorderItem1;

	private ToggleTableCellsInsideHorizontalBorderItem toggleTableCellsInsideHorizontalBorderItem1;

	private ToggleTableCellsInsideVerticalBorderItem toggleTableCellsInsideVerticalBorderItem1;

	private ToggleShowTableGridLinesItem toggleShowTableGridLinesItem1;

	private ChangeTableCellsShadingItem changeTableCellsShadingItem1;

	private SelectTableElementsItem selectTableElementsItem1;

	private SelectTableCellItem selectTableCellItem1;

	private SelectTableColumnItem selectTableColumnItem1;

	private SelectTableRowItem selectTableRowItem1;

	private SelectTableItem selectTableItem1;

	private ShowTablePropertiesFormItem showTablePropertiesFormItem1;

	private DeleteTableElementsItem deleteTableElementsItem1;

	private ShowDeleteTableCellsFormItem showDeleteTableCellsFormItem1;

	private DeleteTableColumnsItem deleteTableColumnsItem1;

	private DeleteTableRowsItem deleteTableRowsItem1;

	private DeleteTableItem deleteTableItem1;

	private InsertTableRowAboveItem insertTableRowAboveItem1;

	private InsertTableRowBelowItem insertTableRowBelowItem1;

	private InsertTableColumnToLeftItem insertTableColumnToLeftItem1;

	private InsertTableColumnToRightItem insertTableColumnToRightItem1;

	private MergeTableCellsItem mergeTableCellsItem1;

	private ShowSplitTableCellsForm showSplitTableCellsForm1;

	private SplitTableItem splitTableItem1;

	private ToggleTableAutoFitItem toggleTableAutoFitItem1;

	private ToggleTableAutoFitContentsItem toggleTableAutoFitContentsItem1;

	private ToggleTableAutoFitWindowItem toggleTableAutoFitWindowItem1;

	private ToggleTableFixedColumnWidthItem toggleTableFixedColumnWidthItem1;

	private ToggleTableCellsTopLeftAlignmentItem toggleTableCellsTopLeftAlignmentItem1;

	private ToggleTableCellsMiddleLeftAlignmentItem toggleTableCellsMiddleLeftAlignmentItem1;

	private ToggleTableCellsBottomLeftAlignmentItem toggleTableCellsBottomLeftAlignmentItem1;

	private ToggleTableCellsTopCenterAlignmentItem toggleTableCellsTopCenterAlignmentItem1;

	private ToggleTableCellsMiddleCenterAlignmentItem toggleTableCellsMiddleCenterAlignmentItem1;

	private ToggleTableCellsBottomCenterAlignmentItem toggleTableCellsBottomCenterAlignmentItem1;

	private ToggleTableCellsTopRightAlignmentItem toggleTableCellsTopRightAlignmentItem1;

	private ToggleTableCellsMiddleRightAlignmentItem toggleTableCellsMiddleRightAlignmentItem1;

	private ToggleTableCellsBottomRightAlignmentItem toggleTableCellsBottomRightAlignmentItem1;

	private ShowTableOptionsFormItem showTableOptionsFormItem1;

	private CheckSpellingItem checkSpellingItem1;

	private InsertTableOfContentsItem insertTableOfContentsItem1;

	private UpdateTableOfContentsItem updateTableOfContentsItem1;

	private AddParagraphsToTableOfContentItem addParagraphsToTableOfContentItem1;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem1;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem2;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem3;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem4;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem5;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem6;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem7;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem8;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem9;

	private SetParagraphHeadingLevelItem setParagraphHeadingLevelItem10;

	private InsertCaptionPlaceholderItem insertCaptionPlaceholderItem1;

	private InsertFiguresCaptionItems insertFiguresCaptionItems1;

	private InsertTablesCaptionItems insertTablesCaptionItems1;

	private InsertEquationsCaptionItems insertEquationsCaptionItems1;

	private InsertTableOfFiguresPlaceholderItem insertTableOfFiguresPlaceholderItem1;

	private InsertTableOfFiguresItems insertTableOfFiguresItems1;

	private InsertTableOfTablesItems insertTableOfTablesItems1;

	private InsertTableOfEquationsItems insertTableOfEquationsItems1;

	private UpdateTableOfFiguresItem updateTableOfFiguresItem1;

	private ChangeFloatingObjectFillColorItem changeFloatingObjectFillColorItem1;

	private ChangeFloatingObjectOutlineColorItem changeFloatingObjectOutlineColorItem1;

	private ChangeFloatingObjectOutlineWeightItem changeFloatingObjectOutlineWeightItem1;

	private RepositoryItemFloatingObjectOutlineWeight repositoryItemFloatingObjectOutlineWeight1;

	private ChangeFloatingObjectTextWrapTypeItem changeFloatingObjectTextWrapTypeItem1;

	private SetFloatingObjectSquareTextWrapTypeItem setFloatingObjectSquareTextWrapTypeItem1;

	private SetFloatingObjectTightTextWrapTypeItem setFloatingObjectTightTextWrapTypeItem1;

	private SetFloatingObjectThroughTextWrapTypeItem setFloatingObjectThroughTextWrapTypeItem1;

	private SetFloatingObjectTopAndBottomTextWrapTypeItem setFloatingObjectTopAndBottomTextWrapTypeItem1;

	private SetFloatingObjectBehindTextWrapTypeItem setFloatingObjectBehindTextWrapTypeItem1;

	private SetFloatingObjectInFrontOfTextWrapTypeItem setFloatingObjectInFrontOfTextWrapTypeItem1;

	private ChangeFloatingObjectAlignmentItem changeFloatingObjectAlignmentItem1;

	private SetFloatingObjectTopLeftAlignmentItem setFloatingObjectTopLeftAlignmentItem1;

	private SetFloatingObjectTopCenterAlignmentItem setFloatingObjectTopCenterAlignmentItem1;

	private SetFloatingObjectTopRightAlignmentItem setFloatingObjectTopRightAlignmentItem1;

	private SetFloatingObjectMiddleLeftAlignmentItem setFloatingObjectMiddleLeftAlignmentItem1;

	private SetFloatingObjectMiddleCenterAlignmentItem setFloatingObjectMiddleCenterAlignmentItem1;

	private SetFloatingObjectMiddleRightAlignmentItem setFloatingObjectMiddleRightAlignmentItem1;

	private SetFloatingObjectBottomLeftAlignmentItem setFloatingObjectBottomLeftAlignmentItem1;

	private SetFloatingObjectBottomCenterAlignmentItem setFloatingObjectBottomCenterAlignmentItem1;

	private SetFloatingObjectBottomRightAlignmentItem setFloatingObjectBottomRightAlignmentItem1;

	private FloatingObjectBringForwardSubItem floatingObjectBringForwardSubItem1;

	private FloatingObjectBringForwardItem floatingObjectBringForwardItem1;

	private FloatingObjectBringToFrontItem floatingObjectBringToFrontItem1;

	private FloatingObjectBringInFrontOfTextItem floatingObjectBringInFrontOfTextItem1;

	private FloatingObjectSendBackwardSubItem floatingObjectSendBackwardSubItem1;

	private FloatingObjectSendBackwardItem floatingObjectSendBackwardItem1;

	private FloatingObjectSendToBackItem floatingObjectSendToBackItem1;

	private FloatingObjectSendBehindTextItem floatingObjectSendBehindTextItem1;

	private ThemesGalleryBarItem themesGalleryBarItem1;

	private CommandBarCheckItem commandBarCheckItem2;

	private CommandBarCheckItem commandBarCheckItem3;

	private CommandBarCheckItem commandBarCheckItem4;

	private CommandBarSubItem commandBarSubItem1;

	private CommandBarCheckItem commandBarCheckItem5;

	private CommandBarCheckItem commandBarCheckItem6;

	private CommandBarCheckItem commandBarCheckItem7;

	private CommandBarCheckItem commandBarCheckItem8;

	private CommandBarCheckItem commandBarCheckItem9;

	private FilterPopupButtonItem filterPopupButtonItem1;

	private PropertiesBarButtonItem propertiesBarButtonItem1;

	private CommandBarSubItem commandBarSubItem2;

	private CommandBarCheckItem commandBarCheckItem10;

	private CommandBarCheckItem commandBarCheckItem11;

	private CommandBarSubItem commandBarSubItem3;

	private CommandBarCheckItem commandBarCheckItem12;

	private CommandBarCheckItem commandBarCheckItem13;

	private CommandBarItem commandBarItem8;

	private CommandBarItem commandBarItem9;

	private CommandBarSubItem commandBarSubItem4;

	private CommandBarCheckItem commandBarCheckItem14;

	private CommandBarCheckItem commandBarCheckItem15;

	private CommandBarCheckItem commandBarCheckItem16;

	private CommandBarCheckItem commandBarCheckItem17;

	private CommandBarCheckItem commandBarCheckItem18;

	private CommandBarCheckItem commandBarCheckItem19;

	private CommandBarCheckItem commandBarCheckItem20;

	private CommandBarSubItem commandBarSubItem5;

	private CommandBarCheckItem commandBarCheckItem21;

	private CommandBarCheckItem commandBarCheckItem22;

	private CommandBarSubItem commandBarSubItem6;

	private CommandBarCheckItem commandBarCheckItem23;

	private CommandBarCheckItem commandBarCheckItem24;

	private CommandBarSubItem commandBarSubItem7;

	private CommandBarCheckItem commandBarCheckItem25;

	private CommandBarCheckItem commandBarCheckItem26;

	private CommandBarCheckItem commandBarCheckItem27;

	private CommandBarCheckItem commandBarCheckItem28;

	private CommandBarCheckItem commandBarCheckItem29;

	private CommandBarCheckItem commandBarCheckItem30;

	private CommandBarCheckItem commandBarCheckItem31;

	private CommandBarItem commandBarItem10;

	private CommandBarItem commandBarItem11;

	private CommandBarItem commandBarItem12;

	private BarButtonGroup barButtonGroup8;

	private ChangeEditorRowLimitItem changeEditorRowLimitItem1;

	private RepositoryItemEditorRowLimitEdit repositoryItemEditorRowLimitEdit1;

	private CreateBarBaseItem createBarBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown1;

	private CreateLineBaseItem createLineBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown2;

	private CreatePieBaseItem createPieBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown3;

	private CreateRotatedBarBaseItem createRotatedBarBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown4;

	private CreateAreaBaseItem createAreaBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown5;

	private CreateOtherSeriesTypesBaseItem createOtherSeriesTypesBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown6;

	private ChangePaletteGalleryBaseItem changePaletteGalleryBaseItem1;

	private CommandBarGalleryDropDown commandBarGalleryDropDown7;

	private ChangeAppearanceGalleryBaseItem changeAppearanceGalleryBaseItem1;

	private RunDesignerChartItem runDesignerChartItem1;

	private MailMergeDataSourceItem mailMergeDataSourceItem1;

	private CommandBarCheckItem commandBarCheckItem32;

	private CommandBarCheckItem commandBarCheckItem33;

	private MailMergeCurrentRecordItem mailMergeCurrentRecordItem1;

	private RepositoryItemMailMergeCurrentRecordEdit repositoryItemMailMergeCurrentRecordEdit1;

	private CommandBarSubItem commandBarSubItem8;

	private CommandBarItem commandBarItem13;

	private CommandBarItem commandBarItem14;

	private CommandBarItem commandBarItem15;

	private HeaderFooterToolsRibbonPageCategory headerFooterToolsRibbonPageCategory1;

	private HeaderFooterToolsDesignRibbonPage headerFooterToolsDesignRibbonPage1;

	private HeaderFooterToolsDesignNavigationRibbonPageGroup headerFooterToolsDesignNavigationRibbonPageGroup1;

	private HeaderFooterToolsDesignOptionsRibbonPageGroup headerFooterToolsDesignOptionsRibbonPageGroup1;

	private HeaderFooterToolsDesignCloseRibbonPageGroup headerFooterToolsDesignCloseRibbonPageGroup1;

	private TableToolsRibbonPageCategory tableToolsRibbonPageCategory1;

	private TableDesignRibbonPage tableDesignRibbonPage1;

	private TableStyleOptionsRibbonPageGroup tableStyleOptionsRibbonPageGroup1;

	private TableStylesRibbonPageGroup tableStylesRibbonPageGroup1;

	private TableCellStylesRibbonPageGroup tableCellStylesRibbonPageGroup1;

	private TableDrawBordersRibbonPageGroup tableDrawBordersRibbonPageGroup1;

	private TableLayoutRibbonPage tableLayoutRibbonPage1;

	private TableTableRibbonPageGroup tableTableRibbonPageGroup1;

	private TableRowsAndColumnsRibbonPageGroup tableRowsAndColumnsRibbonPageGroup1;

	private TableMergeRibbonPageGroup tableMergeRibbonPageGroup1;

	private TableCellSizeRibbonPageGroup tableCellSizeRibbonPageGroup1;

	private TableAlignmentRibbonPageGroup tableAlignmentRibbonPageGroup1;

	private FloatingPictureToolsRibbonPageCategory floatingPictureToolsRibbonPageCategory1;

	private FloatingPictureToolsFormatPage floatingPictureToolsFormatPage1;

	private FloatingPictureToolsShapeStylesPageGroup floatingPictureToolsShapeStylesPageGroup1;

	private FloatingPictureToolsArrangePageGroup floatingPictureToolsArrangePageGroup1;

	private DataToolsRibbonPageCategory dataToolsRibbonPageCategory1;

	private AppearanceRibbonPage appearanceRibbonPage1;

	private ThemesRibbonPageGroup themesRibbonPageGroup1;

	private SNMergeFieldToolsRibbonPage snMergeFieldToolsRibbonPage1;

	private DataShapingRibbonPageGroup dataShapingRibbonPageGroup1;

	private SNMergeFieldPropertiesRibbonPageGroup snMergeFieldPropertiesRibbonPageGroup1;

	private GroupToolsRibbonPage groupToolsRibbonPage1;

	private GroupingRibbonPageGroup groupingRibbonPageGroup1;

	private ListToolsRibbonPage listToolsRibbonPage1;

	private ListHeaderAndFooterRibbonPageGroup listHeaderAndFooterRibbonPageGroup1;

	private ListCommandsRibbonPageGroup listCommandsRibbonPageGroup1;

	private ListEditorRowLimitRibbonPageGroup listEditorRowLimitRibbonPageGroup1;

	private MailMergeRibbonPage mailMergeRibbonPage1;

	private DevExpress.Snap.Extensions.UI.MailMergeRibbonPageGroup mailMergeRibbonPageGroup1;

	private MailMergeCurrentRecordRibbonPageGroup mailMergeCurrentRecordRibbonPageGroup1;

	private FinishAndMergeRibbonPageGroup finishAndMergeRibbonPageGroup1;

	private ChartRibbonPageCategory chartRibbonPageCategory1;

	private CreateChartRibbonPage createChartRibbonPage1;

	private ChartTypeRibbonPageGroup chartTypeRibbonPageGroup1;

	private ChartAppearanceRibbonPageGroup chartAppearanceRibbonPageGroup1;

	private ChartWizardRibbonPageGroup chartWizardRibbonPageGroup1;

	private FileRibbonPage fileRibbonPage1;

	private DevExpress.XtraRichEdit.UI.CommonRibbonPageGroup commonRibbonPageGroup1;

	private InfoRibbonPageGroup infoRibbonPageGroup1;

	private DataRibbonPageGroup dataRibbonPageGroup1;

	private HomeRibbonPage homeRibbonPage1;

	private ClipboardRibbonPageGroup clipboardRibbonPageGroup1;

	private FontRibbonPageGroup fontRibbonPageGroup1;

	private ParagraphRibbonPageGroup paragraphRibbonPageGroup1;

	private StylesRibbonPageGroup stylesRibbonPageGroup1;

	private EditingRibbonPageGroup editingRibbonPageGroup1;

	private InsertRibbonPage insertRibbonPage1;

	private PagesRibbonPageGroup pagesRibbonPageGroup1;

	private TablesRibbonPageGroup tablesRibbonPageGroup1;

	private IllustrationsRibbonPageGroup illustrationsRibbonPageGroup1;

	private ToolboxRibbonPageGroup toolboxRibbonPageGroup1;

	private LinksRibbonPageGroup linksRibbonPageGroup1;

	private HeaderFooterRibbonPageGroup headerFooterRibbonPageGroup1;

	private TextRibbonPageGroup textRibbonPageGroup1;

	private SymbolsRibbonPageGroup symbolsRibbonPageGroup1;

	private PageLayoutRibbonPage pageLayoutRibbonPage1;

	private PageSetupRibbonPageGroup pageSetupRibbonPageGroup1;

	private PageBackgroundRibbonPageGroup pageBackgroundRibbonPageGroup1;

	private ViewRibbonPage viewRibbonPage1;

	private DocumentViewsRibbonPageGroup documentViewsRibbonPageGroup1;

	private ShowRibbonPageGroup showRibbonPageGroup1;

	private ZoomRibbonPageGroup zoomRibbonPageGroup1;

	private ViewRibbonPageGroup viewRibbonPageGroup1;

	private ViewFieldsRibbonPageGroup viewFieldsRibbonPageGroup1;

	private ReviewRibbonPage reviewRibbonPage1;

	private DocumentProofingRibbonPageGroup documentProofingRibbonPageGroup1;

	private ReferencesRibbonPage referencesRibbonPage1;

	private TableOfContentsRibbonPageGroup tableOfContentsRibbonPageGroup1;

	private CaptionsRibbonPageGroup captionsRibbonPageGroup1;

	private SnapBarController snapBarController1;

	private BarButtonItem fileSaveItem1_Custom;

	public DataSet Source { get; set; }

	public new string HtmlText { get; set; }

	public frm_ReportWriter()
	{
		InitializeComponent();
	}

	private void frm_ReportWriter_Load(object sender, EventArgs e)
	{
		try
		{
			fileSaveItem1.Visibility = BarItemVisibility.Never;
			if (Source.IsNotNull())
			{
				BindingSource bindingSource = new BindingSource();
				bindingSource.DataSource = Source;
				bindingSource.DataMember = "";
				snapControl1.DataSource = bindingSource;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void fileSaveItem1_Custom_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			HtmlText = snapControl1.HtmlText;
			DialogResult = DialogResult.Yes;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ReportWriter));
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup2 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup3 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DColumn chartControlCommandGalleryItemGroup2DColumn = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DColumn();
		DevExpress.XtraCharts.UI.CreateBarChartItem createBarChartItem = new DevExpress.XtraCharts.UI.CreateBarChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedBarChartItem createFullStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideFullStackedBarChartItem createSideBySideFullStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideFullStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideStackedBarChartItem createSideBySideStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateStackedBarChartItem createStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateStackedBarChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DColumn chartControlCommandGalleryItemGroup3DColumn = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DColumn();
		DevExpress.XtraCharts.UI.CreateBar3DChartItem createBar3DChartItem = new DevExpress.XtraCharts.UI.CreateBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedBar3DChartItem createFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateManhattanBarChartItem createManhattanBarChartItem = new DevExpress.XtraCharts.UI.CreateManhattanBarChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideFullStackedBar3DChartItem createSideBySideFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideStackedBar3DChartItem createSideBySideStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateStackedBar3DChartItem createStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupCylinderColumn chartControlCommandGalleryItemGroupCylinderColumn = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupCylinderColumn();
		DevExpress.XtraCharts.UI.CreateCylinderBar3DChartItem createCylinderBar3DChartItem = new DevExpress.XtraCharts.UI.CreateCylinderBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateCylinderFullStackedBar3DChartItem createCylinderFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateCylinderFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateCylinderManhattanBarChartItem createCylinderManhattanBarChartItem = new DevExpress.XtraCharts.UI.CreateCylinderManhattanBarChartItem();
		DevExpress.XtraCharts.UI.CreateCylinderSideBySideFullStackedBar3DChartItem createCylinderSideBySideFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateCylinderSideBySideFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateCylinderSideBySideStackedBar3DChartItem createCylinderSideBySideStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateCylinderSideBySideStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateCylinderStackedBar3DChartItem createCylinderStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateCylinderStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupConeColumn chartControlCommandGalleryItemGroupConeColumn = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupConeColumn();
		DevExpress.XtraCharts.UI.CreateConeBar3DChartItem createConeBar3DChartItem = new DevExpress.XtraCharts.UI.CreateConeBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateConeFullStackedBar3DChartItem createConeFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateConeFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateConeManhattanBarChartItem createConeManhattanBarChartItem = new DevExpress.XtraCharts.UI.CreateConeManhattanBarChartItem();
		DevExpress.XtraCharts.UI.CreateConeSideBySideFullStackedBar3DChartItem createConeSideBySideFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateConeSideBySideFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateConeSideBySideStackedBar3DChartItem createConeSideBySideStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateConeSideBySideStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreateConeStackedBar3DChartItem createConeStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreateConeStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPyramidColumn chartControlCommandGalleryItemGroupPyramidColumn = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPyramidColumn();
		DevExpress.XtraCharts.UI.CreatePyramidBar3DChartItem createPyramidBar3DChartItem = new DevExpress.XtraCharts.UI.CreatePyramidBar3DChartItem();
		DevExpress.XtraCharts.UI.CreatePyramidFullStackedBar3DChartItem createPyramidFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreatePyramidFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreatePyramidManhattanBarChartItem createPyramidManhattanBarChartItem = new DevExpress.XtraCharts.UI.CreatePyramidManhattanBarChartItem();
		DevExpress.XtraCharts.UI.CreatePyramidSideBySideFullStackedBar3DChartItem createPyramidSideBySideFullStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreatePyramidSideBySideFullStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreatePyramidSideBySideStackedBar3DChartItem createPyramidSideBySideStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreatePyramidSideBySideStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.CreatePyramidStackedBar3DChartItem createPyramidStackedBar3DChartItem = new DevExpress.XtraCharts.UI.CreatePyramidStackedBar3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DLine chartControlCommandGalleryItemGroup2DLine = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DLine();
		DevExpress.XtraCharts.UI.CreateLineChartItem createLineChartItem = new DevExpress.XtraCharts.UI.CreateLineChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedLineChartItem createFullStackedLineChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedLineChartItem();
		DevExpress.XtraCharts.UI.CreateScatterLineChartItem createScatterLineChartItem = new DevExpress.XtraCharts.UI.CreateScatterLineChartItem();
		DevExpress.XtraCharts.UI.CreateSplineChartItem createSplineChartItem = new DevExpress.XtraCharts.UI.CreateSplineChartItem();
		DevExpress.XtraCharts.UI.CreateStackedLineChartItem createStackedLineChartItem = new DevExpress.XtraCharts.UI.CreateStackedLineChartItem();
		DevExpress.XtraCharts.UI.CreateStepLineChartItem createStepLineChartItem = new DevExpress.XtraCharts.UI.CreateStepLineChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DLine chartControlCommandGalleryItemGroup3DLine = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DLine();
		DevExpress.XtraCharts.UI.CreateLine3DChartItem createLine3DChartItem = new DevExpress.XtraCharts.UI.CreateLine3DChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedLine3DChartItem createFullStackedLine3DChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedLine3DChartItem();
		DevExpress.XtraCharts.UI.CreateSpline3DChartItem createSpline3DChartItem = new DevExpress.XtraCharts.UI.CreateSpline3DChartItem();
		DevExpress.XtraCharts.UI.CreateStackedLine3DChartItem createStackedLine3DChartItem = new DevExpress.XtraCharts.UI.CreateStackedLine3DChartItem();
		DevExpress.XtraCharts.UI.CreateStepLine3DChartItem createStepLine3DChartItem = new DevExpress.XtraCharts.UI.CreateStepLine3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DPie chartControlCommandGalleryItemGroup2DPie = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DPie();
		DevExpress.XtraCharts.UI.CreatePieChartItem createPieChartItem = new DevExpress.XtraCharts.UI.CreatePieChartItem();
		DevExpress.XtraCharts.UI.CreateDoughnutChartItem createDoughnutChartItem = new DevExpress.XtraCharts.UI.CreateDoughnutChartItem();
		DevExpress.XtraCharts.UI.CreateNestedDoughnutChartItem createNestedDoughnutChartItem = new DevExpress.XtraCharts.UI.CreateNestedDoughnutChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DPie chartControlCommandGalleryItemGroup3DPie = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DPie();
		DevExpress.XtraCharts.UI.CreatePie3DChartItem createPie3DChartItem = new DevExpress.XtraCharts.UI.CreatePie3DChartItem();
		DevExpress.XtraCharts.UI.CreateDoughnut3DChartItem createDoughnut3DChartItem = new DevExpress.XtraCharts.UI.CreateDoughnut3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DBar chartControlCommandGalleryItemGroup2DBar = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DBar();
		DevExpress.XtraCharts.UI.CreateRotatedBarChartItem createRotatedBarChartItem = new DevExpress.XtraCharts.UI.CreateRotatedBarChartItem();
		DevExpress.XtraCharts.UI.CreateRotatedFullStackedBarChartItem createRotatedFullStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateRotatedFullStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateRotatedSideBySideFullStackedBarChartItem createRotatedSideBySideFullStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateRotatedSideBySideFullStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateRotatedSideBySideStackedBarChartItem createRotatedSideBySideStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateRotatedSideBySideStackedBarChartItem();
		DevExpress.XtraCharts.UI.CreateRotatedStackedBarChartItem createRotatedStackedBarChartItem = new DevExpress.XtraCharts.UI.CreateRotatedStackedBarChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DArea chartControlCommandGalleryItemGroup2DArea = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup2DArea();
		DevExpress.XtraCharts.UI.CreateAreaChartItem createAreaChartItem = new DevExpress.XtraCharts.UI.CreateAreaChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedAreaChartItem createFullStackedAreaChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedAreaChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedSplineAreaChartItem createFullStackedSplineAreaChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedSplineAreaChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedStepAreaChartItem createFullStackedStepAreaChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedStepAreaChartItem();
		DevExpress.XtraCharts.UI.CreateSplineAreaChartItem createSplineAreaChartItem = new DevExpress.XtraCharts.UI.CreateSplineAreaChartItem();
		DevExpress.XtraCharts.UI.CreateStackedAreaChartItem createStackedAreaChartItem = new DevExpress.XtraCharts.UI.CreateStackedAreaChartItem();
		DevExpress.XtraCharts.UI.CreateStackedStepAreaChartItem createStackedStepAreaChartItem = new DevExpress.XtraCharts.UI.CreateStackedStepAreaChartItem();
		DevExpress.XtraCharts.UI.CreateStackedSplineAreaChartItem createStackedSplineAreaChartItem = new DevExpress.XtraCharts.UI.CreateStackedSplineAreaChartItem();
		DevExpress.XtraCharts.UI.CreateStepAreaChartItem createStepAreaChartItem = new DevExpress.XtraCharts.UI.CreateStepAreaChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DArea chartControlCommandGalleryItemGroup3DArea = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroup3DArea();
		DevExpress.XtraCharts.UI.CreateArea3DChartItem createArea3DChartItem = new DevExpress.XtraCharts.UI.CreateArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedArea3DChartItem createFullStackedArea3DChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateFullStackedSplineArea3DChartItem createFullStackedSplineArea3DChartItem = new DevExpress.XtraCharts.UI.CreateFullStackedSplineArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateSplineArea3DChartItem createSplineArea3DChartItem = new DevExpress.XtraCharts.UI.CreateSplineArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateStackedArea3DChartItem createStackedArea3DChartItem = new DevExpress.XtraCharts.UI.CreateStackedArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateStackedSplineArea3DChartItem createStackedSplineArea3DChartItem = new DevExpress.XtraCharts.UI.CreateStackedSplineArea3DChartItem();
		DevExpress.XtraCharts.UI.CreateStepArea3DChartItem createStepArea3DChartItem = new DevExpress.XtraCharts.UI.CreateStepArea3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPoint chartControlCommandGalleryItemGroupPoint = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPoint();
		DevExpress.XtraCharts.UI.CreatePointChartItem createPointChartItem = new DevExpress.XtraCharts.UI.CreatePointChartItem();
		DevExpress.XtraCharts.UI.CreateBubbleChartItem createBubbleChartItem = new DevExpress.XtraCharts.UI.CreateBubbleChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupFunnel chartControlCommandGalleryItemGroupFunnel = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupFunnel();
		DevExpress.XtraCharts.UI.CreateFunnelChartItem createFunnelChartItem = new DevExpress.XtraCharts.UI.CreateFunnelChartItem();
		DevExpress.XtraCharts.UI.CreateFunnel3DChartItem createFunnel3DChartItem = new DevExpress.XtraCharts.UI.CreateFunnel3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupFinancial chartControlCommandGalleryItemGroupFinancial = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupFinancial();
		DevExpress.XtraCharts.UI.CreateStockChartItem createStockChartItem = new DevExpress.XtraCharts.UI.CreateStockChartItem();
		DevExpress.XtraCharts.UI.CreateCandleStickChartItem createCandleStickChartItem = new DevExpress.XtraCharts.UI.CreateCandleStickChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupRadar chartControlCommandGalleryItemGroupRadar = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupRadar();
		DevExpress.XtraCharts.UI.CreateRadarPointChartItem createRadarPointChartItem = new DevExpress.XtraCharts.UI.CreateRadarPointChartItem();
		DevExpress.XtraCharts.UI.CreateRadarLineChartItem createRadarLineChartItem = new DevExpress.XtraCharts.UI.CreateRadarLineChartItem();
		DevExpress.XtraCharts.UI.CreateRadarAreaChartItem createRadarAreaChartItem = new DevExpress.XtraCharts.UI.CreateRadarAreaChartItem();
		DevExpress.XtraCharts.UI.CreateRadarRangeAreaChartItem createRadarRangeAreaChartItem = new DevExpress.XtraCharts.UI.CreateRadarRangeAreaChartItem();
		DevExpress.XtraCharts.UI.CreateScatterRadarLineChartItem createScatterRadarLineChartItem = new DevExpress.XtraCharts.UI.CreateScatterRadarLineChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPolar chartControlCommandGalleryItemGroupPolar = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupPolar();
		DevExpress.XtraCharts.UI.CreatePolarPointChartItem createPolarPointChartItem = new DevExpress.XtraCharts.UI.CreatePolarPointChartItem();
		DevExpress.XtraCharts.UI.CreatePolarLineChartItem createPolarLineChartItem = new DevExpress.XtraCharts.UI.CreatePolarLineChartItem();
		DevExpress.XtraCharts.UI.CreatePolarAreaChartItem createPolarAreaChartItem = new DevExpress.XtraCharts.UI.CreatePolarAreaChartItem();
		DevExpress.XtraCharts.UI.CreatePolarRangeAreaChartItem createPolarRangeAreaChartItem = new DevExpress.XtraCharts.UI.CreatePolarRangeAreaChartItem();
		DevExpress.XtraCharts.UI.CreateScatterPolarLineChartItem createScatterPolarLineChartItem = new DevExpress.XtraCharts.UI.CreateScatterPolarLineChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupRange chartControlCommandGalleryItemGroupRange = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupRange();
		DevExpress.XtraCharts.UI.CreateRangeBarChartItem createRangeBarChartItem = new DevExpress.XtraCharts.UI.CreateRangeBarChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideRangeBarChartItem createSideBySideRangeBarChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideRangeBarChartItem();
		DevExpress.XtraCharts.UI.CreateRangeAreaChartItem createRangeAreaChartItem = new DevExpress.XtraCharts.UI.CreateRangeAreaChartItem();
		DevExpress.XtraCharts.UI.CreateRangeArea3DChartItem createRangeArea3DChartItem = new DevExpress.XtraCharts.UI.CreateRangeArea3DChartItem();
		DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupGantt chartControlCommandGalleryItemGroupGantt = new DevExpress.XtraCharts.UI.ChartControlCommandGalleryItemGroupGantt();
		DevExpress.XtraCharts.UI.CreateGanttChartItem createGanttChartItem = new DevExpress.XtraCharts.UI.CreateGanttChartItem();
		DevExpress.XtraCharts.UI.CreateSideBySideGanttChartItem createSideBySideGanttChartItem = new DevExpress.XtraCharts.UI.CreateSideBySideGanttChartItem();
		DevExpress.Skins.SkinPaddingEdges skinPaddingEdges = new DevExpress.Skins.SkinPaddingEdges();
		DevExpress.Skins.SkinPaddingEdges skinPaddingEdges2 = new DevExpress.Skins.SkinPaddingEdges();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation = new DevExpress.XtraBars.Ribbon.ReduceOperation();
		DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation2 = new DevExpress.XtraBars.Ribbon.ReduceOperation();
		this.themesRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ThemesRibbonPageGroup();
		this.themesGalleryBarItem1 = new DevExpress.Snap.Extensions.UI.ThemesGalleryBarItem();
		this.stylesRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.StylesRibbonPageGroup();
		this.galleryChangeStyleItem1 = new DevExpress.XtraRichEdit.UI.GalleryChangeStyleItem();
		this.snapControl1 = new DevExpress.Snap.SnapControl();
		this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.undoItem1 = new DevExpress.XtraRichEdit.UI.UndoItem();
		this.redoItem1 = new DevExpress.XtraRichEdit.UI.RedoItem();
		this.fileNewItem1 = new DevExpress.XtraRichEdit.UI.FileNewItem();
		this.fileOpenItem1 = new DevExpress.XtraRichEdit.UI.FileOpenItem();
		this.fileSaveItem1 = new DevExpress.XtraRichEdit.UI.FileSaveItem();
		this.fileSaveAsItem1 = new DevExpress.XtraRichEdit.UI.FileSaveAsItem();
		this.commandBarItem1 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.quickPrintItem1 = new DevExpress.XtraRichEdit.UI.QuickPrintItem();
		this.printItem1 = new DevExpress.XtraRichEdit.UI.PrintItem();
		this.printPreviewItem1 = new DevExpress.XtraRichEdit.UI.PrintPreviewItem();
		this.showDocumentPropertiesFormItem1 = new DevExpress.XtraRichEdit.UI.ShowDocumentPropertiesFormItem();
		this.commandBarItem2 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.pasteItem1 = new DevExpress.XtraRichEdit.UI.PasteItem();
		this.cutItem1 = new DevExpress.XtraRichEdit.UI.CutItem();
		this.copyItem1 = new DevExpress.XtraRichEdit.UI.CopyItem();
		this.pasteSpecialItem1 = new DevExpress.XtraRichEdit.UI.PasteSpecialItem();
		this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeFontNameItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontNameItem();
		this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
		this.changeFontSizeItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontSizeItem();
		this.repositoryItemRichEditFontSizeEdit1 = new DevExpress.XtraRichEdit.Design.RepositoryItemRichEditFontSizeEdit();
		this.fontSizeIncreaseItem1 = new DevExpress.XtraRichEdit.UI.FontSizeIncreaseItem();
		this.fontSizeDecreaseItem1 = new DevExpress.XtraRichEdit.UI.FontSizeDecreaseItem();
		this.barButtonGroup2 = new DevExpress.XtraBars.BarButtonGroup();
		this.toggleFontBoldItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontBoldItem();
		this.toggleFontItalicItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontItalicItem();
		this.toggleFontUnderlineItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontUnderlineItem();
		this.toggleFontDoubleUnderlineItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleUnderlineItem();
		this.toggleFontStrikeoutItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontStrikeoutItem();
		this.toggleFontDoubleStrikeoutItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontDoubleStrikeoutItem();
		this.toggleFontSuperscriptItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontSuperscriptItem();
		this.toggleFontSubscriptItem1 = new DevExpress.XtraRichEdit.UI.ToggleFontSubscriptItem();
		this.barButtonGroup3 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeFontColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontColorItem();
		this.changeFontHighlightColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFontHighlightColorItem();
		this.changeTextCaseItem1 = new DevExpress.XtraRichEdit.UI.ChangeTextCaseItem();
		this.makeTextUpperCaseItem1 = new DevExpress.XtraRichEdit.UI.MakeTextUpperCaseItem();
		this.makeTextLowerCaseItem1 = new DevExpress.XtraRichEdit.UI.MakeTextLowerCaseItem();
		this.capitalizeEachWordCaseItem1 = new DevExpress.XtraRichEdit.UI.CapitalizeEachWordCaseItem();
		this.toggleTextCaseItem1 = new DevExpress.XtraRichEdit.UI.ToggleTextCaseItem();
		this.clearFormattingItem1 = new DevExpress.XtraRichEdit.UI.ClearFormattingItem();
		this.barButtonGroup4 = new DevExpress.XtraBars.BarButtonGroup();
		this.toggleBulletedListItem1 = new DevExpress.XtraRichEdit.UI.ToggleBulletedListItem();
		this.toggleNumberingListItem1 = new DevExpress.XtraRichEdit.UI.ToggleNumberingListItem();
		this.toggleMultiLevelListItem1 = new DevExpress.XtraRichEdit.UI.ToggleMultiLevelListItem();
		this.barButtonGroup5 = new DevExpress.XtraBars.BarButtonGroup();
		this.decreaseIndentItem1 = new DevExpress.XtraRichEdit.UI.DecreaseIndentItem();
		this.increaseIndentItem1 = new DevExpress.XtraRichEdit.UI.IncreaseIndentItem();
		this.toggleShowWhitespaceItem1 = new DevExpress.XtraRichEdit.UI.ToggleShowWhitespaceItem();
		this.barButtonGroup6 = new DevExpress.XtraBars.BarButtonGroup();
		this.toggleParagraphAlignmentLeftItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentLeftItem();
		this.toggleParagraphAlignmentCenterItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentCenterItem();
		this.toggleParagraphAlignmentRightItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentRightItem();
		this.toggleParagraphAlignmentJustifyItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphAlignmentJustifyItem();
		this.barButtonGroup7 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeParagraphLineSpacingItem1 = new DevExpress.XtraRichEdit.UI.ChangeParagraphLineSpacingItem();
		this.setSingleParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetSingleParagraphSpacingItem();
		this.setSesquialteralParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetSesquialteralParagraphSpacingItem();
		this.setDoubleParagraphSpacingItem1 = new DevExpress.XtraRichEdit.UI.SetDoubleParagraphSpacingItem();
		this.showLineSpacingFormItem1 = new DevExpress.XtraRichEdit.UI.ShowLineSpacingFormItem();
		this.addSpacingBeforeParagraphItem1 = new DevExpress.XtraRichEdit.UI.AddSpacingBeforeParagraphItem();
		this.removeSpacingBeforeParagraphItem1 = new DevExpress.XtraRichEdit.UI.RemoveSpacingBeforeParagraphItem();
		this.addSpacingAfterParagraphItem1 = new DevExpress.XtraRichEdit.UI.AddSpacingAfterParagraphItem();
		this.removeSpacingAfterParagraphItem1 = new DevExpress.XtraRichEdit.UI.RemoveSpacingAfterParagraphItem();
		this.changeParagraphBackColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeParagraphBackColorItem();
		this.findItem1 = new DevExpress.XtraRichEdit.UI.FindItem();
		this.replaceItem1 = new DevExpress.XtraRichEdit.UI.ReplaceItem();
		this.insertPageBreakItem21 = new DevExpress.XtraRichEdit.UI.InsertPageBreakItem2();
		this.insertTableItem1 = new DevExpress.XtraRichEdit.UI.InsertTableItem();
		this.insertPictureItem1 = new DevExpress.XtraRichEdit.UI.InsertPictureItem();
		this.insertFloatingPictureItem1 = new DevExpress.XtraRichEdit.UI.InsertFloatingPictureItem();
		this.commandBarItem3 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem4 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem5 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem6 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem7 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.insertBookmarkItem1 = new DevExpress.XtraRichEdit.UI.InsertBookmarkItem();
		this.insertHyperlinkItem1 = new DevExpress.XtraRichEdit.UI.InsertHyperlinkItem();
		this.editPageHeaderItem1 = new DevExpress.XtraRichEdit.UI.EditPageHeaderItem();
		this.editPageFooterItem1 = new DevExpress.XtraRichEdit.UI.EditPageFooterItem();
		this.insertPageNumberItem1 = new DevExpress.XtraRichEdit.UI.InsertPageNumberItem();
		this.insertPageCountItem1 = new DevExpress.XtraRichEdit.UI.InsertPageCountItem();
		this.insertTextBoxItem1 = new DevExpress.XtraRichEdit.UI.InsertTextBoxItem();
		this.insertSymbolItem1 = new DevExpress.XtraRichEdit.UI.InsertSymbolItem();
		this.changeSectionPageMarginsItem1 = new DevExpress.XtraRichEdit.UI.ChangeSectionPageMarginsItem();
		this.setNormalSectionPageMarginsItem1 = new DevExpress.XtraRichEdit.UI.SetNormalSectionPageMarginsItem();
		this.setNarrowSectionPageMarginsItem1 = new DevExpress.XtraRichEdit.UI.SetNarrowSectionPageMarginsItem();
		this.setModerateSectionPageMarginsItem1 = new DevExpress.XtraRichEdit.UI.SetModerateSectionPageMarginsItem();
		this.setWideSectionPageMarginsItem1 = new DevExpress.XtraRichEdit.UI.SetWideSectionPageMarginsItem();
		this.showPageMarginsSetupFormItem1 = new DevExpress.XtraRichEdit.UI.ShowPageMarginsSetupFormItem();
		this.changeSectionPageOrientationItem1 = new DevExpress.XtraRichEdit.UI.ChangeSectionPageOrientationItem();
		this.setPortraitPageOrientationItem1 = new DevExpress.XtraRichEdit.UI.SetPortraitPageOrientationItem();
		this.setLandscapePageOrientationItem1 = new DevExpress.XtraRichEdit.UI.SetLandscapePageOrientationItem();
		this.changeSectionPaperKindItem1 = new DevExpress.XtraRichEdit.UI.ChangeSectionPaperKindItem();
		this.changeSectionColumnsItem1 = new DevExpress.XtraRichEdit.UI.ChangeSectionColumnsItem();
		this.setSectionOneColumnItem1 = new DevExpress.XtraRichEdit.UI.SetSectionOneColumnItem();
		this.setSectionTwoColumnsItem1 = new DevExpress.XtraRichEdit.UI.SetSectionTwoColumnsItem();
		this.setSectionThreeColumnsItem1 = new DevExpress.XtraRichEdit.UI.SetSectionThreeColumnsItem();
		this.showColumnsSetupFormItem1 = new DevExpress.XtraRichEdit.UI.ShowColumnsSetupFormItem();
		this.insertBreakItem1 = new DevExpress.XtraRichEdit.UI.InsertBreakItem();
		this.insertPageBreakItem1 = new DevExpress.XtraRichEdit.UI.InsertPageBreakItem();
		this.insertColumnBreakItem1 = new DevExpress.XtraRichEdit.UI.InsertColumnBreakItem();
		this.insertSectionBreakNextPageItem1 = new DevExpress.XtraRichEdit.UI.InsertSectionBreakNextPageItem();
		this.insertSectionBreakEvenPageItem1 = new DevExpress.XtraRichEdit.UI.InsertSectionBreakEvenPageItem();
		this.insertSectionBreakOddPageItem1 = new DevExpress.XtraRichEdit.UI.InsertSectionBreakOddPageItem();
		this.changeSectionLineNumberingItem1 = new DevExpress.XtraRichEdit.UI.ChangeSectionLineNumberingItem();
		this.setSectionLineNumberingNoneItem1 = new DevExpress.XtraRichEdit.UI.SetSectionLineNumberingNoneItem();
		this.setSectionLineNumberingContinuousItem1 = new DevExpress.XtraRichEdit.UI.SetSectionLineNumberingContinuousItem();
		this.setSectionLineNumberingRestartNewPageItem1 = new DevExpress.XtraRichEdit.UI.SetSectionLineNumberingRestartNewPageItem();
		this.setSectionLineNumberingRestartNewSectionItem1 = new DevExpress.XtraRichEdit.UI.SetSectionLineNumberingRestartNewSectionItem();
		this.toggleParagraphSuppressLineNumbersItem1 = new DevExpress.XtraRichEdit.UI.ToggleParagraphSuppressLineNumbersItem();
		this.showLineNumberingFormItem1 = new DevExpress.XtraRichEdit.UI.ShowLineNumberingFormItem();
		this.changePageColorItem1 = new DevExpress.XtraRichEdit.UI.ChangePageColorItem();
		this.switchToSimpleViewItem1 = new DevExpress.XtraRichEdit.UI.SwitchToSimpleViewItem();
		this.switchToDraftViewItem1 = new DevExpress.XtraRichEdit.UI.SwitchToDraftViewItem();
		this.switchToPrintLayoutViewItem1 = new DevExpress.XtraRichEdit.UI.SwitchToPrintLayoutViewItem();
		this.toggleShowHorizontalRulerItem1 = new DevExpress.XtraRichEdit.UI.ToggleShowHorizontalRulerItem();
		this.toggleShowVerticalRulerItem1 = new DevExpress.XtraRichEdit.UI.ToggleShowVerticalRulerItem();
		this.zoomOutItem1 = new DevExpress.XtraRichEdit.UI.ZoomOutItem();
		this.zoomInItem1 = new DevExpress.XtraRichEdit.UI.ZoomInItem();
		this.snapBarToolbarsListItem1 = new DevExpress.Snap.Extensions.UI.SnapBarToolbarsListItem();
		this.commandBarCheckItem1 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.showAllFieldCodesItem1 = new DevExpress.XtraRichEdit.UI.ShowAllFieldCodesItem();
		this.showAllFieldResultsItem1 = new DevExpress.XtraRichEdit.UI.ShowAllFieldResultsItem();
		this.toggleFieldHighlightingItem1 = new DevExpress.Snap.Extensions.UI.ToggleFieldHighlightingItem();
		this.goToPageHeaderItem1 = new DevExpress.XtraRichEdit.UI.GoToPageHeaderItem();
		this.goToPageFooterItem1 = new DevExpress.XtraRichEdit.UI.GoToPageFooterItem();
		this.goToNextHeaderFooterItem1 = new DevExpress.XtraRichEdit.UI.GoToNextHeaderFooterItem();
		this.goToPreviousHeaderFooterItem1 = new DevExpress.XtraRichEdit.UI.GoToPreviousHeaderFooterItem();
		this.toggleLinkToPreviousItem1 = new DevExpress.XtraRichEdit.UI.ToggleLinkToPreviousItem();
		this.toggleDifferentFirstPageItem1 = new DevExpress.XtraRichEdit.UI.ToggleDifferentFirstPageItem();
		this.toggleDifferentOddAndEvenPagesItem1 = new DevExpress.XtraRichEdit.UI.ToggleDifferentOddAndEvenPagesItem();
		this.closePageHeaderFooterItem1 = new DevExpress.XtraRichEdit.UI.ClosePageHeaderFooterItem();
		this.toggleFirstRowItem1 = new DevExpress.XtraRichEdit.UI.ToggleFirstRowItem();
		this.toggleLastRowItem1 = new DevExpress.XtraRichEdit.UI.ToggleLastRowItem();
		this.toggleBandedRowsItem1 = new DevExpress.XtraRichEdit.UI.ToggleBandedRowsItem();
		this.toggleFirstColumnItem1 = new DevExpress.XtraRichEdit.UI.ToggleFirstColumnItem();
		this.toggleLastColumnItem1 = new DevExpress.XtraRichEdit.UI.ToggleLastColumnItem();
		this.toggleBandedColumnsItem1 = new DevExpress.XtraRichEdit.UI.ToggleBandedColumnsItem();
		this.galleryChangeTableStyleItem1 = new DevExpress.XtraRichEdit.UI.GalleryChangeTableStyleItem();
		this.galleryChangeTableCellStyleItem1 = new DevExpress.Snap.Extensions.UI.GalleryChangeTableCellStyleItem();
		this.changeTableBorderLineStyleItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineStyleItem();
		this.repositoryItemBorderLineStyle1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineStyle();
		this.changeTableBorderLineWeightItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderLineWeightItem();
		this.repositoryItemBorderLineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemBorderLineWeight();
		this.changeTableBorderColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBorderColorItem();
		this.changeTableBordersItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableBordersItem();
		this.toggleTableCellsBottomBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomBorderItem();
		this.toggleTableCellsTopBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopBorderItem();
		this.toggleTableCellsLeftBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsLeftBorderItem();
		this.toggleTableCellsRightBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsRightBorderItem();
		this.resetTableCellsAllBordersItem1 = new DevExpress.XtraRichEdit.UI.ResetTableCellsAllBordersItem();
		this.toggleTableCellsAllBordersItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsAllBordersItem();
		this.toggleTableCellsOutsideBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsOutsideBorderItem();
		this.toggleTableCellsInsideBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideBorderItem();
		this.toggleTableCellsInsideHorizontalBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideHorizontalBorderItem();
		this.toggleTableCellsInsideVerticalBorderItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsInsideVerticalBorderItem();
		this.toggleShowTableGridLinesItem1 = new DevExpress.XtraRichEdit.UI.ToggleShowTableGridLinesItem();
		this.changeTableCellsShadingItem1 = new DevExpress.XtraRichEdit.UI.ChangeTableCellsShadingItem();
		this.selectTableElementsItem1 = new DevExpress.XtraRichEdit.UI.SelectTableElementsItem();
		this.selectTableCellItem1 = new DevExpress.XtraRichEdit.UI.SelectTableCellItem();
		this.selectTableColumnItem1 = new DevExpress.XtraRichEdit.UI.SelectTableColumnItem();
		this.selectTableRowItem1 = new DevExpress.XtraRichEdit.UI.SelectTableRowItem();
		this.selectTableItem1 = new DevExpress.XtraRichEdit.UI.SelectTableItem();
		this.showTablePropertiesFormItem1 = new DevExpress.XtraRichEdit.UI.ShowTablePropertiesFormItem();
		this.deleteTableElementsItem1 = new DevExpress.XtraRichEdit.UI.DeleteTableElementsItem();
		this.showDeleteTableCellsFormItem1 = new DevExpress.XtraRichEdit.UI.ShowDeleteTableCellsFormItem();
		this.deleteTableColumnsItem1 = new DevExpress.XtraRichEdit.UI.DeleteTableColumnsItem();
		this.deleteTableRowsItem1 = new DevExpress.XtraRichEdit.UI.DeleteTableRowsItem();
		this.deleteTableItem1 = new DevExpress.XtraRichEdit.UI.DeleteTableItem();
		this.insertTableRowAboveItem1 = new DevExpress.XtraRichEdit.UI.InsertTableRowAboveItem();
		this.insertTableRowBelowItem1 = new DevExpress.XtraRichEdit.UI.InsertTableRowBelowItem();
		this.insertTableColumnToLeftItem1 = new DevExpress.XtraRichEdit.UI.InsertTableColumnToLeftItem();
		this.insertTableColumnToRightItem1 = new DevExpress.XtraRichEdit.UI.InsertTableColumnToRightItem();
		this.mergeTableCellsItem1 = new DevExpress.XtraRichEdit.UI.MergeTableCellsItem();
		this.showSplitTableCellsForm1 = new DevExpress.XtraRichEdit.UI.ShowSplitTableCellsForm();
		this.splitTableItem1 = new DevExpress.XtraRichEdit.UI.SplitTableItem();
		this.toggleTableAutoFitItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitItem();
		this.toggleTableAutoFitContentsItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitContentsItem();
		this.toggleTableAutoFitWindowItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableAutoFitWindowItem();
		this.toggleTableFixedColumnWidthItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableFixedColumnWidthItem();
		this.toggleTableCellsTopLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopLeftAlignmentItem();
		this.toggleTableCellsMiddleLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleLeftAlignmentItem();
		this.toggleTableCellsBottomLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomLeftAlignmentItem();
		this.toggleTableCellsTopCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopCenterAlignmentItem();
		this.toggleTableCellsMiddleCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleCenterAlignmentItem();
		this.toggleTableCellsBottomCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomCenterAlignmentItem();
		this.toggleTableCellsTopRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsTopRightAlignmentItem();
		this.toggleTableCellsMiddleRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsMiddleRightAlignmentItem();
		this.toggleTableCellsBottomRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ToggleTableCellsBottomRightAlignmentItem();
		this.showTableOptionsFormItem1 = new DevExpress.XtraRichEdit.UI.ShowTableOptionsFormItem();
		this.checkSpellingItem1 = new DevExpress.XtraRichEdit.UI.CheckSpellingItem();
		this.insertTableOfContentsItem1 = new DevExpress.XtraRichEdit.UI.InsertTableOfContentsItem();
		this.updateTableOfContentsItem1 = new DevExpress.XtraRichEdit.UI.UpdateTableOfContentsItem();
		this.addParagraphsToTableOfContentItem1 = new DevExpress.XtraRichEdit.UI.AddParagraphsToTableOfContentItem();
		this.setParagraphHeadingLevelItem1 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem2 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem3 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem4 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem5 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem6 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem7 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem8 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem9 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.setParagraphHeadingLevelItem10 = new DevExpress.XtraRichEdit.UI.SetParagraphHeadingLevelItem();
		this.insertCaptionPlaceholderItem1 = new DevExpress.XtraRichEdit.UI.InsertCaptionPlaceholderItem();
		this.insertFiguresCaptionItems1 = new DevExpress.XtraRichEdit.UI.InsertFiguresCaptionItems();
		this.insertTablesCaptionItems1 = new DevExpress.XtraRichEdit.UI.InsertTablesCaptionItems();
		this.insertEquationsCaptionItems1 = new DevExpress.XtraRichEdit.UI.InsertEquationsCaptionItems();
		this.insertTableOfFiguresPlaceholderItem1 = new DevExpress.XtraRichEdit.UI.InsertTableOfFiguresPlaceholderItem();
		this.insertTableOfFiguresItems1 = new DevExpress.XtraRichEdit.UI.InsertTableOfFiguresItems();
		this.insertTableOfTablesItems1 = new DevExpress.XtraRichEdit.UI.InsertTableOfTablesItems();
		this.insertTableOfEquationsItems1 = new DevExpress.XtraRichEdit.UI.InsertTableOfEquationsItems();
		this.updateTableOfFiguresItem1 = new DevExpress.XtraRichEdit.UI.UpdateTableOfFiguresItem();
		this.changeFloatingObjectFillColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFloatingObjectFillColorItem();
		this.changeFloatingObjectOutlineColorItem1 = new DevExpress.XtraRichEdit.UI.ChangeFloatingObjectOutlineColorItem();
		this.changeFloatingObjectOutlineWeightItem1 = new DevExpress.XtraRichEdit.UI.ChangeFloatingObjectOutlineWeightItem();
		this.repositoryItemFloatingObjectOutlineWeight1 = new DevExpress.XtraRichEdit.Forms.Design.RepositoryItemFloatingObjectOutlineWeight();
		this.changeFloatingObjectTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.ChangeFloatingObjectTextWrapTypeItem();
		this.setFloatingObjectSquareTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectSquareTextWrapTypeItem();
		this.setFloatingObjectTightTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectTightTextWrapTypeItem();
		this.setFloatingObjectThroughTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectThroughTextWrapTypeItem();
		this.setFloatingObjectTopAndBottomTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectTopAndBottomTextWrapTypeItem();
		this.setFloatingObjectBehindTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectBehindTextWrapTypeItem();
		this.setFloatingObjectInFrontOfTextWrapTypeItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectInFrontOfTextWrapTypeItem();
		this.changeFloatingObjectAlignmentItem1 = new DevExpress.XtraRichEdit.UI.ChangeFloatingObjectAlignmentItem();
		this.setFloatingObjectTopLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectTopLeftAlignmentItem();
		this.setFloatingObjectTopCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectTopCenterAlignmentItem();
		this.setFloatingObjectTopRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectTopRightAlignmentItem();
		this.setFloatingObjectMiddleLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectMiddleLeftAlignmentItem();
		this.setFloatingObjectMiddleCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectMiddleCenterAlignmentItem();
		this.setFloatingObjectMiddleRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectMiddleRightAlignmentItem();
		this.setFloatingObjectBottomLeftAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectBottomLeftAlignmentItem();
		this.setFloatingObjectBottomCenterAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectBottomCenterAlignmentItem();
		this.setFloatingObjectBottomRightAlignmentItem1 = new DevExpress.XtraRichEdit.UI.SetFloatingObjectBottomRightAlignmentItem();
		this.floatingObjectBringForwardSubItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectBringForwardSubItem();
		this.floatingObjectBringForwardItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectBringForwardItem();
		this.floatingObjectBringToFrontItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectBringToFrontItem();
		this.floatingObjectBringInFrontOfTextItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectBringInFrontOfTextItem();
		this.floatingObjectSendBackwardSubItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectSendBackwardSubItem();
		this.floatingObjectSendBackwardItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectSendBackwardItem();
		this.floatingObjectSendToBackItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectSendToBackItem();
		this.floatingObjectSendBehindTextItem1 = new DevExpress.XtraRichEdit.UI.FloatingObjectSendBehindTextItem();
		this.commandBarCheckItem2 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem3 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem4 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarSubItem1 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem5 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem6 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem7 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem8 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem9 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.filterPopupButtonItem1 = new DevExpress.Snap.Extensions.UI.FilterPopupButtonItem();
		this.propertiesBarButtonItem1 = new DevExpress.Snap.Extensions.UI.PropertiesBarButtonItem();
		this.commandBarSubItem2 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem10 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem11 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarSubItem3 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem12 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem13 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarItem8 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem9 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarSubItem4 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem14 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem15 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem16 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem17 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem18 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem19 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem20 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarSubItem5 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem21 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem22 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarSubItem6 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem23 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem24 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarSubItem7 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarCheckItem25 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem26 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem27 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem28 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem29 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem30 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem31 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarItem10 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem11 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem12 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.barButtonGroup8 = new DevExpress.XtraBars.BarButtonGroup();
		this.changeEditorRowLimitItem1 = new DevExpress.Snap.Extensions.UI.ChangeEditorRowLimitItem();
		this.repositoryItemEditorRowLimitEdit1 = new DevExpress.Snap.Design.RepositoryItemEditorRowLimitEdit();
		this.createBarBaseItem1 = new DevExpress.XtraCharts.UI.CreateBarBaseItem();
		this.commandBarGalleryDropDown1 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.createLineBaseItem1 = new DevExpress.XtraCharts.UI.CreateLineBaseItem();
		this.commandBarGalleryDropDown2 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.createPieBaseItem1 = new DevExpress.XtraCharts.UI.CreatePieBaseItem();
		this.commandBarGalleryDropDown3 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.createRotatedBarBaseItem1 = new DevExpress.XtraCharts.UI.CreateRotatedBarBaseItem();
		this.commandBarGalleryDropDown4 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.createAreaBaseItem1 = new DevExpress.XtraCharts.UI.CreateAreaBaseItem();
		this.commandBarGalleryDropDown5 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.createOtherSeriesTypesBaseItem1 = new DevExpress.XtraCharts.UI.CreateOtherSeriesTypesBaseItem();
		this.commandBarGalleryDropDown6 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.changePaletteGalleryBaseItem1 = new DevExpress.XtraCharts.UI.ChangePaletteGalleryBaseItem();
		this.commandBarGalleryDropDown7 = new DevExpress.XtraBars.Commands.CommandBarGalleryDropDown(this.components);
		this.changeAppearanceGalleryBaseItem1 = new DevExpress.XtraCharts.UI.ChangeAppearanceGalleryBaseItem();
		this.runDesignerChartItem1 = new DevExpress.XtraCharts.UI.RunDesignerChartItem();
		this.mailMergeDataSourceItem1 = new DevExpress.Snap.Extensions.UI.MailMergeDataSourceItem();
		this.commandBarCheckItem32 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.commandBarCheckItem33 = new DevExpress.Snap.Extensions.UI.CommandBarCheckItem();
		this.mailMergeCurrentRecordItem1 = new DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordItem();
		this.repositoryItemMailMergeCurrentRecordEdit1 = new DevExpress.Snap.Extensions.UI.RepositoryItemMailMergeCurrentRecordEdit();
		this.commandBarSubItem8 = new DevExpress.Snap.Extensions.UI.CommandBarSubItem();
		this.commandBarItem13 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem14 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.commandBarItem15 = new DevExpress.Snap.Extensions.UI.CommandBarItem();
		this.headerFooterToolsRibbonPageCategory1 = new DevExpress.XtraRichEdit.UI.HeaderFooterToolsRibbonPageCategory();
		this.headerFooterToolsDesignRibbonPage1 = new DevExpress.XtraRichEdit.UI.HeaderFooterToolsDesignRibbonPage();
		this.headerFooterToolsDesignNavigationRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.HeaderFooterToolsDesignNavigationRibbonPageGroup();
		this.headerFooterToolsDesignOptionsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.HeaderFooterToolsDesignOptionsRibbonPageGroup();
		this.headerFooterToolsDesignCloseRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.HeaderFooterToolsDesignCloseRibbonPageGroup();
		this.tableToolsRibbonPageCategory1 = new DevExpress.XtraRichEdit.UI.TableToolsRibbonPageCategory();
		this.tableDesignRibbonPage1 = new DevExpress.XtraRichEdit.UI.TableDesignRibbonPage();
		this.tableStyleOptionsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableStyleOptionsRibbonPageGroup();
		this.tableStylesRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableStylesRibbonPageGroup();
		this.tableCellStylesRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.TableCellStylesRibbonPageGroup();
		this.tableDrawBordersRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableDrawBordersRibbonPageGroup();
		this.tableLayoutRibbonPage1 = new DevExpress.XtraRichEdit.UI.TableLayoutRibbonPage();
		this.tableTableRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableTableRibbonPageGroup();
		this.tableRowsAndColumnsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableRowsAndColumnsRibbonPageGroup();
		this.tableMergeRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableMergeRibbonPageGroup();
		this.tableCellSizeRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableCellSizeRibbonPageGroup();
		this.tableAlignmentRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableAlignmentRibbonPageGroup();
		this.floatingPictureToolsRibbonPageCategory1 = new DevExpress.XtraRichEdit.UI.FloatingPictureToolsRibbonPageCategory();
		this.floatingPictureToolsFormatPage1 = new DevExpress.XtraRichEdit.UI.FloatingPictureToolsFormatPage();
		this.floatingPictureToolsShapeStylesPageGroup1 = new DevExpress.XtraRichEdit.UI.FloatingPictureToolsShapeStylesPageGroup();
		this.floatingPictureToolsArrangePageGroup1 = new DevExpress.XtraRichEdit.UI.FloatingPictureToolsArrangePageGroup();
		this.dataToolsRibbonPageCategory1 = new DevExpress.Snap.Extensions.UI.DataToolsRibbonPageCategory();
		this.appearanceRibbonPage1 = new DevExpress.Snap.Extensions.UI.AppearanceRibbonPage();
		this.snMergeFieldToolsRibbonPage1 = new DevExpress.Snap.Extensions.UI.SNMergeFieldToolsRibbonPage();
		this.dataShapingRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.DataShapingRibbonPageGroup();
		this.snMergeFieldPropertiesRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.SNMergeFieldPropertiesRibbonPageGroup();
		this.groupToolsRibbonPage1 = new DevExpress.Snap.Extensions.UI.GroupToolsRibbonPage();
		this.groupingRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.GroupingRibbonPageGroup();
		this.listToolsRibbonPage1 = new DevExpress.Snap.Extensions.UI.ListToolsRibbonPage();
		this.listHeaderAndFooterRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ListHeaderAndFooterRibbonPageGroup();
		this.listCommandsRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ListCommandsRibbonPageGroup();
		this.listEditorRowLimitRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ListEditorRowLimitRibbonPageGroup();
		this.mailMergeRibbonPage1 = new DevExpress.Snap.Extensions.UI.MailMergeRibbonPage();
		this.mailMergeRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.MailMergeRibbonPageGroup();
		this.mailMergeCurrentRecordRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordRibbonPageGroup();
		this.finishAndMergeRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.FinishAndMergeRibbonPageGroup();
		this.chartRibbonPageCategory1 = new DevExpress.XtraCharts.UI.ChartRibbonPageCategory();
		this.createChartRibbonPage1 = new DevExpress.XtraCharts.UI.CreateChartRibbonPage();
		this.chartTypeRibbonPageGroup1 = new DevExpress.XtraCharts.UI.ChartTypeRibbonPageGroup();
		this.chartAppearanceRibbonPageGroup1 = new DevExpress.XtraCharts.UI.ChartAppearanceRibbonPageGroup();
		this.chartWizardRibbonPageGroup1 = new DevExpress.XtraCharts.UI.ChartWizardRibbonPageGroup();
		this.fileRibbonPage1 = new DevExpress.XtraRichEdit.UI.FileRibbonPage();
		this.commonRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.CommonRibbonPageGroup();
		this.infoRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.InfoRibbonPageGroup();
		this.dataRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.DataRibbonPageGroup();
		this.homeRibbonPage1 = new DevExpress.XtraRichEdit.UI.HomeRibbonPage();
		this.clipboardRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.ClipboardRibbonPageGroup();
		this.fontRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.FontRibbonPageGroup();
		this.paragraphRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.ParagraphRibbonPageGroup();
		this.editingRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.EditingRibbonPageGroup();
		this.insertRibbonPage1 = new DevExpress.XtraRichEdit.UI.InsertRibbonPage();
		this.pagesRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.PagesRibbonPageGroup();
		this.tablesRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TablesRibbonPageGroup();
		this.illustrationsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.IllustrationsRibbonPageGroup();
		this.toolboxRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ToolboxRibbonPageGroup();
		this.linksRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.LinksRibbonPageGroup();
		this.headerFooterRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.HeaderFooterRibbonPageGroup();
		this.textRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TextRibbonPageGroup();
		this.symbolsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.SymbolsRibbonPageGroup();
		this.pageLayoutRibbonPage1 = new DevExpress.XtraRichEdit.UI.PageLayoutRibbonPage();
		this.pageSetupRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.PageSetupRibbonPageGroup();
		this.pageBackgroundRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.PageBackgroundRibbonPageGroup();
		this.viewRibbonPage1 = new DevExpress.XtraRichEdit.UI.ViewRibbonPage();
		this.documentViewsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.DocumentViewsRibbonPageGroup();
		this.showRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.ShowRibbonPageGroup();
		this.zoomRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.ZoomRibbonPageGroup();
		this.viewRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ViewRibbonPageGroup();
		this.viewFieldsRibbonPageGroup1 = new DevExpress.Snap.Extensions.UI.ViewFieldsRibbonPageGroup();
		this.reviewRibbonPage1 = new DevExpress.XtraRichEdit.UI.ReviewRibbonPage();
		this.documentProofingRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.DocumentProofingRibbonPageGroup();
		this.referencesRibbonPage1 = new DevExpress.XtraRichEdit.UI.ReferencesRibbonPage();
		this.tableOfContentsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.TableOfContentsRibbonPageGroup();
		this.captionsRibbonPageGroup1 = new DevExpress.XtraRichEdit.UI.CaptionsRibbonPageGroup();
		this.snapDockManager1 = new DevExpress.Snap.Extensions.SnapDockManager(this.components);
		this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
		this.fieldListDockPanel1 = new DevExpress.Snap.Extensions.UI.FieldListDockPanel();
		this.fieldListDockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.reportExplorerDockPanel1 = new DevExpress.Snap.Extensions.UI.ReportExplorerDockPanel();
		this.reportExplorerDockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
		this.snapDocumentManager1 = new DevExpress.Snap.Extensions.SnapDocumentManager(this.components);
		this.noDocumentsView1 = new DevExpress.XtraBars.Docking2010.Views.NoDocuments.NoDocumentsView(this.components);
		this.snapBarController1 = new DevExpress.Snap.Extensions.SnapBarController();
		this.fileSaveItem1_Custom = new DevExpress.XtraBars.BarButtonItem();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditFontSizeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineStyle1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineWeight1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFloatingObjectOutlineWeight1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemEditorRowLimitEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMailMergeCurrentRecordEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snapDockManager1).BeginInit();
		this.panelContainer1.SuspendLayout();
		this.fieldListDockPanel1.SuspendLayout();
		this.reportExplorerDockPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.snapDocumentManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snapBarController1).BeginInit();
		base.SuspendLayout();
		this.themesRibbonPageGroup1.Glyph = (System.Drawing.Image)resources.GetObject("themesRibbonPageGroup1.Glyph");
		this.themesRibbonPageGroup1.ItemLinks.Add(this.themesGalleryBarItem1);
		this.themesRibbonPageGroup1.Name = "themesRibbonPageGroup1";
		this.themesGalleryBarItem1.Gallery.ColumnCount = 7;
		this.themesGalleryBarItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.themesGalleryBarItem1.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleCheck;
		this.themesGalleryBarItem1.Gallery.RowCount = 1;
		this.themesGalleryBarItem1.Id = 240;
		this.themesGalleryBarItem1.Name = "themesGalleryBarItem1";
		this.stylesRibbonPageGroup1.Glyph = (System.Drawing.Image)resources.GetObject("stylesRibbonPageGroup1.Glyph");
		this.stylesRibbonPageGroup1.ItemLinks.Add(this.galleryChangeStyleItem1);
		this.stylesRibbonPageGroup1.Name = "stylesRibbonPageGroup1";
		this.galleryChangeStyleItem1.Gallery.ColumnCount = 10;
		this.galleryChangeStyleItem1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup });
		this.galleryChangeStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChangeStyleItem1.Id = 64;
		this.galleryChangeStyleItem1.Name = "galleryChangeStyleItem1";
		this.snapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.snapControl1.Location = new System.Drawing.Point(0, 141);
		this.snapControl1.MenuManager = this.ribbonControl1;
		this.snapControl1.Name = "snapControl1";
		this.snapControl1.Options.SnapMailMergeVisualOptions.DataSourceName = null;
		this.snapControl1.Size = new System.Drawing.Size(793, 549);
		this.snapControl1.TabIndex = 0;
		this.snapControl1.Text = "snapControl1";
		this.ribbonControl1.ExpandCollapseItem.Id = 0;
		this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[305]
		{
			this.ribbonControl1.ExpandCollapseItem,
			this.undoItem1,
			this.redoItem1,
			this.fileNewItem1,
			this.fileOpenItem1,
			this.fileSaveItem1,
			this.fileSaveAsItem1,
			this.commandBarItem1,
			this.quickPrintItem1,
			this.printItem1,
			this.printPreviewItem1,
			this.showDocumentPropertiesFormItem1,
			this.commandBarItem2,
			this.pasteItem1,
			this.cutItem1,
			this.copyItem1,
			this.pasteSpecialItem1,
			this.barButtonGroup1,
			this.changeFontNameItem1,
			this.changeFontSizeItem1,
			this.fontSizeIncreaseItem1,
			this.fontSizeDecreaseItem1,
			this.barButtonGroup2,
			this.toggleFontBoldItem1,
			this.toggleFontItalicItem1,
			this.toggleFontUnderlineItem1,
			this.toggleFontDoubleUnderlineItem1,
			this.toggleFontStrikeoutItem1,
			this.toggleFontDoubleStrikeoutItem1,
			this.toggleFontSuperscriptItem1,
			this.toggleFontSubscriptItem1,
			this.barButtonGroup3,
			this.changeFontColorItem1,
			this.changeFontHighlightColorItem1,
			this.changeTextCaseItem1,
			this.makeTextUpperCaseItem1,
			this.makeTextLowerCaseItem1,
			this.capitalizeEachWordCaseItem1,
			this.toggleTextCaseItem1,
			this.clearFormattingItem1,
			this.barButtonGroup4,
			this.toggleBulletedListItem1,
			this.toggleNumberingListItem1,
			this.toggleMultiLevelListItem1,
			this.barButtonGroup5,
			this.decreaseIndentItem1,
			this.increaseIndentItem1,
			this.barButtonGroup6,
			this.toggleParagraphAlignmentLeftItem1,
			this.toggleParagraphAlignmentCenterItem1,
			this.toggleParagraphAlignmentRightItem1,
			this.toggleParagraphAlignmentJustifyItem1,
			this.toggleShowWhitespaceItem1,
			this.barButtonGroup7,
			this.changeParagraphLineSpacingItem1,
			this.setSingleParagraphSpacingItem1,
			this.setSesquialteralParagraphSpacingItem1,
			this.setDoubleParagraphSpacingItem1,
			this.showLineSpacingFormItem1,
			this.addSpacingBeforeParagraphItem1,
			this.removeSpacingBeforeParagraphItem1,
			this.addSpacingAfterParagraphItem1,
			this.removeSpacingAfterParagraphItem1,
			this.changeParagraphBackColorItem1,
			this.galleryChangeStyleItem1,
			this.findItem1,
			this.replaceItem1,
			this.insertPageBreakItem21,
			this.insertTableItem1,
			this.insertPictureItem1,
			this.insertFloatingPictureItem1,
			this.commandBarItem3,
			this.commandBarItem4,
			this.commandBarItem5,
			this.commandBarItem6,
			this.commandBarItem7,
			this.insertBookmarkItem1,
			this.insertHyperlinkItem1,
			this.editPageHeaderItem1,
			this.editPageFooterItem1,
			this.insertPageNumberItem1,
			this.insertPageCountItem1,
			this.insertTextBoxItem1,
			this.insertSymbolItem1,
			this.changeSectionPageMarginsItem1,
			this.setNormalSectionPageMarginsItem1,
			this.setNarrowSectionPageMarginsItem1,
			this.setModerateSectionPageMarginsItem1,
			this.setWideSectionPageMarginsItem1,
			this.showPageMarginsSetupFormItem1,
			this.changeSectionPageOrientationItem1,
			this.setPortraitPageOrientationItem1,
			this.setLandscapePageOrientationItem1,
			this.changeSectionPaperKindItem1,
			this.changeSectionColumnsItem1,
			this.setSectionOneColumnItem1,
			this.setSectionTwoColumnsItem1,
			this.setSectionThreeColumnsItem1,
			this.showColumnsSetupFormItem1,
			this.insertBreakItem1,
			this.insertPageBreakItem1,
			this.insertColumnBreakItem1,
			this.insertSectionBreakNextPageItem1,
			this.insertSectionBreakEvenPageItem1,
			this.insertSectionBreakOddPageItem1,
			this.changeSectionLineNumberingItem1,
			this.setSectionLineNumberingNoneItem1,
			this.setSectionLineNumberingContinuousItem1,
			this.setSectionLineNumberingRestartNewPageItem1,
			this.setSectionLineNumberingRestartNewSectionItem1,
			this.toggleParagraphSuppressLineNumbersItem1,
			this.showLineNumberingFormItem1,
			this.changePageColorItem1,
			this.switchToSimpleViewItem1,
			this.switchToDraftViewItem1,
			this.switchToPrintLayoutViewItem1,
			this.toggleShowHorizontalRulerItem1,
			this.toggleShowVerticalRulerItem1,
			this.zoomOutItem1,
			this.zoomInItem1,
			this.snapBarToolbarsListItem1,
			this.commandBarCheckItem1,
			this.showAllFieldCodesItem1,
			this.showAllFieldResultsItem1,
			this.toggleFieldHighlightingItem1,
			this.goToPageHeaderItem1,
			this.goToPageFooterItem1,
			this.goToNextHeaderFooterItem1,
			this.goToPreviousHeaderFooterItem1,
			this.toggleLinkToPreviousItem1,
			this.toggleDifferentFirstPageItem1,
			this.toggleDifferentOddAndEvenPagesItem1,
			this.closePageHeaderFooterItem1,
			this.toggleFirstRowItem1,
			this.toggleLastRowItem1,
			this.toggleBandedRowsItem1,
			this.toggleFirstColumnItem1,
			this.toggleLastColumnItem1,
			this.toggleBandedColumnsItem1,
			this.galleryChangeTableStyleItem1,
			this.galleryChangeTableCellStyleItem1,
			this.changeTableBorderLineStyleItem1,
			this.changeTableBorderLineWeightItem1,
			this.changeTableBorderColorItem1,
			this.changeTableBordersItem1,
			this.toggleTableCellsBottomBorderItem1,
			this.toggleTableCellsTopBorderItem1,
			this.toggleTableCellsLeftBorderItem1,
			this.toggleTableCellsRightBorderItem1,
			this.resetTableCellsAllBordersItem1,
			this.toggleTableCellsAllBordersItem1,
			this.toggleTableCellsOutsideBorderItem1,
			this.toggleTableCellsInsideBorderItem1,
			this.toggleTableCellsInsideHorizontalBorderItem1,
			this.toggleTableCellsInsideVerticalBorderItem1,
			this.toggleShowTableGridLinesItem1,
			this.changeTableCellsShadingItem1,
			this.selectTableElementsItem1,
			this.selectTableCellItem1,
			this.selectTableColumnItem1,
			this.selectTableRowItem1,
			this.selectTableItem1,
			this.showTablePropertiesFormItem1,
			this.deleteTableElementsItem1,
			this.showDeleteTableCellsFormItem1,
			this.deleteTableColumnsItem1,
			this.deleteTableRowsItem1,
			this.deleteTableItem1,
			this.insertTableRowAboveItem1,
			this.insertTableRowBelowItem1,
			this.insertTableColumnToLeftItem1,
			this.insertTableColumnToRightItem1,
			this.mergeTableCellsItem1,
			this.showSplitTableCellsForm1,
			this.splitTableItem1,
			this.toggleTableAutoFitItem1,
			this.toggleTableAutoFitContentsItem1,
			this.toggleTableAutoFitWindowItem1,
			this.toggleTableFixedColumnWidthItem1,
			this.toggleTableCellsTopLeftAlignmentItem1,
			this.toggleTableCellsMiddleLeftAlignmentItem1,
			this.toggleTableCellsBottomLeftAlignmentItem1,
			this.toggleTableCellsTopCenterAlignmentItem1,
			this.toggleTableCellsMiddleCenterAlignmentItem1,
			this.toggleTableCellsBottomCenterAlignmentItem1,
			this.toggleTableCellsTopRightAlignmentItem1,
			this.toggleTableCellsMiddleRightAlignmentItem1,
			this.toggleTableCellsBottomRightAlignmentItem1,
			this.showTableOptionsFormItem1,
			this.checkSpellingItem1,
			this.insertTableOfContentsItem1,
			this.updateTableOfContentsItem1,
			this.addParagraphsToTableOfContentItem1,
			this.setParagraphHeadingLevelItem1,
			this.setParagraphHeadingLevelItem2,
			this.setParagraphHeadingLevelItem3,
			this.setParagraphHeadingLevelItem4,
			this.setParagraphHeadingLevelItem5,
			this.setParagraphHeadingLevelItem6,
			this.setParagraphHeadingLevelItem7,
			this.setParagraphHeadingLevelItem8,
			this.setParagraphHeadingLevelItem9,
			this.setParagraphHeadingLevelItem10,
			this.insertCaptionPlaceholderItem1,
			this.insertFiguresCaptionItems1,
			this.insertTablesCaptionItems1,
			this.insertEquationsCaptionItems1,
			this.insertTableOfFiguresPlaceholderItem1,
			this.insertTableOfFiguresItems1,
			this.insertTableOfTablesItems1,
			this.insertTableOfEquationsItems1,
			this.updateTableOfFiguresItem1,
			this.changeFloatingObjectFillColorItem1,
			this.changeFloatingObjectOutlineColorItem1,
			this.changeFloatingObjectOutlineWeightItem1,
			this.changeFloatingObjectTextWrapTypeItem1,
			this.setFloatingObjectSquareTextWrapTypeItem1,
			this.setFloatingObjectTightTextWrapTypeItem1,
			this.setFloatingObjectThroughTextWrapTypeItem1,
			this.setFloatingObjectTopAndBottomTextWrapTypeItem1,
			this.setFloatingObjectBehindTextWrapTypeItem1,
			this.setFloatingObjectInFrontOfTextWrapTypeItem1,
			this.changeFloatingObjectAlignmentItem1,
			this.setFloatingObjectTopLeftAlignmentItem1,
			this.setFloatingObjectTopCenterAlignmentItem1,
			this.setFloatingObjectTopRightAlignmentItem1,
			this.setFloatingObjectMiddleLeftAlignmentItem1,
			this.setFloatingObjectMiddleCenterAlignmentItem1,
			this.setFloatingObjectMiddleRightAlignmentItem1,
			this.setFloatingObjectBottomLeftAlignmentItem1,
			this.setFloatingObjectBottomCenterAlignmentItem1,
			this.setFloatingObjectBottomRightAlignmentItem1,
			this.floatingObjectBringForwardSubItem1,
			this.floatingObjectBringForwardItem1,
			this.floatingObjectBringToFrontItem1,
			this.floatingObjectBringInFrontOfTextItem1,
			this.floatingObjectSendBackwardSubItem1,
			this.floatingObjectSendBackwardItem1,
			this.floatingObjectSendToBackItem1,
			this.floatingObjectSendBehindTextItem1,
			this.themesGalleryBarItem1,
			this.commandBarCheckItem2,
			this.commandBarCheckItem3,
			this.commandBarCheckItem4,
			this.commandBarSubItem1,
			this.commandBarCheckItem5,
			this.commandBarCheckItem6,
			this.commandBarCheckItem7,
			this.commandBarCheckItem8,
			this.commandBarCheckItem9,
			this.filterPopupButtonItem1,
			this.propertiesBarButtonItem1,
			this.commandBarSubItem2,
			this.commandBarCheckItem10,
			this.commandBarCheckItem11,
			this.commandBarSubItem3,
			this.commandBarCheckItem12,
			this.commandBarCheckItem13,
			this.commandBarItem8,
			this.commandBarItem9,
			this.commandBarSubItem4,
			this.commandBarCheckItem14,
			this.commandBarCheckItem15,
			this.commandBarCheckItem16,
			this.commandBarCheckItem17,
			this.commandBarCheckItem18,
			this.commandBarCheckItem19,
			this.commandBarCheckItem20,
			this.commandBarSubItem5,
			this.commandBarCheckItem21,
			this.commandBarCheckItem22,
			this.commandBarSubItem6,
			this.commandBarCheckItem23,
			this.commandBarCheckItem24,
			this.commandBarSubItem7,
			this.commandBarCheckItem25,
			this.commandBarCheckItem26,
			this.commandBarCheckItem27,
			this.commandBarCheckItem28,
			this.commandBarCheckItem29,
			this.commandBarCheckItem30,
			this.commandBarCheckItem31,
			this.commandBarItem10,
			this.commandBarItem11,
			this.commandBarItem12,
			this.barButtonGroup8,
			this.changeEditorRowLimitItem1,
			this.createBarBaseItem1,
			this.createLineBaseItem1,
			this.createPieBaseItem1,
			this.createRotatedBarBaseItem1,
			this.createAreaBaseItem1,
			this.createOtherSeriesTypesBaseItem1,
			this.changePaletteGalleryBaseItem1,
			this.changeAppearanceGalleryBaseItem1,
			this.runDesignerChartItem1,
			this.mailMergeDataSourceItem1,
			this.commandBarCheckItem32,
			this.commandBarCheckItem33,
			this.mailMergeCurrentRecordItem1,
			this.commandBarSubItem8,
			this.commandBarItem13,
			this.commandBarItem14,
			this.commandBarItem15,
			this.fileSaveItem1_Custom
		});
		this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl1.MaxItemId = 305;
		this.ribbonControl1.Name = "ribbonControl1";
		this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[5] { this.headerFooterToolsRibbonPageCategory1, this.tableToolsRibbonPageCategory1, this.floatingPictureToolsRibbonPageCategory1, this.dataToolsRibbonPageCategory1, this.chartRibbonPageCategory1 });
		this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[7] { this.fileRibbonPage1, this.homeRibbonPage1, this.insertRibbonPage1, this.pageLayoutRibbonPage1, this.viewRibbonPage1, this.reviewRibbonPage1, this.referencesRibbonPage1 });
		this.ribbonControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.repositoryItemFontEdit1, this.repositoryItemRichEditFontSizeEdit1, this.repositoryItemBorderLineStyle1, this.repositoryItemBorderLineWeight1, this.repositoryItemFloatingObjectOutlineWeight1, this.repositoryItemEditorRowLimitEdit1, this.repositoryItemMailMergeCurrentRecordEdit1 });
		this.ribbonControl1.Size = new System.Drawing.Size(993, 141);
		this.undoItem1.Id = 1;
		this.undoItem1.Name = "undoItem1";
		this.redoItem1.Id = 2;
		this.redoItem1.Name = "redoItem1";
		this.fileNewItem1.Id = 3;
		this.fileNewItem1.Name = "fileNewItem1";
		this.fileOpenItem1.Id = 4;
		this.fileOpenItem1.Name = "fileOpenItem1";
		this.fileSaveItem1.Id = 5;
		this.fileSaveItem1.Name = "fileSaveItem1";
		this.fileSaveAsItem1.Id = 6;
		this.fileSaveAsItem1.Name = "fileSaveAsItem1";
		this.commandBarItem1.Id = 7;
		this.commandBarItem1.Name = "commandBarItem1";
		this.commandBarItem1.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.ExportDocument;
		this.quickPrintItem1.Id = 8;
		this.quickPrintItem1.Name = "quickPrintItem1";
		this.printItem1.Id = 9;
		this.printItem1.Name = "printItem1";
		this.printPreviewItem1.Id = 10;
		this.printPreviewItem1.Name = "printPreviewItem1";
		this.showDocumentPropertiesFormItem1.Id = 11;
		this.showDocumentPropertiesFormItem1.Name = "showDocumentPropertiesFormItem1";
		this.commandBarItem2.Id = 12;
		this.commandBarItem2.Name = "commandBarItem2";
		this.commandBarItem2.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.NewDataSource;
		this.pasteItem1.Id = 20;
		this.pasteItem1.Name = "pasteItem1";
		this.cutItem1.Id = 21;
		this.cutItem1.Name = "cutItem1";
		this.copyItem1.Id = 22;
		this.copyItem1.Name = "copyItem1";
		this.pasteSpecialItem1.Id = 23;
		this.pasteSpecialItem1.Name = "pasteSpecialItem1";
		this.barButtonGroup1.Id = 13;
		this.barButtonGroup1.ItemLinks.Add(this.changeFontNameItem1, "FF");
		this.barButtonGroup1.ItemLinks.Add(this.changeFontSizeItem1);
		this.barButtonGroup1.ItemLinks.Add(this.fontSizeIncreaseItem1, "FG");
		this.barButtonGroup1.ItemLinks.Add(this.fontSizeDecreaseItem1, "FK");
		this.barButtonGroup1.Name = "barButtonGroup1";
		this.barButtonGroup1.Tag = "{97BBE334-159B-44d9-A168-0411957565E8}";
		this.changeFontNameItem1.Edit = this.repositoryItemFontEdit1;
		this.changeFontNameItem1.Id = 24;
		this.changeFontNameItem1.Name = "changeFontNameItem1";
		this.repositoryItemFontEdit1.AutoHeight = false;
		this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
		this.changeFontSizeItem1.Edit = this.repositoryItemRichEditFontSizeEdit1;
		this.changeFontSizeItem1.Id = 25;
		this.changeFontSizeItem1.Name = "changeFontSizeItem1";
		this.repositoryItemRichEditFontSizeEdit1.AutoHeight = false;
		this.repositoryItemRichEditFontSizeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemRichEditFontSizeEdit1.Control = this.snapControl1;
		this.repositoryItemRichEditFontSizeEdit1.Name = "repositoryItemRichEditFontSizeEdit1";
		this.fontSizeIncreaseItem1.Id = 26;
		this.fontSizeIncreaseItem1.Name = "fontSizeIncreaseItem1";
		this.fontSizeDecreaseItem1.Id = 27;
		this.fontSizeDecreaseItem1.Name = "fontSizeDecreaseItem1";
		this.barButtonGroup2.Id = 14;
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontBoldItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontItalicItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontUnderlineItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontDoubleUnderlineItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontStrikeoutItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontDoubleStrikeoutItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontSuperscriptItem1);
		this.barButtonGroup2.ItemLinks.Add(this.toggleFontSubscriptItem1);
		this.barButtonGroup2.Name = "barButtonGroup2";
		this.barButtonGroup2.Tag = "{433DA7F0-03E2-4650-9DB5-66DD92D16E39}";
		this.toggleFontBoldItem1.Id = 28;
		this.toggleFontBoldItem1.Name = "toggleFontBoldItem1";
		this.toggleFontItalicItem1.Id = 29;
		this.toggleFontItalicItem1.Name = "toggleFontItalicItem1";
		this.toggleFontUnderlineItem1.Id = 30;
		this.toggleFontUnderlineItem1.Name = "toggleFontUnderlineItem1";
		this.toggleFontDoubleUnderlineItem1.Id = 31;
		this.toggleFontDoubleUnderlineItem1.Name = "toggleFontDoubleUnderlineItem1";
		this.toggleFontStrikeoutItem1.Id = 32;
		this.toggleFontStrikeoutItem1.Name = "toggleFontStrikeoutItem1";
		this.toggleFontDoubleStrikeoutItem1.Id = 33;
		this.toggleFontDoubleStrikeoutItem1.Name = "toggleFontDoubleStrikeoutItem1";
		this.toggleFontSuperscriptItem1.Id = 34;
		this.toggleFontSuperscriptItem1.Name = "toggleFontSuperscriptItem1";
		this.toggleFontSubscriptItem1.Id = 35;
		this.toggleFontSubscriptItem1.Name = "toggleFontSubscriptItem1";
		this.barButtonGroup3.Id = 15;
		this.barButtonGroup3.ItemLinks.Add(this.changeFontColorItem1, "FC");
		this.barButtonGroup3.ItemLinks.Add(this.changeFontHighlightColorItem1, "I");
		this.barButtonGroup3.Name = "barButtonGroup3";
		this.barButtonGroup3.Tag = "{DF8C5334-EDE3-47c9-A42C-FE9A9247E180}";
		this.changeFontColorItem1.Id = 36;
		this.changeFontColorItem1.Name = "changeFontColorItem1";
		this.changeFontHighlightColorItem1.Id = 37;
		this.changeFontHighlightColorItem1.Name = "changeFontHighlightColorItem1";
		this.changeTextCaseItem1.Id = 38;
		this.changeTextCaseItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.makeTextUpperCaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.makeTextLowerCaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.capitalizeEachWordCaseItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTextCaseItem1)
		});
		this.changeTextCaseItem1.Name = "changeTextCaseItem1";
		this.makeTextUpperCaseItem1.Id = 39;
		this.makeTextUpperCaseItem1.Name = "makeTextUpperCaseItem1";
		this.makeTextLowerCaseItem1.Id = 40;
		this.makeTextLowerCaseItem1.Name = "makeTextLowerCaseItem1";
		this.capitalizeEachWordCaseItem1.Id = 41;
		this.capitalizeEachWordCaseItem1.Name = "capitalizeEachWordCaseItem1";
		this.toggleTextCaseItem1.Id = 42;
		this.toggleTextCaseItem1.Name = "toggleTextCaseItem1";
		this.clearFormattingItem1.Id = 43;
		this.clearFormattingItem1.Name = "clearFormattingItem1";
		this.barButtonGroup4.Id = 16;
		this.barButtonGroup4.ItemLinks.Add(this.toggleBulletedListItem1, "U");
		this.barButtonGroup4.ItemLinks.Add(this.toggleNumberingListItem1, "N");
		this.barButtonGroup4.ItemLinks.Add(this.toggleMultiLevelListItem1, "M");
		this.barButtonGroup4.Name = "barButtonGroup4";
		this.barButtonGroup4.Tag = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
		this.toggleBulletedListItem1.Id = 44;
		this.toggleBulletedListItem1.Name = "toggleBulletedListItem1";
		this.toggleNumberingListItem1.Id = 45;
		this.toggleNumberingListItem1.Name = "toggleNumberingListItem1";
		this.toggleMultiLevelListItem1.Id = 46;
		this.toggleMultiLevelListItem1.Name = "toggleMultiLevelListItem1";
		this.barButtonGroup5.Id = 17;
		this.barButtonGroup5.ItemLinks.Add(this.decreaseIndentItem1, "AO");
		this.barButtonGroup5.ItemLinks.Add(this.increaseIndentItem1, "AI");
		this.barButtonGroup5.ItemLinks.Add(this.toggleShowWhitespaceItem1);
		this.barButtonGroup5.Name = "barButtonGroup5";
		this.barButtonGroup5.Tag = "{4747D5AB-2BEB-4ea6-9A1D-8E4FB36F1B40}";
		this.decreaseIndentItem1.Id = 47;
		this.decreaseIndentItem1.Name = "decreaseIndentItem1";
		this.increaseIndentItem1.Id = 48;
		this.increaseIndentItem1.Name = "increaseIndentItem1";
		this.toggleShowWhitespaceItem1.Id = 53;
		this.toggleShowWhitespaceItem1.Name = "toggleShowWhitespaceItem1";
		this.barButtonGroup6.Id = 18;
		this.barButtonGroup6.ItemLinks.Add(this.toggleParagraphAlignmentLeftItem1, "AL");
		this.barButtonGroup6.ItemLinks.Add(this.toggleParagraphAlignmentCenterItem1, "AC");
		this.barButtonGroup6.ItemLinks.Add(this.toggleParagraphAlignmentRightItem1, "AR");
		this.barButtonGroup6.ItemLinks.Add(this.toggleParagraphAlignmentJustifyItem1, "AJ");
		this.barButtonGroup6.Name = "barButtonGroup6";
		this.barButtonGroup6.Tag = "{8E89E775-996E-49a0-AADA-DE338E34732E}";
		this.toggleParagraphAlignmentLeftItem1.Id = 49;
		this.toggleParagraphAlignmentLeftItem1.Name = "toggleParagraphAlignmentLeftItem1";
		this.toggleParagraphAlignmentCenterItem1.Id = 50;
		this.toggleParagraphAlignmentCenterItem1.Name = "toggleParagraphAlignmentCenterItem1";
		this.toggleParagraphAlignmentRightItem1.Id = 51;
		this.toggleParagraphAlignmentRightItem1.Name = "toggleParagraphAlignmentRightItem1";
		this.toggleParagraphAlignmentJustifyItem1.Id = 52;
		this.toggleParagraphAlignmentJustifyItem1.Name = "toggleParagraphAlignmentJustifyItem1";
		this.barButtonGroup7.Id = 19;
		this.barButtonGroup7.ItemLinks.Add(this.changeParagraphLineSpacingItem1, "K");
		this.barButtonGroup7.ItemLinks.Add(this.changeParagraphBackColorItem1, "H");
		this.barButtonGroup7.Name = "barButtonGroup7";
		this.barButtonGroup7.Tag = "{9A8DEAD8-3890-4857-A395-EC625FD02217}";
		this.changeParagraphLineSpacingItem1.Id = 54;
		this.changeParagraphLineSpacingItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setSingleParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSesquialteralParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setDoubleParagraphSpacingItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showLineSpacingFormItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingBeforeParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingBeforeParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.addSpacingAfterParagraphItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeSpacingAfterParagraphItem1)
		});
		this.changeParagraphLineSpacingItem1.Name = "changeParagraphLineSpacingItem1";
		this.setSingleParagraphSpacingItem1.Id = 55;
		this.setSingleParagraphSpacingItem1.Name = "setSingleParagraphSpacingItem1";
		this.setSesquialteralParagraphSpacingItem1.Id = 56;
		this.setSesquialteralParagraphSpacingItem1.Name = "setSesquialteralParagraphSpacingItem1";
		this.setDoubleParagraphSpacingItem1.Id = 57;
		this.setDoubleParagraphSpacingItem1.Name = "setDoubleParagraphSpacingItem1";
		this.showLineSpacingFormItem1.Id = 58;
		this.showLineSpacingFormItem1.Name = "showLineSpacingFormItem1";
		this.addSpacingBeforeParagraphItem1.Id = 59;
		this.addSpacingBeforeParagraphItem1.Name = "addSpacingBeforeParagraphItem1";
		this.removeSpacingBeforeParagraphItem1.Id = 60;
		this.removeSpacingBeforeParagraphItem1.Name = "removeSpacingBeforeParagraphItem1";
		this.addSpacingAfterParagraphItem1.Id = 61;
		this.addSpacingAfterParagraphItem1.Name = "addSpacingAfterParagraphItem1";
		this.removeSpacingAfterParagraphItem1.Id = 62;
		this.removeSpacingAfterParagraphItem1.Name = "removeSpacingAfterParagraphItem1";
		this.changeParagraphBackColorItem1.Id = 63;
		this.changeParagraphBackColorItem1.Name = "changeParagraphBackColorItem1";
		this.findItem1.Id = 65;
		this.findItem1.Name = "findItem1";
		this.replaceItem1.Id = 66;
		this.replaceItem1.Name = "replaceItem1";
		this.insertPageBreakItem21.Id = 67;
		this.insertPageBreakItem21.Name = "insertPageBreakItem21";
		this.insertTableItem1.Id = 68;
		this.insertTableItem1.Name = "insertTableItem1";
		this.insertPictureItem1.Id = 69;
		this.insertPictureItem1.Name = "insertPictureItem1";
		this.insertFloatingPictureItem1.Id = 70;
		this.insertFloatingPictureItem1.Name = "insertFloatingPictureItem1";
		this.commandBarItem3.Id = 71;
		this.commandBarItem3.Name = "commandBarItem3";
		this.commandBarItem3.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertBarCode;
		this.commandBarItem4.Id = 72;
		this.commandBarItem4.Name = "commandBarItem4";
		this.commandBarItem4.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertCheckBox;
		this.commandBarItem5.Id = 73;
		this.commandBarItem5.Name = "commandBarItem5";
		this.commandBarItem5.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertChart;
		this.commandBarItem6.Id = 74;
		this.commandBarItem6.Name = "commandBarItem6";
		this.commandBarItem6.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSparkline;
		this.commandBarItem7.Id = 75;
		this.commandBarItem7.Name = "commandBarItem7";
		this.commandBarItem7.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertIndex;
		this.insertBookmarkItem1.Id = 76;
		this.insertBookmarkItem1.Name = "insertBookmarkItem1";
		this.insertHyperlinkItem1.Id = 77;
		this.insertHyperlinkItem1.Name = "insertHyperlinkItem1";
		this.editPageHeaderItem1.Id = 78;
		this.editPageHeaderItem1.Name = "editPageHeaderItem1";
		this.editPageFooterItem1.Id = 79;
		this.editPageFooterItem1.Name = "editPageFooterItem1";
		this.insertPageNumberItem1.Id = 80;
		this.insertPageNumberItem1.Name = "insertPageNumberItem1";
		this.insertPageCountItem1.Id = 81;
		this.insertPageCountItem1.Name = "insertPageCountItem1";
		this.insertTextBoxItem1.Id = 82;
		this.insertTextBoxItem1.Name = "insertTextBoxItem1";
		this.insertSymbolItem1.Id = 83;
		this.insertSymbolItem1.Name = "insertSymbolItem1";
		this.changeSectionPageMarginsItem1.Id = 84;
		this.changeSectionPageMarginsItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setNormalSectionPageMarginsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setNarrowSectionPageMarginsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setModerateSectionPageMarginsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setWideSectionPageMarginsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showPageMarginsSetupFormItem1, true)
		});
		this.changeSectionPageMarginsItem1.Name = "changeSectionPageMarginsItem1";
		this.setNormalSectionPageMarginsItem1.Id = 85;
		this.setNormalSectionPageMarginsItem1.Name = "setNormalSectionPageMarginsItem1";
		this.setNarrowSectionPageMarginsItem1.Id = 86;
		this.setNarrowSectionPageMarginsItem1.Name = "setNarrowSectionPageMarginsItem1";
		this.setModerateSectionPageMarginsItem1.Id = 87;
		this.setModerateSectionPageMarginsItem1.Name = "setModerateSectionPageMarginsItem1";
		this.setWideSectionPageMarginsItem1.Id = 88;
		this.setWideSectionPageMarginsItem1.Name = "setWideSectionPageMarginsItem1";
		this.showPageMarginsSetupFormItem1.Id = 89;
		this.showPageMarginsSetupFormItem1.Name = "showPageMarginsSetupFormItem1";
		this.changeSectionPageOrientationItem1.Id = 90;
		this.changeSectionPageOrientationItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setPortraitPageOrientationItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setLandscapePageOrientationItem1)
		});
		this.changeSectionPageOrientationItem1.Name = "changeSectionPageOrientationItem1";
		this.setPortraitPageOrientationItem1.Id = 91;
		this.setPortraitPageOrientationItem1.Name = "setPortraitPageOrientationItem1";
		this.setLandscapePageOrientationItem1.Id = 92;
		this.setLandscapePageOrientationItem1.Name = "setLandscapePageOrientationItem1";
		this.changeSectionPaperKindItem1.Id = 93;
		this.changeSectionPaperKindItem1.Name = "changeSectionPaperKindItem1";
		this.changeSectionColumnsItem1.Id = 94;
		this.changeSectionColumnsItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionOneColumnItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionTwoColumnsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionThreeColumnsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showColumnsSetupFormItem1, true)
		});
		this.changeSectionColumnsItem1.Name = "changeSectionColumnsItem1";
		this.setSectionOneColumnItem1.Id = 95;
		this.setSectionOneColumnItem1.Name = "setSectionOneColumnItem1";
		this.setSectionTwoColumnsItem1.Id = 96;
		this.setSectionTwoColumnsItem1.Name = "setSectionTwoColumnsItem1";
		this.setSectionThreeColumnsItem1.Id = 97;
		this.setSectionThreeColumnsItem1.Name = "setSectionThreeColumnsItem1";
		this.showColumnsSetupFormItem1.Id = 98;
		this.showColumnsSetupFormItem1.Name = "showColumnsSetupFormItem1";
		this.insertBreakItem1.Id = 99;
		this.insertBreakItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.insertPageBreakItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertColumnBreakItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertSectionBreakNextPageItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertSectionBreakEvenPageItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertSectionBreakOddPageItem1)
		});
		this.insertBreakItem1.Name = "insertBreakItem1";
		this.insertPageBreakItem1.Id = 100;
		this.insertPageBreakItem1.Name = "insertPageBreakItem1";
		this.insertColumnBreakItem1.Id = 101;
		this.insertColumnBreakItem1.Name = "insertColumnBreakItem1";
		this.insertSectionBreakNextPageItem1.Id = 102;
		this.insertSectionBreakNextPageItem1.Name = "insertSectionBreakNextPageItem1";
		this.insertSectionBreakEvenPageItem1.Id = 103;
		this.insertSectionBreakEvenPageItem1.Name = "insertSectionBreakEvenPageItem1";
		this.insertSectionBreakOddPageItem1.Id = 104;
		this.insertSectionBreakOddPageItem1.Name = "insertSectionBreakOddPageItem1";
		this.changeSectionLineNumberingItem1.Id = 105;
		this.changeSectionLineNumberingItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionLineNumberingNoneItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionLineNumberingContinuousItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionLineNumberingRestartNewPageItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setSectionLineNumberingRestartNewSectionItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleParagraphSuppressLineNumbersItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.showLineNumberingFormItem1, true)
		});
		this.changeSectionLineNumberingItem1.Name = "changeSectionLineNumberingItem1";
		this.setSectionLineNumberingNoneItem1.Id = 106;
		this.setSectionLineNumberingNoneItem1.Name = "setSectionLineNumberingNoneItem1";
		this.setSectionLineNumberingContinuousItem1.Id = 107;
		this.setSectionLineNumberingContinuousItem1.Name = "setSectionLineNumberingContinuousItem1";
		this.setSectionLineNumberingRestartNewPageItem1.Id = 108;
		this.setSectionLineNumberingRestartNewPageItem1.Name = "setSectionLineNumberingRestartNewPageItem1";
		this.setSectionLineNumberingRestartNewSectionItem1.Id = 109;
		this.setSectionLineNumberingRestartNewSectionItem1.Name = "setSectionLineNumberingRestartNewSectionItem1";
		this.toggleParagraphSuppressLineNumbersItem1.Id = 110;
		this.toggleParagraphSuppressLineNumbersItem1.Name = "toggleParagraphSuppressLineNumbersItem1";
		this.showLineNumberingFormItem1.Id = 111;
		this.showLineNumberingFormItem1.Name = "showLineNumberingFormItem1";
		this.changePageColorItem1.Id = 112;
		this.changePageColorItem1.Name = "changePageColorItem1";
		this.switchToSimpleViewItem1.Id = 113;
		this.switchToSimpleViewItem1.Name = "switchToSimpleViewItem1";
		this.switchToDraftViewItem1.Id = 114;
		this.switchToDraftViewItem1.Name = "switchToDraftViewItem1";
		this.switchToPrintLayoutViewItem1.Id = 115;
		this.switchToPrintLayoutViewItem1.Name = "switchToPrintLayoutViewItem1";
		this.toggleShowHorizontalRulerItem1.Id = 116;
		this.toggleShowHorizontalRulerItem1.Name = "toggleShowHorizontalRulerItem1";
		this.toggleShowVerticalRulerItem1.Id = 117;
		this.toggleShowVerticalRulerItem1.Name = "toggleShowVerticalRulerItem1";
		this.zoomOutItem1.Id = 118;
		this.zoomOutItem1.Name = "zoomOutItem1";
		this.zoomInItem1.Id = 119;
		this.zoomInItem1.Name = "zoomInItem1";
		this.snapBarToolbarsListItem1.Hint = "Show or hide the Data Explorer and Report Explorer windows.";
		this.snapBarToolbarsListItem1.Id = 120;
		this.snapBarToolbarsListItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("snapBarToolbarsListItem1.ImageOptions.Image");
		this.snapBarToolbarsListItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("snapBarToolbarsListItem1.ImageOptions.LargeImage");
		this.snapBarToolbarsListItem1.Name = "snapBarToolbarsListItem1";
		this.snapBarToolbarsListItem1.ShowCustomizationItem = false;
		this.snapBarToolbarsListItem1.ShowDockPanels = true;
		this.snapBarToolbarsListItem1.ShowToolbars = false;
		this.commandBarCheckItem1.Id = 121;
		this.commandBarCheckItem1.Name = "commandBarCheckItem1";
		this.commandBarCheckItem1.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.HighlightActiveElement;
		this.showAllFieldCodesItem1.Id = 122;
		this.showAllFieldCodesItem1.Name = "showAllFieldCodesItem1";
		this.showAllFieldResultsItem1.Id = 123;
		this.showAllFieldResultsItem1.Name = "showAllFieldResultsItem1";
		this.toggleFieldHighlightingItem1.Id = 124;
		this.toggleFieldHighlightingItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("toggleFieldHighlightingItem1.ImageOptions.Image");
		this.toggleFieldHighlightingItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("toggleFieldHighlightingItem1.ImageOptions.LargeImage");
		this.toggleFieldHighlightingItem1.Name = "toggleFieldHighlightingItem1";
		this.goToPageHeaderItem1.Id = 125;
		this.goToPageHeaderItem1.Name = "goToPageHeaderItem1";
		this.goToPageFooterItem1.Id = 126;
		this.goToPageFooterItem1.Name = "goToPageFooterItem1";
		this.goToNextHeaderFooterItem1.Id = 127;
		this.goToNextHeaderFooterItem1.Name = "goToNextHeaderFooterItem1";
		this.goToPreviousHeaderFooterItem1.Id = 128;
		this.goToPreviousHeaderFooterItem1.Name = "goToPreviousHeaderFooterItem1";
		this.toggleLinkToPreviousItem1.Id = 129;
		this.toggleLinkToPreviousItem1.Name = "toggleLinkToPreviousItem1";
		this.toggleDifferentFirstPageItem1.Id = 130;
		this.toggleDifferentFirstPageItem1.Name = "toggleDifferentFirstPageItem1";
		this.toggleDifferentOddAndEvenPagesItem1.Id = 131;
		this.toggleDifferentOddAndEvenPagesItem1.Name = "toggleDifferentOddAndEvenPagesItem1";
		this.closePageHeaderFooterItem1.Id = 132;
		this.closePageHeaderFooterItem1.Name = "closePageHeaderFooterItem1";
		this.toggleFirstRowItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleFirstRowItem1.Id = 133;
		this.toggleFirstRowItem1.Name = "toggleFirstRowItem1";
		this.toggleLastRowItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleLastRowItem1.Id = 134;
		this.toggleLastRowItem1.Name = "toggleLastRowItem1";
		this.toggleBandedRowsItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleBandedRowsItem1.Id = 135;
		this.toggleBandedRowsItem1.Name = "toggleBandedRowsItem1";
		this.toggleFirstColumnItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleFirstColumnItem1.Id = 136;
		this.toggleFirstColumnItem1.Name = "toggleFirstColumnItem1";
		this.toggleLastColumnItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleLastColumnItem1.Id = 137;
		this.toggleLastColumnItem1.Name = "toggleLastColumnItem1";
		this.toggleBandedColumnsItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
		this.toggleBandedColumnsItem1.Id = 138;
		this.toggleBandedColumnsItem1.Name = "toggleBandedColumnsItem1";
		this.galleryChangeTableStyleItem1.CurrentItem = null;
		this.galleryChangeTableStyleItem1.DeleteItemLink = null;
		this.galleryChangeTableStyleItem1.Gallery.ColumnCount = 3;
		this.galleryChangeTableStyleItem1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup2 });
		this.galleryChangeTableStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChangeTableStyleItem1.Id = 139;
		this.galleryChangeTableStyleItem1.ModifyItemLink = null;
		this.galleryChangeTableStyleItem1.Name = "galleryChangeTableStyleItem1";
		this.galleryChangeTableStyleItem1.NewItemLink = null;
		this.galleryChangeTableStyleItem1.PopupGallery = null;
		this.galleryChangeTableCellStyleItem1.CurrentItem = null;
		this.galleryChangeTableCellStyleItem1.DeleteItemLink = null;
		this.galleryChangeTableCellStyleItem1.Gallery.ColumnCount = 3;
		this.galleryChangeTableCellStyleItem1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { galleryItemGroup3 });
		this.galleryChangeTableCellStyleItem1.Gallery.ImageSize = new System.Drawing.Size(65, 46);
		this.galleryChangeTableCellStyleItem1.Id = 140;
		this.galleryChangeTableCellStyleItem1.ModifyItemLink = null;
		this.galleryChangeTableCellStyleItem1.Name = "galleryChangeTableCellStyleItem1";
		this.galleryChangeTableCellStyleItem1.NewItemLink = null;
		this.galleryChangeTableCellStyleItem1.PopupGallery = null;
		this.changeTableBorderLineStyleItem1.Edit = this.repositoryItemBorderLineStyle1;
		this.changeTableBorderLineStyleItem1.EditWidth = 130;
		this.changeTableBorderLineStyleItem1.Id = 141;
		this.changeTableBorderLineStyleItem1.Name = "changeTableBorderLineStyleItem1";
		this.repositoryItemBorderLineStyle1.AutoHeight = false;
		this.repositoryItemBorderLineStyle1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemBorderLineStyle1.Control = this.snapControl1;
		this.repositoryItemBorderLineStyle1.Name = "repositoryItemBorderLineStyle1";
		this.changeTableBorderLineWeightItem1.Edit = this.repositoryItemBorderLineWeight1;
		this.changeTableBorderLineWeightItem1.EditValue = 20;
		this.changeTableBorderLineWeightItem1.EditWidth = 130;
		this.changeTableBorderLineWeightItem1.Id = 142;
		this.changeTableBorderLineWeightItem1.Name = "changeTableBorderLineWeightItem1";
		this.repositoryItemBorderLineWeight1.AutoHeight = false;
		this.repositoryItemBorderLineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemBorderLineWeight1.Control = this.snapControl1;
		this.repositoryItemBorderLineWeight1.Name = "repositoryItemBorderLineWeight1";
		this.changeTableBorderColorItem1.Id = 143;
		this.changeTableBorderColorItem1.Name = "changeTableBorderColorItem1";
		this.changeTableBordersItem1.Id = 144;
		this.changeTableBordersItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[11]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsBottomBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsTopBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsLeftBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsRightBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.resetTableCellsAllBordersItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsAllBordersItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsOutsideBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideHorizontalBorderItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableCellsInsideVerticalBorderItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleShowTableGridLinesItem1, true)
		});
		this.changeTableBordersItem1.Name = "changeTableBordersItem1";
		this.toggleTableCellsBottomBorderItem1.Id = 145;
		this.toggleTableCellsBottomBorderItem1.Name = "toggleTableCellsBottomBorderItem1";
		this.toggleTableCellsTopBorderItem1.Id = 146;
		this.toggleTableCellsTopBorderItem1.Name = "toggleTableCellsTopBorderItem1";
		this.toggleTableCellsLeftBorderItem1.Id = 147;
		this.toggleTableCellsLeftBorderItem1.Name = "toggleTableCellsLeftBorderItem1";
		this.toggleTableCellsRightBorderItem1.Id = 148;
		this.toggleTableCellsRightBorderItem1.Name = "toggleTableCellsRightBorderItem1";
		this.resetTableCellsAllBordersItem1.Id = 149;
		this.resetTableCellsAllBordersItem1.Name = "resetTableCellsAllBordersItem1";
		this.toggleTableCellsAllBordersItem1.Id = 150;
		this.toggleTableCellsAllBordersItem1.Name = "toggleTableCellsAllBordersItem1";
		this.toggleTableCellsOutsideBorderItem1.Id = 151;
		this.toggleTableCellsOutsideBorderItem1.Name = "toggleTableCellsOutsideBorderItem1";
		this.toggleTableCellsInsideBorderItem1.Id = 152;
		this.toggleTableCellsInsideBorderItem1.Name = "toggleTableCellsInsideBorderItem1";
		this.toggleTableCellsInsideHorizontalBorderItem1.Id = 153;
		this.toggleTableCellsInsideHorizontalBorderItem1.Name = "toggleTableCellsInsideHorizontalBorderItem1";
		this.toggleTableCellsInsideVerticalBorderItem1.Id = 154;
		this.toggleTableCellsInsideVerticalBorderItem1.Name = "toggleTableCellsInsideVerticalBorderItem1";
		this.toggleShowTableGridLinesItem1.Id = 155;
		this.toggleShowTableGridLinesItem1.Name = "toggleShowTableGridLinesItem1";
		this.changeTableCellsShadingItem1.Id = 156;
		this.changeTableCellsShadingItem1.Name = "changeTableCellsShadingItem1";
		this.selectTableElementsItem1.Id = 157;
		this.selectTableElementsItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableCellItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableColumnItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableRowItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.selectTableItem1)
		});
		this.selectTableElementsItem1.Name = "selectTableElementsItem1";
		this.selectTableCellItem1.Id = 158;
		this.selectTableCellItem1.Name = "selectTableCellItem1";
		this.selectTableColumnItem1.Id = 159;
		this.selectTableColumnItem1.Name = "selectTableColumnItem1";
		this.selectTableRowItem1.Id = 160;
		this.selectTableRowItem1.Name = "selectTableRowItem1";
		this.selectTableItem1.Id = 161;
		this.selectTableItem1.Name = "selectTableItem1";
		this.showTablePropertiesFormItem1.Id = 162;
		this.showTablePropertiesFormItem1.Name = "showTablePropertiesFormItem1";
		this.deleteTableElementsItem1.Id = 163;
		this.deleteTableElementsItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.showDeleteTableCellsFormItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableColumnsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableRowsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteTableItem1)
		});
		this.deleteTableElementsItem1.Name = "deleteTableElementsItem1";
		this.showDeleteTableCellsFormItem1.Id = 164;
		this.showDeleteTableCellsFormItem1.Name = "showDeleteTableCellsFormItem1";
		this.deleteTableColumnsItem1.Id = 165;
		this.deleteTableColumnsItem1.Name = "deleteTableColumnsItem1";
		this.deleteTableRowsItem1.Id = 166;
		this.deleteTableRowsItem1.Name = "deleteTableRowsItem1";
		this.deleteTableItem1.Id = 167;
		this.deleteTableItem1.Name = "deleteTableItem1";
		this.insertTableRowAboveItem1.Id = 168;
		this.insertTableRowAboveItem1.Name = "insertTableRowAboveItem1";
		this.insertTableRowBelowItem1.Id = 169;
		this.insertTableRowBelowItem1.Name = "insertTableRowBelowItem1";
		this.insertTableColumnToLeftItem1.Id = 170;
		this.insertTableColumnToLeftItem1.Name = "insertTableColumnToLeftItem1";
		this.insertTableColumnToRightItem1.Id = 171;
		this.insertTableColumnToRightItem1.Name = "insertTableColumnToRightItem1";
		this.mergeTableCellsItem1.Id = 172;
		this.mergeTableCellsItem1.Name = "mergeTableCellsItem1";
		this.showSplitTableCellsForm1.Id = 173;
		this.showSplitTableCellsForm1.Name = "showSplitTableCellsForm1";
		this.splitTableItem1.Id = 174;
		this.splitTableItem1.Name = "splitTableItem1";
		this.toggleTableAutoFitItem1.Id = 175;
		this.toggleTableAutoFitItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableAutoFitContentsItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableAutoFitWindowItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleTableFixedColumnWidthItem1)
		});
		this.toggleTableAutoFitItem1.Name = "toggleTableAutoFitItem1";
		this.toggleTableAutoFitContentsItem1.Id = 176;
		this.toggleTableAutoFitContentsItem1.Name = "toggleTableAutoFitContentsItem1";
		this.toggleTableAutoFitWindowItem1.Id = 177;
		this.toggleTableAutoFitWindowItem1.Name = "toggleTableAutoFitWindowItem1";
		this.toggleTableFixedColumnWidthItem1.Id = 178;
		this.toggleTableFixedColumnWidthItem1.Name = "toggleTableFixedColumnWidthItem1";
		this.toggleTableCellsTopLeftAlignmentItem1.Id = 179;
		this.toggleTableCellsTopLeftAlignmentItem1.Name = "toggleTableCellsTopLeftAlignmentItem1";
		this.toggleTableCellsMiddleLeftAlignmentItem1.Id = 180;
		this.toggleTableCellsMiddleLeftAlignmentItem1.Name = "toggleTableCellsMiddleLeftAlignmentItem1";
		this.toggleTableCellsBottomLeftAlignmentItem1.Id = 181;
		this.toggleTableCellsBottomLeftAlignmentItem1.Name = "toggleTableCellsBottomLeftAlignmentItem1";
		this.toggleTableCellsTopCenterAlignmentItem1.Id = 182;
		this.toggleTableCellsTopCenterAlignmentItem1.Name = "toggleTableCellsTopCenterAlignmentItem1";
		this.toggleTableCellsMiddleCenterAlignmentItem1.Id = 183;
		this.toggleTableCellsMiddleCenterAlignmentItem1.Name = "toggleTableCellsMiddleCenterAlignmentItem1";
		this.toggleTableCellsBottomCenterAlignmentItem1.Id = 184;
		this.toggleTableCellsBottomCenterAlignmentItem1.Name = "toggleTableCellsBottomCenterAlignmentItem1";
		this.toggleTableCellsTopRightAlignmentItem1.Id = 185;
		this.toggleTableCellsTopRightAlignmentItem1.Name = "toggleTableCellsTopRightAlignmentItem1";
		this.toggleTableCellsMiddleRightAlignmentItem1.Id = 186;
		this.toggleTableCellsMiddleRightAlignmentItem1.Name = "toggleTableCellsMiddleRightAlignmentItem1";
		this.toggleTableCellsBottomRightAlignmentItem1.Id = 187;
		this.toggleTableCellsBottomRightAlignmentItem1.Name = "toggleTableCellsBottomRightAlignmentItem1";
		this.showTableOptionsFormItem1.Id = 188;
		this.showTableOptionsFormItem1.Name = "showTableOptionsFormItem1";
		this.checkSpellingItem1.Id = 189;
		this.checkSpellingItem1.Name = "checkSpellingItem1";
		this.insertTableOfContentsItem1.Id = 190;
		this.insertTableOfContentsItem1.Name = "insertTableOfContentsItem1";
		this.updateTableOfContentsItem1.Id = 191;
		this.updateTableOfContentsItem1.Name = "updateTableOfContentsItem1";
		this.addParagraphsToTableOfContentItem1.Id = 192;
		this.addParagraphsToTableOfContentItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[10]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem8),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem9),
			new DevExpress.XtraBars.LinkPersistInfo(this.setParagraphHeadingLevelItem10)
		});
		this.addParagraphsToTableOfContentItem1.Name = "addParagraphsToTableOfContentItem1";
		this.setParagraphHeadingLevelItem1.Id = 193;
		this.setParagraphHeadingLevelItem1.Name = "setParagraphHeadingLevelItem1";
		this.setParagraphHeadingLevelItem1.OutlineLevel = 0;
		this.setParagraphHeadingLevelItem2.Id = 194;
		this.setParagraphHeadingLevelItem2.Name = "setParagraphHeadingLevelItem2";
		this.setParagraphHeadingLevelItem2.OutlineLevel = 1;
		this.setParagraphHeadingLevelItem3.Id = 195;
		this.setParagraphHeadingLevelItem3.Name = "setParagraphHeadingLevelItem3";
		this.setParagraphHeadingLevelItem3.OutlineLevel = 2;
		this.setParagraphHeadingLevelItem4.Id = 196;
		this.setParagraphHeadingLevelItem4.Name = "setParagraphHeadingLevelItem4";
		this.setParagraphHeadingLevelItem4.OutlineLevel = 3;
		this.setParagraphHeadingLevelItem5.Id = 197;
		this.setParagraphHeadingLevelItem5.Name = "setParagraphHeadingLevelItem5";
		this.setParagraphHeadingLevelItem5.OutlineLevel = 4;
		this.setParagraphHeadingLevelItem6.Id = 198;
		this.setParagraphHeadingLevelItem6.Name = "setParagraphHeadingLevelItem6";
		this.setParagraphHeadingLevelItem6.OutlineLevel = 5;
		this.setParagraphHeadingLevelItem7.Id = 199;
		this.setParagraphHeadingLevelItem7.Name = "setParagraphHeadingLevelItem7";
		this.setParagraphHeadingLevelItem7.OutlineLevel = 6;
		this.setParagraphHeadingLevelItem8.Id = 200;
		this.setParagraphHeadingLevelItem8.Name = "setParagraphHeadingLevelItem8";
		this.setParagraphHeadingLevelItem8.OutlineLevel = 7;
		this.setParagraphHeadingLevelItem9.Id = 201;
		this.setParagraphHeadingLevelItem9.Name = "setParagraphHeadingLevelItem9";
		this.setParagraphHeadingLevelItem9.OutlineLevel = 8;
		this.setParagraphHeadingLevelItem10.Id = 202;
		this.setParagraphHeadingLevelItem10.Name = "setParagraphHeadingLevelItem10";
		this.setParagraphHeadingLevelItem10.OutlineLevel = 9;
		this.insertCaptionPlaceholderItem1.Id = 203;
		this.insertCaptionPlaceholderItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.insertFiguresCaptionItems1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTablesCaptionItems1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertEquationsCaptionItems1)
		});
		this.insertCaptionPlaceholderItem1.Name = "insertCaptionPlaceholderItem1";
		this.insertFiguresCaptionItems1.Id = 204;
		this.insertFiguresCaptionItems1.Name = "insertFiguresCaptionItems1";
		this.insertTablesCaptionItems1.Id = 205;
		this.insertTablesCaptionItems1.Name = "insertTablesCaptionItems1";
		this.insertEquationsCaptionItems1.Id = 206;
		this.insertEquationsCaptionItems1.Name = "insertEquationsCaptionItems1";
		this.insertTableOfFiguresPlaceholderItem1.Id = 207;
		this.insertTableOfFiguresPlaceholderItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTableOfFiguresItems1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTableOfTablesItems1),
			new DevExpress.XtraBars.LinkPersistInfo(this.insertTableOfEquationsItems1)
		});
		this.insertTableOfFiguresPlaceholderItem1.Name = "insertTableOfFiguresPlaceholderItem1";
		this.insertTableOfFiguresItems1.Id = 208;
		this.insertTableOfFiguresItems1.Name = "insertTableOfFiguresItems1";
		this.insertTableOfTablesItems1.Id = 209;
		this.insertTableOfTablesItems1.Name = "insertTableOfTablesItems1";
		this.insertTableOfEquationsItems1.Id = 210;
		this.insertTableOfEquationsItems1.Name = "insertTableOfEquationsItems1";
		this.updateTableOfFiguresItem1.Id = 211;
		this.updateTableOfFiguresItem1.Name = "updateTableOfFiguresItem1";
		this.changeFloatingObjectFillColorItem1.Id = 212;
		this.changeFloatingObjectFillColorItem1.Name = "changeFloatingObjectFillColorItem1";
		this.changeFloatingObjectOutlineColorItem1.Id = 213;
		this.changeFloatingObjectOutlineColorItem1.Name = "changeFloatingObjectOutlineColorItem1";
		this.changeFloatingObjectOutlineWeightItem1.Edit = this.repositoryItemFloatingObjectOutlineWeight1;
		this.changeFloatingObjectOutlineWeightItem1.EditValue = 20;
		this.changeFloatingObjectOutlineWeightItem1.Id = 214;
		this.changeFloatingObjectOutlineWeightItem1.Name = "changeFloatingObjectOutlineWeightItem1";
		this.repositoryItemFloatingObjectOutlineWeight1.AutoHeight = false;
		this.repositoryItemFloatingObjectOutlineWeight1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFloatingObjectOutlineWeight1.Control = this.snapControl1;
		this.repositoryItemFloatingObjectOutlineWeight1.Name = "repositoryItemFloatingObjectOutlineWeight1";
		this.changeFloatingObjectTextWrapTypeItem1.Id = 215;
		this.changeFloatingObjectTextWrapTypeItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[6]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectSquareTextWrapTypeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectTightTextWrapTypeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectThroughTextWrapTypeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectTopAndBottomTextWrapTypeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectBehindTextWrapTypeItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectInFrontOfTextWrapTypeItem1)
		});
		this.changeFloatingObjectTextWrapTypeItem1.Name = "changeFloatingObjectTextWrapTypeItem1";
		this.setFloatingObjectSquareTextWrapTypeItem1.Id = 216;
		this.setFloatingObjectSquareTextWrapTypeItem1.Name = "setFloatingObjectSquareTextWrapTypeItem1";
		this.setFloatingObjectTightTextWrapTypeItem1.Id = 217;
		this.setFloatingObjectTightTextWrapTypeItem1.Name = "setFloatingObjectTightTextWrapTypeItem1";
		this.setFloatingObjectThroughTextWrapTypeItem1.Id = 218;
		this.setFloatingObjectThroughTextWrapTypeItem1.Name = "setFloatingObjectThroughTextWrapTypeItem1";
		this.setFloatingObjectTopAndBottomTextWrapTypeItem1.Id = 219;
		this.setFloatingObjectTopAndBottomTextWrapTypeItem1.Name = "setFloatingObjectTopAndBottomTextWrapTypeItem1";
		this.setFloatingObjectBehindTextWrapTypeItem1.Id = 220;
		this.setFloatingObjectBehindTextWrapTypeItem1.Name = "setFloatingObjectBehindTextWrapTypeItem1";
		this.setFloatingObjectInFrontOfTextWrapTypeItem1.Id = 221;
		this.setFloatingObjectInFrontOfTextWrapTypeItem1.Name = "setFloatingObjectInFrontOfTextWrapTypeItem1";
		this.changeFloatingObjectAlignmentItem1.Id = 222;
		this.changeFloatingObjectAlignmentItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectTopLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectTopCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectTopRightAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectMiddleLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectMiddleCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectMiddleRightAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectBottomLeftAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectBottomCenterAlignmentItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.setFloatingObjectBottomRightAlignmentItem1)
		});
		this.changeFloatingObjectAlignmentItem1.Name = "changeFloatingObjectAlignmentItem1";
		this.setFloatingObjectTopLeftAlignmentItem1.Id = 223;
		this.setFloatingObjectTopLeftAlignmentItem1.Name = "setFloatingObjectTopLeftAlignmentItem1";
		this.setFloatingObjectTopCenterAlignmentItem1.Id = 224;
		this.setFloatingObjectTopCenterAlignmentItem1.Name = "setFloatingObjectTopCenterAlignmentItem1";
		this.setFloatingObjectTopRightAlignmentItem1.Id = 225;
		this.setFloatingObjectTopRightAlignmentItem1.Name = "setFloatingObjectTopRightAlignmentItem1";
		this.setFloatingObjectMiddleLeftAlignmentItem1.Id = 226;
		this.setFloatingObjectMiddleLeftAlignmentItem1.Name = "setFloatingObjectMiddleLeftAlignmentItem1";
		this.setFloatingObjectMiddleCenterAlignmentItem1.Id = 227;
		this.setFloatingObjectMiddleCenterAlignmentItem1.Name = "setFloatingObjectMiddleCenterAlignmentItem1";
		this.setFloatingObjectMiddleRightAlignmentItem1.Id = 228;
		this.setFloatingObjectMiddleRightAlignmentItem1.Name = "setFloatingObjectMiddleRightAlignmentItem1";
		this.setFloatingObjectBottomLeftAlignmentItem1.Id = 229;
		this.setFloatingObjectBottomLeftAlignmentItem1.Name = "setFloatingObjectBottomLeftAlignmentItem1";
		this.setFloatingObjectBottomCenterAlignmentItem1.Id = 230;
		this.setFloatingObjectBottomCenterAlignmentItem1.Name = "setFloatingObjectBottomCenterAlignmentItem1";
		this.setFloatingObjectBottomRightAlignmentItem1.Id = 231;
		this.setFloatingObjectBottomRightAlignmentItem1.Name = "setFloatingObjectBottomRightAlignmentItem1";
		this.floatingObjectBringForwardSubItem1.Id = 232;
		this.floatingObjectBringForwardSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectBringForwardItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectBringToFrontItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectBringInFrontOfTextItem1)
		});
		this.floatingObjectBringForwardSubItem1.Name = "floatingObjectBringForwardSubItem1";
		this.floatingObjectBringForwardItem1.Id = 233;
		this.floatingObjectBringForwardItem1.Name = "floatingObjectBringForwardItem1";
		this.floatingObjectBringToFrontItem1.Id = 234;
		this.floatingObjectBringToFrontItem1.Name = "floatingObjectBringToFrontItem1";
		this.floatingObjectBringInFrontOfTextItem1.Id = 235;
		this.floatingObjectBringInFrontOfTextItem1.Name = "floatingObjectBringInFrontOfTextItem1";
		this.floatingObjectSendBackwardSubItem1.Id = 236;
		this.floatingObjectSendBackwardSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectSendBackwardItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectSendToBackItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.floatingObjectSendBehindTextItem1)
		});
		this.floatingObjectSendBackwardSubItem1.Name = "floatingObjectSendBackwardSubItem1";
		this.floatingObjectSendBackwardItem1.Id = 237;
		this.floatingObjectSendBackwardItem1.Name = "floatingObjectSendBackwardItem1";
		this.floatingObjectSendToBackItem1.Id = 238;
		this.floatingObjectSendToBackItem1.Name = "floatingObjectSendToBackItem1";
		this.floatingObjectSendBehindTextItem1.Id = 239;
		this.floatingObjectSendBehindTextItem1.Name = "floatingObjectSendBehindTextItem1";
		this.commandBarCheckItem2.Id = 241;
		this.commandBarCheckItem2.Name = "commandBarCheckItem2";
		this.commandBarCheckItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarCheckItem2.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.GroupByField;
		this.commandBarCheckItem3.Id = 242;
		this.commandBarCheckItem3.Name = "commandBarCheckItem3";
		this.commandBarCheckItem3.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SnapSortFieldAscending;
		this.commandBarCheckItem4.Id = 243;
		this.commandBarCheckItem4.Name = "commandBarCheckItem4";
		this.commandBarCheckItem4.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SnapSortFieldDescending;
		this.commandBarSubItem1.Id = 244;
		this.commandBarSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem7),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem8),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem9)
		});
		this.commandBarSubItem1.Name = "commandBarSubItem1";
		this.commandBarSubItem1.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummaryByField;
		this.commandBarCheckItem5.Id = 245;
		this.commandBarCheckItem5.Name = "commandBarCheckItem5";
		this.commandBarCheckItem5.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummaryCount;
		this.commandBarCheckItem6.Id = 246;
		this.commandBarCheckItem6.Name = "commandBarCheckItem6";
		this.commandBarCheckItem6.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummarySum;
		this.commandBarCheckItem7.Id = 247;
		this.commandBarCheckItem7.Name = "commandBarCheckItem7";
		this.commandBarCheckItem7.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummaryAverage;
		this.commandBarCheckItem8.Id = 248;
		this.commandBarCheckItem8.Name = "commandBarCheckItem8";
		this.commandBarCheckItem8.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummaryMax;
		this.commandBarCheckItem9.Id = 249;
		this.commandBarCheckItem9.Name = "commandBarCheckItem9";
		this.commandBarCheckItem9.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SummaryMin;
		this.filterPopupButtonItem1.ActAsDropDown = true;
		this.filterPopupButtonItem1.Id = 250;
		this.filterPopupButtonItem1.Name = "filterPopupButtonItem1";
		this.propertiesBarButtonItem1.ActAsDropDown = true;
		this.propertiesBarButtonItem1.Id = 251;
		this.propertiesBarButtonItem1.Name = "propertiesBarButtonItem1";
		this.propertiesBarButtonItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarSubItem2.Id = 252;
		this.commandBarSubItem2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem10),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem11)
		});
		this.commandBarSubItem2.Name = "commandBarSubItem2";
		this.commandBarSubItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarSubItem2.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.GroupHeader;
		this.commandBarCheckItem10.Id = 253;
		this.commandBarCheckItem10.Name = "commandBarCheckItem10";
		this.commandBarCheckItem10.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertGroupHeader;
		this.commandBarCheckItem11.Id = 254;
		this.commandBarCheckItem11.Name = "commandBarCheckItem11";
		this.commandBarCheckItem11.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.RemoveGroupHeader;
		this.commandBarSubItem3.Id = 255;
		this.commandBarSubItem3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem12),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem13)
		});
		this.commandBarSubItem3.Name = "commandBarSubItem3";
		this.commandBarSubItem3.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.GroupFooter;
		this.commandBarCheckItem12.Id = 256;
		this.commandBarCheckItem12.Name = "commandBarCheckItem12";
		this.commandBarCheckItem12.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertGroupFooter;
		this.commandBarCheckItem13.Id = 257;
		this.commandBarCheckItem13.Name = "commandBarCheckItem13";
		this.commandBarCheckItem13.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.RemoveGroupFooter;
		this.commandBarItem8.Id = 258;
		this.commandBarItem8.Name = "commandBarItem8";
		this.commandBarItem8.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.GroupFieldsCollection;
		this.commandBarItem9.Id = 259;
		this.commandBarItem9.Name = "commandBarItem9";
		this.commandBarItem9.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.ShowReportStructureEditorForm;
		this.commandBarSubItem4.Id = 260;
		this.commandBarSubItem4.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem15),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem16),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem17),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem18),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem19),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem20)
		});
		this.commandBarSubItem4.Name = "commandBarSubItem4";
		this.commandBarSubItem4.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertGroupSeparator;
		this.commandBarCheckItem14.Id = 261;
		this.commandBarCheckItem14.Name = "commandBarCheckItem14";
		this.commandBarCheckItem14.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertPageBreakGroupSeparator;
		this.commandBarCheckItem15.Id = 262;
		this.commandBarCheckItem15.Name = "commandBarCheckItem15";
		this.commandBarCheckItem15.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakNextPageGroupSeparator;
		this.commandBarCheckItem16.Id = 263;
		this.commandBarCheckItem16.Name = "commandBarCheckItem16";
		this.commandBarCheckItem16.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakEvenPageGroupSeparator;
		this.commandBarCheckItem17.Id = 264;
		this.commandBarCheckItem17.Name = "commandBarCheckItem17";
		this.commandBarCheckItem17.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakOddPageGroupSeparator;
		this.commandBarCheckItem18.Id = 265;
		this.commandBarCheckItem18.Name = "commandBarCheckItem18";
		this.commandBarCheckItem18.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertEmptyParagraphGroupSeparator;
		this.commandBarCheckItem19.Id = 266;
		this.commandBarCheckItem19.Name = "commandBarCheckItem19";
		this.commandBarCheckItem19.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertEmptyRowGroupSeparator;
		this.commandBarCheckItem20.Id = 267;
		this.commandBarCheckItem20.Name = "commandBarCheckItem20";
		this.commandBarCheckItem20.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertNoneGroupSeparator;
		this.commandBarSubItem5.Id = 269;
		this.commandBarSubItem5.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem21),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem22)
		});
		this.commandBarSubItem5.Name = "commandBarSubItem5";
		this.commandBarSubItem5.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarSubItem5.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.ListHeader;
		this.commandBarCheckItem21.Id = 270;
		this.commandBarCheckItem21.Name = "commandBarCheckItem21";
		this.commandBarCheckItem21.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertListHeader;
		this.commandBarCheckItem22.Id = 271;
		this.commandBarCheckItem22.Name = "commandBarCheckItem22";
		this.commandBarCheckItem22.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.RemoveListHeader;
		this.commandBarSubItem6.Id = 272;
		this.commandBarSubItem6.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem23),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem24)
		});
		this.commandBarSubItem6.Name = "commandBarSubItem6";
		this.commandBarSubItem6.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarSubItem6.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.ListFooter;
		this.commandBarCheckItem23.Id = 273;
		this.commandBarCheckItem23.Name = "commandBarCheckItem23";
		this.commandBarCheckItem23.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertListFooter;
		this.commandBarCheckItem24.Id = 274;
		this.commandBarCheckItem24.Name = "commandBarCheckItem24";
		this.commandBarCheckItem24.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.RemoveListFooter;
		this.commandBarSubItem7.Id = 275;
		this.commandBarSubItem7.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem25),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem26),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem27),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem28),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem29),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem30),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarCheckItem31)
		});
		this.commandBarSubItem7.Name = "commandBarSubItem7";
		this.commandBarSubItem7.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertDataRowSeparator;
		this.commandBarCheckItem25.Id = 276;
		this.commandBarCheckItem25.Name = "commandBarCheckItem25";
		this.commandBarCheckItem25.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertPageBreakDataRowSeparator;
		this.commandBarCheckItem26.Id = 277;
		this.commandBarCheckItem26.Name = "commandBarCheckItem26";
		this.commandBarCheckItem26.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakNextPageDataRowSeparator;
		this.commandBarCheckItem27.Id = 278;
		this.commandBarCheckItem27.Name = "commandBarCheckItem27";
		this.commandBarCheckItem27.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakEvenPageDataRowSeparator;
		this.commandBarCheckItem28.Id = 279;
		this.commandBarCheckItem28.Name = "commandBarCheckItem28";
		this.commandBarCheckItem28.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertSectionBreakOddPageDataRowSeparator;
		this.commandBarCheckItem29.Id = 280;
		this.commandBarCheckItem29.Name = "commandBarCheckItem29";
		this.commandBarCheckItem29.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertEmptyParagraphDataRowSeparator;
		this.commandBarCheckItem30.Id = 281;
		this.commandBarCheckItem30.Name = "commandBarCheckItem30";
		this.commandBarCheckItem30.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertEmptyRowDataRowSeparator;
		this.commandBarCheckItem31.Id = 282;
		this.commandBarCheckItem31.Name = "commandBarCheckItem31";
		this.commandBarCheckItem31.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.InsertNoneDataRowSeparator;
		this.commandBarItem10.Id = 283;
		this.commandBarItem10.Name = "commandBarItem10";
		this.commandBarItem10.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.FilterList;
		this.commandBarItem11.Id = 284;
		this.commandBarItem11.Name = "commandBarItem11";
		this.commandBarItem11.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.ConvertToParagraphs;
		this.commandBarItem12.Id = 285;
		this.commandBarItem12.Name = "commandBarItem12";
		this.commandBarItem12.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.DeleteList;
		this.barButtonGroup8.Id = 268;
		this.barButtonGroup8.ItemLinks.Add(this.changeEditorRowLimitItem1);
		this.barButtonGroup8.Name = "barButtonGroup8";
		this.barButtonGroup8.Tag = "{97BBE334-159B-44d9-A168-0411957565E8}";
		this.changeEditorRowLimitItem1.Edit = this.repositoryItemEditorRowLimitEdit1;
		this.changeEditorRowLimitItem1.EditWidth = 90;
		this.changeEditorRowLimitItem1.Id = 286;
		this.changeEditorRowLimitItem1.Name = "changeEditorRowLimitItem1";
		this.repositoryItemEditorRowLimitEdit1.AutoHeight = false;
		this.repositoryItemEditorRowLimitEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemEditorRowLimitEdit1.Control = this.snapControl1;
		this.repositoryItemEditorRowLimitEdit1.Name = "repositoryItemEditorRowLimitEdit1";
		this.createBarBaseItem1.DropDownControl = this.commandBarGalleryDropDown1;
		this.createBarBaseItem1.Id = 287;
		this.createBarBaseItem1.Name = "createBarBaseItem1";
		this.commandBarGalleryDropDown1.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown1.Gallery.ColumnCount = 4;
		chartControlCommandGalleryItemGroup2DColumn.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { createBarChartItem, createFullStackedBarChartItem, createSideBySideFullStackedBarChartItem, createSideBySideStackedBarChartItem, createStackedBarChartItem });
		chartControlCommandGalleryItemGroup3DColumn.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { createBar3DChartItem, createFullStackedBar3DChartItem, createManhattanBarChartItem, createSideBySideFullStackedBar3DChartItem, createSideBySideStackedBar3DChartItem, createStackedBar3DChartItem });
		chartControlCommandGalleryItemGroupCylinderColumn.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { createCylinderBar3DChartItem, createCylinderFullStackedBar3DChartItem, createCylinderManhattanBarChartItem, createCylinderSideBySideFullStackedBar3DChartItem, createCylinderSideBySideStackedBar3DChartItem, createCylinderStackedBar3DChartItem });
		chartControlCommandGalleryItemGroupConeColumn.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { createConeBar3DChartItem, createConeFullStackedBar3DChartItem, createConeManhattanBarChartItem, createConeSideBySideFullStackedBar3DChartItem, createConeSideBySideStackedBar3DChartItem, createConeStackedBar3DChartItem });
		chartControlCommandGalleryItemGroupPyramidColumn.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { createPyramidBar3DChartItem, createPyramidFullStackedBar3DChartItem, createPyramidManhattanBarChartItem, createPyramidSideBySideFullStackedBar3DChartItem, createPyramidSideBySideStackedBar3DChartItem, createPyramidStackedBar3DChartItem });
		this.commandBarGalleryDropDown1.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[5] { chartControlCommandGalleryItemGroup2DColumn, chartControlCommandGalleryItemGroup3DColumn, chartControlCommandGalleryItemGroupCylinderColumn, chartControlCommandGalleryItemGroupConeColumn, chartControlCommandGalleryItemGroupPyramidColumn });
		this.commandBarGalleryDropDown1.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown1.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown1.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown1.Name = "commandBarGalleryDropDown1";
		this.commandBarGalleryDropDown1.Ribbon = this.ribbonControl1;
		this.createLineBaseItem1.DropDownControl = this.commandBarGalleryDropDown2;
		this.createLineBaseItem1.Id = 288;
		this.createLineBaseItem1.Name = "createLineBaseItem1";
		this.commandBarGalleryDropDown2.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown2.Gallery.ColumnCount = 3;
		chartControlCommandGalleryItemGroup2DLine.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[6] { createLineChartItem, createFullStackedLineChartItem, createScatterLineChartItem, createSplineChartItem, createStackedLineChartItem, createStepLineChartItem });
		chartControlCommandGalleryItemGroup3DLine.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { createLine3DChartItem, createFullStackedLine3DChartItem, createSpline3DChartItem, createStackedLine3DChartItem, createStepLine3DChartItem });
		this.commandBarGalleryDropDown2.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { chartControlCommandGalleryItemGroup2DLine, chartControlCommandGalleryItemGroup3DLine });
		this.commandBarGalleryDropDown2.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown2.Gallery.RowCount = 4;
		this.commandBarGalleryDropDown2.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown2.Name = "commandBarGalleryDropDown2";
		this.commandBarGalleryDropDown2.Ribbon = this.ribbonControl1;
		this.createPieBaseItem1.DropDownControl = this.commandBarGalleryDropDown3;
		this.createPieBaseItem1.Id = 289;
		this.createPieBaseItem1.Name = "createPieBaseItem1";
		this.commandBarGalleryDropDown3.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown3.Gallery.ColumnCount = 3;
		chartControlCommandGalleryItemGroup2DPie.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[3] { createPieChartItem, createDoughnutChartItem, createNestedDoughnutChartItem });
		chartControlCommandGalleryItemGroup3DPie.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { createPie3DChartItem, createDoughnut3DChartItem });
		this.commandBarGalleryDropDown3.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { chartControlCommandGalleryItemGroup2DPie, chartControlCommandGalleryItemGroup3DPie });
		this.commandBarGalleryDropDown3.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown3.Gallery.RowCount = 2;
		this.commandBarGalleryDropDown3.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown3.Name = "commandBarGalleryDropDown3";
		this.commandBarGalleryDropDown3.Ribbon = this.ribbonControl1;
		this.createRotatedBarBaseItem1.DropDownControl = this.commandBarGalleryDropDown4;
		this.createRotatedBarBaseItem1.Id = 290;
		this.createRotatedBarBaseItem1.Name = "createRotatedBarBaseItem1";
		this.commandBarGalleryDropDown4.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown4.Gallery.ColumnCount = 3;
		chartControlCommandGalleryItemGroup2DBar.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { createRotatedBarChartItem, createRotatedFullStackedBarChartItem, createRotatedSideBySideFullStackedBarChartItem, createRotatedSideBySideStackedBarChartItem, createRotatedStackedBarChartItem });
		this.commandBarGalleryDropDown4.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[1] { chartControlCommandGalleryItemGroup2DBar });
		this.commandBarGalleryDropDown4.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown4.Gallery.RowCount = 2;
		this.commandBarGalleryDropDown4.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown4.Name = "commandBarGalleryDropDown4";
		this.commandBarGalleryDropDown4.Ribbon = this.ribbonControl1;
		this.createAreaBaseItem1.DropDownControl = this.commandBarGalleryDropDown5;
		this.createAreaBaseItem1.Id = 291;
		this.createAreaBaseItem1.Name = "createAreaBaseItem1";
		this.commandBarGalleryDropDown5.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown5.Gallery.ColumnCount = 4;
		chartControlCommandGalleryItemGroup2DArea.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[9] { createAreaChartItem, createFullStackedAreaChartItem, createFullStackedSplineAreaChartItem, createFullStackedStepAreaChartItem, createSplineAreaChartItem, createStackedAreaChartItem, createStackedStepAreaChartItem, createStackedSplineAreaChartItem, createStepAreaChartItem });
		chartControlCommandGalleryItemGroup3DArea.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[7] { createArea3DChartItem, createFullStackedArea3DChartItem, createFullStackedSplineArea3DChartItem, createSplineArea3DChartItem, createStackedArea3DChartItem, createStackedSplineArea3DChartItem, createStepArea3DChartItem });
		this.commandBarGalleryDropDown5.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[2] { chartControlCommandGalleryItemGroup2DArea, chartControlCommandGalleryItemGroup3DArea });
		this.commandBarGalleryDropDown5.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown5.Gallery.RowCount = 4;
		this.commandBarGalleryDropDown5.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown5.Name = "commandBarGalleryDropDown5";
		this.commandBarGalleryDropDown5.Ribbon = this.ribbonControl1;
		this.createOtherSeriesTypesBaseItem1.DropDownControl = this.commandBarGalleryDropDown6;
		this.createOtherSeriesTypesBaseItem1.Id = 292;
		this.createOtherSeriesTypesBaseItem1.Name = "createOtherSeriesTypesBaseItem1";
		this.commandBarGalleryDropDown6.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown6.Gallery.ColumnCount = 4;
		chartControlCommandGalleryItemGroupPoint.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { createPointChartItem, createBubbleChartItem });
		chartControlCommandGalleryItemGroupFunnel.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { createFunnelChartItem, createFunnel3DChartItem });
		chartControlCommandGalleryItemGroupFinancial.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { createStockChartItem, createCandleStickChartItem });
		chartControlCommandGalleryItemGroupRadar.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { createRadarPointChartItem, createRadarLineChartItem, createRadarAreaChartItem, createRadarRangeAreaChartItem, createScatterRadarLineChartItem });
		chartControlCommandGalleryItemGroupPolar.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[5] { createPolarPointChartItem, createPolarLineChartItem, createPolarAreaChartItem, createPolarRangeAreaChartItem, createScatterPolarLineChartItem });
		chartControlCommandGalleryItemGroupRange.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[4] { createRangeBarChartItem, createSideBySideRangeBarChartItem, createRangeAreaChartItem, createRangeArea3DChartItem });
		chartControlCommandGalleryItemGroupGantt.Items.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItem[2] { createGanttChartItem, createSideBySideGanttChartItem });
		this.commandBarGalleryDropDown6.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[7] { chartControlCommandGalleryItemGroupPoint, chartControlCommandGalleryItemGroupFunnel, chartControlCommandGalleryItemGroupFinancial, chartControlCommandGalleryItemGroupRadar, chartControlCommandGalleryItemGroupPolar, chartControlCommandGalleryItemGroupRange, chartControlCommandGalleryItemGroupGantt });
		this.commandBarGalleryDropDown6.Gallery.ImageSize = new System.Drawing.Size(32, 32);
		this.commandBarGalleryDropDown6.Gallery.RowCount = 7;
		this.commandBarGalleryDropDown6.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown6.Name = "commandBarGalleryDropDown6";
		this.commandBarGalleryDropDown6.Ribbon = this.ribbonControl1;
		this.changePaletteGalleryBaseItem1.DropDownControl = this.commandBarGalleryDropDown7;
		this.changePaletteGalleryBaseItem1.Id = 293;
		this.changePaletteGalleryBaseItem1.Name = "changePaletteGalleryBaseItem1";
		this.commandBarGalleryDropDown7.Gallery.AllowFilter = false;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Hovered.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemCaptionAppearance.Pressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Hovered.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Hovered.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Hovered.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Normal.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Normal.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Pressed.Options.UseFont = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Pressed.Options.UseTextOptions = true;
		this.commandBarGalleryDropDown7.Gallery.Appearance.ItemDescriptionAppearance.Pressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.commandBarGalleryDropDown7.Gallery.ColumnCount = 1;
		this.commandBarGalleryDropDown7.Gallery.ImageSize = new System.Drawing.Size(160, 10);
		this.commandBarGalleryDropDown7.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
		this.commandBarGalleryDropDown7.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Right;
		skinPaddingEdges.Bottom = -3;
		skinPaddingEdges.Top = -3;
		this.commandBarGalleryDropDown7.Gallery.ItemImagePadding = skinPaddingEdges;
		skinPaddingEdges2.Bottom = -3;
		skinPaddingEdges2.Top = -3;
		this.commandBarGalleryDropDown7.Gallery.ItemTextPadding = skinPaddingEdges2;
		this.commandBarGalleryDropDown7.Gallery.RowCount = 10;
		this.commandBarGalleryDropDown7.Gallery.ShowGroupCaption = false;
		this.commandBarGalleryDropDown7.Gallery.ShowItemText = true;
		this.commandBarGalleryDropDown7.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
		this.commandBarGalleryDropDown7.Name = "commandBarGalleryDropDown7";
		this.commandBarGalleryDropDown7.Ribbon = this.ribbonControl1;
		this.changeAppearanceGalleryBaseItem1.Gallery.ColumnCount = 7;
		this.changeAppearanceGalleryBaseItem1.Gallery.ImageSize = new System.Drawing.Size(80, 50);
		this.changeAppearanceGalleryBaseItem1.Gallery.RowCount = 4;
		this.changeAppearanceGalleryBaseItem1.Id = 294;
		this.changeAppearanceGalleryBaseItem1.Name = "changeAppearanceGalleryBaseItem1";
		this.runDesignerChartItem1.Id = 295;
		this.runDesignerChartItem1.Name = "runDesignerChartItem1";
		this.mailMergeDataSourceItem1.Id = 296;
		this.mailMergeDataSourceItem1.Name = "mailMergeDataSourceItem1";
		this.commandBarCheckItem32.Id = 297;
		this.commandBarCheckItem32.Name = "commandBarCheckItem32";
		this.commandBarCheckItem32.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.MailMergeFilters;
		this.commandBarCheckItem33.Id = 298;
		this.commandBarCheckItem33.Name = "commandBarCheckItem33";
		this.commandBarCheckItem33.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.MailMergeSorting;
		this.mailMergeCurrentRecordItem1.Description = resources.GetString("mailMergeCurrentRecordItem1.Description");
		this.mailMergeCurrentRecordItem1.Edit = this.repositoryItemMailMergeCurrentRecordEdit1;
		this.mailMergeCurrentRecordItem1.EditWidth = 90;
		this.mailMergeCurrentRecordItem1.Id = 299;
		this.mailMergeCurrentRecordItem1.Name = "mailMergeCurrentRecordItem1";
		this.repositoryItemMailMergeCurrentRecordEdit1.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		editorButtonImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		editorButtonImageOptions2.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		editorButtonImageOptions3.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		editorButtonImageOptions4.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		this.repositoryItemMailMergeCurrentRecordEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[4]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 6, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Previous", DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordEditorButtonTag.Prev),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 6, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Next", DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordEditorButtonTag.Next),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 6, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "First", DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordEditorButtonTag.First),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 6, true, true, false, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Last", DevExpress.Snap.Extensions.UI.MailMergeCurrentRecordEditorButtonTag.Last)
		});
		this.repositoryItemMailMergeCurrentRecordEdit1.Name = "repositoryItemMailMergeCurrentRecordEdit1";
		this.commandBarSubItem8.Id = 300;
		this.commandBarSubItem8.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarItem13),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarItem14),
			new DevExpress.XtraBars.LinkPersistInfo(this.commandBarItem15)
		});
		this.commandBarSubItem8.Name = "commandBarSubItem8";
		this.commandBarSubItem8.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
		this.commandBarSubItem8.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.FinishAndMerge;
		this.commandBarItem13.Id = 301;
		this.commandBarItem13.Name = "commandBarItem13";
		this.commandBarItem13.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SnapMailMergeExportDocument;
		this.commandBarItem14.Id = 302;
		this.commandBarItem14.Name = "commandBarItem14";
		this.commandBarItem14.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SnapMailMergePrint;
		this.commandBarItem15.Id = 303;
		this.commandBarItem15.Name = "commandBarItem15";
		this.commandBarItem15.SnapCommand = DevExpress.Snap.Extensions.UI.SnapCommand.SnapMailMergePrintPreview;
		this.headerFooterToolsRibbonPageCategory1.Color = System.Drawing.Color.FromArgb(38, 176, 35);
		this.headerFooterToolsRibbonPageCategory1.Control = this.snapControl1;
		this.headerFooterToolsRibbonPageCategory1.Name = "headerFooterToolsRibbonPageCategory1";
		this.headerFooterToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.headerFooterToolsDesignRibbonPage1 });
		this.headerFooterToolsDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.headerFooterToolsDesignNavigationRibbonPageGroup1, this.headerFooterToolsDesignOptionsRibbonPageGroup1, this.headerFooterToolsDesignCloseRibbonPageGroup1 });
		this.headerFooterToolsDesignRibbonPage1.Name = "headerFooterToolsDesignRibbonPage1";
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.ItemLinks.Add(this.goToPageHeaderItem1, "E");
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.ItemLinks.Add(this.goToPageFooterItem1, "G");
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.ItemLinks.Add(this.goToNextHeaderFooterItem1, "X");
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.ItemLinks.Add(this.goToPreviousHeaderFooterItem1, "R");
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.ItemLinks.Add(this.toggleLinkToPreviousItem1, "K");
		this.headerFooterToolsDesignNavigationRibbonPageGroup1.Name = "headerFooterToolsDesignNavigationRibbonPageGroup1";
		this.headerFooterToolsDesignOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleDifferentFirstPageItem1, "A");
		this.headerFooterToolsDesignOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleDifferentOddAndEvenPagesItem1, "V");
		this.headerFooterToolsDesignOptionsRibbonPageGroup1.Name = "headerFooterToolsDesignOptionsRibbonPageGroup1";
		this.headerFooterToolsDesignCloseRibbonPageGroup1.ItemLinks.Add(this.closePageHeaderFooterItem1, "C");
		this.headerFooterToolsDesignCloseRibbonPageGroup1.Name = "headerFooterToolsDesignCloseRibbonPageGroup1";
		this.tableToolsRibbonPageCategory1.Color = System.Drawing.Color.FromArgb(252, 233, 20);
		this.tableToolsRibbonPageCategory1.Control = this.snapControl1;
		this.tableToolsRibbonPageCategory1.Name = "tableToolsRibbonPageCategory1";
		this.tableToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[2] { this.tableDesignRibbonPage1, this.tableLayoutRibbonPage1 });
		this.tableDesignRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[4] { this.tableStyleOptionsRibbonPageGroup1, this.tableStylesRibbonPageGroup1, this.tableCellStylesRibbonPageGroup1, this.tableDrawBordersRibbonPageGroup1 });
		this.tableDesignRibbonPage1.Name = "tableDesignRibbonPage1";
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleFirstRowItem1);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleLastRowItem1);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleBandedRowsItem1);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleFirstColumnItem1);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleLastColumnItem1);
		this.tableStyleOptionsRibbonPageGroup1.ItemLinks.Add(this.toggleBandedColumnsItem1);
		this.tableStyleOptionsRibbonPageGroup1.Name = "tableStyleOptionsRibbonPageGroup1";
		this.tableStylesRibbonPageGroup1.ItemLinks.Add(this.galleryChangeTableStyleItem1);
		this.tableStylesRibbonPageGroup1.Name = "tableStylesRibbonPageGroup1";
		this.tableCellStylesRibbonPageGroup1.ItemLinks.Add(this.galleryChangeTableCellStyleItem1);
		this.tableCellStylesRibbonPageGroup1.Name = "tableCellStylesRibbonPageGroup1";
		this.tableDrawBordersRibbonPageGroup1.ItemLinks.Add(this.changeTableBorderLineStyleItem1);
		this.tableDrawBordersRibbonPageGroup1.ItemLinks.Add(this.changeTableBorderLineWeightItem1);
		this.tableDrawBordersRibbonPageGroup1.ItemLinks.Add(this.changeTableBorderColorItem1, "C");
		this.tableDrawBordersRibbonPageGroup1.ItemLinks.Add(this.changeTableBordersItem1, "B");
		this.tableDrawBordersRibbonPageGroup1.ItemLinks.Add(this.changeTableCellsShadingItem1, "H");
		this.tableDrawBordersRibbonPageGroup1.Name = "tableDrawBordersRibbonPageGroup1";
		this.tableLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[5] { this.tableTableRibbonPageGroup1, this.tableRowsAndColumnsRibbonPageGroup1, this.tableMergeRibbonPageGroup1, this.tableCellSizeRibbonPageGroup1, this.tableAlignmentRibbonPageGroup1 });
		this.tableLayoutRibbonPage1.Name = "tableLayoutRibbonPage1";
		this.tableTableRibbonPageGroup1.ItemLinks.Add(this.selectTableElementsItem1, "K");
		this.tableTableRibbonPageGroup1.ItemLinks.Add(this.toggleShowTableGridLinesItem1, "TG");
		this.tableTableRibbonPageGroup1.ItemLinks.Add(this.showTablePropertiesFormItem1, "O");
		this.tableTableRibbonPageGroup1.Name = "tableTableRibbonPageGroup1";
		this.tableRowsAndColumnsRibbonPageGroup1.ItemLinks.Add(this.deleteTableElementsItem1, "D");
		this.tableRowsAndColumnsRibbonPageGroup1.ItemLinks.Add(this.insertTableRowAboveItem1, "A");
		this.tableRowsAndColumnsRibbonPageGroup1.ItemLinks.Add(this.insertTableRowBelowItem1, "E");
		this.tableRowsAndColumnsRibbonPageGroup1.ItemLinks.Add(this.insertTableColumnToLeftItem1, "L");
		this.tableRowsAndColumnsRibbonPageGroup1.ItemLinks.Add(this.insertTableColumnToRightItem1, "R");
		this.tableRowsAndColumnsRibbonPageGroup1.Name = "tableRowsAndColumnsRibbonPageGroup1";
		this.tableMergeRibbonPageGroup1.ItemLinks.Add(this.mergeTableCellsItem1);
		this.tableMergeRibbonPageGroup1.ItemLinks.Add(this.showSplitTableCellsForm1, "P");
		this.tableMergeRibbonPageGroup1.ItemLinks.Add(this.splitTableItem1, "Q");
		this.tableMergeRibbonPageGroup1.Name = "tableMergeRibbonPageGroup1";
		this.tableCellSizeRibbonPageGroup1.AllowTextClipping = false;
		this.tableCellSizeRibbonPageGroup1.ItemLinks.Add(this.toggleTableAutoFitItem1, "F");
		this.tableCellSizeRibbonPageGroup1.Name = "tableCellSizeRibbonPageGroup1";
		this.tableAlignmentRibbonPageGroup1.Glyph = (System.Drawing.Image)resources.GetObject("tableAlignmentRibbonPageGroup1.Glyph");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsTopLeftAlignmentItem1, "TL");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsMiddleLeftAlignmentItem1, "CL");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsBottomLeftAlignmentItem1, "BL");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsTopCenterAlignmentItem1, "TC");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsMiddleCenterAlignmentItem1, "CC");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsBottomCenterAlignmentItem1, "BC");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsTopRightAlignmentItem1, "TR");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsMiddleRightAlignmentItem1, "CR");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.toggleTableCellsBottomRightAlignmentItem1, "BR");
		this.tableAlignmentRibbonPageGroup1.ItemLinks.Add(this.showTableOptionsFormItem1, "N");
		this.tableAlignmentRibbonPageGroup1.Name = "tableAlignmentRibbonPageGroup1";
		this.floatingPictureToolsRibbonPageCategory1.Color = System.Drawing.Color.FromArgb(201, 0, 119);
		this.floatingPictureToolsRibbonPageCategory1.Control = this.snapControl1;
		this.floatingPictureToolsRibbonPageCategory1.Name = "floatingPictureToolsRibbonPageCategory1";
		this.floatingPictureToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.floatingPictureToolsFormatPage1 });
		this.floatingPictureToolsFormatPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.floatingPictureToolsShapeStylesPageGroup1, this.floatingPictureToolsArrangePageGroup1 });
		this.floatingPictureToolsFormatPage1.Name = "floatingPictureToolsFormatPage1";
		this.floatingPictureToolsShapeStylesPageGroup1.ItemLinks.Add(this.changeFloatingObjectFillColorItem1);
		this.floatingPictureToolsShapeStylesPageGroup1.ItemLinks.Add(this.changeFloatingObjectOutlineColorItem1);
		this.floatingPictureToolsShapeStylesPageGroup1.ItemLinks.Add(this.changeFloatingObjectOutlineWeightItem1);
		this.floatingPictureToolsShapeStylesPageGroup1.Name = "floatingPictureToolsShapeStylesPageGroup1";
		this.floatingPictureToolsArrangePageGroup1.ItemLinks.Add(this.changeFloatingObjectTextWrapTypeItem1, "TW");
		this.floatingPictureToolsArrangePageGroup1.ItemLinks.Add(this.changeFloatingObjectAlignmentItem1, "PO");
		this.floatingPictureToolsArrangePageGroup1.ItemLinks.Add(this.floatingObjectBringForwardSubItem1, "AF");
		this.floatingPictureToolsArrangePageGroup1.ItemLinks.Add(this.floatingObjectSendBackwardSubItem1, "AE");
		this.floatingPictureToolsArrangePageGroup1.Name = "floatingPictureToolsArrangePageGroup1";
		this.dataToolsRibbonPageCategory1.Color = System.Drawing.Color.FromArgb(31, 166, 207);
		this.dataToolsRibbonPageCategory1.Control = this.snapControl1;
		this.dataToolsRibbonPageCategory1.Name = "dataToolsRibbonPageCategory1";
		this.dataToolsRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[5] { this.appearanceRibbonPage1, this.snMergeFieldToolsRibbonPage1, this.groupToolsRibbonPage1, this.listToolsRibbonPage1, this.mailMergeRibbonPage1 });
		this.appearanceRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.themesRibbonPageGroup1 });
		this.appearanceRibbonPage1.Name = "appearanceRibbonPage1";
		reduceOperation.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.UntilAvailable;
		reduceOperation.Group = this.themesRibbonPageGroup1;
		reduceOperation.ItemLinkIndex = 0;
		reduceOperation.ItemLinksCount = 0;
		reduceOperation.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
		this.appearanceRibbonPage1.ReduceOperations.Add(reduceOperation);
		this.snMergeFieldToolsRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.dataShapingRibbonPageGroup1, this.snMergeFieldPropertiesRibbonPageGroup1 });
		this.snMergeFieldToolsRibbonPage1.Name = "snMergeFieldToolsRibbonPage1";
		this.dataShapingRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem2);
		this.dataShapingRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem3);
		this.dataShapingRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem4);
		this.dataShapingRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem1);
		this.dataShapingRibbonPageGroup1.ItemLinks.Add(this.filterPopupButtonItem1);
		this.dataShapingRibbonPageGroup1.Name = "dataShapingRibbonPageGroup1";
		this.snMergeFieldPropertiesRibbonPageGroup1.ItemLinks.Add(this.propertiesBarButtonItem1);
		this.snMergeFieldPropertiesRibbonPageGroup1.Name = "snMergeFieldPropertiesRibbonPageGroup1";
		this.groupToolsRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.groupingRibbonPageGroup1 });
		this.groupToolsRibbonPage1.Name = "groupToolsRibbonPage1";
		this.groupingRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem2);
		this.groupingRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem3);
		this.groupingRibbonPageGroup1.ItemLinks.Add(this.commandBarItem8);
		this.groupingRibbonPageGroup1.ItemLinks.Add(this.commandBarItem9);
		this.groupingRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem4);
		this.groupingRibbonPageGroup1.Name = "groupingRibbonPageGroup1";
		this.listToolsRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.listHeaderAndFooterRibbonPageGroup1, this.listCommandsRibbonPageGroup1, this.listEditorRowLimitRibbonPageGroup1 });
		this.listToolsRibbonPage1.Name = "listToolsRibbonPage1";
		this.listHeaderAndFooterRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem5);
		this.listHeaderAndFooterRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem6);
		this.listHeaderAndFooterRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem7);
		this.listHeaderAndFooterRibbonPageGroup1.Name = "listHeaderAndFooterRibbonPageGroup1";
		this.listCommandsRibbonPageGroup1.ItemLinks.Add(this.commandBarItem10);
		this.listCommandsRibbonPageGroup1.ItemLinks.Add(this.commandBarItem11);
		this.listCommandsRibbonPageGroup1.ItemLinks.Add(this.commandBarItem12);
		this.listCommandsRibbonPageGroup1.Name = "listCommandsRibbonPageGroup1";
		this.listEditorRowLimitRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup8);
		this.listEditorRowLimitRibbonPageGroup1.Name = "listEditorRowLimitRibbonPageGroup1";
		this.mailMergeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.mailMergeRibbonPageGroup1, this.mailMergeCurrentRecordRibbonPageGroup1, this.finishAndMergeRibbonPageGroup1 });
		this.mailMergeRibbonPage1.Name = "mailMergeRibbonPage1";
		this.mailMergeRibbonPageGroup1.ItemLinks.Add(this.mailMergeDataSourceItem1);
		this.mailMergeRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem32);
		this.mailMergeRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem33);
		this.mailMergeRibbonPageGroup1.Name = "mailMergeRibbonPageGroup1";
		this.mailMergeCurrentRecordRibbonPageGroup1.ItemLinks.Add(this.mailMergeCurrentRecordItem1);
		this.mailMergeCurrentRecordRibbonPageGroup1.Name = "mailMergeCurrentRecordRibbonPageGroup1";
		this.finishAndMergeRibbonPageGroup1.ItemLinks.Add(this.commandBarSubItem8);
		this.finishAndMergeRibbonPageGroup1.Name = "finishAndMergeRibbonPageGroup1";
		this.chartRibbonPageCategory1.Control = null;
		this.chartRibbonPageCategory1.Name = "chartRibbonPageCategory1";
		this.chartRibbonPageCategory1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.createChartRibbonPage1 });
		this.createChartRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.chartTypeRibbonPageGroup1, this.chartAppearanceRibbonPageGroup1, this.chartWizardRibbonPageGroup1 });
		this.createChartRibbonPage1.Name = "createChartRibbonPage1";
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createBarBaseItem1);
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createLineBaseItem1);
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createPieBaseItem1);
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createRotatedBarBaseItem1);
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createAreaBaseItem1);
		this.chartTypeRibbonPageGroup1.ItemLinks.Add(this.createOtherSeriesTypesBaseItem1);
		this.chartTypeRibbonPageGroup1.Name = "chartTypeRibbonPageGroup1";
		this.chartAppearanceRibbonPageGroup1.ItemLinks.Add(this.changePaletteGalleryBaseItem1);
		this.chartAppearanceRibbonPageGroup1.ItemLinks.Add(this.changeAppearanceGalleryBaseItem1);
		this.chartAppearanceRibbonPageGroup1.Name = "chartAppearanceRibbonPageGroup1";
		this.chartWizardRibbonPageGroup1.AllowMinimize = false;
		this.chartWizardRibbonPageGroup1.ItemLinks.Add(this.runDesignerChartItem1);
		this.chartWizardRibbonPageGroup1.Name = "chartWizardRibbonPageGroup1";
		this.fileRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.commonRibbonPageGroup1, this.infoRibbonPageGroup1, this.dataRibbonPageGroup1 });
		this.fileRibbonPage1.Name = "fileRibbonPage1";
		this.commonRibbonPageGroup1.ItemLinks.Add(this.undoItem1);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.redoItem1);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.fileNewItem1, "N");
		this.commonRibbonPageGroup1.ItemLinks.Add(this.fileOpenItem1, "O");
		this.commonRibbonPageGroup1.ItemLinks.Add(this.fileSaveItem1, "S");
		this.commonRibbonPageGroup1.ItemLinks.Add(this.fileSaveItem1_Custom);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.fileSaveAsItem1, "A");
		this.commonRibbonPageGroup1.ItemLinks.Add(this.commandBarItem1);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.quickPrintItem1);
		this.commonRibbonPageGroup1.ItemLinks.Add(this.printItem1, "P");
		this.commonRibbonPageGroup1.ItemLinks.Add(this.printPreviewItem1);
		this.commonRibbonPageGroup1.Name = "commonRibbonPageGroup1";
		this.infoRibbonPageGroup1.ItemLinks.Add(this.showDocumentPropertiesFormItem1);
		this.infoRibbonPageGroup1.Name = "infoRibbonPageGroup1";
		this.dataRibbonPageGroup1.ItemLinks.Add(this.commandBarItem2);
		this.dataRibbonPageGroup1.Name = "dataRibbonPageGroup1";
		this.homeRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[5] { this.clipboardRibbonPageGroup1, this.fontRibbonPageGroup1, this.paragraphRibbonPageGroup1, this.stylesRibbonPageGroup1, this.editingRibbonPageGroup1 });
		this.homeRibbonPage1.Name = "homeRibbonPage1";
		reduceOperation2.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.UntilAvailable;
		reduceOperation2.Group = this.stylesRibbonPageGroup1;
		reduceOperation2.ItemLinkIndex = 0;
		reduceOperation2.ItemLinksCount = 0;
		reduceOperation2.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
		this.homeRibbonPage1.ReduceOperations.Add(reduceOperation2);
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.pasteItem1, "V");
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.cutItem1, "X");
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.copyItem1, "C");
		this.clipboardRibbonPageGroup1.ItemLinks.Add(this.pasteSpecialItem1);
		this.clipboardRibbonPageGroup1.Name = "clipboardRibbonPageGroup1";
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup1);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup2);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup3);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.changeTextCaseItem1);
		this.fontRibbonPageGroup1.ItemLinks.Add(this.clearFormattingItem1, "E");
		this.fontRibbonPageGroup1.Name = "fontRibbonPageGroup1";
		this.paragraphRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup4);
		this.paragraphRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup5);
		this.paragraphRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup6);
		this.paragraphRibbonPageGroup1.ItemLinks.Add(this.barButtonGroup7);
		this.paragraphRibbonPageGroup1.Name = "paragraphRibbonPageGroup1";
		this.editingRibbonPageGroup1.ItemLinks.Add(this.findItem1, "FD");
		this.editingRibbonPageGroup1.ItemLinks.Add(this.replaceItem1, "R");
		this.editingRibbonPageGroup1.Name = "editingRibbonPageGroup1";
		this.insertRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[8] { this.pagesRibbonPageGroup1, this.tablesRibbonPageGroup1, this.illustrationsRibbonPageGroup1, this.toolboxRibbonPageGroup1, this.linksRibbonPageGroup1, this.headerFooterRibbonPageGroup1, this.textRibbonPageGroup1, this.symbolsRibbonPageGroup1 });
		this.insertRibbonPage1.Name = "insertRibbonPage1";
		this.pagesRibbonPageGroup1.AllowTextClipping = false;
		this.pagesRibbonPageGroup1.ItemLinks.Add(this.insertPageBreakItem21, "B");
		this.pagesRibbonPageGroup1.Name = "pagesRibbonPageGroup1";
		this.tablesRibbonPageGroup1.AllowTextClipping = false;
		this.tablesRibbonPageGroup1.ItemLinks.Add(this.insertTableItem1, "T");
		this.tablesRibbonPageGroup1.Name = "tablesRibbonPageGroup1";
		this.illustrationsRibbonPageGroup1.ItemLinks.Add(this.insertPictureItem1, "P");
		this.illustrationsRibbonPageGroup1.ItemLinks.Add(this.insertFloatingPictureItem1);
		this.illustrationsRibbonPageGroup1.Name = "illustrationsRibbonPageGroup1";
		this.toolboxRibbonPageGroup1.ItemLinks.Add(this.commandBarItem3);
		this.toolboxRibbonPageGroup1.ItemLinks.Add(this.commandBarItem4);
		this.toolboxRibbonPageGroup1.ItemLinks.Add(this.commandBarItem5);
		this.toolboxRibbonPageGroup1.ItemLinks.Add(this.commandBarItem6);
		this.toolboxRibbonPageGroup1.ItemLinks.Add(this.commandBarItem7);
		this.toolboxRibbonPageGroup1.Name = "toolboxRibbonPageGroup1";
		this.linksRibbonPageGroup1.ItemLinks.Add(this.insertBookmarkItem1, "K");
		this.linksRibbonPageGroup1.ItemLinks.Add(this.insertHyperlinkItem1, "I");
		this.linksRibbonPageGroup1.Name = "linksRibbonPageGroup1";
		this.headerFooterRibbonPageGroup1.ItemLinks.Add(this.editPageHeaderItem1, "H");
		this.headerFooterRibbonPageGroup1.ItemLinks.Add(this.editPageFooterItem1, "O");
		this.headerFooterRibbonPageGroup1.ItemLinks.Add(this.insertPageNumberItem1, "NU");
		this.headerFooterRibbonPageGroup1.ItemLinks.Add(this.insertPageCountItem1);
		this.headerFooterRibbonPageGroup1.Name = "headerFooterRibbonPageGroup1";
		this.textRibbonPageGroup1.Glyph = (System.Drawing.Image)resources.GetObject("textRibbonPageGroup1.Glyph");
		this.textRibbonPageGroup1.ItemLinks.Add(this.insertTextBoxItem1, "X");
		this.textRibbonPageGroup1.Name = "textRibbonPageGroup1";
		this.symbolsRibbonPageGroup1.AllowTextClipping = false;
		this.symbolsRibbonPageGroup1.ItemLinks.Add(this.insertSymbolItem1, "U");
		this.symbolsRibbonPageGroup1.Name = "symbolsRibbonPageGroup1";
		this.pageLayoutRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.pageSetupRibbonPageGroup1, this.pageBackgroundRibbonPageGroup1 });
		this.pageLayoutRibbonPage1.Name = "pageLayoutRibbonPage1";
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.changeSectionPageMarginsItem1, "M");
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.changeSectionPageOrientationItem1, "O");
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.changeSectionPaperKindItem1, "SZ");
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.changeSectionColumnsItem1, "J");
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.insertBreakItem1, "B");
		this.pageSetupRibbonPageGroup1.ItemLinks.Add(this.changeSectionLineNumberingItem1, "LN");
		this.pageSetupRibbonPageGroup1.Name = "pageSetupRibbonPageGroup1";
		this.pageBackgroundRibbonPageGroup1.AllowTextClipping = false;
		this.pageBackgroundRibbonPageGroup1.ItemLinks.Add(this.changePageColorItem1, "PC");
		this.pageBackgroundRibbonPageGroup1.Name = "pageBackgroundRibbonPageGroup1";
		this.viewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[5] { this.documentViewsRibbonPageGroup1, this.showRibbonPageGroup1, this.zoomRibbonPageGroup1, this.viewRibbonPageGroup1, this.viewFieldsRibbonPageGroup1 });
		this.viewRibbonPage1.Name = "viewRibbonPage1";
		this.documentViewsRibbonPageGroup1.ItemLinks.Add(this.switchToSimpleViewItem1, "L");
		this.documentViewsRibbonPageGroup1.ItemLinks.Add(this.switchToDraftViewItem1, "E");
		this.documentViewsRibbonPageGroup1.ItemLinks.Add(this.switchToPrintLayoutViewItem1, "P");
		this.documentViewsRibbonPageGroup1.Name = "documentViewsRibbonPageGroup1";
		this.showRibbonPageGroup1.ItemLinks.Add(this.toggleShowHorizontalRulerItem1);
		this.showRibbonPageGroup1.ItemLinks.Add(this.toggleShowVerticalRulerItem1);
		this.showRibbonPageGroup1.Name = "showRibbonPageGroup1";
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.zoomOutItem1);
		this.zoomRibbonPageGroup1.ItemLinks.Add(this.zoomInItem1);
		this.zoomRibbonPageGroup1.Name = "zoomRibbonPageGroup1";
		this.viewRibbonPageGroup1.ItemLinks.Add(this.snapBarToolbarsListItem1);
		this.viewRibbonPageGroup1.ItemLinks.Add(this.commandBarCheckItem1);
		this.viewRibbonPageGroup1.Name = "viewRibbonPageGroup1";
		this.viewFieldsRibbonPageGroup1.ItemLinks.Add(this.showAllFieldCodesItem1);
		this.viewFieldsRibbonPageGroup1.ItemLinks.Add(this.showAllFieldResultsItem1);
		this.viewFieldsRibbonPageGroup1.ItemLinks.Add(this.toggleFieldHighlightingItem1);
		this.viewFieldsRibbonPageGroup1.Name = "viewFieldsRibbonPageGroup1";
		this.reviewRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.documentProofingRibbonPageGroup1 });
		this.reviewRibbonPage1.Name = "reviewRibbonPage1";
		this.documentProofingRibbonPageGroup1.ItemLinks.Add(this.checkSpellingItem1, "S");
		this.documentProofingRibbonPageGroup1.Name = "documentProofingRibbonPageGroup1";
		this.referencesRibbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.tableOfContentsRibbonPageGroup1, this.captionsRibbonPageGroup1 });
		this.referencesRibbonPage1.Name = "referencesRibbonPage1";
		this.tableOfContentsRibbonPageGroup1.ItemLinks.Add(this.insertTableOfContentsItem1, "T");
		this.tableOfContentsRibbonPageGroup1.ItemLinks.Add(this.updateTableOfContentsItem1, "U");
		this.tableOfContentsRibbonPageGroup1.ItemLinks.Add(this.addParagraphsToTableOfContentItem1, "A");
		this.tableOfContentsRibbonPageGroup1.Name = "tableOfContentsRibbonPageGroup1";
		this.captionsRibbonPageGroup1.ItemLinks.Add(this.insertCaptionPlaceholderItem1, "C");
		this.captionsRibbonPageGroup1.ItemLinks.Add(this.insertTableOfFiguresPlaceholderItem1, "G");
		this.captionsRibbonPageGroup1.ItemLinks.Add(this.updateTableOfFiguresItem1, "U");
		this.captionsRibbonPageGroup1.Name = "captionsRibbonPageGroup1";
		this.snapDockManager1.Form = this;
		this.snapDockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[1] { this.panelContainer1 });
		this.snapDockManager1.SnapControl = this.snapControl1;
		this.snapDockManager1.TopZIndexControls.AddRange(new string[10] { "DevExpress.XtraBars.BarDockControl", "DevExpress.XtraBars.StandaloneBarDockControl", "System.Windows.Forms.StatusBar", "System.Windows.Forms.MenuStrip", "System.Windows.Forms.StatusStrip", "DevExpress.XtraBars.Ribbon.RibbonStatusBar", "DevExpress.XtraBars.Ribbon.RibbonControl", "DevExpress.XtraBars.Navigation.OfficeNavigationBar", "DevExpress.XtraBars.Navigation.TileNavPane", "DevExpress.XtraBars.TabFormControl" });
		this.panelContainer1.Controls.Add(this.fieldListDockPanel1);
		this.panelContainer1.Controls.Add(this.reportExplorerDockPanel1);
		this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
		this.panelContainer1.ID = new System.Guid("b136f17a-61ce-49e8-ad54-2aceddfdb5d3");
		this.panelContainer1.Location = new System.Drawing.Point(793, 141);
		this.panelContainer1.Name = "panelContainer1";
		this.panelContainer1.OriginalSize = new System.Drawing.Size(200, 200);
		this.panelContainer1.Size = new System.Drawing.Size(200, 549);
		this.panelContainer1.Text = "panelContainer1";
		this.fieldListDockPanel1.Controls.Add(this.fieldListDockPanel1_Container);
		this.fieldListDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.fieldListDockPanel1.ID = new System.Guid("b3f4c208-90c7-4ef2-8978-8fc4bf8def19");
		this.fieldListDockPanel1.Location = new System.Drawing.Point(0, 0);
		this.fieldListDockPanel1.Name = "fieldListDockPanel1";
		this.fieldListDockPanel1.OriginalSize = new System.Drawing.Size(200, 303);
		this.fieldListDockPanel1.Size = new System.Drawing.Size(200, 275);
		this.fieldListDockPanel1.SnapControl = this.snapControl1;
		this.fieldListDockPanel1_Container.Location = new System.Drawing.Point(5, 23);
		this.fieldListDockPanel1_Container.Name = "fieldListDockPanel1_Container";
		this.fieldListDockPanel1_Container.Size = new System.Drawing.Size(191, 247);
		this.fieldListDockPanel1_Container.TabIndex = 0;
		this.reportExplorerDockPanel1.Controls.Add(this.reportExplorerDockPanel1_Container);
		this.reportExplorerDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
		this.reportExplorerDockPanel1.ID = new System.Guid("65dfadb5-eb88-4648-8b2e-ac0c2bbc4db6");
		this.reportExplorerDockPanel1.Location = new System.Drawing.Point(0, 275);
		this.reportExplorerDockPanel1.Name = "reportExplorerDockPanel1";
		this.reportExplorerDockPanel1.OriginalSize = new System.Drawing.Size(200, 303);
		this.reportExplorerDockPanel1.Size = new System.Drawing.Size(200, 274);
		this.reportExplorerDockPanel1.SnapControl = this.snapControl1;
		this.reportExplorerDockPanel1_Container.Location = new System.Drawing.Point(5, 23);
		this.reportExplorerDockPanel1_Container.Name = "reportExplorerDockPanel1_Container";
		this.reportExplorerDockPanel1_Container.Size = new System.Drawing.Size(191, 247);
		this.reportExplorerDockPanel1_Container.TabIndex = 0;
		this.snapDocumentManager1.ClientControl = this.snapControl1;
		this.snapDocumentManager1.View = this.noDocumentsView1;
		this.snapDocumentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[1] { this.noDocumentsView1 });
		this.snapBarController1.BarItems.Add(this.undoItem1);
		this.snapBarController1.BarItems.Add(this.redoItem1);
		this.snapBarController1.BarItems.Add(this.fileNewItem1);
		this.snapBarController1.BarItems.Add(this.fileOpenItem1);
		this.snapBarController1.BarItems.Add(this.fileSaveItem1);
		this.snapBarController1.BarItems.Add(this.fileSaveAsItem1);
		this.snapBarController1.BarItems.Add(this.commandBarItem1);
		this.snapBarController1.BarItems.Add(this.quickPrintItem1);
		this.snapBarController1.BarItems.Add(this.printItem1);
		this.snapBarController1.BarItems.Add(this.printPreviewItem1);
		this.snapBarController1.BarItems.Add(this.showDocumentPropertiesFormItem1);
		this.snapBarController1.BarItems.Add(this.commandBarItem2);
		this.snapBarController1.BarItems.Add(this.pasteItem1);
		this.snapBarController1.BarItems.Add(this.cutItem1);
		this.snapBarController1.BarItems.Add(this.copyItem1);
		this.snapBarController1.BarItems.Add(this.pasteSpecialItem1);
		this.snapBarController1.BarItems.Add(this.changeFontNameItem1);
		this.snapBarController1.BarItems.Add(this.changeFontSizeItem1);
		this.snapBarController1.BarItems.Add(this.fontSizeIncreaseItem1);
		this.snapBarController1.BarItems.Add(this.fontSizeDecreaseItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontBoldItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontItalicItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontUnderlineItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontDoubleUnderlineItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontStrikeoutItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontDoubleStrikeoutItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontSuperscriptItem1);
		this.snapBarController1.BarItems.Add(this.toggleFontSubscriptItem1);
		this.snapBarController1.BarItems.Add(this.changeFontColorItem1);
		this.snapBarController1.BarItems.Add(this.changeFontHighlightColorItem1);
		this.snapBarController1.BarItems.Add(this.changeTextCaseItem1);
		this.snapBarController1.BarItems.Add(this.makeTextUpperCaseItem1);
		this.snapBarController1.BarItems.Add(this.makeTextLowerCaseItem1);
		this.snapBarController1.BarItems.Add(this.capitalizeEachWordCaseItem1);
		this.snapBarController1.BarItems.Add(this.toggleTextCaseItem1);
		this.snapBarController1.BarItems.Add(this.clearFormattingItem1);
		this.snapBarController1.BarItems.Add(this.toggleBulletedListItem1);
		this.snapBarController1.BarItems.Add(this.toggleNumberingListItem1);
		this.snapBarController1.BarItems.Add(this.toggleMultiLevelListItem1);
		this.snapBarController1.BarItems.Add(this.decreaseIndentItem1);
		this.snapBarController1.BarItems.Add(this.increaseIndentItem1);
		this.snapBarController1.BarItems.Add(this.toggleParagraphAlignmentLeftItem1);
		this.snapBarController1.BarItems.Add(this.toggleParagraphAlignmentCenterItem1);
		this.snapBarController1.BarItems.Add(this.toggleParagraphAlignmentRightItem1);
		this.snapBarController1.BarItems.Add(this.toggleParagraphAlignmentJustifyItem1);
		this.snapBarController1.BarItems.Add(this.toggleShowWhitespaceItem1);
		this.snapBarController1.BarItems.Add(this.changeParagraphLineSpacingItem1);
		this.snapBarController1.BarItems.Add(this.setSingleParagraphSpacingItem1);
		this.snapBarController1.BarItems.Add(this.setSesquialteralParagraphSpacingItem1);
		this.snapBarController1.BarItems.Add(this.setDoubleParagraphSpacingItem1);
		this.snapBarController1.BarItems.Add(this.showLineSpacingFormItem1);
		this.snapBarController1.BarItems.Add(this.addSpacingBeforeParagraphItem1);
		this.snapBarController1.BarItems.Add(this.removeSpacingBeforeParagraphItem1);
		this.snapBarController1.BarItems.Add(this.addSpacingAfterParagraphItem1);
		this.snapBarController1.BarItems.Add(this.removeSpacingAfterParagraphItem1);
		this.snapBarController1.BarItems.Add(this.changeParagraphBackColorItem1);
		this.snapBarController1.BarItems.Add(this.galleryChangeStyleItem1);
		this.snapBarController1.BarItems.Add(this.findItem1);
		this.snapBarController1.BarItems.Add(this.replaceItem1);
		this.snapBarController1.BarItems.Add(this.insertPageBreakItem21);
		this.snapBarController1.BarItems.Add(this.insertTableItem1);
		this.snapBarController1.BarItems.Add(this.insertPictureItem1);
		this.snapBarController1.BarItems.Add(this.insertFloatingPictureItem1);
		this.snapBarController1.BarItems.Add(this.commandBarItem3);
		this.snapBarController1.BarItems.Add(this.commandBarItem4);
		this.snapBarController1.BarItems.Add(this.commandBarItem5);
		this.snapBarController1.BarItems.Add(this.commandBarItem6);
		this.snapBarController1.BarItems.Add(this.commandBarItem7);
		this.snapBarController1.BarItems.Add(this.insertBookmarkItem1);
		this.snapBarController1.BarItems.Add(this.insertHyperlinkItem1);
		this.snapBarController1.BarItems.Add(this.editPageHeaderItem1);
		this.snapBarController1.BarItems.Add(this.editPageFooterItem1);
		this.snapBarController1.BarItems.Add(this.insertPageNumberItem1);
		this.snapBarController1.BarItems.Add(this.insertPageCountItem1);
		this.snapBarController1.BarItems.Add(this.insertTextBoxItem1);
		this.snapBarController1.BarItems.Add(this.insertSymbolItem1);
		this.snapBarController1.BarItems.Add(this.changeSectionPageMarginsItem1);
		this.snapBarController1.BarItems.Add(this.setNormalSectionPageMarginsItem1);
		this.snapBarController1.BarItems.Add(this.setNarrowSectionPageMarginsItem1);
		this.snapBarController1.BarItems.Add(this.setModerateSectionPageMarginsItem1);
		this.snapBarController1.BarItems.Add(this.setWideSectionPageMarginsItem1);
		this.snapBarController1.BarItems.Add(this.showPageMarginsSetupFormItem1);
		this.snapBarController1.BarItems.Add(this.changeSectionPageOrientationItem1);
		this.snapBarController1.BarItems.Add(this.setPortraitPageOrientationItem1);
		this.snapBarController1.BarItems.Add(this.setLandscapePageOrientationItem1);
		this.snapBarController1.BarItems.Add(this.changeSectionPaperKindItem1);
		this.snapBarController1.BarItems.Add(this.changeSectionColumnsItem1);
		this.snapBarController1.BarItems.Add(this.setSectionOneColumnItem1);
		this.snapBarController1.BarItems.Add(this.setSectionTwoColumnsItem1);
		this.snapBarController1.BarItems.Add(this.setSectionThreeColumnsItem1);
		this.snapBarController1.BarItems.Add(this.showColumnsSetupFormItem1);
		this.snapBarController1.BarItems.Add(this.insertBreakItem1);
		this.snapBarController1.BarItems.Add(this.insertPageBreakItem1);
		this.snapBarController1.BarItems.Add(this.insertColumnBreakItem1);
		this.snapBarController1.BarItems.Add(this.insertSectionBreakNextPageItem1);
		this.snapBarController1.BarItems.Add(this.insertSectionBreakEvenPageItem1);
		this.snapBarController1.BarItems.Add(this.insertSectionBreakOddPageItem1);
		this.snapBarController1.BarItems.Add(this.changeSectionLineNumberingItem1);
		this.snapBarController1.BarItems.Add(this.setSectionLineNumberingNoneItem1);
		this.snapBarController1.BarItems.Add(this.setSectionLineNumberingContinuousItem1);
		this.snapBarController1.BarItems.Add(this.setSectionLineNumberingRestartNewPageItem1);
		this.snapBarController1.BarItems.Add(this.setSectionLineNumberingRestartNewSectionItem1);
		this.snapBarController1.BarItems.Add(this.toggleParagraphSuppressLineNumbersItem1);
		this.snapBarController1.BarItems.Add(this.showLineNumberingFormItem1);
		this.snapBarController1.BarItems.Add(this.changePageColorItem1);
		this.snapBarController1.BarItems.Add(this.switchToSimpleViewItem1);
		this.snapBarController1.BarItems.Add(this.switchToDraftViewItem1);
		this.snapBarController1.BarItems.Add(this.switchToPrintLayoutViewItem1);
		this.snapBarController1.BarItems.Add(this.toggleShowHorizontalRulerItem1);
		this.snapBarController1.BarItems.Add(this.toggleShowVerticalRulerItem1);
		this.snapBarController1.BarItems.Add(this.zoomOutItem1);
		this.snapBarController1.BarItems.Add(this.zoomInItem1);
		this.snapBarController1.BarItems.Add(this.snapBarToolbarsListItem1);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem1);
		this.snapBarController1.BarItems.Add(this.showAllFieldCodesItem1);
		this.snapBarController1.BarItems.Add(this.showAllFieldResultsItem1);
		this.snapBarController1.BarItems.Add(this.toggleFieldHighlightingItem1);
		this.snapBarController1.BarItems.Add(this.goToPageHeaderItem1);
		this.snapBarController1.BarItems.Add(this.goToPageFooterItem1);
		this.snapBarController1.BarItems.Add(this.goToNextHeaderFooterItem1);
		this.snapBarController1.BarItems.Add(this.goToPreviousHeaderFooterItem1);
		this.snapBarController1.BarItems.Add(this.toggleLinkToPreviousItem1);
		this.snapBarController1.BarItems.Add(this.toggleDifferentFirstPageItem1);
		this.snapBarController1.BarItems.Add(this.toggleDifferentOddAndEvenPagesItem1);
		this.snapBarController1.BarItems.Add(this.closePageHeaderFooterItem1);
		this.snapBarController1.BarItems.Add(this.toggleFirstRowItem1);
		this.snapBarController1.BarItems.Add(this.toggleLastRowItem1);
		this.snapBarController1.BarItems.Add(this.toggleBandedRowsItem1);
		this.snapBarController1.BarItems.Add(this.toggleFirstColumnItem1);
		this.snapBarController1.BarItems.Add(this.toggleLastColumnItem1);
		this.snapBarController1.BarItems.Add(this.toggleBandedColumnsItem1);
		this.snapBarController1.BarItems.Add(this.galleryChangeTableStyleItem1);
		this.snapBarController1.BarItems.Add(this.galleryChangeTableCellStyleItem1);
		this.snapBarController1.BarItems.Add(this.changeTableBorderLineStyleItem1);
		this.snapBarController1.BarItems.Add(this.changeTableBorderLineWeightItem1);
		this.snapBarController1.BarItems.Add(this.changeTableBorderColorItem1);
		this.snapBarController1.BarItems.Add(this.changeTableBordersItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsBottomBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsTopBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsLeftBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsRightBorderItem1);
		this.snapBarController1.BarItems.Add(this.resetTableCellsAllBordersItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsAllBordersItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsOutsideBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsInsideBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsInsideHorizontalBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsInsideVerticalBorderItem1);
		this.snapBarController1.BarItems.Add(this.toggleShowTableGridLinesItem1);
		this.snapBarController1.BarItems.Add(this.changeTableCellsShadingItem1);
		this.snapBarController1.BarItems.Add(this.selectTableElementsItem1);
		this.snapBarController1.BarItems.Add(this.selectTableCellItem1);
		this.snapBarController1.BarItems.Add(this.selectTableColumnItem1);
		this.snapBarController1.BarItems.Add(this.selectTableRowItem1);
		this.snapBarController1.BarItems.Add(this.selectTableItem1);
		this.snapBarController1.BarItems.Add(this.showTablePropertiesFormItem1);
		this.snapBarController1.BarItems.Add(this.deleteTableElementsItem1);
		this.snapBarController1.BarItems.Add(this.showDeleteTableCellsFormItem1);
		this.snapBarController1.BarItems.Add(this.deleteTableColumnsItem1);
		this.snapBarController1.BarItems.Add(this.deleteTableRowsItem1);
		this.snapBarController1.BarItems.Add(this.deleteTableItem1);
		this.snapBarController1.BarItems.Add(this.insertTableRowAboveItem1);
		this.snapBarController1.BarItems.Add(this.insertTableRowBelowItem1);
		this.snapBarController1.BarItems.Add(this.insertTableColumnToLeftItem1);
		this.snapBarController1.BarItems.Add(this.insertTableColumnToRightItem1);
		this.snapBarController1.BarItems.Add(this.mergeTableCellsItem1);
		this.snapBarController1.BarItems.Add(this.showSplitTableCellsForm1);
		this.snapBarController1.BarItems.Add(this.splitTableItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableAutoFitItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableAutoFitContentsItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableAutoFitWindowItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableFixedColumnWidthItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsTopLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsMiddleLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsBottomLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsTopCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsMiddleCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsBottomCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsTopRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsMiddleRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.toggleTableCellsBottomRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.showTableOptionsFormItem1);
		this.snapBarController1.BarItems.Add(this.checkSpellingItem1);
		this.snapBarController1.BarItems.Add(this.insertTableOfContentsItem1);
		this.snapBarController1.BarItems.Add(this.updateTableOfContentsItem1);
		this.snapBarController1.BarItems.Add(this.addParagraphsToTableOfContentItem1);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem1);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem2);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem3);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem4);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem5);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem6);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem7);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem8);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem9);
		this.snapBarController1.BarItems.Add(this.setParagraphHeadingLevelItem10);
		this.snapBarController1.BarItems.Add(this.insertCaptionPlaceholderItem1);
		this.snapBarController1.BarItems.Add(this.insertFiguresCaptionItems1);
		this.snapBarController1.BarItems.Add(this.insertTablesCaptionItems1);
		this.snapBarController1.BarItems.Add(this.insertEquationsCaptionItems1);
		this.snapBarController1.BarItems.Add(this.insertTableOfFiguresPlaceholderItem1);
		this.snapBarController1.BarItems.Add(this.insertTableOfFiguresItems1);
		this.snapBarController1.BarItems.Add(this.insertTableOfTablesItems1);
		this.snapBarController1.BarItems.Add(this.insertTableOfEquationsItems1);
		this.snapBarController1.BarItems.Add(this.updateTableOfFiguresItem1);
		this.snapBarController1.BarItems.Add(this.changeFloatingObjectFillColorItem1);
		this.snapBarController1.BarItems.Add(this.changeFloatingObjectOutlineColorItem1);
		this.snapBarController1.BarItems.Add(this.changeFloatingObjectOutlineWeightItem1);
		this.snapBarController1.BarItems.Add(this.changeFloatingObjectTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectSquareTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectTightTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectThroughTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectTopAndBottomTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectBehindTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectInFrontOfTextWrapTypeItem1);
		this.snapBarController1.BarItems.Add(this.changeFloatingObjectAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectTopLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectTopCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectTopRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectMiddleLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectMiddleCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectMiddleRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectBottomLeftAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectBottomCenterAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.setFloatingObjectBottomRightAlignmentItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectBringForwardSubItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectBringForwardItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectBringToFrontItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectBringInFrontOfTextItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectSendBackwardSubItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectSendBackwardItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectSendToBackItem1);
		this.snapBarController1.BarItems.Add(this.floatingObjectSendBehindTextItem1);
		this.snapBarController1.BarItems.Add(this.themesGalleryBarItem1);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem2);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem3);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem4);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem1);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem5);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem6);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem7);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem8);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem9);
		this.snapBarController1.BarItems.Add(this.filterPopupButtonItem1);
		this.snapBarController1.BarItems.Add(this.propertiesBarButtonItem1);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem2);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem10);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem11);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem3);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem12);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem13);
		this.snapBarController1.BarItems.Add(this.commandBarItem8);
		this.snapBarController1.BarItems.Add(this.commandBarItem9);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem4);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem14);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem15);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem16);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem17);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem18);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem19);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem20);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem5);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem21);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem22);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem6);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem23);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem24);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem7);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem25);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem26);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem27);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem28);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem29);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem30);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem31);
		this.snapBarController1.BarItems.Add(this.commandBarItem10);
		this.snapBarController1.BarItems.Add(this.commandBarItem11);
		this.snapBarController1.BarItems.Add(this.commandBarItem12);
		this.snapBarController1.BarItems.Add(this.changeEditorRowLimitItem1);
		this.snapBarController1.BarItems.Add(this.createBarBaseItem1);
		this.snapBarController1.BarItems.Add(this.createLineBaseItem1);
		this.snapBarController1.BarItems.Add(this.createPieBaseItem1);
		this.snapBarController1.BarItems.Add(this.createRotatedBarBaseItem1);
		this.snapBarController1.BarItems.Add(this.createAreaBaseItem1);
		this.snapBarController1.BarItems.Add(this.createOtherSeriesTypesBaseItem1);
		this.snapBarController1.BarItems.Add(this.changePaletteGalleryBaseItem1);
		this.snapBarController1.BarItems.Add(this.changeAppearanceGalleryBaseItem1);
		this.snapBarController1.BarItems.Add(this.runDesignerChartItem1);
		this.snapBarController1.BarItems.Add(this.mailMergeDataSourceItem1);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem32);
		this.snapBarController1.BarItems.Add(this.commandBarCheckItem33);
		this.snapBarController1.BarItems.Add(this.mailMergeCurrentRecordItem1);
		this.snapBarController1.BarItems.Add(this.commandBarSubItem8);
		this.snapBarController1.BarItems.Add(this.commandBarItem13);
		this.snapBarController1.BarItems.Add(this.commandBarItem14);
		this.snapBarController1.BarItems.Add(this.commandBarItem15);
		this.snapBarController1.ContextPageCategories.Add(new DevExpress.Snap.Extensions.UI.ContextRibbonPageCategoryItem(this.headerFooterToolsRibbonPageCategory1));
		this.snapBarController1.ContextPageCategories.Add(new DevExpress.Snap.Extensions.UI.ContextRibbonPageCategoryItem(this.tableToolsRibbonPageCategory1));
		this.snapBarController1.ContextPageCategories.Add(new DevExpress.Snap.Extensions.UI.ContextRibbonPageCategoryItem(this.floatingPictureToolsRibbonPageCategory1));
		this.snapBarController1.ContextPageCategories.Add(new DevExpress.Snap.Extensions.UI.ContextRibbonPageCategoryItem(this.dataToolsRibbonPageCategory1));
		this.snapBarController1.ContextPageCategories.Add(new DevExpress.Snap.Extensions.UI.ContextRibbonPageCategoryItem(this.chartRibbonPageCategory1));
		this.snapBarController1.Control = this.snapControl1;
		this.snapBarController1.RibbonControl = this.ribbonControl1;
		this.snapBarController1.SnapDockManager = this.snapDockManager1;
		this.fileSaveItem1_Custom.Caption = "Save";
		this.fileSaveItem1_Custom.Id = 304;
		this.fileSaveItem1_Custom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("fileSaveItem1_Custom.ImageOptions.Image");
		this.fileSaveItem1_Custom.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("fileSaveItem1_Custom.ImageOptions.LargeImage");
		this.fileSaveItem1_Custom.Name = "fileSaveItem1_Custom";
		this.fileSaveItem1_Custom.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.fileSaveItem1_Custom_ItemClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(993, 690);
		base.Controls.Add(this.snapControl1);
		base.Controls.Add(this.panelContainer1);
		base.Controls.Add(this.ribbonControl1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_ReportWriter";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Report Writer";
		base.Load += new System.EventHandler(this.frm_ReportWriter_Load);
		((System.ComponentModel.ISupportInitialize)this.ribbonControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemRichEditFontSizeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineStyle1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemBorderLineWeight1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFloatingObjectOutlineWeight1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemEditorRowLimitEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commandBarGalleryDropDown7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMailMergeCurrentRecordEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snapDockManager1).EndInit();
		this.panelContainer1.ResumeLayout(false);
		this.fieldListDockPanel1.ResumeLayout(false);
		this.reportExplorerDockPanel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.snapDocumentManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noDocumentsView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snapBarController1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
