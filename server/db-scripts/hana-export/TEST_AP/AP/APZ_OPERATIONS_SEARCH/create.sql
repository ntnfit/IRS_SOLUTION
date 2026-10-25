
CREATE PROCEDURE APZ_Operations_Search(in OpeCode nvarchar(50) default '', in Status nvarchar(1) default '')
AS
BEGIN
	select * from APZ_OOMD a where ifnull(a."OpeCode",'') like '%' || :OpeCode || '%' and ifnull(a."Status",'') like '%' || :Status || '%';
	
END




