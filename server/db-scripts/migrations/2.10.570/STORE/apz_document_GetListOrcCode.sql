
call APZ_EXISTSPROC('apz_document_GetListOrcCode');

GO

Create Procedure apz_document_GetListOrcCode(in DimCode NVARCHAR(10), DocDate datetime default null)
As
Begin
	select distinct a.* from oocr a join ocr1 b on a."OcrCode"=b."OcrCode"
	where a."DimCode"=:DimCode and a."Active"='Y' and (:DocDate is null or :DocDate between ifnull(b."ValidFrom", :DocDate) and ifnull(b."ValidTo", :DocDate));
End;