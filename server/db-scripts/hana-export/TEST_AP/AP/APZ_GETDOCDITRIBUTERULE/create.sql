

CREATE PROCEDURE APZ_GetDocDitributeRule
as
begin
	select a.*,a."ValidFrom",a."ValidTo" from OCR1 a
	inner join OOCR b on a."OcrCode"=b."OcrCode"
	limit 1;
end;


	




