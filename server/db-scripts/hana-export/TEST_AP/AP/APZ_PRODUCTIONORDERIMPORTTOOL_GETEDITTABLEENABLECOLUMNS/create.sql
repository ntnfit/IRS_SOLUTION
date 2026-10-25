
CREATE PROCEDURE APZ_ProductionOrderImportTool_GetEditTableEnableColumns
AS
BEGIN
	SELECT
	'DueDate,U_PlnCode,U_LotNo' "allowEditColWhenUpdateFG",
	'DfltWH,RequestQty' "allowEditTR",
	'CmpQuantiy,RjQuantity,PostingDate' "allowEditRC",
	'CloseDate' "allowEditCL",
	'DocNum,DfltWH,RequestQty,DocItemCode,ItemCode' "allowShowingEditorTR",
	'ItemCode,DocNum,DueDate,U_PlnCode,U_LotNo' "allowShowingEditorFG",
	'ItemCode,DocNum,DocItemCode' "allowShowingEditorCP",
	'ItemCode,DocNum,CloseDate' "allowShowingEditorCL",
	'ItemCode,DocNum,CmpQuantiy,RjQuantity,PostingDate' "allowShowingEditorRC" from DUMMY;
END;



