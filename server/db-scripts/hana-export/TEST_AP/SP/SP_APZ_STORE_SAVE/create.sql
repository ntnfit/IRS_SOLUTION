
CREATE PROCEDURE SP_APZ_STORE_SAVE
AS
BEGIN
create local temporary table #temp_Store as (select top 0* from APZ_STORE);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #temp_Store select * from #APZ_STORE';
	UPSERT APZ_STORE ("StoreID", "Code", "BPLId", "Dscription","WhsCode","Address","Email","CardCode","SlpCode","ListNum","UserSign","UpdateDate","CreateDate","UserSign2","Locked","DataSource","LogInstanc","OSKeyBoardEnable")
	SELECT case when IFNULL(S."StoreID",0) = 0 then (SELECT IFNULL(MAX("StoreID"), 0) + 1 FROM APZ_STORE)
		else S."StoreID" end, 
	S."Code", S."BPLId", S."Dscription", S."WhsCode", S."Address", S."Email", S."CardCode", S."SlpCode", S."ListNum", S."UserSign", S."UpdateDate", S."CreateDate", S."UserSign2", S."Locked", S."DataSource", S."LogInstanc", S."OSKeyBoardEnable" FROM #temp_Store S;
END




