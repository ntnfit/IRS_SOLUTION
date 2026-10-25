
CREATE PROCEDURE APZ_ResourceWorkSchedule_SearchData(in code nvarchar(50) default '', in status char(1) default '', in getType int default 0)
AS
BEGIN

	declare getCode nvarchar(50) = '';

	---------------------------------chuẩn hóa input để loại bỏ các trường hợp cần kiểm tra-------------------------------------------
	if :getType is null THEN
		getType = 4;
	elseif :getType = 2
	THEN
		if ifnull(:code,'') = '' THEN
			getType = 3;
		END IF;
	elseif :getType = 1
	THEN
		if ifnull(:code,'') = '' THEN
			getType = 0;
		END IF;
	END IF;
	--------------------------------- ENd chuẩn hóa input ------------------------------------------------------------------

	if :getType = 4 THEN
		select * from APZ_ORWS where (ifnull(:code, '') = '' or "Code" = :code) and (ifnull(:status, '') = '' or "Status" = :status);
	elseif :getType = 3
	THEN

		select ifnull((select max("Code") from APZ_ORWS),'') into getCode from DUMMY;
		select * from APZ_ORWS where "Code" = :getCode;

	elseif :getType = 2
	THEN

		select IFNULL((select min("Code") from APZ_ORWS where "Code" > :code),'') into getCode from DUMMY;

		if :getCode = '' THEN
			select IFNULL((select min("Code") from APZ_ORWS),'') into getCode from DUMMY;
		END IF;

		select * from APZ_ORWS where "Code" = :getCode;

	elseif :getType = 1
	THEN

		select IFNULL((select Max("Code") from APZ_ORWS where "Code" < :code), '') into getCode from DUMMY;

		if :getCode = '' THEN
			select IFNULL((select Max("Code") from APZ_ORWS),'') into getCode from DUMMY;
		END IF;

		select * from APZ_ORWS where "Code" = :getCode;

	else

		select IFNULL((select min("Code") from APZ_ORWS), '') into getCode FROM DUMMY;
		select * from APZ_ORWS where "Code" = :getCode;

	END IF;
		
END



