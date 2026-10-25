
CREATE PROCEDURE APZ_ActivitySubjects_GetAll(in TypeName nvarchar(20) default '')
AS
BEGIN
if(:TypeName <> null) or (:TypeName <> '') or (:TypeName <> '0')
THEN
	select a."Code", ifnull (a."Name",'''') as "Name", IfNULL (a."DataSource",'N') as DataSource ,IfNULL (a."UserSign",1) as "UserSign" , IfNULL ("Type",-1) as Type 
				from OCLS a 
					LEFT JOIN OCLT b ON a."Type" = b."Code" Where a."Type" = :TypeName Order by a."Name" asc;
END iF;
END



