CREATE PROCEDURE APZ_Outsourcing_Search
(
	in DocNum int default 0,
	in Comments nvarchar(100) default '',
	in DocDate nvarchar(8) default ''
)
AS
BEGIN
	SELECT * FROM "@TBL_OUTS_DOCNUM" 
	WHERE (("Code" = :DocNum AND :DocNum <> 0) OR :DocNum = 0) 
		AND (("U_comments" LIKE '%' || :Comments || '%' AND Ltrim(rtrim(IFNULL(:Comments, ''))) <> '') OR Ltrim(rtrim(IFNULL(:Comments, ''))) = '') 
		AND (("U_docDate" = CAST(:DocDate AS timestamp) AND Ltrim(rtrim(IFNULL(:DocDate, ''))) <> '') OR Ltrim(rtrim(IFNULL(:DocDate, ''))) = '');
END

