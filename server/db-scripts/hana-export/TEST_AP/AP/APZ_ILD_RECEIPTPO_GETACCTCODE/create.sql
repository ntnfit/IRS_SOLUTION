create procedure APZ_ILD_ReceiptPO_GetAcctCode(in baseEntry int, baseLine int)
as
begin
	declare acctCode nvarchar(50);
	
	select "WipActCode" into acctCode default null from WOR1 where "DocEntry"=:baseEntry and "LineNum"=:baseLine;
	if ifnull(:acctCode,'')<>'' then
		select :acctCode from dummy;
		return;
	end if;
	
	select case when b."GLMethod"='C' then (select "WipAcct" from OITB where "ItmsGrpCod"=b."ItmsGrpCod")
			when b."GLMethod"='W' then (select "WipAcct" from OWHS where "WhsCode"=a."wareHouse")
			else '' end "AcctCode"
	from WOR1 a join OITM b on a."DocEntry"=:baseEntry and a."LineNum"=:baseLine and a."ItemCode"=b."ItemCode";
end

