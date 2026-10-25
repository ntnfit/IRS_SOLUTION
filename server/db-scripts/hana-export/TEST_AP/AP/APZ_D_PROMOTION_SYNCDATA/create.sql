

CREATE  PROCEDURE APZ_D_Promotion_SyncData
AS
BEGIN
	SELECT * 
	FROM APZ_OPPR;
	SELECT * 
	FROM APZ_D_PMO3;
	SELECT A.*, B."Name" AS "Name" 
	FROM APZ_PPR2 A 
	    INNER JOIN APZ_OPCN B ON A."ConditionCode" = B."Code";
	SELECT A.*, B."Name" AS "Name" 
	FROM APZ_PPR1 A 
	    INNER JOIN APZ_OPBF B ON A."BenefitCode" = B."Code";
END




