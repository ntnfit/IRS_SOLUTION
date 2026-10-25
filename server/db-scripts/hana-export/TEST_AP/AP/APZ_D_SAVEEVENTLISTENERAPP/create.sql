

CREATE PROCEDURE Apz_D_SaveEventListenerApp

AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tblLocal AS (SELECT TOP 0 * FROM APZ_D_ELN1);
	EXEC 'DROP TABLE #tblLocal';
	EXEC 'CREATE LOCAL TEMPORARY TABLE #tblLocal AS (SELECT * FROM #APZ_D_ELN1)';

	MERGE INTO APZ_D_ELN1 AS T
    USING #tblLocal AS S
    ON T.ID = S.ID

	WHEN MATCHED THEN update set 
	T."Pending" = ifnull(nullif(S."Pending",''),T."Pending"),
	T."Success" = ifnull(nullif(S."Success",''),T."Success"),
	T."Retry" = ifnull(S."Retry",T."Retry"),
	T."UpdateDate" = ifnull(nullif(S."UpdateDate",''),current_date),
	T."Read" = ifnull(nullif(S."Read",''),T."Read"),
	T."IsRemove" = ifnull(nullif(S."IsRemove",''),T."IsRemove")
	
 	WHEN NOT MATCHED THEN  insert (
	"EventCode",
	"Title",
	"Message",
	"Image",
	"Data",
	"DataExtra",
	"SendUser",
	"SendAll",
	"Pending",
	"Success",
	"Retry",
	"Method",
	"CreateDate" ,
	"UpdateDate",
	"Read",
	"Note",
	"DatabaseName",
	"IsRemove" 
	) 
	values
	(
	S."EventCode",
	S."Title",
	S."Message",
	S."Image",
	S."Data",
	S."DataExtra",
	S."SendUser",
	S."SendAll",
	S."Pending",
	S."Success",
	S."Retry",
	S."Method",
	S."CreateDate",
	S."UpdateDate",
	S."Read",
	S."Note",
	S."DatabaseName",
	S."IsRemove"
	);
	
	select max(ID) from APZ_D_ELN1;
END	




