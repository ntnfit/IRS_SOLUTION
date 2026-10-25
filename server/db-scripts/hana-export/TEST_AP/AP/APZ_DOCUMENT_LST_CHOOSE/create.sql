

CREATE PROCEDURE apz_document_lst_choose(in TableName nvarchar(10),   FromDate nvarchar(20) default null,  ToDate nvarchar(20) default null)
AS 
begin
	DECLARE SQLQuery NVARCHAR(50000)='';
	SQLQuery='SELECT "DocEntry","DocNum", "DocDate","CardName","Comments","DocDueDate","DocTotal" FROM '||TableName || ' WHERE 1=1';
	 IF :FromDate IS NOT NULL and :FromDate!=''
                        then 
                            SQLQuery = :SQLQuery || ' AND  "DocDate">='''|| cast(cast(:FromDate as date) as nvarchar) || '''';
                        END if;
    IF :ToDate IS NOT NULL  and :ToDate!=''
        then 
            SQLQuery = :SQLQuery || ' AND  "DocDate"<='''|| cast(cast(:ToDate as date) as nvarchar) || '''';
        END if;
	execute immediate :SQLQuery;
end

 



