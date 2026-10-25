Create FUNCTION ILD_FT_SinhMaHopDong(sothutu int) 
 RETURNS result_ContractNo  nvarchar(20) 
 LANGUAGE SQLSCRIPT READS SQL DATA AS
 BEGIN
     --- ILD+Nam+###
 
    declare prefix nvarchar(20) = 'ILD'||to_varchar(year(CURRENT_DATE));
	declare codeNo int;
	--select to_varchar("DocDate",'yy/MM') into prefix from "ODLN" where "DocEntry"=list_of_cols_val_tab_del;
	
	select count(*) into codeNo from "APZ_OHEC" where  "code" like 'ILD'|| to_varchar(year(CURRENT_DATE))||'-%';
	
	 result_ContractNo := prefix  ||  '-'||REPLICATE(0, 3 - (LENGTH(codeNo+1))) ||codeNo+1;
 
     
 END;


