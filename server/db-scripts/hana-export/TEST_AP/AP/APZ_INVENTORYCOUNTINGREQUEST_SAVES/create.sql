
Create PROCEDURE APZ_InventoryCountingRequest_Saves
AS 
BEGIN

  	declare code int = 0;
  	create local temporary table #APZ_OCRQLocal as (select top 0 * from APZ_OCRQ);
	exec 'drop table #APZ_OCRQLocal;';
	exec 'create local temporary table #APZ_OCRQLocal as (select * from #APZ_OCRQ)';
  	
  	--UPDATE #APZ_OCRQLocal set "DocEntry"= (SELECT :code +  ROW_NUMBER() OVER( ORDER BY "DocEntry")  FROM #APZ_OCRQLocal) where ifnull("DocEntry" ,0) = 0;
	delete from  APZ_OCRQ;
	insert into APZ_OCRQ("DocEntry", "DocDate", "WhsCode", "Remarks", "Active") select ROW_NUMBER() OVER(ORDER BY "DocEntry"), "DocDate", "WhsCode", "Remarks", "Active" from #APZ_OCRQLocal;
	select 0, '' from dummy;
	
END;



