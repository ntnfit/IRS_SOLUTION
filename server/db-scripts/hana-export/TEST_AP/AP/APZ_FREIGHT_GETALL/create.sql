
CREATE  procedure APZ_Freight_GetAll
as
begin
	SELECT "ExpnsCode", "ExpnsName", "RevAcct", "ExpnsAcct", "VatGroupO", "VatGroupI", "RevFixSum", "ExpFixSum", "WTLiable", "DistrbMthd", "BaseMethod", "Stock", "LstPchPrce", "OcrCode", "OcrCode2", "Project" 
	FROM OEXD;
end



