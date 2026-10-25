
CREATE PROCEDURE APZ_SaveWorkCode
AS
BEGIN
	Create local temporary table #temp_OHWC as (select top 0 * from APZ_OHWC);
	Execute immediate 'insert into #temp_OHWC select * from #APZ_OHWC';
	Delete From  APZ_OHWC;
	
	--Trim Code before Insert
	Update #temp_OHWC Set "Code" = Trim(' ' From "Code");
	
	INSERT INTO APZ_OHWC SELECT * FROM #temp_OHWC;
END





