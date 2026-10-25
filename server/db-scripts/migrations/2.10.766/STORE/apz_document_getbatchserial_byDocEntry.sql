
CALL APZ_EXISTSPROC('apz_document_getbatchserial_byDocEntry');

GO

CREATE  PROCEDURE apz_document_getbatchserial_byDocEntry
(
	in DocEntry VARCHAR(5000),
	in ObjType VARCHAR(20)
)
AS
BEGIN
	call apz_fun_getbatchserial_byDocEntry(:docEntry, :objType, batchSerial);
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
	union all
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";
END