
Create procedure APZ_JobVacancy_GetData
(
in Status Char(1) default '',
in DocNo int default 0,
in FromDate timestamp default '',
in ToDate timestamp default ''
)
as
Begin
DECLARE MaxStart timestamp;
DECLARE MaxEnd timestamp;

SELECT ifnull( MAX("FromDate"),Current_date) into MaxStart FROM APZ_OHJV;
SELECT ifnull( MAX("ToDate"),current_date) into MaxEnd FROM APZ_OHJV;


SELECT *FROM APZ_OHJV
WHERE 
(ifnull(:status, '') = '' or "Status" = :status) and
(ifnull(:DocNo, 0) = 0 or "DocNum" = :DocNo) and
(ifnull(:FromDate, '') = '' or "FromDate" >= :FromDate) and
(ifnull(:ToDate,'') = '' or "FromDate" <= :ToDate);
End;




