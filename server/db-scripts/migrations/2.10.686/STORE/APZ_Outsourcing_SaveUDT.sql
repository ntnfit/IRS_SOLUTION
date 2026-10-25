
CALL APZ_EXISTSPROC('APZ_Outsourcing_SaveUDT');

GO

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
END