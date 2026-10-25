
CREATE PROCEDURE APZ_D_SalesOpportunity_GetStage (OpprId int default 0)
AS
Begin

	SELECT a.*, b."Descript", IFNULL(c."SlpName", '') AS "EmployeeName" 
	FROM OPR1 a 
	LEFT JOIN OOST b ON a."Step_Id" = b."StepId" 
	LEFT JOIN OSLP c ON c."SlpCode" = a."SlpCode" 
	WHERE a."OpprId" = :OpprId;
	
end




