

CREATE PROCEDURE APZ_GetLineDitributeRule(in keyCode nvarchar(255) default '')
as
begin
	SELECT A."OcrCode", A."PrcCode", B."PrcName", A."PrcAmount", A."OcrTotal", A."Direct", A."UserSign", A."ValidFrom", A."ValidTo", A."logInstanc", A."UserSign2", A."updateDate" 
	FROM OCR1 A 
	LEFT OUTER JOIN OPRC B ON A."PrcCode" = B."PrcCode" WHERE A."OcrCode" = :keyCode
	AND LOCATE(A."PrcCode", 'Centr_z') <= 0;
end;




