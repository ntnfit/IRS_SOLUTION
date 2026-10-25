
CREATE PROCEDURE APZ_ProjectManager_GetTaskByWbsCode
(
    baseType nvarchar(20) DEFAULT '',
    baseEntry int DEFAULT -1,
    baseLine int DEFAULT -1,
    wbsCode nvarchar(50) DEFAULT ''
)
AS
BEGIN
    select *  from APZ_OCLG
     where
     (IFNULL(:baseType,'')='' or "BaseType"=:baseType) and
     (IFNULL(:baseEntry,0)=0 or "BaseEntry"=:baseEntry) and
    (IFNULL(:baseLine,0)=0 or "BaseLine"=:baseLine)
     and (IFNULL(:wbsCode,'')='' or "BaseCode"=:wbsCode);
END



