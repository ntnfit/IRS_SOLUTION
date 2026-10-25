CREATE PROCEDURE APZ_Outsourcing_SaveUDT
(
	in docDate TimeStamp,
	in comments nvarchar(200) default '',
	in clearAct nvarchar(50) default '',
	in issueID nvarchar(10) default '',
	in receiptID nvarchar(10) default ''
)
AS
BEGIN
	DECLARE maxCode INT;
	
	Select IFNULL((Select Max(CAST("Code" As Int)) From "@TBL_OUTS_DOCNUM"),0) Into maxCode From Dummy;
	
	maxCode := :maxCode + 1;
	
	INSERT INTO "@TBL_OUTS_DOCNUM" ("Code","U_docDate", "U_comments", "U_clearAct", "U_issueID", "U_receiptID") 
	Select :maxCode,:docDate, :comments, :clearAct, :issueID, :receiptID From Dummy;
	
	update a
	set a."U_Type" = (select b."U_Type" from IGE1 b where b."DocEntry"  = Cast (:issueID as int))
	from  "@TBL_OUTS_DOCNUM" a
	where "Code" = :maxCode;
END

