
CREATE PROCEDURE APZ_BarcodeInventoryCounting_Delete(in docEntry int default -1)
AS
BEGIN

	declare checkCount int = 0;
	
	select IFNULL((select top 1 1 from APZ_OBIC where "DocEntry" = :docEntry),0) into checkCount from DUMMY;
	if :checkCount = 0
	THEN

		select 10011, 'Document does not exists' from DUMMY;
		return;
	
	END IF;

	select IFNULL((select top 1 1 from APZ_OBIC where "DocEntry" = :docEntry and "Status" != 'D'),0) into checkCount from DUMMY;
	if :checkCount = 1
	then

		select 10012, 'Can not delete document, Status is not Draft' from DUMMY;
		return;
	
	END IF;

	delete from APZ_BIC1 where "DocEntry" = :docEntry;
	delete from APZ_OBIC where "DocEntry" = :docEntry;
	select :docEntry, '' from DUMMY;
	
END;



