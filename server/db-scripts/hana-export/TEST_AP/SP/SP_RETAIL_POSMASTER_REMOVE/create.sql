
CREATE PROCEDURE SP_Retail_PosMaster_REMOVE
 (in PosID nvarchar(50) )
AS
BEGIN
	declare udf int = 0;
	select ifnull((select count(*)
                    from TABLE_COLUMNS
                    
					where TABLE_NAME = 'ORDR' AND COLUMN_NAME = 'U_Posid'),0) into udf from DUMMY;
	if(udf >0)
	then
	BEGIN
			DECLARE checkExist int = 0;
			execute immediate 'select IFNULL(( SELECT Count(*) FROM ORDR WHERE "U_Posid"=:PosID),0) into checkExist from DUMMY';

			IF checkExist>0
			then
			BEGIN
			SELECT 'You cannot remove this POS; linked transactions exist' from DUMMY;
			return;
			END;
			end if;
	END;
	end if;
	DELETE FROM APZ_RPOS WHERE "PosID"  = :PosID;
END




