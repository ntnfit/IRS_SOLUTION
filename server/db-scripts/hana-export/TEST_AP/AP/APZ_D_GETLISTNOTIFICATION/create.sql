

CREATE PROCEDURE APZ_D_GetListNotification
(
	IN userName NVARCHAR(50),	
	IN appName NVARCHAR(50),	
	IN dbName NVARCHAR(50),
	IN startIndex int DEFAULT 0,
	IN pageSize int DEFAULT 0
)
AS
BEGIN
	IF (:startIndex = 0) THEN 
		startIndex := (SELECT MAX(ID) FROM APZ_D_ELN1) + 1;
	END IF;

	SELECT  *
FROM    ( SELECT *,ROW_NUMBER() OVER ( ORDER BY ID DESC) AS RowNum FROM APZ_D_ELN1 WHERE 
"Method" = :appName  
and "DatabaseName" = :dbName 
and (comparesplit(:userName, "SendUser", ',') = 1) 
and "IsRemove" = 'N'
	and ID < :startIndex  
        ) AS RowConstrainedResult
WHERE   RowNum >= 1
    AND (:pageSize <=0 or (:pageSize > 0 AND RowNum <= :pageSize))
ORDER BY RowNum;
END	


