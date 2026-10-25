
CREATE PROCEDURE APZ_QuickBPMasterData_GetData(
	in lowLimit int,
	in upLimit int,
	in bpCode nvarchar(100) default '',
	in bpName nvarchar(100) default '',
	in bpType nvarchar(100) default '',
	in bpGroup nvarchar(100) default '',
	in currency nvarchar(100) default '')
AS
BEGIN
	SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY "CardCode") row_num, * FROM OCRD 
                       where "CardCode" like '%'|| :bpCode || '%' and
							((:bpName='' and ("CardName" is null or "CardName" like '%'|| :bpName || '%')) or (:bpName<>'' and ("CardName" like '%' || :bpName || '%'))) and
							((:bpType='' and ("CardType" is null or "CardType" like '%'|| :bpType || '%')) or (:bpType<>'' and ("CardType" like '%'|| :bpType || '%'))) and
							((:bpGroup='' and ("GroupCode" is null or "GroupCode" like '%' || :bpGroup || '%')) or (:bpGroup<>'' and ("GroupCode" like '%'|| :bpGroup || '%'))) and
							((:currency='' and ("Currency" is null or "Currency" like '%'|| :currency || '%')) or (:currency<>'' and ("Currency" like '%' || :currency || '%')))) a
                        WHERE row_num > :lowLimit AND row_num <=:upLimit;
                        
	SELECT COUNT(1) as "NumRecord" FROM (select * from OCRD where "CardCode" like '%' || :bpCode || '%' and
						((:bpName='' and ("CardName" is null or "CardName" like '%' || :bpName || '%')) or (:bpName<>'' and ("CardName" like '%' || :bpName || '%'))) and
						((:bpType='' and ("CardType" is null or "CardType" like '%' || :bpType || '%')) or (:bpType<>'' and ("CardType" like '%' || :bpType || '%'))) and
						((:bpGroup='' and ("GroupCode" is null or "GroupCode" like '%' || :bpGroup || '%')) or (:bpGroup<>'' and ("GroupCode" like '%' || :bpGroup || '%'))) and
						((:currency='' and ("Currency" is null or "Currency" like '%' || :currency || '%')) or (:currency<>'' and ("Currency" like '%' || :currency || '%')))) b;
END



