
CREATE PROCEDURE APZ_CostCenters_DeleteCostCenters(
PrcCode nvarchar(50) default '')
as
BEGIN
declare linked int = 0;
 PrcCode := LTRIM(RTRIM(:PrcCode));

IF (UPPER(LEFT(:PrcCode,7)) = 'CENTR_Z')
then
BEGIN
	Select N'Cannot remove general cost center' from DUMMY;
	return;
END;
end if;


 /* OCR1 */
 select IFNULL((Select top 1 1 From OCR1 Where "PrcCode" = :PrcCode),0) into linked from DUMMY;
IF linked>0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
end if;

 /* AOC1 */
 select IFNULL((Select top 1 1 From AOC1 Where "PrcCode" = :PrcCode),0) into linked from DUMMY;
IF linked >0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
end if;

 /* OHEM */
 select IFNULL((Select top 1 1 From OHEM Where "CostCenter" = :PrcCode),0) into linked from DUMMY;
IF linked >0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
end if;


 /* AHEM */
 select IFNULL((Select top 1 1 From AHEM Where "CostCenter" = :PrcCode),0) into linked from DUMMY;
IF linked >0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
end if;

 /* MDR1 */
 select IFNULL((Select top 1 1 From MDR1 Where "PrcCode" = :PrcCode),0) into linked from DUMMY;
IF linked >0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
END if;

 /* AMDR1 */
 select IFNULL((Select top 1 1 From AMDR1 Where "PrcCode" = :PrcCode),0) into linked from DUMMY;
IF linked >0
then
BEGIN
	Select N'Cannot remove cost centers linked to a loading factor' from DUMMY;
	return;
END;
END if;

DELETE from OPRC WHERE "PrcCode" = :PrcCode;
END




